namespace EchoDevGames.EchoSave
{
    internal readonly struct SaveParticipantMigrationPlanStep
    {
        internal SaveParticipantMigrationPlanStep(
            ISaveParticipantMigrationStep step,
            SaveParticipantMigrationId migrationId,
            SaveParticipantId participantId,
            int fromSchemaVersion,
            int toSchemaVersion,
            long ownershipToken)
        {
            Step =
                step;

            MigrationId =
                migrationId;

            ParticipantId =
                participantId;

            FromSchemaVersion =
                fromSchemaVersion;

            ToSchemaVersion =
                toSchemaVersion;

            OwnershipToken =
                ownershipToken;
        }

        internal ISaveParticipantMigrationStep Step { get; }

        internal SaveParticipantMigrationId MigrationId { get; }

        internal SaveParticipantId ParticipantId { get; }

        internal int FromSchemaVersion { get; }

        internal int ToSchemaVersion { get; }

        internal long OwnershipToken { get; }
    }
}
