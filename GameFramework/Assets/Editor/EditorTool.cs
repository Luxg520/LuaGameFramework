using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditorTool : Editor
{
    #region BuildAssetBundle

    // 打包所有AssetBundle
    [MenuItem("Tool/BuildAssetbundle/BuildAll")]
    static void BuildAll()
    {
        BuildAssetBundle.DoBuildAll(ResourceConfig.ResourcePath);
    }

    #endregion

    #region UI

    // UI编辑助手
    [MenuItem("Tool/UI/UIHelper")]
    static void UIHelper()
    {

    }

    // 设置所有Sprite参数
    [MenuItem("Tool/UI/SetSpriteSetting/All")]
    static void SetAllSpriteSeting()
    {

    }

    // 设置选择的Sprite参数
    [MenuItem("Tool/UI/SetSpriteSetting/Selected")]
    static void SetSelectedSpriteSeting()
    {


    }

    #endregion
}
