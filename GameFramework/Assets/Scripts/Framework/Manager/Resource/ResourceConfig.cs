using UnityEngine;
using System.Collections;
/// <summary>
/// 资源路径配置
/// </summary>
public class ResourceConfig
{
    // Assets路径
    public const string AssetsPath = "Assets";
    // 资源路径
    public static readonly string ResourcePath = AssetsPath + "/Bundles/";
    // UI资源路径
    public static readonly string UIPath = ResourcePath + "Prefabs/UI/";


    #region AssetBundle相关

    // AssetBundle 文件路径
    public static string ABPath
    {
        get
        {
#if UNITY_EDITOR
            return BuildPath;
#endif
        }
    }

    // AssetBundle Manifest 文件路径
    public static string ABManifestPath
    {
        get
        {
#if UNITY_EDITOR
            return ABPath + "AssetBundle";
#endif
        }
    }

    // 获取指定UI的AB_Name
    public static string GetUIPath(string path)
    {
        // AssetBundle名称均为小写
        return (UIPath + path).ToLower();
    }

    #endregion


    #region 打包相关路径

    // 打包路径
    public static readonly string BuildPath = Application.dataPath + "/../Build/AssetBundle/";

    // 打包进度路径
    public static readonly string BuildProgressPath = BuildPath + "/Progress.txt";

    #endregion
}
