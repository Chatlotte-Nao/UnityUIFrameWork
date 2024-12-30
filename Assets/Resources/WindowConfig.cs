using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
[CreateAssetMenu(fileName = "WindowConfig",menuName = "WindowConfig",order = 0)]
public class WindowConfig : ScriptableObject
{
   private string[] _windowRootArr = new string[] { "Game", "Hall", "Window" };
   public List<WindowData> windowDatList = new List<WindowData>();

   public void GeneratorWindowConfig()
   {
      //预制体没有新增，就不生成配置
      int count = 0;
      foreach (var item in _windowRootArr)
      {
         string[] filePathArr = Directory.GetFiles(Application.dataPath+"/Resources/"+item, "*.prefab", SearchOption.AllDirectories);
         foreach (var path in filePathArr)
         {
            if (!path.EndsWith(".meta"))
            {
               count += 1;
            }
         }
      }

      if (count == windowDatList.Count)
      {
         Debug.Log("预制体个数没有发生改变,不生成窗口配置");
         return;
      }
            
      windowDatList.Clear();
      foreach (var item in _windowRootArr)
      {
         string folder = Application.dataPath + "/Resources/" + item;
         string[] filePathArr = Directory.GetFiles(folder, "*.prefab", SearchOption.AllDirectories);
         foreach (var path in filePathArr)
         {
            if (!path.EndsWith(".meta"))
            {
               string fileName = Path.GetFileNameWithoutExtension(path);
               string filePath = item + "/" + fileName;
               WindowData data = new WindowData { Name = fileName, Path = filePath };
               windowDatList.Add(data);
            }
         }
      }
   }

   public string GetWindowPath(string windowName)
   {
      foreach (var item in windowDatList)
      {
         if (string.Equals(item.Name, windowName))
         {
            return item.Path;
         }
      }
      Debug.LogError(windowName+"不存在于配置文件中，请检查预制体存放位置，或配置文件");
      return string.Empty;
   }
}

[Serializable]
public class WindowData
{
   public string Name;
   public string Path;
}
