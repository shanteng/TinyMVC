using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PlayerIdentity;
using UnityEngine.PlayerIdentity.UI;
using UnityEngine.CloudSave;

public class UnitySdkUI : MonoBehaviour
{
    public PlayerIdentityCore _sdkCore;
    private PanelController _panelController;
    private SignInPanel _signPanel;
    private AccountPanel _accoutPanel;
    public static UnitySdkUI Intance { get; private set; }

    //云存储
    [HideInInspector]
    public CloudSaveController _cloudSave;

    void Awake()
    {
        this._cloudSave = this.gameObject.GetComponent<CloudSaveController>();
        this._panelController = this.transform.Find("MainCanvas/Panels").GetComponent<PanelController>();
        this._signPanel = this.transform.Find("MainCanvas/Panels/Login/Sign In Panel").GetComponent<SignInPanel>();
        this._accoutPanel = this.transform.Find("MainCanvas/Panels/Account/Account Panel").GetComponent<AccountPanel>();
        Intance = this;
    }

    public void OnLogin()
    {
        //登录成功后隐藏Sdk
      //  UIRoot.Intance.SetSdkVisible(false);
        GameIndex.UserId = PlayerIdentityManager.Current.userId;
        this._cloudSave.InitCloud();
    }

    public void OnLogout()
    {
        //返回登录
        GameIndex.UserId = "";
        MediatorUtil.SendNotification(NotiDefine.GAME_RESET);
    }

}
