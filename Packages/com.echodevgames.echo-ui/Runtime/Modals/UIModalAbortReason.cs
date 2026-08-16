namespace EchoDevGames.EchoUI
{
    public enum UIModalAbortReason
    {
        None = 0,
        ExplicitAbort = 1,
        OwnerLost = 2,
        ViewLost = 3,
        RootShutdown = 4,
        TransitionFailed = 5
    }
}
