
public enum WindowLayer
{
    Main = 1,
    FullScreen,
    Window,
    Popup,
    Tips,
    Sdk,
    Count
}

public enum WindowState
{
    UNINIT = 0,
    SHOW,
    HIDE,
    SHOWING_WAIT,
    HIDEING_WAIT,
}

public enum MediatorDefine
{
    TEST,
}

public class NotiDefine
{
    public const string APP_START_UP = "APP_START_UP";
    public const string MVC_STARTED = "MVC_STARTED";

    public const string WINDOW_DO_SHOW = "WINDOW_DO_SHOW";
    public const string WINDOW_DO_HIDE = "WINDOW_DO_HIDE";

    public const string WINDOW_HAS_SHOW = "WINDOW_HAS_SHOW";
}

public class MediatorUtil
{
    public static void ShowMediator(MediatorDefine mediatorName, object param = null)
    {
        string name = MediatorUtil.GetName(mediatorName);
        var noti = $"{NotiDefine.WINDOW_DO_SHOW}_{name}";
        SendNotification(noti, param);
    }

    public static void HideMediator(MediatorDefine mediatorName)
    {
        string name = MediatorUtil.GetName(mediatorName);
        var noti = $"{NotiDefine.WINDOW_DO_HIDE}_{name}";
        SendNotification(noti);
    }

    public static void SendNotification(string notify, object obj = null)
    {
        ApplicationFacade.instance.SendNotification(notify, obj);
    }

    public static string GetName(MediatorDefine me)
    {
        return System.Enum.GetName(typeof(MediatorDefine), me);
    }

}