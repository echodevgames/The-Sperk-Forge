namespace EchoDevGames.EchoUI
{
    public enum UIModalCompletionStatus
    {
        Succeeded = 0,
        AlreadyCompleted = 1,
        StaleHandle = 2,
        InvalidResult = 3,
        NotFound = 4,
        BackDisabled = 5,
        NotReady = 6
    }

    /// <summary>
    /// Result of attempting to settle or dismiss one modal generation.
    /// </summary>
    public readonly struct UIModalCompletionAttemptResult
    {
        public UIModalCompletionAttemptResult(
            UIModalCompletionStatus status,
            string message = "")
        {
            Status = status;
            Message = message ?? string.Empty;
        }

        public UIModalCompletionStatus Status { get; }

        public string Message { get; }

        public bool Succeeded =>
            Status == UIModalCompletionStatus.Succeeded;
    }
}
