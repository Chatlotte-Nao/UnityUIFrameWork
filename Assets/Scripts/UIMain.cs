using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("UserinfoWindow");
            UIModule.Instance.PushAndPopStackWindow<UserInfoWIndow>();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("SettingWindow");
            UIModule.Instance.PushAndPopStackWindow<SettingWIndow>();
        }
    }
}
