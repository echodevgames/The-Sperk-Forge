
using UnityEngine;

namespace EchoDevGames.EchoSave
{
    internal sealed class
        DefaultEchoSaveStorageBackendFactory :
        IEchoSaveStorageBackendFactory
    {
        internal static readonly
            DefaultEchoSaveStorageBackendFactory
                Instance =
                    new DefaultEchoSaveStorageBackendFactory();

        private DefaultEchoSaveStorageBackendFactory()
        {
        }

        public SaveStorageResult TryCreate(
            EchoSaveConfiguration configuration,
            out ISaveStorageBackend backend)
        {
            backend = null;

            SaveStorageResult rootResult =
                SaveStorageRootResolver
                    .TryResolveProductionRoot(
                        configuration,
                        Application.persistentDataPath,
                        out string rootPath);

            if (!rootResult.Succeeded)
            {
                return rootResult;
            }

            backend =
                new LocalFileSaveStorageBackend(
                    rootPath);

            return SaveStorageResult.Success(
                "The default Chronicle local storage backend was created.");
        }
    }
}
