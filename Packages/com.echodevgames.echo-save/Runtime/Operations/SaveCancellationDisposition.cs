namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Public cancellation truth for one manual-save request.
    /// </summary>
    public enum SaveCancellationDisposition
    {
        None = 0,
        Canceled = 1,
        TooLate = 2
    }
}
