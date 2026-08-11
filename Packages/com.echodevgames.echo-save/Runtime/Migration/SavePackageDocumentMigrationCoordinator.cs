using System;
using System.Collections.Generic;
using System.Text;

namespace EchoDevGames.EchoSave
{
    internal enum SavePackageDocumentMigrationStatus
    {
        Succeeded = 0,
        InvalidRequest = 1,
        PlanUnavailable = 2,
        StepFailed = 3,
        InvalidOutput = 4
    }

    internal readonly struct SavePackageDocumentMigrationProvenanceEntry
    {
        internal SavePackageDocumentMigrationProvenanceEntry(
            string stepId,
            string documentKind,
            SavePackageDocumentVersion sourceVersion,
            SavePackageDocumentVersion targetVersion,
            int chainPosition,
            int chainCount)
        {
            StepId = stepId ?? string.Empty;
            DocumentKind = documentKind ?? string.Empty;
            SourceVersion = sourceVersion;
            TargetVersion = targetVersion;
            ChainPosition = chainPosition;
            ChainCount = chainCount;
        }

        internal string StepId { get; }

        internal string DocumentKind { get; }

        internal SavePackageDocumentVersion SourceVersion { get; }

        internal SavePackageDocumentVersion TargetVersion { get; }

        internal int ChainPosition { get; }

        internal int ChainCount { get; }
    }

    internal sealed class SavePackageDocumentMigrationResult
    {
        private readonly SavePackageDocumentMigrationProvenanceEntry[] provenance;

        internal SavePackageDocumentMigrationResult(
            SavePackageDocumentMigrationStatus status,
            string serializedDocument,
            SavePackageDocumentMigrationProvenanceEntry[] provenance,
            SavePackageDocumentMigrationPlanStatus planStatus,
            string diagnosticCode,
            string message)
        {
            Status = status;
            SerializedDocument = serializedDocument;
            this.provenance =
                provenance == null
                    ? Array.Empty<SavePackageDocumentMigrationProvenanceEntry>()
                    : (SavePackageDocumentMigrationProvenanceEntry[])provenance.Clone();
            PlanStatus = planStatus;
            DiagnosticCode =
                SavePackageDocumentMigrationText.BoundDiagnosticCode(
                    diagnosticCode);
            Message =
                SavePackageDocumentMigrationText.BoundMessage(
                    message);
        }

        internal SavePackageDocumentMigrationStatus Status { get; }

        internal string SerializedDocument { get; }

        internal IReadOnlyList<SavePackageDocumentMigrationProvenanceEntry>
            Provenance =>
            provenance;

        internal SavePackageDocumentMigrationPlanStatus PlanStatus { get; }

        internal string DiagnosticCode { get; }

        internal string Message { get; }

        internal bool Succeeded =>
            Status == SavePackageDocumentMigrationStatus.Succeeded;
    }

    /// <summary>
    /// Executes one complete package-owned migration chain entirely in memory.
    /// </summary>
    internal sealed class SavePackageDocumentMigrationCoordinator
    {
        private readonly SavePackageDocumentMigrationRegistry registry;
        private readonly int maximumPlanSteps;

        internal SavePackageDocumentMigrationCoordinator(
            SavePackageDocumentMigrationRegistry registry,
            int maximumPlanSteps =
                SavePackageDocumentMigrationRegistry.DefaultMaximumPlanSteps)
        {
            this.registry =
                registry ??
                throw new ArgumentNullException(
                    nameof(registry));

            if (maximumPlanSteps <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maximumPlanSteps));
            }

            this.maximumPlanSteps =
                maximumPlanSteps;
        }

        internal SavePackageDocumentMigrationResult MigrateToCurrent(
            string documentKind,
            SavePackageDocumentVersion sourceVersion,
            SavePackageDocumentVersion currentVersion,
            string serializedDocument)
        {
            if (serializedDocument == null ||
                !SavePackageDocumentVersionAuthority.TryGetCurrent(
                    documentKind,
                    out SavePackageDocumentVersion authorityCurrent) ||
                authorityCurrent != currentVersion)
            {
                return Failure(
                    SavePackageDocumentMigrationStatus.InvalidRequest,
                    SavePackageDocumentMigrationPlanStatus.InvalidRequest,
                    SavePackageDocumentMigrationDiagnosticCodes.InvalidRequest,
                    "Chronicle package-document migration requires a supported kind, exact current target, and detached serialized source.");
            }

            SavePackageDocumentMigrationPlanResult planResult =
                registry.TryBuildPlan(
                    documentKind,
                    sourceVersion,
                    currentVersion,
                    maximumPlanSteps,
                    out SavePackageDocumentMigrationPlan plan);

            if (!planResult.Succeeded ||
                plan == null)
            {
                return Failure(
                    SavePackageDocumentMigrationStatus.PlanUnavailable,
                    planResult.Status,
                    planResult.DiagnosticCode,
                    planResult.Message);
            }

            string currentSerialized =
                serializedDocument;
            SavePackageDocumentVersion cursor =
                sourceVersion;

            List<SavePackageDocumentMigrationProvenanceEntry> provenance =
                new List<SavePackageDocumentMigrationProvenanceEntry>(
                    plan.Count);

            IReadOnlyList<SavePackageDocumentMigrationPlanStep> steps =
                plan.Steps;

            for (int i = 0;
                 i < steps.Count;
                 i++)
            {
                SavePackageDocumentMigrationPlanStep plannedStep =
                    steps[i];

                if (!registry.Owns(
                        plannedStep) ||
                    !string.Equals(
                        plannedStep.DocumentKind,
                        documentKind,
                        StringComparison.Ordinal) ||
                    plannedStep.SourceVersion != cursor)
                {
                    return Failure(
                        SavePackageDocumentMigrationStatus.PlanUnavailable,
                        SavePackageDocumentMigrationPlanStatus.InvalidChain,
                        SavePackageDocumentMigrationDiagnosticCodes.InvalidChain,
                        "Chronicle package-document migration registry identity changed or no longer matches the planned exact chain.");
                }

                SavePackageDocumentMigrationStepResult stepResult;
                try
                {
                    stepResult =
                        plannedStep.Step.Migrate(
                            currentSerialized);
                }
                catch (Exception exception)
                {
                    return Failure(
                        SavePackageDocumentMigrationStatus.StepFailed,
                        SavePackageDocumentMigrationPlanStatus.Succeeded,
                        SavePackageDocumentMigrationDiagnosticCodes.StepFailed,
                        "Chronicle package-document migration step '" +
                        plannedStep.StepId +
                        "' threw " +
                        exception.GetType().Name +
                        ": " +
                        exception.Message);
                }

                if (!stepResult.Succeeded)
                {
                    return Failure(
                        SavePackageDocumentMigrationStatus.StepFailed,
                        SavePackageDocumentMigrationPlanStatus.Succeeded,
                        string.IsNullOrEmpty(
                            stepResult.DiagnosticCode)
                            ? SavePackageDocumentMigrationDiagnosticCodes.StepFailed
                            : stepResult.DiagnosticCode,
                        string.IsNullOrEmpty(
                            stepResult.Message)
                            ? "Chronicle package-document migration step reported failure."
                            : stepResult.Message);
                }

                string output =
                    stepResult.SerializedDocument;

                if (string.IsNullOrWhiteSpace(
                        output))
                {
                    return Failure(
                        SavePackageDocumentMigrationStatus.InvalidOutput,
                        SavePackageDocumentMigrationPlanStatus.Succeeded,
                        SavePackageDocumentMigrationDiagnosticCodes.InvalidOutput,
                        "Chronicle package-document migration step returned empty serialized output.");
                }

                int outputBytes;
                try
                {
                    outputBytes =
                        Encoding.UTF8.GetByteCount(
                            output);
                }
                catch (ArgumentException exception)
                {
                    return Failure(
                        SavePackageDocumentMigrationStatus.InvalidOutput,
                        SavePackageDocumentMigrationPlanStatus.Succeeded,
                        SavePackageDocumentMigrationDiagnosticCodes.InvalidOutput,
                        "Chronicle package-document migration step returned invalid UTF-8 text. " +
                        exception.Message);
                }

                if (outputBytes >
                    SavePackageDocumentVersionProbe.MaximumSerializedDocumentBytes)
                {
                    return Failure(
                        SavePackageDocumentMigrationStatus.InvalidOutput,
                        SavePackageDocumentMigrationPlanStatus.Succeeded,
                        SavePackageDocumentMigrationDiagnosticCodes.InvalidOutput,
                        "Chronicle package-document migration step returned serialized output beyond the bounded in-memory size.");
                }

                SavePackageDocumentVersionProbeResult outputProbe =
                    SavePackageDocumentVersionProbe.Probe(
                        output);

                if (!outputProbe.Succeeded ||
                    !string.Equals(
                        outputProbe.DocumentKind,
                        documentKind,
                        StringComparison.Ordinal) ||
                    outputProbe.Version !=
                        plannedStep.TargetVersion)
                {
                    return Failure(
                        SavePackageDocumentMigrationStatus.InvalidOutput,
                        SavePackageDocumentMigrationPlanStatus.Succeeded,
                        SavePackageDocumentMigrationDiagnosticCodes.InvalidOutput,
                        "Chronicle package-document migration step output changed document kind, declared the wrong target version, or was malformed.");
                }

                provenance.Add(
                    new SavePackageDocumentMigrationProvenanceEntry(
                        plannedStep.StepId,
                        plannedStep.DocumentKind,
                        plannedStep.SourceVersion,
                        plannedStep.TargetVersion,
                        i + 1,
                        steps.Count));

                cursor =
                    plannedStep.TargetVersion;
                currentSerialized =
                    output;
            }

            if (cursor != currentVersion)
            {
                return Failure(
                    SavePackageDocumentMigrationStatus.InvalidOutput,
                    SavePackageDocumentMigrationPlanStatus.InvalidChain,
                    SavePackageDocumentMigrationDiagnosticCodes.InvalidOutput,
                    "Chronicle package-document migration ended before the exact current format.");
            }

            return new SavePackageDocumentMigrationResult(
                SavePackageDocumentMigrationStatus.Succeeded,
                currentSerialized,
                provenance.ToArray(),
                SavePackageDocumentMigrationPlanStatus.Succeeded,
                string.Empty,
                "Chronicle package-document migration completed in memory without source mutation.");
        }

        private static SavePackageDocumentMigrationResult Failure(
            SavePackageDocumentMigrationStatus status,
            SavePackageDocumentMigrationPlanStatus planStatus,
            string diagnosticCode,
            string message) =>
            new SavePackageDocumentMigrationResult(
                status,
                null,
                Array.Empty<SavePackageDocumentMigrationProvenanceEntry>(),
                planStatus,
                diagnosticCode,
                message);
    }
}
