using UnityEngine;
using System.Collections;
/// <summary>
/// 资源路径配置
/// </summary>
public class ResourceConfig
{
    #region 资源路径

    // Assets路径
    public const string AssetsPath = "Assets";    
    // 资源路径
    public static readonly string ResourcePath = AssetsPath + "/Bundles/";
    // UI资源路径
    public static readonly string UIPath = ResourcePath + "Prefabs/UI/";
    // 场景资源路径
    public static readonly string ScenePath = ResourcePath + "Scenes/";
    // lua脚本路径
    public static readonly string LuaPath = Application.dataPath + "/Scripts/Lua/";
    // 配置表路径
    public static readonly string ConfigPath = ResourcePath + "Config/";
    // 版本路径
    public static readonly string VersionPath = ConfigPath + "version.txt";
    // 临时路径
    public static readonly string TempPath = AssetsPath + "/TempData/";

    #endregion

    #region AssetBundle相关

    // AssetBundle 文件路径
    public static string ABPath
    {
        get
        {
#if UNITY_EDITOR
            return Application.dataPath + "/../Build/AssetBundle/";
#elif  UNITY_IPHONE
            return null;
#elif  UNITY_ANDROID
            return null;
#else 
            return null;
#endif
        }
    }

    // AssetBundle Manifest 文件路径
    public static string ABManifestPath
    {
        get
        {
            return ABPath + "AssetBundle";
        }
    }

    // 获取指定UI的AB_Name
    public static string GetUIPath(string path)
    {
        // AssetBundle名称均为小写
        return (UIPath + path).ToLower();
    }

    #endregion

    #region 更新相关

    // 资源更新地址
    public static string UpdateUrl
    {
        get
        {
#if UNITY_5
            string url = "file://C:\\MyFile\\Update\\win\\";
#else
            string url = "file://E:\\Update\\win\\";
#endif
            return url;
        }
    }

    // 资源版本地址
    public static string ResVersionUrl
    {
        get 
        {
            return UpdateUrl + "version.txt";
        }
    }

    // 获取资源信息地址
    public static string GetABInfoUrl(string version)
    {
        return UpdateUrl + version + "\\ABInfo.txt";
    }

    #endregion 

}
