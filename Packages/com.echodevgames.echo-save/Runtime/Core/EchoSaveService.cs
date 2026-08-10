using System;
using UnityEngine;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Chronicle lifecycle and bounded public manual-save service.
    ///
    /// M4-04 adds the first public active-slot SaveAsync facade plus one
    /// root-local mutating-operation admission authority. Autosave, generic
    /// queue policy, retention, recovery, and later slot operations remain
    /// intentionally deferred.
    /// </summary>
    internal sealed class EchoSaveService :
        IEchoSaveService
    {
        internal const int DefaultCatalogScanLimit =
            256;

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
        private ISaveManualTransactionExecutor
            manualSaveTransactionExecutor;

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
        }

        public EchoSaveServiceState State =>
            state;

        public EchoSaveConfiguration Configuration =>
            configuration;

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

        internal SaveOperationResult
            SaveSynchronouslyForTesting(
                SaveRequest request) =>
            SaveCore(
                request);

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

        private SaveOperationResult SaveCore(
            SaveRequest request)
        {
            if (state !=
                EchoSaveServiceState.Ready)
            {
                bool admissionClosed =
                    saveOperationAdmission.IsClosed ||
                    state ==
                        EchoSaveServiceState.ShuttingDown ||
                    state ==
                        EchoSaveServiceState.Shutdown;

                return Failure(
                    admissionClosed
                        ? SaveOperationStatus.AdmissionClosed
                        : SaveOperationStatus.ServiceNotReady,
                    admissionClosed
                        ? EchoSaveDiagnosticCodes
                            .PublicSaveAdmissionClosed
                        : EchoSaveDiagnosticCodes
                            .PublicSaveServiceNotReady,
                    admissionClosed
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
                if (request.CancellationToken
                    .IsCancellationRequested)
                {
                    return Failure(
                        SaveOperationStatus.Canceled,
                        EchoSaveDiagnosticCodes
                            .ManualSaveCanceled,
                        "The Chronicle manual-save request was canceled before transaction execution.",
                        SaveCancellationDisposition.Canceled);
                }

                if (manualSaveTransactionExecutor == null)
                {
                    return Failure(
                        SaveOperationStatus.ServiceNotReady,
                        EchoSaveDiagnosticCodes
                            .PublicSaveServiceNotReady,
                        "The Chronicle manual-save transaction runtime is unavailable.");
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
        }

        private void BuildManualSaveRuntime()
        {
            ISaveSerializer serializer =
                new UnityJsonSaveSerializer();

            IIntegrityProvider integrity =
                new Sha256IntegrityProvider();

            SaveSerializerRegistry serializerRegistry =
                new SaveSerializerRegistry();

            participantRegistry =
                new SaveParticipantRegistry();

            unknownPayloadStore =
                new SaveUnknownPayloadStore();

            slotCatalog =
                new SaveSlotCatalog(
                    storageBackend,
                    serializer,
                    DefaultCatalogScanLimit);

            SaveCurrentGenerationReader currentReader =
                new SaveCurrentGenerationReader(
                    storageBackend,
                    serializer,
                    integrity,
                    participantRegistry,
                    unknownPayloadStore);

            SaveParticipantCaptureCoordinator captureCoordinator =
                new SaveParticipantCaptureCoordinator(
                    serializerRegistry,
                    integrity);

            SaveGenerationPublicationCoordinator publicationCoordinator =
                new SaveGenerationPublicationCoordinator(
                    storageBackend,
                    serializer,
                    integrity);

            SaveUnknownPayloadCarryForwardCoordinator
                carryForwardCoordinator =
                    new SaveUnknownPayloadCarryForwardCoordinator(
                        storageBackend,
                        serializer,
                        integrity,
                        participantRegistry,
                        publicationCoordinator);

            manualSaveTransactionExecutor =
                new SaveManualTransactionCoordinator(
                    slotCatalog,
                    currentReader,
                    captureCoordinator,
                    participantRegistry,
                    unknownPayloadStore,
                    carryForwardCoordinator);
        }

        private void ResetManualSaveRuntime()
        {
            manualSaveTransactionExecutor =
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
                transaction.ReconciledEntry);
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
