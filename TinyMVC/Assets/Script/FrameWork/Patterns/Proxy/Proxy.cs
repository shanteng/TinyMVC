using SMVC.Interfaces;

namespace SMVC.Patterns
{
    public class Proxy : Notifier, IProxy
    {
        public static string NAME = "Proxy";

      
        public Proxy()
            : this(NAME, null)
        {
        }

      
        public Proxy(string proxyName)
            : this(proxyName, null)
        {
        }

    
        public Proxy(string proxyName, object data)
        {
            ProxyName = proxyName ?? NAME;
            if (data != null) Data = data;
        }


        public virtual void OnRegister()
        {
        }

     
        public virtual void OnRemove()
        {
        }


      
        public string ProxyName { get; protected set; }

        public object Data { get; set; }

    }
}
