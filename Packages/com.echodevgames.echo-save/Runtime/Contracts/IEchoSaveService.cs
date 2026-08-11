using UnityEngine;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Primary package-local Chronicle service surface.
    ///
    /// M4-R1 composes the already-proven participant, catalog, slot-creation,
    /// prepared-load, and apply foundations into one bounded consumer facade
    /// while preserving M4 save/recovery/slot-operation semantics.
    /// </summary>
    public interface IEchoSaveService
    {
        EchoSaveServiceState State { get; }

        EchoSaveConfiguration Configuration { get; }

        SaveParticipantRegistrationResult RegisterParticipant(
            ISaveParticipant participant);

        SaveSlotCatalogSnapshot GetCatalogSnapshot();

        Awaitable<SaveSlotCatalogRefreshResult> RefreshCatalogAsync();

        Awaitable<SaveSlotCreateResult> CreateSlotAsync(
            SaveSlotCreateRequest request);

        SaveActiveSlotSelectionResult SelectSlot(
            SaveSlotId slotId);

        Awaitable<PreparedLoadCreationResult> PrepareLoadAsync(
            SaveLoadRequest request);

        Awaitable<SavePreparedLoadApplyResult> ApplyPreparedLoadAsync(
            PreparedSaveLoad handle);

        Awaitable<SaveLoadResult> LoadAndApplyAsync(
            SaveLoadRequest request);

        Awaitable<EchoSaveLifecycleResult> InitializeAsync();

        Awaitable<SaveOperationResult> SaveAsync(
            SaveRequest request);

        AutosaveSubmissionResult RequestAutosave(
            AutosaveRequest request);

        Awaitable<SaveRecoveryPlan> BuildRecoveryPlanAsync(
            SaveSlotId slotId);

        Awaitable<SaveRecoveryResult> ExecuteRecoveryAsync(
            SaveRecoveryPlan plan,
            SaveRecoveryCandidate candidate);

        Awaitable<SaveSlotRenameResult> RenameSlotAsync(
            SaveSlotRenameRequest request);

        Awaitable<SaveSlotDuplicateResult> DuplicateSlotAsync(
            SaveSlotDuplicateRequest request);

        Awaitable<SaveDeletionPlan> PrepareDeleteSlotAsync(
            SaveSlotId slotId);

        Awaitable<SaveSlotDeleteResult> ConfirmDeleteSlotAsync(
            SaveDeletionPlan plan);

        Awaitable<EchoSaveLifecycleResult> ShutdownAsync();
    }
}
