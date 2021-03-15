
using SMVC.Interfaces;
using SMVC.Patterns;
public class ViewsInitializeCommand : SimpleCommand
{
    public override void Execute(INotification notification)
    {
        //应用层的UI Mediator 注册
        //Facade.RegisterMediator(new DataCenterMediator());
    }
}