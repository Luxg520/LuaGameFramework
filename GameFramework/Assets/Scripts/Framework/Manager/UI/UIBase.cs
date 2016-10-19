using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// UI基类
/// 所有UI界面都继承此类
/// </summary>
public abstract class UIBase : MonoBehaviour
{

    #region 常用变量

    // UI信息    
    public UIInfo UIInfo;    

    // UI类型
    public UIType UIType
    {
        get { return UIInfo.type; }
    }

    // UI路径
    public string UIPath
    {
        get { return UIInfo.path; }
    }

    // UI样式
    public UIStyle UIStyle
    {
        get { return UIInfo.style; }
    }

    // UI层级
    public UILayer UILayer
    {
        get { return UIInfo.layer; }
    }

    // UI动画
    public UIAni UIAnimation;

    // UI状态
    private UIState m_UIState = UIState.None;
    public UIState UIState
    {
        get { return m_UIState; }
    }

    private GameObject m_MyGo = null;
    public GameObject MyGo
    {
        get
        {
            if (m_MyGo == null)
                m_MyGo = gameObject;
            return m_MyGo;
        }
    }

    private Transform m_MyTrans = null;
    public Transform MyTrans
    {
        get
        {
            if (m_MyTrans == null)
                m_MyTrans = transform;
            return m_MyTrans;
        }
    }

    private RectTransform m_MyRect = null;
    public RectTransform MyRect
    {
        get
        {
            if (m_MyRect == null)
                m_MyRect = GetComponent<RectTransform>();
            return m_MyRect;
        }
    }

    #endregion

    #region 事件

    // 界面初始化前事件
    public event Action OnInitStartEvent;

    // 界面显示前事件
    public event Action OnShowStartEvent;

    // 界面隐藏前事件
    public event Action OnHideStartEvent;

    // 界面卸载前事件
    public event Action OnReleaseStartEvent;

    // 界面初始化后事件
    public event Action OnInitEndEvent;

    // 界面显示后事件
    public event Action OnShowEndEvent;

    // 界面隐藏后事件
    public event Action OnHideEndEvent;

    // 界面卸载后事件
    public event Action OnReleaseEndEvent;

    #endregion

    #region 常用函数

    /// <summary>
    /// 显示界面
    /// </summary>
    /// <param name="_type">UI类型</param>
    /// <param name="_layer">UI显示层级</param>
    /// <param name="_params">相关参数</param>
    protected static void ShowUI(UIType _type, UILayer _layer, params object[] _params)
    {
        UIManager.Instance.ShowUI(_type, _layer, _params);
    }

    // 获取UI实例
    public static T GetInstance<T>() where T : UIBase
    {
        return UIManager.Instance.GetUI<T>();
    }

    // 切换UI状态
    public void ChangeState(UIState _state)
    {
        m_UIState = _state;
    }

    // 卸载该界面
    public void Release()
    {
        UIManager.Instance.ReleaseUI(this.UIType);
    }

    #endregion

    #region 公共基本函数
    /// <summary>
    /// 界面初始化阶段（主要用于逻辑处理）
    /// 每次开启界面都会调用一次
    /// 不要在这里面播放UI动画相关操作
    /// </summary>
    public virtual void Init(params object[] _params)
    {

    }

    /// <summary>
    /// 界面显示阶段（相当于Start）
    /// 可用于播放动画
    /// </summary>
    public virtual void OnShow()
    {

    }

    /// <summary>
    /// 每帧执行函数
    /// 尽量不用，非必要时才用
    /// </summary>
    protected virtual void OnUpdate(float _deltaTime)
    {

    }

    /// <summary>
    /// 当界面被卸载时
    /// </summary>
    protected virtual void OnRelease()
    {

    }

    #endregion

    #region 内部基本函数

    /// <summary>
    /// 暂时没什么用，屏蔽掉不让用
    /// </summary>
    private void Awake()
    {
    }

    /// <summary>
    /// 第一次运行时（内部函数）
    /// </summary>
    private void Start()
    {
    }

    /// <summary>
    /// 每帧运行（内部函数）
    /// </summary>
    private void Update()
    {
        OnUpdate(Time.deltaTime);
    }

    /// <summary>
    /// 当界面被销毁时
    /// </summary>
    private void OnDestroy()
    {

    }

    #endregion

    #region 音乐

    /// <summary>
    /// 当播放音乐时
    /// </summary>
    protected virtual void OnPlayAudio()
    {
    }

    /// <summary>
    /// 关闭音乐时
    /// </summary>
    protected virtual void OnCloseAudio()
    {
    }

    #endregion

    #region 动画

    public virtual void OnShowAni()
    {

    }

    public virtual void OnHideAni()
    {

    }

    #endregion
}
