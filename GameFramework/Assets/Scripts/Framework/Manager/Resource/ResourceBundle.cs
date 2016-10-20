using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// AssetBundle资源管理
/// </summary>
public class ResourceBundle : ManagerBase<ResourceBundle>
{
#if UNITY_5
    #region 资源管理

    // 已加载资源缓存
    private Dictionary<string, ResourceInfo> loadedResources = new Dictionary<string, ResourceInfo>();

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

    // 获取Asset资源名字
    private string GetAssetName(string assetBundleName)
    {
        return assetBundleName.Substring(assetBundleName.LastIndexOf("/") + 1);
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
        // 加载该资源
        ResourceInfo mainRes = LoadAsset(assetBundleName);
        return mainRes.mainObj;
    }

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
            string assetName = GetAssetName(assetBundleName);
            // 加载asset镜像到内存
            mainRes.mainObj = mainRes.assetBundle.LoadAsset(assetName);
        }

        // 如果该资源已经被使用过，证明Asset镜像已加载在内存里
        // 记录一下引用计数即可

        // 引用 + 1
        mainRes.RefCount++;

        return mainRes;
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
        yield return StartCoroutine(LoadResourceAsync(assetBundleName,
            (resInfo) =>
            {
                StartCoroutine(LoadAssetAsync(resInfo, cb));
            }));
    }

    // 异步加载一个资源对象
    private IEnumerator LoadResourceAsync(string assetBundleName, Action<ResourceInfo> cb)
    {
        ResourceInfo mainRes = GetResource(assetBundleName);
        // 该资源是否已加载
        if (mainRes == null)
        {
            // 创建资源
            yield return StartCoroutine(CreateResourceAsync(assetBundleName));
            mainRes = GetResource(assetBundleName);
        }
        // 返回该资源
        if (cb != null)
            cb(mainRes);
    }

    // 异步创建一个资源对象
    private IEnumerator CreateResourceAsync(string assetBundleName)
    {
        List<ResourceInfo> depends = new List<ResourceInfo>();
        // 加载该AssetBundle所有依赖的资源
        string[] dps = abMainfest.GetAllDependencies(assetBundleName);
        foreach (string dp in dps)
        {
            yield return StartCoroutine(LoadResourceAsync(dp, (dpRes) =>
            {
                depends.Add(dpRes);
            }));
        }

        // 从磁盘上加载主体AssetBundle到内存
        string mainUrl = "file://" + ResourceConfig.ABPath + assetBundleName;
        WWW www = new WWW(mainUrl);
        yield return www;
        if (www.error != null)
        {
            Debug.LogError(www.error);
            yield break;
        }

        ResourceInfo mainRes = new ResourceInfo();
        mainRes.assetBundleName = assetBundleName;
        mainRes.assetBundle = www.assetBundle;
        mainRes.depends = depends;

        // 缓存下来
        loadedResources.Add(assetBundleName, mainRes);

        www.Dispose();
    }

    // 异步加载Asset
    private IEnumerator LoadAssetAsync(ResourceInfo resInfo, Action<UnityEngine.Object> cb)
    {
        // 如果该资源未被使用过
        if (!resInfo.Used)
        {
            // 获取真实的asset名称
            string assetName = GetAssetName(resInfo.assetBundleName);
            // 加载asset镜像到内存
            AssetBundleRequest request = resInfo.assetBundle.LoadAssetAsync(assetName);

            yield return request;

            if (request.isDone)
            {
                resInfo.mainObj = request.asset;
            }
        }

        // 如果该资源已经被使用过，证明Asset镜像已加载在内存里
        // 记录一下引用计数即可
        // 引用 + 1
        resInfo.RefCount++;

        // 回调
        if (cb != null)
            cb(resInfo.mainObj);

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
        if (cb != null)
            cb(UnityEditor.AssetDatabase.LoadMainAssetAtPath(path));
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

#else
    #region 资源管理

    // 已加载资源缓存
    private Dictionary<string, ResourceInfo> loadedResources = new Dictionary<string, ResourceInfo>();

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
        
    }

    // 创建常驻资源
    private void CreateResidentResources()
    {
        Debug.Log("创建常驻资源");
    }

    // 获取Asset资源名字
    private string GetAssetName(string assetBundleName)
    {
        return assetBundleName.Substring(assetBundleName.LastIndexOf("/") + 1);
    }

    public class AssetBundleManifest
    {
        public string[] GetAllDependencies(string assetBundleName)
        {
            return null;
        }
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
        // 加载该资源
        ResourceInfo mainRes = LoadAsset(assetBundleName);
        return mainRes.mainObj;
    }

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
        mainRes.assetBundle = AssetBundle.CreateFromFile(path);
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
            string assetName = GetAssetName(assetBundleName);
            // 加载asset镜像到内存
            mainRes.mainObj = mainRes.assetBundle.Load(assetName);
        }

        // 如果该资源已经被使用过，证明Asset镜像已加载在内存里
        // 记录一下引用计数即可

        // 引用 + 1
        mainRes.RefCount++;

        return mainRes;
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
        yield return StartCoroutine(LoadResourceAsync(assetBundleName,
            (resInfo) =>
            {
                StartCoroutine(LoadAssetAsync(resInfo, cb));
            }));
    }

    // 异步加载一个资源对象
    private IEnumerator LoadResourceAsync(string assetBundleName, Action<ResourceInfo> cb)
    {
        ResourceInfo mainRes = GetResource(assetBundleName);
        // 该资源是否已加载
        if (mainRes == null)
        {
            // 创建资源
            yield return StartCoroutine(CreateResourceAsync(assetBundleName));
            mainRes = GetResource(assetBundleName);
        }
        // 返回该资源
        if (cb != null)
            cb(mainRes);
    }

    // 异步创建一个资源对象
    private IEnumerator CreateResourceAsync(string assetBundleName)
    {
        List<ResourceInfo> depends = new List<ResourceInfo>();
        // 加载该AssetBundle所有依赖的资源
        string[] dps = abMainfest.GetAllDependencies(assetBundleName);
        foreach (string dp in dps)
        {
            yield return StartCoroutine(LoadResourceAsync(dp, (dpRes) =>
            {
                depends.Add(dpRes);
            }));
        }

        // 从磁盘上加载主体AssetBundle到内存
        string mainUrl = "file://" + ResourceConfig.ABPath + assetBundleName;
        WWW www = new WWW(mainUrl);
        yield return www;
        if (www.error != null)
        {
            Debug.LogError(www.error);
            yield break;
        }

        ResourceInfo mainRes = new ResourceInfo();
        mainRes.assetBundleName = assetBundleName;
        mainRes.assetBundle = www.assetBundle;
        mainRes.depends = depends;

        // 缓存下来
        loadedResources.Add(assetBundleName, mainRes);

        www.Dispose();
    }

    // 异步加载Asset
    private IEnumerator LoadAssetAsync(ResourceInfo resInfo, Action<UnityEngine.Object> cb)
    {
        // 如果该资源未被使用过
        if (!resInfo.Used)
        {
            // 获取真实的asset名称
            string assetName = GetAssetName(resInfo.assetBundleName);
            // 加载asset镜像到内存
            AssetBundleRequest request = resInfo.assetBundle.LoadAsync(assetName,typeof(UnityEngine.Object));

            yield return request;

            if (request.isDone)
            {
                resInfo.mainObj = request.asset;
            }
        }

        // 如果该资源已经被使用过，证明Asset镜像已加载在内存里
        // 记录一下引用计数即可
        // 引用 + 1
        resInfo.RefCount++;

        // 回调
        if (cb != null)
            cb(resInfo.mainObj);

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
        if (cb != null)
            cb(UnityEditor.AssetDatabase.LoadMainAssetAtPath(path));
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
#endif

}
