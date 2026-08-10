namespace EchoDevGames.EchoSave
{
    internal sealed class SaveParticipantPreparationResult
    {
        internal SaveParticipantPreparationResult(
            SaveParticipantPreparationStatus status,
            SaveParticipantId failingPersistedParticipantId,
            SaveParticipantId failingCanonicalParticipantId,
            string diagnosticCode,
            string message,
            SavePreparedParticipantBatch batch)
        {
            Status =
                status;

            FailingPersistedParticipantId =
                failingPersistedParticipantId;

            FailingCanonicalParticipantId =
                failingCanonicalParticipantId;

            DiagnosticCode =
                diagnosticCode ?? string.Empty;

            Message =
                message ?? string.Empty;

            Batch =
                batch;
        }

        internal SaveParticipantPreparationStatus Status { get; }

        internal SaveParticipantId FailingPersistedParticipantId { get; }

        internal SaveParticipantId FailingCanonicalParticipantId { get; }

        internal string DiagnosticCode { get; }

        internal string Message { get; }

        internal SavePreparedParticipantBatch Batch { get; }

        internal int PreparedCount =>
            Succeeded &&
            Batch != null
                ? Batch.Count
                : 0;

        internal bool Succeeded =>
            Status ==
            SaveParticipantPreparationStatus.Succeeded;

        internal static SaveParticipantPreparationResult Success(
            SavePreparedParticipantBatch batch) =>
            new SaveParticipantPreparationResult(
                SaveParticipantPreparationStatus.Succeeded,
                default,
                default,
                string.Empty,
                "The Chronicle participant payload set was prepared successfully.",
                batch);

        internal static SaveParticipantPreparationResult Failure(
            SaveParticipantPreparationStatus status,
            SaveParticipantId persistedParticipantId,
            SaveParticipantId canonicalParticipantId,
            string diagnosticCode,
            string message) =>
            new SaveParticipantPreparationResult(
                status,
                persistedParticipantId,
                canonicalParticipantId,
                diagnosticCode,
                message,
                null);
    }
}
