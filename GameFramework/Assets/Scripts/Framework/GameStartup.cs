using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 游戏启动器
/// </summary>
public class GameStartup : MonoBehaviour
{    
    private void Start()
    {
        // 设置目标帧率
        UnityEngine.Application.targetFrameRate = 40;

        // 不锁屏
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // 启动游戏加载更新
#if UNITY_5
        SceneManager.Instance.Load("GameLoader");
#else
        SceneManager.Instance.Load("GameLoader_4");
#endif
    }
}
