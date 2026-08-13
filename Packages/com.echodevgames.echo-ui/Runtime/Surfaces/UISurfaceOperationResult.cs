namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Immutable result returned by Looking Glass surface operations.
    /// </summary>
    public readonly struct UISurfaceOperationResult
    {
        public UISurfaceOperationResult(
            UISurfaceOperationStatus status,
            string surfaceId = "",
            string scopeId = "",
            string message = "")
        {
            Status = status;
            SurfaceId = surfaceId ?? string.Empty;
            ScopeId = scopeId ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public UISurfaceOperationStatus Status { get; }

        public string SurfaceId { get; }

        public string ScopeId { get; }

        public string Message { get; }

        public bool Succeeded =>
            Status == UISurfaceOperationStatus.Succeeded;

        public static UISurfaceOperationResult Success(
            string surfaceId = "",
            string scopeId = "",
            string message = "") =>
            new UISurfaceOperationResult(
                UISurfaceOperationStatus.Succeeded,
                surfaceId,
                scopeId,
                message);
    }
}
