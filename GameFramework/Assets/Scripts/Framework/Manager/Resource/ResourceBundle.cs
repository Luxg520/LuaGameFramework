using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// AssetBundle资源管理
/// </summary>
public class ResourceBundle : ManagerBase<ResourceBundle>
{
    // 已加载资源缓存
    private Dictionary<string, ResourceInfo> loadedResources = new Dictionary<string, ResourceInfo>();

    #region 外部接口

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
    /// 异步加载资源
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cb"></param>
    public void LoadAsync(string path, Action<UnityEngine.Object> cb)
    {
        StartCoroutine(LoadAsyncInternal(path, cb));
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

    #endregion

    #region 资源管理内部函数

    // 初始化
    protected override void Init()
    {
        base.Init();

    }

    // 创建一个资源对象
    private ResourceInfo CreateResource(string path)
    {
        ResourceInfo res = new ResourceInfo();
        res.path = path;
        return res;
    }

    // 获取一个资源对象
    private ResourceInfo GetResource(string path)
    {
        ResourceInfo res = null;
        loadedResources.TryGetValue(path, out res);
        return res;
    }

    // 创建常驻资源
    private void CreateResidentResources()
    {
        Debug.Log("创建常驻资源");
    }

    // 同步加载资源内部函数
    private UnityEngine.Object LoadInternal(string path)
    {

        return AssetBundle.LoadFromFile(path);
    }

    // 异步加载资源内部
    private IEnumerator LoadAsyncInternal(string path, Action<UnityEngine.Object> cb)
    {
        AssetBundleCreateRequest createRequest = AssetBundle.LoadFromFileAsync(path);
        yield return createRequest;
        AssetBundle assetBundle = createRequest.assetBundle;
        if (assetBundle == null)
        {
            Debug.LogError("Failed to load AssetBundle");
            yield break;
        }

        ResourceInfo resInfo = new ResourceInfo();
        resInfo.path = path;
        resInfo.assetBundle = assetBundle;

        AssetBundleRequest loadRequest = resInfo.assetBundle.LoadAssetAsync<GameObject>(path);
        yield return loadRequest;
        resInfo.mainObj = loadRequest.asset;

        // 加入缓存
        loadedResources.Add(path, resInfo);

        if (cb != null)
            cb(loadRequest.asset);
    }

    // 协程加载资源内部
    private IEnumerator LoadCoroutineInternal(string path, Action<UnityEngine.Object> cb)
    {
        yield return null;
        if (cb != null)
            cb(UnityEditor.AssetDatabase.LoadMainAssetAtPath(path));
    }

    #endregion
}
