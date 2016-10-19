using UnityEngine;
using System.Collections;
using Swift;

/// <summary>
/// 游戏核心
/// </summary>
public class GameCore : Core
{
    private static GameCore instance;
    public static GameCore Instance
    {
        get
        {
            if (instance == null)
                instance = new GameCore();
            return instance;
        }
    }

    // 初始化
    public void Init()
    {
        // 
    }

}
