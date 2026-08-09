namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Package-local Chronicle lifecycle state.
    /// </summary>
    public enum EchoSaveServiceState
    {
        None = 0,
        AuthorityClaimed = 1,
        Initializing = 2,
        Ready = 3,
        Blocked = 4,
        ShuttingDown = 5,
        Shutdown = 6,
        RejectedDuplicate = 7
    }
}
