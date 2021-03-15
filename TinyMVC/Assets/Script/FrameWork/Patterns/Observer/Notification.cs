using System;
using SMVC.Interfaces;

namespace SMVC.Patterns
{
    public class Notification : INotification
    {
        public Notification(string name)
         : this(name, null, null)
        { }

        public Notification(string name, object body)
         : this(name, body, null)
        { }

        public Notification(string name, object body, string type)
        {
            Name = name;
            Body = body;
            Type = type;
        }

        public override string ToString()
        {
            var msg = "Notification Name: " + Name;
            msg += Environment.NewLine + "Body:" + ((Body == null) ? "null" : Body.ToString());
            msg += Environment.NewLine + "Type:" + (Type ?? "null");
            return msg;
        }

        public string Name { get; private set; }

        public object Body { get; set; }

        public string Type { get; set; }
    }
}