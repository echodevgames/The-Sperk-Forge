
using System;
using UnityEngine;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Project-owned Chronicle configuration definition.
    ///
    /// ESV-M2-01 uses the configured storage-root directory name to resolve the
    /// default local Chronicle root beneath Application.persistentDataPath.
    /// The value remains a single safe relative directory segment.
    /// </summary>
    [CreateAssetMenu(
        fileName = "EchoSaveConfiguration",
        menuName = "EchoDevGames/The Chronicle/Echo Save Configuration")]
    public sealed class EchoSaveConfiguration : ScriptableObject
    {
        public const int CurrentSchemaVersion = 1;

        [SerializeField]
        private int schemaVersion = CurrentSchemaVersion;

        [SerializeField]
        private string storageRootDirectoryName = "EchoSave";

        public int SchemaVersion => schemaVersion;

        public string StorageRootDirectoryName =>
            storageRootDirectoryName ?? string.Empty;

        public bool IsCurrentSchema =>
            schemaVersion == CurrentSchemaVersion;

        internal bool TryValidate(
            out string message)
        {
            if (!IsCurrentSchema)
            {
                message =
                    $"EchoSaveConfiguration schema {schemaVersion} is unsupported. " +
                    $"Expected schema {CurrentSchemaVersion}.";
                return false;
            }

            string value = StorageRootDirectoryName.Trim();

            if (value.Length == 0)
            {
                message =
                    "The Chronicle storage-root directory name is empty.";
                return false;
            }

            if (value == "." ||
                value == ".." ||
                value.IndexOf('/') >= 0 ||
                value.IndexOf('\\') >= 0 ||
                value.IndexOf(':') >= 0)
            {
                message =
                    "The Chronicle storage-root directory name must be one safe relative directory segment.";
                return false;
            }

            for (int i = 0; i < value.Length; i++)
            {
                if (char.IsControl(value[i]))
                {
                    message =
                        "The Chronicle storage-root directory name contains a control character.";
                    return false;
                }
            }

            message = string.Empty;
            return true;
        }

        internal void SetDefinitionForTesting(
            int schema,
            string rootDirectoryName)
        {
            schemaVersion = schema;
            storageRootDirectoryName = rootDirectoryName;
        }
    }
}
