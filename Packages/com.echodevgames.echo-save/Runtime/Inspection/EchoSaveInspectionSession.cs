using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Read-only package inspection session used by M5-03 Editor tooling.
    ///
    /// Opening a missing production root succeeds as an empty inspection state
    /// and does not create the directory.
    /// </summary>
    public sealed class EchoSaveInspectionSession :
        IDisposable
    {
        private readonly LocalFileSaveStorageBackend storageBackend;
        private readonly SaveSlotCatalog catalog;
        private readonly SavePackageDocumentReader packageDocumentReader;
        private bool disposed;

        private EchoSaveInspectionSession(
            EchoSaveRuntimePolicy runtimePolicy,
            LocalFileSaveStorageBackend storageBackend,
            SaveSlotCatalog catalog,
            SavePackageDocumentReader packageDocumentReader,
            bool rootPresent,
            SaveMigrationGraphSnapshot migrationGraph)
        {
            RuntimePolicy = runtimePolicy;
            this.storageBackend = storageBackend;
            this.catalog = catalog;
            this.packageDocumentReader = packageDocumentReader;
            RootPresent = rootPresent;
            MigrationGraph = migrationGraph;
        }

        public EchoSaveRuntimePolicy RuntimePolicy { get; }

        public bool RootPresent { get; }

        public SaveMigrationGraphSnapshot MigrationGraph { get; }

        public SaveSlotCatalogSnapshot CatalogSnapshot =>
            catalog == null
                ? SaveSlotCatalogSnapshot.Empty
                : catalog.Snapshot;

        public static EchoSaveInspectionOpenResult TryOpen(
            EchoSaveConfiguration configuration,
            out EchoSaveInspectionSession session)
        {
            session = null;

            if (configuration == null)
            {
                return Failure(
                    EchoSaveDiagnosticCodes.MissingOrInvalidConfiguration,
                    "Chronicle read-only inspection requires a configuration.");
            }

            if (!configuration.TryResolveRuntimePolicy(
                    out EchoSaveRuntimePolicy runtimePolicy,
                    out string policyMessage))
            {
                return Failure(
                    EchoSaveDiagnosticCodes.MissingOrInvalidConfiguration,
                    policyMessage);
            }

            SaveStorageResult rootResult =
                SaveStorageRootResolver.TryResolveProductionRoot(
                    configuration,
                    Application.persistentDataPath,
                    out string rootPath);

            if (!rootResult.Succeeded)
            {
                return Failure(
                    rootResult.DiagnosticCode,
                    rootResult.Message);
            }

            SavePackageDocumentMigrationRegistry migrationRegistry =
                SavePackageDocumentMigrationRegistry.CreateProduction();

            SaveMigrationGraphSnapshot migrationGraph =
                SavePackageDocumentMigrationGraphBuilder.Build(
                    migrationRegistry);

            if (!Directory.Exists(
                    rootPath))
            {
                session =
                    new EchoSaveInspectionSession(
                        runtimePolicy,
                        null,
                        null,
                        null,
                        false,
                        migrationGraph);

                return new EchoSaveInspectionOpenResult(
                    true,
                    false,
                    string.Empty,
                    "The Chronicle production save root does not exist. Read-only inspection is open with an empty catalog and no directory was created.");
            }

            LocalFileSaveStorageBackend backend;
            try
            {
                backend =
                    new LocalFileSaveStorageBackend(
                        rootPath);
            }
            catch (ArgumentException exception)
            {
                return Failure(
                    EchoSaveDiagnosticCodes.StorageInvalidPath,
                    "Chronicle read-only inspection could not normalize the production save root. " +
                    exception.Message);
            }

            SaveStorageResult initialized =
                backend.InitializeReadOnly();

            if (!initialized.Succeeded)
            {
                return Failure(
                    initialized.DiagnosticCode,
                    initialized.Message);
            }

            UnityJsonSaveSerializer serializer =
                new UnityJsonSaveSerializer();

            SaveSlotCatalog catalog =
                new SaveSlotCatalog(
                    backend,
                    serializer,
                    runtimePolicy.Limits.CatalogScanLimit);

            SavePackageDocumentReader packageDocumentReader =
                new SavePackageDocumentReader(
                    serializer,
                    migrationRegistry);

            session =
                new EchoSaveInspectionSession(
                    runtimePolicy,
                    backend,
                    catalog,
                    packageDocumentReader,
                    true,
                    migrationGraph);

            return new EchoSaveInspectionOpenResult(
                true,
                true,
                string.Empty,
                "The Chronicle production save root is open for read-only inspection.");
        }

        public SaveSlotCatalogRefreshResult RefreshCatalog()
        {
            if (disposed)
            {
                return new SaveSlotCatalogRefreshResult(
                    SaveSlotCatalogRefreshStatus.DiscoveryFailed,
                    EchoSaveDiagnosticCodes.CatalogDiscoveryFailed,
                    "The Chronicle read-only inspection session is closed.",
                    null,
                    false);
            }

            if (!RootPresent ||
                catalog == null)
            {
                return new SaveSlotCatalogRefreshResult(
                    SaveSlotCatalogRefreshStatus.SucceededEmpty,
                    string.Empty,
                    "The Chronicle production save root does not exist; the read-only catalog is empty.",
                    SaveSlotCatalogSnapshot.Empty,
                    false);
            }

            return catalog.Refresh();
        }

        public SaveGenerationInspectionSnapshot InspectGenerations(
            SaveSlotId slotId)
        {
            if (disposed)
            {
                return SnapshotFailure(
                    SaveGenerationInspectionSnapshotStatus.SessionClosed,
                    slotId,
                    string.Empty,
                    EchoSaveDiagnosticCodes.StorageNotReady,
                    "The Chronicle read-only inspection session is closed.");
            }

            if (!SaveSlotId.TryParse(
                    slotId.Value,
                    out SaveSlotId validatedSlot))
            {
                return SnapshotFailure(
                    SaveGenerationInspectionSnapshotStatus.InvalidSlot,
                    default,
                    string.Empty,
                    EchoSaveDiagnosticCodes.StorageInvalidPath,
                    "Chronicle generation inspection requires a valid technical slot ID.");
            }

            string currentGenerationId =
                string.Empty;

            if (catalog != null &&
                catalog.Snapshot.TryGetEntry(
                    validatedSlot,
                    out SaveSlotCatalogEntry catalogEntry))
            {
                currentGenerationId =
                    catalogEntry.CurrentGenerationId.Value ??
                    string.Empty;
            }

            if (!RootPresent ||
                storageBackend == null)
            {
                return new SaveGenerationInspectionSnapshot(
                    SaveGenerationInspectionSnapshotStatus.RootMissing,
                    validatedSlot,
                    currentGenerationId,
                    Array.Empty<SaveGenerationInspectionEntry>(),
                    string.Empty,
                    "The Chronicle production save root does not exist; there are no generations to inspect.");
            }

            SaveStorageResult generationsKeyResult =
                SaveStorageKey.TryCreate(
                    "slots/" +
                    validatedSlot.Value +
                    "/generations",
                    out SaveStorageKey generationsKey);

            if (!generationsKeyResult.Succeeded)
            {
                return SnapshotFailure(
                    SaveGenerationInspectionSnapshotStatus.DiscoveryFailed,
                    validatedSlot,
                    currentGenerationId,
                    generationsKeyResult.DiagnosticCode,
                    generationsKeyResult.Message);
            }

            SaveStorageDiscoveryResult discovery =
                storageBackend.DiscoverChildDirectories(
                    generationsKey,
                    RuntimePolicy.Limits.RecoveryDiscoveryLimit);

            if (discovery.Status ==
                SaveStorageDiscoveryStatus.ParentNotFound)
            {
                return new SaveGenerationInspectionSnapshot(
                    SaveGenerationInspectionSnapshotStatus.SucceededEmpty,
                    validatedSlot,
                    currentGenerationId,
                    Array.Empty<SaveGenerationInspectionEntry>(),
                    string.Empty,
                    "The Chronicle slot has no committed-generation directory.");
            }

            if (discovery.Status ==
                SaveStorageDiscoveryStatus.LimitExceeded)
            {
                return SnapshotFailure(
                    SaveGenerationInspectionSnapshotStatus.DiscoveryLimitExceeded,
                    validatedSlot,
                    currentGenerationId,
                    discovery.DiagnosticCode,
                    discovery.Message);
            }

            if (!discovery.Succeeded)
            {
                return SnapshotFailure(
                    SaveGenerationInspectionSnapshotStatus.DiscoveryFailed,
                    validatedSlot,
                    currentGenerationId,
                    discovery.DiagnosticCode,
                    discovery.Message);
            }

            List<string> names =
                new List<string>(
                    discovery.ChildNames);

            names.Sort(
                StringComparer.Ordinal);

            List<SaveGenerationInspectionEntry> entries =
                new List<SaveGenerationInspectionEntry>(
                    names.Count);

            for (int i = 0;
                 i < names.Count;
                 i++)
            {
                entries.Add(
                    InspectGeneration(
                        validatedSlot,
                        names[i],
                        currentGenerationId));
            }

            return new SaveGenerationInspectionSnapshot(
                entries.Count == 0
                    ? SaveGenerationInspectionSnapshotStatus.SucceededEmpty
                    : SaveGenerationInspectionSnapshotStatus.Succeeded,
                validatedSlot,
                currentGenerationId,
                entries.ToArray(),
                string.Empty,
                "The Chronicle committed-generation inspection completed without mutating durable state.");
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            if (storageBackend != null)
            {
                storageBackend.Shutdown();
            }
        }

        private SaveGenerationInspectionEntry InspectGeneration(
            SaveSlotId slotId,
            string discoveredName,
            string currentGenerationId)
        {
            bool isCurrent =
                string.Equals(
                    discoveredName,
                    currentGenerationId,
                    StringComparison.Ordinal);

            if (!SaveGenerationId.TryParse(
                    discoveredName,
                    out SaveGenerationId generationId))
            {
                return EntryFailure(
                    discoveredName,
                    isCurrent,
                    SaveGenerationInspectionStatus.InvalidGenerationId,
                    EchoSaveDiagnosticCodes.StorageInvalidPath,
                    "The Chronicle discovered a generation directory with an invalid technical generation ID.");
            }

            SaveStorageResult keysResult =
                SaveGenerationStorageKeys.TryCreate(
                    slotId,
                    generationId,
                    out SaveGenerationStorageKeys keys);

            if (!keysResult.Succeeded)
            {
                return EntryFailure(
                    generationId.Value,
                    isCurrent,
                    SaveGenerationInspectionStatus.InvalidGenerationId,
                    keysResult.DiagnosticCode,
                    keysResult.Message);
            }

            SaveStorageReadResult manifestRead =
                storageBackend.Read(
                    keys.GenerationManifest);

            if (manifestRead.Result.Status ==
                SaveStorageStatus.NotFound)
            {
                return EntryFailure(
                    generationId.Value,
                    isCurrent,
                    SaveGenerationInspectionStatus.MissingManifest,
                    EchoSaveDiagnosticCodes.CatalogManifestMissing,
                    "The Chronicle committed generation is missing manifest.json.");
            }

            if (!manifestRead.Succeeded)
            {
                return EntryFailure(
                    generationId.Value,
                    isCurrent,
                    SaveGenerationInspectionStatus.BackendReadFailure,
                    manifestRead.Result.DiagnosticCode,
                    manifestRead.Result.Message);
            }

            string serialized =
                Encoding.UTF8.GetString(
                    manifestRead.Data);

            SavePackageDocumentVersionProbeResult probe =
                SavePackageDocumentVersionProbe.Probe(
                    serialized);

            string sourceVersion =
                probe.Succeeded
                    ? probe.Version.ToString()
                    : string.Empty;

            SavePackageDocumentVersionAuthority.TryGetCurrent(
                SaveDocumentKinds.Manifest,
                out SavePackageDocumentVersion currentVersion);

            SavePackageDocumentReadResult readResult =
                packageDocumentReader.ReadCurrent(
                    serialized,
                    SaveDocumentKinds.Manifest,
                    out SaveManifest manifest);

            if (!readResult.Succeeded)
            {
                return new SaveGenerationInspectionEntry(
                    generationId.Value,
                    isCurrent,
                    readResult.IsUnsupported
                        ? SaveGenerationInspectionStatus.UnsupportedManifest
                        : SaveGenerationInspectionStatus.InvalidManifest,
                    sourceVersion,
                    currentVersion.ToString(),
                    false,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    0,
                    0,
                    readResult.DiagnosticCode,
                    readResult.Message);
            }

            if (manifest == null ||
                !string.Equals(
                    manifest.slotId,
                    slotId.Value,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    manifest.generationId,
                    generationId.Value,
                    StringComparison.Ordinal))
            {
                return new SaveGenerationInspectionEntry(
                    generationId.Value,
                    isCurrent,
                    SaveGenerationInspectionStatus.IdentityMismatch,
                    readResult.SourceVersion.ToString(),
                    readResult.CurrentVersion.ToString(),
                    readResult.WasMigrated,
                    manifest == null
                        ? string.Empty
                        : manifest.commitState.ToString(),
                    manifest == null ? string.Empty : manifest.createdUtc,
                    manifest == null ? string.Empty : manifest.updatedUtc,
                    manifest == null ? string.Empty : manifest.displayName,
                    manifest == null ? string.Empty : manifest.saveKind,
                    manifest == null ? string.Empty : manifest.projectId,
                    manifest == null ? string.Empty : manifest.projectVersion,
                    manifest == null ? string.Empty : manifest.buildId,
                    manifest == null || manifest.payloadEntries == null
                        ? 0
                        : manifest.payloadEntries.Length,
                    manifest == null ? 0 : manifest.payloadByteLength,
                    EchoSaveDiagnosticCodes.CatalogIdentityMismatch,
                    "The Chronicle generation manifest identity does not match the discovered slot/generation path.");
            }

            if (manifest.commitState !=
                SaveGenerationCommitState.Committed)
            {
                return new SaveGenerationInspectionEntry(
                    generationId.Value,
                    isCurrent,
                    SaveGenerationInspectionStatus.InvalidManifest,
                    readResult.SourceVersion.ToString(),
                    readResult.CurrentVersion.ToString(),
                    readResult.WasMigrated,
                    manifest.commitState.ToString(),
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
                    manifest.payloadByteLength,
                    EchoSaveDiagnosticCodes.CatalogManifestInvalid,
                    "The Chronicle generation manifest is not marked committed.");
            }

            return new SaveGenerationInspectionEntry(
                generationId.Value,
                isCurrent,
                SaveGenerationInspectionStatus.Healthy,
                readResult.SourceVersion.ToString(),
                readResult.CurrentVersion.ToString(),
                readResult.WasMigrated,
                manifest.commitState.ToString(),
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
                manifest.payloadByteLength,
                string.Empty,
                "The Chronicle immutable generation manifest is readable and supported.");
        }

        private static SaveGenerationInspectionEntry EntryFailure(
            string generationId,
            bool isCurrent,
            SaveGenerationInspectionStatus status,
            string diagnosticCode,
            string message) =>
            new SaveGenerationInspectionEntry(
                generationId,
                isCurrent,
                status,
                string.Empty,
                CurrentManifestVersion(),
                false,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                0,
                0,
                diagnosticCode,
                message);

        private static string CurrentManifestVersion()
        {
            return SavePackageDocumentVersionAuthority.TryGetCurrent(
                    SaveDocumentKinds.Manifest,
                    out SavePackageDocumentVersion current)
                ? current.ToString()
                : string.Empty;
        }

        private static SaveGenerationInspectionSnapshot SnapshotFailure(
            SaveGenerationInspectionSnapshotStatus status,
            SaveSlotId slotId,
            string currentGenerationId,
            string diagnosticCode,
            string message) =>
            new SaveGenerationInspectionSnapshot(
                status,
                slotId,
                currentGenerationId,
                Array.Empty<SaveGenerationInspectionEntry>(),
                diagnosticCode,
                message);

        private static EchoSaveInspectionOpenResult Failure(
            string diagnosticCode,
            string message) =>
            new EchoSaveInspectionOpenResult(
                false,
                false,
                diagnosticCode,
                message);
    }
}
