
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Main-thread capture result containing participant-owned detached state.
    ///
    /// The detached state must not be a live Unity object graph, scene object,
    /// or mutable shared ScriptableObject. M3-01 defines the contract only;
    /// later checkpoints own serialization/capture orchestration.
    /// </summary>
    public readonly struct SaveParticipantCaptureResult
    {
        public SaveParticipantCaptureResult(
            SaveParticipantCaptureStatus status,
            object detachedState,
            string diagnosticCode,
            string message)
        {
            Status = status;
            DetachedState = detachedState;
            DiagnosticCode =
                diagnosticCode ?? string.Empty;
            Message =
                message ?? string.Empty;
        }

        public SaveParticipantCaptureStatus Status
        {
            get;
        }

        public object DetachedState { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public bool Succeeded =>
            Status ==
            SaveParticipantCaptureStatus.Succeeded;

        public static SaveParticipantCaptureResult
            Success(
                object detachedState) =>
            new SaveParticipantCaptureResult(
                SaveParticipantCaptureStatus.Succeeded,
                detachedState,
                string.Empty,
                "The Chronicle participant captured detached state.");

        public static SaveParticipantCaptureResult
            Failure(
                string message,
                string diagnosticCode = null) =>
            new SaveParticipantCaptureResult(
                SaveParticipantCaptureStatus.Failed,
                null,
                string.IsNullOrEmpty(
                    diagnosticCode)
                    ? EchoSaveDiagnosticCodes
                        .ParticipantCaptureFailed
                    : diagnosticCode,
                message);
    }
}
