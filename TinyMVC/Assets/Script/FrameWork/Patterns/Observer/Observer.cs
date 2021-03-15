using System.Reflection;
using SMVC.Interfaces;

namespace SMVC.Patterns
{
    public class Observer : IObserver
    {


        public Observer(HandleNotification notifyMethod, object notifyContext)
        {
            NotifyMethod = notifyMethod;
            NotifyContext = notifyContext;
        }




        public void NotifyObserver(INotification notification)
        {
            NotifyMethod?.Invoke(notification);
        }

        public bool CompareNotifyContext(object obj)
        {
            lock (this)
            {
                // Compare on the current state
                return NotifyContext.Equals(obj);
                //return object.Equals(NotifyContext, obj);
            }
        }

        public HandleNotification NotifyMethod { private get; set; }
        public object NotifyContext { private get; set; }

    }
}