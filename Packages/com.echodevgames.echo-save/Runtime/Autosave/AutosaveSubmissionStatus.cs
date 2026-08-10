namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Immediate public truth for one RequestAutosave submission.
    /// </summary>
    public enum AutosaveSubmissionStatus
    {
        Executed = 0,
        Pending = 1,
        Coalesced = 2,
        RejectedInvalidRequest = 3,
        RejectedServiceNotReady = 4,
        RejectedAdmissionClosed = 5,
        RejectedNoActiveSlot = 6,
        RejectedCanceled = 7
    }
}
