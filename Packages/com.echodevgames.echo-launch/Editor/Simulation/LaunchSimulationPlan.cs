using System;
using System.Globalization;

namespace EchoDevGames.EchoLaunch.Editor.Simulation
{
    internal enum LaunchSimulationStepBehavior
    {
        Success = 0,
        TimedProgressSuccess = 1,
        Warning = 2,
        RecoverableFailure = 3,
        BlockingFailure = 4,
        WaitForTimeout = 5,
        ThrowException = 6,
        WaitForCancellation = 7
    }

    internal sealed class LaunchSimulationStepPlan
    {
        internal LaunchSimulationStepPlan(
            int authoredIndex,
            string entryId,
            string stepId,
            string displayName,
            LaunchSimulationStepBehavior behavior,
            bool isRequired,
            StartupStepFailureAction failureAction,
            double timeoutSeconds,
            bool supportsCancellation,
            double logicalDurationSeconds,
            int progressSampleCount,
            string message)
        {
            AuthoredIndex = authoredIndex;
            EntryId = entryId;
            StepId = stepId;
            DisplayName = displayName ?? string.Empty;
            Behavior = behavior;
            IsRequired = isRequired;
            FailureAction = failureAction;
            TimeoutSeconds = timeoutSeconds;
            SupportsCancellation = supportsCancellation;
            LogicalDurationSeconds = logicalDurationSeconds;
            ProgressSampleCount = progressSampleCount;
            Message = message ?? string.Empty;
        }

        internal int AuthoredIndex { get; }
        internal string EntryId { get; }
        internal string StepId { get; }
        internal string DisplayName { get; }
        internal LaunchSimulationStepBehavior Behavior { get; }
        internal bool IsRequired { get; }
        internal StartupStepFailureAction FailureAction { get; }
        internal double TimeoutSeconds { get; }
        internal bool SupportsCancellation { get; }
        internal double LogicalDurationSeconds { get; }
        internal int ProgressSampleCount { get; }
        internal string Message { get; }

        internal string ToCanonicalText()
        {
            return string.Join(
                "|",
                AuthoredIndex.ToString(
                    CultureInfo.InvariantCulture),
                EntryId,
                StepId,
                DisplayName,
                ((int)Behavior).ToString(
                    CultureInfo.InvariantCulture),
                IsRequired.ToString(),
                ((int)FailureAction).ToString(
                    CultureInfo.InvariantCulture),
                TimeoutSeconds.ToString(
                    "R",
                    CultureInfo.InvariantCulture),
                SupportsCancellation.ToString(),
                LogicalDurationSeconds.ToString(
                    "R",
                    CultureInfo.InvariantCulture),
                ProgressSampleCount.ToString(
                    CultureInfo.InvariantCulture),
                Message);
        }
    }

    internal sealed class LaunchSimulationPlan
    {
        private readonly LaunchSimulationStepPlan[] steps;

        private LaunchSimulationPlan(
            LaunchSimulationRequest request,
            LaunchSimulationStepPlan[] steps)
        {
            Request = request ??
                      throw new ArgumentNullException(
                          nameof(request));

            this.steps =
                steps ??
                throw new ArgumentNullException(
                    nameof(steps));

            PlanFingerprint =
                LaunchSimulationFingerprint.ComputePlan(
                    request,
                    steps);

            ClockTickSeconds =
                DetermineClockTickSeconds(request);
        }

        internal LaunchSimulationRequest Request { get; }

        internal string PlanFingerprint { get; }

        internal double ClockTickSeconds { get; }

        internal int StepCount => steps.Length;

        internal LaunchSimulationStepPlan GetStep(
            int index)
        {
            if (index < 0 ||
                index >= steps.Length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(index));
            }

            return steps[index];
        }

        internal LaunchSimulationStepPlan[] CopySteps()
        {
            return (LaunchSimulationStepPlan[])steps.Clone();
        }

        internal static LaunchSimulationPlan Create(
            LaunchSimulationRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(
                    nameof(request));
            }

            string error;
            if (!request.TryValidate(out error))
            {
                throw new ArgumentException(
                    error,
                    nameof(request));
            }

            string message =
                string.IsNullOrEmpty(request.Message)
                    ? "Launch Simulator scenario"
                    : request.Message;

            LaunchSimulationStepPlan[] built;

            switch (request.Preset)
            {
                case LaunchSimulationPreset.ImmediateSuccess:
                    built = new[]
                    {
                        CreateStep(
                            request,
                            0,
                            "Immediate Success",
                            LaunchSimulationStepBehavior.Success,
                            true,
                            StartupStepFailureAction.BlockLaunch,
                            0d,
                            true,
                            0d,
                            0,
                            message)
                    };
                    break;

                case LaunchSimulationPreset.TimedProgressSuccess:
                    built = new[]
                    {
                        CreateStep(
                            request,
                            0,
                            "Timed Progress Success",
                            LaunchSimulationStepBehavior
                                .TimedProgressSuccess,
                            true,
                            StartupStepFailureAction.BlockLaunch,
                            0d,
                            true,
                            request.LogicalDurationSeconds,
                            request.ProgressSampleCount,
                            message)
                    };
                    break;

                case LaunchSimulationPreset.WarningContinues:
                    built = new[]
                    {
                        CreateStep(
                            request,
                            0,
                            "Simulated Warning",
                            LaunchSimulationStepBehavior.Warning,
                            true,
                            StartupStepFailureAction.BlockLaunch,
                            0d,
                            true,
                            0d,
                            0,
                            message),
                        CreateStep(
                            request,
                            1,
                            "Continuation Proof",
                            LaunchSimulationStepBehavior.Success,
                            true,
                            StartupStepFailureAction.BlockLaunch,
                            0d,
                            true,
                            0d,
                            0,
                            "Traversal continued after warning.")
                    };
                    break;

                case LaunchSimulationPreset
                    .RecoverableFailureContinues:
                    built = new[]
                    {
                        CreateStep(
                            request,
                            0,
                            "Recoverable Failure",
                            LaunchSimulationStepBehavior
                                .RecoverableFailure,
                            false,
                            StartupStepFailureAction
                                .ContinueWithWarning,
                            0d,
                            true,
                            0d,
                            0,
                            message),
                        CreateStep(
                            request,
                            1,
                            "Continuation Proof",
                            LaunchSimulationStepBehavior.Success,
                            true,
                            StartupStepFailureAction.BlockLaunch,
                            0d,
                            true,
                            0d,
                            0,
                            "Traversal continued after policy conversion.")
                    };
                    break;

                case LaunchSimulationPreset.BlockingFailureStops:
                    built = new[]
                    {
                        CreateStep(
                            request,
                            0,
                            "Blocking Failure",
                            LaunchSimulationStepBehavior
                                .BlockingFailure,
                            true,
                            StartupStepFailureAction.BlockLaunch,
                            0d,
                            true,
                            0d,
                            0,
                            message),
                        CreateStep(
                            request,
                            1,
                            "Must Remain Unvisited",
                            LaunchSimulationStepBehavior.Success,
                            true,
                            StartupStepFailureAction.BlockLaunch,
                            0d,
                            true,
                            0d,
                            0,
                            "This step must not execute.")
                    };
                    break;

                case LaunchSimulationPreset.TimeoutStops:
                    built = new[]
                    {
                        CreateStep(
                            request,
                            0,
                            "Timeout",
                            LaunchSimulationStepBehavior.WaitForTimeout,
                            true,
                            StartupStepFailureAction.BlockLaunch,
                            request.TimeoutSeconds,
                            true,
                            0d,
                            0,
                            message),
                        CreateStep(
                            request,
                            1,
                            "Must Remain Unvisited",
                            LaunchSimulationStepBehavior.Success,
                            true,
                            StartupStepFailureAction.BlockLaunch,
                            0d,
                            true,
                            0d,
                            0,
                            "This step must not execute.")
                    };
                    break;

                case LaunchSimulationPreset.ExecutorExceptionStops:
                    built = new[]
                    {
                        CreateStep(
                            request,
                            0,
                            "Executor Exception",
                            LaunchSimulationStepBehavior.ThrowException,
                            true,
                            StartupStepFailureAction.BlockLaunch,
                            0d,
                            true,
                            0d,
                            0,
                            message),
                        CreateStep(
                            request,
                            1,
                            "Must Remain Unvisited",
                            LaunchSimulationStepBehavior.Success,
                            true,
                            StartupStepFailureAction.BlockLaunch,
                            0d,
                            true,
                            0d,
                            0,
                            "This step must not execute.")
                    };
                    break;

                case LaunchSimulationPreset.Cancellation:
                    built = new[]
                    {
                        CreateStep(
                            request,
                            0,
                            "Await User Cancellation",
                            LaunchSimulationStepBehavior
                                .WaitForCancellation,
                            true,
                            StartupStepFailureAction.BlockLaunch,
                            0d,
                            true,
                            0d,
                            0,
                            message),
                        CreateStep(
                            request,
                            1,
                            "Must Remain Unvisited",
                            LaunchSimulationStepBehavior.Success,
                            true,
                            StartupStepFailureAction.BlockLaunch,
                            0d,
                            true,
                            0d,
                            0,
                            "This step must not execute.")
                    };
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(request),
                        request.Preset,
                        "The Launch Simulator preset is unsupported.");
            }

            return new LaunchSimulationPlan(
                request,
                built);
        }

        private static LaunchSimulationStepPlan CreateStep(
            LaunchSimulationRequest request,
            int index,
            string displayName,
            LaunchSimulationStepBehavior behavior,
            bool isRequired,
            StartupStepFailureAction failureAction,
            double timeoutSeconds,
            bool supportsCancellation,
            double logicalDurationSeconds,
            int progressSampleCount,
            string message)
        {
            string seed =
                request.RequestFingerprint +
                "|Step|" +
                index.ToString(
                    CultureInfo.InvariantCulture);

            return new LaunchSimulationStepPlan(
                index,
                LaunchSimulationFingerprint.StableId(
                    seed + "|Entry"),
                LaunchSimulationFingerprint.StableId(
                    seed + "|Definition"),
                displayName,
                behavior,
                isRequired,
                failureAction,
                timeoutSeconds,
                supportsCancellation,
                logicalDurationSeconds,
                progressSampleCount,
                message);
        }

        private static double DetermineClockTickSeconds(
            LaunchSimulationRequest request)
        {
            if (request.Preset ==
                    LaunchSimulationPreset.TimedProgressSuccess &&
                request.ProgressSampleCount > 0)
            {
                return request.LogicalDurationSeconds /
                       request.ProgressSampleCount;
            }

            if (request.Preset ==
                    LaunchSimulationPreset.TimeoutStops &&
                request.TimeoutSeconds > 0d)
            {
                return request.TimeoutSeconds / 2d;
            }

            return 0.25d;
        }
    }
}
