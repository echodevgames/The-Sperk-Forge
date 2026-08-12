
using System;

namespace EchoDevGames.EchoSave
{
    [Serializable]
    internal sealed class SaveCatalogCacheDocument
    {
        public int schemaVersion = 1;
        public string generatedUtc = string.Empty;
        public string snapshotFingerprint = string.Empty;
        public SaveCatalogCacheEntryDocument[] entries =
            Array.Empty<SaveCatalogCacheEntryDocument>();
    }

    [Serializable]
    internal sealed class SaveCatalogCacheEntryDocument
    {
        public string slotId = string.Empty;
        public string currentGenerationId = string.Empty;
        public int health;
        public string diagnosticCode = string.Empty;
        public string message = string.Empty;
        public string createdUtc = string.Empty;
        public string updatedUtc = string.Empty;
        public string displayName = string.Empty;
        public string saveKind = string.Empty;
        public string projectId = string.Empty;
        public string projectVersion = string.Empty;
        public string buildId = string.Empty;
        public int participantCount;
        public long payloadByteLength;
    }

    public sealed class SaveCatalogCachePreview
    {
        internal SaveCatalogCachePreview(
            SaveCatalogCacheState state,
            string diagnosticCode,
            string message,
            SaveSlotCatalogSnapshot durableSnapshot,
            int cachedEntryCount,
            string durableFingerprint,
            string cacheFingerprint,
            bool canRebuild)
        {
            State = state;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
            DurableSnapshot =
                durableSnapshot ??
                SaveSlotCatalogSnapshot.Empty;
            CachedEntryCount = cachedEntryCount;
            DurableFingerprint = durableFingerprint ?? string.Empty;
            CacheFingerprint = cacheFingerprint ?? string.Empty;
            CanRebuild = canRebuild;
        }

        public SaveCatalogCacheState State { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public SaveSlotCatalogSnapshot DurableSnapshot { get; }

        public int CachedEntryCount { get; }

        public string DurableFingerprint { get; }

        public string CacheFingerprint { get; }

        public bool CanRebuild { get; }
    }

    public sealed class SaveCatalogCacheRebuildResult
    {
        internal SaveCatalogCacheRebuildResult(
            bool succeeded,
            SaveCatalogCacheState state,
            string diagnosticCode,
            string message,
            SaveSlotCatalogSnapshot snapshot,
            string fingerprint)
        {
            Succeeded = succeeded;
            State = state;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
            Snapshot =
                snapshot ??
                SaveSlotCatalogSnapshot.Empty;
            Fingerprint = fingerprint ?? string.Empty;
        }

        public bool Succeeded { get; }

        public SaveCatalogCacheState State { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public SaveSlotCatalogSnapshot Snapshot { get; }

        public string Fingerprint { get; }
    }

    internal sealed class SaveCatalogCacheReadResult
    {
        internal SaveCatalogCacheReadResult(
            SaveCatalogCacheState state,
            string diagnosticCode,
            string message,
            SaveSlotCatalogSnapshot snapshot,
            int entryCount,
            string fingerprint)
        {
            State = state;
            DiagnosticCode = diagnosticCode ?? string.Empty;
            Message = message ?? string.Empty;
            Snapshot =
                snapshot ??
                SaveSlotCatalogSnapshot.Empty;
            EntryCount = entryCount;
            Fingerprint = fingerprint ?? string.Empty;
        }

        internal SaveCatalogCacheState State { get; }

        internal string DiagnosticCode { get; }

        internal string Message { get; }

        internal SaveSlotCatalogSnapshot Snapshot { get; }

        internal int EntryCount { get; }

        internal string Fingerprint { get; }

        internal bool Succeeded =>
            State == SaveCatalogCacheState.Valid;
    }
}
