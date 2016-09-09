using UnityEngine;
using System.Collections;
/// <summary>
/// 管理器基类
/// </summary>
public abstract class ManagerBase<T> : MonoBehaviour where T : ManagerBase<T>
{
    private static T instance = null;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<T>();
            }
            return instance;
        }
    }

    protected virtual void Init()
    {

    }

    protected virtual void Release()
    {

    }

    private void Start()
    {
        Init();
    }

    private void Destroy()
    {
        Release();
    }

}
