
using UnityEngine;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Chronicle lifecycle service.
    ///
    /// ESV-M2-01 adds storage-root/backend initialization only. Save documents,
    /// serializers, slots, generations, participants, and recovery remain
    /// intentionally absent.
    /// </summary>
    internal sealed class EchoSaveService :
        IEchoSaveService
    {
        private EchoSaveConfiguration configuration;
        private IEchoSaveLifecycleProbe lifecycleProbe;
        private IEchoSaveStorageBackendFactory storageBackendFactory;
        private ISaveStorageBackend storageBackend;
        private EchoSaveServiceState state;

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
                throw new System.ArgumentNullException(
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
                throw new System.ArgumentNullException(
                    nameof(factory));
            }

            EnsurePreInitializationMutationAllowed(
                "The Chronicle storage backend factory");

            storageBackendFactory = factory;
            storageBackend = null;
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

            lifecycleProbe.OnInitializeAccepted(
                configuration);

            state =
                EchoSaveServiceState.Ready;

            return new EchoSaveLifecycleResult(
                EchoSaveLifecycleStatus.Succeeded,
                state,
                string.Empty,
                "The Chronicle initialized its storage backend successfully.");
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

            SaveStorageResult storageShutdown =
                storageBackend != null
                    ? storageBackend.Shutdown()
                    : SaveStorageResult.NoChange(
                        "No Chronicle storage backend was active.");

            lifecycleProbe.OnShutdown();
            storageBackend = null;
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

            if (storageBackend != null)
            {
                storageBackend.Shutdown();
            }

            lifecycleProbe.OnShutdown();
            storageBackend = null;
            state =
                EchoSaveServiceState.Shutdown;
        }

        internal ISaveStorageBackend
            StorageBackendForTesting =>
                storageBackend;

        private EchoSaveLifecycleResult BlockInitialization(
            string diagnosticCode,
            string message)
        {
            state =
                EchoSaveServiceState.Blocked;

            return new EchoSaveLifecycleResult(
                EchoSaveLifecycleStatus.Blocked,
                state,
                diagnosticCode,
                message);
        }

        private void EnsurePreInitializationMutationAllowed(
            string subject)
        {
            if (state !=
                    EchoSaveServiceState.AuthorityClaimed &&
                state !=
                    EchoSaveServiceState.Blocked)
            {
                throw new System.InvalidOperationException(
                    $"{subject} may only be replaced before successful initialization.");
            }
        }
    }
}
