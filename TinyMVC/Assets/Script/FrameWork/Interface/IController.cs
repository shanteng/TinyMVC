using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SMVC.Interfaces;
using SMVC.Patterns;

namespace SMVC.Interfaces
{
    public interface IController
    {
        void RegisterCommand(string notificationName, Type commandType);
        void RegisterCommand(string notificationName, ICommand command);
        void ExecuteCommand(INotification notification);
        object RemoveCommand(string notificationName);
        bool HasCommand(string notificationName);
        IEnumerable<string> ListNotificationNames { get; }
    }
}
