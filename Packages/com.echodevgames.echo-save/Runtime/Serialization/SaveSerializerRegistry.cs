
using System.Collections.Generic;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Package-local serializer-provider registry.
    ///
    /// The default registry contains UnityJsonSaveSerializer. Projects may
    /// register additional providers without replacing Chronicle authority.
    /// </summary>
    public sealed class SaveSerializerRegistry
    {
        private readonly Dictionary<
            SaveSerializerId,
            ISaveSerializer> serializers =
                new Dictionary<
                    SaveSerializerId,
                    ISaveSerializer>();

        public SaveSerializerRegistry()
            : this(registerDefaults: true)
        {
        }

        public SaveSerializerRegistry(
            bool registerDefaults)
        {
            if (registerDefaults)
            {
                TryRegister(
                    new UnityJsonSaveSerializer());
            }
        }

        public int Count =>
            serializers.Count;

        public SaveSerializerResult TryRegister(
            ISaveSerializer serializer)
        {
            if (serializer == null)
            {
                return new SaveSerializerResult(
                    SaveSerializerStatus.InvalidRequest,
                    EchoSaveDiagnosticCodes
                        .SerializerInvalidRequest,
                    "A serializer provider is required.");
            }

            if (serializers.ContainsKey(
                    serializer.Id))
            {
                return new SaveSerializerResult(
                    SaveSerializerStatus.DuplicateProvider,
                    EchoSaveDiagnosticCodes
                        .SerializerDuplicateProvider,
                    $"Serializer provider '{serializer.Id}' is already registered.");
            }

            serializers.Add(
                serializer.Id,
                serializer);

            return SaveSerializerResult.Success(
                $"Serializer provider '{serializer.Id}' was registered.");
        }

        public SaveSerializerResult TryResolve(
            SaveSerializerId id,
            out ISaveSerializer serializer)
        {
            if (serializers.TryGetValue(
                    id,
                    out serializer))
            {
                return SaveSerializerResult.Success(
                    $"Serializer provider '{id}' was resolved.");
            }

            serializer = null;

            return new SaveSerializerResult(
                SaveSerializerStatus.ProviderNotFound,
                EchoSaveDiagnosticCodes
                    .SerializerProviderNotFound,
                $"Serializer provider '{id}' is not registered.");
        }
    }
}
