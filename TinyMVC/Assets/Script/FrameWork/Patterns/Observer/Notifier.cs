using System;
using SMVC.Interfaces;

namespace SMVC.Patterns
{
    public class Notifier : INotifier
    {


       
        public virtual void SendNotification(string notificationName)
        {
            // The Facade SendNotification is thread safe, therefore this method is thread safe.
            Facade.SendNotification(notificationName);
        }

        public virtual void SendNotification(string notificationName, object body)
        {
            // The Facade SendNotification is thread safe, therefore this method is thread safe.
            Facade.SendNotification(notificationName, body);
        }

        public void InitializeNotifier()
        {
              
        }

        protected IFacade Facade
        {
            get
            {
                return Patterns.Facade.GetInstance();
            }
        }

        public void InitializeNotifier(string key)
        {
            MultitonKey = key;
        }

        public string MultitonKey { get; protected set; }
    }
}