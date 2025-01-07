using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WindowBase : WindowBehaviour
{
    private List<Button> _allButtonList = new List<Button>();
    private List<Toggle> _toggleList = new List<Toggle>();
    private List<InputField> _inputFieldList = new List<InputField>();

    private CanvasGroup _uiMask;
    private CanvasGroup _canvasGroup;
    protected Transform _uiContent;
    protected bool IsDisableAnim = false;
    /// <summary>
    /// 初始化基类组件
    /// </summary>
    private void InitializeBaseComponent()
    {
        _uiMask = Transform.Find("UIMask").GetComponent<CanvasGroup>();
        _uiContent = Transform.Find("UIContent");
        _canvasGroup=Transform.GetComponent<CanvasGroup>();
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
        _allButtonList.Clear();
        
    }
    #endregion

    #region 动画管理

    public void ShowAnimation()
    {
        //基础弹窗不需要动画
        if (Canvas.sortingOrder > 90 && IsDisableAnim==false)
        {
            _uiContent.localScale = Vector3.one * 0.8f;
            _uiContent.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }
    }

    public void HideAnimation()
    {
        _uiContent.DOScale(Vector3.one * 1.1f, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
        {       
            UIManager.Instance.HideWindow(Name);
        });
    }

    #endregion

    public void HideWindow()
    {
        //UIModule.Instance.HideWindow(Name);
        HideAnimation();
    }
    
    public override void SetVisible(bool isVisible)
    {
        _canvasGroup.alpha = isVisible ? 1 : 0;
        _canvasGroup.blocksRaycasts = isVisible;
        Visible = isVisible;
    }

    public void SetMaskVisible(bool isVisble)
    {
        if (UISetting.Instance.SINGMASK_SYSTEM)
        {
            _uiMask.alpha = isVisble ? 1 : 0;
        }
    }

    #region 事件管理

    public void AddButtonClickListener(Button btn, UnityAction action)
    {
        if (btn != null)
        {
            if (!_allButtonList.Contains(btn))
            {
                _allButtonList.Add(btn);
            }
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(action);
        }
    }
    
    public void AddToggleClickListener(Toggle toggle, UnityAction<bool,Toggle> action)
    {
        if (toggle != null)
        {
            if (!_toggleList.Contains(toggle))
            {
                _toggleList.Add(toggle);
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
            if (!_inputFieldList.Contains(input))
            {
                _inputFieldList.Add(input);    
            }
            input.onValueChanged.RemoveAllListeners();
            input.onEndEdit.RemoveAllListeners();
            input.onValueChanged.AddListener(onChangeAction);
            input.onEndEdit.AddListener(endAction);
        }
    }

    public void RemoveAllButtonListener()
    {
        foreach (var item in _allButtonList)
        {
            item.onClick.RemoveAllListeners();
        }
    }
    
    public void RemoveAllToggleListener()
    {
        foreach (var item in _toggleList)
        {
            item.onValueChanged.RemoveAllListeners();
        }
    }
    
    public void RemoveAllInputListener()
    {
        foreach (var item in _inputFieldList)
        {
            item.onValueChanged.RemoveAllListeners();
            item.onEndEdit.RemoveAllListeners();
        }
    }

    #endregion

}
