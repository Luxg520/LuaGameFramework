/// <summary>
/// UI类型，UI的唯一标识
/// </summary>
public enum UIType
{
    LoginUI,    // 登录
}


/// <summary>
/// UI样式
/// 普通、全屏、模态等
/// </summary>
public enum UIStyle
{
    Simple, // 普通
    Full,   // 全屏
    Modal,  // 模态
}

/// <summary>
/// UI层级
/// </summary>
public enum UILayer
{
    Game,   // 游戏UI层
    Prepose,// 游戏前置层
    Guide,  // 游戏指引层
    Test,   // 游戏测试层
}

public class UIInfo
{
    public UIType type;     // UI类型
    public UIStyle style;   // UI样式
    public UILayer layer;   // UI层级
    public string path;     // UI路径
    public UIBase ui;       // UI基层实例
}