
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Derived catalog-cache maintenance.
    ///
    /// The durable scanner always establishes catalog truth first in M5-05.
    /// catalog.cache.json is then compared with that truth and may be rebuilt.
    /// It never makes a slot healthy, selectable, or existent on its own.
    /// </summary>
    internal sealed class SaveCatalogCacheCoordinator
    {
        internal const int CurrentSchemaVersion = 1;
        internal const string CacheFileName =
            "catalog.cache.json";

        private readonly ISaveStorageBackend storage;
        private readonly ISaveSerializer serializer;
        private readonly SaveSlotCatalogScanner scanner;
        private readonly int maxEntries;

        internal SaveCatalogCacheCoordinator(
            ISaveStorageBackend storage,
            ISaveSerializer serializer,
            int maxEntries)
        {
            this.storage =
                storage ??
                throw new ArgumentNullException(
                    nameof(storage));

            this.serializer =
                serializer ??
                throw new ArgumentNullException(
                    nameof(serializer));

            if (maxEntries <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxEntries));
            }

            this.maxEntries =
                maxEntries;

            scanner =
                new SaveSlotCatalogScanner(
                    storage,
                    serializer,
                    maxEntries);
        }

        internal SaveCatalogCachePreview Preview()
        {
            SaveSlotCatalogRefreshResult durable =
                scanner.Scan();

            if (!durable.Succeeded)
            {
                return new SaveCatalogCachePreview(
                    SaveCatalogCacheState
                        .DurableCatalogUnavailable,
                    durable.DiagnosticCode,
                    "Chronicle could not establish bounded durable catalog truth for cache Preview. " +
                    durable.Message,
                    durable.Snapshot,
                    0,
                    string.Empty,
                    string.Empty,
                    false);
            }

            return Inspect(
                durable.Snapshot);
        }

        internal SaveCatalogCachePreview Inspect(
            SaveSlotCatalogSnapshot durableSnapshot)
        {
            if (durableSnapshot == null)
            {
                return new SaveCatalogCachePreview(
                    SaveCatalogCacheState
                        .DurableCatalogUnavailable,
                    "ECHOSAVE-CACHE-SNAPSHOT",
                    "Chronicle catalog-cache inspection requires one truthful durable catalog snapshot.",
                    SaveSlotCatalogSnapshot.Empty,
                    0,
                    string.Empty,
                    string.Empty,
                    false);
            }

            SaveCatalogCacheEntryDocument[] durableEntries =
                CreateEntries(
                    durableSnapshot);

            string durableFingerprint =
                ComputeFingerprint(
                    durableEntries);

            SaveCatalogCacheReadResult cache =
                ReadCache();

            SaveCatalogCacheState state =
                cache.State;

            string message =
                cache.Message;

            if (cache.Succeeded)
            {
                if (string.Equals(
                        cache.Fingerprint,
                        durableFingerprint,
                        StringComparison.Ordinal))
                {
                    state =
                        SaveCatalogCacheState.Valid;

                    message =
                        "The derived Chronicle catalog cache matches current bounded durable head/manifest truth.";
                }
                else
                {
                    state =
                        SaveCatalogCacheState.Stale;

                    message =
                        "The derived Chronicle catalog cache is stale relative to current bounded durable head/manifest truth.";
                }
            }

            bool canRebuild =
                storage is
                    ISaveStoragePublicationBackend publication &&
                publication.PublicationCapabilities
                    .SupportsCurrentObjectPublication;

            return new SaveCatalogCachePreview(
                state,
                cache.DiagnosticCode,
                message,
                durableSnapshot,
                cache.EntryCount,
                durableFingerprint,
                cache.Fingerprint,
                canRebuild);
        }

        internal SaveCatalogCacheRebuildResult Rebuild()
        {
            SaveSlotCatalogRefreshResult durable =
                scanner.Scan();

            if (!durable.Succeeded)
            {
                return new SaveCatalogCacheRebuildResult(
                    false,
                    SaveCatalogCacheState
                        .DurableCatalogUnavailable,
                    durable.DiagnosticCode,
                    "Chronicle refused catalog-cache rebuild because bounded durable catalog truth is unavailable. " +
                    durable.Message,
                    durable.Snapshot,
                    string.Empty);
            }

            return RebuildFromSnapshot(
                durable.Snapshot);
        }

        internal SaveCatalogCacheRebuildResult
            RebuildFromSnapshot(
                SaveSlotCatalogSnapshot snapshot)
        {
            if (snapshot == null)
            {
                return Failure(
                    SaveCatalogCacheState
                        .DurableCatalogUnavailable,
                    "ECHOSAVE-CACHE-SNAPSHOT",
                    "Chronicle catalog-cache rebuild requires one truthful durable catalog snapshot.",
                    SaveSlotCatalogSnapshot.Empty,
                    string.Empty);
            }

            if (!(storage is
                ISaveStoragePublicationBackend publication) ||
                !publication.PublicationCapabilities
                    .SupportsCurrentObjectPublication)
            {
                return Failure(
                    SaveCatalogCacheState
                        .BackendUnsupported,
                    "ECHOSAVE-CACHE-PUBLICATION",
                    "The active Chronicle storage backend does not expose current-object publication for derived cache maintenance.",
                    snapshot,
                    string.Empty);
            }

            SaveStorageResult keyResult =
                SaveStorageKey.TryCreate(
                    CacheFileName,
                    out SaveStorageKey cacheKey);

            if (!keyResult.Succeeded)
            {
                return Failure(
                    SaveCatalogCacheState
                        .BackendUnsupported,
                    keyResult.DiagnosticCode,
                    keyResult.Message,
                    snapshot,
                    string.Empty);
            }

            SaveCatalogCacheEntryDocument[] entries =
                CreateEntries(
                    snapshot);

            string fingerprint =
                ComputeFingerprint(
                    entries);

            SaveCatalogCacheDocument document =
                new SaveCatalogCacheDocument
                {
                    schemaVersion =
                        CurrentSchemaVersion,
                    generatedUtc =
                        DateTimeOffset.UtcNow
                            .ToString(
                                "O",
                                CultureInfo.InvariantCulture),
                    snapshotFingerprint =
                        fingerprint,
                    entries =
                        entries
                };

            SaveSerializerResult serialized =
                serializer.Serialize(
                    document,
                    out string json);

            if (!serialized.Succeeded)
            {
                return Failure(
                    SaveCatalogCacheState.Corrupt,
                    "ECHOSAVE-CACHE-SERIALIZE",
                    "Chronicle could not serialize the derived catalog cache. " +
                    serialized.Message,
                    snapshot,
                    fingerprint);
            }

            SaveStorageResult write =
                publication.PublishCurrentObject(
                    cacheKey,
                    Encoding.UTF8.GetBytes(
                        json));

            if (!write.Succeeded)
            {
                return Failure(
                    SaveCatalogCacheState.Stale,
                    string.IsNullOrEmpty(
                        write.DiagnosticCode)
                        ? "ECHOSAVE-CACHE-WRITE"
                        : write.DiagnosticCode,
                    "Chronicle durable catalog truth remains valid, but derived cache publication failed. " +
                    write.Message,
                    snapshot,
                    fingerprint);
            }

            SaveCatalogCacheReadResult verify =
                ReadCache();

            if (!verify.Succeeded ||
                !string.Equals(
                    verify.Fingerprint,
                    fingerprint,
                    StringComparison.Ordinal))
            {
                return Failure(
                    SaveCatalogCacheState.Stale,
                    "ECHOSAVE-CACHE-VERIFY",
                    "Chronicle published catalog.cache.json, but post-publication verification did not reproduce the durable snapshot fingerprint.",
                    snapshot,
                    fingerprint);
            }

            return new SaveCatalogCacheRebuildResult(
                true,
                SaveCatalogCacheState.Valid,
                string.Empty,
                "The derived Chronicle catalog cache was rebuilt from bounded durable catalog truth and verified.",
                snapshot,
                fingerprint);
        }

        internal SaveCatalogCacheReadResult ReadCache()
        {
            SaveStorageResult keyResult =
                SaveStorageKey.TryCreate(
                    CacheFileName,
                    out SaveStorageKey cacheKey);

            if (!keyResult.Succeeded)
            {
                return ReadFailure(
                    SaveCatalogCacheState
                        .BackendUnsupported,
                    keyResult.DiagnosticCode,
                    keyResult.Message);
            }

            SaveStorageReadResult read =
                storage.Read(
                    cacheKey);

            if (read.Result.Status ==
                SaveStorageStatus.NotFound)
            {
                return ReadFailure(
                    SaveCatalogCacheState.Missing,
                    string.Empty,
                    "The derived Chronicle catalog cache is absent.");
            }

            if (!read.Succeeded)
            {
                return ReadFailure(
                    SaveCatalogCacheState.Corrupt,
                    string.IsNullOrEmpty(
                        read.Result.DiagnosticCode)
                        ? "ECHOSAVE-CACHE-READ"
                        : read.Result.DiagnosticCode,
                    "The derived Chronicle catalog cache could not be read. " +
                    read.Result.Message);
            }

            SaveSerializerResult deserialize =
                serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        read.Data),
                    out SaveCatalogCacheDocument document);

            if (!deserialize.Succeeded ||
                document == null)
            {
                return ReadFailure(
                    SaveCatalogCacheState.Corrupt,
                    "ECHOSAVE-CACHE-DESERIALIZE",
                    "The derived Chronicle catalog cache is not valid for the current cache document shape.");
            }

            if (document.schemaVersion !=
                CurrentSchemaVersion)
            {
                return new SaveCatalogCacheReadResult(
                    SaveCatalogCacheState.Incompatible,
                    "ECHOSAVE-CACHE-SCHEMA",
                    $"The derived Chronicle catalog cache schema {document.schemaVersion} is incompatible with schema {CurrentSchemaVersion}.",
                    SaveSlotCatalogSnapshot.Empty,
                    document.entries == null
                        ? 0
                        : document.entries.Length,
                    document.snapshotFingerprint);
            }

            SaveCatalogCacheEntryDocument[] entries =
                document.entries ??
                Array.Empty<SaveCatalogCacheEntryDocument>();

            if (entries.Length >
                maxEntries)
            {
                return new SaveCatalogCacheReadResult(
                    SaveCatalogCacheState.Corrupt,
                    "ECHOSAVE-CACHE-BOUND",
                    "The derived Chronicle catalog cache exceeds the configured bounded catalog entry limit.",
                    SaveSlotCatalogSnapshot.Empty,
                    entries.Length,
                    document.snapshotFingerprint);
            }

            if (!TryBuildSnapshot(
                    entries,
                    out SaveSlotCatalogSnapshot snapshot,
                    out string structuralFailure))
            {
                return new SaveCatalogCacheReadResult(
                    SaveCatalogCacheState.Corrupt,
                    "ECHOSAVE-CACHE-STRUCTURE",
                    structuralFailure,
                    SaveSlotCatalogSnapshot.Empty,
                    entries.Length,
                    document.snapshotFingerprint);
            }

            string fingerprint =
                ComputeFingerprint(
                    entries);

            if (!string.Equals(
                    fingerprint,
                    document.snapshotFingerprint,
                    StringComparison.Ordinal))
            {
                return new SaveCatalogCacheReadResult(
                    SaveCatalogCacheState.Corrupt,
                    "ECHOSAVE-CACHE-FINGERPRINT",
                    "The derived Chronicle catalog cache does not agree with its own deterministic fingerprint.",
                    SaveSlotCatalogSnapshot.Empty,
                    entries.Length,
                    document.snapshotFingerprint);
            }

            return new SaveCatalogCacheReadResult(
                SaveCatalogCacheState.Valid,
                string.Empty,
                "The derived Chronicle catalog cache is structurally valid.",
                snapshot,
                entries.Length,
                fingerprint);
        }

        private static SaveCatalogCacheEntryDocument[]
            CreateEntries(
                SaveSlotCatalogSnapshot snapshot)
        {
            SaveCatalogCacheEntryDocument[] entries =
                new SaveCatalogCacheEntryDocument[
                    snapshot.Count];

            for (int i = 0;
                 i < snapshot.Count;
                 i++)
            {
                SaveSlotCatalogEntry source =
                    snapshot.Entries[i];

                entries[i] =
                    new SaveCatalogCacheEntryDocument
                    {
                        slotId =
                            source.SlotId.Value,
                        currentGenerationId =
                            source.CurrentGenerationId.Value ??
                            string.Empty,
                        health =
                            (int)source.Health,
                        diagnosticCode =
                            source.DiagnosticCode,
                        message =
                            source.Message,
                        createdUtc =
                            source.CreatedUtc,
                        updatedUtc =
                            source.UpdatedUtc,
                        displayName =
                            source.DisplayName,
                        saveKind =
                            source.SaveKind,
                        projectId =
                            source.ProjectId,
                        projectVersion =
                            source.ProjectVersion,
                        buildId =
                            source.BuildId,
                        participantCount =
                            source.ParticipantCount,
                        payloadByteLength =
                            source.PayloadByteLength
                    };
            }

            Array.Sort(
                entries,
                (left, right) =>
                    string.Compare(
                        left.slotId,
                        right.slotId,
                        StringComparison.Ordinal));

            return entries;
        }

        private static bool TryBuildSnapshot(
            SaveCatalogCacheEntryDocument[] source,
            out SaveSlotCatalogSnapshot snapshot,
            out string failure)
        {
            snapshot =
                SaveSlotCatalogSnapshot.Empty;

            failure =
                string.Empty;

            SaveSlotCatalogEntry[] entries =
                new SaveSlotCatalogEntry[
                    source.Length];

            string previousSlot =
                null;

            for (int i = 0;
                 i < source.Length;
                 i++)
            {
                SaveCatalogCacheEntryDocument item =
                    source[i];

                if (item == null ||
                    !SaveSlotId.TryParse(
                        item.slotId,
                        out SaveSlotId slotId) ||
                    !Enum.IsDefined(
                        typeof(SaveSlotHealth),
                        item.health) ||
                    item.participantCount < 0 ||
                    item.payloadByteLength < 0)
                {
                    failure =
                        "The derived Chronicle catalog cache contains invalid bounded metadata.";
                    return false;
                }

                if (previousSlot != null &&
                    string.Compare(
                        previousSlot,
                        slotId.Value,
                        StringComparison.Ordinal) >= 0)
                {
                    failure =
                        "The derived Chronicle catalog cache is not in strict canonical slot-ID order.";
                    return false;
                }

                previousSlot =
                    slotId.Value;

                SaveGenerationId generationId =
                    default;

                if (!string.IsNullOrEmpty(
                        item.currentGenerationId) &&
                    !SaveGenerationId.TryParse(
                        item.currentGenerationId,
                        out generationId))
                {
                    failure =
                        "The derived Chronicle catalog cache contains an invalid current generation identity.";
                    return false;
                }

                entries[i] =
                    new SaveSlotCatalogEntry(
                        slotId,
                        generationId,
                        (SaveSlotHealth)item.health,
                        item.diagnosticCode,
                        item.message,
                        item.createdUtc,
                        item.updatedUtc,
                        item.displayName,
                        item.saveKind,
                        item.projectId,
                        item.projectVersion,
                        item.buildId,
                        item.participantCount,
                        item.payloadByteLength);
            }

            snapshot =
                new SaveSlotCatalogSnapshot(
                    entries);

            return true;
        }

        private static string ComputeFingerprint(
            SaveCatalogCacheEntryDocument[] entries)
        {
            StringBuilder canonical =
                new StringBuilder();

            for (int i = 0;
                 i < entries.Length;
                 i++)
            {
                SaveCatalogCacheEntryDocument entry =
                    entries[i];

                Append(
                    canonical,
                    entry.slotId);
                Append(
                    canonical,
                    entry.currentGenerationId);
                Append(
                    canonical,
                    entry.health.ToString(
                        CultureInfo.InvariantCulture));
                Append(
                    canonical,
                    entry.diagnosticCode);
                Append(
                    canonical,
                    entry.message);
                Append(
                    canonical,
                    entry.createdUtc);
                Append(
                    canonical,
                    entry.updatedUtc);
                Append(
                    canonical,
                    entry.displayName);
                Append(
                    canonical,
                    entry.saveKind);
                Append(
                    canonical,
                    entry.projectId);
                Append(
                    canonical,
                    entry.projectVersion);
                Append(
                    canonical,
                    entry.buildId);
                Append(
                    canonical,
                    entry.participantCount.ToString(
                        CultureInfo.InvariantCulture));
                Append(
                    canonical,
                    entry.payloadByteLength.ToString(
                        CultureInfo.InvariantCulture));
            }

            using (SHA256 sha =
                   SHA256.Create())
            {
                byte[] hash =
                    sha.ComputeHash(
                        Encoding.UTF8.GetBytes(
                            canonical.ToString()));

                StringBuilder hex =
                    new StringBuilder(
                        hash.Length * 2);

                for (int i = 0;
                     i < hash.Length;
                     i++)
                {
                    hex.Append(
                        hash[i].ToString("x2"));
                }

                return hex.ToString();
            }
        }

        private static void Append(
            StringBuilder builder,
            string value)
        {
            string text =
                value ?? string.Empty;

            builder.Append(
                text.Length.ToString(
                    CultureInfo.InvariantCulture));
            builder.Append(':');
            builder.Append(
                text);
            builder.Append('|');
        }

        private static SaveCatalogCacheReadResult
            ReadFailure(
                SaveCatalogCacheState state,
                string diagnosticCode,
                string message) =>
            new SaveCatalogCacheReadResult(
                state,
                diagnosticCode,
                message,
                SaveSlotCatalogSnapshot.Empty,
                0,
                string.Empty);

        private static SaveCatalogCacheRebuildResult Failure(
            SaveCatalogCacheState state,
            string diagnosticCode,
            string message,
            SaveSlotCatalogSnapshot snapshot,
            string fingerprint) =>
            new SaveCatalogCacheRebuildResult(
                false,
                state,
                diagnosticCode,
                message,
                snapshot,
                fingerprint);
    }
}
