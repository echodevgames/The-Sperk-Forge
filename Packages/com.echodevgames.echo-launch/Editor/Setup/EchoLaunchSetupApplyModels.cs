using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal sealed class EchoLaunchSetupApplyRequest
    {
        internal EchoLaunchSetupApplyRequest(
            EchoLaunchSetupPlan displayedPlan,
            bool confirmed,
            bool approvePlaceFirst)
        {
            DisplayedPlan = displayedPlan;
            Confirmed = confirmed;
            ApprovePlaceFirst = approvePlaceFirst;
        }

        internal EchoLaunchSetupPlan DisplayedPlan { get; }
        internal bool Confirmed { get; }
        internal bool ApprovePlaceFirst { get; }
    }

    internal sealed class EchoLaunchSetupApplyEligibility
    {
        internal EchoLaunchSetupApplyEligibility(
            bool canApply,
            string diagnosticCode,
            string message)
        {
            CanApply = canApply;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
        }

        internal bool CanApply { get; }
        internal string DiagnosticCode { get; }
        internal string Message { get; }
    }

    internal sealed class EchoLaunchSetupChange :
        IEquatable<EchoLaunchSetupChange>
    {
        internal EchoLaunchSetupChange(
            EchoLaunchSetupChangeKind kind,
            string path,
            string message)
        {
            Kind = kind;
            Path = EchoLaunchSetupPathUtility.NormalizeSeparators(path);
            Message = message ?? string.Empty;
        }

        internal EchoLaunchSetupChangeKind Kind { get; }
        internal string Path { get; }
        internal string Message { get; }

        public bool Equals(EchoLaunchSetupChange other)
        {
            return other != null &&
                   Kind == other.Kind &&
                   string.Equals(Path, other.Path, StringComparison.Ordinal) &&
                   string.Equals(Message, other.Message, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EchoLaunchSetupChange);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Kind * 397) ^ Path.GetHashCode();
            }
        }
    }

    internal sealed class EchoLaunchSetupRollbackResult
    {
        private readonly ReadOnlyCollection<string> manualRecoveryPaths;

        internal EchoLaunchSetupRollbackResult(
            bool completed,
            IEnumerable<string> manualRecoveryPaths)
        {
            Completed = completed;
            this.manualRecoveryPaths =
                new ReadOnlyCollection<string>(
                    CopyPaths(manualRecoveryPaths));
        }

        internal bool Completed { get; }
        internal IReadOnlyList<string> ManualRecoveryPaths =>
            manualRecoveryPaths;

        private static List<string> CopyPaths(IEnumerable<string> source)
        {
            List<string> result =
                source == null
                    ? new List<string>()
                    : new List<string>(source);

            for (int index = 0; index < result.Count; index++)
            {
                result[index] =
                    EchoLaunchSetupPathUtility.NormalizeSeparators(
                        result[index]);
            }

            result.Sort(StringComparer.Ordinal);
            return result;
        }
    }

    internal sealed class EchoLaunchSetupApplyResult
    {
        private readonly ReadOnlyCollection<EchoLaunchSetupChange> changes;
        private readonly ReadOnlyCollection<string> createdPaths;
        private readonly ReadOnlyCollection<string> reusedPaths;
        private readonly ReadOnlyCollection<string> manualRecoveryPaths;

        internal EchoLaunchSetupApplyResult(
            EchoLaunchSetupApplyStatus status,
            string diagnosticCode,
            string message,
            IEnumerable<EchoLaunchSetupChange> changes,
            IEnumerable<string> createdPaths,
            IEnumerable<string> reusedPaths,
            string buildSettingsBefore,
            string buildSettingsAfter,
            bool rollbackCompleted,
            IEnumerable<string> manualRecoveryPaths,
            EchoLaunchSetupPlanStatus? finalPlanStatus,
            string finalPlanFingerprint)
        {
            Status = status;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
            this.changes = new ReadOnlyCollection<EchoLaunchSetupChange>(
                changes == null
                    ? new List<EchoLaunchSetupChange>()
                    : new List<EchoLaunchSetupChange>(changes));
            this.createdPaths =
                new ReadOnlyCollection<string>(CopyPaths(createdPaths));
            this.reusedPaths =
                new ReadOnlyCollection<string>(CopyPaths(reusedPaths));
            BuildSettingsBefore = buildSettingsBefore ?? string.Empty;
            BuildSettingsAfter = buildSettingsAfter ?? string.Empty;
            RollbackCompleted = rollbackCompleted;
            this.manualRecoveryPaths =
                new ReadOnlyCollection<string>(
                    CopyPaths(manualRecoveryPaths));
            FinalPlanStatus = finalPlanStatus;
            FinalPlanFingerprint = finalPlanFingerprint ?? string.Empty;
        }

        internal EchoLaunchSetupApplyStatus Status { get; }
        internal string DiagnosticCode { get; }
        internal string Message { get; }
        internal IReadOnlyList<EchoLaunchSetupChange> Changes => changes;
        internal IReadOnlyList<string> CreatedPaths => createdPaths;
        internal IReadOnlyList<string> ReusedPaths => reusedPaths;
        internal string BuildSettingsBefore { get; }
        internal string BuildSettingsAfter { get; }
        internal bool RollbackCompleted { get; }
        internal IReadOnlyList<string> ManualRecoveryPaths =>
            manualRecoveryPaths;
        internal EchoLaunchSetupPlanStatus? FinalPlanStatus { get; }
        internal string FinalPlanFingerprint { get; }

        internal static EchoLaunchSetupApplyResult Simple(
            EchoLaunchSetupApplyStatus status,
            string diagnosticCode,
            string message,
            EchoLaunchSetupPlan plan = null)
        {
            return new EchoLaunchSetupApplyResult(
                status,
                diagnosticCode,
                message,
                Array.Empty<EchoLaunchSetupChange>(),
                Array.Empty<string>(),
                Array.Empty<string>(),
                string.Empty,
                string.Empty,
                false,
                Array.Empty<string>(),
                plan == null ? (EchoLaunchSetupPlanStatus?)null : plan.Status,
                plan == null ? string.Empty : plan.PlanFingerprint);
        }

        private static List<string> CopyPaths(IEnumerable<string> source)
        {
            List<string> result =
                source == null
                    ? new List<string>()
                    : new List<string>(source);

            for (int index = 0; index < result.Count; index++)
            {
                result[index] =
                    EchoLaunchSetupPathUtility.NormalizeSeparators(
                        result[index]);
            }

            result.Sort(StringComparer.Ordinal);
            return result;
        }
    }
}
