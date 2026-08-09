
namespace EchoDevGames.EchoSave
{
    public readonly struct SaveIntegrityResult
    {
        public SaveIntegrityResult(
            SaveIntegrityStatus status,
            string diagnosticCode,
            string message)
        {
            Status = status;
            DiagnosticCode =
                diagnosticCode ?? string.Empty;
            Message =
                message ?? string.Empty;
        }

        public SaveIntegrityStatus Status { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public bool Succeeded =>
            Status == SaveIntegrityStatus.Succeeded;

        public static SaveIntegrityResult Success(
            string message) =>
            new SaveIntegrityResult(
                SaveIntegrityStatus.Succeeded,
                string.Empty,
                message);
    }
}
