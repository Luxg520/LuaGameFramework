using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// UI管理器
/// </summary>
public class UIManager : ManagerBase<UIManager>
{
    #region 公共函数

    // 所有已注册UI
    private Dictionary<UIType, UIInfo> m_UIDict = new Dictionary<UIType, UIInfo>();
    // 已加载UI
    private Dictionary<UIType, UIInfo> m_LoadedUI = new Dictionary<UIType, UIInfo>();

    // 显示UI界面（采用异步方式）
    public void ShowUI(UIType _type, UILayer _layer, params object[] _params)
    {
        // UI 是否已注册
        if (!m_UIDict.ContainsKey(_type))
            throw new Exception("ui has not register!");

        // 加载并显示UI        
        if (m_LoadedUI.ContainsKey(_type))
            ShowUI_Internal(m_LoadedUI[_type], _layer, _params);
        else
        {
            // 加载UI
            Instance.LoadAsyncUI(_type, (uiInfo) =>
            {
                ShowUI_Internal(uiInfo, _layer, _params);
            });
        }
    }

    // 显示UI界面内部函数
    private void ShowUI_Internal(UIInfo uiInfo, UILayer _layer, params object[] _params)
    {
        uiInfo.layer = _layer;
        UIBase ui = uiInfo.ui;

        // 显示UI
        Transform uiTrans = UIManager.Instance.GetUICanvas(_layer).transform;
        UIUtils.AddUIPrefab(uiTrans, ui.MyTrans);
        ui.gameObject.SetActive(true);

        // 初始化
        ui.Init(_params);
    }

    // 隐藏UI界面
    public UIBase HideUI(UIType _type, params object[] _params)
    {
        return null;
    }

    // 卸载UI界面
    public void ReleaseUI(UIType _type)
    {
        // UI 是否已注册
        if (!m_UIDict.ContainsKey(_type))
            throw new Exception("ui has not register!");

        if (m_LoadedUI.ContainsKey(_type))
        {
            UIInfo uiInfo = m_LoadedUI[_type];
            string uiPath = ResourceConfig.GetUIPath(uiInfo.path);
            ResourceManager.Instance.UnLoadAll(uiPath);
            m_LoadedUI.Remove(_type);
        }
    }

    // 获取UI实例
    public T GetUI<T>() where T : class
    {
        foreach (var uiInfo in m_UIDict.Values)
        {
            if (uiInfo.ui is T)
            {
                return uiInfo.ui as T;
            }
        }
        return null;
    }

    // 获取UI实例
    public UIBase GetUI(UIType _type)
    {
        if (m_UIDict.ContainsKey(_type))
        {
            return m_UIDict[_type].ui;
        }
        return null;
    }

    // 获取UI资源路径
    public string GetUIPath(UIBase _ui)
    {
        return GetUIPath(_ui);
    }

    // 获取UI资源路径
    public string GetUIPath(UIType _type)
    {
        UIInfo uiInfo;
        if (!m_UIDict.TryGetValue(_type, out uiInfo))
            return null;
        return uiInfo.path;
    }

    #endregion

    #region UI层级

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

    #region 内部函数

    // 初始化
    public override void Init()
    {
        base.Init();

		InitUIs();

        GameUICanvas = GameObject.Find("GameUICanvas").GetComponent<Canvas>();
        PreposeUICanvas = GameObject.Find("PreposeUICanvas").GetComponent<Canvas>();
        GuideUICanvas = GameObject.Find("GuideUICanvas").GetComponent<Canvas>();
        TestUICanvas = GameObject.Find("TestUICanvas").GetComponent<Canvas>();
	
    }

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

    // 加载UI
    private void LoadUI(UIType _type, Action<UIInfo> cb)
    {
        // 加载UI资源并实例化GameObject
        UIInfo uiInfo = m_UIDict[_type];
        string uiPath = ResourceConfig.GetUIPath(uiInfo.path);
        UnityEngine.Object obj = ResourceManager.Instance.Load(uiPath);
        GameObject go = (GameObject)Instantiate(obj);
        uiInfo.ui = go.GetComponent<UIBase>();
        uiInfo.ui.UIInfo = uiInfo;
                        
        // 缓存界面
        m_LoadedUI.Add(_type, uiInfo);

        // 回调
        if (cb != null)
            cb(uiInfo);
    }

    // 异步加载UI
    private void LoadAsyncUI(UIType _type, Action<UIInfo> cb)
    {
        // 加载UI资源并实例化GameObject
        UIInfo uiInfo = m_UIDict[_type];
        string uiPath = ResourceConfig.GetUIPath(uiInfo.path);
        ResourceManager.Instance.LoadAsync(uiPath,(obj)=> 
        {
            GameObject go = (GameObject)Instantiate(obj);
            uiInfo.ui = go.GetComponent<UIBase>();
            uiInfo.ui.UIInfo = uiInfo;

            // 缓存界面
            m_LoadedUI.Add(_type, uiInfo);

            // 回调
            if (cb != null)
                cb(uiInfo);
        });
    }

    #endregion

}
