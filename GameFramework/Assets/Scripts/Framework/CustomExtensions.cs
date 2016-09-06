using UnityEngine;
using System.Collections;
/// <summary>
/// 扩展方法
/// </summary>
public static class CustomExtensions
{   
    // 去除实例化物体的Clone后缀
    public static void EraseNameClone(this GameObject go)
    {
        string Clone = "(Clone)";
        string n = go.name;
        if (n.EndsWith(Clone))
            go.name = n.Substring(0, n.Length - Clone.Length);
    }
}
