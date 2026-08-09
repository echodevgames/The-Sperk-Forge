
using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Manifest-only transport inventory record. It intentionally excludes the
    /// serialized gameplay payload body.
    /// </summary>
    [Serializable]
    public sealed class SavePayloadInventoryEntry
    {
        public string participantId =
            string.Empty;

        public int participantSchemaVersion;

        public string serializerId =
            UnityJsonSaveSerializer.StableId;

        public bool required;

        public long byteLength;

        public string checksum =
            string.Empty;

        public int flags;
    }
}
