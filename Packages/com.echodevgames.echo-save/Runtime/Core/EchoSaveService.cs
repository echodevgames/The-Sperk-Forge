using System;
using UnityEngine;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Chronicle lifecycle, public manual-save, and bounded autosave service.
    ///
    /// M4-05 adds explicit caller-triggered autosave submission with one
    /// latest-wins pending request. Manual save and autosave reuse the same
    /// root-local admission authority and the same durable save transaction.
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
