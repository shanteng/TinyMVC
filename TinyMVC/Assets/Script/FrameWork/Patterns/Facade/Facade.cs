using System;
using SMVC.Core;
using SMVC.Interfaces;


namespace SMVC.Patterns
{
    public class Facade : Notifier, IFacade
    {
        protected IController m_controller;
        protected IModel m_model;
        protected IView m_view;
        protected static IFacade _instance;

        public Facade()
        {
            if (null != _instance)
                throw new Exception("facade exist");
            InitializeNotifier();
            _instance = this;
            InitializeFacade();
        }

        public void Dispose()
        {
            m_view = null;
            m_model = null;
            m_controller = null;
            _instance = null;
        }

        public void RegisterProxy(IProxy proxy)
        {
            // The model is initialized in the constructor of the singleton, so this call should be thread safe.
            // This method is thread safe on the model.
            m_model.RegisterProxy(proxy);
        }


        public IProxy RetrieveProxy(string proxyName)
        {
            // The model is initialized in the constructor of the singleton, so this call should be thread safe.
            // This method is thread safe on the model.
            return m_model.RetrieveProxy(proxyName);
        }

    
        public IProxy RemoveProxy(string proxyName)
        {
            // The model is initialized in the constructor of the singleton, so this call should be thread safe.
            // This method is thread safe on the model.
            return m_model.RemoveProxy(proxyName);
        }

     
        public bool HasProxy(string proxyName)
        {
            // The model is initialized in the constructor of the singleton, so this call should be thread safe.
            // This method is thread safe on the model.
            return m_model.HasProxy(proxyName);
        }

       


     
        public void RegisterCommand(string notificationName, Type commandType)
        {
            // The controller is initialized in the constructor of the singleton, so this call should be thread safe.
            // This method is thread safe on the controller.
            m_controller.RegisterCommand(notificationName, commandType);
        }

     
        public void RegisterCommand(string notificationName, ICommand command)
        {
            m_controller.RegisterCommand(notificationName, command);
        }

     
        public object RemoveCommand(string notificationName)
        {
            // The controller is initialized in the constructor of the singleton, so this call should be thread safe.
            // This method is thread safe on the controller.
            return m_controller.RemoveCommand(notificationName);
        }

      
        public bool HasCommand(string notificationName)
        {
            // The controller is initialized in the constructor of the singleton, so this call should be thread safe.
            // This method is thread safe on the controller.
            return m_controller.HasCommand(notificationName);
        }

    




       
        public void RegisterMediator(IMediator mediator)
        {
            if (null == mediator)
            {
                return;
            }
            // The view is initialized in the constructor of the singleton, so this call should be thread safe.
            // This method is thread safe on the view.
            m_view.RegisterMediator(mediator);
        }

    
        public IMediator RetrieveMediator(string mediatorName)
        {
            // The view is initialized in the constructor of the singleton, so this call should be thread safe.
            // This method is thread safe on the view.
            return m_view.RetrieveMediator(mediatorName);
        }

       
        public IMediator RemoveMediator(string mediatorName)
        {
            // The view is initialized in the constructor of the singleton, so this call should be thread safe.
            // This method is thread safe on the view.
            return m_view.RemoveMediator(mediatorName);
        }

        public bool HasMediator(string mediatorName)
        {
            // The view is initialized in the constructor of the singleton, so this call should be thread safe.
            // This method is thread safe on the view.
            return m_view.HasMediator(mediatorName);
        }


        public void NotifyObservers(INotification notification)
        {
            // The view is initialized in the constructor of the singleton, so this call should be thread safe.
            // This method is thread safe on the view.
            m_view.NotifyObservers(notification);
        }

     

        public override void SendNotification(string notificationName)
        {
            NotifyObservers(new Notification(notificationName));
        }

      
        public override void SendNotification(string notificationName, object body)
        {
            NotifyObservers(new Notification(notificationName, body));
        }



        public static IFacade Instance => _instance;

       
        public static IFacade GetInstance()
        {
            return _instance;
        }

        public static void SendNoti(string notificationName, object body)
        {
            Instance.SendNotification(notificationName, body);
        }

        protected virtual void InitializeFacade()
        {
            InitializeModel();
            InitializeController();
            InitializeView();
        }

        protected virtual void InitializeController()
        {
            if (m_controller != null) return;
            m_controller = Controller.GetInstance();
        }
        protected virtual void InitializeModel()
        {
            if (m_model != null) return;
            m_model = Model.GetInstance();
        }

        protected virtual void InitializeView()
        {
            if (m_view != null) return;
            m_view = View.GetInstance();
        }

       
     
      

    }
}