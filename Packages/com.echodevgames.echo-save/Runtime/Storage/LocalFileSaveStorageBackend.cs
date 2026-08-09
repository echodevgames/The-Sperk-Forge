
using System;
using System.IO;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Default local filesystem storage provider.
    ///
    /// This backend stores opaque bytes beneath one validated root. It has no
    /// knowledge of Chronicle documents, slots, generations, or participants.
    /// </summary>
    public sealed class LocalFileSaveStorageBackend :
        ISaveStoragePublicationBackend
    {
        private static readonly
            SaveStorageBackendId BackendId =
                new SaveStorageBackendId(
                    "echodevgames.local-file");

        private bool initialized;

        public LocalFileSaveStorageBackend(
            string rootPath)
        {
            SaveStorageResult result =
                SaveStoragePath.TryNormalizeRoot(
                    rootPath,
                    out string normalizedRoot);

            if (!result.Succeeded)
            {
                throw new ArgumentException(
                    result.Message,
                    nameof(rootPath));
            }

            RootPath =
                normalizedRoot;
        }

        public SaveStorageBackendId Id =>
            BackendId;

        public string RootPath { get; }

        public SaveStoragePublicationCapabilities
            PublicationCapabilities =>
            new SaveStoragePublicationCapabilities(
                supportsNewTreePublication: true,
                supportsCurrentObjectPublication: true,
                usesSameRootDirectoryMove: true,
                usesNativeReplaceForExistingCurrent: true,
                claimsPowerLossAtomicity: false);

        public SaveStorageResult Initialize()
        {
            if (initialized)
            {
                return SaveStorageResult.NoChange(
                    "The Chronicle local storage backend is already initialized.");
            }

            try
            {
                Directory.CreateDirectory(
                    RootPath);

                initialized = true;

                return SaveStorageResult.Success(
                    "The Chronicle local storage root is ready.");
            }
            catch (Exception exception)
                when (IsExpectedIoException(
                    exception))
            {
                initialized = false;

                return IoFailure(
                    EchoSaveDiagnosticCodes
                        .StorageInitializationFailed,
                    "The Chronicle local storage root could not be created.",
                    exception);
            }
        }

        public SaveStorageResult Exists(
            SaveStorageKey key,
            out bool exists)
        {
            exists = false;

            SaveStorageResult ready =
                EnsureReady();

            if (!ready.Succeeded)
            {
                return ready;
            }

            SaveStorageResult resolved =
                Resolve(
                    key,
                    out string fullPath);

            if (!resolved.Succeeded)
            {
                return resolved;
            }

            try
            {
                exists =
                    File.Exists(
                        fullPath);

                return SaveStorageResult.Success(
                    exists
                        ? "The Chronicle storage object exists."
                        : "The Chronicle storage object does not exist.");
            }
            catch (Exception exception)
                when (IsExpectedIoException(
                    exception))
            {
                exists = false;

                return IoFailure(
                    EchoSaveDiagnosticCodes
                        .StorageIoFailure,
                    "The Chronicle storage existence check failed.",
                    exception);
            }
        }

        public SaveStorageReadResult Read(
            SaveStorageKey key)
        {
            SaveStorageResult ready =
                EnsureReady();

            if (!ready.Succeeded)
            {
                return new SaveStorageReadResult(
                    ready,
                    null);
            }

            SaveStorageResult resolved =
                Resolve(
                    key,
                    out string fullPath);

            if (!resolved.Succeeded)
            {
                return new SaveStorageReadResult(
                    resolved,
                    null);
            }

            try
            {
                if (!File.Exists(
                        fullPath))
                {
                    return new SaveStorageReadResult(
                        new SaveStorageResult(
                            SaveStorageStatus.NotFound,
                            EchoSaveDiagnosticCodes
                                .StorageNotFound,
                            "The requested Chronicle storage object was not found."),
                        null);
                }

                byte[] bytes =
                    File.ReadAllBytes(
                        fullPath);

                return new SaveStorageReadResult(
                    SaveStorageResult.Success(
                        "The Chronicle storage object was read successfully."),
                    bytes);
            }
            catch (Exception exception)
                when (IsExpectedIoException(
                    exception))
            {
                return new SaveStorageReadResult(
                    IoFailure(
                        EchoSaveDiagnosticCodes
                            .StorageIoFailure,
                        "The Chronicle storage read failed.",
                        exception),
                    null);
            }
        }

        public SaveStorageResult WriteNew(
            SaveStorageKey key,
            byte[] data)
        {
            SaveStorageResult ready =
                EnsureReady();

            if (!ready.Succeeded)
            {
                return ready;
            }

            if (data == null)
            {
                return new SaveStorageResult(
                    SaveStorageStatus.Failed,
                    EchoSaveDiagnosticCodes
                        .StorageInvalidData,
                    "Chronicle storage cannot write a null byte payload.");
            }

            SaveStorageResult resolved =
                Resolve(
                    key,
                    out string fullPath);

            if (!resolved.Succeeded)
            {
                return resolved;
            }

            bool createdFile =
                false;

            try
            {
                string directory =
                    Path.GetDirectoryName(
                        fullPath);

                if (string.IsNullOrEmpty(
                        directory))
                {
                    return new SaveStorageResult(
                        SaveStorageStatus.InvalidPath,
                        EchoSaveDiagnosticCodes
                            .StorageInvalidPath,
                        "The Chronicle storage object has no containing directory.");
                }

                Directory.CreateDirectory(
                    directory);

                using (FileStream stream =
                    new FileStream(
                        fullPath,
                        FileMode.CreateNew,
                        FileAccess.Write,
                        FileShare.None))
                {
                    createdFile = true;
                    stream.Write(
                        data,
                        0,
                        data.Length);
                    stream.Flush();
                }

                return SaveStorageResult.Success(
                    "The Chronicle storage object was created successfully.");
            }
            catch (IOException exception)
            {
                if (!createdFile &&
                    File.Exists(
                        fullPath))
                {
                    return new SaveStorageResult(
                        SaveStorageStatus.Conflict,
                        EchoSaveDiagnosticCodes
                            .StorageConflict,
                        "The Chronicle storage object already exists.");
                }

                TryRemoveFailedCreation(
                    fullPath,
                    createdFile);

                return IoFailure(
                    EchoSaveDiagnosticCodes
                        .StorageIoFailure,
                    "The Chronicle storage write failed.",
                    exception);
            }
            catch (Exception exception)
                when (IsExpectedIoException(
                    exception))
            {
                TryRemoveFailedCreation(
                    fullPath,
                    createdFile);

                return IoFailure(
                    EchoSaveDiagnosticCodes
                        .StorageIoFailure,
                    "The Chronicle storage write failed.",
                    exception);
            }
        }

        public SaveStorageResult Delete(
            SaveStorageKey key)
        {
            SaveStorageResult ready =
                EnsureReady();

            if (!ready.Succeeded)
            {
                return ready;
            }

            SaveStorageResult resolved =
                Resolve(
                    key,
                    out string fullPath);

            if (!resolved.Succeeded)
            {
                return resolved;
            }

            try
            {
                if (!File.Exists(
                        fullPath))
                {
                    return new SaveStorageResult(
                        SaveStorageStatus.NotFound,
                        EchoSaveDiagnosticCodes
                            .StorageNotFound,
                        "The requested Chronicle storage object was not found.");
                }

                File.Delete(
                    fullPath);

                return SaveStorageResult.Success(
                    "The Chronicle storage object was deleted successfully.");
            }
            catch (Exception exception)
                when (IsExpectedIoException(
                    exception))
            {
                return IoFailure(
                    EchoSaveDiagnosticCodes
                        .StorageIoFailure,
                    "The Chronicle storage delete failed.",
                    exception);
            }
        }


        public SaveStorageResult PublishNewTree(
            SaveStorageKey sourceDirectoryKey,
            SaveStorageKey destinationDirectoryKey)
        {
            SaveStorageResult ready =
                EnsureReady();

            if (!ready.Succeeded)
            {
                return ready;
            }

            SaveStorageResult sourceResolved =
                Resolve(
                    sourceDirectoryKey,
                    out string sourcePath);

            if (!sourceResolved.Succeeded)
            {
                return sourceResolved;
            }

            SaveStorageResult destinationResolved =
                Resolve(
                    destinationDirectoryKey,
                    out string destinationPath);

            if (!destinationResolved.Succeeded)
            {
                return destinationResolved;
            }

            if (string.Equals(
                    sourcePath,
                    destinationPath,
                    Path.DirectorySeparatorChar == '\\'
                        ? StringComparison.OrdinalIgnoreCase
                        : StringComparison.Ordinal))
            {
                return new SaveStorageResult(
                    SaveStorageStatus.InvalidPath,
                    EchoSaveDiagnosticCodes
                        .StorageInvalidPath,
                    "Chronicle publication source and destination directories must differ.");
            }

            try
            {
                if (!Directory.Exists(
                        sourcePath))
                {
                    return new SaveStorageResult(
                        SaveStorageStatus.NotFound,
                        EchoSaveDiagnosticCodes
                            .StorageNotFound,
                        "The Chronicle publication source directory was not found.");
                }

                if (Directory.Exists(
                        destinationPath) ||
                    File.Exists(
                        destinationPath))
                {
                    return new SaveStorageResult(
                        SaveStorageStatus.Conflict,
                        EchoSaveDiagnosticCodes
                            .StorageConflict,
                        "The Chronicle publication destination already exists.");
                }

                string destinationParent =
                    Path.GetDirectoryName(
                        destinationPath);

                if (string.IsNullOrEmpty(
                        destinationParent))
                {
                    return new SaveStorageResult(
                        SaveStorageStatus.InvalidPath,
                        EchoSaveDiagnosticCodes
                            .StorageInvalidPath,
                        "The Chronicle publication destination has no containing directory.");
                }

                Directory.CreateDirectory(
                    destinationParent);

                Directory.Move(
                    sourcePath,
                    destinationPath);

                return SaveStorageResult.Success(
                    "The Chronicle storage tree was published through a same-root directory move.");
            }
            catch (Exception exception)
                when (IsExpectedIoException(
                    exception))
            {
                return IoFailure(
                    EchoSaveDiagnosticCodes
                        .StoragePublicationFailed,
                    "The Chronicle storage tree publication failed.",
                    exception);
            }
        }

        public SaveStorageResult PublishCurrentObject(
            SaveStorageKey key,
            byte[] data)
        {
            SaveStorageResult ready =
                EnsureReady();

            if (!ready.Succeeded)
            {
                return ready;
            }

            if (data == null)
            {
                return new SaveStorageResult(
                    SaveStorageStatus.Failed,
                    EchoSaveDiagnosticCodes
                        .StorageInvalidData,
                    "Chronicle current-object publication cannot write a null byte payload.");
            }

            SaveStorageResult resolved =
                Resolve(
                    key,
                    out string fullPath);

            if (!resolved.Succeeded)
            {
                return resolved;
            }

            string pendingPath =
                string.Empty;

            try
            {
                string directory =
                    Path.GetDirectoryName(
                        fullPath);

                if (string.IsNullOrEmpty(
                        directory))
                {
                    return new SaveStorageResult(
                        SaveStorageStatus.InvalidPath,
                        EchoSaveDiagnosticCodes
                            .StorageInvalidPath,
                        "The Chronicle current object has no containing directory.");
                }

                Directory.CreateDirectory(
                    directory);

                pendingPath =
                    fullPath +
                    ".chronicle-pending-" +
                    Guid.NewGuid()
                        .ToString("N") +
                    ".tmp";

                using (FileStream stream =
                    new FileStream(
                        pendingPath,
                        FileMode.CreateNew,
                        FileAccess.Write,
                        FileShare.None))
                {
                    stream.Write(
                        data,
                        0,
                        data.Length);
                    stream.Flush();
                }

                if (File.Exists(
                        fullPath))
                {
                    File.Replace(
                        pendingPath,
                        fullPath,
                        null);
                }
                else
                {
                    File.Move(
                        pendingPath,
                        fullPath);
                }

                pendingPath =
                    string.Empty;

                return SaveStorageResult.Success(
                    "The Chronicle current object was published through the platform file move/replace primitive.");
            }
            catch (PlatformNotSupportedException exception)
            {
                TryRemovePendingPublication(
                    pendingPath);

                return IoFailure(
                    EchoSaveDiagnosticCodes
                        .StoragePublicationUnsupported,
                    "The Chronicle platform does not support the requested current-object replacement primitive.",
                    exception);
            }
            catch (Exception exception)
                when (IsExpectedIoException(
                    exception))
            {
                TryRemovePendingPublication(
                    pendingPath);

                return IoFailure(
                    EchoSaveDiagnosticCodes
                        .StoragePublicationFailed,
                    "The Chronicle current-object publication failed.",
                    exception);
            }
        }

        public SaveStorageResult Shutdown()
        {
            if (!initialized)
            {
                return SaveStorageResult.NoChange(
                    "The Chronicle local storage backend is already stopped.");
            }

            initialized = false;

            return SaveStorageResult.Success(
                "The Chronicle local storage backend shut down cleanly.");
        }

        private SaveStorageResult EnsureReady()
        {
            if (initialized)
            {
                return SaveStorageResult.Success(
                    "The Chronicle local storage backend is ready.");
            }

            return new SaveStorageResult(
                SaveStorageStatus.Failed,
                EchoSaveDiagnosticCodes
                    .StorageNotReady,
                "The Chronicle local storage backend is not initialized.");
        }

        private SaveStorageResult Resolve(
            SaveStorageKey key,
            out string fullPath) =>
            SaveStoragePath.TryResolveUnderRoot(
                RootPath,
                key,
                out fullPath);

        private static SaveStorageResult IoFailure(
            string diagnosticCode,
            string message,
            Exception exception) =>
            new SaveStorageResult(
                SaveStorageStatus.Failed,
                diagnosticCode,
                $"{message} {exception.GetType().Name}: {exception.Message}");

        private static bool IsExpectedIoException(
            Exception exception) =>
            exception is IOException ||
            exception is UnauthorizedAccessException ||
            exception is ArgumentException ||
            exception is NotSupportedException ||
            exception is PathTooLongException;


        private static void TryRemovePendingPublication(
            string pendingPath)
        {
            if (string.IsNullOrEmpty(
                    pendingPath))
            {
                return;
            }

            try
            {
                if (File.Exists(
                        pendingPath))
                {
                    File.Delete(
                        pendingPath);
                }
            }
            catch
            {
                // Pending publication material is never current authority.
                // Later recovery/quarantine work owns policy for leftovers.
            }
        }

        private static void TryRemoveFailedCreation(
            string fullPath,
            bool createdFile)
        {
            if (!createdFile)
            {
                return;
            }

            try
            {
                if (File.Exists(
                        fullPath))
                {
                    File.Delete(
                        fullPath);
                }
            }
            catch
            {
                // The failed candidate remains untrusted. A later recovery
                // checkpoint will own quarantine/cleanup policy.
            }
        }
    }
}
