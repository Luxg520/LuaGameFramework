﻿using UnityEngine;
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

    #region 加载

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

    // 同步加载资源
    public UnityEngine.Object Load(string path)
    {
        return LoadInternal(path);
    }

    // 异步加载资源
    public void LoadAsync(string path, Action<UnityEngine.Object> cb)
    {
        StartCoroutine(LoadAsyncInternal(path, cb));
    }

    // 协程加载资源
    public void LoadCoroutine(string path, Action<UnityEngine.Object> cb)
    {
        StartCoroutine(LoadCoroutineInternal(path, cb));
    }

    #endregion

    #region 卸载

    // 安全卸载资源
    public void SafeUnLoad(string assetBundleName)
    {
        ResourceInfo res = GetResource(assetBundleName);

        // 是否正在引用该资源
        if (res.Used)
            return;

        // 遍历卸载所有依赖资源
        for (int i = 0; i < res.depends.Count; i++)
        {
            SafeUnLoad(res.depends[i].assetBundleName);
        }

        // 卸载主体
        DestroyImmediate(res.mainObj, true);
        res.mainObj = null;
        res.assetBundle.Unload(false);
        res.assetBundle = null;
        loadedResources.Remove(assetBundleName);

        Debug.Log("卸载成功:" + assetBundleName);
    }

    // 卸载该资源及全部引用
    public void UnLoadAll(string assetBundleName)
    {
        ResourceInfo res = GetResource(assetBundleName);

        // 遍历卸载所有依赖资源
        for (int i = 0; i < res.depends.Count; i++)
        {
            SafeUnLoad(res.depends[i].assetBundleName);
        }

        // 卸载主体
        DestroyImmediate(res.mainObj, true);
        res.mainObj = null;
        res.assetBundle.Unload(true);
        res.assetBundle = null;
        loadedResources.Remove(assetBundleName);

        Debug.Log("卸载成功:" + assetBundleName);
    }

    #endregion

    #region 资源管理内部函数

    // AB总依赖文件
    AssetBundleManifest abMainfest;

    // 初始化
    public override void Init()
    {
        base.Init();

        // 加载依赖关系文件
        InitDepend();

        // 创建常驻资源
        CreateResidentResources();
        
    }

    // 初始化所有依赖关系
    private void InitDepend()
    {
        //StartCoroutine(LoadManifest(ResourceConfig.ABManifestUrl, cb));
        AssetBundle ab = AssetBundle.LoadFromFile(ResourceConfig.ABManifestPath);
        abMainfest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        ab.Unload(false);
    }

    // 创建常驻资源
    private void CreateResidentResources()
    {
        Debug.Log("创建常驻资源");
    }

    // 加载依赖关系文件
    //private IEnumerator LoadManifest(string url, Action cb)
    //{
    //    WWW www = WWW.LoadFromCacheOrDownload(url, 0);
    //    yield return www;
    //    if (www.error != null)
    //    {
    //        Debug.LogError("加载 ABManifest 失败:" + www.error);
    //        yield break;
    //    }

    //    AssetBundle ab = www.assetBundle;
    //    abMainfest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");       
    //    ab.Unload(false);

    //    if (abMainfest == null)
    //        Debug.LogError("加载 ABManifest 失败");
    //    else
    //    {
    //        Debug.Log("加载 ABManifest 成功");
    //        if (cb != null)
    //            cb();
    //    }
    //}

    // 获取一个资源对象
    private ResourceInfo GetResource(string assetBundleName)
    {
        ResourceInfo mainRes = null;
        loadedResources.TryGetValue(assetBundleName, out mainRes);
        return mainRes;
    }
    // 加载一个资源对象
    private ResourceInfo LoadResource(string assetBundleName)
    {
        ResourceInfo mainRes = GetResource(assetBundleName);
        // 该资源是否已加载
        if (mainRes == null)
        {
            // 创建资源
            mainRes = CreateResource(assetBundleName);
        }
        return mainRes;
    }

    // 创建一个资源对象
    private ResourceInfo CreateResource(string assetBundleName)
    {
        ResourceInfo mainRes = new ResourceInfo();
        // 加载该AssetBundle所有依赖的资源
        string[] dps = abMainfest.GetAllDependencies(assetBundleName);
        for (int i = 0; i < dps.Length; i++)
        {
            ResourceInfo dpRes = LoadResource(dps[i]);
            mainRes.depends.Add(dpRes);
        }
        // 从磁盘上加载主体AssetBundle到内存
        string path = ResourceConfig.ABPath + assetBundleName;
        mainRes.assetBundle = AssetBundle.LoadFromFile(path);
        mainRes.assetBundleName = assetBundleName;
        // 缓存下来
        loadedResources.Add(assetBundleName, mainRes);
        return mainRes;
    }

    // 加载一个资源Asset
    private ResourceInfo LoadAsset(string assetBundleName)
    {
        // 先获取该资源
        ResourceInfo mainRes = LoadResource(assetBundleName);

        // 如果该资源未被使用过
        if (!mainRes.Used)
        {
            // 获取真实的asset名称
            string assetName = assetBundleName.Substring(assetBundleName.LastIndexOf("/") + 1);
            // 加载asset镜像到内存
            mainRes.mainObj = mainRes.assetBundle.LoadAsset(assetName);
        }

        // 如果该资源已经被使用过，证明Asset镜像已加载在内存里
        // 记录一下引用计数即可

        // 引用 + 1
        mainRes.RefCount++;

        return mainRes;
    }

    // 同步加载资源内部函数
    private UnityEngine.Object LoadInternal(string assetBundleName)
    {
        // 加载该资源
        ResourceInfo mainRes = LoadAsset(assetBundleName);
        return mainRes.mainObj;
    }

    // 异步加载资源内部
    private IEnumerator LoadAsyncInternal(string assetBundleName, Action<UnityEngine.Object> cb)
    {
        // 是否在缓存中
        if (loadedResources.ContainsKey(assetBundleName))
        {
            if (cb != null)
                cb(loadedResources[assetBundleName].mainObj);
            yield break;
        }

        WWW www;
        // 加载该AssetBundle所有依赖的资源
        string[] dps = abMainfest.GetAllDependencies(assetBundleName);
        AssetBundle[] abs = new AssetBundle[dps.Length];
        for (int i = 0; i < dps.Length; i++)
        {
            string url = ResourceConfig.ABUrl + dps[i];
            www = WWW.LoadFromCacheOrDownload(url, abMainfest.GetAssetBundleHash(dps[i]));
            yield return www;
            abs[i] = www.assetBundle;
        }

        // 加载主体
        www = WWW.LoadFromCacheOrDownload(ResourceConfig.ABUrl + assetBundleName, abMainfest.GetAssetBundleHash(assetBundleName), 0);
        yield return www;
        if (www.error != null)
        {
            Debug.LogError(www.error);
            yield break;
        }       

        AssetBundle ab = www.assetBundle;
        UnityEngine.Object obj = ab.LoadAsset("LoginUI");            

        ResourceInfo resInfo = new ResourceInfo();
        resInfo.assetBundleName = assetBundleName;
        //resInfo.dependsAB = abs;
        resInfo.assetBundle = www.assetBundle;
        resInfo.mainObj = obj;

        // 加入缓存
        loadedResources.Add(assetBundleName, resInfo);

        if (cb != null)
            cb(obj);


        //AssetBundleCreateRequest createRequest = AssetBundle.LoadFromFileAsync(path);
        //yield return createRequest;
        //AssetBundle assetBundle = createRequest.assetBundle;
        //if (assetBundle == null)
        //{
        //    Debug.LogError("Failed to load AssetBundle");
        //    yield break;
        //}

        //ResourceInfo resInfo = new ResourceInfo();
        //resInfo.path = path;
        //resInfo.assetBundle = assetBundle;

        //AssetBundleRequest loadRequest = resInfo.assetBundle.LoadAssetAsync<GameObject>(path);
        //yield return loadRequest;
        //resInfo.mainObj = loadRequest.asset;


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
