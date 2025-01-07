using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SystemUIEditor : Editor
{
    //InitializeOnLoadMethod这个特性
    // 所标记的方法会在 Unity 编辑器加载时（比如打开项目、脚本重新编译、进入或退出 Play 模式）被自动调用。
    // 无需手动调用，也无需挂载到特定对象。
    [InitializeOnLoadMethod]
    private static void InitEditor()
    {
        //监听hierarchy发生改变的委托
        EditorApplication.hierarchyChanged += HandleTextOrImageRaycast;
        EditorApplication.hierarchyChanged += LoadWindowCamera;
    }

    private static void HandleTextOrImageRaycast()
    {
        GameObject obj = Selection.activeGameObject;
        if (obj != null)
        {
            if (obj.name.Contains("Text"))
            {
                Text text = obj.GetComponent<Text>();
                if (text != null)
                {
                    text.raycastTarget = false;
                }
            }
            else if (obj.name.Contains("Image"))
            {
                Image image = obj.GetComponent<Image>();
                if (image != null)
                {
                    image.raycastTarget = false;
                }
            }
        }
    }

    private static void LoadWindowCamera()
    {
        if (Selection.activeGameObject != null)
        {
            GameObject uiCameraObj = GameObject.Find("UICamera");
            if (uiCameraObj != null)
            {
                Camera camera = uiCameraObj.GetComponent<Camera>();
                if (Selection.activeGameObject.name.Contains("Window"))
                {
                    Canvas canvas = Selection.activeGameObject.GetComponent<Canvas>();
                    if (canvas != null)
                    {
                        canvas.worldCamera = camera;
                    }
                }
            }
        }
    }
}
