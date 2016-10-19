using UnityEngine;
using System.Collections;
/// <summary>
/// 游戏加载器
/// </summary>
public class GameLoader : MonoBehaviour
{
    private void Start()
    {
        // 1: 获取版本信息，启动版本比较，如无更新则直接进入游戏

        // 2: 获取资源列表，资源更新下载

        // 3: 资源更新完毕，卸载Loader场景，进入游戏GameMain场景
    }
}
