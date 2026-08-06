//----- UnityInitialDestinationLoader.cs START -----

using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Standalone Unity scene loader used when no optional scene-flow bridge is
    /// installed.
    ///
    /// Unity scene operations cannot be reliably cancelled after activation
    /// begins. A cancellation request observed after start therefore waits for
    /// the operation to settle before returning a cancelled result.
    ///
    /// A configured destination that is already loaded and active settles as a
    /// successful no-reload handoff for direct-scene development entry.
    /// </summary>
    internal sealed class
        UnityInitialDestinationLoader :
            IInitialDestinationLoader,
            IInitialDestinationPreflightValidator
    {
        internal const string
            DestinationLoadDiagnosticCode =
                "ELAUNCH-DEST-002";

        internal static
            UnityInitialDestinationLoader Shared
        {
            get;
        } =
            new UnityInitialDestinationLoader();

        private readonly Func<string, bool>
            isDestinationActive;

        private readonly Func<string, int>
            getBuildIndex;

        private readonly Func<string, AsyncOperation>
            beginSingleSceneLoad;

        private UnityInitialDestinationLoader()
            : this(
                IsUnityDestinationActive,
                SceneUtility.GetBuildIndexByScenePath,
                BeginUnitySingleSceneLoad)
        {
        }

        internal UnityInitialDestinationLoader(
            Func<string, bool> isDestinationActive,
            Func<string, int> getBuildIndex,
            Func<string, AsyncOperation> beginSingleSceneLoad)
        {
            this.isDestinationActive =
                isDestinationActive ??
                throw new ArgumentNullException(
                    nameof(isDestinationActive));

            this.getBuildIndex =
                getBuildIndex ??
                throw new ArgumentNullException(
                    nameof(getBuildIndex));

            this.beginSingleSceneLoad =
                beginSingleSceneLoad ??
                throw new ArgumentNullException(
                    nameof(beginSingleSceneLoad));
        }

        public bool TryValidate(
            LaunchDestination destination,
            out string failureMessage)
        {
            if (destination == null)
            {
                failureMessage =
                    "The initial destination is missing.";

                return false;
            }

            if (isDestinationActive(destination.ScenePath))
            {
                failureMessage = string.Empty;
                return true;
            }

            if (getBuildIndex(destination.ScenePath) < 0)
            {
                failureMessage =
                    "The initial destination scene is not included in the player build settings.";

                return false;
            }

            failureMessage =
                string.Empty;

            return true;
        }

        public async Awaitable<
            InitialDestinationLoadResult>
            LoadAsync(
                LaunchDestination destination,
                IProgress<float> progress,
                CancellationToken cancellationToken)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(
                    nameof(destination));
            }

            if (progress == null)
            {
                throw new ArgumentNullException(
                    nameof(progress));
            }

            if (cancellationToken
                .IsCancellationRequested)
            {
                return InitialDestinationLoadResult
                    .Cancelled(
                        destination.DestinationId,
                        EchoLaunchRoot
                            .LifecycleDiagnosticCode,
                        "Initial destination loading was cancelled before it began.");
            }

            if (!TryValidate(
                    destination,
                    out string failureMessage))
            {
                return InitialDestinationLoadResult
                    .Failed(
                        destination.DestinationId,
                        DestinationLoadDiagnosticCode,
                        failureMessage);
            }

            if (isDestinationActive(destination.ScenePath))
            {
                progress.Report(1f);

                return InitialDestinationLoadResult
                    .Success(
                        destination.DestinationId,
                        $"Initial destination '{destination.DisplayName}' is already active.");
            }

            AsyncOperation operation;

            try
            {
                operation =
                    beginSingleSceneLoad(
                        destination.ScenePath);
            }
            catch (Exception exception)
            {
                return CreateFailure(
                    destination,
                    "Unity could not start the initial destination load.",
                    exception);
            }

            if (operation == null)
            {
                return InitialDestinationLoadResult
                    .Failed(
                        destination.DestinationId,
                        DestinationLoadDiagnosticCode,
                        "Unity returned no asynchronous scene-load operation.");
            }

            bool cancellationObserved = false;

            while (!operation.isDone)
            {
                if (cancellationToken
                    .IsCancellationRequested)
                {
                    cancellationObserved = true;
                }

                progress.Report(
                    NormalizeProgress(
                        operation.progress));

                await Awaitable.NextFrameAsync(
                    CancellationToken.None);
            }

            progress.Report(1f);

            if (cancellationToken
                .IsCancellationRequested)
            {
                cancellationObserved = true;
            }

            if (!isDestinationActive(destination.ScenePath))
            {
                Scene activeScene =
                    SceneManager.GetActiveScene();

                return InitialDestinationLoadResult
                    .Failed(
                        destination.DestinationId,
                        DestinationLoadDiagnosticCode,
                        "Unity completed the scene-load operation without activating the configured initial destination.",
                        $"ActiveScenePath: {activeScene.path}");
            }

            if (cancellationObserved)
            {
                return InitialDestinationLoadResult
                    .Cancelled(
                        destination.DestinationId,
                        EchoLaunchRoot
                            .LifecycleDiagnosticCode,
                        "Initial destination loading settled after cancellation was requested.");
            }

            return InitialDestinationLoadResult
                .Success(
                    destination.DestinationId,
                    $"Initial destination '{destination.DisplayName}' activated.");
        }

        private static bool IsUnityDestinationActive(
            string scenePath)
        {
            Scene activeScene =
                SceneManager.GetActiveScene();

            return activeScene.IsValid() &&
                   activeScene.isLoaded &&
                   string.Equals(
                       activeScene.path,
                       scenePath,
                       StringComparison.Ordinal);
        }

        private static AsyncOperation BeginUnitySingleSceneLoad(
            string scenePath)
        {
            return SceneManager.LoadSceneAsync(
                scenePath,
                LoadSceneMode.Single);
        }

        private static float NormalizeProgress(
            float value)
        {
            if (float.IsNaN(value) ||
                float.IsInfinity(value))
            {
                return 0f;
            }

            return Mathf.Clamp01(value);
        }

        private static InitialDestinationLoadResult
            CreateFailure(
                LaunchDestination destination,
                string message,
                Exception exception)
        {
            string details =
                exception == null
                    ? string.Empty
                    : $"ExceptionType: {exception.GetType().FullName}\n" +
                      $"ExceptionMessage: {exception.Message}";

            return InitialDestinationLoadResult
                .Failed(
                    destination.DestinationId,
                    DestinationLoadDiagnosticCode,
                    message,
                    details);
        }
    }
}

//----- UnityInitialDestinationLoader.cs END -----
