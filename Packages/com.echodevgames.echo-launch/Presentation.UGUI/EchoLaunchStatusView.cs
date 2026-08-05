//----- EchoLaunchStatusView.cs START -----

using System;
using EchoDevGames.EchoLaunch;
using UnityEngine;
using UnityEngine.UI;

namespace EchoDevGames.EchoLaunch.Presentation.UGUI
{
    /// <summary>
    /// Default plain uGUI presenter for startup-only First Light status.
    ///
    /// The view renders accepted immutable launch snapshots and finalized
    /// reports. It does not own launch authority, lifecycle transitions,
    /// startup execution, destination loading, or report finalization.
    /// </summary>
    [AddComponentMenu(
        "EchoDevGames/First Light/Echo Launch Status View")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class EchoLaunchStatusView :
        MonoBehaviour,
        ILaunchStatusPresenter
    {
        [Header("References")]
        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private Text stateText;

        [SerializeField]
        private Text messageText;

        [SerializeField]
        private Text stepText;

        [SerializeField]
        private Text progressText;

        [SerializeField]
        private Text elapsedText;

        [SerializeField]
        private Slider determinateProgress;

        [SerializeField]
        private GameObject determinateProgressRoot;

        [SerializeField]
        private GameObject indeterminateProgressRoot;

        [Header("State Copy")]
        [SerializeField]
        private string authorityClaimedText =
            "Preparing launch.";

        [SerializeField]
        private string validatingText =
            "Validating launch.";

        [SerializeField]
        private string runningText =
            "Starting systems.";

        [SerializeField]
        private string warningText =
            "Continuing with a warning.";

        [SerializeField]
        private string transitioningText =
            "Loading destination.";

        [SerializeField]
        private string completedText =
            "Launch complete.";

        [SerializeField]
        private string failedText =
            "Launch blocked.";

        [SerializeField]
        private string interruptedText =
            "Launch interrupted.";

        [Header("Progress Copy")]
        [SerializeField]
        private string indeterminateText =
            "Working...";

        [SerializeField]
        private string elapsedPrefix =
            "Elapsed";

        [Header("Behavior")]
        [SerializeField]
        private bool showOnBind = true;

        [SerializeField]
        private bool hideOnUnbind = true;

        [SerializeField]
        private bool clearOnUnbind;

        /// <summary>
        /// Gets whether the view is currently bound to an authoritative
        /// launch attempt.
        /// </summary>
        public bool IsBound
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the latest accepted snapshot rendered by the view.
        /// </summary>
        public LaunchProgressSnapshot LastSnapshot
        {
            get;
            private set;
        } =
            LaunchProgressSnapshot.Empty;

        /// <summary>
        /// Gets the latest finalized report rendered by the view.
        /// </summary>
        public LaunchReport LastReport
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets whether the view's CanvasGroup is currently visible.
        /// </summary>
        public bool IsVisible =>
            canvasGroup == null ||
            canvasGroup.alpha > 0.001f;

        /// <summary>
        /// Gets whether the determinate progress surface is active.
        /// </summary>
        public bool IsShowingDeterminateProgress =>
            determinateProgressRoot != null &&
            determinateProgressRoot.activeSelf;

        /// <summary>
        /// Gets whether the indeterminate progress surface is active.
        /// </summary>
        public bool IsShowingIndeterminateProgress =>
            indeterminateProgressRoot != null &&
            indeterminateProgressRoot.activeSelf;

        private void Reset()
        {
            canvasGroup =
                GetComponent<CanvasGroup>();
        }

        private void Awake()
        {
            ResolveCanvasGroup();
            NormalizeSlider();

            if (hideOnUnbind)
            {
                SetVisible(false);
            }
        }

        /// <inheritdoc />
        public void Bind(
            LaunchProgressSnapshot initialSnapshot)
        {
            ResolveCanvasGroup();
            NormalizeSlider();

            IsBound = true;
            LastReport = null;

            if (showOnBind)
            {
                SetVisible(true);
            }

            ApplySnapshot(
                initialSnapshot);
        }

        /// <inheritdoc />
        public void Present(
            LaunchProgressSnapshot snapshot)
        {
            if (!IsBound)
            {
                return;
            }

            ApplySnapshot(snapshot);
        }

        /// <inheritdoc />
        public void PresentTerminal(
            LaunchReport report)
        {
            if (report == null)
            {
                throw new ArgumentNullException(
                    nameof(report));
            }

            if (!IsBound)
            {
                return;
            }

            LastReport = report;

            SetText(
                stateText,
                GetTerminalStateText(
                    report.FinalStatus));

            SetText(
                messageText,
                FormatResult(
                    report.FinalResult));

            SetText(
                stepText,
                report.DestinationDisplayName);

            SetText(
                elapsedText,
                FormatElapsed(
                    report.ElapsedSeconds));

            if (report.FinalStatus ==
                LaunchStatus.Completed)
            {
                ApplyProgress(
                    1f,
                    false);

                return;
            }

            ApplyProgress(
                LastSnapshot.Progress01,
                LastSnapshot
                    .IsProgressIndeterminate);
        }

        /// <inheritdoc />
        public void Unbind()
        {
            if (!IsBound)
            {
                return;
            }

            IsBound = false;

            if (hideOnUnbind)
            {
                SetVisible(false);
            }

            if (clearOnUnbind)
            {
                ClearPresentation();
            }
        }

        internal void ConfigureForTesting(
            CanvasGroup configuredCanvasGroup,
            Text configuredStateText,
            Text configuredMessageText,
            Text configuredStepText,
            Text configuredProgressText,
            Text configuredElapsedText,
            Slider configuredDeterminateProgress,
            GameObject configuredDeterminateRoot,
            GameObject configuredIndeterminateRoot)
        {
            if (IsBound)
            {
                throw new InvalidOperationException(
                    "The status view cannot be reconfigured while bound.");
            }

            canvasGroup =
                configuredCanvasGroup;

            stateText =
                configuredStateText;

            messageText =
                configuredMessageText;

            stepText =
                configuredStepText;

            progressText =
                configuredProgressText;

            elapsedText =
                configuredElapsedText;

            determinateProgress =
                configuredDeterminateProgress;

            determinateProgressRoot =
                configuredDeterminateRoot;

            indeterminateProgressRoot =
                configuredIndeterminateRoot;

            ResolveCanvasGroup();
            NormalizeSlider();

            if (hideOnUnbind)
            {
                SetVisible(false);
            }
        }

        internal void ConfigureBehaviorForTesting(
            bool shouldShowOnBind,
            bool shouldHideOnUnbind,
            bool shouldClearOnUnbind)
        {
            if (IsBound)
            {
                throw new InvalidOperationException(
                    "Status-view behavior cannot change while bound.");
            }

            showOnBind =
                shouldShowOnBind;

            hideOnUnbind =
                shouldHideOnUnbind;

            clearOnUnbind =
                shouldClearOnUnbind;

            if (hideOnUnbind)
            {
                SetVisible(false);
            }
        }

        private void ApplySnapshot(
            LaunchProgressSnapshot snapshot)
        {
            LastSnapshot = snapshot;

            SetText(
                stateText,
                GetSnapshotStateText(
                    snapshot));

            SetText(
                messageText,
                GetSnapshotMessage(
                    snapshot));

            SetText(
                stepText,
                FormatStep(
                    snapshot));

            SetText(
                elapsedText,
                FormatElapsed(
                    snapshot.ElapsedSeconds));

            ApplyProgress(
                snapshot.Progress01,
                snapshot
                    .IsProgressIndeterminate);
        }

        private string GetSnapshotStateText(
            LaunchProgressSnapshot snapshot)
        {
            if (snapshot.Status ==
                    LaunchStatus.Running &&
                snapshot.LastResult != null &&
                snapshot.LastResult.Status ==
                    StartupStepStatus.Warning)
            {
                return warningText;
            }

            switch (snapshot.Status)
            {
                case LaunchStatus.AuthorityClaimed:
                    return authorityClaimedText;

                case LaunchStatus.Validating:
                    return validatingText;

                case LaunchStatus.Running:
                    return runningText;

                case LaunchStatus.Transitioning:
                    return transitioningText;

                case LaunchStatus.Completed:
                    return completedText;

                case LaunchStatus.Failed:
                    return failedText;

                case LaunchStatus.Interrupted:
                    return interruptedText;

                default:
                    return string.Empty;
            }
        }

        private string GetTerminalStateText(
            LaunchStatus finalStatus)
        {
            switch (finalStatus)
            {
                case LaunchStatus.Completed:
                    return completedText;

                case LaunchStatus.Failed:
                    return failedText;

                case LaunchStatus.Interrupted:
                    return interruptedText;

                default:
                    return string.Empty;
            }
        }

        private string GetSnapshotMessage(
            LaunchProgressSnapshot snapshot)
        {
            StartupStepResult result =
                snapshot.LastResult;

            if (result != null &&
                !string.IsNullOrEmpty(
                    result.Code))
            {
                return FormatResult(result);
            }

            if (!string.IsNullOrWhiteSpace(
                    snapshot.Message))
            {
                return snapshot.Message.Trim();
            }

            return result == null
                ? string.Empty
                : FormatResult(result);
        }

        private static string FormatResult(
            StartupStepResult result)
        {
            if (result == null)
            {
                return string.Empty;
            }

            string message =
                NormalizeText(
                    result.Message);

            string code =
                NormalizeText(
                    result.Code);

            if (string.IsNullOrEmpty(code))
            {
                return message;
            }

            if (string.IsNullOrEmpty(message))
            {
                return $"[{code}]";
            }

            return $"[{code}] {message}";
        }

        private static string FormatStep(
            LaunchProgressSnapshot snapshot)
        {
            if (snapshot.ActiveStepIndex < 0 ||
                snapshot.TotalStepCount <= 0)
            {
                return string.Empty;
            }

            string position =
                $"Step {snapshot.ActiveStepIndex + 1} " +
                $"of {snapshot.TotalStepCount}";

            string stepId =
                NormalizeText(
                    snapshot.ActiveStepId);

            return string.IsNullOrEmpty(
                    stepId)
                ? position
                : $"{position} - {stepId}";
        }

        private string FormatElapsed(
            double seconds)
        {
            string prefix =
                NormalizeText(
                    elapsedPrefix);

            string value =
                $"{seconds:0.0}s";

            return string.IsNullOrEmpty(
                prefix)
                ? value
                : $"{prefix} {value}";
        }

        private void ApplyProgress(
            float progress01,
            bool isIndeterminate)
        {
            if (determinateProgressRoot !=
                null)
            {
                determinateProgressRoot
                    .SetActive(
                        !isIndeterminate);
            }

            if (indeterminateProgressRoot !=
                null)
            {
                indeterminateProgressRoot
                    .SetActive(
                        isIndeterminate);
            }

            if (determinateProgress != null)
            {
                determinateProgress.value =
                    progress01;
            }

            SetText(
                progressText,
                isIndeterminate
                    ? indeterminateText
                    : FormatPercent(
                        progress01));
        }

        private void ClearPresentation()
        {
            LastSnapshot =
                LaunchProgressSnapshot.Empty;

            LastReport = null;

            SetText(
                stateText,
                string.Empty);

            SetText(
                messageText,
                string.Empty);

            SetText(
                stepText,
                string.Empty);

            SetText(
                progressText,
                string.Empty);

            SetText(
                elapsedText,
                string.Empty);

            if (determinateProgress != null)
            {
                determinateProgress.value =
                    0f;
            }

            if (determinateProgressRoot !=
                null)
            {
                determinateProgressRoot
                    .SetActive(false);
            }

            if (indeterminateProgressRoot !=
                null)
            {
                indeterminateProgressRoot
                    .SetActive(false);
            }
        }

        private void ResolveCanvasGroup()
        {
            if (canvasGroup == null)
            {
                canvasGroup =
                    GetComponent<CanvasGroup>();
            }
        }

        private void NormalizeSlider()
        {
            if (determinateProgress == null)
            {
                return;
            }

            determinateProgress.minValue =
                0f;

            determinateProgress.maxValue =
                1f;

            determinateProgress.wholeNumbers =
                false;
        }

        private void SetVisible(
            bool visible)
        {
            if (canvasGroup == null)
            {
                return;
            }

            canvasGroup.alpha =
                visible
                    ? 1f
                    : 0f;

            canvasGroup.interactable =
                false;

            canvasGroup.blocksRaycasts =
                false;
        }

        private static string FormatPercent(
            float progress01)
        {
            return
                $"{Mathf.RoundToInt(progress01 * 100f)}%";
        }

        private static string NormalizeText(
            string value)
        {
            return string.IsNullOrWhiteSpace(
                value)
                ? string.Empty
                : value.Trim();
        }

        private static void SetText(
            Text target,
            string value)
        {
            if (target != null)
            {
                target.text =
                    value ?? string.Empty;
            }
        }
    }
}

//----- EchoLaunchStatusView.cs END -----
