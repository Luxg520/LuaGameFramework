using UnityEngine;
using System.Collections;
/// <summary>
/// 游戏主体
/// </summary>
public class GameMain : MonoBehaviour {

    private void Start()
    {
        //设置目标帧率
        UnityEngine.Application.targetFrameRate = 40;

        // 不锁屏
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // 初始化管理器
        InitManager();

        // 初始化游戏核心组件
        GameCore.Instance.Init();
    }

    // 初始化管理器
    private void InitManager()
    {
        ResourceManager.Instance.Init();

        UIManager.Instance.Init();
    }
}
