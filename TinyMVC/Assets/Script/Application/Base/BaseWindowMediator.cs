using SMVC.Interfaces;
using SMVC.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//控制UI显示的Mediator基类
public abstract class BaseWindowMediator<T> : Mediator
{
    protected object ShowData;
    protected WindowLayer m_eWindowLayer;
    protected WindowState m_eWindowState;
    protected List<string> m_lInterestNotifications;
    
    protected string m_sShowNoity;
    protected string m_sHideNoity;
    protected bool DestroyWhenHide = true;//默认关闭就销毁
    protected T m_viewScript;
    protected GameObject m_view;
    protected string _prefabName = "";

    public bool windowVisible => (this.m_eWindowState == WindowState.SHOW);

    protected BaseWindowMediator(MediatorDefine mediatorName, WindowLayer layer)
    {
        this.m_mediatorName = MediatorUtil.GetName(mediatorName);
        this._prefabName = typeof(T).ToString();
        m_eWindowLayer = layer;
        m_eWindowState = WindowState.UNINIT;
        m_sShowNoity = $"{NotiDefine.WINDOW_DO_SHOW}_{mediatorName}";
        m_sHideNoity = $"{NotiDefine.WINDOW_DO_HIDE}_{mediatorName}";
    }

    string viewResource()
    {
        return this._prefabName;
    }

    public override IEnumerable<string> ListNotificationInterests
    {
        get
        {
            if (null == m_lInterestNotifications)
            {
                InitListNotificationInterests();
            }
            return m_lInterestNotifications;
        }
    }

    public void InitListNotificationInterests()
    {
        m_lInterestNotifications = new List<string>();
        m_lInterestNotifications.Add(m_sShowNoity);
        m_lInterestNotifications.Add(m_sHideNoity);
        InitListNotificationInterestsInner();
    }

    public override void HandleNotification(INotification notification)
    {
        if (notification.Name.Equals(m_sShowNoity))
        {
            ShowData = notification.Body;
            ShowWindow();
        }
        else if (notification.Name.Equals(m_sHideNoity))
        {
            HideWindow();
        }
        else
        {
            this.HandleNotificationInner(notification);
        }
    }

    protected virtual void HideWindow()
    {
        if (this.m_viewComponent == null)
            return;
        if ((m_eWindowState == WindowState.HIDE || m_eWindowState == WindowState.HIDEING_WAIT) && this.m_view.activeSelf == false)
            return;

        m_eWindowState = WindowState.HIDE;
        this.m_view.SetActive(false);
        this.hideWindowInner();

        if (this.DestroyWhenHide)
            DestroyWindow();
    }

    protected void DestroyWindow()
    {
        if (this.m_viewComponent == null)
            return;
        if (windowVisible)
            hideWindowInner();

        this.m_eWindowState = WindowState.UNINIT;
        GameObject.Destroy(m_view);
        this.m_view = null;
        this.m_viewComponent = null;
        DestroyWindowInner();
    }

    protected void ShowWindow()
    {
        if (WindowState.SHOWING_WAIT == m_eWindowState)
            return;
        m_eWindowState = WindowState.SHOWING_WAIT;
        if (null == m_viewComponent)
        {
            LoadViewComponent();
        }
        else
        {
            DoInitialize();
        }
        this.SendNotification(NotiDefine.WINDOW_HAS_SHOW, this.MediatorName);

    }

    public T GetViewScript()
    {
        return this.m_viewScript;
    }

    protected void LoadViewComponent()
    {
        GameObject obj = ResourcesManager.Instance.LoadUIRes(viewResource());
        if (null == obj)
            return;

        this.m_view = UIRoot.Intance.InstantiateUIInCenter(obj, this.m_eWindowLayer, true, false);
        m_viewScript = m_view.GetComponent<T>();
        InitViewComponent(m_view);
        DoInitialize();
    }

    protected virtual void DoInitialize()
    {
        this.SetAsLastSibling();
        m_eWindowState = WindowState.SHOW;
        this.DoInitializeInner();
    }

    public void SetAsLastSibling()
    {
        this.m_view.transform.SetAsLastSibling();
    }

    #region VirtualFunction
    protected virtual void InitListNotificationInterestsInner() { }
    protected virtual void HandleNotificationInner(INotification notification) { }

    protected abstract void InitViewComponent(GameObject view);
    protected virtual void DoInitializeInner() { }

    protected virtual void hideWindowInner() { }

    protected virtual void DestroyWindowInner() { }
    #endregion
}//end class

