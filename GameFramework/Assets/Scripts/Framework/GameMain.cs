using UnityEngine;
using System.Collections;
using LuaInterface;
/// <summary>
/// 游戏主程序
/// </summary>
public class GameMain : MonoBehaviour
{
    public static GameMain Instance = null;

    public LuaState luaState = null;
    public LuaLooper looper = null;

    // 游戏开始
    private void Start()
    {
        Instance = this;

        luaState = new LuaState();
        luaState.Start();
        LuaBinder.Bind(luaState);
        looper = gameObject.AddComponent<LuaLooper>();
        looper.luaState = luaState;

        string path = ResourceConfig.LuaPath;
        luaState.AddSearchPath(path);
        luaState.DoFile("GameMain.lua");

        // 执行游戏核心初始化函数
        LuaFunction initFunc = luaState.GetFunction("Init");
        initFunc.Call();
        initFunc.Dispose();
        initFunc = null; 
    }

    // 游戏退出
    private void OnDestroy()
    {
        looper.Destroy();
        luaState.Dispose();
        luaState = null;
    }
}
