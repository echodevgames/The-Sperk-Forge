
namespace EchoDevGames.EchoSave
{
    public readonly struct SaveParticipantApplyResult
    {
        public SaveParticipantApplyResult(
            SaveParticipantApplyStatus status,
            string diagnosticCode,
            string message)
        {
            Status = status;
            DiagnosticCode =
                diagnosticCode ?? string.Empty;
            Message =
                message ?? string.Empty;
        }

        public SaveParticipantApplyStatus Status { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public bool Succeeded =>
            Status ==
            SaveParticipantApplyStatus.Succeeded;

        public static SaveParticipantApplyResult
            Success() =>
            new SaveParticipantApplyResult(
                SaveParticipantApplyStatus.Succeeded,
                string.Empty,
                "The Chronicle participant applied detached state.");

        public static SaveParticipantApplyResult
            Failure(
                string message,
                string diagnosticCode = null) =>
            new SaveParticipantApplyResult(
                SaveParticipantApplyStatus.Failed,
                string.IsNullOrEmpty(
                    diagnosticCode)
                    ? EchoSaveDiagnosticCodes
                        .ParticipantApplyFailed
                    : diagnosticCode,
                message);
    }
}
