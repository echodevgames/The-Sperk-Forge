
namespace EchoDevGames.EchoSave
{
    public readonly struct PreparedLoadCreationResult
    {
        internal PreparedLoadCreationResult(
            PreparedLoadCreationStatus status,
            PreparedSaveLoad handle,
            string diagnosticCode,
            string message)
        {
            Status = status;
            Handle = handle;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public PreparedLoadCreationStatus Status { get; }

        public PreparedSaveLoad Handle { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public bool Succeeded =>
            Status == PreparedLoadCreationStatus.Succeeded &&
            Handle != null;
    }
}
