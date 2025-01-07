using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SystemUIEditor : Editor
{
    //Allow an editor class method to be initialized when Unity loads without action from the user.
    //InitializeOnLoadMethod特性允许unity在运行时调用这个方法
    [InitializeOnLoadMethod]
    private static void InitEditor()
    {
        //监听hierarchy发生改变的委托
        EditorApplication.hierarchyChanged += HandleTextOrImageRaycast;
        EditorApplication.hierarchyChanged += HandleTextOrImageRaycast;
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
        
    }
}
