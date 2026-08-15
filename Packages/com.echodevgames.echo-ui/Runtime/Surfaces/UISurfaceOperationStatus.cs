namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Describes the terminal result of one Looking Glass surface operation.
    /// </summary>
    public enum UISurfaceOperationStatus
    {
        Succeeded = 0,
        NotAuthoritative = 1,
        NotInitialized = 2,
        AlreadyInitialized = 3,
        InvalidDefinition = 4,
        DuplicateSurfaceId = 5,
        InitialScopeConflict = 6,
        UnknownSurface = 7,
        WrongSurfaceRole = 8,
        NoHistory = 9,
        BlockedByModal = 10
    }
}
