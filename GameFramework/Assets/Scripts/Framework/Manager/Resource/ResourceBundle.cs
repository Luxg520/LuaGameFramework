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
        GameObject go = InstantiateGo(Load(path));
        return go;
    }

    /// <summary>
    /// 实例化 Object
    /// </summary>
    /// <param name="asset"></param>
    /// <returns></returns>
    public GameObject InstantiateGo(UnityEngine.Object asset)
    {
        GameObject go = (GameObject)InstantiateGo(asset);
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

    // AB总依赖文件
    AssetBundleManifest abMainfest;

    // 初始化
    protected override void Init()
    {
        base.Init();

        // 加载依赖关系文件
        InitDepend(()=> 
        {
            // 创建常驻资源
            CreateResidentResources();

            //CreateResource("5d4fdb67af4ec7544b412c2036a87da7.ab");
            // 测试
            LoginUI.ShowUI(1);
        });
    }

    // 初始化所有依赖关系
    private void InitDepend(Action cb)
    {
        StartCoroutine(LoadManifest(ResourceConfig.ABManifestUrl, cb));
    }

    // 加载依赖关系文件
    private IEnumerator LoadManifest(string url, Action cb)
    {
        WWW www = WWW.LoadFromCacheOrDownload(url, 0);
        yield return www;
        if (www.error != null)
        {
            Debug.LogError("加载 ABManifest 失败:" + www.error);
            yield break;
        }

        AssetBundle ab = www.assetBundle;
        abMainfest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");       
        ab.Unload(false);

        if (abMainfest == null)
            Debug.LogError("加载 ABManifest 失败");
        else
        {
            Debug.Log("加载 ABManifest 成功");
            if (cb != null)
                cb();
        }
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
    private UnityEngine.Object LoadInternal(string assetBundleName)
    {
        // 加载该AssetBundle所有依赖的资源
        string[] dps = abMainfest.GetAllDependencies(assetBundleName);
        AssetBundle[] abs = new AssetBundle[dps.Length];
        for (int i = 0; i < dps.Length; i++)
        {
            string url = ResourceConfig.BuildPath + dps[i];

            abs[i] = AssetBundle.LoadFromFile(url);
        }

        // 加载主体
        AssetBundle mainAB = AssetBundle.LoadFromFile(ResourceConfig.BuildPath + assetBundleName);
        UnityEngine.Object obj = mainAB.LoadAsset("LoginUI");

        return obj;
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
        resInfo.path = assetBundleName;
        resInfo.dependsAB = abs;
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
