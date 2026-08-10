
namespace EchoDevGames.EchoSave
{
    internal sealed class SaveParticipantApplyPlanResult
    {
        private SaveParticipantApplyPlanResult(
            SaveParticipantApplyPlanStatus status,
            SaveParticipantApplyPlan plan,
            SaveParticipantId failureParticipantId,
            string diagnosticCode,
            string message)
        {
            Status = status;
            Plan = plan;
            FailureParticipantId = failureParticipantId;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
        }

        internal SaveParticipantApplyPlanStatus Status { get; }

        internal SaveParticipantApplyPlan Plan { get; }

        internal SaveParticipantId FailureParticipantId { get; }

        internal string DiagnosticCode { get; }

        internal string Message { get; }

        internal bool Succeeded =>
            Status ==
            SaveParticipantApplyPlanStatus.Succeeded &&
            Plan != null;

        internal static SaveParticipantApplyPlanResult
            Success(
                SaveParticipantApplyPlan plan) =>
            new SaveParticipantApplyPlanResult(
                SaveParticipantApplyPlanStatus.Succeeded,
                plan,
                default,
                string.Empty,
                "Chronicle prepared-load apply preflight succeeded.");

        internal static SaveParticipantApplyPlanResult
            Failure(
                SaveParticipantApplyPlanStatus status,
                SaveParticipantId participantId,
                string diagnosticCode,
                string message) =>
            new SaveParticipantApplyPlanResult(
                status,
                null,
                participantId,
                diagnosticCode,
                message);
    }
}
