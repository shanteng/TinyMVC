namespace SMVC.Interfaces
{
    public delegate void HandleNotification(INotification notification);
    public interface IObserver
    {

        object NotifyContext { set; }

        HandleNotification NotifyMethod { set; }

      
        void NotifyObserver(INotification notification);

       
        bool CompareNotifyContext(object obj);
    }
}