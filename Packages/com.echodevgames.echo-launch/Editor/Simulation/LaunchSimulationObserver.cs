using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoLaunch.Editor.Simulation
{
    internal sealed class LaunchSimulationObserver :
        IStartupSequenceObserver
    {
        private readonly LaunchSimulationPlan plan;
        private readonly List<LaunchSimulationProgressSample>
            progress =
                new List<LaunchSimulationProgressSample>();

        private readonly int[] sampleCounts;
        private readonly double[] latestLogicalSeconds;

        internal LaunchSimulationObserver(
            LaunchSimulationPlan plan)
        {
            this.plan =
                plan ??
                throw new ArgumentNullException(
                    nameof(plan));

            sampleCounts = new int[plan.StepCount];
            latestLogicalSeconds =
                new double[plan.StepCount];
        }

        internal LaunchSimulationProgressSample[]
            CopyProgressSamples()
        {
            return progress.ToArray();
        }

        internal int GetProgressCountForStep(
            int authoredStepIndex)
        {
            return sampleCounts[authoredStepIndex];
        }

        public void SequenceValidated(
            StartupSequence sequence)
        {
        }

        public void StepStarted(
            StartupStepExecution execution)
        {
        }

        public void StepProgressChanged(
            StartupStepExecution execution,
            StartupStepProgress stepProgress)
        {
            if (execution == null)
            {
                throw new ArgumentNullException(
                    nameof(execution));
            }

            int authoredIndex = execution.StepIndex;
            LaunchSimulationStepPlan step =
                plan.GetStep(authoredIndex);

            int sampleIndex =
                sampleCounts[authoredIndex]++;

            double logicalSeconds;

            if (stepProgress.IsIndeterminate)
            {
                logicalSeconds =
                    latestLogicalSeconds[authoredIndex];
            }
            else
            {
                logicalSeconds =
                    step.LogicalDurationSeconds *
                    stepProgress.Progress01;

                if (logicalSeconds <
                    latestLogicalSeconds[authoredIndex])
                {
                    logicalSeconds =
                        latestLogicalSeconds[authoredIndex];
                }
            }

            latestLogicalSeconds[authoredIndex] =
                logicalSeconds;

            progress.Add(
                new LaunchSimulationProgressSample(
                    authoredIndex,
                    sampleIndex,
                    stepProgress.Progress01,
                    stepProgress.IsIndeterminate,
                    stepProgress.Message,
                    logicalSeconds));
        }

        public void StepCompleted(
            StartupStepExecution execution)
        {
        }
    }
}
