using SMVC.Patterns;


public class StartupCommand : MacroCommand
{
    protected override void InitializeMacroCommand()
    {
        AddSubCommand(typeof(ModelsInitializeCommand));
        AddSubCommand(typeof(ViewsInitializeCommand));
        AddSubCommand(typeof(ControllersInitializeCommand));
    }
}