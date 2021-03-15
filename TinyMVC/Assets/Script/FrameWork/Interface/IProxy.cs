namespace SMVC.Interfaces
{
    public interface IProxy : INotifier
    {
        /// <summary>
        /// The Proxy instance name
        /// </summary>
        string ProxyName { get; }

        /// <summary>
        /// The data of the proxy
        /// </summary>
        object Data { get; set; }

        /// <summary>
        /// Called by the Model when the Proxy is registered
        /// </summary>
        void OnRegister();

        /// <summary>
        /// Called by the Model when the Proxy is removed
        /// </summary>
        void OnRemove();
    }
}