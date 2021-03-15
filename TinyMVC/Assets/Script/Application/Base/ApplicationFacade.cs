using SMVC.Interfaces;
public class ApplicationFacade
{
    public static IFacade instance;

    public static IFacade GetInstance()
    {
        return instance;
    }
}