using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave
{
    internal enum SavePackageDocumentReadStatus
    {
        Succeeded = 0,
        InvalidRequest = 1,
        InvalidDocument = 2,
        UnsupportedVersion = 3,
        MigrationUnavailable = 4,
        MigrationFailed = 5,
        FinalValidationFailed = 6
    }

    internal sealed class SavePackageDocumentReadResult
    {
        private readonly SavePackageDocumentMigrationProvenanceEntry[] provenance;

        internal SavePackageDocumentReadResult(
            SavePackageDocumentReadStatus status,
            SavePackageDocumentVersion sourceVersion,
            SavePackageDocumentVersion currentVersion,
            bool wasMigrated,
            SavePackageDocumentMigrationProvenanceEntry[] provenance,
            string diagnosticCode,
            string message)
        {
            Status = status;
            SourceVersion = sourceVersion;
            CurrentVersion = currentVersion;
            WasMigrated = wasMigrated;
            this.provenance =
                provenance == null
                    ? Array.Empty<SavePackageDocumentMigrationProvenanceEntry>()
                    : (SavePackageDocumentMigrationProvenanceEntry[])provenance.Clone();
            DiagnosticCode =
                SavePackageDocumentMigrationText.BoundDiagnosticCode(
                    diagnosticCode);
            Message =
                SavePackageDocumentMigrationText.BoundMessage(
                    message);
        }

        internal SavePackageDocumentReadStatus Status { get; }

        internal SavePackageDocumentVersion SourceVersion { get; }

        internal SavePackageDocumentVersion CurrentVersion { get; }

        internal bool WasMigrated { get; }

        internal IReadOnlyList<SavePackageDocumentMigrationProvenanceEntry>
            Provenance =>
            provenance;

        internal string DiagnosticCode { get; }

        internal string Message { get; }

        internal bool Succeeded =>
            Status == SavePackageDocumentReadStatus.Succeeded;

        internal bool IsUnsupported =>
            Status == SavePackageDocumentReadStatus.UnsupportedVersion ||
            Status == SavePackageDocumentReadStatus.MigrationUnavailable ||
            Status == SavePackageDocumentReadStatus.MigrationFailed;
    }

    /// <summary>
    /// Internal package-document read seam. Historical normalization occurs here,
    /// before the existing strict exact-current serializer validation.
    /// </summary>
    internal sealed class SavePackageDocumentReader
    {
        private readonly ISaveSerializer serializer;
        private readonly SavePackageDocumentMigrationRegistry registry;
        private readonly SavePackageDocumentMigrationCoordinator coordinator;

        internal SavePackageDocumentReader(
            ISaveSerializer serializer,
            SavePackageDocumentMigrationRegistry registry)
        {
            this.serializer =
                serializer ??
                throw new ArgumentNullException(
                    nameof(serializer));

            this.registry =
                registry ??
                throw new ArgumentNullException(
                    nameof(registry));

            coordinator =
                new SavePackageDocumentMigrationCoordinator(
                    registry);
        }

        internal SavePackageDocumentReadResult ReadCurrent<T>(
            string serializedDocument,
            string expectedDocumentKind,
            out T document)
            where T : class, ISavePackageDocument
        {
            document = null;

            if (!SavePackageDocumentVersionAuthority.TryGetCurrent(
                    expectedDocumentKind,
                    out SavePackageDocumentVersion currentVersion))
            {
                return Failure(
                    SavePackageDocumentReadStatus.InvalidRequest,
                    default,
                    default,
                    false,
                    SavePackageDocumentMigrationDiagnosticCodes.InvalidRequest,
                    "Chronicle package-document reading requires one supported expected document kind.");
            }

            SavePackageDocumentVersionProbeResult probe =
                SavePackageDocumentVersionProbe.Probe(
                    serializedDocument);

            if (!probe.Succeeded)
            {
                return Failure(
                    SavePackageDocumentReadStatus.InvalidDocument,
                    default,
                    currentVersion,
                    false,
                    probe.DiagnosticCode,
                    probe.Message);
            }

            if (!string.Equals(
                    probe.DocumentKind,
                    expectedDocumentKind,
                    StringComparison.Ordinal))
            {
                return Failure(
                    SavePackageDocumentReadStatus.InvalidDocument,
                    probe.Version,
                    currentVersion,
                    false,
                    SavePackageDocumentMigrationDiagnosticCodes.ProbeFailed,
                    "Chronicle package-document reading found a different document kind than the requested read seam.");
            }

            if (probe.Version > currentVersion)
            {
                return Failure(
                    SavePackageDocumentReadStatus.UnsupportedVersion,
                    probe.Version,
                    currentVersion,
                    false,
                    SavePackageDocumentMigrationDiagnosticCodes.NewerVersionUnsupported,
                    "Chronicle package-document reading refuses newer package formats and does not downgrade them.");
            }

            if (probe.Version == currentVersion)
            {
                return DeserializeCurrent(
                    serializedDocument,
                    expectedDocumentKind,
                    probe.Version,
                    currentVersion,
                    false,
                    Array.Empty<SavePackageDocumentMigrationProvenanceEntry>(),
                    out document);
            }

            if (!registry.IsValid)
            {
                return Failure(
                    SavePackageDocumentReadStatus.MigrationUnavailable,
                    probe.Version,
                    currentVersion,
                    false,
                    string.IsNullOrEmpty(
                        registry.DiagnosticCode)
                        ? SavePackageDocumentMigrationDiagnosticCodes.RegistryInvalid
                        : registry.DiagnosticCode,
                    registry.Message);
            }

            SavePackageDocumentMigrationResult migration =
                coordinator.MigrateToCurrent(
                    expectedDocumentKind,
                    probe.Version,
                    currentVersion,
                    serializedDocument);

            if (!migration.Succeeded)
            {
                SavePackageDocumentReadStatus status =
                    migration.Status ==
                        SavePackageDocumentMigrationStatus.PlanUnavailable
                        ? SavePackageDocumentReadStatus.MigrationUnavailable
                        : SavePackageDocumentReadStatus.MigrationFailed;

                return Failure(
                    status,
                    probe.Version,
                    currentVersion,
                    false,
                    migration.DiagnosticCode,
                    migration.Message);
            }

            SavePackageDocumentMigrationProvenanceEntry[] provenance =
                new SavePackageDocumentMigrationProvenanceEntry[
                    migration.Provenance.Count];

            for (int i = 0;
                 i < provenance.Length;
                 i++)
            {
                provenance[i] =
                    migration.Provenance[i];
            }

            return DeserializeCurrent(
                migration.SerializedDocument,
                expectedDocumentKind,
                probe.Version,
                currentVersion,
                true,
                provenance,
                out document);
        }

        private SavePackageDocumentReadResult DeserializeCurrent<T>(
            string serializedDocument,
            string expectedDocumentKind,
            SavePackageDocumentVersion sourceVersion,
            SavePackageDocumentVersion currentVersion,
            bool wasMigrated,
            SavePackageDocumentMigrationProvenanceEntry[] provenance,
            out T document)
            where T : class, ISavePackageDocument
        {
            document = null;

            SaveSerializerResult deserialized =
                serializer.Deserialize(
                    serializedDocument,
                    out T candidate);

            if (!deserialized.Succeeded ||
                candidate == null)
            {
                return Failure(
                    deserialized.Status ==
                        SaveSerializerStatus.UnsupportedDocumentVersion
                        ? SavePackageDocumentReadStatus.UnsupportedVersion
                        : SavePackageDocumentReadStatus.FinalValidationFailed,
                    sourceVersion,
                    currentVersion,
                    wasMigrated,
                    string.IsNullOrEmpty(
                        deserialized.DiagnosticCode)
                        ? SavePackageDocumentMigrationDiagnosticCodes.FinalValidationFailed
                        : deserialized.DiagnosticCode,
                    string.IsNullOrEmpty(
                        deserialized.Message)
                        ? "Chronicle exact-current package-document deserialization failed."
                        : deserialized.Message);
            }

            if (!string.Equals(
                    candidate.DocumentKind,
                    expectedDocumentKind,
                    StringComparison.Ordinal) ||
                candidate.FormatMajor != currentVersion.Major ||
                candidate.FormatMinor != currentVersion.Minor ||
                candidate.FormatRevision != currentVersion.Revision)
            {
                return Failure(
                    SavePackageDocumentReadStatus.FinalValidationFailed,
                    sourceVersion,
                    currentVersion,
                    wasMigrated,
                    SavePackageDocumentMigrationDiagnosticCodes.FinalValidationFailed,
                    "Chronicle package-document reading rejected a final object whose kind or exact current version did not match the requested read seam.");
            }

            SaveSerializerResult validation =
                SavePackageDocumentValidator.ValidateCurrent(
                    candidate);

            if (!validation.Succeeded)
            {
                return Failure(
                    SavePackageDocumentReadStatus.FinalValidationFailed,
                    sourceVersion,
                    currentVersion,
                    wasMigrated,
                    string.IsNullOrEmpty(
                        validation.DiagnosticCode)
                        ? SavePackageDocumentMigrationDiagnosticCodes.FinalValidationFailed
                        : validation.DiagnosticCode,
                    validation.Message);
            }

            document = candidate;

            return new SavePackageDocumentReadResult(
                SavePackageDocumentReadStatus.Succeeded,
                sourceVersion,
                currentVersion,
                wasMigrated,
                provenance,
                string.Empty,
                wasMigrated
                    ? "Chronicle package document migrated in memory and passed exact-current validation."
                    : "Chronicle package document already used the exact current format and required no migration.");
        }

        private static SavePackageDocumentReadResult Failure(
            SavePackageDocumentReadStatus status,
            SavePackageDocumentVersion sourceVersion,
            SavePackageDocumentVersion currentVersion,
            bool wasMigrated,
            string diagnosticCode,
            string message) =>
            new SavePackageDocumentReadResult(
                status,
                sourceVersion,
                currentVersion,
                wasMigrated,
                Array.Empty<SavePackageDocumentMigrationProvenanceEntry>(),
                diagnosticCode,
                message);
    }
}
