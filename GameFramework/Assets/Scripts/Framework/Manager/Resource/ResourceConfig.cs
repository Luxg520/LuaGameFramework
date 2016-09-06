using UnityEngine;
using System.Collections;
/// <summary>
/// 资源路径配置
/// </summary>
public class ResourceConfig
{
    // 资源路径
    public static readonly string ResourcePath = "Assets/Bundles/";
    // UI资源路径
    public static readonly string UIPath = ResourcePath + "Prefabs/UI/";

    // 获取指定UI路径
    public static string GetUIPath(string path)
    {
        return UIPath + path + ".prefab";
    }
}
