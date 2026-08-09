
using System;
using System.Collections.Generic;
using System.Text;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Side-effect-free M3-03 validation for participant-bearing publication.
    ///
    /// This validator deliberately rechecks the M3-02 capture batch at the
    /// durable-publication boundary. Validation must complete before the first
    /// candidate storage mutation.
    /// </summary>
    internal static class SaveParticipantPublicationBatchValidator
    {
        internal static SaveDocumentValidationResult
            ValidateCaptureBatch(
                SaveParticipantCaptureBatchResult batch,
                IIntegrityProvider integrityProvider,
                out SavePayloadEntry[] payloadEntries,
                out SavePayloadInventoryEntry[] inventoryEntries)
        {
            payloadEntries =
                Array.Empty<SavePayloadEntry>();

            inventoryEntries =
                Array.Empty<SavePayloadInventoryEntry>();

            if (batch == null ||
                !batch.Succeeded ||
                integrityProvider == null)
            {
                return Invalid(
                    "Chronicle participant publication requires one successful capture batch and one integrity provider.");
            }

            IReadOnlyList<SavePayloadEntry>
                capturedPayload =
                    batch.PayloadEntries;

            IReadOnlyList<SavePayloadInventoryEntry>
                capturedInventory =
                    batch.InventoryEntries;

            if (capturedPayload == null ||
                capturedInventory == null ||
                capturedPayload.Count !=
                    capturedInventory.Count ||
                capturedPayload.Count !=
                    batch.Count ||
                capturedPayload.Count == 0)
            {
                return Invalid(
                    "Chronicle participant publication requires one non-empty capture batch with matching payload and inventory counts.");
            }

            payloadEntries =
                ClonePayloadEntries(
                    capturedPayload);

            inventoryEntries =
                CloneInventoryEntries(
                    capturedInventory);

            SaveDocumentValidationResult validation =
                ValidateEntries(
                    payloadEntries,
                    inventoryEntries,
                    integrityProvider,
                    false,
                    out long totalPayloadBytes);

            if (!validation.Succeeded)
            {
                payloadEntries =
                    Array.Empty<SavePayloadEntry>();

                inventoryEntries =
                    Array.Empty<SavePayloadInventoryEntry>();

                return validation;
            }

            if (totalPayloadBytes !=
                batch.TotalPayloadBytes)
            {
                payloadEntries =
                    Array.Empty<SavePayloadEntry>();

                inventoryEntries =
                    Array.Empty<SavePayloadInventoryEntry>();

                return Invalid(
                    "Chronicle participant capture-batch total bytes do not match the validated participant entries.");
            }

            return SaveDocumentValidationResult.Success(
                "The Chronicle participant capture batch is safe to cross the publication boundary.");
        }

        internal static SaveDocumentValidationResult
            ValidateStoredEntries(
                SavePayloadEntry[] payloadEntries,
                SavePayloadInventoryEntry[] inventoryEntries,
                IIntegrityProvider integrityProvider) =>
            ValidateEntries(
                payloadEntries,
                inventoryEntries,
                integrityProvider,
                true,
                out _);

        private static SaveDocumentValidationResult
            ValidateEntries(
                SavePayloadEntry[] payloadEntries,
                SavePayloadInventoryEntry[] inventoryEntries,
                IIntegrityProvider integrityProvider,
                bool allowEmpty,
                out long totalPayloadBytes)
        {
            totalPayloadBytes =
                0L;

            if (payloadEntries == null ||
                inventoryEntries == null ||
                integrityProvider == null ||
                payloadEntries.Length !=
                    inventoryEntries.Length)
            {
                return Invalid(
                    "Chronicle participant payload and inventory arrays must exist and have matching counts.");
            }

            if (!allowEmpty &&
                payloadEntries.Length == 0)
            {
                return Invalid(
                    "Participant-backed Chronicle publication requires at least one participant entry.");
            }

            HashSet<string> identities =
                new HashSet<string>(
                    StringComparer.Ordinal);

            string previousParticipantId =
                null;

            for (int i = 0;
                 i < payloadEntries.Length;
                 i++)
            {
                SavePayloadEntry payload =
                    payloadEntries[i];

                SavePayloadInventoryEntry inventory =
                    inventoryEntries[i];

                if (payload == null ||
                    inventory == null)
                {
                    return Invalid(
                        $"Chronicle participant publication entry {i} is null.");
                }

                if (!SaveParticipantId.TryParse(
                        payload.participantId,
                        out SaveParticipantId participantId))
                {
                    return Invalid(
                        $"Chronicle participant publication entry {i} contains an invalid participant ID.");
                }

                if (!identities.Add(
                        participantId.Value))
                {
                    return Invalid(
                        $"Chronicle participant publication contains duplicate participant ID '{participantId.Value}'.");
                }

                if (previousParticipantId != null &&
                    string.Compare(
                        previousParticipantId,
                        participantId.Value,
                        StringComparison.Ordinal) >= 0)
                {
                    return Invalid(
                        "Chronicle participant publication entries must remain in strict canonical participant-ID order.");
                }

                previousParticipantId =
                    participantId.Value;

                if (payload.participantSchemaVersion <= 0)
                {
                    return Invalid(
                        $"Chronicle participant '{participantId.Value}' has a non-positive schema version.");
                }

                if (!IsCanonicalSerializerId(
                        payload.serializerId))
                {
                    return Invalid(
                        $"Chronicle participant '{participantId.Value}' has an invalid or noncanonical serializer ID.");
                }

                if (payload.flags != 0)
                {
                    return Invalid(
                        $"Chronicle participant '{participantId.Value}' uses unsupported payload flags.");
                }

                if (!string.IsNullOrEmpty(
                        payload.byteProviderReference))
                {
                    return Invalid(
                        $"Chronicle participant '{participantId.Value}' uses an unsupported byte-provider reference.");
                }

                if (string.IsNullOrEmpty(
                        payload.serializedPayload) ||
                    payload.byteLength < 0)
                {
                    return Invalid(
                        $"Chronicle participant '{participantId.Value}' has invalid inline payload content or byte length.");
                }

                byte[] inlineBytes =
                    Encoding.UTF8.GetBytes(
                        payload.serializedPayload);

                if (inlineBytes.LongLength !=
                    payload.byteLength)
                {
                    return Invalid(
                        $"Chronicle participant '{participantId.Value}' inline payload byte length does not match its metadata.");
                }

                SaveIntegrityResult integrity =
                    integrityProvider.Verify(
                        inlineBytes,
                        payload.checksum);

                if (!integrity.Succeeded)
                {
                    return new SaveDocumentValidationResult(
                        SaveDocumentValidationStatus
                            .IntegrityMismatch,
                        string.IsNullOrEmpty(
                            integrity.DiagnosticCode)
                            ? EchoSaveDiagnosticCodes
                                .PublicationParticipantBatchInvalid
                            : integrity.DiagnosticCode,
                        $"Chronicle participant '{participantId.Value}' inline payload checksum is invalid. {integrity.Message}");
                }

                if (!string.Equals(
                        inventory.participantId,
                        payload.participantId,
                        StringComparison.Ordinal) ||
                    inventory.participantSchemaVersion !=
                        payload.participantSchemaVersion ||
                    !string.Equals(
                        inventory.serializerId,
                        payload.serializerId,
                        StringComparison.Ordinal) ||
                    inventory.required !=
                        payload.required ||
                    inventory.byteLength !=
                        payload.byteLength ||
                    !string.Equals(
                        inventory.checksum,
                        payload.checksum,
                        StringComparison.Ordinal) ||
                    inventory.flags !=
                        payload.flags)
                {
                    return new SaveDocumentValidationResult(
                        SaveDocumentValidationStatus
                            .InventoryMismatch,
                        EchoSaveDiagnosticCodes
                            .PublicationParticipantBatchInvalid,
                        $"Chronicle participant '{participantId.Value}' payload and manifest inventory metadata do not agree.");
                }

                try
                {
                    checked
                    {
                        totalPayloadBytes +=
                            payload.byteLength;
                    }
                }
                catch (OverflowException)
                {
                    return Invalid(
                        "Chronicle participant publication total inline payload bytes exceed the supported range.");
                }
            }

            return SaveDocumentValidationResult.Success(
                "Chronicle participant payload entries and manifest inventory are valid and agree.");
        }

        private static bool IsCanonicalSerializerId(
            string value)
        {
            if (string.IsNullOrEmpty(
                    value))
            {
                return false;
            }

            try
            {
                SaveSerializerId id =
                    new SaveSerializerId(
                        value);

                return string.Equals(
                    id.Value,
                    value,
                    StringComparison.Ordinal);
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private static SavePayloadEntry[]
            ClonePayloadEntries(
                IReadOnlyList<SavePayloadEntry> source)
        {
            SavePayloadEntry[] copy =
                new SavePayloadEntry[
                    source.Count];

            for (int i = 0;
                 i < source.Count;
                 i++)
            {
                SavePayloadEntry entry =
                    source[i];

                copy[i] =
                    entry == null
                        ? null
                        : new SavePayloadEntry
                        {
                            participantId =
                                entry.participantId,
                            participantSchemaVersion =
                                entry.participantSchemaVersion,
                            serializerId =
                                entry.serializerId,
                            required =
                                entry.required,
                            serializedPayload =
                                entry.serializedPayload,
                            byteProviderReference =
                                entry.byteProviderReference,
                            byteLength =
                                entry.byteLength,
                            checksum =
                                entry.checksum,
                            flags =
                                entry.flags
                        };
            }

            return copy;
        }

        private static SavePayloadInventoryEntry[]
            CloneInventoryEntries(
                IReadOnlyList<SavePayloadInventoryEntry>
                    source)
        {
            SavePayloadInventoryEntry[] copy =
                new SavePayloadInventoryEntry[
                    source.Count];

            for (int i = 0;
                 i < source.Count;
                 i++)
            {
                SavePayloadInventoryEntry entry =
                    source[i];

                copy[i] =
                    entry == null
                        ? null
                        : new SavePayloadInventoryEntry
                        {
                            participantId =
                                entry.participantId,
                            participantSchemaVersion =
                                entry.participantSchemaVersion,
                            serializerId =
                                entry.serializerId,
                            required =
                                entry.required,
                            byteLength =
                                entry.byteLength,
                            checksum =
                                entry.checksum,
                            flags =
                                entry.flags
                        };
            }

            return copy;
        }

        private static SaveDocumentValidationResult Invalid(
            string message) =>
            new SaveDocumentValidationResult(
                SaveDocumentValidationStatus.InvalidDocument,
                EchoSaveDiagnosticCodes
                    .PublicationParticipantBatchInvalid,
                message);
    }
}
