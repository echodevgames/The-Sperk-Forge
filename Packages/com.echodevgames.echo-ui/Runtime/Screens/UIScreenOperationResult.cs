namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Structured terminal result for one Screen lifecycle request.
    /// </summary>
    public readonly struct UIScreenOperationResult
    {
        public UIScreenOperationResult(
            UIScreenOperationStatus status,
            UIScreenOperationKind kind,
            string screenId = "",
            string scopeId = "",
            long sequence = 0,
            string message = "")
        {
            Status = status;
            Kind = kind;
            ScreenId = screenId ?? string.Empty;
            ScopeId = scopeId ?? string.Empty;
            Sequence = sequence;
            Message = message ?? string.Empty;
        }

        public UIScreenOperationStatus Status { get; }

        public UIScreenOperationKind Kind { get; }

        public string ScreenId { get; }

        public string ScopeId { get; }

        public long Sequence { get; }

        public string Message { get; }

        public bool Succeeded =>
            Status == UIScreenOperationStatus.Succeeded;

        public bool IsTerminal =>
            true;

        public static UIScreenOperationResult Success(
            UIScreenOperationRequest request,
            string screenId,
            string scopeId,
            string message) =>
            new UIScreenOperationResult(
                UIScreenOperationStatus.Succeeded,
                request.Kind,
                screenId,
                scopeId,
                request.Sequence,
                message);
    }
}
