
using System;

namespace EchoDevGames.EchoSave
{
    [Serializable]
    public sealed class SaveManifest :
        ISavePackageDocument
    {
        public string documentKind =
            SaveDocumentKinds.Manifest;

        public int formatMajor =
            SaveDocumentVersions.ManifestMajor;

        public int formatMinor =
            SaveDocumentVersions.ManifestMinor;

        public int formatRevision =
            SaveDocumentVersions.ManifestRevision;

        public string slotId =
            string.Empty;

        public string generationId =
            string.Empty;

        public string createdUtc =
            string.Empty;

        public string updatedUtc =
            string.Empty;

        public string saveKind =
            string.Empty;

        public string projectId =
            string.Empty;

        public string projectVersion =
            string.Empty;

        public string buildId =
            string.Empty;

        public string displayName =
            string.Empty;

        public string payloadFileName =
            "payload.json";

        public long payloadByteLength;

        public string payloadChecksum =
            string.Empty;

        public string integrityAlgorithm =
            Sha256IntegrityProvider.StableId;

        public SavePayloadInventoryEntry[] payloadEntries =
            Array.Empty<SavePayloadInventoryEntry>();

        public SaveGenerationCommitState commitState =
            SaveGenerationCommitState.Candidate;

        public string thumbnailDescriptor =
            string.Empty;

        public string migrationProvenance =
            string.Empty;

        public string recoveryProvenance =
            string.Empty;

        public string DocumentKind =>
            documentKind ?? string.Empty;

        public int FormatMajor =>
            formatMajor;

        public int FormatMinor =>
            formatMinor;

        public int FormatRevision =>
            formatRevision;
    }
}
