using UnityEngine;
using System.Collections;
/// <summary>
/// 场景管理器
/// </summary>
public class SceneManager : ManagerBase<SceneManager>
{
    public Transform SceneRoot;

    public override void Init()
    {
        base.Init();

        SceneRoot = GameObject.Find("SceneRoot").transform;
    }

    // 加载场景
    public void Load(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }

    // 异步加载场景
    public void LoadAsync(string sceneName)
    {
        Application.LoadLevelAsync(sceneName);
    }

    // 加载并附加场景
    public void LoadAdditive(string sceneName)
    {
        Application.LoadLevelAdditive(sceneName);
    }

    // 异步加载并附加场景
    public void LoadAsyncAdditive(string sceneName)
    {
        Application.LoadLevelAdditiveAsync(sceneName);
    }

}
