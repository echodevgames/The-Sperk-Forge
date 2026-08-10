namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Immediate result of submitting one explicit autosave request.
    /// </summary>
    public readonly struct AutosaveSubmissionResult
    {
        internal AutosaveSubmissionResult(
            AutosaveSubmissionStatus status,
            string diagnosticCode,
            string message,
            AutosaveTicket ticket,
            AutosaveTicket supersededTicket,
            bool hasSaveResult,
            SaveOperationResult saveResult)
        {
            Status =
                status;

            DiagnosticCode =
                diagnosticCode ??
                string.Empty;

            Message =
                message ??
                string.Empty;

            Ticket =
                ticket;

            SupersededTicket =
                supersededTicket;

            HasSaveResult =
                hasSaveResult;

            SaveResult =
                saveResult;
        }

        public AutosaveSubmissionStatus Status { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public AutosaveTicket Ticket { get; }

        public AutosaveTicket SupersededTicket { get; }

        public bool HasSaveResult { get; }

        public SaveOperationResult SaveResult { get; }

        public bool Accepted =>
            Status ==
                AutosaveSubmissionStatus.Executed ||
            Status ==
                AutosaveSubmissionStatus.Pending ||
            Status ==
                AutosaveSubmissionStatus.Coalesced;
    }
}
