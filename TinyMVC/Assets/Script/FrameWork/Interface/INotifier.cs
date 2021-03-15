namespace SMVC.Interfaces
{
    public interface INotifier
    {
        void SendNotification(string notificationName);
        void SendNotification(string notificationName, object body);

        void InitializeNotifier(string key);

        string MultitonKey { get; }
    }
}