
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Read-only payload-free rebuild of one complete technical slot catalog.
    /// </summary>
    internal sealed class SaveSlotCatalogScanner
    {
        private const int MaximumMetadataTextLength = 256;
        private const int MaximumParticipantCount = 4096;

        private readonly ISaveStorageBackend storageBackend;
        private readonly ISaveSerializer serializer;
        private readonly int maxScanEntries;

        internal SaveSlotCatalogScanner(
            ISaveStorageBackend storageBackend,
            ISaveSerializer serializer,
            int maxScanEntries)
        {
            this.storageBackend =
                storageBackend ??
                throw new ArgumentNullException(
                    nameof(storageBackend));

            this.serializer =
                serializer ??
                throw new ArgumentNullException(
                    nameof(serializer));

            if (maxScanEntries <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxScanEntries));
            }

            this.maxScanEntries =
                maxScanEntries;
        }

        internal SaveSlotCatalogRefreshResult Scan()
        {
            if (!(storageBackend is
                ISaveStorageDiscoveryBackend discoveryBackend))
            {
                return Failure(
                    SaveSlotCatalogRefreshStatus.DiscoveryUnavailable,
                    EchoSaveDiagnosticCodes.CatalogDiscoveryUnavailable,
                    "The active Chronicle storage backend does not expose slot discovery.");
            }

            SaveStorageResult slotsKeyResult =
                SaveStorageKey.TryCreate(
                    "slots",
                    out SaveStorageKey slotsKey);

            if (!slotsKeyResult.Succeeded)
            {
                return Failure(
                    SaveSlotCatalogRefreshStatus.DiscoveryFailed,
                    EchoSaveDiagnosticCodes.CatalogDiscoveryFailed,
                    slotsKeyResult.Message);
            }

            SaveStorageDiscoveryResult discovery =
                discoveryBackend.DiscoverChildDirectories(
                    slotsKey,
                    maxScanEntries);

            if (discovery.Status ==
                SaveStorageDiscoveryStatus.ParentNotFound)
            {
                return Success(
                    SaveSlotCatalogSnapshot.Empty);
            }

            if (discovery.Status ==
                SaveStorageDiscoveryStatus.LimitExceeded)
            {
                return Failure(
                    SaveSlotCatalogRefreshStatus.ScanLimitExceeded,
                    EchoSaveDiagnosticCodes.CatalogScanLimitExceeded,
                    discovery.Message);
            }

            if (!discovery.Succeeded)
            {
                return Failure(
                    SaveSlotCatalogRefreshStatus.DiscoveryFailed,
                    EchoSaveDiagnosticCodes.CatalogDiscoveryFailed,
                    discovery.Message);
            }

            List<SaveSlotId> slotIds =
                new List<SaveSlotId>();

            HashSet<string> discoveredCanonicalIds =
                new HashSet<string>(
                    StringComparer.Ordinal);

            for (int i = 0;
                 i < discovery.ChildNames.Count;
                 i++)
            {
                if (SaveSlotId.TryParse(
                        discovery.ChildNames[i],
                        out SaveSlotId slotId) &&
                    discoveredCanonicalIds.Add(
                        slotId.Value))
                {
                    slotIds.Add(
                        slotId);
                }
            }

            slotIds.Sort(
                (left, right) =>
                    string.CompareOrdinal(
                        left.Value,
                        right.Value));

            List<SaveSlotCatalogEntry> entries =
                new List<SaveSlotCatalogEntry>(
                    slotIds.Count);

            for (int i = 0;
                 i < slotIds.Count;
                 i++)
            {
                entries.Add(
                    ReadEntry(
                        slotIds[i]));
            }

            return Success(
                new SaveSlotCatalogSnapshot(
                    entries.ToArray()));
        }

        private SaveSlotCatalogEntry ReadEntry(
            SaveSlotId slotId)
        {
            SaveStorageResult headKeyResult =
                SaveStorageKey.TryCreate(
                    "slots/" +
                    slotId.Value +
                    "/head.json",
                    out SaveStorageKey headKey);

            if (!headKeyResult.Succeeded)
            {
                return Degraded(
                    slotId,
                    default,
                    SaveSlotHealth.InvalidHead,
                    EchoSaveDiagnosticCodes.CatalogHeadInvalid,
                    "The Chronicle slot head storage key is invalid.");
            }

            SaveStorageReadResult headRead =
                storageBackend.Read(
                    headKey);

            if (headRead.Result.Status ==
                SaveStorageStatus.NotFound)
            {
                return Degraded(
                    slotId,
                    default,
                    SaveSlotHealth.MissingHead,
                    EchoSaveDiagnosticCodes.CatalogHeadMissing,
                    "The Chronicle slot does not have a current head.");
            }

            if (!headRead.Succeeded)
            {
                return Degraded(
                    slotId,
                    default,
                    SaveSlotHealth.BackendReadFailure,
                    EchoSaveDiagnosticCodes.CatalogBackendReadFailed,
                    "The Chronicle slot head could not be read.");
            }

            SaveSerializerResult headDeserialize =
                serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        headRead.Data),
                    out SaveHeadPointer head);

            if (!headDeserialize.Succeeded)
            {
                return Degraded(
                    slotId,
                    default,
                    headDeserialize.Status ==
                        SaveSerializerStatus.UnsupportedDocumentVersion
                        ? SaveSlotHealth.UnsupportedHead
                        : SaveSlotHealth.InvalidHead,
                    headDeserialize.Status ==
                        SaveSerializerStatus.UnsupportedDocumentVersion
                        ? EchoSaveDiagnosticCodes.CatalogHeadUnsupported
                        : EchoSaveDiagnosticCodes.CatalogHeadInvalid,
                    "The Chronicle slot head is invalid or unsupported.");
            }

            SaveDocumentValidationResult headValidation =
                SaveCommitDocumentValidator.ValidateHead(
                    head);

            if (!headValidation.Succeeded)
            {
                return Degraded(
                    slotId,
                    default,
                    SaveSlotHealth.InvalidHead,
                    EchoSaveDiagnosticCodes.CatalogHeadInvalid,
                    headValidation.Message);
            }

            if (!string.Equals(
                    head.slotId,
                    slotId.Value,
                    StringComparison.Ordinal))
            {
                return Degraded(
                    slotId,
                    default,
                    SaveSlotHealth.IdentityMismatch,
                    EchoSaveDiagnosticCodes.CatalogIdentityMismatch,
                    "The Chronicle head slot identity does not agree with the discovered technical slot.");
            }

            if (!SaveGenerationId.TryParse(
                    head.currentGenerationId,
                    out SaveGenerationId generationId))
            {
                return Degraded(
                    slotId,
                    default,
                    SaveSlotHealth.InvalidHead,
                    EchoSaveDiagnosticCodes.CatalogHeadInvalid,
                    "The Chronicle slot head has no valid current generation.");
            }

            SaveStorageResult keysResult =
                SaveGenerationStorageKeys.TryCreate(
                    slotId,
                    generationId,
                    out SaveGenerationStorageKeys keys);

            if (!keysResult.Succeeded)
            {
                return Degraded(
                    slotId,
                    generationId,
                    SaveSlotHealth.InvalidHead,
                    EchoSaveDiagnosticCodes.CatalogHeadInvalid,
                    "The Chronicle current generation storage key is invalid.");
            }

            SaveStorageReadResult manifestRead =
                storageBackend.Read(
                    keys.GenerationManifest);

            if (manifestRead.Result.Status ==
                SaveStorageStatus.NotFound)
            {
                return Degraded(
                    slotId,
                    generationId,
                    SaveSlotHealth.MissingManifest,
                    EchoSaveDiagnosticCodes.CatalogManifestMissing,
                    "The Chronicle current generation manifest is missing.");
            }

            if (!manifestRead.Succeeded)
            {
                return Degraded(
                    slotId,
                    generationId,
                    SaveSlotHealth.BackendReadFailure,
                    EchoSaveDiagnosticCodes.CatalogBackendReadFailed,
                    "The Chronicle current generation manifest could not be read.");
            }

            SaveSerializerResult manifestDeserialize =
                serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        manifestRead.Data),
                    out SaveManifest manifest);

            if (!manifestDeserialize.Succeeded)
            {
                return Degraded(
                    slotId,
                    generationId,
                    manifestDeserialize.Status ==
                        SaveSerializerStatus.UnsupportedDocumentVersion
                        ? SaveSlotHealth.UnsupportedManifest
                        : SaveSlotHealth.InvalidManifest,
                    manifestDeserialize.Status ==
                        SaveSerializerStatus.UnsupportedDocumentVersion
                        ? EchoSaveDiagnosticCodes.CatalogManifestUnsupported
                        : EchoSaveDiagnosticCodes.CatalogManifestInvalid,
                    "The Chronicle current generation manifest is invalid or unsupported.");
            }

            if (!string.Equals(
                    manifest.slotId,
                    slotId.Value,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    manifest.generationId,
                    generationId.Value,
                    StringComparison.Ordinal))
            {
                return Degraded(
                    slotId,
                    generationId,
                    SaveSlotHealth.IdentityMismatch,
                    EchoSaveDiagnosticCodes.CatalogIdentityMismatch,
                    "The Chronicle head and current manifest identities do not agree.");
            }

            if (!ValidateManifestMetadata(
                    manifest))
            {
                return Degraded(
                    slotId,
                    generationId,
                    SaveSlotHealth.InvalidManifest,
                    EchoSaveDiagnosticCodes.CatalogManifestInvalid,
                    "The Chronicle current manifest contains invalid or unbounded lightweight metadata.");
            }

            return new SaveSlotCatalogEntry(
                slotId,
                generationId,
                SaveSlotHealth.Healthy,
                string.Empty,
                "The Chronicle slot metadata is healthy and selectable.",
                manifest.createdUtc,
                manifest.updatedUtc,
                manifest.displayName,
                manifest.saveKind,
                manifest.projectId,
                manifest.projectVersion,
                manifest.buildId,
                manifest.payloadEntries == null
                    ? 0
                    : manifest.payloadEntries.Length,
                manifest.payloadByteLength);
        }

        private static bool ValidateManifestMetadata(
            SaveManifest manifest)
        {
            if (manifest == null ||
                manifest.commitState !=
                    SaveGenerationCommitState.Committed ||
                manifest.payloadByteLength < 0)
            {
                return false;
            }

            SavePayloadInventoryEntry[] entries =
                manifest.payloadEntries ??
                Array.Empty<SavePayloadInventoryEntry>();

            if (entries.Length >
                MaximumParticipantCount)
            {
                return false;
            }

            if (!Bounded(
                    manifest.createdUtc) ||
                !Bounded(
                    manifest.updatedUtc) ||
                !Bounded(
                    manifest.displayName) ||
                !Bounded(
                    manifest.saveKind) ||
                !Bounded(
                    manifest.projectId) ||
                !Bounded(
                    manifest.projectVersion) ||
                !Bounded(
                    manifest.buildId))
            {
                return false;
            }

            return
                ValidUtcOrEmpty(
                    manifest.createdUtc) &&
                ValidUtcOrEmpty(
                    manifest.updatedUtc);
        }

        private static bool Bounded(
            string value) =>
            value == null ||
            value.Length <= MaximumMetadataTextLength;

        private static bool ValidUtcOrEmpty(
            string value)
        {
            if (string.IsNullOrEmpty(
                    value))
            {
                return true;
            }

            return DateTimeOffset.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out _);
        }

        private static SaveSlotCatalogEntry Degraded(
            SaveSlotId slotId,
            SaveGenerationId generationId,
            SaveSlotHealth health,
            string diagnosticCode,
            string message) =>
            new SaveSlotCatalogEntry(
                slotId,
                generationId,
                health,
                diagnosticCode,
                message,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                0,
                0);

        private static SaveSlotCatalogRefreshResult Success(
            SaveSlotCatalogSnapshot snapshot)
        {
            SaveSlotCatalogRefreshStatus status;

            if (snapshot.Count == 0)
            {
                status =
                    SaveSlotCatalogRefreshStatus.SucceededEmpty;
            }
            else if (snapshot.DegradedCount > 0)
            {
                status =
                    SaveSlotCatalogRefreshStatus.SucceededWithDegradedSlots;
            }
            else
            {
                status =
                    SaveSlotCatalogRefreshStatus.Succeeded;
            }

            return new SaveSlotCatalogRefreshResult(
                status,
                string.Empty,
                "The Chronicle slot catalog scan completed successfully.",
                snapshot,
                false);
        }

        private static SaveSlotCatalogRefreshResult Failure(
            SaveSlotCatalogRefreshStatus status,
            string diagnosticCode,
            string message) =>
            new SaveSlotCatalogRefreshResult(
                status,
                diagnosticCode,
                message,
                null,
                false);
    }
}
