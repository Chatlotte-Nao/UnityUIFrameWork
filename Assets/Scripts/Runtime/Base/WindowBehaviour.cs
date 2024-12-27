using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowBehaviour
{
    public GameObject gameObject { get; set; }
    
    public Transform transform { get; set; }
    
    public Canvas Canvas { get; set; }
    
    public string Name { get; set; }
    
    public bool Visible { get; set; }

    public virtual void OnAwake(){}
    
    public virtual void OnShow(){}
    
    public virtual void OnUpdate(){}
    
    public virtual void OnHide() {}
    
    public virtual void OnDestroy(){}
    
    public virtual void SetVisible(bool isVisble){}
    
    
}