using UnityEngine;
using System.Collections;

public class LoginUI : UIBase
{
    public void ShowUI(int index)
    {
        ShowUI(UIType.LoginUI, UILayer.Game);
    }

    public override void Init(params object[] _params)
    {
        base.Init(_params);

        int index = (int)_params[0];

    }

}
