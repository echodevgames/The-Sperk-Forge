//----- StartupStepPolicy.cs START -----

using System;
using UnityEngine;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Stores immutable authored policy for one startup-sequence entry.
    ///
    /// Active timeout state, cancellation state, retry counts, progress,
    /// and results belong to runtime execution objects introduced by later
    /// checkpoints.
    /// </summary>
    [Serializable]
    public struct StartupStepPolicy
    {
        private enum RequirementMode
        {
            Required = 0,
            Optional = 1
        }

        private enum CancellationMode
        {
            Supported = 0,
            Unsupported = 1
        }

        [SerializeField]
        private RequirementMode requirement;

        [SerializeField]
        private StartupStepFailureAction failureAction;

        [SerializeField]
        [Min(0f)]
        private float timeoutSeconds;

        [SerializeField]
        private CancellationMode cancellation;

        /// <summary>
        /// Gets a safe required-step policy that blocks launch on failure.
        /// No timeout is configured.
        /// </summary>
        public static StartupStepPolicy RequiredBlocking =>
            new StartupStepPolicy(
                true,
                StartupStepFailureAction.BlockLaunch,
                0f,
                true);

        /// <summary>
        /// Gets a safe optional-step policy that continues with a warning.
        /// No timeout is configured.
        /// </summary>
        public static StartupStepPolicy OptionalWarning =>
            new StartupStepPolicy(
                false,
                StartupStepFailureAction
                    .ContinueWithWarning,
                0f,
                true);

        /// <summary>
        /// Gets whether this entry is authored as required.
        /// </summary>
        public bool IsRequired =>
            requirement ==
            RequirementMode.Required;

        /// <summary>
        /// Gets whether this entry is authored as optional.
        /// </summary>
        public bool IsOptional =>
            requirement ==
            RequirementMode.Optional;

        /// <summary>
        /// Gets the action applied by the future runner when policy
        /// interprets a step failure.
        /// </summary>
        public StartupStepFailureAction FailureAction =>
            failureAction;

        /// <summary>
        /// Gets the authored timeout duration in seconds.
        /// Zero means no timeout is configured.
        /// </summary>
        public float TimeoutSeconds =>
            timeoutSeconds;

        /// <summary>
        /// Gets whether this policy enables timeout handling.
        /// </summary>
        public bool HasTimeout =>
            HasValidTimeout &&
            timeoutSeconds > 0f;

        /// <summary>
        /// Gets whether the executor declares support for cooperative
        /// cancellation.
        /// </summary>
        public bool SupportsCancellation =>
            cancellation ==
            CancellationMode.Supported;

        /// <summary>
        /// Returns true when the requirement value is supported.
        /// </summary>
        internal bool HasValidRequirement =>
            Enum.IsDefined(
                typeof(RequirementMode),
                requirement);

        /// <summary>
        /// Returns true when the failure action is one of the approved MVP
        /// values.
        /// </summary>
        internal bool HasValidFailureAction =>
            Enum.IsDefined(
                typeof(StartupStepFailureAction),
                failureAction);

        /// <summary>
        /// Returns true when timeout data is finite and nonnegative.
        /// </summary>
        internal bool HasValidTimeout =>
            !float.IsNaN(timeoutSeconds) &&
            !float.IsInfinity(timeoutSeconds) &&
            timeoutSeconds >= 0f;

        /// <summary>
        /// Returns true when cancellation support data is recognized.
        /// </summary>
        internal bool HasValidCancellation =>
            Enum.IsDefined(
                typeof(CancellationMode),
                cancellation);

        /// <summary>
        /// Returns true when all authored policy values are supported.
        /// </summary>
        internal bool IsValid =>
            HasValidRequirement &&
            HasValidFailureAction &&
            HasValidTimeout &&
            HasValidCancellation;

        private StartupStepPolicy(
            bool isRequired,
            StartupStepFailureAction failureAction,
            float timeoutSeconds,
            bool supportsCancellation)
        {
            requirement =
                isRequired
                    ? RequirementMode.Required
                    : RequirementMode.Optional;

            this.failureAction = failureAction;
            this.timeoutSeconds = timeoutSeconds;

            cancellation =
                supportsCancellation
                    ? CancellationMode.Supported
                    : CancellationMode.Unsupported;
        }
    }
}

//----- StartupStepPolicy.cs END -----
