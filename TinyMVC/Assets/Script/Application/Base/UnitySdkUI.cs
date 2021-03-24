using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PlayerIdentity;
using UnityEngine.PlayerIdentity.UI;

public class UnitySdkUI : MonoBehaviour
{
    public PlayerIdentityCore _sdkCore;
    private PanelController _panelController;
    private SignInPanel _signPanel;
    private AccountPanel _accoutPanel;
    public static UnitySdkUI Intance { get; private set; }
    
    void Awake()
    {
        this._panelController = this.transform.Find("MainCanvas/Panels").GetComponent<PanelController>();
        this._signPanel = this.transform.Find("MainCanvas/Panels/Login/Sign In Panel").GetComponent<SignInPanel>();
        this._accoutPanel = this.transform.Find("MainCanvas/Panels/Account/Account Panel").GetComponent<AccountPanel>();
        Intance = this;
    }

}
