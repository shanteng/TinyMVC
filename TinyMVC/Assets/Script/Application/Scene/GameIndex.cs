using System;
using System.Collections;
using SMVC.Patterns;
using UnityEngine;

public class GameIndex : MonoBehaviour
{
    
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        //初始化MVC
        ApplicationFacade.instance = GameFacade.instance;
        this.InitMvc();
    }

    private void InitMvc()
    {
        ApplicationFacade.instance.RegisterCommand(NotiDefine.APP_START_UP, new StartupCommand());
        ApplicationFacade.instance.SendNotification(NotiDefine.APP_START_UP);
        ApplicationFacade.instance.SendNotification(NotiDefine.MVC_STARTED);
        StartCoroutine(InitializeGame());
    }

    private IEnumerator InitializeGame()
    {
        WaitForEndOfFrame waitYield = new WaitForEndOfFrame();
        while (UIRoot.Intance == null)
        {
            yield return waitYield;
        }
        ShowLogin();
    }



    private void Update()
    {
      
    }

    private void ShowLogin()
    {
        //加载第一个UI
        MediatorUtil.ShowMediator(MediatorDefine.TEST);
    }
}