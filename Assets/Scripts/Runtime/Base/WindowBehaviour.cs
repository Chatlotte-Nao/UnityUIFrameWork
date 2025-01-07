using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowBehaviour
{
    /// <summary>
    /// Window根节点自身对应的GameObject
    /// </summary>
    public GameObject GameObject { get; set; }
    /// <summary>
    /// Window根节点自身对应的Transform
    /// </summary>
    public Transform Transform { get; set; }
    /// <summary>
    /// Window根节点自身对应的Canvas
    /// </summary>
    public Canvas Canvas { get; set; }
    /// <summary>
    /// Window名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Window当前的显隐状态
    /// </summary>
    public bool Visible { get; set; }
    /// <summary>
    /// 是否是通过堆栈系统弹出的界面
    /// </summary>
    public bool IsPopStack { get; set; }
    /// <summary>
    /// PopStack监听器
    /// </summary>
    public Action<WindowBase> PopStackListener { get; set; }
    //下面的方法都同Unity生命周期一样执行规则
    public virtual void OnAwake(){}
    
    public virtual void OnShow(){}
    
    public virtual void OnUpdate(){}
    
    public virtual void OnHide() {}
    
    public virtual void OnDestroy(){}
    /// <summary>
    /// 设置显隐
    /// </summary>
    public virtual void SetVisible(bool isVisible){}
    
    
}
