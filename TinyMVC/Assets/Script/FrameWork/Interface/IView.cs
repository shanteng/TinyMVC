using System;
using System.Collections.Generic;


namespace SMVC.Interfaces
{
    public interface IView
    {
        void RegisterObserver(string notificationName, IObserver observer);
        void RemoveObserver(string notificationName, object notifyContext);
        void NotifyObservers(INotification notification);



        void RegisterMediator(IMediator mediator);

        IMediator RetrieveMediator(string mediatorName);

        IMediator RemoveMediator(string mediatorName);

        bool HasMediator(string mediatorName);
        IEnumerable<string> ListMediatorNames { get; }

    }
}
