using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 资源信息
/// </summary>
public class ResourceInfo
{    
    public string assetBundleName;    
    public AssetBundle assetBundle;    

    public UnityEngine.Object mainObj;
    public UnityEngine.Object[] allObjs;

    // 依赖列表
    public List<ResourceInfo> depends = new List<ResourceInfo>();
    // 被依赖列表
    public List<ResourceInfo> beDepends = null;

    // 是否被使用
    private bool used = false;
    public bool Used
    {
        get { return used; }
    }

    // 引用计数
    private int refCount = 1;
    public int RefCount
    {
        get { return refCount; }
        set
        {
            if (value < 0)
                refCount = 0;

            refCount = value;
            if (refCount > 1)
                used = true;
            else
                used = false;
            
        }
    }
}
