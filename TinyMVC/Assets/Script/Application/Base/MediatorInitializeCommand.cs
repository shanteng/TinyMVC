
using SMVC.Interfaces;
using SMVC.Patterns;
public class ViewsInitializeCommand : SimpleCommand
{
    public override void Execute(INotification notification)
    {
        //UI Mediator 注册
        Facade.RegisterMediator(new TestUIMediator());
    }
}