namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Single package-local M4-05 pending autosave slot.
    ///
    /// This is deliberately one request, never a list or generic queue.
    /// </summary>
    internal sealed class PendingAutosaveRequest
    {
        internal PendingAutosaveRequest(
            AutosaveRequest request,
            AutosaveTicket ticket)
        {
            Request =
                request;

            Ticket =
                ticket;
        }

        internal AutosaveRequest Request { get; }

        internal AutosaveTicket Ticket { get; }
    }
}
