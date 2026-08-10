namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Current bounded state of one autosave ticket.
    /// </summary>
    public enum AutosaveTicketState
    {
        Pending = 0,
        Executing = 1,
        Succeeded = 2,
        Failed = 3,
        Canceled = 4,
        Superseded = 5,
        Discarded = 6
    }
}
