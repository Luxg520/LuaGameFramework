using UnityEngine;
using System.Collections;
/// <summary>
/// 本地资源管理
/// </summary>
public class ResourceLocal : ManagerBase<ResourceLocal>
{
    /// <summary>
    /// 加载并实例化Object
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <returns>GameObject</returns>
    public GameObject Instantiate(string path)
    {
        GameObject go = (GameObject)Instantiate(LoadResource(path));        
        go.EraseNameClone();
        return go;
    }

    /// <summary>
    /// // 加载资源
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <returns></returns>
    public UnityEngine.Object LoadResource(string path)
    {
        return LoadResourceInternal(path);
    }

    /// <summary>
    /// 加载资源内部函数
    /// </summary>
    /// <param name="path">资源路径</param>
    private UnityEngine.Object LoadResourceInternal(string path)
    {
#if UNITY_EDITOR
        return UnityEditor.AssetDatabase.LoadMainAssetAtPath(path);
#endif
    }
}
