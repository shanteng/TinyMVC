using SMVC.Interfaces;
using SMVC.Patterns;


public class TestCommand : SimpleCommand
{
    public override void Execute(INotification notification)
    {
        TestProxy proxy = Facade.RetrieveProxy(ProxyNameDefine.TEST) as TestProxy;
        switch (notification.Name)
        {
            case NotiDefine.TEST_NOTI:
                {
                    proxy.OnReceiveNoti((string)notification.Body);
                    break;
                }
        }
    }//end func
}