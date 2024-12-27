using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMain : MonoBehaviour
{
    
    void Start()
    {

        UIModule.Instance.Initialize();
        UIModule.Instance.PopUpWindow<LoginWindow>();
    }

    
    void Update()
    {
        
    }
}
