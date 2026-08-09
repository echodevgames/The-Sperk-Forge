
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Structured result for primitive Chronicle storage operations.
    /// </summary>
    public readonly struct SaveStorageResult
    {
        public SaveStorageResult(
            SaveStorageStatus status,
            string diagnosticCode,
            string message)
        {
            Status = status;
            DiagnosticCode =
                diagnosticCode ?? string.Empty;
            Message =
                message ?? string.Empty;
        }

        public SaveStorageStatus Status { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public bool Succeeded =>
            Status == SaveStorageStatus.Succeeded ||
            Status == SaveStorageStatus.NoChange;

        public static SaveStorageResult Success(
            string message) =>
            new SaveStorageResult(
                SaveStorageStatus.Succeeded,
                string.Empty,
                message);

        public static SaveStorageResult NoChange(
            string message) =>
            new SaveStorageResult(
                SaveStorageStatus.NoChange,
                string.Empty,
                message);

        public override string ToString()
        {
            if (DiagnosticCode.Length == 0)
            {
                return $"{Status}: {Message}";
            }

            return
                $"[{DiagnosticCode}] {Status}: {Message}";
        }
    }
}
