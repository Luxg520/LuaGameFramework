using UnityEngine;
using System;
using System.Collections;
/// <summary>
/// 协程管理器
/// </summary>
public class CoroutineMgr : ManagerBase<CoroutineMgr>
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

    // 延迟任务
    public void DelayTask(float t, Action cb)
    {
        StartCoroutine(DelayTask_Internal(t, cb));
    }

    // 延迟任务内部
    private IEnumerator DelayTask_Internal(float t, Action cb)
    {
        yield return new WaitForSeconds(t);
        if (cb != null)
            cb();
    }
}
