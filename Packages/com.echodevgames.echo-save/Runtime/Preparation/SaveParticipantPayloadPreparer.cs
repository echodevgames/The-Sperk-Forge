using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// M3-06 side-effect-free participant payload preparation coordinator.
    ///
    /// Durable payload entries are accepted only from one fully validated
    /// current-generation snapshot. Unknown entries are skipped before any
    /// serializer lookup. Known current-schema entries are deserialized using
    /// runtime DTO Type authority supplied by live participant registration.
    /// </summary>
    internal sealed class SaveParticipantPayloadPreparer
    {
        private readonly SaveParticipantRegistry participantRegistry;
        private readonly SaveSerializerRegistry serializerRegistry;

        internal SaveParticipantPayloadPreparer(
            SaveParticipantRegistry participantRegistry,
            SaveSerializerRegistry serializerRegistry)
        {
            this.participantRegistry =
                participantRegistry;

            this.serializerRegistry =
                serializerRegistry;
        }

        internal SaveParticipantPreparationResult Prepare(
            SaveValidatedParticipantSnapshot snapshot)
        {
            if (snapshot == null ||
                participantRegistry == null ||
                serializerRegistry == null ||
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
                    "Chronicle participant preparation requires a fully validated source snapshot and active participant/serializer registries.");
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
                    // Unknown payloads stay opaque. Do not inspect serializer ID
                    // or serialized payload contents for unowned entries.
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

                if (payloadEntry.participantSchemaVersion <
                    descriptor.CurrentSchemaVersion)
                {
                    return Failure(
                        SaveParticipantPreparationStatus.MigrationRequired,
                        persistedId,
                        canonicalId,
                        EchoSaveDiagnosticCodes
                            .ParticipantPreparationMigrationRequired,
                        "The persisted Chronicle participant payload is older than the current participant schema and requires an explicit migration chain.");
                }

                if (payloadEntry.participantSchemaVersion >
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

                SaveSerializerId serializerId;

                try
                {
                    serializerId =
                        new SaveSerializerId(
                            payloadEntry.serializerId);
                }
                catch (ArgumentException)
                {
                    return Failure(
                        SaveParticipantPreparationStatus.SerializerUnavailable,
                        persistedId,
                        canonicalId,
                        EchoSaveDiagnosticCodes
                            .ParticipantPreparationSerializerUnavailable,
                        "The persisted Chronicle participant serializer provider ID is invalid.");
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
                            : "The persisted Chronicle serializer provider does not support trusted runtime-Type deserialization.");
                }

                SaveSerializerResult deserialize;

                object detachedState;

                try
                {
                    deserialize =
                        runtimeSerializer.Deserialize(
                            payloadEntry.serializedPayload,
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
                        descriptor.CurrentSchemaVersion,
                        serializerId,
                        detachedType,
                        detachedState));
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
