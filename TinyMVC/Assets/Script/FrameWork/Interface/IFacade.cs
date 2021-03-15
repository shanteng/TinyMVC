using System;

namespace SMVC.Interfaces
{
    public interface IFacade : INotifier
    {
        
        void RegisterProxy(IProxy proxy);

 
        IProxy RetrieveProxy(string proxyName);

    
        IProxy RemoveProxy(string proxyName);

   
        bool HasProxy(string proxyName);


        
        void RegisterCommand(string notificationName, ICommand command);

      
        void RegisterCommand(string notificationName, Type commandType);

      
        object RemoveCommand(string notificationName);

      
        bool HasCommand(string notificationName);

    
        void RegisterMediator(IMediator mediator);

     
        IMediator RetrieveMediator(string mediatorName);

      
        IMediator RemoveMediator(string mediatorName);

       
        bool HasMediator(string mediatorName);

      
        void NotifyObservers(INotification notification);

    }
}