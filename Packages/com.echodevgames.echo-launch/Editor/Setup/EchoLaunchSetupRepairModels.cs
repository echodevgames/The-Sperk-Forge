using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EchoDevGames.EchoLaunch.Editor.Setup
{
    internal sealed class EchoLaunchSetupRepairRequest
    {
        internal EchoLaunchSetupRepairRequest(
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

    internal sealed class EchoLaunchSetupRepairEligibility
    {
        internal EchoLaunchSetupRepairEligibility(
            bool canRepair,
            string diagnosticCode,
            string message)
        {
            CanRepair = canRepair;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
        }

        internal bool CanRepair { get; }
        internal string DiagnosticCode { get; }
        internal string Message { get; }
    }

    internal sealed class EchoLaunchSetupRepairResult
    {
        private readonly ReadOnlyCollection<EchoLaunchSetupChange> changes;
        private readonly ReadOnlyCollection<string> createdPaths;
        private readonly ReadOnlyCollection<string> repairedPaths;
        private readonly ReadOnlyCollection<string> reusedPaths;
        private readonly ReadOnlyCollection<string> unchangedPaths;
        private readonly ReadOnlyCollection<EchoLaunchSetupOperation>
            repairOperations;
        private readonly ReadOnlyCollection<string> manualRecoveryPaths;

        internal EchoLaunchSetupRepairResult(
            EchoLaunchSetupRepairStatus status,
            string diagnosticCode,
            string message,
            IEnumerable<EchoLaunchSetupChange> changes,
            IEnumerable<string> createdPaths,
            IEnumerable<string> repairedPaths,
            IEnumerable<string> reusedPaths,
            string backupDirectory,
            string buildSettingsBefore,
            string buildSettingsAfter,
            bool rollbackCompleted,
            IEnumerable<string> manualRecoveryPaths,
            EchoLaunchSetupPlanStatus? finalPlanStatus,
            string finalPlanFingerprint,
            IEnumerable<string> unchangedPaths = null,
            IEnumerable<EchoLaunchSetupOperation> repairOperations = null)
        {
            Status = status;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
            this.changes = CopyChanges(changes);
            this.createdPaths = CopyPaths(createdPaths);
            this.repairedPaths = CopyPaths(repairedPaths);
            this.reusedPaths = CopyPaths(reusedPaths);
            this.unchangedPaths = CopyPaths(unchangedPaths);
            this.repairOperations =
                new ReadOnlyCollection<EchoLaunchSetupOperation>(
                    repairOperations == null
                        ? new List<EchoLaunchSetupOperation>()
                        : new List<EchoLaunchSetupOperation>(repairOperations));
            BackupDirectory = backupDirectory ?? string.Empty;
            BuildSettingsBefore = buildSettingsBefore ?? string.Empty;
            BuildSettingsAfter = buildSettingsAfter ?? string.Empty;
            RollbackCompleted = rollbackCompleted;
            this.manualRecoveryPaths = CopyPaths(manualRecoveryPaths);
            FinalPlanStatus = finalPlanStatus;
            FinalPlanFingerprint = finalPlanFingerprint ?? string.Empty;
        }

        internal EchoLaunchSetupRepairStatus Status { get; }
        internal string DiagnosticCode { get; }
        internal string Message { get; }
        internal IReadOnlyList<EchoLaunchSetupChange> Changes => changes;
        internal IReadOnlyList<string> CreatedPaths => createdPaths;
        internal IReadOnlyList<string> RepairedPaths => repairedPaths;
        internal IReadOnlyList<string> ReusedPaths => reusedPaths;
        internal IReadOnlyList<string> UnchangedPaths => unchangedPaths;
        internal IReadOnlyList<EchoLaunchSetupOperation> RepairOperations =>
            repairOperations;
        internal string BackupDirectory { get; }
        internal string BuildSettingsBefore { get; }
        internal string BuildSettingsAfter { get; }
        internal bool RollbackCompleted { get; }
        internal IReadOnlyList<string> ManualRecoveryPaths =>
            manualRecoveryPaths;
        internal EchoLaunchSetupPlanStatus? FinalPlanStatus { get; }
        internal string FinalPlanFingerprint { get; }

        internal static EchoLaunchSetupRepairResult Simple(
            EchoLaunchSetupRepairStatus status,
            string diagnosticCode,
            string message,
            EchoLaunchSetupPlan plan = null)
        {
            return new EchoLaunchSetupRepairResult(
                status,
                diagnosticCode,
                message,
                Array.Empty<EchoLaunchSetupChange>(),
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>(),
                string.Empty,
                string.Empty,
                string.Empty,
                false,
                Array.Empty<string>(),
                plan == null ? (EchoLaunchSetupPlanStatus?)null : plan.Status,
                plan == null ? string.Empty : plan.PlanFingerprint);
        }

        private static ReadOnlyCollection<EchoLaunchSetupChange> CopyChanges(
            IEnumerable<EchoLaunchSetupChange> source)
        {
            return new ReadOnlyCollection<EchoLaunchSetupChange>(
                source == null
                    ? new List<EchoLaunchSetupChange>()
                    : new List<EchoLaunchSetupChange>(source));
        }

        private static ReadOnlyCollection<string> CopyPaths(
            IEnumerable<string> source)
        {
            List<string> result = source == null
                ? new List<string>()
                : new List<string>(source);
            for (int index = 0; index < result.Count; index++)
            {
                result[index] =
                    EchoLaunchSetupPathUtility.NormalizeSeparators(
                        result[index]);
            }
            result.Sort(StringComparer.Ordinal);
            return new ReadOnlyCollection<string>(result);
        }
    }
}
