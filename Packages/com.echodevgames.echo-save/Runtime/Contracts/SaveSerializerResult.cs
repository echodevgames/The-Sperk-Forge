
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Structured terminal result for serializer and serializer-registry work.
    /// </summary>
    public readonly struct SaveSerializerResult
    {
        public SaveSerializerResult(
            SaveSerializerStatus status,
            string diagnosticCode,
            string message)
        {
            Status = status;
            DiagnosticCode =
                diagnosticCode ?? string.Empty;
            Message =
                message ?? string.Empty;
        }

        public SaveSerializerStatus Status { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public bool Succeeded =>
            Status == SaveSerializerStatus.Succeeded;

        public static SaveSerializerResult Success(
            string message) =>
            new SaveSerializerResult(
                SaveSerializerStatus.Succeeded,
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
