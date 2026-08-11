namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Result of opening one read-only Chronicle inspection session.
    /// </summary>
    public sealed class EchoSaveInspectionOpenResult
    {
        internal EchoSaveInspectionOpenResult(
            bool succeeded,
            bool rootPresent,
            string diagnosticCode,
            string message)
        {
            Succeeded = succeeded;
            RootPresent = rootPresent;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public bool Succeeded { get; }

        public bool RootPresent { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }
    }
}
