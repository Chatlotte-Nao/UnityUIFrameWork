using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class UIManager
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIManager();
            }
            return _instance;
        }
    }

    private Camera _uiCamera;
    private Transform _uiRoot;
    private Dictionary<string, WindowBase> _allWindowDic = new Dictionary<string, WindowBase>();
    private List<WindowBase> _allWindowList = new List<WindowBase>();
    private List<WindowBase> _visibleWindowList = new List<WindowBase>();//所有可见窗口列表
    private WindowConfig _windowConfig;

    private Queue<WindowBase> _windowStack = new Queue<WindowBase>();//队列，可以用来管理弹窗的循环弹出
    private bool _startPopStackWindowStatus = false; //开始弹出堆栈的标志，可以用来处理多种情况，比如正在出栈中有其他界面弹出，可以直接放到栈内进行弹出等

    #region 智能显隐
    private bool mSmartShowHide=true;
    //智能显隐开关（可根据情况选择开启或关闭）
    //智能显隐：主要用来优化窗口叠加时被遮挡的窗口参与渲染计算，导致帧率降低的问题。
    //显隐规则：由程序设定某个窗口是否为全屏窗口。(全屏窗口设定方式：在窗口的OnAwake接口中设定该窗口是否为全屏窗口如 FullScreenWindow=true)
    //1.智能隐藏:当FullScreenWindow=true的全屏窗口打开时，框架会自动通过伪隐藏的方式隐藏所有被当前全屏窗口遮挡住的窗口，避免这些看不到的窗口参与渲染运算，
    //从而提高性能。
    //2.智能显示：当FullScreenWindow=true的全屏窗口关闭时，框架会自动找到上一个伪隐藏的窗口把其设置为可见状态，若上一个窗口为非全屏窗口，框架则会找上上个窗口进行显示，
    //以此类推进行循环，直到找到全屏窗口则停止智能显示流程。
    //注意：通过智能显隐进行伪隐藏的窗口在逻辑上仍属于显示中的窗口，可以通过GetWindow获取到该窗口。但是在表现上该窗口为不可见窗口，故称之为伪隐藏。
    //智能显隐逻辑与（打开当前窗口时隐藏其他所有窗口相似）但本质上有非常大的区别，
    //1.通过智能显隐设置为不可见的窗口属于伪隐藏窗口，在逻辑上属于显示中的窗口。
    //2.通过智能显隐设置为不可见的窗口可以通过关闭当前窗口，自动恢复当前窗口之前的窗口的显示。
    //3.通过智能显隐设置为不可见的窗口不会触发UGUI重绘、不会参与渲染计算、不会影响帧率。
    //4.程序只需要通过FullScreenWindow=true配置那些窗口为全屏窗口即可，智能显隐的所有逻辑均有框架自动维护处理。
    #endregion
    
    public void Initialize()
    {
        _uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
        _uiRoot = GameObject.Find("UIRoot").transform;
        _windowConfig = Resources.Load<WindowConfig>("WindowConfig");
#if UNITY_EDITOR
        _windowConfig.GeneratorWindowConfig();
#endif
    }

    #region 窗口管理
    /// <summary>
    /// 预加载界面 
    /// </summary>
    public void PreLoadWindow<T>() where T : WindowBase, new()
    {
        Type type = typeof(T);
        string windowName = type.Name;
        T windowBase = new T();
        GameObject newWindow = InstantiateWindow(windowName);
        if (newWindow != null)
        {
            windowBase.GameObject = newWindow;
            windowBase.Transform = newWindow.transform;
            windowBase.Canvas = newWindow.GetComponent<Canvas>();
            windowBase.Canvas.worldCamera = _uiCamera;
            windowBase.Transform.SetAsLastSibling();
            windowBase.Name = newWindow.name;
            windowBase.OnAwake();
            windowBase.SetVisible(false);
            RectTransform rectTrans = newWindow.GetComponent<RectTransform>();
            rectTrans.anchorMax=Vector2.one;
            rectTrans.offsetMax = Vector2.zero;
            rectTrans.offsetMin=Vector2.zero;
            _allWindowDic.Add(windowName,windowBase);
            _allWindowList.Add(windowBase);
        }
        Debug.Log("预加载窗口 窗口名字:"+windowName);
    }

    public T OpenWindow<T>() where T: WindowBase,new ()
    {
        Type type = typeof(T);
        string windowName = type.Name;
        WindowBase window = GetWindow(windowName);
        if (window != null)
        { 
          return ShowWindow(windowName) as T;
        }

        T t = new T();
        return InitializeWindow(t, windowName) as T;
    }
    /// <summary>
    /// 用于打开界面
    /// </summary>
    public WindowBase OpenWindow(WindowBase window)
    {
        Type type = window.GetType();
        string windowName = type.Name;
        WindowBase wnd = GetWindow(windowName);
        if (wnd != null)
        { 
            return ShowWindow(windowName);
        }
        return InitializeWindow(window, windowName);
    }
    //初始化界面
    private WindowBase InitializeWindow(WindowBase windowBase, string windowName)
    {
        GameObject newWindow = InstantiateWindow(windowName);
        if (newWindow != null)
        {
            windowBase.GameObject = newWindow;
            windowBase.Transform = newWindow.transform;
            windowBase.Canvas = newWindow.GetComponent<Canvas>();
            windowBase.Canvas.worldCamera = _uiCamera;
            windowBase.Transform.SetAsLastSibling();
            windowBase.Name = newWindow.name;
            windowBase.OnAwake();
            windowBase.SetVisible(true);
            windowBase.OnShow();
            RectTransform rectTrans = newWindow.GetComponent<RectTransform>();
            rectTrans.anchorMax=Vector2.one;
            rectTrans.offsetMax = Vector2.zero;
            rectTrans.offsetMin=Vector2.zero;
            _allWindowDic.Add(windowName,windowBase);
            _allWindowList.Add(windowBase);
            _visibleWindowList.Add(windowBase);
            SetWindowMaskVisible();
            ShowWindowAndModifyAllWindowCanvasGroup(windowBase, 0);
            return windowBase;
        }
        
        Debug.LogError("没有加载到对应窗口 窗口名字:"+windowName);
        return null;
    }
    //显示界面(类似于把gameObject SetActive(true)这样)
    private WindowBase ShowWindow(string windowName)
    {
        if (_allWindowDic.ContainsKey(windowName))
        {
            WindowBase window = null;
            window = _allWindowDic[windowName];
            if (window.GameObject != null && window.Visible == false)
            {
                _visibleWindowList.Add(window);
                window.Transform.SetAsLastSibling();
                window.SetVisible(true);
                SetWindowMaskVisible();
                ShowWindowAndModifyAllWindowCanvasGroup(window, 0);
                window.OnShow();
            }
            return window;
        }
        else
        {
            Debug.LogError(windowName+"窗口不存在，请调用PopUpWindow进行弹出");
        }
        return null;
    }

    private WindowBase GetWindow(string windowName)
    {
        if (_allWindowDic.ContainsKey(windowName))
        {
            return _allWindowDic[windowName];
        }

        return null;
    }
    
    public T GetWindow<T>() where T : WindowBase
    {
        Type type = typeof(T);
        foreach (var item in _visibleWindowList)
        {
            if (item.Name == type.Name)
            {
                return (T)item;
            }
        }
        Debug.LogError("该窗口没有获取到:"+type.Name);
        return null;
    }
    /// <summary>
    /// 隐藏界面(类似于把gameObject SetActive(false)这样)
    /// </summary>
    public void HideWindow(string windowName)
    {
        WindowBase window = GetWindow(windowName);
        HideWindow(window);
    }
    
    public void HideWindow<T>() where T: WindowBase
    {
        HideWindow(typeof(T).Name);
    }

    private void HideWindow(WindowBase window)
    {
        if (window != null && window.Visible)
        {
            _visibleWindowList.Remove(window);
            window.SetVisible(false);
            SetWindowMaskVisible();
            window.OnHide();
        }
        //在出栈的情况下，上一个界面隐藏时，自动打开栈种的下一个界面
        PopNextStackWindow(window);
    }

    private void DestroyWindow(string windowName)
    {
        WindowBase window = GetWindow(windowName);
        DestroyWindow(window);
    }

    public void DestroyWindow<T>() where T : WindowBase
    {
        DestroyWindow(typeof(T).Name);
    }
    //销毁界面
    private void DestroyWindow(WindowBase window)
    {
        if (window != null)
        {
            if (_allWindowDic.ContainsKey(window.Name))
            {
                _allWindowDic.Remove(window.Name);
                _allWindowList.Remove(window);
                _visibleWindowList.Remove(window);
            }
            window.SetVisible(false);
            SetWindowMaskVisible();
            window.OnHide();
            window.OnDestroy();
            Object.Destroy(window.GameObject);
            //在出栈的情况下，上一个界面隐藏时，自动打开栈中的下一个界面
            PopNextStackWindow(window);
        }
    }
    /// <summary>
    /// 销毁所有界面
    /// </summary>
    public void DestroyAllWindow(List<string> filterlist = null)
    {
        for (int i = _allWindowList.Count-1; i >=0; i--)
        {
            WindowBase window = _allWindowList[i];
            if (window == null || (filterlist != null && filterlist.Contains(window.Name)))
            {
                continue;
            }
            DestroyWindow(window.Name);
        }
        Resources.UnloadUnusedAssets();
    }

    private void SetWindowMaskVisible()
    {
        if (UISetting.Instance.SINGMASK_SYSTEM)
        {
            WindowBase maxOrderWindowBase = null;//最大渲染层级的窗口
            int maxOrder = 0;//最大渲染层级
            int maxIndex = 0;//最大排序下标，在相同父节点下的位置下标
            //关闭所有窗口的Mask 设置为不可见
            //从所有可见窗口中找到一个层级最大的窗口，把Mask设置为可见
            for (int i = 0; i < _visibleWindowList.Count; i++)
            {
                WindowBase window = _visibleWindowList[i];
                if (window != null && window.GameObject != null)
                {
                    int windowSortingOrder = window.Canvas.sortingOrder;
                    window.SetMaskVisible(false);
                    if (maxOrderWindowBase == null)
                    {
                        maxOrderWindowBase = window;
                        maxOrder = windowSortingOrder;
                        maxIndex = window.Transform.GetSiblingIndex();
                    }
                    else
                    {
                        //找到最大渲染层级的窗口，拿到它
                        if (maxOrder < windowSortingOrder)
                        {
                            maxOrderWindowBase = window;
                            maxOrder = windowSortingOrder;
                        }
                        else if (maxOrder == windowSortingOrder && maxIndex<window.Transform.GetSiblingIndex())
                        {
                            maxOrderWindowBase = window;
                            maxIndex = window.Transform.GetSiblingIndex();
                        }
                    }
                }
            }

            if (maxOrderWindowBase != null)
            {
                maxOrderWindowBase.SetMaskVisible(true );
            }
        }
    }
    //实例化界面预制体
    private GameObject InstantiateWindow(string windowName)
    {
        GameObject window = Object.Instantiate(Resources.Load<GameObject>(_windowConfig.GetWindowPath(windowName)), _uiRoot, true);
        window.transform.localScale=Vector3.one;
        window.transform.localPosition=Vector3.zero;
        window.transform.rotation=Quaternion.identity;
        window.name = windowName;
        return window;
    }
    #endregion

    #region 堆栈系统
    
    /// <summary>
    /// 进栈一个界面
    /// </summary>
    public void PushWindowToStack<T>(Action<WindowBase> popCallBack=null) where T : WindowBase, new()
    {
        T windowBase = new T();
        windowBase.PopStackListener = popCallBack;
        _windowStack.Enqueue(windowBase);
    }
    /// <summary>
    /// 弹出堆栈中第一个弹窗
    /// </summary>
    public void StartPopFirstStackWindow()
    {
        if (_startPopStackWindowStatus) return;
        _startPopStackWindowStatus = true;//已经开始进行堆栈弹出的流程，
        PopStackWindow();
    }
    /// <summary>
    /// 压入并且弹出堆栈弹窗
    /// </summary>
    public void PushAndPopStackWindow<T>(Action<WindowBase> popCallBack = null) where T : WindowBase, new()
    {
        PushWindowToStack<T>(popCallBack);
        StartPopFirstStackWindow();
    }
    
    /// <summary>
    /// 弹出堆栈弹窗
    /// </summary>
    public bool PopStackWindow()
    {
        if (_windowStack.Count>0)
        {
            WindowBase window = _windowStack.Dequeue();
            WindowBase popWindow= OpenWindow(window);
            popWindow.PopStackListener = window.PopStackListener;
            popWindow.IsPopStackWindow = true;
            popWindow.PopStackListener?.Invoke(popWindow);
            popWindow.PopStackListener = null;
            return true;
        }
        _startPopStackWindowStatus = false;
        return false;
    }

    public void ClearStackWindow()
    {
        _windowStack.Clear();
    }
    
    /// <summary>
    /// 弹出堆栈中的下一个窗口
    /// </summary>
    private void PopNextStackWindow(WindowBase windowBase)
    {
        if (windowBase != null && _startPopStackWindowStatus && windowBase.IsPopStackWindow)
        {
            windowBase.IsPopStackWindow = false;
            PopStackWindow();
        }
    }
    
    #endregion

    #region 智能显隐
    private void ShowWindowAndModifyAllWindowCanvasGroup(WindowBase window, int value)
    {
        if (!mSmartShowHide)
        {
            return;
        }

        //if (WorldManager.IsHallWorld && window.FullScreenWindow) 可以以此种方式决定智能显隐开启场景
        if (window.FullScreenWindow)
        {
            try
            {
                //当显示的弹窗是大厅是，不对其他弹窗进行伪隐藏，
                if (string.Equals(window.Name, "HallWindow"))
                {
                    return;
                }

                if (_visibleWindowList.Count > 1)
                {
                    //处理先弹弹窗 后关弹窗的情况
                    WindowBase curShowBase = _visibleWindowList[_visibleWindowList.Count - 2];
                    if (!curShowBase.FullScreenWindow && window.Canvas.sortingOrder < curShowBase.Canvas.sortingOrder)
                    {
                        return;
                    }
                }

                for (int i = _visibleWindowList.Count - 1; i >= 0; i--)
                {
                    WindowBase item = _visibleWindowList[i];
                    if (item.Name != window.Name)
                    {
                        item.PseudoHidden(value);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error:" + ex);
            }
        }
    }

    private void HideWindowAndModifyAllWindowCanvasGroup(WindowBase window, int value)
    {
        if (!mSmartShowHide)
        {
            return;
        }

        //if (WorldManager.IsHallWorld && window.FullScreenWindow) 可以以此种方式决定智能显隐开启场景
        if (window.FullScreenWindow)
        {
            for (int i = _visibleWindowList.Count - 1; i >= 0; i--)
            {
                if (i >= 0 && _visibleWindowList[i] != window)
                {
                    _visibleWindowList[i].PseudoHidden(1);
                    //找到上一个窗口，如果是全屏窗口，将其设置可见，终止循转。否则循环至最终
                    if (_visibleWindowList[i].FullScreenWindow)
                    {
                        break;
                    }
                }
            }
        }
    }

    #endregion
    

}

