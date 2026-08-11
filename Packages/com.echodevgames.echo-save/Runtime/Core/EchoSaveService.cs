using System;
using UnityEngine;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Chronicle lifecycle and bounded public runtime facade.
    ///
    /// M4-R1 composes the already-proven participant registry, catalog,
    /// technical slot creation, prepared-load, apply, save, recovery, and slot
    /// mutation authorities without adding generic queues, scene authority, or
    /// project lifetime ownership.
    /// </summary>
    internal sealed class EchoSaveService :
        IEchoSaveService
    {
        internal const int DefaultCatalogScanLimit =
            256;

        internal const int DefaultRetentionDiscoveryLimit =
            512;

        internal const int DefaultRecoveryDiscoveryLimit =
            512;

        internal const int DefaultTechnicalSlotCapacity =
            64;

        internal const int DefaultSlotIdentityAttempts =
            8;

        internal const int DefaultTrashDiscoveryLimit =
            128;

        internal const int DefaultTrashRetentionRecords =
            8;

        internal const int DefaultTrashIdentityAttempts =
            4;

        internal static readonly TimeSpan
            DefaultDeletionPlanLifetime =
                TimeSpan.FromMinutes(5);

        private EchoSaveConfiguration configuration;
        private IEchoSaveLifecycleProbe lifecycleProbe;
        private IEchoSaveStorageBackendFactory storageBackendFactory;
        private ISaveStorageBackend storageBackend;
        private EchoSaveServiceState state;

        private readonly SaveOperationAdmissionCoordinator
            saveOperationAdmission =
                new SaveOperationAdmissionCoordinator();

        private SaveParticipantRegistry participantRegistry;
        private SaveUnknownPayloadStore unknownPayloadStore;
        private SaveSlotCatalog slotCatalog;
        private SaveSerializerRegistry serializerRegistry;
        private SaveParticipantMigrationRegistry participantMigrationRegistry;
        private SaveCurrentGenerationReader currentGenerationReader;
        private SaveParticipantPayloadPreparer participantPayloadPreparer;
        private SavePreparedLoadStore preparedLoadStore;
        private SavePreparedLoadApplyCoordinator preparedLoadApplyCoordinator;
        private SaveTechnicalSlotCreationCoordinator
            technicalSlotCreationCoordinator;
        private ISaveManualTransactionExecutor
            manualSaveTransactionExecutor;
        private ISaveRecoveryPlanBuilder
            recoveryPlanBuilder;
        private ISaveRecoveryExecutor
            recoveryExecutor;

        private SaveSlotMutationCoordinator
            slotMutationCoordinator;

        private SaveSlotDeletionCoordinator
            slotDeletionCoordinator;

        private PendingAutosaveRequest pendingAutosave;
        private long nextAutosaveTicketId;
        private bool drainingPendingAutosave;

        internal EchoSaveService(
            EchoSaveConfiguration configuration)
        {
            this.configuration = configuration;
            lifecycleProbe =
                NullEchoSaveLifecycleProbe.Instance;
            storageBackendFactory =
                DefaultEchoSaveStorageBackendFactory.Instance;
            state =
                EchoSaveServiceState.AuthorityClaimed;

            saveOperationAdmission.AvailabilityBecameAvailable +=
                OnOperationAdmissionAvailable;
        }

        public EchoSaveServiceState State =>
            state;

        public EchoSaveConfiguration Configuration =>
            configuration;

        public SaveParticipantRegistrationResult RegisterParticipant(
            ISaveParticipant participant) =>
            RegisterParticipantCore(
                participant);

        public SaveSlotCatalogSnapshot GetCatalogSnapshot() =>
            GetCatalogSnapshotCore();

        public async Awaitable<SaveSlotCatalogRefreshResult>
            RefreshCatalogAsync()
        {
            await Awaitable.MainThreadAsync();

            return RefreshCatalogCore();
        }

        public async Awaitable<SaveSlotCreateResult>
            CreateSlotAsync(
                SaveSlotCreateRequest request)
        {
            await Awaitable.MainThreadAsync();

            return CreateSlotCore(
                request);
        }

        public SaveActiveSlotSelectionResult SelectSlot(
            SaveSlotId slotId) =>
            SelectSlotCore(
                slotId);

        public async Awaitable<PreparedLoadCreationResult>
            PrepareLoadAsync(
                SaveLoadRequest request)
        {
            await Awaitable.MainThreadAsync();

            return PrepareLoadCore(
                request);
        }

        public async Awaitable<SavePreparedLoadApplyResult>
            ApplyPreparedLoadAsync(
                PreparedSaveLoad handle)
        {
            await Awaitable.MainThreadAsync();

            return ApplyPreparedLoadCore(
                handle);
        }

        public async Awaitable<SaveLoadResult>
            LoadAndApplyAsync(
                SaveLoadRequest request)
        {
            await Awaitable.MainThreadAsync();

            return LoadAndApplyCore(
                request);
        }

        public async Awaitable<EchoSaveLifecycleResult>
            InitializeAsync()
        {
            await Awaitable.MainThreadAsync();
            return InitializeCore();
        }

        public async Awaitable<SaveOperationResult>
            SaveAsync(
                SaveRequest request)
        {
            await Awaitable.MainThreadAsync();
            return SaveCore(
                request);
        }

        public AutosaveSubmissionResult RequestAutosave(
            AutosaveRequest request) =>
            RequestAutosaveCore(
                request);

        public async Awaitable<SaveRecoveryPlan>
            BuildRecoveryPlanAsync(
                SaveSlotId slotId)
        {
            await Awaitable.MainThreadAsync();

            return BuildRecoveryPlanCore(
                slotId);
        }

        public async Awaitable<SaveRecoveryResult>
            ExecuteRecoveryAsync(
                SaveRecoveryPlan plan,
                SaveRecoveryCandidate candidate)
        {
            await Awaitable.MainThreadAsync();

            return ExecuteRecoveryCore(
                plan,
                candidate);
        }

        public async Awaitable<SaveSlotRenameResult>
            RenameSlotAsync(
                SaveSlotRenameRequest request)
        {
            await Awaitable.MainThreadAsync();

            return RenameSlotCore(
                request);
        }

        public async Awaitable<SaveSlotDuplicateResult>
            DuplicateSlotAsync(
                SaveSlotDuplicateRequest request)
        {
            await Awaitable.MainThreadAsync();

            return DuplicateSlotCore(
                request);
        }

        public async Awaitable<SaveDeletionPlan>
            PrepareDeleteSlotAsync(
                SaveSlotId slotId)
        {
            await Awaitable.MainThreadAsync();

            return PrepareDeleteSlotCore(
                slotId);
        }

        public async Awaitable<SaveSlotDeleteResult>
            ConfirmDeleteSlotAsync(
                SaveDeletionPlan plan)
        {
            await Awaitable.MainThreadAsync();

            return ConfirmDeleteSlotCore(
                plan);
        }

        public async Awaitable<EchoSaveLifecycleResult>
            ShutdownAsync()
        {
            await Awaitable.MainThreadAsync();
            return ShutdownCore();
        }

        internal void SetConfiguration(
            EchoSaveConfiguration value)
        {
            EnsurePreInitializationMutationAllowed(
                "Chronicle configuration");

            configuration = value;

            if (state == EchoSaveServiceState.Blocked)
            {
                state =
                    EchoSaveServiceState.AuthorityClaimed;
            }
        }

        internal void SetLifecycleProbe(
            IEchoSaveLifecycleProbe probe)
        {
            if (probe == null)
            {
                throw new ArgumentNullException(
                    nameof(probe));
            }

            EnsurePreInitializationMutationAllowed(
                "The Chronicle lifecycle probe");

            lifecycleProbe = probe;
        }

        internal void SetStorageBackendFactory(
            IEchoSaveStorageBackendFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(
                    nameof(factory));
            }

            EnsurePreInitializationMutationAllowed(
                "The Chronicle storage backend factory");

            storageBackendFactory = factory;
            storageBackend = null;

            saveOperationAdmission.Close();
            DiscardPendingAutosave(
                EchoSaveDiagnosticCodes.AutosaveDiscarded,
                "The Chronicle autosave runtime was reset before initialization.");
            ResetManualSaveRuntime();
        }

        internal EchoSaveLifecycleResult InitializeCore()
        {
            if (state == EchoSaveServiceState.Ready)
            {
                return new EchoSaveLifecycleResult(
                    EchoSaveLifecycleStatus.NoChange,
                    state,
                    string.Empty,
                    "The Chronicle is already initialized.");
            }

            if (state ==
                    EchoSaveServiceState.ShuttingDown ||
                state ==
                    EchoSaveServiceState.Shutdown)
            {
                return new EchoSaveLifecycleResult(
                    EchoSaveLifecycleStatus.Rejected,
                    state,
                    EchoSaveDiagnosticCodes.InvalidLifecycle,
                    "The Chronicle cannot initialize after shutdown has begun.");
            }

            saveOperationAdmission.Close();
            DiscardPendingAutosave(
                EchoSaveDiagnosticCodes.AutosaveDiscarded,
                "The Chronicle cleared pending autosave state before initialization.");
            ResetManualSaveRuntime();

            state =
                EchoSaveServiceState.Initializing;

            if (configuration == null)
            {
                return BlockInitialization(
                    EchoSaveDiagnosticCodes
                        .MissingOrInvalidConfiguration,
                    "The Chronicle configuration is missing.");
            }

            if (!configuration.TryValidate(
                    out string validationMessage))
            {
                return BlockInitialization(
                    EchoSaveDiagnosticCodes
                        .MissingOrInvalidConfiguration,
                    validationMessage);
            }

            SaveStorageResult creation =
                storageBackendFactory.TryCreate(
                    configuration,
                    out storageBackend);

            if (!creation.Succeeded ||
                storageBackend == null)
            {
                storageBackend = null;

                return BlockInitialization(
                    creation.DiagnosticCode.Length == 0
                        ? EchoSaveDiagnosticCodes
                            .StorageInitializationFailed
                        : creation.DiagnosticCode,
                    creation.Message.Length == 0
                        ? "The Chronicle storage backend could not be created."
                        : creation.Message);
            }

            SaveStorageResult storageInitialization =
                storageBackend.Initialize();

            if (!storageInitialization.Succeeded)
            {
                storageBackend = null;

                return BlockInitialization(
                    storageInitialization.DiagnosticCode.Length == 0
                        ? EchoSaveDiagnosticCodes
                            .StorageInitializationFailed
                        : storageInitialization.DiagnosticCode,
                    storageInitialization.Message.Length == 0
                        ? "The Chronicle storage backend could not initialize."
                        : storageInitialization.Message);
            }

            try
            {
                BuildManualSaveRuntime();
            }
            catch (Exception exception)
            {
                storageBackend.Shutdown();
                storageBackend = null;

                saveOperationAdmission.Close();
                ResetManualSaveRuntime();

                return BlockInitialization(
                    EchoSaveDiagnosticCodes
                        .StorageInitializationFailed,
                    $"The Chronicle manual-save runtime could not initialize. {exception.GetType().Name}: {exception.Message}");
            }

            lifecycleProbe.OnInitializeAccepted(
                configuration);

            saveOperationAdmission.Open();

            state =
                EchoSaveServiceState.Ready;

            return new EchoSaveLifecycleResult(
                EchoSaveLifecycleStatus.Succeeded,
                state,
                string.Empty,
                "The Chronicle initialized its storage backend and manual-save runtime successfully.");
        }

        internal EchoSaveLifecycleResult ShutdownCore()
        {
            if (state == EchoSaveServiceState.Shutdown)
            {
                return new EchoSaveLifecycleResult(
                    EchoSaveLifecycleStatus.NoChange,
                    state,
                    string.Empty,
                    "The Chronicle is already shut down.");
            }

            state =
                EchoSaveServiceState.ShuttingDown;

            saveOperationAdmission.Close();
            DiscardPendingAutosave(
                EchoSaveDiagnosticCodes.AutosaveDiscarded,
                "The Chronicle discarded the pending autosave because shutdown closed new admission.");

            if (saveOperationAdmission.IsOccupied)
            {
                return new EchoSaveLifecycleResult(
                    EchoSaveLifecycleStatus.Rejected,
                    state,
                    EchoSaveDiagnosticCodes
                        .PublicSaveShutdownPending,
                    "The Chronicle closed new save admission and is waiting for the admitted mutating operation to settle before storage shutdown.");
            }

            SaveStorageResult storageShutdown =
                storageBackend != null
                    ? storageBackend.Shutdown()
                    : SaveStorageResult.NoChange(
                        "No Chronicle storage backend was active.");

            lifecycleProbe.OnShutdown();

            storageBackend = null;
            ResetManualSaveRuntime();

            state =
                EchoSaveServiceState.Shutdown;

            if (!storageShutdown.Succeeded)
            {
                return new EchoSaveLifecycleResult(
                    EchoSaveLifecycleStatus.Rejected,
                    state,
                    storageShutdown.DiagnosticCode,
                    storageShutdown.Message);
            }

            return new EchoSaveLifecycleResult(
                EchoSaveLifecycleStatus.Succeeded,
                state,
                string.Empty,
                "The Chronicle shut down cleanly.");
        }

        internal void ShutdownImmediate()
        {
            if (state ==
                EchoSaveServiceState.Shutdown)
            {
                return;
            }

            state =
                EchoSaveServiceState.ShuttingDown;

            saveOperationAdmission.Close();
            DiscardPendingAutosave(
                EchoSaveDiagnosticCodes.AutosaveDiscarded,
                "The Chronicle discarded the pending autosave during immediate shutdown.");

            if (storageBackend != null)
            {
                storageBackend.Shutdown();
            }

            lifecycleProbe.OnShutdown();

            storageBackend = null;
            ResetManualSaveRuntime();

            state =
                EchoSaveServiceState.Shutdown;
        }

        internal SaveSlotCatalogRefreshResult
            RefreshCatalogSynchronouslyForTesting() =>
            RefreshCatalogCore();

        internal SaveSlotCreateResult
            CreateSlotSynchronouslyForTesting(
                SaveSlotCreateRequest request) =>
            CreateSlotCore(
                request);

        internal SaveActiveSlotSelectionResult
            SelectSlotSynchronouslyForTesting(
                SaveSlotId slotId) =>
            SelectSlotCore(
                slotId);

        internal PreparedLoadCreationResult
            PrepareLoadSynchronouslyForTesting(
                SaveLoadRequest request) =>
            PrepareLoadCore(
                request);

        internal SavePreparedLoadApplyResult
            ApplyPreparedLoadSynchronouslyForTesting(
                PreparedSaveLoad handle) =>
            ApplyPreparedLoadCore(
                handle);

        internal SaveLoadResult
            LoadAndApplySynchronouslyForTesting(
                SaveLoadRequest request) =>
            LoadAndApplyCore(
                request);

        internal SaveOperationResult
            SaveSynchronouslyForTesting(
                SaveRequest request) =>
            SaveCore(
                request);

        internal AutosaveSubmissionResult
            RequestAutosaveSynchronouslyForTesting(
                AutosaveRequest request) =>
            RequestAutosaveCore(
                request);

        internal SaveRecoveryPlan
            BuildRecoveryPlanSynchronouslyForTesting(
                SaveSlotId slotId) =>
            BuildRecoveryPlanCore(
                slotId);

        internal SaveRecoveryResult
            ExecuteRecoverySynchronouslyForTesting(
                SaveRecoveryPlan plan,
                SaveRecoveryCandidate candidate) =>
            ExecuteRecoveryCore(
                plan,
                candidate);

        internal SaveSlotRenameResult
            RenameSlotSynchronouslyForTesting(
                SaveSlotRenameRequest request) =>
            RenameSlotCore(
                request);

        internal SaveSlotDuplicateResult
            DuplicateSlotSynchronouslyForTesting(
                SaveSlotDuplicateRequest request) =>
            DuplicateSlotCore(
                request);

        internal SaveDeletionPlan
            PrepareDeleteSlotSynchronouslyForTesting(
                SaveSlotId slotId) =>
            PrepareDeleteSlotCore(
                slotId);

        internal SaveSlotDeleteResult
            ConfirmDeleteSlotSynchronouslyForTesting(
                SaveDeletionPlan plan) =>
            ConfirmDeleteSlotCore(
                plan);

        internal int PendingAutosaveCountForTesting =>
            pendingAutosave == null
                ? 0
                : 1;

        internal AutosaveTicket PendingAutosaveTicketForTesting =>
            pendingAutosave?.Ticket;

        internal void SetManualSaveTransactionExecutorForTesting(
            ISaveManualTransactionExecutor executor)
        {
            if (executor == null)
            {
                throw new ArgumentNullException(
                    nameof(executor));
            }

            if (state !=
                EchoSaveServiceState.Ready)
            {
                throw new InvalidOperationException(
                    "Chronicle testing may replace the manual-save executor only after successful initialization.");
            }

            manualSaveTransactionExecutor =
                executor;
        }

        internal ISaveStorageBackend
            StorageBackendForTesting =>
                storageBackend;

        internal SaveOperationAdmissionCoordinator
            SaveOperationAdmissionForTesting =>
                saveOperationAdmission;

        internal SaveParticipantRegistry
            ParticipantRegistryForTesting =>
                participantRegistry;

        internal SaveSlotCatalog
            SlotCatalogForTesting =>
                slotCatalog;

        internal SavePreparedLoadStore
            PreparedLoadStoreForTesting =>
                preparedLoadStore;

        internal SaveParticipantMigrationRegistry
            ParticipantMigrationRegistryForTesting =>
                participantMigrationRegistry;

        internal SaveSerializerRegistry
            SerializerRegistryForTesting =>
                serializerRegistry;


        private SaveParticipantRegistrationResult
            RegisterParticipantCore(
                ISaveParticipant participant)
        {
            if (state !=
                EchoSaveServiceState.Ready)
            {
                bool admissionClosed =
                    state ==
                        EchoSaveServiceState.ShuttingDown ||
                    state ==
                        EchoSaveServiceState.Shutdown;

                return new SaveParticipantRegistrationResult(
                    admissionClosed
                        ? SaveParticipantRegistrationStatus
                            .AdmissionClosed
                        : SaveParticipantRegistrationStatus
                            .ServiceNotReady,
                    null,
                    admissionClosed
                        ? EchoSaveDiagnosticCodes
                            .PublicParticipantAdmissionClosed
                        : EchoSaveDiagnosticCodes
                            .PublicParticipantServiceNotReady,
                    admissionClosed
                        ? "The Chronicle is not accepting new participant registrations after shutdown admission closed."
                        : "The Chronicle must be Ready before participants can register through the public service.");
            }

            if (participantRegistry == null)
            {
                return new SaveParticipantRegistrationResult(
                    SaveParticipantRegistrationStatus.ServiceNotReady,
                    null,
                    EchoSaveDiagnosticCodes
                        .PublicParticipantServiceNotReady,
                    "The Chronicle participant registry is unavailable.");
            }

            return participantRegistry.Register(
                participant);
        }

        private SaveSlotCatalogSnapshot
            GetCatalogSnapshotCore() =>
            slotCatalog == null
                ? SaveSlotCatalogSnapshot.Empty
                : slotCatalog.Snapshot;

        private SaveSlotCatalogRefreshResult
            RefreshCatalogCore()
        {
            if (state !=
                EchoSaveServiceState.Ready)
            {
                bool admissionClosed =
                    state ==
                        EchoSaveServiceState.ShuttingDown ||
                    state ==
                        EchoSaveServiceState.Shutdown;

                return CatalogRefreshFailure(
                    admissionClosed
                        ? SaveSlotCatalogRefreshStatus
                            .AdmissionClosed
                        : SaveSlotCatalogRefreshStatus
                            .ServiceNotReady,
                    admissionClosed
                        ? EchoSaveDiagnosticCodes
                            .PublicCatalogAdmissionClosed
                        : EchoSaveDiagnosticCodes
                            .PublicCatalogServiceNotReady,
                    admissionClosed
                        ? "The Chronicle is not accepting catalog refresh operations after shutdown admission closed."
                        : "The Chronicle must be Ready before the catalog can be refreshed.");
            }

            if (slotCatalog == null)
            {
                return CatalogRefreshFailure(
                    SaveSlotCatalogRefreshStatus.ServiceNotReady,
                    EchoSaveDiagnosticCodes
                        .PublicCatalogServiceNotReady,
                    "The Chronicle slot catalog is unavailable.");
            }

            SaveOperationAdmissionStatus admission =
                saveOperationAdmission.TryAcquire(
                    out SaveOperationAdmissionLease lease);

            if (admission ==
                SaveOperationAdmissionStatus.Closed)
            {
                return CatalogRefreshFailure(
                    SaveSlotCatalogRefreshStatus.AdmissionClosed,
                    EchoSaveDiagnosticCodes
                        .PublicCatalogAdmissionClosed,
                    "The Chronicle is not accepting catalog refresh operations.");
            }

            if (admission ==
                SaveOperationAdmissionStatus.Busy)
            {
                return CatalogRefreshFailure(
                    SaveSlotCatalogRefreshStatus.Busy,
                    EchoSaveDiagnosticCodes
                        .PublicCatalogBusy,
                    "Another Chronicle operation already owns the root-local admission lease. Catalog refresh was rejected as Busy and was not queued.");
            }

            using (lease)
            {
                return slotCatalog.Refresh();
            }
        }

        private SaveSlotCatalogRefreshResult
            CatalogRefreshFailure(
                SaveSlotCatalogRefreshStatus status,
                string diagnosticCode,
                string message) =>
            new SaveSlotCatalogRefreshResult(
                status,
                diagnosticCode,
                message,
                GetCatalogSnapshotCore(),
                false);

        private SaveSlotCreateResult CreateSlotCore(
            SaveSlotCreateRequest request)
        {
            if (state !=
                EchoSaveServiceState.Ready)
            {
                bool admissionClosed =
                    state ==
                        EchoSaveServiceState.ShuttingDown ||
                    state ==
                        EchoSaveServiceState.Shutdown;

                return SlotCreateFailure(
                    admissionClosed
                        ? SaveSlotCreateStatus.AdmissionClosed
                        : SaveSlotCreateStatus.ServiceNotReady,
                    admissionClosed
                        ? EchoSaveDiagnosticCodes
                            .PublicSlotCreateAdmissionClosed
                        : EchoSaveDiagnosticCodes
                            .PublicSlotCreateServiceNotReady,
                    admissionClosed
                        ? "The Chronicle is not accepting new slot-creation operations after shutdown admission closed."
                        : "The Chronicle must be Ready before a slot can be created.");
            }

            if (technicalSlotCreationCoordinator == null)
            {
                return SlotCreateFailure(
                    SaveSlotCreateStatus.ServiceNotReady,
                    EchoSaveDiagnosticCodes
                        .PublicSlotCreateServiceNotReady,
                    "The Chronicle slot-creation runtime is unavailable.");
            }

            if (request == null)
            {
                return SlotCreateFailure(
                    SaveSlotCreateStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes
                        .SlotCreateInvalidRequest,
                    "Chronicle public slot creation requires one request.");
            }

            SaveOperationAdmissionStatus admission =
                saveOperationAdmission.TryAcquire(
                    out SaveOperationAdmissionLease lease);

            if (admission ==
                SaveOperationAdmissionStatus.Closed)
            {
                return SlotCreateFailure(
                    SaveSlotCreateStatus.AdmissionClosed,
                    EchoSaveDiagnosticCodes
                        .PublicSlotCreateAdmissionClosed,
                    "The Chronicle is not accepting new slot-creation operations.");
            }

            if (admission ==
                SaveOperationAdmissionStatus.Busy)
            {
                return SlotCreateFailure(
                    SaveSlotCreateStatus.Busy,
                    EchoSaveDiagnosticCodes
                        .PublicSlotCreateBusy,
                    "Another Chronicle operation already owns the root-local admission lease. Slot creation was rejected as Busy and was not queued.");
            }

            using (lease)
            {
                SaveTechnicalSlotCreateResult technical =
                    technicalSlotCreationCoordinator.Create(
                        new SaveTechnicalSlotCreateRequest(
                            request.DisplayName,
                            request.ProjectId,
                            request.ProjectVersion,
                            request.BuildId));

                return MapTechnicalSlotCreateResult(
                    technical);
            }
        }

        private static SaveSlotCreateResult
            MapTechnicalSlotCreateResult(
                SaveTechnicalSlotCreateResult result)
        {
            if (result == null)
            {
                return SlotCreateFailure(
                    SaveSlotCreateStatus.PublicationFailed,
                    EchoSaveDiagnosticCodes
                        .SlotCreatePublicationFailed,
                    "The Chronicle technical slot-creation coordinator returned no result.");
            }

            SaveSlotCreateStatus status;

            switch (result.Status)
            {
                case SaveTechnicalSlotCreateStatus.Succeeded:
                    status =
                        SaveSlotCreateStatus.Succeeded;
                    break;

                case SaveTechnicalSlotCreateStatus.InvalidRequest:
                    status =
                        SaveSlotCreateStatus.InvalidRequest;
                    break;

                case SaveTechnicalSlotCreateStatus.CatalogUnavailable:
                    status =
                        SaveSlotCreateStatus.CatalogUnavailable;
                    break;

                case SaveTechnicalSlotCreateStatus.CapacityReached:
                    status =
                        SaveSlotCreateStatus.CapacityReached;
                    break;

                case SaveTechnicalSlotCreateStatus.SlotIdGenerationFailed:
                    status =
                        SaveSlotCreateStatus.SlotIdGenerationFailed;
                    break;

                case SaveTechnicalSlotCreateStatus
                    .SlotIdCollisionLimitExceeded:
                    status =
                        SaveSlotCreateStatus
                            .SlotIdCollisionLimitExceeded;
                    break;

                case SaveTechnicalSlotCreateStatus.PublicationFailed:
                    status =
                        SaveSlotCreateStatus.PublicationFailed;
                    break;

                case SaveTechnicalSlotCreateStatus
                    .PublishedCatalogReconciliationFailed:
                    status =
                        SaveSlotCreateStatus
                            .PublishedCatalogReconciliationFailed;
                    break;

                default:
                    status =
                        SaveSlotCreateStatus.PublicationFailed;
                    break;
            }

            return new SaveSlotCreateResult(
                status,
                result.DiagnosticCode,
                result.Message,
                result.SlotId,
                result.GenerationId,
                result.SlotPublished,
                result.CatalogReconciled,
                result.CreatedEntry);
        }

        private static SaveSlotCreateResult SlotCreateFailure(
            SaveSlotCreateStatus status,
            string diagnosticCode,
            string message) =>
            new SaveSlotCreateResult(
                status,
                diagnosticCode,
                message,
                default,
                default,
                false,
                false,
                null);

        private SaveActiveSlotSelectionResult SelectSlotCore(
            SaveSlotId slotId)
        {
            if (state !=
                EchoSaveServiceState.Ready)
            {
                bool admissionClosed =
                    state ==
                        EchoSaveServiceState.ShuttingDown ||
                    state ==
                        EchoSaveServiceState.Shutdown;

                return new SaveActiveSlotSelectionResult(
                    admissionClosed
                        ? SaveActiveSlotSelectionStatus
                            .AdmissionClosed
                        : SaveActiveSlotSelectionStatus
                            .ServiceNotReady,
                    slotCatalog != null &&
                        slotCatalog.HasActiveSlot,
                    slotCatalog != null &&
                        slotCatalog.HasActiveSlot
                        ? slotCatalog.ActiveSlotId
                        : default,
                    admissionClosed
                        ? EchoSaveDiagnosticCodes
                            .PublicSelectAdmissionClosed
                        : EchoSaveDiagnosticCodes
                            .PublicSelectServiceNotReady,
                    admissionClosed
                        ? "The Chronicle is not accepting active-slot selection after shutdown admission closed."
                        : "The Chronicle must be Ready before an active slot can be selected.");
            }

            if (slotCatalog == null)
            {
                return new SaveActiveSlotSelectionResult(
                    SaveActiveSlotSelectionStatus.ServiceNotReady,
                    false,
                    default,
                    EchoSaveDiagnosticCodes
                        .PublicSelectServiceNotReady,
                    "The Chronicle slot catalog is unavailable.");
            }

            return slotCatalog.SelectActiveSlot(
                slotId);
        }

        private PreparedLoadCreationResult PrepareLoadCore(
            SaveLoadRequest request)
        {
            if (state !=
                EchoSaveServiceState.Ready)
            {
                bool admissionClosed =
                    state ==
                        EchoSaveServiceState.ShuttingDown ||
                    state ==
                        EchoSaveServiceState.Shutdown;

                return PreparedLoadFailure(
                    admissionClosed
                        ? PreparedLoadCreationStatus.AdmissionClosed
                        : PreparedLoadCreationStatus.ServiceNotReady,
                    admissionClosed
                        ? EchoSaveDiagnosticCodes
                            .PublicLoadAdmissionClosed
                        : EchoSaveDiagnosticCodes
                            .PublicLoadServiceNotReady,
                    admissionClosed
                        ? "The Chronicle is not accepting load preparation after shutdown admission closed."
                        : "The Chronicle must be Ready before load preparation can begin.");
            }

            if (!TryValidateLoadRequest(
                    request,
                    out SaveSlotId slotId,
                    out PreparedLoadCreationResult requestFailure))
            {
                return requestFailure;
            }

            if (!HasPreparedLoadRuntime())
            {
                return PreparedLoadFailure(
                    PreparedLoadCreationStatus.ServiceNotReady,
                    EchoSaveDiagnosticCodes
                        .PublicLoadServiceNotReady,
                    "The Chronicle prepared-load runtime is unavailable.");
            }

            SaveOperationAdmissionStatus admission =
                saveOperationAdmission.TryAcquire(
                    out SaveOperationAdmissionLease lease);

            if (admission ==
                SaveOperationAdmissionStatus.Closed)
            {
                return PreparedLoadFailure(
                    PreparedLoadCreationStatus.AdmissionClosed,
                    EchoSaveDiagnosticCodes
                        .PublicLoadAdmissionClosed,
                    "The Chronicle is not accepting load preparation operations.");
            }

            if (admission ==
                SaveOperationAdmissionStatus.Busy)
            {
                return PreparedLoadFailure(
                    PreparedLoadCreationStatus.Busy,
                    EchoSaveDiagnosticCodes
                        .PublicLoadBusy,
                    "Another Chronicle operation already owns the root-local admission lease. Load preparation was rejected as Busy and was not queued.");
            }

            using (lease)
            {
                return PrepareLoadUnderAdmission(
                    slotId);
            }
        }

        private PreparedLoadCreationResult
            PrepareLoadUnderAdmission(
                SaveSlotId slotId)
        {
            SaveCurrentGenerationReadResult read =
                currentGenerationReader.ReadCurrent(
                    slotId);

            if (!read.Succeeded)
            {
                bool unavailable =
                    read.Status ==
                        SaveCurrentGenerationReadStatus.HeadUnavailable ||
                    read.Status ==
                        SaveCurrentGenerationReadStatus
                            .GenerationUnavailable;

                return PreparedLoadFailure(
                    unavailable
                        ? PreparedLoadCreationStatus.SourceUnavailable
                        : PreparedLoadCreationStatus.SourceInvalid,
                    string.IsNullOrEmpty(
                        read.DiagnosticCode)
                        ? unavailable
                            ? EchoSaveDiagnosticCodes
                                .PublicLoadSourceUnavailable
                            : EchoSaveDiagnosticCodes
                                .PublicLoadSourceInvalid
                        : read.DiagnosticCode,
                    string.IsNullOrEmpty(
                        read.Message)
                        ? unavailable
                            ? "The Chronicle load source is unavailable."
                            : "The Chronicle load source is invalid."
                        : read.Message);
            }

            SaveParticipantPreparationResult preparation =
                participantPayloadPreparer.Prepare(
                    read.ValidatedParticipants);

            if (preparation == null ||
                !preparation.Succeeded)
            {
                return PreparedLoadFailure(
                    PreparedLoadCreationStatus
                        .ParticipantPreparationFailed,
                    preparation == null ||
                    string.IsNullOrEmpty(
                        preparation.DiagnosticCode)
                        ? EchoSaveDiagnosticCodes
                            .PublicLoadPreparationFailed
                        : preparation.DiagnosticCode,
                    preparation == null ||
                    string.IsNullOrEmpty(
                        preparation.Message)
                        ? "The Chronicle participant payload set could not be prepared."
                        : preparation.Message);
            }

            SaveUnknownPayloadSnapshot unknowns =
                unknownPayloadStore.GetSnapshot();

            return preparedLoadStore.TryCreate(
                read,
                preparation,
                unknowns);
        }

        private SavePreparedLoadApplyResult
            ApplyPreparedLoadCore(
                PreparedSaveLoad handle)
        {
            if (state !=
                EchoSaveServiceState.Ready)
            {
                bool admissionClosed =
                    state ==
                        EchoSaveServiceState.ShuttingDown ||
                    state ==
                        EchoSaveServiceState.Shutdown;

                return PreparedApplyFailure(
                    admissionClosed
                        ? SavePreparedLoadApplyStatus.AdmissionClosed
                        : SavePreparedLoadApplyStatus.ServiceNotReady,
                    handle,
                    admissionClosed
                        ? EchoSaveDiagnosticCodes
                            .PublicLoadAdmissionClosed
                        : EchoSaveDiagnosticCodes
                            .PublicLoadServiceNotReady,
                    admissionClosed
                        ? "The Chronicle is not accepting prepared-load apply after shutdown admission closed."
                        : "The Chronicle must be Ready before a prepared load can be applied.");
            }

            if (preparedLoadApplyCoordinator == null)
            {
                return PreparedApplyFailure(
                    SavePreparedLoadApplyStatus.ServiceNotReady,
                    handle,
                    EchoSaveDiagnosticCodes
                        .PublicLoadServiceNotReady,
                    "The Chronicle prepared-load apply runtime is unavailable.");
            }

            SaveOperationAdmissionStatus admission =
                saveOperationAdmission.TryAcquire(
                    out SaveOperationAdmissionLease lease);

            if (admission ==
                SaveOperationAdmissionStatus.Closed)
            {
                return PreparedApplyFailure(
                    SavePreparedLoadApplyStatus.AdmissionClosed,
                    handle,
                    EchoSaveDiagnosticCodes
                        .PublicLoadAdmissionClosed,
                    "The Chronicle is not accepting prepared-load apply operations.");
            }

            if (admission ==
                SaveOperationAdmissionStatus.Busy)
            {
                return PreparedApplyFailure(
                    SavePreparedLoadApplyStatus.Busy,
                    handle,
                    EchoSaveDiagnosticCodes
                        .PublicLoadBusy,
                    "Another Chronicle operation already owns the root-local admission lease. Prepared-load apply was rejected as Busy and was not queued.");
            }

            using (lease)
            {
                return ApplyPreparedLoadUnderAdmission(
                    handle);
            }
        }

        private SavePreparedLoadApplyResult
            ApplyPreparedLoadUnderAdmission(
                PreparedSaveLoad handle) =>
            preparedLoadApplyCoordinator.Apply(
                handle);

        private SaveLoadResult LoadAndApplyCore(
            SaveLoadRequest request)
        {
            if (state !=
                EchoSaveServiceState.Ready)
            {
                bool admissionClosed =
                    state ==
                        EchoSaveServiceState.ShuttingDown ||
                    state ==
                        EchoSaveServiceState.Shutdown;

                return LoadFailure(
                    admissionClosed
                        ? SaveLoadStatus.AdmissionClosed
                        : SaveLoadStatus.ServiceNotReady,
                    request.SlotId,
                    default,
                    false,
                    false,
                    false,
                    false,
                    admissionClosed
                        ? EchoSaveDiagnosticCodes
                            .PublicLoadAdmissionClosed
                        : EchoSaveDiagnosticCodes
                            .PublicLoadServiceNotReady,
                    admissionClosed
                        ? "The Chronicle is not accepting convenience loads after shutdown admission closed."
                        : "The Chronicle must be Ready before convenience load can begin.",
                    null);
            }

            if (!TryValidateLoadRequest(
                    request,
                    out SaveSlotId slotId,
                    out PreparedLoadCreationResult requestFailure))
            {
                return LoadFailure(
                    SaveLoadStatus.InvalidRequest,
                    default,
                    default,
                    false,
                    false,
                    false,
                    false,
                    requestFailure.DiagnosticCode,
                    requestFailure.Message,
                    null);
            }

            if (!HasPreparedLoadRuntime())
            {
                return LoadFailure(
                    SaveLoadStatus.ServiceNotReady,
                    slotId,
                    default,
                    false,
                    false,
                    false,
                    false,
                    EchoSaveDiagnosticCodes
                        .PublicLoadServiceNotReady,
                    "The Chronicle prepared-load runtime is unavailable.",
                    null);
            }

            SaveOperationAdmissionStatus admission =
                saveOperationAdmission.TryAcquire(
                    out SaveOperationAdmissionLease lease);

            if (admission ==
                SaveOperationAdmissionStatus.Closed)
            {
                return LoadFailure(
                    SaveLoadStatus.AdmissionClosed,
                    slotId,
                    default,
                    false,
                    false,
                    false,
                    false,
                    EchoSaveDiagnosticCodes
                        .PublicLoadAdmissionClosed,
                    "The Chronicle is not accepting convenience load operations.",
                    null);
            }

            if (admission ==
                SaveOperationAdmissionStatus.Busy)
            {
                return LoadFailure(
                    SaveLoadStatus.Busy,
                    slotId,
                    default,
                    false,
                    false,
                    false,
                    false,
                    EchoSaveDiagnosticCodes
                        .PublicLoadBusy,
                    "Another Chronicle operation already owns the root-local admission lease. Convenience load was rejected as Busy and was not queued.",
                    null);
            }

            using (lease)
            {
                PreparedLoadCreationResult prepared =
                    PrepareLoadUnderAdmission(
                        slotId);

                if (!prepared.Succeeded)
                {
                    return LoadFailure(
                        SaveLoadStatus.PreparationFailed,
                        slotId,
                        default,
                        false,
                        false,
                        false,
                        false,
                        prepared.DiagnosticCode,
                        prepared.Message,
                        null);
                }

                PreparedSaveLoad handle =
                    prepared.Handle;

                SaveSlotId sourceSlot =
                    handle.SourceSlotId;

                SaveGenerationId sourceGeneration =
                    handle.SourceGenerationId;

                SavePreparedLoadApplyResult apply =
                    null;

                try
                {
                    apply =
                        ApplyPreparedLoadUnderAdmission(
                            handle);
                }
                finally
                {
                    if (handle != null &&
                        handle.IsValid)
                    {
                        handle.Dispose();
                    }
                }

                bool handleConsumed =
                    apply != null &&
                    apply.HandleConsumed;

                if (apply == null)
                {
                    return LoadFailure(
                        SaveLoadStatus.ApplyFailed,
                        sourceSlot,
                        sourceGeneration,
                        true,
                        true,
                        false,
                        handleConsumed,
                        EchoSaveDiagnosticCodes
                            .PublicLoadApplyFailed,
                        "The Chronicle convenience load produced no prepared-apply result.",
                        null);
                }

                if (!apply.Succeeded)
                {
                    return LoadFailure(
                        SaveLoadStatus.ApplyFailed,
                        sourceSlot,
                        sourceGeneration,
                        true,
                        true,
                        apply.MutationBegan,
                        handleConsumed,
                        string.IsNullOrEmpty(
                            apply.DiagnosticCode)
                            ? EchoSaveDiagnosticCodes
                                .PublicLoadApplyFailed
                            : apply.DiagnosticCode,
                        string.IsNullOrEmpty(
                            apply.Message)
                            ? "The Chronicle convenience-load apply phase failed."
                            : apply.Message,
                        apply);
                }

                return new SaveLoadResult(
                    SaveLoadStatus.Succeeded,
                    sourceSlot,
                    sourceGeneration,
                    true,
                    true,
                    apply.MutationBegan,
                    handleConsumed,
                    string.Empty,
                    "The Chronicle prepared and applied the requested slot in the current scene successfully.",
                    apply);
            }
        }

        private static bool TryValidateLoadRequest(
            SaveLoadRequest request,
            out SaveSlotId slotId,
            out PreparedLoadCreationResult failure)
        {
            slotId =
                default;

            failure =
                default;

            if (!SaveSlotId.TryParse(
                    request.SlotId.Value,
                    out slotId))
            {
                failure =
                    PreparedLoadFailure(
                        PreparedLoadCreationStatus.InvalidRequest,
                        EchoSaveDiagnosticCodes
                            .PublicLoadInvalidRequest,
                        "Chronicle load requires one valid explicit technical slot identity.");

                return false;
            }

            return true;
        }

        private bool HasPreparedLoadRuntime() =>
            currentGenerationReader != null &&
            participantPayloadPreparer != null &&
            unknownPayloadStore != null &&
            preparedLoadStore != null &&
            preparedLoadApplyCoordinator != null;

        private static PreparedLoadCreationResult
            PreparedLoadFailure(
                PreparedLoadCreationStatus status,
                string diagnosticCode,
                string message) =>
            new PreparedLoadCreationResult(
                status,
                null,
                diagnosticCode,
                message);

        private static SavePreparedLoadApplyResult
            PreparedApplyFailure(
                SavePreparedLoadApplyStatus status,
                PreparedSaveLoad handle,
                string diagnosticCode,
                string message) =>
            new SavePreparedLoadApplyResult(
                status,
                handle == null
                    ? default
                    : handle.SourceSlotId,
                handle == null
                    ? default
                    : handle.SourceGenerationId,
                false,
                false,
                default,
                diagnosticCode,
                message,
                Array.Empty<
                    SaveParticipantApplyReportEntry>());

        private static SaveLoadResult LoadFailure(
            SaveLoadStatus status,
            SaveSlotId sourceSlotId,
            SaveGenerationId sourceGenerationId,
            bool preparationSucceeded,
            bool applyAttempted,
            bool mutationBegan,
            bool handleConsumed,
            string diagnosticCode,
            string message,
            SavePreparedLoadApplyResult applyResult) =>
            new SaveLoadResult(
                status,
                sourceSlotId,
                sourceGenerationId,
                preparationSucceeded,
                applyAttempted,
                mutationBegan,
                handleConsumed,
                diagnosticCode,
                message,
                applyResult);
        private SaveRecoveryPlan BuildRecoveryPlanCore(
            SaveSlotId slotId)
        {
            if (state !=
                EchoSaveServiceState.Ready ||
                recoveryPlanBuilder == null)
            {
                return SaveRecoveryPlan.Failure(
                    SaveRecoveryPlanStatus.ServiceNotReady,
                    EchoSaveDiagnosticCodes
                        .RecoveryServiceNotReady,
                    "The Chronicle must be Ready before a recovery plan can be built.",
                    slotId);
            }

            if (!SaveSlotId.TryParse(
                    slotId.Value,
                    out SaveSlotId validatedSlot))
            {
                return SaveRecoveryPlan.Failure(
                    SaveRecoveryPlanStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes
                        .RecoveryInvalidRequest,
                    "Chronicle recovery planning requires one valid technical slot identity.",
                    slotId);
            }

            return recoveryPlanBuilder.Build(
                validatedSlot);
        }


        private SaveRecoveryResult ExecuteRecoveryCore(
            SaveRecoveryPlan plan,
            SaveRecoveryCandidate candidate)
        {
            if (state !=
                EchoSaveServiceState.Ready)
            {
                bool admissionClosed =
                    state ==
                        EchoSaveServiceState.ShuttingDown ||
                    state ==
                        EchoSaveServiceState.Shutdown;

                return SaveRecoveryResult.Failure(
                    admissionClosed
                        ? SaveRecoveryExecutionStatus
                            .AdmissionClosed
                        : SaveRecoveryExecutionStatus
                            .ServiceNotReady,
                    admissionClosed
                        ? EchoSaveDiagnosticCodes
                            .RecoveryExecuteAdmissionClosed
                        : EchoSaveDiagnosticCodes
                            .RecoveryExecuteServiceNotReady,
                    admissionClosed
                        ? "The Chronicle is not accepting new recovery mutations after admission closed."
                        : "The Chronicle must be Ready before explicit recovery execution can begin.",
                    plan == null
                        ? default
                        : plan.SlotId,
                    candidate.GenerationId);
            }

            if (recoveryExecutor == null)
            {
                return SaveRecoveryResult.Failure(
                    SaveRecoveryExecutionStatus
                        .ServiceNotReady,
                    EchoSaveDiagnosticCodes
                        .RecoveryExecuteServiceNotReady,
                    "The Chronicle recovery-execution runtime is unavailable.",
                    plan == null
                        ? default
                        : plan.SlotId,
                    candidate.GenerationId);
            }

            SaveOperationAdmissionStatus admission =
                saveOperationAdmission.TryAcquire(
                    out SaveOperationAdmissionLease lease);

            if (admission ==
                SaveOperationAdmissionStatus.Closed)
            {
                return SaveRecoveryResult.Failure(
                    SaveRecoveryExecutionStatus
                        .AdmissionClosed,
                    EchoSaveDiagnosticCodes
                        .RecoveryExecuteAdmissionClosed,
                    "The Chronicle is not accepting new mutating operations.",
                    plan == null
                        ? default
                        : plan.SlotId,
                    candidate.GenerationId);
            }

            if (admission ==
                SaveOperationAdmissionStatus.Busy)
            {
                return SaveRecoveryResult.Failure(
                    SaveRecoveryExecutionStatus.Busy,
                    EchoSaveDiagnosticCodes
                        .RecoveryExecuteBusy,
                    "Another Chronicle mutating operation already owns the root-local admission lease. Recovery execution was rejected as Busy and was not queued.",
                    plan == null
                        ? default
                        : plan.SlotId,
                    candidate.GenerationId);
            }

            using (lease)
            {
                return recoveryExecutor.Execute(
                    plan,
                    candidate);
            }
        }



        private SaveSlotRenameResult RenameSlotCore(
            SaveSlotRenameRequest request)
        {
            if (state != EchoSaveServiceState.Ready)
            {
                bool admissionClosed =
                    state == EchoSaveServiceState.ShuttingDown ||
                    state == EchoSaveServiceState.Shutdown;

                return SaveSlotRenameResult.Failure(
                    admissionClosed
                        ? SaveSlotRenameStatus.AdmissionClosed
                        : SaveSlotRenameStatus.ServiceNotReady,
                    admissionClosed
                        ? EchoSaveDiagnosticCodes.SlotRenameAdmissionClosed
                        : EchoSaveDiagnosticCodes.SlotRenameServiceNotReady,
                    admissionClosed
                        ? "The Chronicle is not accepting new slot-rename mutations after admission closed."
                        : "The Chronicle must be Ready before slot rename can begin.",
                    request == null
                        ? default
                        : request.SlotId);
            }

            if (slotMutationCoordinator == null)
            {
                return SaveSlotRenameResult.Failure(
                    SaveSlotRenameStatus.ServiceNotReady,
                    EchoSaveDiagnosticCodes.SlotRenameServiceNotReady,
                    "The Chronicle slot-mutation runtime is unavailable.",
                    request == null
                        ? default
                        : request.SlotId);
            }

            SaveOperationAdmissionStatus admission =
                saveOperationAdmission.TryAcquire(
                    out SaveOperationAdmissionLease lease);

            if (admission == SaveOperationAdmissionStatus.Closed)
            {
                return SaveSlotRenameResult.Failure(
                    SaveSlotRenameStatus.AdmissionClosed,
                    EchoSaveDiagnosticCodes.SlotRenameAdmissionClosed,
                    "The Chronicle is not accepting new mutating operations.",
                    request == null
                        ? default
                        : request.SlotId);
            }

            if (admission == SaveOperationAdmissionStatus.Busy)
            {
                return SaveSlotRenameResult.Failure(
                    SaveSlotRenameStatus.Busy,
                    EchoSaveDiagnosticCodes.SlotRenameBusy,
                    "Another Chronicle mutating operation already owns the root-local admission lease. Slot rename was rejected as Busy and was not queued.",
                    request == null
                        ? default
                        : request.SlotId);
            }

            using (lease)
            {
                return slotMutationCoordinator.Rename(
                    request);
            }
        }

        private SaveSlotDuplicateResult DuplicateSlotCore(
            SaveSlotDuplicateRequest request)
        {
            if (state != EchoSaveServiceState.Ready)
            {
                bool admissionClosed =
                    state == EchoSaveServiceState.ShuttingDown ||
                    state == EchoSaveServiceState.Shutdown;

                return SaveSlotDuplicateResult.Failure(
                    admissionClosed
                        ? SaveSlotDuplicateStatus.AdmissionClosed
                        : SaveSlotDuplicateStatus.ServiceNotReady,
                    admissionClosed
                        ? EchoSaveDiagnosticCodes.SlotDuplicateAdmissionClosed
                        : EchoSaveDiagnosticCodes.SlotDuplicateServiceNotReady,
                    admissionClosed
                        ? "The Chronicle is not accepting new slot-duplicate mutations after admission closed."
                        : "The Chronicle must be Ready before slot duplication can begin.",
                    request == null
                        ? default
                        : request.SourceSlotId);
            }

            if (slotMutationCoordinator == null)
            {
                return SaveSlotDuplicateResult.Failure(
                    SaveSlotDuplicateStatus.ServiceNotReady,
                    EchoSaveDiagnosticCodes.SlotDuplicateServiceNotReady,
                    "The Chronicle slot-mutation runtime is unavailable.",
                    request == null
                        ? default
                        : request.SourceSlotId);
            }

            SaveOperationAdmissionStatus admission =
                saveOperationAdmission.TryAcquire(
                    out SaveOperationAdmissionLease lease);

            if (admission == SaveOperationAdmissionStatus.Closed)
            {
                return SaveSlotDuplicateResult.Failure(
                    SaveSlotDuplicateStatus.AdmissionClosed,
                    EchoSaveDiagnosticCodes.SlotDuplicateAdmissionClosed,
                    "The Chronicle is not accepting new mutating operations.",
                    request == null
                        ? default
                        : request.SourceSlotId);
            }

            if (admission == SaveOperationAdmissionStatus.Busy)
            {
                return SaveSlotDuplicateResult.Failure(
                    SaveSlotDuplicateStatus.Busy,
                    EchoSaveDiagnosticCodes.SlotDuplicateBusy,
                    "Another Chronicle mutating operation already owns the root-local admission lease. Slot duplication was rejected as Busy and was not queued.",
                    request == null
                        ? default
                        : request.SourceSlotId);
            }

            using (lease)
            {
                return slotMutationCoordinator.Duplicate(
                    request);
            }
        }


        private SaveDeletionPlan PrepareDeleteSlotCore(
            SaveSlotId slotId)
        {
            if (state != EchoSaveServiceState.Ready ||
                slotDeletionCoordinator == null)
            {
                return SaveDeletionPlan.Failure(
                    SaveDeletionPlanStatus.ServiceNotReady,
                    EchoSaveDiagnosticCodes.DeletePlanServiceNotReady,
                    "The Chronicle must be Ready before a deletion plan can be prepared.",
                    slotId);
            }

            return slotDeletionCoordinator.Prepare(
                slotId);
        }

        private SaveSlotDeleteResult ConfirmDeleteSlotCore(
            SaveDeletionPlan plan)
        {
            if (state != EchoSaveServiceState.Ready)
            {
                bool admissionClosed =
                    state == EchoSaveServiceState.ShuttingDown ||
                    state == EchoSaveServiceState.Shutdown;

                return SaveSlotDeleteResult.Failure(
                    admissionClosed
                        ? SaveSlotDeleteStatus.AdmissionClosed
                        : SaveSlotDeleteStatus.ServiceNotReady,
                    admissionClosed
                        ? EchoSaveDiagnosticCodes.DeleteConfirmAdmissionClosed
                        : EchoSaveDiagnosticCodes.DeleteConfirmServiceNotReady,
                    admissionClosed
                        ? "The Chronicle is not accepting confirmed delete mutations after admission closed."
                        : "The Chronicle must be Ready before confirmed deletion can begin.",
                    plan == null
                        ? default
                        : plan.SlotId,
                    plan == null
                        ? default
                        : plan.CurrentGenerationId);
            }

            if (slotDeletionCoordinator == null)
            {
                return SaveSlotDeleteResult.Failure(
                    SaveSlotDeleteStatus.ServiceNotReady,
                    EchoSaveDiagnosticCodes.DeleteConfirmServiceNotReady,
                    "The Chronicle slot-deletion runtime is unavailable.",
                    plan == null
                        ? default
                        : plan.SlotId,
                    plan == null
                        ? default
                        : plan.CurrentGenerationId);
            }

            SaveOperationAdmissionStatus admission =
                saveOperationAdmission.TryAcquire(
                    out SaveOperationAdmissionLease lease);

            if (admission == SaveOperationAdmissionStatus.Closed)
            {
                return SaveSlotDeleteResult.Failure(
                    SaveSlotDeleteStatus.AdmissionClosed,
                    EchoSaveDiagnosticCodes.DeleteConfirmAdmissionClosed,
                    "The Chronicle is not accepting new mutating operations.",
                    plan == null
                        ? default
                        : plan.SlotId,
                    plan == null
                        ? default
                        : plan.CurrentGenerationId);
            }

            if (admission == SaveOperationAdmissionStatus.Busy)
            {
                return SaveSlotDeleteResult.Failure(
                    SaveSlotDeleteStatus.Busy,
                    EchoSaveDiagnosticCodes.DeleteConfirmBusy,
                    "Another Chronicle mutating operation already owns the root-local admission lease. Confirmed deletion was rejected as Busy and was not queued.",
                    plan == null
                        ? default
                        : plan.SlotId,
                    plan == null
                        ? default
                        : plan.CurrentGenerationId);
            }

            using (lease)
            {
                return slotDeletionCoordinator.Confirm(
                    plan);
            }
        }

        private SaveOperationResult SaveCore(
            SaveRequest request)
        {
            if (state !=
                EchoSaveServiceState.Ready)
            {
                bool lifecycleAdmissionClosed =
                    state ==
                        EchoSaveServiceState.ShuttingDown ||
                    state ==
                        EchoSaveServiceState.Shutdown;

                return Failure(
                    lifecycleAdmissionClosed
                        ? SaveOperationStatus.AdmissionClosed
                        : SaveOperationStatus.ServiceNotReady,
                    lifecycleAdmissionClosed
                        ? EchoSaveDiagnosticCodes
                            .PublicSaveAdmissionClosed
                        : EchoSaveDiagnosticCodes
                            .PublicSaveServiceNotReady,
                    lifecycleAdmissionClosed
                        ? "The Chronicle is not accepting new manual-save operations."
                        : "The Chronicle must be Ready before public manual save can begin.");
            }

            if (!ValidatePublicSaveRequest(
                    request,
                    out string requestMessage))
            {
                return Failure(
                    SaveOperationStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes
                        .PublicSaveInvalidRequest,
                    requestMessage);
            }

            if (request.CancellationToken
                .IsCancellationRequested)
            {
                return Failure(
                    SaveOperationStatus.Canceled,
                    EchoSaveDiagnosticCodes
                        .ManualSaveCanceled,
                    "The Chronicle manual-save request was already canceled.",
                    SaveCancellationDisposition.Canceled);
            }

            SaveOperationAdmissionStatus admission =
                saveOperationAdmission.TryAcquire(
                    out SaveOperationAdmissionLease lease);

            if (admission ==
                SaveOperationAdmissionStatus.Closed)
            {
                return Failure(
                    SaveOperationStatus.AdmissionClosed,
                    EchoSaveDiagnosticCodes
                        .PublicSaveAdmissionClosed,
                    "The Chronicle is not accepting new mutating operations.");
            }

            if (admission ==
                SaveOperationAdmissionStatus.Busy)
            {
                return Failure(
                    SaveOperationStatus.Busy,
                    EchoSaveDiagnosticCodes
                        .PublicSaveBusy,
                    "Another Chronicle mutating operation already owns the root-local admission lease. Manual save was rejected as Busy and was not queued.");
            }

            using (lease)
            {
                return ExecuteSaveUnderAdmission(
                    request);
            }
        }

        private SaveOperationResult ExecuteSaveUnderAdmission(
            SaveRequest request)
        {
            if (request.CancellationToken
                .IsCancellationRequested)
            {
                return Failure(
                    SaveOperationStatus.Canceled,
                    EchoSaveDiagnosticCodes
                        .ManualSaveCanceled,
                    "The Chronicle save request was canceled before transaction execution.",
                    SaveCancellationDisposition.Canceled);
            }

            if (manualSaveTransactionExecutor == null)
            {
                return Failure(
                    SaveOperationStatus.ServiceNotReady,
                    EchoSaveDiagnosticCodes
                        .PublicSaveServiceNotReady,
                    "The Chronicle save transaction runtime is unavailable.");
            }

            SaveManualTransactionControl control =
                new SaveManualTransactionControl(
                    request.CancellationToken);

            SaveManualTransactionResult transaction =
                manualSaveTransactionExecutor.Save(
                    new SaveManualTransactionRequest(
                        request.ProjectId,
                        request.ProjectVersion,
                        request.BuildId),
                    control);

            return MapManualSaveResult(
                transaction,
                control,
                request.CancellationToken
                    .IsCancellationRequested);
        }

        private AutosaveSubmissionResult RequestAutosaveCore(
            AutosaveRequest request)
        {
            if (!TryValidateAutosaveSubmission(
                    request,
                    out AutosaveSubmissionStatus rejectionStatus,
                    out string diagnosticCode,
                    out string message))
            {
                return AutosaveRejected(
                    rejectionStatus,
                    diagnosticCode,
                    message);
            }

            AutosaveTicket ticket =
                CreateAutosaveTicket();

            SaveOperationAdmissionStatus admission =
                saveOperationAdmission.TryAcquire(
                    out SaveOperationAdmissionLease lease);

            if (admission ==
                SaveOperationAdmissionStatus.Closed)
            {
                ticket.MarkDiscarded(
                    EchoSaveDiagnosticCodes
                        .AutosaveAdmissionClosed,
                    "The Chronicle autosave request was rejected because mutating admission is closed.",
                    false);

                return AutosaveRejected(
                    AutosaveSubmissionStatus
                        .RejectedAdmissionClosed,
                    EchoSaveDiagnosticCodes
                        .AutosaveAdmissionClosed,
                    ticket.Message);
            }

            if (admission ==
                SaveOperationAdmissionStatus.Admitted)
            {
                SaveOperationResult result;

                using (lease)
                {
                    ticket.MarkExecuting();

                    result =
                        ExecuteAutosaveUnderAdmission(
                            request);

                    ticket.Complete(
                        result);
                }

                return new AutosaveSubmissionResult(
                    AutosaveSubmissionStatus.Executed,
                    EchoSaveDiagnosticCodes
                        .AutosaveExecuted,
                    "The Chronicle admitted and executed this autosave request.",
                    ticket,
                    null,
                    true,
                    result);
            }

            AutosaveTicket superseded =
                pendingAutosave?.Ticket;

            if (superseded != null)
            {
                superseded.MarkSuperseded();
            }

            ticket.MarkPending(
                superseded == null
                    ? EchoSaveDiagnosticCodes
                        .AutosavePending
                    : EchoSaveDiagnosticCodes
                        .AutosaveCoalesced,
                superseded == null
                    ? "The Chronicle retained this autosave as the one pending latest request."
                    : "The Chronicle replaced the prior pending autosave with this newer latest request.");

            pendingAutosave =
                new PendingAutosaveRequest(
                    request,
                    ticket);

            return new AutosaveSubmissionResult(
                superseded == null
                    ? AutosaveSubmissionStatus.Pending
                    : AutosaveSubmissionStatus.Coalesced,
                superseded == null
                    ? EchoSaveDiagnosticCodes
                        .AutosavePending
                    : EchoSaveDiagnosticCodes
                        .AutosaveCoalesced,
                ticket.Message,
                ticket,
                superseded,
                false,
                default);
        }

        private SaveOperationResult ExecuteAutosaveUnderAdmission(
            AutosaveRequest request) =>
            ExecuteSaveUnderAdmission(
                new SaveRequest(
                    request.ProjectId,
                    request.ProjectVersion,
                    request.BuildId,
                    request.CancellationToken));

        private void OnOperationAdmissionAvailable()
        {
            if (drainingPendingAutosave ||
                pendingAutosave == null)
            {
                return;
            }

            DrainPendingAutosave();
        }

        private void DrainPendingAutosave()
        {
            if (drainingPendingAutosave)
            {
                return;
            }

            drainingPendingAutosave =
                true;

            try
            {
                PendingAutosaveRequest pending =
                    pendingAutosave;

                if (pending == null)
                {
                    return;
                }

                pendingAutosave =
                    null;

                if (!TryValidateAutosaveSubmission(
                        pending.Request,
                        out AutosaveSubmissionStatus rejectionStatus,
                        out string diagnosticCode,
                        out string message))
                {
                    pending.Ticket.MarkDiscarded(
                        diagnosticCode,
                        message,
                        rejectionStatus ==
                            AutosaveSubmissionStatus
                                .RejectedCanceled);

                    return;
                }

                SaveOperationAdmissionStatus admission =
                    saveOperationAdmission.TryAcquire(
                        out SaveOperationAdmissionLease lease);

                if (admission ==
                    SaveOperationAdmissionStatus.Closed)
                {
                    pending.Ticket.MarkDiscarded(
                        EchoSaveDiagnosticCodes
                            .AutosaveAdmissionClosed,
                        "The Chronicle discarded the pending autosave because admission closed before execution.",
                        false);

                    return;
                }

                if (admission ==
                    SaveOperationAdmissionStatus.Busy)
                {
                    pendingAutosave =
                        pending;

                    return;
                }

                using (lease)
                {
                    pending.Ticket.MarkExecuting();

                    SaveOperationResult result =
                        ExecuteAutosaveUnderAdmission(
                            pending.Request);

                    pending.Ticket.Complete(
                        result);
                }
            }
            finally
            {
                drainingPendingAutosave =
                    false;
            }

            if (pendingAutosave != null &&
                state ==
                    EchoSaveServiceState.Ready &&
                !saveOperationAdmission.IsClosed &&
                !saveOperationAdmission.IsOccupied)
            {
                DrainPendingAutosave();
            }
        }

        private bool TryValidateAutosaveSubmission(
            AutosaveRequest request,
            out AutosaveSubmissionStatus rejectionStatus,
            out string diagnosticCode,
            out string message)
        {
            rejectionStatus =
                default;

            diagnosticCode =
                string.Empty;

            message =
                string.Empty;

            if (state !=
                EchoSaveServiceState.Ready)
            {
                bool admissionClosed =
                    state ==
                        EchoSaveServiceState.ShuttingDown ||
                    state ==
                        EchoSaveServiceState.Shutdown;

                rejectionStatus =
                    admissionClosed
                        ? AutosaveSubmissionStatus
                            .RejectedAdmissionClosed
                        : AutosaveSubmissionStatus
                            .RejectedServiceNotReady;

                diagnosticCode =
                    admissionClosed
                        ? EchoSaveDiagnosticCodes
                            .AutosaveAdmissionClosed
                        : EchoSaveDiagnosticCodes
                            .AutosaveServiceNotReady;

                message =
                    admissionClosed
                        ? "The Chronicle is not accepting autosave requests after shutdown admission closes."
                        : "The Chronicle must be Ready before autosave can be requested.";

                return false;
            }

            if (!Bounded(
                    request.ProjectId) ||
                !Bounded(
                    request.ProjectVersion) ||
                !Bounded(
                    request.BuildId))
            {
                rejectionStatus =
                    AutosaveSubmissionStatus
                        .RejectedInvalidRequest;

                diagnosticCode =
                    EchoSaveDiagnosticCodes
                        .AutosaveInvalidRequest;

                message =
                    "Chronicle autosave metadata must be non-null and no longer than 256 characters per field.";

                return false;
            }

            if (request.CancellationToken
                .IsCancellationRequested)
            {
                rejectionStatus =
                    AutosaveSubmissionStatus
                        .RejectedCanceled;

                diagnosticCode =
                    EchoSaveDiagnosticCodes
                        .AutosaveCanceled;

                message =
                    "The Chronicle autosave request was already canceled.";

                return false;
            }

            if (!HasSelectableActiveSlot())
            {
                rejectionStatus =
                    AutosaveSubmissionStatus
                        .RejectedNoActiveSlot;

                diagnosticCode =
                    EchoSaveDiagnosticCodes
                        .AutosaveNoActiveSlot;

                message =
                    "Chronicle autosave requires one explicitly selected healthy active slot.";

                return false;
            }

            return true;
        }

        private bool HasSelectableActiveSlot()
        {
            if (slotCatalog == null ||
                !slotCatalog.HasActiveSlot)
            {
                return false;
            }

            return slotCatalog.Snapshot.TryGetEntry(
                       slotCatalog.ActiveSlotId,
                       out SaveSlotCatalogEntry entry) &&
                   entry != null &&
                   entry.IsSelectable;
        }

        private AutosaveTicket CreateAutosaveTicket()
        {
            if (nextAutosaveTicketId ==
                long.MaxValue)
            {
                nextAutosaveTicketId =
                    0L;
            }

            long id =
                ++nextAutosaveTicketId;

            if (id == 0L)
            {
                id =
                    ++nextAutosaveTicketId;
            }

            return new AutosaveTicket(
                id);
        }

        private void DiscardPendingAutosave(
            string diagnosticCode,
            string message)
        {
            PendingAutosaveRequest pending =
                pendingAutosave;

            pendingAutosave =
                null;

            pending?.Ticket.MarkDiscarded(
                diagnosticCode,
                message,
                false);
        }

        private static AutosaveSubmissionResult AutosaveRejected(
            AutosaveSubmissionStatus status,
            string diagnosticCode,
            string message) =>
            new AutosaveSubmissionResult(
                status,
                diagnosticCode,
                message,
                null,
                null,
                false,
                default);

        private void BuildManualSaveRuntime()
        {
            ISaveSerializer serializer =
                new UnityJsonSaveSerializer();

            IIntegrityProvider integrity =
                new Sha256IntegrityProvider();

            serializerRegistry =
                new SaveSerializerRegistry();

            participantMigrationRegistry =
                new SaveParticipantMigrationRegistry();

            participantRegistry =
                new SaveParticipantRegistry();

            unknownPayloadStore =
                new SaveUnknownPayloadStore();

            slotCatalog =
                new SaveSlotCatalog(
                    storageBackend,
                    serializer,
                    DefaultCatalogScanLimit);

            currentGenerationReader =
                new SaveCurrentGenerationReader(
                    storageBackend,
                    serializer,
                    integrity,
                    participantRegistry,
                    unknownPayloadStore);

            participantPayloadPreparer =
                new SaveParticipantPayloadPreparer(
                    participantRegistry,
                    serializerRegistry,
                    participantMigrationRegistry);

            preparedLoadStore =
                new SavePreparedLoadStore();

            preparedLoadApplyCoordinator =
                new SavePreparedLoadApplyCoordinator(
                    preparedLoadStore,
                    participantRegistry);

            SaveParticipantCaptureCoordinator captureCoordinator =
                new SaveParticipantCaptureCoordinator(
                    serializerRegistry,
                    integrity);

            SaveGenerationPublicationCoordinator publicationCoordinator =
                new SaveGenerationPublicationCoordinator(
                    storageBackend,
                    serializer,
                    integrity);

            technicalSlotCreationCoordinator =
                new SaveTechnicalSlotCreationCoordinator(
                    slotCatalog,
                    publicationCoordinator,
                    DefaultTechnicalSlotCapacity,
                    DefaultSlotIdentityAttempts,
                    SaveSlotId.NewId);

            SaveUnknownPayloadCarryForwardCoordinator
                carryForwardCoordinator =
                    new SaveUnknownPayloadCarryForwardCoordinator(
                        storageBackend,
                        serializer,
                        integrity,
                        participantRegistry,
                        publicationCoordinator);

            SaveGenerationRetentionCoordinator
                retentionCoordinator =
                    new SaveGenerationRetentionCoordinator(
                        storageBackend,
                        serializer,
                        DefaultRetentionDiscoveryLimit);

            recoveryPlanBuilder =
                new SaveRecoveryPlanBuilder(
                    storageBackend,
                    serializer,
                    integrity,
                    DefaultRecoveryDiscoveryLimit);

            recoveryExecutor =
                new SaveRecoveryExecutionCoordinator(
                    storageBackend,
                    serializer,
                    recoveryPlanBuilder,
                    slotCatalog);

            SaveSlotMutationSourceReader
                slotMutationSourceReader =
                    new SaveSlotMutationSourceReader(
                        storageBackend,
                        serializer,
                        integrity);

            slotMutationCoordinator =
                new SaveSlotMutationCoordinator(
                    slotCatalog,
                    slotMutationSourceReader,
                    publicationCoordinator,
                    retentionCoordinator,
                    SaveRetentionPolicy.Default,
                    DefaultTechnicalSlotCapacity,
                    DefaultSlotIdentityAttempts,
                    SaveSlotId.NewId);

            slotDeletionCoordinator =
                new SaveSlotDeletionCoordinator(
                    slotCatalog,
                    storageBackend,
                    serializer,
                    integrity,
                    Guid.NewGuid().ToString("N"),
                    () => DateTimeOffset.UtcNow,
                    DefaultDeletionPlanLifetime,
                    DefaultCatalogScanLimit,
                    DefaultTrashDiscoveryLimit,
                    DefaultTrashRetentionRecords,
                    DefaultTrashIdentityAttempts);

            manualSaveTransactionExecutor =
                new SaveManualTransactionCoordinator(
                    slotCatalog,
                    currentGenerationReader,
                    captureCoordinator,
                    participantRegistry,
                    unknownPayloadStore,
                    carryForwardCoordinator,
                    retentionCoordinator,
                    SaveRetentionPolicy.Default);
        }

        private void ResetManualSaveRuntime()
        {
            preparedLoadStore?.Dispose();

            participantMigrationRegistry?.Clear();
            participantRegistry?.Clear();

            manualSaveTransactionExecutor =
                null;

            recoveryPlanBuilder =
                null;

            recoveryExecutor =
                null;

            slotMutationCoordinator =
                null;

            slotDeletionCoordinator =
                null;

            technicalSlotCreationCoordinator =
                null;

            preparedLoadApplyCoordinator =
                null;

            preparedLoadStore =
                null;

            participantPayloadPreparer =
                null;

            currentGenerationReader =
                null;

            participantMigrationRegistry =
                null;

            serializerRegistry =
                null;

            slotCatalog =
                null;

            unknownPayloadStore =
                null;

            participantRegistry =
                null;
        }
        private EchoSaveLifecycleResult BlockInitialization(
            string diagnosticCode,
            string message)
        {
            saveOperationAdmission.Close();
            DiscardPendingAutosave(
                EchoSaveDiagnosticCodes.AutosaveDiscarded,
                "The Chronicle discarded pending autosave state because initialization blocked.");
            ResetManualSaveRuntime();

            state =
                EchoSaveServiceState.Blocked;

            return new EchoSaveLifecycleResult(
                EchoSaveLifecycleStatus.Blocked,
                state,
                diagnosticCode,
                message);
        }

        private static bool ValidatePublicSaveRequest(
            SaveRequest request,
            out string message)
        {
            message =
                string.Empty;

            if (!Bounded(
                    request.ProjectId) ||
                !Bounded(
                    request.ProjectVersion) ||
                !Bounded(
                    request.BuildId))
            {
                message =
                    "Chronicle public manual-save metadata must be non-null and no longer than 256 characters per field.";

                return false;
            }

            return true;
        }

        private static bool Bounded(
            string value) =>
            value != null &&
            value.Length <=
                SaveManualTransactionCoordinator
                    .MaximumMetadataTextLength;

        private static SaveOperationResult MapManualSaveResult(
            SaveManualTransactionResult transaction,
            SaveManualTransactionControl control,
            bool cancellationRequested)
        {
            if (transaction == null)
            {
                return Failure(
                    SaveOperationStatus.TransactionFailed,
                    EchoSaveDiagnosticCodes
                        .ManualSavePublicationFailed,
                    "The Chronicle manual-save transaction returned no terminal result.");
            }

            SaveOperationStatus status;

            switch (transaction.Status)
            {
                case SaveManualTransactionStatus.Succeeded:
                    status =
                        SaveOperationStatus.Succeeded;
                    break;

                case SaveManualTransactionStatus.InvalidRequest:
                    status =
                        SaveOperationStatus.InvalidRequest;
                    break;

                case SaveManualTransactionStatus.Canceled:
                    status =
                        SaveOperationStatus.Canceled;
                    break;

                case SaveManualTransactionStatus
                    .PublishedCatalogReconciliationFailed:
                    status =
                        SaveOperationStatus
                            .PublishedCatalogReconciliationFailed;
                    break;

                default:
                    status =
                        SaveOperationStatus.TransactionFailed;
                    break;
            }

            SaveCancellationDisposition cancellationDisposition =
                transaction.Status ==
                SaveManualTransactionStatus.Canceled
                    ? SaveCancellationDisposition.Canceled
                    : control != null &&
                      control.PublicationStarted &&
                      cancellationRequested
                        ? SaveCancellationDisposition.TooLate
                        : SaveCancellationDisposition.None;

            string diagnosticCode =
                transaction.DiagnosticCode;

            string message =
                transaction.Message;

            if (cancellationDisposition ==
                SaveCancellationDisposition.TooLate)
            {
                if (string.IsNullOrEmpty(
                        diagnosticCode) &&
                    transaction.Succeeded)
                {
                    diagnosticCode =
                        EchoSaveDiagnosticCodes
                            .PublicSaveCancellationTooLate;
                }

                message =
                    string.IsNullOrEmpty(
                        message)
                        ? "Chronicle cancellation arrived after durable publication began and was too late to stop the transaction."
                        : message +
                          " Cancellation arrived after durable publication began and was too late to stop the transaction.";
            }

            return new SaveOperationResult(
                status,
                cancellationDisposition,
                diagnosticCode,
                message,
                transaction.SlotId,
                transaction.SourceGenerationId,
                transaction.PublishedGenerationId,
                transaction.FailingParticipantId,
                transaction.CurrentOwnerId,
                transaction.FreshParticipantCount,
                transaction.PreservedUnknownCount,
                transaction.TotalPayloadBytes,
                transaction.GenerationPublished,
                transaction.HeadPublished,
                transaction.CatalogReconciled,
                transaction.ReconciledEntry,
                transaction.RetentionResult);
        }

        private static SaveOperationResult Failure(
            SaveOperationStatus status,
            string diagnosticCode,
            string message,
            SaveCancellationDisposition cancellationDisposition =
                SaveCancellationDisposition.None) =>
            new SaveOperationResult(
                status,
                cancellationDisposition,
                diagnosticCode,
                message,
                default,
                default,
                default,
                default,
                default,
                0,
                0,
                0L,
                false,
                false,
                false,
                null);

        private void EnsurePreInitializationMutationAllowed(
            string subject)
        {
            if (state !=
                    EchoSaveServiceState.AuthorityClaimed &&
                state !=
                    EchoSaveServiceState.Blocked)
            {
                throw new InvalidOperationException(
                    $"{subject} may only be replaced before successful initialization.");
            }
        }
    }
}
