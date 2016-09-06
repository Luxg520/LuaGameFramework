using UnityEngine;
using System.Collections;
/// <summary>
/// 协程管理器
/// </summary>
public class CoroutineManager : ManagerBase<CoroutineManager>
{
    // 开始协程
    public void StartCor(IEnumerator cor)
    {
        StartCoroutine(cor);
    }

    // 停止协程
    public void StopCor(IEnumerator cor)
    {
        StopCoroutine(cor);        
    }

    // 停止所有协程
    public void StopAll()
    {
        StopAllCoroutines();
    }
}
