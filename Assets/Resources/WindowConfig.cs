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
      windowDatList.Clear();
      foreach (var item in _windowRootArr)
      {
         string folder = Application.dataPath + "/Resources/" + item;
         string[] filePathArr = Directory.GetFiles(folder, "*.prefab", SearchOption.AllDirectories);
         foreach (var path in filePathArr)
         {
            if (!path.EndsWith(".meta"))
            {
               
            }
         }

      }
   }
}

[Serializable]
public class WindowData
{
   public string Name;
   public string Path;
}
