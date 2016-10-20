using UnityEngine;
using System.IO;
using System.Collections;
/// <summary>
/// 游戏加载器
/// </summary>
public class GameLoader : MonoBehaviour
{
    // 最新版本号
    private string newVersion;

    // 本地版本号
    private string localVersion;

    private void Start()
    {
        // 检查更新
        StartCoroutine(CheckUpdate());        
    }

    // 检查更新
    IEnumerator CheckUpdate()
    {
        // 1: 获取最新版本信息
        yield return StartCoroutine(GetNewVersionInfo());

        // 2: 启动版本比较，如无更新则直接进入游戏
        if (!CompareVersion())
        {
            // 3: 直接进入游戏
            // TODO
            Debug.Log("无需更新，即将进入游戏！");
        }
        else
        {
            // 3: 获取资源列表，差异资源更新下载
            yield return StartCoroutine(DownloadResource());

            // 4: 资源更新完毕，卸载Loader场景，进入游戏GameMain场景
            // TODO
            Debug.Log("进入游戏");
            object resMgr = ResourceManager.Instance;

#if UNITY_5
            SceneManager.Instance.Load("GameMain");
#else 
            SceneManager.Instance.Load("GameMain_4");
#endif
        }
    }
    
    // 获取最新版本信息
    IEnumerator GetNewVersionInfo()
    {
        string url = ResourceConfig.ResVersionUrl;
        WWW www = new WWW(url);
        yield return www;
        if (www.error != null)
        {
            Debug.LogError(www.error);
            yield break;
        }
        newVersion = www.text;
        Debug.Log("当前最新版本:" + newVersion);

    }

    // 比较版本是否有资源差异
    bool CompareVersion()
    {
        // 加载本地版本号
        localVersion = File.ReadAllText(ResourceConfig.VersionPath);

        return localVersion != newVersion;
    }

    // 下载差异资源
    IEnumerator DownloadResource()
    {
        // 下载 ABInfo 文件进行增量更新
        string abInfoUrl = ResourceConfig.GetABInfoUrl(newVersion);
        WWW www = new WWW(abInfoUrl);
        yield return www;
        if (www.error != null)
        {
            Debug.LogError(www.error);
            yield break;
        }
        Debug.Log(www.text);
    }
}
