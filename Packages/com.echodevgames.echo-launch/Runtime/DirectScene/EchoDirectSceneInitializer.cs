//----- EchoDirectSceneInitializer.cs START -----

using System;
using UnityEngine;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// One-shot development helper that enters the existing First Light launch
    /// architecture when a configured scene is opened directly.
    ///
    /// Scene-authored roots claim in Awake. This helper waits until Start,
    /// reuses existing authority first, and only then considers creating one
    /// explicitly configured development root.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class EchoDirectSceneInitializer :
        MonoBehaviour
    {
        public const string PolicyDiagnosticCode =
            "ELAUNCH-DIRECT-001";

        public const string ConfigurationDiagnosticCode =
            "ELAUNCH-DIRECT-002";

        public const string InstantiationDiagnosticCode =
            "ELAUNCH-DIRECT-003";

        [SerializeField]
        private DirectSceneConfiguration directSceneConfiguration;

        [SerializeField]
        private bool logSettlement = true;

        private IDirectSceneRuntimeEnvironment runtimeEnvironment;
        private IDirectSceneRootFactory rootFactory;
        private DirectSceneInitializationResult lastResult;
        private string containingScenePathOverrideForTesting;

        public DirectSceneConfiguration Configuration =>
            directSceneConfiguration;

        public bool HasSettled =>
            lastResult != null;

        public DirectSceneInitializationResult LastResult =>
            lastResult;

        private void Awake()
        {
            runtimeEnvironment =
                UnityDirectSceneRuntimeEnvironment.Shared;

            rootFactory =
                UnityDirectSceneRootFactory.Shared;
        }

        private void Start()
        {
            EnsureDevelopmentLaunch();
        }

        /// <summary>
        /// Idempotently settles direct-scene authority exactly once.
        /// </summary>
        public DirectSceneInitializationResult
            EnsureDevelopmentLaunch()
        {
            if (lastResult != null)
            {
                return lastResult;
            }

            EchoLaunchRoot existing = EchoLaunchRoot.Current;

            if (existing != null)
            {
                return Settle(
                    DirectSceneInitializationStatus
                        .ReusedExistingAuthority,
                    GetPolicyOrDefault(),
                    string.Empty,
                    "Existing First Light authority reused.",
                    existing,
                    false,
                    true);
            }

            DirectSceneConfiguration authored =
                directSceneConfiguration;

            if (authored == null ||
                !authored.HasValidIdentity ||
                !authored.HasSupportedSchema ||
                !authored.HasSupportedPolicy)
            {
                return SettleInvalid(
                    authored == null
                        ? DirectSceneEntryPolicy.EditorOnly
                        : authored.EntryPolicy,
                    "The direct-scene configuration is missing or unsupported.");
            }

            DirectSceneEntryPolicy policy =
                authored.EntryPolicy;

            if (policy == DirectSceneEntryPolicy.BootRequired)
            {
                return Settle(
                    DirectSceneInitializationStatus.BlockedByPolicy,
                    policy,
                    PolicyDiagnosticCode,
                    "This scene requires canonical Boot entry.",
                    null,
                    false,
                    false);
            }

            IDirectSceneRuntimeEnvironment environment =
                runtimeEnvironment ??
                UnityDirectSceneRuntimeEnvironment.Shared;

            if (!environment.IsEditor &&
                !environment.IsDevelopmentBuild)
            {
                return Settle(
                    DirectSceneInitializationStatus.BlockedByEnvironment,
                    policy,
                    PolicyDiagnosticCode,
                    "Direct-scene root creation is prohibited in non-development player builds.",
                    null,
                    false,
                    false);
            }

            if (policy == DirectSceneEntryPolicy.EditorOnly &&
                !environment.IsEditor)
            {
                return Settle(
                    DirectSceneInitializationStatus.BlockedByEnvironment,
                    policy,
                    PolicyDiagnosticCode,
                    "This direct-scene configuration permits root creation only in the Unity Editor.",
                    null,
                    false,
                    false);
            }

            string containingScenePath =
                ResolveContainingScenePath();

            if (!TryValidateRoot(
                    authored,
                    containingScenePath,
                    out EchoLaunchRoot rootPrefab,
                    out string validationMessage))
            {
                return SettleInvalid(
                    policy,
                    validationMessage);
            }

            EchoLaunchRoot createdRoot;

            try
            {
                IDirectSceneRootFactory factory =
                    rootFactory ??
                    UnityDirectSceneRootFactory.Shared;

                createdRoot = factory.Instantiate(rootPrefab);
            }
            catch (Exception exception)
            {
                return Settle(
                    DirectSceneInitializationStatus.InstantiationFailed,
                    policy,
                    InstantiationDiagnosticCode,
                    "Direct-scene root instantiation failed (" +
                    exception.GetType().Name +
                    ").",
                    null,
                    false,
                    false);
            }

            EchoLaunchRoot accepted = EchoLaunchRoot.Current;

            if (accepted == null)
            {
                DestroyCreatedRoot(createdRoot);

                return Settle(
                    DirectSceneInitializationStatus.InstantiationFailed,
                    policy,
                    InstantiationDiagnosticCode,
                    "The instantiated direct-scene root did not claim First Light authority.",
                    null,
                    false,
                    false);
            }

            if (!ReferenceEquals(accepted, createdRoot))
            {
                DestroyCreatedRoot(createdRoot);

                return Settle(
                    DirectSceneInitializationStatus.ReusedExistingAuthority,
                    policy,
                    string.Empty,
                    "First Light authority was claimed while the direct-scene root was being created; the accepted authority was reused.",
                    accepted,
                    false,
                    true);
            }

            return Settle(
                DirectSceneInitializationStatus.CreatedDevelopmentAuthority,
                policy,
                string.Empty,
                "Direct-scene development authority created.",
                accepted,
                true,
                false);
        }

        internal void SetConfigurationForTesting(
            DirectSceneConfiguration configuration)
        {
            EnsureNotSettled();
            directSceneConfiguration = configuration;
        }

        internal void SetRuntimeEnvironmentForTesting(
            IDirectSceneRuntimeEnvironment environment)
        {
            EnsureNotSettled();
            runtimeEnvironment =
                environment ??
                throw new ArgumentNullException(nameof(environment));
        }

        internal void SetRootFactoryForTesting(
            IDirectSceneRootFactory factory)
        {
            EnsureNotSettled();
            rootFactory =
                factory ??
                throw new ArgumentNullException(nameof(factory));
        }

        internal void SetContainingScenePathForTesting(
            string scenePath)
        {
            EnsureNotSettled();
            containingScenePathOverrideForTesting =
                NormalizeScenePath(scenePath);
        }

        internal void SetLoggingForTesting(bool enabled)
        {
            EnsureNotSettled();
            logSettlement = enabled;
        }

        private bool TryValidateRoot(
            DirectSceneConfiguration authored,
            string containingScenePath,
            out EchoLaunchRoot rootPrefab,
            out string failureMessage)
        {
            rootPrefab = authored.RootPrefab;

            if (string.IsNullOrEmpty(containingScenePath) ||
                !containingScenePath.StartsWith(
                    "Assets/",
                    StringComparison.Ordinal) ||
                !containingScenePath.EndsWith(
                    ".unity",
                    StringComparison.OrdinalIgnoreCase))
            {
                failureMessage =
                    "The direct-scene initializer must belong to a saved project scene.";

                return false;
            }

            if (rootPrefab == null)
            {
                failureMessage =
                    "The direct-scene configuration does not reference a root prefab.";

                return false;
            }

            EchoLaunchRoot[] roots =
                rootPrefab.gameObject
                    .GetComponentsInChildren<EchoLaunchRoot>(true);

            if (roots.Length != 1 ||
                !ReferenceEquals(roots[0], rootPrefab) ||
                !rootPrefab.enabled ||
                !IsActiveWithinConfiguredObject(rootPrefab.transform))
            {
                failureMessage =
                    "The configured direct root must contain exactly one active EchoLaunchRoot.";

                return false;
            }

            if (rootPrefab.AuthoredLaunchMode !=
                LaunchMode.DirectSceneDevelopment)
            {
                failureMessage =
                    "The configured direct root is not authored for DirectSceneDevelopment.";

                return false;
            }

            EchoLaunchConfiguration launchConfiguration =
                rootPrefab.AuthoredConfiguration;

            if (launchConfiguration == null ||
                !launchConfiguration.HasValidIdentity ||
                !launchConfiguration.HasSupportedSchema)
            {
                failureMessage =
                    "The configured direct root does not reference a supported launch configuration.";

                return false;
            }

            StartupSequence sequence =
                launchConfiguration.StartupSequence;

            if (sequence == null ||
                !sequence.HasValidIdentity ||
                !sequence.HasSupportedSchema)
            {
                failureMessage =
                    "The direct launch configuration does not reference a supported startup sequence.";

                return false;
            }

            LaunchDestination destination =
                launchConfiguration.InitialDestination;

            if (destination == null ||
                !destination.HasValidIdentity ||
                !destination.HasSupportedSchema ||
                !destination.HasValidDisplayName ||
                !destination.HasValidScenePath)
            {
                failureMessage =
                    "The direct launch configuration does not reference a supported destination.";

                return false;
            }

            if (!string.Equals(
                    NormalizeScenePath(destination.ScenePath),
                    containingScenePath,
                    StringComparison.Ordinal))
            {
                failureMessage =
                    "The direct launch destination does not match the scene containing the initializer.";

                return false;
            }

            failureMessage = string.Empty;
            return true;
        }

        private DirectSceneInitializationResult SettleInvalid(
            DirectSceneEntryPolicy policy,
            string message)
        {
            return Settle(
                DirectSceneInitializationStatus.InvalidConfiguration,
                policy,
                ConfigurationDiagnosticCode,
                message,
                null,
                false,
                false);
        }

        private DirectSceneInitializationResult Settle(
            DirectSceneInitializationStatus status,
            DirectSceneEntryPolicy policy,
            string diagnosticCode,
            string message,
            EchoLaunchRoot authoritativeRoot,
            bool createdRoot,
            bool reusedExistingAuthority)
        {
            lastResult =
                new DirectSceneInitializationResult(
                    status,
                    policy,
                    diagnosticCode,
                    message,
                    ResolveContainingScenePath(),
                    authoritativeRoot,
                    createdRoot,
                    reusedExistingAuthority);

            enabled = false;
            LogSettlement(lastResult);
            return lastResult;
        }

        private void LogSettlement(
            DirectSceneInitializationResult result)
        {
            if (!logSettlement || result == null)
            {
                return;
            }

            string prefix =
                string.IsNullOrEmpty(result.DiagnosticCode)
                    ? "[First Light Direct Scene] "
                    : "[" + result.DiagnosticCode + "] ";

            if (result.IsSuccessful)
            {
                Debug.Log(prefix + result.Message, this);
                return;
            }

            if (result.Status ==
                DirectSceneInitializationStatus.InstantiationFailed)
            {
                Debug.LogError(prefix + result.Message, this);
                return;
            }

            Debug.LogWarning(prefix + result.Message, this);
        }

        private void DestroyCreatedRoot(EchoLaunchRoot root)
        {
            if (root == null)
            {
                return;
            }

            IDirectSceneRootFactory factory =
                rootFactory ??
                UnityDirectSceneRootFactory.Shared;

            factory.Destroy(root);
        }

        private DirectSceneEntryPolicy GetPolicyOrDefault()
        {
            return directSceneConfiguration == null
                ? DirectSceneEntryPolicy.EditorOnly
                : directSceneConfiguration.EntryPolicy;
        }

        private string ResolveContainingScenePath()
        {
            if (!string.IsNullOrEmpty(
                    containingScenePathOverrideForTesting))
            {
                return containingScenePathOverrideForTesting;
            }

            return NormalizeScenePath(gameObject.scene.path);
        }

        private static string NormalizeScenePath(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Trim().Replace('\\', '/');
        }

        private static bool IsActiveWithinConfiguredObject(
            Transform target)
        {
            Transform current = target;

            while (current != null)
            {
                if (!current.gameObject.activeSelf)
                {
                    return false;
                }

                current = current.parent;
            }

            return true;
        }

        private void EnsureNotSettled()
        {
            if (lastResult != null)
            {
                throw new InvalidOperationException(
                    "The direct-scene initializer has already settled.");
            }
        }
    }
}

//----- EchoDirectSceneInitializer.cs END -----
