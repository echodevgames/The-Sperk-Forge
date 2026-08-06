namespace EchoDevGames.EchoLaunch.Editor.Validation
{
    internal enum EchoLaunchValidationSeverity
    {
        Information = 0,
        Warning = 1,
        Error = 2,
        Blocker = 3
    }

    internal enum EchoLaunchProjectHealth
    {
        Healthy = 0,
        NeedsAttention = 1,
        Invalid = 2,
        Blocked = 3
    }
}
