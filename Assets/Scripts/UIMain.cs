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
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("UserinfoWindow");
            UIManager.Instance.OpenWindow<UserInfoWIndow>();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("SettingWindow");
            UIManager.Instance.OpenWindow<SettingWIndow>();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("SettingWindow");
            UIManager.Instance.OpenWindow<HallWindow>();
        }
    }
}
