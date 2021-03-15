using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SMVC.Interfaces;
using SMVC.Patterns;

namespace SMVC.Core
{
    public class Controller : IController
    {
        protected string m_multitonKey;
        private IView m_view;
        private readonly IDictionary<string, object> m_commandMap;
        protected static IController _instance;
        public const string DEFAULT_KEY = "TinyMVC";
        protected const string MULTITON_MSG = "Model instance for this Multiton key already constructed!";

        public static IController Instance => GetInstance();
        public Controller(string key)
        {
            m_multitonKey = key;
            m_commandMap = new ConcurrentDictionary<string, object>();
            if (null != _instance)
                throw new Exception(MULTITON_MSG);
            _instance = this;
            InitializeController();
        }

        private void InitializeController()
        {
            m_view = View.GetInstance();
        }

        public IEnumerable<string> ListNotificationNames
        {
            get { return m_commandMap.Keys; }
        }

        public void Dispose()
        {
            _instance = null;
            m_commandMap.Clear();
        }

        public void ExecuteCommand(INotification notification)
        {
            if (!m_commandMap.ContainsKey(notification.Name)) return;
            var commandReference = m_commandMap[notification.Name];

            ICommand command;
            var commandType = commandReference as Type;
            if (commandType != null)
            {
                var commandInstance = Activator.CreateInstance(commandType);
                command = commandInstance as ICommand;
                if (command == null)
                    return;
            }
            else
            {
                command = commandReference as ICommand;
                if (command == null) return;
            }
            command.InitializeNotifier(m_multitonKey);
            command.Execute(notification);
        }

        public void RegisterCommand(string notificationName, Type commandType)
        {
            if (!m_commandMap.ContainsKey(notificationName))
            {
                // This call needs to be monitored carefully. Have to make sure that RegisterObserver
                // doesn't call back into the controller, or a dead lock could happen.
                m_view.RegisterObserver(notificationName, new Observer(ExecuteCommand, this));
            }

            m_commandMap[notificationName] = commandType;
        }

        public void RegisterCommand(string notificationName, ICommand command)
        {
            if (!m_commandMap.ContainsKey(notificationName))
            {
                // This call needs to be monitored carefully. Have to make sure that RegisterObserver
                // doesn't call back into the controller, or a dead lock could happen.
                m_view.RegisterObserver(notificationName, new Observer(ExecuteCommand, this));
            }
            command.InitializeNotifier(m_multitonKey);
            m_commandMap[notificationName] = command;
        }

        public bool HasCommand(string notificationName)
        {
            return m_commandMap.ContainsKey(notificationName);
        }

        public object RemoveCommand(string notificationName)
        {
            if (!m_commandMap.ContainsKey(notificationName)) return null;
            // remove the observer

            // This call needs to be monitored carefully. Have to make sure that RemoveObserver
            // doesn't call back into the controller, or a dead lock could happen.
            m_view.RemoveObserver(notificationName, this);
            var command = m_commandMap[notificationName];
            m_commandMap.Remove(notificationName);
            return command;
        }


        public static IController GetInstance()
        {
            return _instance ?? (_instance = new Controller(DEFAULT_KEY));
        }

    }//end class
}//end namespace
