
namespace EchoDevGames.EchoSave
{
    public readonly struct SaveDocumentValidationResult
    {
        public SaveDocumentValidationResult(
            SaveDocumentValidationStatus status,
            string diagnosticCode,
            string message)
        {
            Status = status;
            DiagnosticCode =
                diagnosticCode ?? string.Empty;
            Message =
                message ?? string.Empty;
        }

        public SaveDocumentValidationStatus Status { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public bool Succeeded =>
            Status ==
            SaveDocumentValidationStatus.Succeeded;

        public static SaveDocumentValidationResult
            Success(
                string message) =>
            new SaveDocumentValidationResult(
                SaveDocumentValidationStatus.Succeeded,
                string.Empty,
                message);
    }
}
