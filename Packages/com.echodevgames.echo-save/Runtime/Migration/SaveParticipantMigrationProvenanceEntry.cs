namespace EchoDevGames.EchoSave
{
    internal readonly struct SaveParticipantMigrationProvenanceEntry
    {
        internal SaveParticipantMigrationProvenanceEntry(
            SaveParticipantMigrationId migrationId,
            int fromSchemaVersion,
            int toSchemaVersion)
        {
            MigrationId =
                migrationId;

            FromSchemaVersion =
                fromSchemaVersion;

            ToSchemaVersion =
                toSchemaVersion;
        }

        internal SaveParticipantMigrationId MigrationId { get; }

        internal int FromSchemaVersion { get; }

        internal int ToSchemaVersion { get; }
    }
}
