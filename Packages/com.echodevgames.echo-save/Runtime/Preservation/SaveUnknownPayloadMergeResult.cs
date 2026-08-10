
namespace EchoDevGames.EchoSave
{
    internal readonly struct SaveUnknownPayloadMergeResult
    {
        internal SaveUnknownPayloadMergeResult(
            SaveUnknownPayloadMergeStatus status,
            string diagnosticCode,
            string message,
            SaveParticipantId failingPersistedId,
            SaveParticipantId currentOwnerId,
            SaveMergedParticipantTransportBatch batch)
        {
            Status =
                status;

            DiagnosticCode =
                diagnosticCode ?? string.Empty;

            Message =
                message ?? string.Empty;

            FailingPersistedId =
                failingPersistedId;

            CurrentOwnerId =
                currentOwnerId;

            Batch =
                batch;
        }

        internal SaveUnknownPayloadMergeStatus Status { get; }

        internal string DiagnosticCode { get; }

        internal string Message { get; }

        internal SaveParticipantId FailingPersistedId { get; }

        internal SaveParticipantId CurrentOwnerId { get; }

        internal SaveMergedParticipantTransportBatch Batch { get; }

        internal bool Succeeded =>
            Status ==
            SaveUnknownPayloadMergeStatus.Succeeded;
    }
}
