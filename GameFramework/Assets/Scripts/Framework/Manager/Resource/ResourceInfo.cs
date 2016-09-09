using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 资源信息
/// </summary>
public class ResourceInfo
{    
    public string path;    
    public AssetBundle assetBundle;
    public AssetBundle[] dependsAB;

    public UnityEngine.Object mainObj;
    public UnityEngine.Object[] allObjs;

    // 依赖列表
    public List<ResourceInfo> depends = null;
    // 被依赖列表
    public List<ResourceInfo> beDepends = null;

    // 引用计数
    private int refCount;
}
