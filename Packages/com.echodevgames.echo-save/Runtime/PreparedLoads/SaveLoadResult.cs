
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Terminal truth for one same-scene prepare-then-apply convenience load.
    /// A post-mutation participant failure is reported honestly and never
    /// presented as though runtime state rolled back.
    /// </summary>
    public sealed class SaveLoadResult
    {
        internal SaveLoadResult(
            SaveLoadStatus status,
            SaveSlotId sourceSlotId,
            SaveGenerationId sourceGenerationId,
            bool preparationSucceeded,
            bool applyAttempted,
            bool mutationBegan,
            bool handleConsumed,
            string diagnosticCode,
            string message,
            SavePreparedLoadApplyResult applyResult)
        {
            Status = status;
            SourceSlotId = sourceSlotId;
            SourceGenerationId = sourceGenerationId;
            PreparationSucceeded = preparationSucceeded;
            ApplyAttempted = applyAttempted;
            MutationBegan = mutationBegan;
            HandleConsumed = handleConsumed;
            DiagnosticCode =
                diagnosticCode ?? string.Empty;

            Message =
                message ?? string.Empty;

            ApplyResult = applyResult;
        }

        public SaveLoadStatus Status { get; }

        public SaveSlotId SourceSlotId { get; }

        public SaveGenerationId SourceGenerationId { get; }

        public bool PreparationSucceeded { get; }

        public bool ApplyAttempted { get; }

        public bool MutationBegan { get; }

        public bool HandleConsumed { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public SavePreparedLoadApplyResult ApplyResult { get; }

        public bool Succeeded =>
            Status == SaveLoadStatus.Succeeded;
    }
}
