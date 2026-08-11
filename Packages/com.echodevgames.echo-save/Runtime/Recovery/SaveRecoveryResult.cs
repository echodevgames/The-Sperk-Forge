
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Truthful result of one explicit Chronicle recovery execution.
    ///
    /// HeadPublished is the durable recovery commit boundary. A later catalog
    /// reconciliation failure never fabricates rollback of a committed head.
    /// </summary>
    public sealed class SaveRecoveryResult
    {
        internal SaveRecoveryResult(
            SaveRecoveryExecutionStatus status,
            string diagnosticCode,
            string message,
            SaveSlotId slotId,
            SaveGenerationId selectedGenerationId,
            bool headPublished,
            bool catalogReconciled)
        {
            Status =
                status;

            DiagnosticCode =
                diagnosticCode ?? string.Empty;

            Message =
                message ?? string.Empty;

            SlotId =
                slotId;

            SelectedGenerationId =
                selectedGenerationId;

            HeadPublished =
                headPublished;

            CatalogReconciled =
                catalogReconciled;
        }

        public SaveRecoveryExecutionStatus Status { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public SaveSlotId SlotId { get; }

        public SaveGenerationId SelectedGenerationId { get; }

        public bool HeadPublished { get; }

        public bool CatalogReconciled { get; }

        public bool RecoveryCommitted =>
            HeadPublished;

        public bool Succeeded =>
            Status ==
                SaveRecoveryExecutionStatus.Succeeded;

        internal static SaveRecoveryResult Failure(
            SaveRecoveryExecutionStatus status,
            string diagnosticCode,
            string message,
            SaveSlotId slotId,
            SaveGenerationId selectedGenerationId) =>
            new SaveRecoveryResult(
                status,
                diagnosticCode,
                message,
                slotId,
                selectedGenerationId,
                false,
                false);
    }
}
