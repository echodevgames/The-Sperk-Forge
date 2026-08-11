using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave
{
    internal enum SavePackageDocumentMigrationPlanStatus
    {
        Succeeded = 0,
        InvalidRequest = 1,
        RegistryInvalid = 2,
        NewerVersionUnsupported = 3,
        MissingEdge = 4,
        StepLimitExceeded = 5,
        LoopDetected = 6,
        InvalidChain = 7
    }

    internal readonly struct SavePackageDocumentMigrationPlanResult
    {
        internal SavePackageDocumentMigrationPlanResult(
            SavePackageDocumentMigrationPlanStatus status,
            string diagnosticCode,
            string message)
        {
            Status = status;
            DiagnosticCode =
                SavePackageDocumentMigrationText.BoundDiagnosticCode(
                    diagnosticCode);
            Message =
                SavePackageDocumentMigrationText.BoundMessage(
                    message);
        }

        internal SavePackageDocumentMigrationPlanStatus Status { get; }

        internal string DiagnosticCode { get; }

        internal string Message { get; }

        internal bool Succeeded =>
            Status == SavePackageDocumentMigrationPlanStatus.Succeeded;
    }

    internal sealed class SavePackageDocumentMigrationPlan
    {
        private readonly SavePackageDocumentMigrationPlanStep[] steps;

        internal SavePackageDocumentMigrationPlan(
            string documentKind,
            SavePackageDocumentVersion sourceVersion,
            SavePackageDocumentVersion targetVersion,
            SavePackageDocumentMigrationPlanStep[] steps)
        {
            DocumentKind = documentKind ?? string.Empty;
            SourceVersion = sourceVersion;
            TargetVersion = targetVersion;
            this.steps =
                steps == null
                    ? Array.Empty<SavePackageDocumentMigrationPlanStep>()
                    : (SavePackageDocumentMigrationPlanStep[])steps.Clone();
        }

        internal string DocumentKind { get; }

        internal SavePackageDocumentVersion SourceVersion { get; }

        internal SavePackageDocumentVersion TargetVersion { get; }

        internal IReadOnlyList<SavePackageDocumentMigrationPlanStep> Steps =>
            steps;

        internal int Count =>
            steps.Length;
    }

    internal readonly struct SavePackageDocumentMigrationPlanStep
    {
        internal SavePackageDocumentMigrationPlanStep(
            ISavePackageDocumentMigrationStep step,
            string stepId,
            string documentKind,
            SavePackageDocumentVersion sourceVersion,
            SavePackageDocumentVersion targetVersion)
        {
            Step = step;
            StepId = stepId ?? string.Empty;
            DocumentKind = documentKind ?? string.Empty;
            SourceVersion = sourceVersion;
            TargetVersion = targetVersion;
        }

        internal ISavePackageDocumentMigrationStep Step { get; }

        internal string StepId { get; }

        internal string DocumentKind { get; }

        internal SavePackageDocumentVersion SourceVersion { get; }

        internal SavePackageDocumentVersion TargetVersion { get; }
    }

    /// <summary>
    /// Immutable package-owned registry. Production registration is not exposed.
    /// </summary>
    internal sealed class SavePackageDocumentMigrationRegistry
    {
        internal const int DefaultMaximumPlanSteps =
            64;

        private const int MaximumStepIdLength =
            128;

        private readonly struct EdgeKey :
            IEquatable<EdgeKey>
        {
            internal EdgeKey(
                string documentKind,
                SavePackageDocumentVersion sourceVersion)
            {
                DocumentKind = documentKind ?? string.Empty;
                SourceVersion = sourceVersion;
            }

            internal string DocumentKind { get; }

            internal SavePackageDocumentVersion SourceVersion { get; }

            public bool Equals(
                EdgeKey other) =>
                string.Equals(
                    DocumentKind,
                    other.DocumentKind,
                    StringComparison.Ordinal) &&
                SourceVersion == other.SourceVersion;

            public override bool Equals(
                object obj) =>
                obj is EdgeKey other &&
                Equals(other);

            public override int GetHashCode()
            {
                unchecked
                {
                    return
                        ((DocumentKind == null
                            ? 0
                            : StringComparer.Ordinal.GetHashCode(
                                DocumentKind)) * 397) ^
                        SourceVersion.GetHashCode();
                }
            }
        }

        private sealed class Entry
        {
            internal Entry(
                ISavePackageDocumentMigrationStep step,
                string stepId,
                string documentKind,
                SavePackageDocumentVersion sourceVersion,
                SavePackageDocumentVersion targetVersion)
            {
                Step = step;
                StepId = stepId;
                DocumentKind = documentKind;
                SourceVersion = sourceVersion;
                TargetVersion = targetVersion;
            }

            internal ISavePackageDocumentMigrationStep Step { get; }

            internal string StepId { get; }

            internal string DocumentKind { get; }

            internal SavePackageDocumentVersion SourceVersion { get; }

            internal SavePackageDocumentVersion TargetVersion { get; }
        }

        private readonly Dictionary<EdgeKey, Entry> entries =
            new Dictionary<EdgeKey, Entry>();

        private readonly HashSet<string> stepIds =
            new HashSet<string>(
                StringComparer.Ordinal);

        internal SavePackageDocumentMigrationRegistry(
            IEnumerable<ISavePackageDocumentMigrationStep> steps)
        {
            IsValid = true;
            DiagnosticCode = string.Empty;
            Message =
                "The Chronicle package-document migration registry is valid.";

            if (steps == null)
            {
                Invalidate(
                    SavePackageDocumentMigrationDiagnosticCodes.RegistryInvalid,
                    "Chronicle package-document migration requires a non-null package-owned step set.");
                return;
            }

            try
            {
                foreach (ISavePackageDocumentMigrationStep step in steps)
                {
                    if (!TryCreateEntry(
                            step,
                            out Entry entry,
                            out string diagnosticCode,
                            out string message))
                    {
                        Invalidate(
                            diagnosticCode,
                            message);
                        return;
                    }

                    if (!stepIds.Add(
                            entry.StepId))
                    {
                        Invalidate(
                            SavePackageDocumentMigrationDiagnosticCodes.RegistryInvalid,
                            "Chronicle package-document migration contains duplicate stable step IDs.");
                        return;
                    }

                    EdgeKey key =
                        new EdgeKey(
                            entry.DocumentKind,
                            entry.SourceVersion);

                    if (entries.ContainsKey(key))
                    {
                        Invalidate(
                            SavePackageDocumentMigrationDiagnosticCodes.DuplicateEdge,
                            "Chronicle package-document migration contains duplicate or ambiguous outbound edges for one exact document-kind/source-version identity.");
                        return;
                    }

                    entries.Add(
                        key,
                        entry);
                }
            }
            catch (Exception exception)
            {
                Invalidate(
                    SavePackageDocumentMigrationDiagnosticCodes.RegistryInvalid,
                    "Chronicle package-document migration could not enumerate its package-owned step set. " +
                    exception.GetType().Name +
                    ": " +
                    exception.Message);
            }
        }

        internal bool IsValid { get; private set; }

        internal string DiagnosticCode { get; private set; }

        internal string Message { get; private set; }

        internal int Count =>
            entries.Count;

        internal static SavePackageDocumentMigrationRegistry CreateProduction() =>
            new SavePackageDocumentMigrationRegistry(
                Array.Empty<ISavePackageDocumentMigrationStep>());

        internal SavePackageDocumentMigrationPlanResult TryBuildPlan(
            string documentKind,
            SavePackageDocumentVersion sourceVersion,
            SavePackageDocumentVersion targetVersion,
            int maxSteps,
            out SavePackageDocumentMigrationPlan plan)
        {
            plan = null;

            if (!SavePackageDocumentVersionAuthority.TryGetCurrent(
                    documentKind,
                    out SavePackageDocumentVersion currentVersion) ||
                currentVersion != targetVersion ||
                maxSteps <= 0)
            {
                return Failure(
                    SavePackageDocumentMigrationPlanStatus.InvalidRequest,
                    SavePackageDocumentMigrationDiagnosticCodes.InvalidRequest,
                    "Chronicle package-document migration planning requires one supported document kind, its exact current target, and a positive step bound.");
            }

            if (!IsValid)
            {
                return Failure(
                    SavePackageDocumentMigrationPlanStatus.RegistryInvalid,
                    string.IsNullOrEmpty(DiagnosticCode)
                        ? SavePackageDocumentMigrationDiagnosticCodes.RegistryInvalid
                        : DiagnosticCode,
                    Message);
            }

            if (sourceVersion > targetVersion)
            {
                return Failure(
                    SavePackageDocumentMigrationPlanStatus.NewerVersionUnsupported,
                    SavePackageDocumentMigrationDiagnosticCodes.NewerVersionUnsupported,
                    "Chronicle package-document migration does not support downgrade paths from newer stored package formats.");
            }

            if (sourceVersion == targetVersion)
            {
                plan =
                    new SavePackageDocumentMigrationPlan(
                        documentKind,
                        sourceVersion,
                        targetVersion,
                        Array.Empty<SavePackageDocumentMigrationPlanStep>());

                return Success(
                    "The Chronicle package document already uses the exact current format.");
            }

            List<SavePackageDocumentMigrationPlanStep> planned =
                new List<SavePackageDocumentMigrationPlanStep>();

            HashSet<SavePackageDocumentVersion> visited =
                new HashSet<SavePackageDocumentVersion>();

            SavePackageDocumentVersion cursor =
                sourceVersion;

            while (cursor != targetVersion)
            {
                if (!visited.Add(cursor))
                {
                    return Failure(
                        SavePackageDocumentMigrationPlanStatus.LoopDetected,
                        SavePackageDocumentMigrationDiagnosticCodes.LoopDetected,
                        "Chronicle package-document migration detected a repeated exact source version while resolving the chain.");
                }

                if (planned.Count >= maxSteps)
                {
                    return Failure(
                        SavePackageDocumentMigrationPlanStatus.StepLimitExceeded,
                        SavePackageDocumentMigrationDiagnosticCodes.StepLimitExceeded,
                        "Chronicle package-document migration exceeds the bounded chain length.");
                }

                EdgeKey key =
                    new EdgeKey(
                        documentKind,
                        cursor);

                if (!entries.TryGetValue(
                        key,
                        out Entry entry))
                {
                    return Failure(
                        SavePackageDocumentMigrationPlanStatus.MissingEdge,
                        SavePackageDocumentMigrationDiagnosticCodes.ChainMissing,
                        "Chronicle package-document migration has no complete contiguous edge from " +
                        documentKind +
                        " " +
                        cursor +
                        ".");
                }

                if (entry.SourceVersion != cursor ||
                    entry.TargetVersion <= cursor ||
                    entry.TargetVersion > targetVersion)
                {
                    return Failure(
                        SavePackageDocumentMigrationPlanStatus.InvalidChain,
                        SavePackageDocumentMigrationDiagnosticCodes.InvalidChain,
                        "Chronicle package-document migration encountered a non-contiguous, downgrade, or overshooting edge.");
                }

                planned.Add(
                    new SavePackageDocumentMigrationPlanStep(
                        entry.Step,
                        entry.StepId,
                        entry.DocumentKind,
                        entry.SourceVersion,
                        entry.TargetVersion));

                cursor =
                    entry.TargetVersion;
            }

            plan =
                new SavePackageDocumentMigrationPlan(
                    documentKind,
                    sourceVersion,
                    targetVersion,
                    planned.ToArray());

            return Success(
                "The Chronicle package-document migration chain is complete and exact.");
        }

        internal bool Owns(
            SavePackageDocumentMigrationPlanStep plannedStep)
        {
            EdgeKey key =
                new EdgeKey(
                    plannedStep.DocumentKind,
                    plannedStep.SourceVersion);

            return
                entries.TryGetValue(
                    key,
                    out Entry entry) &&
                ReferenceEquals(
                    entry.Step,
                    plannedStep.Step) &&
                string.Equals(
                    entry.StepId,
                    plannedStep.StepId,
                    StringComparison.Ordinal) &&
                entry.TargetVersion ==
                    plannedStep.TargetVersion;
        }

        private static bool TryCreateEntry(
            ISavePackageDocumentMigrationStep step,
            out Entry entry,
            out string diagnosticCode,
            out string message)
        {
            entry = null;
            diagnosticCode =
                SavePackageDocumentMigrationDiagnosticCodes.RegistryInvalid;
            message =
                "Chronicle package-document migration contains an invalid package-owned step.";

            if (step == null)
            {
                return false;
            }

            string stepId;
            string documentKind;
            SavePackageDocumentVersion sourceVersion;
            SavePackageDocumentVersion targetVersion;

            try
            {
                stepId = step.StepId;
                documentKind = step.DocumentKind;
                sourceVersion = step.SourceVersion;
                targetVersion = step.TargetVersion;
            }
            catch (Exception exception)
            {
                message =
                    "Chronicle package-document migration could not read one step descriptor. " +
                    exception.GetType().Name +
                    ": " +
                    exception.Message;
                return false;
            }

            if (string.IsNullOrWhiteSpace(stepId) ||
                stepId.Length > MaximumStepIdLength ||
                !string.Equals(
                    stepId,
                    stepId.Trim(),
                    StringComparison.Ordinal) ||
                !SavePackageDocumentVersionAuthority.TryGetCurrent(
                    documentKind,
                    out SavePackageDocumentVersion currentVersion) ||
                sourceVersion >= targetVersion ||
                targetVersion > currentVersion)
            {
                message =
                    "Chronicle package-document migration steps require one bounded stable ID, supported document kind, strictly increasing exact edge, and target no newer than the runtime current format.";
                return false;
            }

            entry =
                new Entry(
                    step,
                    stepId,
                    documentKind,
                    sourceVersion,
                    targetVersion);

            diagnosticCode = string.Empty;
            message = string.Empty;
            return true;
        }

        private void Invalidate(
            string diagnosticCode,
            string message)
        {
            IsValid = false;
            DiagnosticCode =
                SavePackageDocumentMigrationText.BoundDiagnosticCode(
                    diagnosticCode);
            Message =
                SavePackageDocumentMigrationText.BoundMessage(
                    message);
            entries.Clear();
            stepIds.Clear();
        }

        private static SavePackageDocumentMigrationPlanResult Success(
            string message) =>
            new SavePackageDocumentMigrationPlanResult(
                SavePackageDocumentMigrationPlanStatus.Succeeded,
                string.Empty,
                message);

        private static SavePackageDocumentMigrationPlanResult Failure(
            SavePackageDocumentMigrationPlanStatus status,
            string diagnosticCode,
            string message) =>
            new SavePackageDocumentMigrationPlanResult(
                status,
                diagnosticCode,
                message);
    }

    internal static class SavePackageDocumentMigrationText
    {
        internal const int MaximumDiagnosticCodeLength =
            128;

        internal const int MaximumMessageLength =
            1024;

        internal static string BoundDiagnosticCode(
            string value) =>
            Bound(
                value,
                MaximumDiagnosticCodeLength);

        internal static string BoundMessage(
            string value) =>
            Bound(
                value,
                MaximumMessageLength);

        private static string Bound(
            string value,
            int maximumLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return
                value.Length <= maximumLength
                    ? value
                    : value.Substring(
                        0,
                        maximumLength);
        }
    }

    internal static class SavePackageDocumentMigrationDiagnosticCodes
    {
        internal const string InvalidRequest =
            "ESV-PKG-MIG-001";
        internal const string RegistryInvalid =
            "ESV-PKG-MIG-002";
        internal const string DuplicateEdge =
            "ESV-PKG-MIG-003";
        internal const string NewerVersionUnsupported =
            "ESV-PKG-MIG-004";
        internal const string ChainMissing =
            "ESV-PKG-MIG-005";
        internal const string StepLimitExceeded =
            "ESV-PKG-MIG-006";
        internal const string LoopDetected =
            "ESV-PKG-MIG-007";
        internal const string InvalidChain =
            "ESV-PKG-MIG-008";
        internal const string ProbeFailed =
            "ESV-PKG-MIG-009";
        internal const string StepFailed =
            "ESV-PKG-MIG-010";
        internal const string InvalidOutput =
            "ESV-PKG-MIG-011";
        internal const string FinalValidationFailed =
            "ESV-PKG-MIG-012";
    }
}
