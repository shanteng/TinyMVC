using SMVC.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUIMediator : BaseWindowMediator<TestUIView>
{
    public TestUIMediator() : base(MediatorDefine.TEST, WindowLayer.Window)
    {

    }

    protected override void InitListNotificationInterestsInner()
    {
        m_lInterestNotifications.Add(NotiDefine.TEST_NOTI);
    }

    protected override void HandleNotificationInner(INotification notification)
    {
        switch (notification.Name)
        {
            case NotiDefine.TEST_NOTI:
                {
                    if (this.windowVisible)
                    {
                        this.m_viewScript.OnReceiveNoti((string)notification.Body);
                    }
                    break;
                }
        }
    }//end func

    protected override void InitViewComponent(GameObject view)
    {

    }

    protected override void DoInitializeInner()
    {
        this.m_viewScript.InitData();
    }

    protected override void hideWindowInner()
    {

    }
}
