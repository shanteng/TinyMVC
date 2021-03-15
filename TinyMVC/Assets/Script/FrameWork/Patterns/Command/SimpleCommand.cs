using SMVC.Interfaces;

namespace SMVC.Patterns
{
    public class SimpleCommand : Notifier, ICommand
    {  

        public virtual void Execute(INotification notification)
        {
        }

    }
}
