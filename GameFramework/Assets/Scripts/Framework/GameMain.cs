using UnityEngine;
using System.Collections;
/// <summary>
/// 游戏主体
/// </summary>
public class GameMain : MonoBehaviour {

    private void Start()
    {
        // 初始化游戏核心组件
        GameCore.Instance.Init();

        // 显示登录界面
        LoginUI.ShowUI(1);
    }
}
