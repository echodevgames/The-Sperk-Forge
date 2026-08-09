
using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Side-effect-free agreement validation for pre-publication Chronicle
    /// commit documents.
    /// </summary>
    public static class SaveCommitDocumentValidator
    {
        public static SaveDocumentValidationResult
            ValidateManifestAndPayload(
                SaveManifest manifest,
                SavePayloadDocument payload,
                byte[] serializedPayloadBytes,
                IIntegrityProvider integrityProvider)
        {
            if (manifest == null ||
                payload == null ||
                serializedPayloadBytes == null ||
                integrityProvider == null)
            {
                return Invalid(
                    "Manifest, payload, detached payload bytes, and integrity provider are required.");
            }

            SaveSerializerResult manifestVersion =
                SavePackageDocumentValidator
                    .ValidateCurrent(
                        manifest);

            SaveSerializerResult payloadVersion =
                SavePackageDocumentValidator
                    .ValidateCurrent(
                        payload);

            if (!manifestVersion.Succeeded ||
                !payloadVersion.Succeeded)
            {
                return Invalid(
                    "Manifest or payload package-document version is unsupported.");
            }

            if (!SaveSlotId.TryParse(
                    manifest.slotId,
                    out SaveSlotId manifestSlot) ||
                !SaveSlotId.TryParse(
                    payload.slotId,
                    out SaveSlotId payloadSlot) ||
                !SaveGenerationId.TryParse(
                    manifest.generationId,
                    out SaveGenerationId manifestGeneration) ||
                !SaveGenerationId.TryParse(
                    payload.generationId,
                    out SaveGenerationId payloadGeneration))
            {
                return Invalid(
                    "Manifest or payload contains an invalid technical slot/generation identity.");
            }

            if (manifestSlot != payloadSlot ||
                manifestGeneration != payloadGeneration)
            {
                return new SaveDocumentValidationResult(
                    SaveDocumentValidationStatus
                        .IdentityMismatch,
                    EchoSaveDiagnosticCodes
                        .DocumentIdentityMismatch,
                    "Manifest and payload slot/generation identities do not agree.");
            }

            if (manifest.payloadByteLength !=
                serializedPayloadBytes.LongLength)
            {
                return new SaveDocumentValidationResult(
                    SaveDocumentValidationStatus
                        .PayloadLengthMismatch,
                    EchoSaveDiagnosticCodes
                        .DocumentPayloadLengthMismatch,
                    "Manifest payload byte length does not match the detached payload bytes.");
            }

            if (!string.Equals(
                    manifest.integrityAlgorithm,
                    integrityProvider.Id.Value,
                    StringComparison.Ordinal))
            {
                return new SaveDocumentValidationResult(
                    SaveDocumentValidationStatus
                        .UnsupportedIntegrityProvider,
                    EchoSaveDiagnosticCodes
                        .DocumentUnsupportedIntegrityProvider,
                    "Manifest integrity algorithm does not match the active integrity provider.");
            }

            SaveIntegrityResult integrity =
                integrityProvider.Verify(
                    serializedPayloadBytes,
                    manifest.payloadChecksum);

            if (!integrity.Succeeded)
            {
                return new SaveDocumentValidationResult(
                    SaveDocumentValidationStatus
                        .IntegrityMismatch,
                    integrity.DiagnosticCode,
                    integrity.Message);
            }

            SavePayloadInventoryEntry[] inventory =
                manifest.payloadEntries ??
                Array.Empty<SavePayloadInventoryEntry>();

            SavePayloadEntry[] entries =
                payload.entries ??
                Array.Empty<SavePayloadEntry>();

            if (inventory.Length !=
                entries.Length)
            {
                return InventoryMismatch(
                    "Manifest payload inventory count does not match the payload entry count.");
            }

            for (int i = 0;
                 i < entries.Length;
                 i++)
            {
                SavePayloadInventoryEntry descriptor =
                    inventory[i];

                SavePayloadEntry entry =
                    entries[i];

                if (descriptor == null ||
                    entry == null ||
                    !string.Equals(
                        descriptor.participantId,
                        entry.participantId,
                        StringComparison.Ordinal) ||
                    descriptor.participantSchemaVersion !=
                        entry.participantSchemaVersion ||
                    !string.Equals(
                        descriptor.serializerId,
                        entry.serializerId,
                        StringComparison.Ordinal) ||
                    descriptor.required !=
                        entry.required ||
                    descriptor.byteLength !=
                        entry.byteLength ||
                    !string.Equals(
                        descriptor.checksum,
                        entry.checksum,
                        StringComparison.Ordinal) ||
                    descriptor.flags !=
                        entry.flags)
                {
                    return InventoryMismatch(
                        $"Manifest payload inventory entry {i} does not match its payload transport entry.");
                }
            }

            return SaveDocumentValidationResult.Success(
                "Manifest, payload, detached bytes, and integrity metadata agree.");
        }

        public static SaveDocumentValidationResult
            ValidateHead(
                SaveHeadPointer head)
        {
            if (head == null)
            {
                return Invalid(
                    "A Chronicle head pointer is required.");
            }

            SaveSerializerResult version =
                SavePackageDocumentValidator
                    .ValidateCurrent(
                        head);

            if (!version.Succeeded)
            {
                return Invalid(
                    "The Chronicle head pointer version is unsupported.");
            }

            if (!SaveSlotId.TryParse(
                    head.slotId,
                    out _) ||
                !SaveGenerationId.TryParse(
                    head.currentGenerationId,
                    out _))
            {
                return new SaveDocumentValidationResult(
                    SaveDocumentValidationStatus
                        .InvalidDocument,
                    EchoSaveDiagnosticCodes
                        .DocumentInvalidHead,
                    "The Chronicle head pointer contains an invalid slot/current-generation identity.");
            }

            if (!string.IsNullOrEmpty(
                    head.previousGenerationId) &&
                !SaveGenerationId.TryParse(
                    head.previousGenerationId,
                    out _))
            {
                return new SaveDocumentValidationResult(
                    SaveDocumentValidationStatus
                        .InvalidDocument,
                    EchoSaveDiagnosticCodes
                        .DocumentInvalidHead,
                    "The Chronicle head pointer contains an invalid previous-generation identity.");
            }

            if (head.updateSequence < 0)
            {
                return new SaveDocumentValidationResult(
                    SaveDocumentValidationStatus
                        .InvalidDocument,
                    EchoSaveDiagnosticCodes
                        .DocumentInvalidHead,
                    "The Chronicle head update sequence cannot be negative.");
            }

            return SaveDocumentValidationResult.Success(
                "The Chronicle head pointer identity/version is structurally valid.");
        }

        private static SaveDocumentValidationResult Invalid(
            string message) =>
            new SaveDocumentValidationResult(
                SaveDocumentValidationStatus.InvalidDocument,
                EchoSaveDiagnosticCodes
                    .DocumentInvalidIdentity,
                message);

        private static SaveDocumentValidationResult
            InventoryMismatch(
                string message) =>
            new SaveDocumentValidationResult(
                SaveDocumentValidationStatus
                    .InventoryMismatch,
                EchoSaveDiagnosticCodes
                    .DocumentInventoryMismatch,
                message);
    }
}
