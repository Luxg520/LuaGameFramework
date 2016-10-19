using UnityEngine;
using System.Collections;
/// <summary>
/// Loading界面
/// </summary>
public class LoadingUI : UIBase
{
    public static void ShowUI()
    {
        UIBase.ShowUI(UIType.LoadingUI, UILayer.Prepose);
    }

    public override void Init(params object[] _params)
    {
        base.Init(_params);

    }

}
