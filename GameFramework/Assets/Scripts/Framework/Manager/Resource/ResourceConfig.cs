using UnityEngine;
using System.Collections;
/// <summary>
/// 资源路径配置
/// </summary>
public class ResourceConfig
{
    // Assets路径
    public const string AssetsPath = "Assets";
    // AssetBundle存放路径
    public static readonly string AssetBundlePath = Application.streamingAssetsPath + "/AssetBundles/";
    // 资源路径
    public static readonly string ResourcePath = AssetsPath + "/Bundles/";
    // UI资源路径
    public static readonly string UIPath = ResourcePath + "Prefabs/UI/";

    // 获取指定UI路径
    public static string GetUIPath(string path)
    {
        return UIPath + path + ".prefab";
    }
}
