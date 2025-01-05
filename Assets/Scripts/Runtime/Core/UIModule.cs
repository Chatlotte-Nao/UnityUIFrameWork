using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class UIModule
{
    private static UIModule _instance;
    public static UIModule Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIModule();
            }
            return _instance;
        }
    }

    private Camera mUICamera;
    private Transform mUIRoot;
    private Dictionary<string, WindowBase> mAllWindowDic = new Dictionary<string, WindowBase>();
    private List<WindowBase> mAllWindowList = new List<WindowBase>();
    private List<WindowBase> mVisibleWindowList = new List<WindowBase>();//所有可见窗口列表
    private WindowConfig mWindowConfig;

    private Queue<WindowBase> mWindowStack = new Queue<WindowBase>();//队列，可以用来管理弹窗的循环弹出
    private bool mStartPopStackWndStatus = false; //开始弹出堆栈的标志，可以用来处理多种情况，比如正在出栈中有其他界面弹出，可以直接放到栈内进行弹出等

    public void Initialize()
    {
        mUICamera = GameObject.Find("UICamera").GetComponent<Camera>();
        mUIRoot = GameObject.Find("UIRoot").transform;
        mWindowConfig = Resources.Load<WindowConfig>("WindowConfig");
#if UNITY_EDITOR
        mWindowConfig.GeneratorWindowConfig();
#endif
    }

    #region 窗口管理
    public T PopUpWindow<T>() where T: WindowBase,new ()
    {
        Type type = typeof(T);
        string windowName = type.Name;
        WindowBase window = GetWindow(windowName);
        if (window != null)
        { 
          return  ShowWindow(windowName) as T;
        }

        T t = new T();
        return InitializeWindow(t, windowName) as T;
    }
    
    public WindowBase PopUpWindow(WindowBase window)
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

    private WindowBase InitializeWindow(WindowBase windowBase, string windowName)
    {
        GameObject newWindow = TempLoadWindow(windowName);
        if (newWindow != null)
        {
            windowBase.gameObject = newWindow;
            windowBase.transform = newWindow.transform;
            windowBase.Canvas = newWindow.GetComponent<Canvas>();
            windowBase.Canvas.worldCamera = mUICamera;
            windowBase.transform.SetAsLastSibling();
            windowBase.Name = newWindow.name;
            windowBase.OnAwake();
            windowBase.SetVisible(true);
            windowBase.OnShow();
            RectTransform rectTrans = newWindow.GetComponent<RectTransform>();
            rectTrans.anchorMax=Vector2.one;
            rectTrans.offsetMax = Vector2.zero;
            rectTrans.offsetMin=Vector2.zero;
            mAllWindowDic.Add(windowName,windowBase);
            mAllWindowList.Add(windowBase);
            mVisibleWindowList.Add(windowBase);
            SetWindowMaskVisible();
            return windowBase;
        }
        
        Debug.LogError("没有加载到对应窗口 窗口名字:"+windowName);
        return null;
    }

    private WindowBase ShowWindow(string windowName)
    {
        if (mAllWindowDic.ContainsKey(windowName))
        {
            WindowBase window = null;
            window = mAllWindowDic[windowName];
            if (window.gameObject != null && window.Visible == false)
            {
                mVisibleWindowList.Add(window);
                window.transform.SetAsLastSibling();
                window.SetVisible(true);
                SetWindowMaskVisible();
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
        if (mAllWindowDic.ContainsKey(windowName))
        {
            return mAllWindowDic[windowName];
        }

        return null;
    }

    public T GetWindow<T>() where T : WindowBase
    {
        Type type = typeof(T);
        foreach (var item in mVisibleWindowList)
        {
            if (item.Name == type.Name)
            {
                return (T)item;
            }
        }
        Debug.LogError("该窗口没有获取到:"+type.Name);
        return null;
    }

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
            mVisibleWindowList.Remove(window);
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

    private void DestroyWindow(WindowBase window)
    {
        if (window != null)
        {
            if (mAllWindowDic.ContainsKey(window.Name))
            {
                mAllWindowDic.Remove(window.Name);
                mAllWindowList.Remove(window);
                mVisibleWindowList.Remove(window);
            }
            window.SetVisible(false);
            SetWindowMaskVisible();
            window.OnHide();
            window.OnDestroy();
            Object.Destroy(window.gameObject);
            //在出栈的情况下，上一个界面隐藏时，自动打开栈种的下一个界面
            PopNextStackWindow(window);
        }
    }

    public void DestroyAllWindow(List<string> filterlist = null)
    {
        for (int i = mAllWindowList.Count-1; i >=0; i--)
        {
            WindowBase window = mAllWindowList[i];
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
            for (int i = 0; i < mVisibleWindowList.Count; i++)
            {
                WindowBase window = mVisibleWindowList[i];
                if (window != null && window.gameObject != null)
                {
                    int windowSortingOrder = window.Canvas.sortingOrder;
                    window.SetMaskVisible(false);
                    if (maxOrderWindowBase == null)
                    {
                        maxOrderWindowBase = window;
                        maxOrder = windowSortingOrder;
                        maxIndex = window.transform.GetSiblingIndex();
                    }
                    else
                    {
                        //找到最大渲染层级的窗口，拿到它
                        if (maxOrder < windowSortingOrder)
                        {
                            maxOrderWindowBase = window;
                            maxOrder = windowSortingOrder;
                        }
                        else if (maxOrder == windowSortingOrder && maxIndex<window.transform.GetSiblingIndex())
                        {
                            maxOrderWindowBase = window;
                            maxIndex = window.transform.GetSiblingIndex();
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
    
    public GameObject TempLoadWindow(string windowName)
    {
        GameObject window = Object.Instantiate(Resources.Load<GameObject>(mWindowConfig.GetWindowPath(windowName)), mUIRoot, true);
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
        mWindowStack.Enqueue(windowBase);
    }
    /// <summary>
    /// 弹出堆栈中第一个弹窗
    /// </summary>
    public void StartPopFirstStackWindow()
    {
        if (mStartPopStackWndStatus) return;
        mStartPopStackWndStatus = true;//已经开始进行堆栈弹出的流程，
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
        if (mWindowStack.Count>0)
        {
            WindowBase window = mWindowStack.Dequeue();
            WindowBase popWindow= PopUpWindow(window);
            popWindow.PopStackListener = window.PopStackListener;
            popWindow.PopStack = true;
            popWindow.PopStackListener?.Invoke(popWindow);
            popWindow.PopStackListener = null;
            return true;
        }
        else
        {
            mStartPopStackWndStatus = false;
            return false;
        }
    }

    public void ClearStackWindow()
    {
        mWindowStack.Clear();
    }
    
    /// <summary>
    /// 弹出堆栈中的下一个窗口
    /// </summary>
    private void PopNextStackWindow(WindowBase windowBase)
    {
        if (windowBase != null && mStartPopStackWndStatus && windowBase.PopStack)
        {
            windowBase.PopStack = false;
            PopStackWindow();
        }
    }
    
    #endregion
}

