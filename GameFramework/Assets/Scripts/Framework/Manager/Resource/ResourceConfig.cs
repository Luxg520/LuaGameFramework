using UnityEngine;
using System.Collections;
/// <summary>
/// 资源路径配置
/// </summary>
public class ResourceConfig
{
    // Assets路径
    public const string AssetsPath = "Assets";
    //// AssetBundle存放路径
    //public static readonly string AssetBundlePath = Application.streamingAssetsPath + "/AssetBundles/";
    // 资源路径
    public static readonly string ResourcePath = AssetsPath + "/Bundles/";
    // UI资源路径
    public static readonly string UIPath = ResourcePath + "Prefabs/UI/";

    // AssetBundle 文件路径
    public static string ABUrl
    {
        get
        {
#if UNITY_EDITOR
            return "file://" + BuildPath;
#endif
        }
    }

    // AssetBundle Manifest 文件路径
    public static string ABManifestUrl
    {
        get
        {
#if UNITY_EDITOR
            return ABUrl + "AssetBundle";
#endif
        }
    }


    // 获取指定UI路径
    public static string GetUIPath(string path)
    {
        return UIPath + path + ".prefab";
    }


    #region 打包相关路径

    // 打包路径
    public static readonly string BuildPath = Application.dataPath + "/../Build/AssetBundle/";

    // 打包进度路径
    public static readonly string BuildProgressPath = BuildPath + "/Progress.txt";

    #endregion
}
