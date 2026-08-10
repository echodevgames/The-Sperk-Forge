namespace EchoDevGames.EchoSave
{
    internal readonly struct SaveParticipantMigrationDescriptor
    {
        internal SaveParticipantMigrationDescriptor(
            SaveParticipantMigrationId id,
            SaveParticipantId participantId,
            int fromSchemaVersion,
            int toSchemaVersion)
        {
            Id =
                id;

            ParticipantId =
                participantId;

            FromSchemaVersion =
                fromSchemaVersion;

            ToSchemaVersion =
                toSchemaVersion;
        }

        internal SaveParticipantMigrationId Id { get; }

        internal SaveParticipantId ParticipantId { get; }

        internal int FromSchemaVersion { get; }

        internal int ToSchemaVersion { get; }
    }
}
