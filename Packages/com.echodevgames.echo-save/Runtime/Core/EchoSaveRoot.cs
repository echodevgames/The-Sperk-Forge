
using UnityEngine;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Scene-facing package-local Chronicle authority.
    ///
    /// EchoSaveRoot owns only Chronicle lifecycle. The consumer project owns
    /// project-wide service composition and any DontDestroyOnLoad root.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class EchoSaveRoot : MonoBehaviour
    {
        [Header("Chronicle")]
        [SerializeField]
        private EchoSaveConfiguration configuration;

        [SerializeField]
        private bool autoInitialize;

        private EchoSaveService service;
        private bool isDestroying;

        public static EchoSaveRoot Current =>
            EchoSaveAuthorityClaim.Current;

        public bool IsAuthoritative =>
            ReferenceEquals(
                Current,
                this);

        public bool WasRejectedAsDuplicate
        {
            get;
            private set;
        }

        public EchoSaveConfiguration Configuration =>
            service != null
                ? service.Configuration
                : configuration;

        public EchoSaveServiceState State
        {
            get
            {
                if (WasRejectedAsDuplicate)
                {
                    return EchoSaveServiceState
                        .RejectedDuplicate;
                }

                return service != null
                    ? service.State
                    : EchoSaveServiceState.None;
            }
        }

        public IEchoSaveService Service =>
            IsAuthoritative
                ? service
                : null;

        private void Awake()
        {
            if (!EchoSaveAuthorityClaim.TryClaim(
                    this))
            {
                WasRejectedAsDuplicate = true;
                enabled = false;

                Debug.LogWarning(
                    $"[{EchoSaveDiagnosticCodes.DuplicateRoot}] " +
                    "Duplicate EchoSaveRoot rejected before Chronicle initialization side effects. " +
                    "The existing Chronicle authority remains active.",
                    this);

                return;
            }

            WasRejectedAsDuplicate = false;

            try
            {
                service =
                    new EchoSaveService(
                        configuration);
            }
            catch
            {
                EchoSaveAuthorityClaim.Release(
                    this);
                throw;
            }
        }

        private async Awaitable Start()
        {
            if (!autoInitialize ||
                !IsAuthoritative ||
                service == null ||
                isDestroying)
            {
                return;
            }

            await InitializeAsync();
        }

        public async Awaitable<EchoSaveLifecycleResult>
            InitializeAsync()
        {
            await Awaitable.MainThreadAsync();

            if (!IsAuthoritative ||
                service == null ||
                isDestroying)
            {
                return CreateAuthorityRejectedResult();
            }

            return service.InitializeCore();
        }

        public async Awaitable<EchoSaveLifecycleResult>
            ShutdownAsync()
        {
            await Awaitable.MainThreadAsync();

            if (!IsAuthoritative ||
                service == null)
            {
                return CreateAuthorityRejectedResult();
            }

            EchoSaveLifecycleResult result =
                service.ShutdownCore();

            if (service.State ==
                EchoSaveServiceState.Shutdown)
            {
                EchoSaveAuthorityClaim.Release(
                    this);
            }

            return result;
        }

        private void OnDestroy()
        {
            isDestroying = true;

            if (IsAuthoritative &&
                service != null)
            {
                service.ShutdownImmediate();
            }

            EchoSaveAuthorityClaim.Release(
                this);
            service = null;
        }

        internal EchoSaveLifecycleResult
            InitializeSynchronouslyForTesting()
        {
            if (!IsAuthoritative ||
                service == null ||
                isDestroying)
            {
                return CreateAuthorityRejectedResult();
            }

            return service.InitializeCore();
        }

        internal EchoSaveLifecycleResult
            ShutdownSynchronouslyForTesting()
        {
            if (!IsAuthoritative ||
                service == null)
            {
                return CreateAuthorityRejectedResult();
            }

            EchoSaveLifecycleResult result =
                service.ShutdownCore();

            if (service.State ==
                EchoSaveServiceState.Shutdown)
            {
                EchoSaveAuthorityClaim.Release(
                    this);
            }

            return result;
        }

        internal void SetConfigurationForTesting(
            EchoSaveConfiguration value)
        {
            configuration = value;

            if (service != null)
            {
                service.SetConfiguration(
                    value);
            }
        }

        internal void SetLifecycleProbeForTesting(
            IEchoSaveLifecycleProbe probe)
        {
            if (service == null)
            {
                throw new System.InvalidOperationException(
                    "Only an accepted Chronicle authority has a lifecycle service.");
            }

            service.SetLifecycleProbe(
                probe);
        }

        internal void SetStorageBackendFactoryForTesting(
            IEchoSaveStorageBackendFactory factory)
        {
            if (service == null)
            {
                throw new System.InvalidOperationException(
                    "Only an accepted Chronicle authority has a storage lifecycle.");
            }

            service.SetStorageBackendFactory(
                factory);
        }

        internal ISaveStorageBackend
            StorageBackendForTesting =>
                service != null
                    ? service.StorageBackendForTesting
                    : null;

        internal bool HasConstructedServiceForTesting =>
            service != null;

        /// <summary>
        /// EditMode tests do not have a reliable MonoBehaviour Awake dispatch
        /// contract when components are created directly with AddComponent.
        /// This seam deterministically exercises the exact production Awake
        /// authority path only when Unity has not already done so.
        /// </summary>
        internal void EnsureAuthorityClaimedForTesting()
        {
            if (service != null ||
                WasRejectedAsDuplicate ||
                IsAuthoritative)
            {
                return;
            }

            Awake();
        }

        private EchoSaveLifecycleResult
            CreateAuthorityRejectedResult()
        {
            return new EchoSaveLifecycleResult(
                EchoSaveLifecycleStatus.Rejected,
                State,
                EchoSaveDiagnosticCodes
                    .AuthorityUnavailable,
                "Only the active Chronicle authority may perform this lifecycle operation.");
        }
    }
}
