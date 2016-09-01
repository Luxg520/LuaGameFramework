using UnityEngine;
using System.Collections;
/// <summary>
/// 单例Mono类
/// </summary>
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance = null;
    public static T Instance
    {
        get
        {
            if (instance = null)
            {
                instance = GameObject.FindObjectOfType<T>();
            }
            return instance;
        }
    }

    public virtual void Init()
    {

    }

    public virtual void Release()
    {

    }

}
