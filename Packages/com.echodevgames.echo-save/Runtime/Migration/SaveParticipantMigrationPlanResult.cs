namespace EchoDevGames.EchoSave
{
    internal readonly struct SaveParticipantMigrationPlanResult
    {
        internal SaveParticipantMigrationPlanResult(
            SaveParticipantMigrationPlanStatus status,
            string diagnosticCode,
            string message)
        {
            Status =
                status;

            DiagnosticCode =
                diagnosticCode ?? string.Empty;

            Message =
                message ?? string.Empty;
        }

        internal SaveParticipantMigrationPlanStatus Status { get; }

        internal string DiagnosticCode { get; }

        internal string Message { get; }

        internal bool Succeeded =>
            Status ==
            SaveParticipantMigrationPlanStatus.Succeeded;

        internal static SaveParticipantMigrationPlanResult Success(
            string message) =>
            new SaveParticipantMigrationPlanResult(
                SaveParticipantMigrationPlanStatus.Succeeded,
                string.Empty,
                message);
    }
}
