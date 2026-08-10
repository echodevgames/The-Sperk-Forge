
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Public payload-free report for one planned participant apply action.
    /// </summary>
    public sealed class SaveParticipantApplyReportEntry
    {
        internal SaveParticipantApplyReportEntry(
            SaveParticipantId participantId,
            SaveParticipantApplyActionKind action,
            SaveParticipantApplyOutcome outcome,
            string diagnosticCode,
            string message)
        {
            ParticipantId = participantId;
            Action = action;
            Outcome = outcome;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public SaveParticipantId ParticipantId { get; }

        public SaveParticipantApplyActionKind Action { get; }

        public SaveParticipantApplyOutcome Outcome { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }
    }
}
