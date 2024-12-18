using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginWindow : WindowBase
{
    public override void OnAwake()
    {
        base.OnAwake();
        Debug.Log("LoginWindow OnAwake");
    }

    public override void OnShow()
    {
        base.OnShow();
        Debug.Log("LoginWindow OnShow");
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Debug.Log("LoginWindow Destroy");
    }
    
    public void Test()
    {
        Debug.Log("micro test");
    }
}
