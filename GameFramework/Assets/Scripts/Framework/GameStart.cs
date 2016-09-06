using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 游戏启动器
/// </summary>
public class GameStart : MonoBehaviour
{
    ////  管理器集合
    //private Dictionary<string, MonoBehaviour> ManagerDict = new Dictionary<string, MonoBehaviour>();

    private void Start()
    {
        //InitManager();
        GameCore.Instance.Init();

        // 测试加载UI
        LoginUI.ShowUI(1);
    }

    // 初始化
    private void Init()
    {
        // 可用于做一些启动相关的初始化工作

        
    }

    //// 初始化管理器
    //private void InitManager()
    //{
    //    // 资源管理器
    //    AddManager<ResourceManager>("ResourceManager");

    //    // 协程管理器
    //    AddManager<CoroutineManager>("CoroutineManager");

    //    // 音乐管理器
    //    AddManager<AudioManager>("AudioManager");

    //    // UI管理器
    //    AddManager<UIManager>("UIManager");
    //}    

    //// 添加一个管理器
    //private T AddManager<T>(string name) where T : MonoBehaviour
    //{
    //    T mgr = FindObjectOfType<T>();
    //    if (mgr == null)
    //    {
    //        GameObject go = new GameObject("_" + name);
    //        go.transform.SetParent(ManagerRoot);
    //        mgr = go.AddComponent<T>();
    //    }
    //    return mgr;
    //}
}
