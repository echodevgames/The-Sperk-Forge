
using System;

namespace EchoDevGames.EchoSave
{
    public sealed class SaveDeletionPlan
    {
        internal SaveDeletionPlan(
            SaveDeletionPlanStatus status,
            string diagnosticCode,
            string message,
            string planId,
            string sessionId,
            SaveSlotId slotId,
            SaveGenerationId currentGenerationId,
            string displayName,
            bool wasActiveSlot,
            DateTimeOffset issuedUtc,
            DateTimeOffset expiresUtc,
            SaveDeletionSourceSnapshot sourceSnapshot)
        {
            Status = status;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
            PlanId = planId ?? string.Empty;
            SessionId = sessionId ?? string.Empty;
            SlotId = slotId;
            CurrentGenerationId = currentGenerationId;
            DisplayName = displayName ?? string.Empty;
            WasActiveSlot = wasActiveSlot;
            IssuedUtc = issuedUtc;
            ExpiresUtc = expiresUtc;
            SourceSnapshot = sourceSnapshot;
        }

        public SaveDeletionPlanStatus Status { get; }
        public string DiagnosticCode { get; }
        public string Message { get; }
        public string PlanId { get; }
        public SaveSlotId SlotId { get; }
        public SaveGenerationId CurrentGenerationId { get; }
        public string DisplayName { get; }
        public bool WasActiveSlot { get; }
        public DateTimeOffset IssuedUtc { get; }
        public DateTimeOffset ExpiresUtc { get; }

        public bool Succeeded =>
            Status == SaveDeletionPlanStatus.Ready;

        internal string SessionId { get; }
        internal SaveDeletionSourceSnapshot SourceSnapshot { get; }

        internal static SaveDeletionPlan Failure(
            SaveDeletionPlanStatus status,
            string diagnosticCode,
            string message,
            SaveSlotId slotId = default) =>
            new SaveDeletionPlan(
                status,
                diagnosticCode,
                message,
                string.Empty,
                string.Empty,
                slotId,
                default,
                string.Empty,
                false,
                default,
                default,
                null);
    }
}
