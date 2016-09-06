using UnityEngine;
using System.Collections;
/// <summary>
/// UI工具库
/// </summary>
public static class UIUtils
{
    /// <summary>
    /// 添加UI到指定父节点下
    /// </summary>
    /// <param name="parent">父节点</param>
    /// <param name="ui">uiPrefab</param>
    public static void AddUIPrefab(Transform _parent, Transform _ui)
    {
        _ui.SetParent(_parent, false);
    }

}
