namespace SMVC.Interfaces
{
    public interface ICommand : INotifier
    {
        void Execute(INotification notification);
    }
}