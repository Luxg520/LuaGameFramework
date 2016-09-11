using UnityEngine;
using System.Collections;

public class LoginUI : UIBase
{
    public static void ShowUI(int index)
    {
        ShowUI(UIType.LoginUI, UILayer.Game, index);
    }

    public override void Init(params object[] _params)
    {
        base.Init(_params);

        int index = (int)_params[0];

    }

    // 登录
    public void OnLogin()
    {
        Destroy(this.gameObject);
        Release();
    }

}
