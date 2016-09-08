using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 本地资源管理
/// </summary>
public class ResourceLocal : ManagerBase<ResourceLocal>
{
    /// <summary>
    /// 同步加载并实例化Object
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <returns>GameObject</returns>
    public GameObject Instantiate(string path)
    {
        GameObject go = Instantiate(Load(path));        
        return go;
    }

    /// <summary>
    /// 实例化 Object
    /// </summary>
    /// <param name="asset"></param>
    /// <returns></returns>
    public GameObject Instantiate(UnityEngine.Object asset)
    {
        GameObject go = (GameObject)Instantiate(asset);        
        go.EraseNameClone();
        return go;
    }

    /// <summary>
    /// 同步加载资源
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <returns></returns>
    public UnityEngine.Object Load(string path)
    {
        return LoadInternal(path);        
    }

    /// <summary>
    /// 同步加载资源内部函数
    /// </summary>
    /// <param name="path">资源路径</param>
    private UnityEngine.Object LoadInternal(string path)
    {
#if UNITY_EDITOR
        return UnityEditor.AssetDatabase.LoadMainAssetAtPath(path);
#endif
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cb"></param>
    public void LoadAsync(string path, Action<UnityEngine.Object> cb)
    {
        LoadAsyncInternal(path, cb);
    }

    /// <summary>
    /// 异步加载资源内部
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cb"></param>
    private void LoadAsyncInternal(string path, Action<UnityEngine.Object> cb)
    {
#if UNITY_EDITOR
        if (cb != null)
            cb(UnityEditor.AssetDatabase.LoadMainAssetAtPath(path));
#endif                
    }

    /// <summary>
    /// 协程加载资源
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cb"></param>
    public void LoadCoroutine(string path, Action<UnityEngine.Object> cb)
    {
        StartCoroutine(LoadCoroutineInternal(path, cb));
    }

    /// <summary>
    /// 协程加载资源内部
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cb"></param>
    private IEnumerator LoadCoroutineInternal(string path, Action<UnityEngine.Object> cb)
    {        
        yield return null;
#if UNITY_EDITOR               
        if (cb != null)
            cb(UnityEditor.AssetDatabase.LoadMainAssetAtPath(path));
#endif                
    }
}
