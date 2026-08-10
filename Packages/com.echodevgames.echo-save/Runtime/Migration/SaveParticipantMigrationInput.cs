namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Detached transport-safe input for one explicit participant migration
    /// edge. The payload is in-memory only and does not grant storage authority.
    /// </summary>
    public readonly struct SaveParticipantMigrationInput
    {
        public SaveParticipantMigrationInput(
            SaveParticipantId persistedParticipantId,
            SaveParticipantId canonicalParticipantId,
            int sourceSchemaVersion,
            SaveSerializerId serializerId,
            string serializedPayload,
            bool required,
            int flags)
        {
            PersistedParticipantId =
                persistedParticipantId;

            CanonicalParticipantId =
                canonicalParticipantId;

            SourceSchemaVersion =
                sourceSchemaVersion;

            SerializerId =
                serializerId;

            SerializedPayload =
                serializedPayload;

            Required =
                required;

            Flags =
                flags;
        }

        public SaveParticipantId PersistedParticipantId { get; }

        public SaveParticipantId CanonicalParticipantId { get; }

        public int SourceSchemaVersion { get; }

        public SaveSerializerId SerializerId { get; }

        public string SerializedPayload { get; }

        public bool Required { get; }

        public int Flags { get; }
    }
}
