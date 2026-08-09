
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// M3-02 in-memory participant capture coordinator.
    ///
    /// This coordinator deliberately has no storage/publication dependency.
    /// It turns registered participants into verified package transport entries
    /// and stops before any generation/head mutation.
    /// </summary>
    internal sealed class SaveParticipantCaptureCoordinator
    {
        private readonly
            SaveSerializerRegistry serializerRegistry;

        private readonly
            IIntegrityProvider integrityProvider;

        internal SaveParticipantCaptureCoordinator(
            SaveSerializerRegistry serializerRegistry,
            IIntegrityProvider integrityProvider)
        {
            this.serializerRegistry =
                serializerRegistry;
            this.integrityProvider =
                integrityProvider;
        }

        internal SaveParticipantCaptureBatchResult
            Capture(
                SaveParticipantRegistry registry)
        {
            if (registry == null ||
                serializerRegistry == null ||
                integrityProvider == null)
            {
                return Failure(
                    SaveParticipantCaptureBatchStatus.InvalidRequest,
                    default,
                    EchoSaveDiagnosticCodes
                        .ParticipantCaptureInvalidRequest,
                    "Chronicle detached capture requires a participant registry, serializer registry, and integrity provider.");
            }

            SaveParticipantRegistrySnapshot snapshot =
                registry.GetSnapshot();

            ParticipantSource[] sources =
                new ParticipantSource[
                    snapshot.Count];

            for (int i = 0;
                 i < snapshot.Count;
                 i++)
            {
                SaveParticipantDescriptor descriptor =
                    snapshot.Participants[i];

                if (!registry.TryResolve(
                        descriptor.Id,
                        out ISaveParticipant participant) ||
                    participant == null)
                {
                    return Failure(
                        SaveParticipantCaptureBatchStatus.ParticipantUnavailable,
                        descriptor.Id,
                        EchoSaveDiagnosticCodes
                            .ParticipantCaptureRegistryChanged,
                        "A participant from the Chronicle capture snapshot is no longer resolvable.");
                }

                sources[i] =
                    new ParticipantSource(
                        participant,
                        descriptor);
            }

            List<SavePayloadEntry>
                payloadEntries =
                    new List<SavePayloadEntry>(
                        sources.Length);

            List<SavePayloadInventoryEntry>
                inventoryEntries =
                    new List<SavePayloadInventoryEntry>(
                        sources.Length);

            long totalPayloadBytes =
                0L;

            for (int i = 0;
                 i < sources.Length;
                 i++)
            {
                ParticipantSource source =
                    sources[i];

                SaveParticipantCaptureBatchResult
                    participantResult =
                        CaptureParticipant(
                            source,
                            out SavePayloadEntry payloadEntry,
                            out SavePayloadInventoryEntry inventoryEntry);

                if (!participantResult.Succeeded)
                {
                    return participantResult;
                }

                try
                {
                    checked
                    {
                        totalPayloadBytes +=
                            payloadEntry.byteLength;
                    }
                }
                catch (OverflowException)
                {
                    return Failure(
                        SaveParticipantCaptureBatchStatus.SerializationFailed,
                        source.Descriptor.Id,
                        EchoSaveDiagnosticCodes
                            .ParticipantCaptureSerializationFailed,
                        "The Chronicle participant capture batch byte total exceeded the supported range.");
                }

                payloadEntries.Add(
                    payloadEntry);

                inventoryEntries.Add(
                    inventoryEntry);
            }

            return SaveParticipantCaptureBatchResult.Success(
                payloadEntries.ToArray(),
                inventoryEntries.ToArray(),
                totalPayloadBytes);
        }

        private SaveParticipantCaptureBatchResult
            CaptureParticipant(
                ParticipantSource source,
                out SavePayloadEntry payloadEntry,
                out SavePayloadInventoryEntry inventoryEntry)
        {
            payloadEntry =
                null;
            inventoryEntry =
                null;

            if (!(source.Participant is
                ISaveTypedParticipant typedParticipant))
            {
                return Failure(
                    SaveParticipantCaptureBatchStatus.DetachedStateInvalid,
                    source.Descriptor.Id,
                    EchoSaveDiagnosticCodes
                        .ParticipantCaptureTypeUnavailable,
                    "The Chronicle participant does not declare runtime detached DTO type authority.");
            }

            Type detachedType =
                typedParticipant
                    .DetachedStateType;

            if (!IsUsableDetachedType(
                    detachedType))
            {
                return Failure(
                    SaveParticipantCaptureBatchStatus.DetachedStateInvalid,
                    source.Descriptor.Id,
                    EchoSaveDiagnosticCodes
                        .ParticipantCaptureTypeUnavailable,
                    "The Chronicle participant declared an unusable detached DTO type.");
            }

            SaveParticipantCaptureResult capture;

            try
            {
                capture =
                    source.Participant
                        .Capture();
            }
            catch (Exception exception)
            {
                return Failure(
                    SaveParticipantCaptureBatchStatus.CaptureFailed,
                    source.Descriptor.Id,
                    EchoSaveDiagnosticCodes
                        .ParticipantCaptureFailed,
                    $"Chronicle participant capture threw {exception.GetType().Name}: {exception.Message}");
            }

            if (!capture.Succeeded)
            {
                return Failure(
                    SaveParticipantCaptureBatchStatus.CaptureFailed,
                    source.Descriptor.Id,
                    string.IsNullOrEmpty(
                        capture.DiagnosticCode)
                        ? EchoSaveDiagnosticCodes
                            .ParticipantCaptureFailed
                        : capture.DiagnosticCode,
                    string.IsNullOrEmpty(
                        capture.Message)
                        ? "The Chronicle participant capture failed."
                        : capture.Message);
            }

            object detachedState =
                capture.DetachedState;

            if (detachedState == null ||
                !detachedType.IsInstanceOfType(
                    detachedState) ||
                detachedState is UnityEngine.Object)
            {
                return Failure(
                    SaveParticipantCaptureBatchStatus.DetachedStateInvalid,
                    source.Descriptor.Id,
                    EchoSaveDiagnosticCodes
                        .ParticipantCaptureTypeMismatch,
                    "The Chronicle participant returned null, a live Unity object, or detached state incompatible with its trusted runtime DTO type.");
            }

            SaveSerializerId requestedSerializer =
                source.Descriptor
                    .SerializerId;

            if (string.IsNullOrEmpty(
                    requestedSerializer.Value))
            {
                requestedSerializer =
                    new SaveSerializerId(
                        UnityJsonSaveSerializer
                            .StableId);
            }

            SaveSerializerResult resolved =
                serializerRegistry.TryResolve(
                    requestedSerializer,
                    out ISaveSerializer serializer);

            if (!resolved.Succeeded ||
                serializer == null ||
                !(serializer is
                    IRuntimeTypeSaveSerializer runtimeSerializer))
            {
                return Failure(
                    SaveParticipantCaptureBatchStatus.SerializerUnavailable,
                    source.Descriptor.Id,
                    EchoSaveDiagnosticCodes
                        .ParticipantCaptureSerializerUnavailable,
                    !resolved.Succeeded
                        ? resolved.Message
                        : "The selected Chronicle serializer does not support trusted runtime DTO type routing.");
            }

            SaveSerializerResult serialized =
                runtimeSerializer.Serialize(
                    detachedState,
                    detachedType,
                    out string serializedPayload);

            if (!serialized.Succeeded)
            {
                return Failure(
                    SaveParticipantCaptureBatchStatus.SerializationFailed,
                    source.Descriptor.Id,
                    string.IsNullOrEmpty(
                        serialized.DiagnosticCode)
                        ? EchoSaveDiagnosticCodes
                            .ParticipantCaptureSerializationFailed
                        : serialized.DiagnosticCode,
                    serialized.Message);
            }

            byte[] payloadBytes =
                Encoding.UTF8.GetBytes(
                    serializedPayload);

            SaveIntegrityResult integrity =
                integrityProvider.Calculate(
                    payloadBytes,
                    out string checksum);

            if (!integrity.Succeeded)
            {
                return Failure(
                    SaveParticipantCaptureBatchStatus.IntegrityFailed,
                    source.Descriptor.Id,
                    string.IsNullOrEmpty(
                        integrity.DiagnosticCode)
                        ? EchoSaveDiagnosticCodes
                            .ParticipantCaptureIntegrityFailed
                        : integrity.DiagnosticCode,
                    integrity.Message);
            }

            bool required =
                source.Descriptor.Criticality ==
                SaveParticipantCriticality.Required;

            payloadEntry =
                new SavePayloadEntry
                {
                    participantId =
                        source.Descriptor
                            .Id.Value,
                    participantSchemaVersion =
                        source.Descriptor
                            .CurrentSchemaVersion,
                    serializerId =
                        serializer.Id.Value,
                    required =
                        required,
                    serializedPayload =
                        serializedPayload,
                    byteProviderReference =
                        string.Empty,
                    byteLength =
                        payloadBytes.LongLength,
                    checksum =
                        checksum,
                    flags =
                        0
                };

            inventoryEntry =
                new SavePayloadInventoryEntry
                {
                    participantId =
                        payloadEntry.participantId,
                    participantSchemaVersion =
                        payloadEntry
                            .participantSchemaVersion,
                    serializerId =
                        payloadEntry.serializerId,
                    required =
                        payloadEntry.required,
                    byteLength =
                        payloadEntry.byteLength,
                    checksum =
                        payloadEntry.checksum,
                    flags =
                        payloadEntry.flags
                };

            return SaveParticipantCaptureBatchResult.Success(
                new[]
                {
                    payloadEntry
                },
                new[]
                {
                    inventoryEntry
                },
                payloadEntry.byteLength);
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

        private static SaveParticipantCaptureBatchResult
            Failure(
                SaveParticipantCaptureBatchStatus status,
                SaveParticipantId participantId,
                string diagnosticCode,
                string message) =>
            SaveParticipantCaptureBatchResult.Failure(
                status,
                participantId,
                diagnosticCode,
                message);

        private readonly struct ParticipantSource
        {
            internal ParticipantSource(
                ISaveParticipant participant,
                SaveParticipantDescriptor descriptor)
            {
                Participant =
                    participant;
                Descriptor =
                    descriptor;
            }

            internal ISaveParticipant Participant { get; }

            internal SaveParticipantDescriptor Descriptor
            {
                get;
            }
        }
    }
}
