using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIMain : MonoBehaviour
{
    
    void Start()
    {

        UIManager.Instance.Initialize();
        UIManager.Instance.OpenWindow<LoginWindow>();
        UIManager.Instance.PreLoadWindow<UserInfoWIndow>();
        UIManager.Instance.PreLoadWindow<SettingWIndow>();
        UIManager.Instance.PreLoadWindow<ChatWIndow>();
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("UserinfoWindow");
            UIManager.Instance.PushAndPopStackWindow<UserInfoWIndow>();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("SettingWindow");
            UIManager.Instance.PushAndPopStackWindow<SettingWIndow>();
        }
    }
}
