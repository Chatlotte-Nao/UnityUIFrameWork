using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class UIWindowEditor : EditorWindow
{
    private string scriptContent;
    private string filePath;
    private Vector2 scroll = new Vector2();
    /// <summary>
    /// 显示代码窗口
    /// </summary>
    public static void Showindow(string content, string filePath, Dictionary<string, string> insterDic = null)
    {
        UIWindowEditor window =
            (UIWindowEditor)GetWindowWithRect(typeof(UIWindowEditor), new Rect(100, 50, 800, 700), true, "Window生成界面");
        window.scriptContent = content;
        window.filePath = filePath;
        if (File.Exists(filePath) && insterDic != null)
        {
            string originScript = File.ReadAllText(filePath);
            foreach (var item in insterDic)
            {
                if (!originScript.Contains(item.Key))
                {
                    int index = window.GetInsertIndex(content);
                    window.scriptContent = originScript.Insert(index, item.Value + "\t\t");
                }
            }
        }
        window.Show();
    }

    private void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(600), GUILayout.Width(800));
        EditorGUILayout.TextArea(scriptContent);
        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextArea("脚本生成路径:" + filePath);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("生成脚本", GUILayout.Height(30)))
        {
            ButtonClick();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void ButtonClick()
    {
        //生成脚本文件
         if (File.Exists(filePath))
         {
             File.Delete(filePath);
         }
         StreamWriter writer = File.CreateText(filePath);
         writer.Write(scriptContent);
         writer.Close();
         AssetDatabase.Refresh();
         if (EditorUtility.DisplayDialog("自动化生成工具", "生成脚本成功！", "确定"))
         {
             Close();
         }
    }

    /// <summary>
    /// 获取插入代码的下标
    /// </summary>
    public int GetInsertIndex(string content)
    {
        //找到UI事件组件下面的第一个public 所在的位置进行
        Regex regex = new Regex("UI组件事件");
        Match match = regex.Match(content);

        Regex regex1 = new Regex("public");
        MatchCollection matchCollection = regex1.Matches(content);
        for (int i = 0; i < matchCollection.Count; i++)
        {
            if (matchCollection[i].Index > match.Index)
            {
                return matchCollection[i].Index;
            }
        }

        return -1;
    }
}
