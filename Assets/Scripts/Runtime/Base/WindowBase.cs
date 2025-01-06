using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WindowBase : WindowBehaviour
{
    private List<Button> mAllButtonList = new List<Button>();
    private List<Toggle> mToggleList = new List<Toggle>();
    private List<InputField> mInputList = new List<InputField>();

    private CanvasGroup mUIMask;
    protected Transform mUIContent;
    protected bool mDisableAnim = false;
    /// <summary>
    /// 初始化基类组件
    /// </summary>
    private void InitializeBaseComponent()
    {
        mUIMask = transform.Find("UIMask").GetComponent<CanvasGroup>();
        mUIContent = transform.Find("UIContent");
    }
    
    #region 生命周期
    public override void OnAwake()
    {
        base.OnAwake();
        InitializeBaseComponent();
    }

    public override void OnShow()
    {
        base.OnShow();
        ShowAnimation();
    }
        
    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        RemoveAllButtonListener();
        RemoveAllToggleListener();
        RemoveAllInputListener();
        mAllButtonList.Clear();
        
    }
    #endregion

    #region 动画管理

    public void ShowAnimation()
    {
        //基础弹窗不需要动画
        if (Canvas.sortingOrder > 90 && mDisableAnim==false)
        {
            mUIContent.localScale = Vector3.one * 0.8f;
            mUIContent.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }
    }

    public void HideAnimation()
    {
        mUIContent.DOScale(Vector3.one * 1.1f, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
        {       
            UIModule.Instance.HideWindow(Name);
        });
    }

    #endregion

    public void HideWindow()
    {
        //UIModule.Instance.HideWindow(Name);
        HideAnimation();
    }
    
    public override void SetVisible(bool isVisble)
    {
        gameObject.SetActive(isVisble);//TempCode
        Visible = isVisble;
    }

    public void SetMaskVisible(bool isVisble)
    {
        if (UISetting.Instance.SINGMASK_SYSTEM)
        {
            mUIMask.alpha = isVisble ? 1 : 0;
        }
    }

    #region 事件管理

    public void AddButtonClickListener(Button btn, UnityAction action)
    {
        if (btn != null)
        {
            if (!mAllButtonList.Contains(btn))
            {
                mAllButtonList.Add(btn);
            }
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(action);
        }
    }
    
    public void AddToggleClickListener(Toggle toggle, UnityAction<bool,Toggle> action)
    {
        if (toggle != null)
        {
            if (!mToggleList.Contains(toggle))
            {
                mToggleList.Add(toggle);
            }
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener((isOn =>
            {
                action?.Invoke(isOn,toggle);
            } ));
        }
    }
    
    public void AddInputFieldListener(InputField input, UnityAction<string> onChangeAction,UnityAction<string> endAction)
    {
        if (input != null)
        {
            if (!mInputList.Contains(input))
            {
                mInputList.Add(input);    
            }
            input.onValueChanged.RemoveAllListeners();
            input.onEndEdit.RemoveAllListeners();
            input.onValueChanged.AddListener(onChangeAction);
            input.onEndEdit.AddListener(endAction);
        }
    }

    public void RemoveAllButtonListener()
    {
        foreach (var item in mAllButtonList)
        {
            item.onClick.RemoveAllListeners();
        }
    }
    
    public void RemoveAllToggleListener()
    {
        foreach (var item in mToggleList)
        {
            item.onValueChanged.RemoveAllListeners();
        }
    }
    
    public void RemoveAllInputListener()
    {
        foreach (var item in mInputList)
        {
            item.onValueChanged.RemoveAllListeners();
            item.onEndEdit.RemoveAllListeners();
        }
    }

    #endregion

}
