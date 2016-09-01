using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// UI管理器
/// </summary>
public class UIManager : Singleton<UIManager>
{
    #region 公共成员

    // 游戏主UI层
    public Canvas GameUICanvas;
    // 游戏前置UI层
    public Canvas PreposeUICanvas;
    // 游戏指引UI层
    public Canvas GuideUICanvas;
    // 游戏测试UI层
    public Canvas TestUICanvas;

    // 返回指定层级UICanvas
    public Canvas GetUICanvas(UILayer _layer)
    {
        switch (_layer)
        {
            case UILayer.Game:
                return GameUICanvas;
            case UILayer.Prepose:
                return PreposeUICanvas;
            case UILayer.Guide:
                return GuideUICanvas;
            case UILayer.Test:
                return TestUICanvas;
        }
        return null;
    }
    #endregion

    #region 静态成员

    // 所有已注册UI
    private static Dictionary<UIType, UIInfo> m_UIDict = new Dictionary<UIType, UIInfo>();
    // 已加载UI
    private static Dictionary<UIType, UIInfo> m_LoadedUI = new Dictionary<UIType, UIInfo>();

    /// <summary>
    /// 显示UI界面
    /// </summary>
    /// <param name="_type">UI类型</param>
    /// <param name="_params">初始化参数</param>
    /// <returns></returns>
    public static UIBase ShowUI(UIType _type, UILayer _layer, params object[] _params)
    {
        // UI 是否已注册
        if (!m_UIDict.ContainsKey(_type))
            throw new Exception("ui has not register!");

        // 加载并显示UI
        UIInfo uiInfo;
        if (!m_LoadedUI.TryGetValue(_type, out uiInfo))
        {
            // 加载UI
            uiInfo = Instance.LoadUI(_type, _layer);
        }

        uiInfo.layer = _layer;
        UIBase ui = uiInfo.ui;

        // 显示UI
        Transform uiTrans = UIManager.Instance.GetUICanvas(_layer).transform;
        UIUtils.AddUIPrefab(uiTrans, ui.MyTrans);
        ui.gameObject.SetActive(true);

        // 初始化
        ui.Init(_params);

        return ui;
    }

    /// <summary>
    /// 隐藏UI界面
    /// </summary>
    /// <param name="_type">UI类型</param>
    /// <param name="_params">所需参数</param>
    /// <returns></returns>
    public static UIBase HideUI(UIType _type, params object[] _params)
    {
        return null;
    }

    /// <summary>
    /// 卸载UI界面
    /// </summary>
    /// <param name="_type">UI类型</param>
    /// <param name="_params">卸载所需参数</param>
    public static void ReleaseUI(UIType _type, params object[] _params)
    {

    }

    // 获取UI实例
    public static T GetUI<T>() where T : class
    {
        foreach (var ui in m_UIDict.Values)
        {
            if (ui is T)
            {
                return ui as T;
            }
        }
        return null;
    }

    // 获取UI实例
    public static UIBase GetUI(UIType _type)
    {
        if (m_UIDict.ContainsKey(_type))
        {
            return m_UIDict[_type].ui;
        }
        return null;
    }

    // 获取UI资源路径
    public static string GetUIPath(UIBase _ui)
    {
        return GetUIPath(_ui);
    }

    // 获取UI资源路径
    public static string GetUIPath(UIType _type)
    {
        UIInfo uiInfo;
        if (!m_UIDict.TryGetValue(_type, out uiInfo))
            return null;
        return uiInfo.path;
    }

    #endregion

    #region UI资源管理

    public UIInfo LoadUI(UIType _type, UILayer _layer)
    {
        return null;
    }

    public void ReleaseUI(UIType _type)
    {

    }

    #endregion

    #region 内部函数

    // 初始化UI
    private void InitUIs()
    {
        // 登录
        AddUI(UIType.LoginUI, UIStyle.Simple, "Login/LoginUI");


    }

    // 注册UI，所有需要显示的UI都要注册，否则无法显示
    private void AddUI(UIType _type, UIStyle _style, string _path)
    {        
        UIInfo uiInfo = new UIInfo();
        {
            uiInfo.type = _type;
            uiInfo.style = _style;
            uiInfo.path = _path;
        }

        m_UIDict.Add(_type, uiInfo);
    }

    #endregion

}
