namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Deterministic policy used only when a channel's pending bound is full.
    /// Visible entries are never preempted by these policies.
    /// </summary>
    public enum UINotificationOverflowPolicy
    {
        RejectNewest = 0,
        DropOldestPending = 1,
        ReplaceLowestPriorityPending = 2
    }

    public enum UINotificationLifetimeMode
    {
        Automatic = 0,
        Manual = 1
    }
}
