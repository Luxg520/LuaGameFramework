using System;
using System.IO;
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

    // 初始化完成
    public event Action OnInitilized = null;

    // 初始化
    public void Init()
    {
        // 加载配置表
        CsvMgr.Load();

        // 协程组件
        CoroutineManager coMgr = new CoroutineManager();
        Add("CoroutineManager", coMgr);

        // 网络模块
        NetCore nc = new NetCore();
        Add("NetCore", nc);
        // 请求超时时间
        nc.RequestExpireTime = 3;

        // 计时器
        Timer T = new Timer();
        Add("Timer", T);

        // 初始化完成
        if (OnInitilized != null)
            OnInitilized();
    }

}
