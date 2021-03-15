using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SMVC.Interfaces;
using SMVC.Patterns;

namespace SMVC.Core
{
    public class View : IView
    {

        protected string m_multitonKey;
        protected IDictionary<string, IMediator> m_mediatorMap;
        protected IDictionary<string, IList<IObserver>> m_observerMap;
        protected static IView m_instance;
        public const string DEFAULT_KEY = "TinyMVC";
        protected const string MULTITON_MSG = "View instance for this Multiton key already constructed!";

        protected View(string key)
        {
            m_multitonKey = key;
            m_mediatorMap = new ConcurrentDictionary<string, IMediator>();
            m_observerMap = new ConcurrentDictionary<string, IList<IObserver>>();
            if (null != m_instance)
                throw new Exception(MULTITON_MSG);
            m_instance = this;
            InitializeView();
        }

        public static IView Instance => GetInstance();
        public static IView GetInstance()
        {
            return m_instance ?? (m_instance = new View());
        }

        protected View() : this(DEFAULT_KEY) { }

        public virtual void RegisterObserver(string notificationName, IObserver observer)
        {
            IList<IObserver> list;
            if (!m_observerMap.TryGetValue(notificationName, out list))
            {
                list = new List<IObserver>();
                m_observerMap.Add(notificationName, list);
            }
            list.Add(observer);
        }

        public virtual void NotifyObservers(INotification notification)
        {
            //edit by wp
            IList<IObserver> observers = null;
            if (!m_observerMap.TryGetValue(notification.Name, out observers))
            {
                return;
            }
            var obs = observers.ToArray();
            int len = obs.Length;
            for (int i = 0; i < len; i++)
            {
                obs[i].NotifyObserver(notification);
            }
        }

        public virtual void RemoveObserver(string notificationName, object notifyContext)
        {
            if (!m_observerMap.ContainsKey(notificationName)) return;
            var observers = m_observerMap[notificationName];
            lock (observers)
            {
                // find the observer for the notifyContext
                int count = observers.Count;
                for (var i = 0; i < count; i++)
                {
                    if (!observers[i].CompareNotifyContext(notifyContext)) continue;
                    // there can only be one Observer for a given notifyContext 
                    // in any given Observer list, so remove it and break
                    observers.RemoveAt(i);
                    break;
                }

                // Also, when a Notification's Observer list length falls to 
                // zero, delete the notification key from the observer map
                if (observers.Count == 0)
                    m_observerMap.Remove(notificationName);
            }
        }

        public virtual void RegisterMediator(IMediator mediator)
        {
            lock (m_mediatorMap)
            {
                if (m_mediatorMap.ContainsKey(mediator.MediatorName)) return;
                mediator.InitializeNotifier(m_multitonKey);
                m_mediatorMap[mediator.MediatorName] = mediator;
                var interests = mediator.ListNotificationInterests;

                IObserver observer = new Observer(mediator.HandleNotification, mediator);
                foreach (var t in interests)
                {
                    RegisterObserver(t, observer);
                }
            }
            mediator.OnRegister();
        }

        public virtual IMediator RetrieveMediator(string mediatorName)
        {
            if (!m_mediatorMap.ContainsKey(mediatorName)) return null;
            return m_mediatorMap[mediatorName];
        }

        public virtual IMediator RemoveMediator(string mediatorName)
        {
            lock (m_mediatorMap)
            {
            
                if (!m_mediatorMap.ContainsKey(mediatorName)) return null;
                var mediator = m_mediatorMap[mediatorName];
                var interests = mediator.ListNotificationInterests;

                foreach (var t in interests)
                {
                    RemoveObserver(t, mediator);
                }

                m_mediatorMap.Remove(mediatorName);

                mediator.OnRemove();
                return mediator;
            }
        }

        public virtual bool HasMediator(string mediatorName)
        {
            return m_mediatorMap.ContainsKey(mediatorName);
        }

        public IEnumerable<string> ListMediatorNames
        {
            get { return m_mediatorMap.Keys; }
        }

        public void Dispose()
        {
            m_observerMap.Clear();
            m_mediatorMap.Clear();
            m_instance = null;
        }

        protected virtual void InitializeView()
        {
        }
    }//end class

}//end 