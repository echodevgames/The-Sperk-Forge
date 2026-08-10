namespace EchoDevGames.EchoSave
{
    internal enum SaveParticipantPreparationStatus
    {
        Succeeded = 0,
        InvalidRequest = 1,
        ParticipantUnavailable = 2,
        RuntimeTypeUnavailable = 3,
        MigrationRequired = 4,
        NewerSchemaUnsupported = 5,
        SerializerUnavailable = 6,
        DeserializationFailed = 7,
        DetachedStateInvalid = 8,
        DuplicateCanonicalOwner = 9,
        MigrationChainUnavailable = 10,
        MigrationFailed = 11
    }
}
