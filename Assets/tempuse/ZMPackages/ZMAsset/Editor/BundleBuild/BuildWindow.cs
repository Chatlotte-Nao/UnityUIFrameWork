using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BuildWindows : OdinMenuEditorWindow
{
    [SerializeField]
    public BuildBundleWindow buildBundleWindow = new BuildBundleWindow();

    [SerializeField]
    public BuildHotPatchWindow buildHotWindow = new BuildHotPatchWindow();

    [SerializeField]
    public BundleSettings settingWindow ;

    [MenuItem("ZMFrame/AssetBundle")]
    public static void ShowAssetBundleWindow()
    {
        try
        {
            BuildWindows window = GetWindow<BuildWindows>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(985, 612);
            window.ForceMenuTreeRebuild();
        }
        catch (System.Exception)
        {

       
        }
       
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        buildBundleWindow.Initzation();
        buildHotWindow.Initzation();
        OdinMenuTree menuTree = new OdinMenuTree(supportsMultiSelect: false)
        {
            { "Build",null,EditorIcons.House},
            { "Build/AssetBundle",buildBundleWindow,EditorIcons.UnityLogo},
            { "Build/HotPatch",buildHotWindow,EditorIcons.UnityLogo},
            { "Bundle Setting",BundleSettings.Instance,EditorIcons.SettingsCog},
        };
        return menuTree;
    }
}
