
using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Opaque Chronicle transport entry.
    ///
    /// M2-03 defines the package document shape only. No participant registry,
    /// capture/apply behavior, project DTO binding, or gameplay schema meaning
    /// is activated by this type.
    /// </summary>
    [Serializable]
    public sealed class SavePayloadEntry
    {
        public string participantId =
            string.Empty;

        public int participantSchemaVersion;

        public string serializerId =
            UnityJsonSaveSerializer.StableId;

        public bool required;

        public string serializedPayload =
            string.Empty;

        public string byteProviderReference =
            string.Empty;

        public long byteLength;

        public string checksum =
            string.Empty;

        public int flags;
    }
}
