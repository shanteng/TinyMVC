using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SMVC.Interfaces;

namespace SMVC.Core
{
    public class Model : IModel
    {
        protected string m_multitonKey;
        protected IDictionary<string, IProxy> m_proxyMap;
        protected static IModel m_instance;
        public const string DEFAULT_KEY = "TinyMVC";
        protected const string MULTITON_MSG = "Model instance for this Multiton key already constructed!";

        public Model(string key)
        {
            m_multitonKey = key;
            m_proxyMap = new ConcurrentDictionary<string, IProxy>();
            if (null != m_instance)
                throw new Exception(MULTITON_MSG);
            m_instance = this;
            InitializeModel();
        }

        public static IModel Instance => GetInstance();
        public static IModel GetInstance()
        {
            return m_instance ?? (m_instance = new Model(DEFAULT_KEY));
        }

        public virtual void RegisterProxy(IProxy proxy)
        {
            proxy.InitializeNotifier(m_multitonKey);
            m_proxyMap[proxy.ProxyName] = proxy;

            proxy.OnRegister();
        }

        public virtual IProxy RetrieveProxy(string proxyName)
        {
            if (!m_proxyMap.ContainsKey(proxyName)) return null;

            return m_proxyMap[proxyName];
        }

        public virtual bool HasProxy(string proxyName)
        {
            return m_proxyMap.ContainsKey(proxyName);
        }

        public IEnumerable<string> ListProxyNames
        {
            get { return m_proxyMap.Keys; }
        }

        public virtual IProxy RemoveProxy(string proxyName)
        {
            IProxy proxy = null;

            if (m_proxyMap.ContainsKey(proxyName))
            {
                proxy = RetrieveProxy(proxyName);
                m_proxyMap.Remove(proxyName);
            }

            if (proxy != null) proxy.OnRemove();
            return proxy;
        }

        protected virtual void InitializeModel()
        {
        }

        public void Dispose()
        {
            m_proxyMap.Clear();
            m_instance = null;
        }
    }
}