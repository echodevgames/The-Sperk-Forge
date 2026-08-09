
namespace EchoDevGames.EchoSave
{
    internal static class SaveStorageRootResolver
    {
        internal static SaveStorageResult
            TryResolveProductionRoot(
                EchoSaveConfiguration configuration,
                string persistentDataPath,
                out string rootPath)
        {
            rootPath =
                string.Empty;

            if (configuration == null)
            {
                return new SaveStorageResult(
                    SaveStorageStatus.Failed,
                    EchoSaveDiagnosticCodes
                        .MissingOrInvalidConfiguration,
                    "The Chronicle configuration is missing.");
            }

            if (!configuration.TryValidate(
                    out string validationMessage))
            {
                return new SaveStorageResult(
                    SaveStorageStatus.Failed,
                    EchoSaveDiagnosticCodes
                        .MissingOrInvalidConfiguration,
                    validationMessage);
            }

            SaveStorageResult keyResult =
                SaveStorageKey.TryCreate(
                    configuration
                        .StorageRootDirectoryName,
                    out SaveStorageKey rootKey);

            if (!keyResult.Succeeded)
            {
                return keyResult;
            }

            return SaveStoragePath
                .TryResolveUnderRoot(
                    persistentDataPath,
                    rootKey,
                    out rootPath);
        }
    }
}
