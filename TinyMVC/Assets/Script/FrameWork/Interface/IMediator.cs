using System.Collections.Generic;

namespace SMVC.Interfaces
{
    public interface IMediator : INotifier
    {
        string MediatorName { get; }

        object ViewComponent { get; set; }

        IEnumerable<string> ListNotificationInterests { get; }

        void HandleNotification(INotification notification);

        void OnRegister();
        void OnRemove();
    }
}