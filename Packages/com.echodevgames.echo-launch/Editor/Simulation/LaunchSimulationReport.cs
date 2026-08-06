using System;

namespace EchoDevGames.EchoLaunch.Editor.Simulation
{
    internal sealed class LaunchSimulationReport
    {
        internal const int CurrentSchemaVersion = 1;

        private readonly LaunchSimulationStepReport[] steps;
        private readonly LaunchSimulationProgressSample[] progressSamples;

        internal LaunchSimulationReport(
            LaunchSimulationStatus status,
            LaunchSimulationRequest request,
            string planFingerprint,
            int authoredEntryCount,
            int disabledEntryCount,
            int unvisitedEntryCount,
            bool wasCancelled,
            LaunchSimulationStepReport[] steps,
            LaunchSimulationProgressSample[] progressSamples,
            string diagnosticCode,
            string diagnosticMessage,
            string diagnosticDetails)
        {
            if (authoredEntryCount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(authoredEntryCount));
            }

            if (disabledEntryCount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(disabledEntryCount));
            }

            if (unvisitedEntryCount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(unvisitedEntryCount));
            }

            SchemaVersion = CurrentSchemaVersion;
            Status = status;
            Preset = request != null
                ? request.Preset
                : LaunchSimulationPreset.ImmediateSuccess;
            LogicalDurationSeconds = request != null
                ? request.LogicalDurationSeconds
                : 0d;
            ProgressSampleRequestCount = request != null
                ? request.ProgressSampleCount
                : 0;
            TimeoutSeconds = request != null
                ? request.TimeoutSeconds
                : 0d;
            RequestMessage = request?.Message ?? string.Empty;
            RequestFingerprint =
                request?.RequestFingerprint ?? string.Empty;
            PlanFingerprint =
                planFingerprint ?? string.Empty;

            this.steps =
                steps != null
                    ? (LaunchSimulationStepReport[])steps.Clone()
                    : Array.Empty<LaunchSimulationStepReport>();

            this.progressSamples =
                progressSamples != null
                    ? (LaunchSimulationProgressSample[])
                        progressSamples.Clone()
                    : Array.Empty<LaunchSimulationProgressSample>();

            AuthoredEntryCount = authoredEntryCount;
            DisabledEntryCount = disabledEntryCount;
            UnvisitedEntryCount = unvisitedEntryCount;
            WasCancelled = wasCancelled;
            DiagnosticCode =
                Normalize(diagnosticCode);
            DiagnosticMessage =
                Normalize(diagnosticMessage);
            DiagnosticDetails =
                Normalize(diagnosticDetails);

            ReportFingerprint =
                LaunchSimulationFingerprint.ComputeReport(this);

            Text =
                LaunchSimulationTextFormatter.Format(this);
        }

        internal int SchemaVersion { get; }
        internal LaunchSimulationStatus Status { get; }
        internal LaunchSimulationPreset Preset { get; }
        internal double LogicalDurationSeconds { get; }
        internal int ProgressSampleRequestCount { get; }
        internal double TimeoutSeconds { get; }
        internal string RequestMessage { get; }
        internal string RequestFingerprint { get; }
        internal string PlanFingerprint { get; }
        internal string ReportFingerprint { get; }
        internal int AuthoredEntryCount { get; }
        internal int DisabledEntryCount { get; }
        internal int AttemptedEntryCount => steps.Length;
        internal int UnvisitedEntryCount { get; }
        internal bool WasCancelled { get; }
        internal string DiagnosticCode { get; }
        internal string DiagnosticMessage { get; }
        internal string DiagnosticDetails { get; }
        internal int StepCount => steps.Length;
        internal int ProgressSampleCount => progressSamples.Length;
        internal string Text { get; }

        internal LaunchSimulationStepReport GetStep(
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

        internal LaunchSimulationProgressSample GetProgressSample(
            int index)
        {
            if (index < 0 ||
                index >= progressSamples.Length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(index));
            }

            return progressSamples[index];
        }

        private static string Normalize(
            string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Trim();
        }
    }
}
