
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Immutable exact-ID one-use unknown-payload prune plan.
    /// </summary>
    public sealed class SaveUnknownPayloadPrunePlan
    {
        private readonly ReadOnlyCollection<SaveParticipantId>
            participantIds;

        internal SaveUnknownPayloadPrunePlan(
            SaveUnknownPayloadPrunePlanStatus status,
            string diagnosticCode,
            string message,
            string planId,
            string sessionId,
            SaveSlotId slotId,
            SaveGenerationId sourceGenerationId,
            string sourceProvenanceFingerprint,
            SaveParticipantId[] participantIds,
            DateTimeOffset issuedUtc,
            DateTimeOffset expiresUtc,
            SaveUnknownPayloadPruneSourceSnapshot sourceSnapshot)
        {
            Status = status;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
            PlanId = planId ?? string.Empty;
            SessionId = sessionId ?? string.Empty;
            SlotId = slotId;
            SourceGenerationId = sourceGenerationId;
            SourceProvenanceFingerprint =
                sourceProvenanceFingerprint ?? string.Empty;

            this.participantIds =
                Array.AsReadOnly(
                    participantIds == null
                        ? Array.Empty<SaveParticipantId>()
                        : (SaveParticipantId[])
                            participantIds.Clone());

            IssuedUtc = issuedUtc;
            ExpiresUtc = expiresUtc;
            SourceSnapshot = sourceSnapshot;
        }

        public SaveUnknownPayloadPrunePlanStatus Status { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public string PlanId { get; }

        public SaveSlotId SlotId { get; }

        public SaveGenerationId SourceGenerationId { get; }

        public string SourceProvenanceFingerprint { get; }

        public IReadOnlyList<SaveParticipantId> ParticipantIds =>
            participantIds;

        public DateTimeOffset IssuedUtc { get; }

        public DateTimeOffset ExpiresUtc { get; }

        public bool Succeeded =>
            Status == SaveUnknownPayloadPrunePlanStatus.Ready;

        internal string SessionId { get; }

        internal SaveUnknownPayloadPruneSourceSnapshot SourceSnapshot
        {
            get;
        }

        internal static SaveUnknownPayloadPrunePlan Failure(
            SaveUnknownPayloadPrunePlanStatus status,
            string diagnosticCode,
            string message,
            SaveSlotId slotId = default) =>
            new SaveUnknownPayloadPrunePlan(
                status,
                diagnosticCode,
                message,
                string.Empty,
                string.Empty,
                slotId,
                default,
                string.Empty,
                Array.Empty<SaveParticipantId>(),
                default,
                default,
                null);
    }
}
