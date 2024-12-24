using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GeneratorConfig
{
    public static string BindComponentGeneratorPath = Application.dataPath + "/Scripts/BindComponent";
    public static string FindComponentGeneratorPath = Application.dataPath + "/Scripts/FindComponent";
    public static string WindowGeneratorPath=Application.dataPath + "/Scripts/Window";
    public static string OBJDATALIST_KEY = "objDataList";
    public static GeneratorType GeneratorType = GeneratorType.Bind;
}

public enum GeneratorType
{
    Find,//组件查找
    Bind,//组件绑定
}
//考虑下自己手动添加一下组件