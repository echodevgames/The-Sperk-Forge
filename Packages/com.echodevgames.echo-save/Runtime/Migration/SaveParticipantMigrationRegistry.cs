using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Runtime-memory-only registry for explicit participant migration edges.
    ///
    /// Chronicle owns no hardcoded migration catalog. Future systems register
    /// the same public migration-step contract.
    /// </summary>
    internal sealed class SaveParticipantMigrationRegistry
    {
        private readonly struct EdgeKey :
            IEquatable<EdgeKey>
        {
            internal EdgeKey(
                SaveParticipantId participantId,
                int fromSchemaVersion)
            {
                ParticipantId =
                    participantId;

                FromSchemaVersion =
                    fromSchemaVersion;
            }

            internal SaveParticipantId ParticipantId { get; }

            internal int FromSchemaVersion { get; }

            public bool Equals(
                EdgeKey other) =>
                ParticipantId ==
                    other.ParticipantId &&
                FromSchemaVersion ==
                    other.FromSchemaVersion;

            public override bool Equals(
                object obj) =>
                obj is EdgeKey other &&
                Equals(
                    other);

            public override int GetHashCode() =>
                HashCode.Combine(
                    ParticipantId,
                    FromSchemaVersion);
        }

        private sealed class Entry
        {
            internal Entry(
                ISaveParticipantMigrationStep step,
                SaveParticipantMigrationDescriptor descriptor,
                long ownershipToken)
            {
                Step =
                    step;

                Descriptor =
                    descriptor;

                OwnershipToken =
                    ownershipToken;
            }

            internal ISaveParticipantMigrationStep Step { get; }

            internal SaveParticipantMigrationDescriptor Descriptor { get; }

            internal long OwnershipToken { get; }
        }

        private readonly
            Dictionary<SaveParticipantMigrationId, Entry>
                idEntries =
                    new Dictionary<
                        SaveParticipantMigrationId,
                        Entry>();

        private readonly
            Dictionary<EdgeKey, Entry>
                edgeEntries =
                    new Dictionary<EdgeKey, Entry>();

        private long nextOwnershipToken;

        internal int Count =>
            idEntries.Count;

        internal int ResolveEdgeCalls { get; private set; }

        internal SaveParticipantMigrationRegistrationResult
            Register(
                ISaveParticipantMigrationStep step)
        {
            if (!TryValidateStep(
                    step,
                    out SaveParticipantMigrationDescriptor descriptor,
                    out string diagnosticCode,
                    out string validationMessage))
            {
                return Failure(
                    SaveParticipantMigrationRegistrationStatus.InvalidStep,
                    diagnosticCode,
                    validationMessage);
            }

            if (idEntries.ContainsKey(
                    descriptor.Id))
            {
                return Failure(
                    SaveParticipantMigrationRegistrationStatus.DuplicateId,
                    EchoSaveDiagnosticCodes
                        .ParticipantMigrationDuplicateId,
                    "A Chronicle participant migration with this stable ID is already registered.");
            }

            EdgeKey edge =
                new EdgeKey(
                    descriptor.ParticipantId,
                    descriptor.FromSchemaVersion);

            if (edgeEntries.ContainsKey(
                    edge))
            {
                return Failure(
                    SaveParticipantMigrationRegistrationStatus.DuplicateEdge,
                    EchoSaveDiagnosticCodes
                        .ParticipantMigrationDuplicateEdge,
                    "A Chronicle participant migration already owns this canonical participant/from-version edge.");
            }

            if (nextOwnershipToken ==
                long.MaxValue)
            {
                return Failure(
                    SaveParticipantMigrationRegistrationStatus.InvalidStep,
                    EchoSaveDiagnosticCodes
                        .ParticipantMigrationInvalidStep,
                    "The Chronicle participant migration registry ownership-token space is exhausted.");
            }

            long token =
                ++nextOwnershipToken;

            Entry entry =
                new Entry(
                    step,
                    descriptor,
                    token);

            idEntries.Add(
                descriptor.Id,
                entry);

            edgeEntries.Add(
                edge,
                entry);

            return new SaveParticipantMigrationRegistrationResult(
                SaveParticipantMigrationRegistrationStatus.Succeeded,
                new SaveParticipantMigrationRegistration(
                    this,
                    descriptor.Id,
                    token),
                string.Empty,
                "The Chronicle participant migration step registered successfully.");
        }

        internal SaveParticipantMigrationPlanResult
            TryBuildPlan(
                SaveParticipantId canonicalParticipantId,
                int sourceSchemaVersion,
                int targetSchemaVersion,
                int maxSteps,
                out SaveParticipantMigrationPlan plan)
        {
            plan =
                null;

            if (!SaveParticipantId.TryParse(
                    canonicalParticipantId.Value,
                    out SaveParticipantId validatedParticipantId) ||
                sourceSchemaVersion <= 0 ||
                targetSchemaVersion <= 0 ||
                maxSteps <= 0)
            {
                return new SaveParticipantMigrationPlanResult(
                    SaveParticipantMigrationPlanStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes
                        .ParticipantMigrationInvalidRequest,
                    "Chronicle migration planning requires one valid canonical participant ID, positive schema versions, and a positive step bound.");
            }

            if (sourceSchemaVersion >
                targetSchemaVersion)
            {
                return new SaveParticipantMigrationPlanResult(
                    SaveParticipantMigrationPlanStatus.NewerSchemaUnsupported,
                    EchoSaveDiagnosticCodes
                        .ParticipantPreparationNewerSchema,
                    "Chronicle participant migration planning does not support downgrade paths.");
            }

            int requiredSteps =
                targetSchemaVersion -
                sourceSchemaVersion;

            if (requiredSteps >
                maxSteps)
            {
                return new SaveParticipantMigrationPlanResult(
                    SaveParticipantMigrationPlanStatus.StepLimitExceeded,
                    EchoSaveDiagnosticCodes
                        .ParticipantMigrationStepLimitExceeded,
                    "The Chronicle participant migration chain exceeds the configured step bound.");
            }

            if (requiredSteps == 0)
            {
                plan =
                    new SaveParticipantMigrationPlan(
                        validatedParticipantId,
                        sourceSchemaVersion,
                        targetSchemaVersion,
                        Array.Empty<
                            SaveParticipantMigrationPlanStep>());

                return SaveParticipantMigrationPlanResult.Success(
                    "The Chronicle participant payload already uses the current schema.");
            }

            List<SaveParticipantMigrationPlanStep>
                steps =
                    new List<SaveParticipantMigrationPlanStep>(
                        requiredSteps);

            for (int version =
                    sourceSchemaVersion;
                 version <
                    targetSchemaVersion;
                 version++)
            {
                ResolveEdgeCalls++;

                EdgeKey edge =
                    new EdgeKey(
                        validatedParticipantId,
                        version);

                if (!edgeEntries.TryGetValue(
                        edge,
                        out Entry entry))
                {
                    return new SaveParticipantMigrationPlanResult(
                        SaveParticipantMigrationPlanStatus.MissingEdge,
                        EchoSaveDiagnosticCodes
                            .ParticipantMigrationChainMissing,
                        $"Chronicle participant migration chain is missing edge {version} -> {version + 1} for '{validatedParticipantId.Value}'.");
                }

                steps.Add(
                    new SaveParticipantMigrationPlanStep(
                        entry.Step,
                        entry.Descriptor.Id,
                        entry.Descriptor.ParticipantId,
                        entry.Descriptor.FromSchemaVersion,
                        entry.Descriptor.ToSchemaVersion,
                        entry.OwnershipToken));
            }

            plan =
                new SaveParticipantMigrationPlan(
                    validatedParticipantId,
                    sourceSchemaVersion,
                    targetSchemaVersion,
                    steps.ToArray());

            return SaveParticipantMigrationPlanResult.Success(
                "The Chronicle participant migration chain is complete and contiguous.");
        }

        internal SaveParticipantMigrationRegistrySnapshot
            GetSnapshot()
        {
            List<SaveParticipantMigrationDescriptor>
                descriptors =
                    new List<SaveParticipantMigrationDescriptor>(
                        idEntries.Count);

            foreach (Entry entry in
                idEntries.Values)
            {
                descriptors.Add(
                    entry.Descriptor);
            }

            descriptors.Sort(
                CompareDescriptors);

            return new SaveParticipantMigrationRegistrySnapshot(
                descriptors.ToArray());
        }

        internal void Clear()
        {
            idEntries.Clear();
            edgeEntries.Clear();
        }

        internal bool Owns(
            SaveParticipantMigrationId migrationId,
            long ownershipToken)
        {
            return
                SaveParticipantMigrationId.TryParse(
                    migrationId.Value,
                    out SaveParticipantMigrationId validated) &&
                idEntries.TryGetValue(
                    validated,
                    out Entry entry) &&
                entry.OwnershipToken ==
                    ownershipToken;
        }

        internal bool Owns(
            SaveParticipantMigrationPlanStep plannedStep)
        {
            EdgeKey edge =
                new EdgeKey(
                    plannedStep.ParticipantId,
                    plannedStep.FromSchemaVersion);

            return
                idEntries.TryGetValue(
                    plannedStep.MigrationId,
                    out Entry idEntry) &&
                edgeEntries.TryGetValue(
                    edge,
                    out Entry edgeEntry) &&
                ReferenceEquals(
                    idEntry,
                    edgeEntry) &&
                idEntry.OwnershipToken ==
                    plannedStep.OwnershipToken &&
                ReferenceEquals(
                    idEntry.Step,
                    plannedStep.Step) &&
                idEntry.Descriptor.ToSchemaVersion ==
                    plannedStep.ToSchemaVersion;
        }

        internal void Release(
            SaveParticipantMigrationId migrationId,
            long ownershipToken)
        {
            if (!idEntries.TryGetValue(
                    migrationId,
                    out Entry entry) ||
                entry.OwnershipToken !=
                    ownershipToken)
            {
                return;
            }

            idEntries.Remove(
                migrationId);

            EdgeKey edge =
                new EdgeKey(
                    entry.Descriptor.ParticipantId,
                    entry.Descriptor.FromSchemaVersion);

            if (edgeEntries.TryGetValue(
                    edge,
                    out Entry edgeEntry) &&
                ReferenceEquals(
                    edgeEntry,
                    entry))
            {
                edgeEntries.Remove(
                    edge);
            }
        }

        private static bool TryValidateStep(
            ISaveParticipantMigrationStep step,
            out SaveParticipantMigrationDescriptor descriptor,
            out string diagnosticCode,
            out string message)
        {
            descriptor =
                default;

            diagnosticCode =
                EchoSaveDiagnosticCodes
                    .ParticipantMigrationInvalidStep;

            message =
                "The Chronicle participant migration step is invalid.";

            if (step == null)
            {
                return false;
            }

            SaveParticipantMigrationId migrationId;
            SaveParticipantId participantId;
            int fromSchemaVersion;
            int toSchemaVersion;

            try
            {
                migrationId =
                    step.Id;

                participantId =
                    step.ParticipantId;

                fromSchemaVersion =
                    step.FromSchemaVersion;

                toSchemaVersion =
                    step.ToSchemaVersion;
            }
            catch (Exception exception)
            {
                message =
                    $"The Chronicle participant migration descriptor could not be read. {exception.GetType().Name}: {exception.Message}";

                return false;
            }

            if (!SaveParticipantMigrationId.TryParse(
                    migrationId.Value,
                    out SaveParticipantMigrationId validatedMigrationId) ||
                !SaveParticipantId.TryParse(
                    participantId.Value,
                    out SaveParticipantId validatedParticipantId) ||
                fromSchemaVersion <= 0 ||
                toSchemaVersion !=
                    fromSchemaVersion + 1)
            {
                message =
                    "Chronicle participant migration steps require valid IDs and exactly one contiguous positive schema edge.";

                return false;
            }

            descriptor =
                new SaveParticipantMigrationDescriptor(
                    validatedMigrationId,
                    validatedParticipantId,
                    fromSchemaVersion,
                    toSchemaVersion);

            diagnosticCode =
                string.Empty;

            message =
                string.Empty;

            return true;
        }

        private static int CompareDescriptors(
            SaveParticipantMigrationDescriptor left,
            SaveParticipantMigrationDescriptor right)
        {
            int participantComparison =
                left.ParticipantId.CompareTo(
                    right.ParticipantId);

            if (participantComparison != 0)
            {
                return participantComparison;
            }

            int versionComparison =
                left.FromSchemaVersion.CompareTo(
                    right.FromSchemaVersion);

            if (versionComparison != 0)
            {
                return versionComparison;
            }

            return left.Id.CompareTo(
                right.Id);
        }

        private static SaveParticipantMigrationRegistrationResult
            Failure(
                SaveParticipantMigrationRegistrationStatus status,
                string diagnosticCode,
                string message) =>
            new SaveParticipantMigrationRegistrationResult(
                status,
                null,
                diagnosticCode,
                message);
    }
}
