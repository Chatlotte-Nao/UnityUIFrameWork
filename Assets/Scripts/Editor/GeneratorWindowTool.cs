using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Text;

public class GeneratorWindowTool : Editor
{

    static Dictionary<string, string> methodDic = new Dictionary<string, string>();
    [MenuItem("GameObject/生成Window脚本(Shift+V) #V", false, 0)]
    static void CreateFindComponentScripts()
    {
        GameObject obj = Selection.objects.First() as GameObject;//获取到当前选择的物体
        if (obj == null)
        {
            Debug.LogError("需要选择 GameObject");
            return;
        }


        //设置脚本生成路径
        if (!Directory.Exists(GeneratorConfig.WindowGeneratorPath))
        {
            Directory.CreateDirectory(GeneratorConfig.WindowGeneratorPath);
        }

        //生成CS脚本
        string csContnet = CreateWindoCs(obj.name);

        Debug.Log("CsConent:\n" + csContnet);
        string cspath = GeneratorConfig.WindowGeneratorPath + "/" + obj.name + ".cs";
        UIWindowEditor.ShowWindow(csContnet, cspath, methodDic);
    }
    /// <summary>
    /// 生成Window脚本
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string CreateWindoCs(string name)
    {
        //储存字段名称
        string datalistJson = PlayerPrefs.GetString(GeneratorConfig.OBJDATALIST_KEY);
        List<EditorObjectData> objDatalist = JsonConvert.DeserializeObject<List<EditorObjectData>>(datalistJson);
        methodDic.Clear();
        StringBuilder sb = new StringBuilder();

        //添加引用
        sb.AppendLine("/*---------------------------------");
        sb.AppendLine(" *Title:UI表现层脚本自动化生成工具");
        sb.AppendLine(" *Date:" + System.DateTime.Now);
        sb.AppendLine(" *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用");
        sb.AppendLine("---------------------------------*/");
        sb.AppendLine("using UnityEngine.UI;");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("using UIFrameWork;");

        //生成类命
        sb.AppendLine($"public class {name}:WindowBase");
        sb.AppendLine("{");
        //sb.AppendLine("t");
        if (GeneratorConfig.GeneratorType == GeneratorType.Bind)
        {
            //生成字段
            sb.AppendLine($"\t public {name}DataComponent dataCompt;");
        }
        else
        {
            //生成字段
            sb.AppendLine($"\t public {name}UIComponent uiCompt=new {name}UIComponent();");
        }


        //生成生命周期函数 Awake
        sb.AppendLine("\t");
        sb.AppendLine($"\t #region 生命周期函数");
        sb.AppendLine($"\t //调用机制与Mono Awake一致");
        sb.AppendLine("\t public override void OnAwake()");
        sb.AppendLine("\t {");
        if (GeneratorConfig.GeneratorType == GeneratorType.Bind)
        {
            sb.AppendLine($"\t\t dataCompt=gameObject.GetComponent<{name}DataComponent>();");
            sb.AppendLine($"\t\t dataCompt.InitComponent(this);");
        }
        else
            sb.AppendLine($"\t\t uiCompt.InitComponent(this);");
        sb.AppendLine("\t\t base.OnAwake();");
        sb.AppendLine("\t }");
        //OnShow
        sb.AppendLine($"\t //物体显示时执行");
        sb.AppendLine("\t public override void OnShow()");
        sb.AppendLine("\t {");
        sb.AppendLine("\t\t base.OnShow();");
        sb.AppendLine("\t }");
        //OnHide
        sb.AppendLine($"\t //物体隐藏时执行");
        sb.AppendLine("\t public override void OnHide()");
        sb.AppendLine("\t {");
        sb.AppendLine("\t\t base.OnHide();");
        sb.AppendLine("\t }");

        //OnDestroy
        sb.AppendLine($"\t //物体销毁时执行");
        sb.AppendLine("\t public override void OnDestroy()");
        sb.AppendLine("\t {");
        sb.AppendLine("\t\t base.OnDestroy();");
        sb.AppendLine("\t }");

        sb.AppendLine($"\t #endregion");

        //API Function 
        sb.AppendLine($"\t #region API Function");
        sb.AppendLine($"\t    ");
        sb.AppendLine($"\t #endregion");

        //UI组件事件生成
        sb.AppendLine($"\t #region UI组件事件");
        foreach (var item in objDatalist)
        {
            string type = item.fieldType;
            string methodName = "On" + item.fieldName;
            string suffix = "";
            if (type.Contains("Button"))
            {
                suffix = "ButtonClick";
                CreateMethod(sb, ref methodDic, methodName + suffix);
            }
            else if (type.Contains("InputField"))
            {
                suffix = "InputChange";
                CreateMethod(sb, ref methodDic, methodName + suffix, "string text");
                suffix = "InputEnd";
                CreateMethod(sb, ref methodDic, methodName + suffix, "string text");
            }
            else if (type.Contains("Toggle"))
            {
                suffix = "ToggleChange";
                CreateMethod(sb, ref methodDic, methodName + suffix, "bool state,Toggle toggle");
            }
        }

        sb.AppendLine($"\t #endregion");

        sb.AppendLine("\t}");
        return sb.ToString();
    }
    /// <summary>
    /// 生成UI事件方法
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="methodDic"></param>
    /// <param name="modthName"></param>
    /// <param name="param"></param>
    public static void CreateMethod(StringBuilder sb, ref Dictionary<string, string> methodDic, string methodName, string param = "")
    {
        //声明UI组件事件
        sb.AppendLine($"\t public void {methodName}({param})");
        sb.AppendLine("\t {");
        if (methodName == "OnCloseButtonClick")
        {
            sb.AppendLine("\t\tHideWindow();");
        }
        sb.AppendLine("\t }");

        //存储UI组件事件 提供给后续新增代码使用
        StringBuilder builder = new StringBuilder();
        builder.AppendLine($"\t public void {methodName}({param})");
        builder.AppendLine("\t {");
        builder.AppendLine("\t");
        builder.AppendLine("\t }");
        methodDic.Add(methodName, builder.ToString());
    }
}
