using System.Collections.Generic;
using SMVC.Core;
using SMVC.Patterns;

public class GameFacade : Facade
{
    private static GameFacade m_instance;

    protected GameFacade()
    {
        m_instance = this;
    }

    public static GameFacade instance
    {
        get
        {
            if (null == m_instance)
            {
                m_instance = new GameFacade();
            }
            return m_instance;
        }
    }

    public static void SendNotify(string notificationName, object body = null)
    {
        if (null == body)
        {
            instance.SendNotification(notificationName);
        }
        else
        {
            instance.SendNotification(notificationName, body);
        }
    }

    public IEnumerable<string> GetProxyList()
    {
        return this.m_model.ListProxyNames;
    }

    protected override void InitializeModel()
    {
        if (m_model != null) return;
        m_model = new Model("mvcModel");
    }

    protected override void InitializeController()
    {
        if (m_controller != null) return;
        m_controller = new Controller("MvcController");
    }



}