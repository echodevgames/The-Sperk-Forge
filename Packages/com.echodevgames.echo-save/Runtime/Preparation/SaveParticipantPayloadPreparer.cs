using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Side-effect-free participant payload preparation coordinator.
    ///
    /// M3-06 prepares validated current-version known participant payloads.
    /// M3-07 optionally migrates explicitly supported older known participant
    /// payloads through a complete contiguous in-memory chain before the same
    /// trusted current DTO deserialization path runs.
    ///
    /// Unknown payloads are skipped before migration or serializer lookup.
    /// No participant Capture/Apply call or storage mutation occurs here.
    /// </summary>
    internal sealed class SaveParticipantPayloadPreparer
    {
        internal const int DefaultMaxMigrationSteps =
            32;

        private readonly SaveParticipantRegistry participantRegistry;
        private readonly SaveSerializerRegistry serializerRegistry;
        private readonly SaveParticipantMigrationRegistry migrationRegistry;
        private readonly int maxMigrationSteps;

        internal SaveParticipantPayloadPreparer(
            SaveParticipantRegistry participantRegistry,
            SaveSerializerRegistry serializerRegistry)
            : this(
                participantRegistry,
                serializerRegistry,
                null,
                DefaultMaxMigrationSteps)
        {
        }

        internal SaveParticipantPayloadPreparer(
            SaveParticipantRegistry participantRegistry,
            SaveSerializerRegistry serializerRegistry,
            SaveParticipantMigrationRegistry migrationRegistry,
            int maxMigrationSteps =
                DefaultMaxMigrationSteps)
        {
            this.participantRegistry =
                participantRegistry;

            this.serializerRegistry =
                serializerRegistry;

            this.migrationRegistry =
                migrationRegistry;

            this.maxMigrationSteps =
                maxMigrationSteps;
        }

        internal SaveParticipantPreparationResult Prepare(
            SaveValidatedParticipantSnapshot snapshot)
        {
            if (snapshot == null ||
                participantRegistry == null ||
                serializerRegistry == null ||
                maxMigrationSteps <= 0 ||
                !SaveSlotId.TryParse(
                    snapshot.SourceSlotId.Value,
                    out SaveSlotId sourceSlot) ||
                !SaveGenerationId.TryParse(
                    snapshot.SourceGenerationId.Value,
                    out SaveGenerationId sourceGeneration))
            {
                return Failure(
                    SaveParticipantPreparationStatus.InvalidRequest,
                    default,
                    default,
                    EchoSaveDiagnosticCodes
                        .ParticipantPreparationInvalidRequest,
                    "Chronicle participant preparation requires a fully validated source snapshot, active participant/serializer registries, and a positive migration-step bound.");
            }

            IReadOnlyList<SavePayloadEntry>
                payloadEntries =
                    snapshot.Entries;

            List<SavePreparedParticipantEntry>
                prepared =
                    new List<SavePreparedParticipantEntry>();

            HashSet<SaveParticipantId>
                canonicalOwners =
                    new HashSet<SaveParticipantId>();

            for (int i = 0;
                 i < payloadEntries.Count;
                 i++)
            {
                SavePayloadEntry payloadEntry =
                    payloadEntries[i];

                if (payloadEntry == null ||
                    !SaveParticipantId.TryParse(
                        payloadEntry.participantId,
                        out SaveParticipantId persistedId))
                {
                    return Failure(
                        SaveParticipantPreparationStatus.InvalidRequest,
                        default,
                        default,
                        EchoSaveDiagnosticCodes
                            .ParticipantPreparationInvalidRequest,
                        "A validated Chronicle participant entry has an invalid persisted participant ID.");
                }

                if (!participantRegistry.TryResolve(
                        persistedId,
                        out ISaveParticipant participant))
                {
                    // Unknown payloads stay opaque. Do not inspect migration
                    // registry, serializer ID, or payload content.
                    continue;
                }

                if (participant == null ||
                    !participantRegistry.TryResolveDescriptor(
                        persistedId,
                        out SaveParticipantDescriptor descriptor))
                {
                    return Failure(
                        SaveParticipantPreparationStatus.ParticipantUnavailable,
                        persistedId,
                        default,
                        EchoSaveDiagnosticCodes
                            .ParticipantPreparationRegistryChanged,
                        "A Chronicle participant owner became unavailable during payload preparation.");
                }

                SaveParticipantId canonicalId =
                    descriptor.Id;

                if (!canonicalOwners.Add(
                        canonicalId))
                {
                    return Failure(
                        SaveParticipantPreparationStatus.DuplicateCanonicalOwner,
                        persistedId,
                        canonicalId,
                        EchoSaveDiagnosticCodes
                            .ParticipantPreparationDuplicateOwner,
                        "Multiple persisted Chronicle participant IDs resolve to the same current canonical owner.");
                }

                if (!(participant is
                    ISaveTypedParticipant typedParticipant))
                {
                    return Failure(
                        SaveParticipantPreparationStatus.RuntimeTypeUnavailable,
                        persistedId,
                        canonicalId,
                        EchoSaveDiagnosticCodes
                            .ParticipantPreparationTypeUnavailable,
                        "The current Chronicle participant owner does not declare trusted detached DTO Type authority.");
                }

                Type detachedType =
                    typedParticipant.DetachedStateType;

                if (!IsUsableDetachedType(
                        detachedType))
                {
                    return Failure(
                        SaveParticipantPreparationStatus.RuntimeTypeUnavailable,
                        persistedId,
                        canonicalId,
                        EchoSaveDiagnosticCodes
                            .ParticipantPreparationTypeUnavailable,
                        "The current Chronicle participant owner declared an unusable detached DTO Type.");
                }

                int storedSchemaVersion =
                    payloadEntry.participantSchemaVersion;

                if (storedSchemaVersion >
                    descriptor.CurrentSchemaVersion)
                {
                    return Failure(
                        SaveParticipantPreparationStatus.NewerSchemaUnsupported,
                        persistedId,
                        canonicalId,
                        EchoSaveDiagnosticCodes
                            .ParticipantPreparationNewerSchema,
                        "The persisted Chronicle participant payload uses a newer schema than the current participant supports.");
                }

                string serializedPayload =
                    payloadEntry.serializedPayload;

                string serializerIdText =
                    payloadEntry.serializerId;

                SaveParticipantMigrationProvenanceEntry[]
                    migrationProvenance =
                        Array.Empty<
                            SaveParticipantMigrationProvenanceEntry>();

                if (storedSchemaVersion <
                    descriptor.CurrentSchemaVersion)
                {
                    if (migrationRegistry == null)
                    {
                        return Failure(
                            SaveParticipantPreparationStatus.MigrationRequired,
                            persistedId,
                            canonicalId,
                            EchoSaveDiagnosticCodes
                                .ParticipantPreparationMigrationRequired,
                            "The persisted Chronicle participant payload is older than the current participant schema and requires an explicit migration chain.");
                    }

                    SaveParticipantMigrationPlanResult
                        planResult =
                            migrationRegistry.TryBuildPlan(
                                canonicalId,
                                storedSchemaVersion,
                                descriptor.CurrentSchemaVersion,
                                maxMigrationSteps,
                                out SaveParticipantMigrationPlan plan);

                    if (!planResult.Succeeded)
                    {
                        return Failure(
                            SaveParticipantPreparationStatus.MigrationChainUnavailable,
                            persistedId,
                            canonicalId,
                            planResult.DiagnosticCode,
                            planResult.Message);
                    }

                    SaveSerializerId initialSerializerId;

                    try
                    {
                        initialSerializerId =
                            new SaveSerializerId(
                                serializerIdText);
                    }
                    catch (ArgumentException)
                    {
                        return Failure(
                            SaveParticipantPreparationStatus.MigrationFailed,
                            persistedId,
                            canonicalId,
                            EchoSaveDiagnosticCodes
                                .ParticipantMigrationInvalidOutput,
                            "The persisted Chronicle participant serializer provider ID is invalid before migration.");
                    }

                    SaveParticipantMigrationExecutionResult
                        migration =
                            SaveParticipantMigrationExecutor
                                .Execute(
                                    migrationRegistry,
                                    plan,
                                    new SaveParticipantMigrationInput(
                                        persistedId,
                                        canonicalId,
                                        storedSchemaVersion,
                                        initialSerializerId,
                                        serializedPayload,
                                        payloadEntry.required,
                                        payloadEntry.flags));

                    if (!migration.Succeeded)
                    {
                        return Failure(
                            SaveParticipantPreparationStatus.MigrationFailed,
                            persistedId,
                            canonicalId,
                            migration.DiagnosticCode,
                            migration.Message);
                    }

                    serializedPayload =
                        migration.SerializedPayload;

                    serializerIdText =
                        migration.SerializerId.Value;

                    migrationProvenance =
                        migration.Provenance;
                }

                SaveSerializerId serializerId;

                try
                {
                    serializerId =
                        new SaveSerializerId(
                            serializerIdText);
                }
                catch (ArgumentException)
                {
                    return Failure(
                        SaveParticipantPreparationStatus.SerializerUnavailable,
                        persistedId,
                        canonicalId,
                        EchoSaveDiagnosticCodes
                            .ParticipantPreparationSerializerUnavailable,
                        "The prepared Chronicle participant serializer provider ID is invalid.");
                }

                SaveSerializerResult resolve =
                    serializerRegistry.TryResolve(
                        serializerId,
                        out ISaveSerializer serializer);

                if (!resolve.Succeeded ||
                    serializer == null ||
                    !(serializer is
                        IRuntimeTypeSaveSerializer runtimeSerializer))
                {
                    return Failure(
                        SaveParticipantPreparationStatus.SerializerUnavailable,
                        persistedId,
                        canonicalId,
                        EchoSaveDiagnosticCodes
                            .ParticipantPreparationSerializerUnavailable,
                        !resolve.Succeeded
                            ? resolve.Message
                            : "The prepared Chronicle serializer provider does not support trusted runtime-Type deserialization.");
                }

                SaveSerializerResult deserialize;

                object detachedState;

                try
                {
                    deserialize =
                        runtimeSerializer.Deserialize(
                            serializedPayload,
                            detachedType,
                            out detachedState);
                }
                catch (Exception exception)
                    when (exception is ArgumentException ||
                          exception is InvalidOperationException ||
                          exception is NotSupportedException)
                {
                    return Failure(
                        SaveParticipantPreparationStatus.DeserializationFailed,
                        persistedId,
                        canonicalId,
                        EchoSaveDiagnosticCodes
                            .ParticipantPreparationDeserializationFailed,
                        $"Chronicle participant deserialization threw {exception.GetType().Name}: {exception.Message}");
                }

                if (!deserialize.Succeeded)
                {
                    return Failure(
                        SaveParticipantPreparationStatus.DeserializationFailed,
                        persistedId,
                        canonicalId,
                        string.IsNullOrEmpty(
                            deserialize.DiagnosticCode)
                            ? EchoSaveDiagnosticCodes
                                .ParticipantPreparationDeserializationFailed
                            : deserialize.DiagnosticCode,
                        deserialize.Message);
                }

                if (detachedState == null ||
                    !detachedType.IsInstanceOfType(
                        detachedState) ||
                    detachedState is UnityEngine.Object)
                {
                    return Failure(
                        SaveParticipantPreparationStatus.DetachedStateInvalid,
                        persistedId,
                        canonicalId,
                        EchoSaveDiagnosticCodes
                            .ParticipantPreparationStateInvalid,
                        "Chronicle deserialization did not produce a detached state object compatible with the trusted live DTO Type.");
                }

                prepared.Add(
                    new SavePreparedParticipantEntry(
                        persistedId,
                        canonicalId,
                        storedSchemaVersion,
                        descriptor.CurrentSchemaVersion,
                        serializerId,
                        detachedType,
                        detachedState,
                        migrationProvenance));
            }

            prepared.Sort(
                ComparePreparedEntries);

            SavePreparedParticipantBatch batch =
                new SavePreparedParticipantBatch(
                    sourceSlot,
                    sourceGeneration,
                    prepared.ToArray());

            return SaveParticipantPreparationResult
                .Success(
                    batch);
        }

        private static bool IsUsableDetachedType(
            Type detachedType)
        {
            if (detachedType == null ||
                detachedType == typeof(void) ||
                detachedType.IsPointer ||
                detachedType.IsByRef ||
                detachedType.ContainsGenericParameters ||
                typeof(UnityEngine.Object)
                    .IsAssignableFrom(
                        detachedType))
            {
                return false;
            }

            return true;
        }

        private static int ComparePreparedEntries(
            SavePreparedParticipantEntry left,
            SavePreparedParticipantEntry right) =>
            left.CanonicalParticipantId
                .CompareTo(
                    right.CanonicalParticipantId);

        private static SaveParticipantPreparationResult Failure(
            SaveParticipantPreparationStatus status,
            SaveParticipantId persistedParticipantId,
            SaveParticipantId canonicalParticipantId,
            string diagnosticCode,
            string message) =>
            SaveParticipantPreparationResult.Failure(
                status,
                persistedParticipantId,
                canonicalParticipantId,
                diagnosticCode,
                message);
    }
}
