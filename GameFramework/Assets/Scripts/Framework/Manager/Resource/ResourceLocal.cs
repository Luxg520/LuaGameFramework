using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 本地资源管理
/// </summary>
public class ResourceLocal : ManagerBase<ResourceLocal>
{
    #region 资源管理

    // 初始化
    public override void Init()
    {
        base.Init();

    }

    // 获取Asset资源名字
    private string GetAssetName(string assetBundleName)
    {
        return assetBundleName.Substring(assetBundleName.LastIndexOf("/") + 1);
    }

    #endregion

    #region 实例化

    // 同步加载并实例化Object
    public GameObject Instantiate(string path)
    {
        GameObject go = InstantiateGo(Load(path));
        return go;
    }

    // 实例化 Object
    public GameObject InstantiateGo(UnityEngine.Object asset)
    {
        GameObject go = (GameObject)InstantiateGo(asset);
        go.EraseNameClone();
        return go;
    }

    #endregion

    #region 同步加载

    // 同步加载资源
    public UnityEngine.Object Load(string path)
    {
        return LoadInternal(path);
    }

    // 同步加载资源内部函数
    private UnityEngine.Object LoadInternal(string assetBundleName)
    {
#if UNITY_EDITOR
        return UnityEditor.AssetDatabase.LoadMainAssetAtPath(assetBundleName);
#endif
    }

    #endregion

    #region 异步加载

    // 异步加载资源
    public void LoadAsync(string path, Action<UnityEngine.Object> cb)
    {
        StartCoroutine(LoadAsyncInternal(path, cb));
    }

    // 异步加载资源内部
    private IEnumerator LoadAsyncInternal(string assetBundleName, Action<UnityEngine.Object> cb)
    {
        yield return null;
#if UNITY_EDITOR
        if (cb != null)
            cb(UnityEditor.AssetDatabase.LoadMainAssetAtPath(assetBundleName));
#endif                
    }
    #endregion

    #region 协程加载

    // 协程加载资源
    public void LoadCoroutine(string path, Action<UnityEngine.Object> cb)
    {
        StartCoroutine(LoadCoroutineInternal(path, cb));
    }

    // 协程加载资源内部
    private IEnumerator LoadCoroutineInternal(string path, Action<UnityEngine.Object> cb)
    {
        yield return null;
#if UNITY_EDITOR
        if (cb != null)
            cb(UnityEditor.AssetDatabase.LoadMainAssetAtPath(path));
#endif
    }

    #endregion

    #region 卸载

    // 安全卸载资源
    public void SafeUnLoad(string assetBundleName)
    {        
        Debug.Log("卸载:" + assetBundleName);
    }

    // 卸载该资源及全部引用
    public void UnLoadAll(string assetBundleName)
    {
        Debug.Log("卸载:" + assetBundleName);
    }

    #endregion
}
