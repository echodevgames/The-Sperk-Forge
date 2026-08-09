using UnityEngine;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// M1 Chronicle lifecycle service.
    ///
    /// No durable storage, serializer, slot, participant, or generation
    /// behavior exists in this checkpoint.
    /// </summary>
    internal sealed class EchoSaveService :
        IEchoSaveService
    {
        private EchoSaveConfiguration configuration;
        private IEchoSaveLifecycleProbe lifecycleProbe;
        private EchoSaveServiceState state;

        internal EchoSaveService(
            EchoSaveConfiguration configuration)
        {
            this.configuration = configuration;
            lifecycleProbe =
                NullEchoSaveLifecycleProbe.Instance;
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
            if (state !=
                EchoSaveServiceState.AuthorityClaimed &&
                state !=
                EchoSaveServiceState.Blocked)
            {
                throw new System.InvalidOperationException(
                    "Chronicle configuration may only be replaced before successful initialization.");
            }

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

            if (state !=
                EchoSaveServiceState.AuthorityClaimed &&
                state !=
                EchoSaveServiceState.Blocked)
            {
                throw new System.InvalidOperationException(
                    "The Chronicle lifecycle probe may only be replaced before successful initialization.");
            }

            lifecycleProbe = probe;
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
                state =
                    EchoSaveServiceState.Blocked;

                return new EchoSaveLifecycleResult(
                    EchoSaveLifecycleStatus.Blocked,
                    state,
                    EchoSaveDiagnosticCodes
                        .MissingOrInvalidConfiguration,
                    "The Chronicle configuration is missing.");
            }

            if (!configuration.TryValidate(
                    out string validationMessage))
            {
                state =
                    EchoSaveServiceState.Blocked;

                return new EchoSaveLifecycleResult(
                    EchoSaveLifecycleStatus.Blocked,
                    state,
                    EchoSaveDiagnosticCodes
                        .MissingOrInvalidConfiguration,
                    validationMessage);
            }

            lifecycleProbe.OnInitializeAccepted(
                configuration);

            state =
                EchoSaveServiceState.Ready;

            return new EchoSaveLifecycleResult(
                EchoSaveLifecycleStatus.Succeeded,
                state,
                string.Empty,
                "The Chronicle initialized without durable storage side effects.");
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

            lifecycleProbe.OnShutdown();

            state =
                EchoSaveServiceState.Shutdown;

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

            lifecycleProbe.OnShutdown();

            state =
                EchoSaveServiceState.Shutdown;
        }
    }
}
