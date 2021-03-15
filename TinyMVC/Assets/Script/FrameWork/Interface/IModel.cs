using System.Collections.Generic;

namespace SMVC.Interfaces
{
    public interface IModel
    {
        /// <summary>
        /// Register an <c>IProxy</c> instance with the <c>Model</c>
        /// </summary>
        /// <param name="proxy">A reference to the proxy object to be held by the <c>Model</c></param>
        void RegisterProxy(IProxy proxy);

        /// <summary>
        /// Retrieve an <c>IProxy</c> instance from the Model
        /// </summary>
        /// <param name="proxyName">The name of the proxy to retrieve</param>
        /// <returns>The <c>IProxy</c> instance previously registered with the given <c>proxyName</c></returns>
        IProxy RetrieveProxy(string proxyName);

        /// <summary>
        /// Remove an <c>IProxy</c> instance from the Model
        /// </summary>
        /// <param name="proxyName">The name of the <c>IProxy</c> instance to be removed</param>
        IProxy RemoveProxy(string proxyName);

        /// <summary>
        /// Check if a Proxy is registered
        /// </summary>
        /// <param name="proxyName">The name of the proxy to check for</param>
        /// <returns>whether a Proxy is currently registered with the given <c>proxyName</c>.</returns>
        bool HasProxy(string proxyName);

        /// <summary>
        /// List all proxy name
        /// </summary>
        IEnumerable<string> ListProxyNames { get; }
    }
}