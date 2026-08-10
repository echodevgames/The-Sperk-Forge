namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Explicit project/package-owned migration edge for one Chronicle
    /// participant. Every step must cover exactly one vN -> vN+1 edge.
    /// </summary>
    public interface ISaveParticipantMigrationStep
    {
        SaveParticipantMigrationId Id { get; }

        SaveParticipantId ParticipantId { get; }

        int FromSchemaVersion { get; }

        int ToSchemaVersion { get; }

        SaveParticipantMigrationStepResult Migrate(
            SaveParticipantMigrationInput input);
    }
}
