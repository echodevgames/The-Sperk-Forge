namespace EchoDevGames.EchoSave
{
    internal enum SaveParticipantMigrationPlanStatus
    {
        Succeeded = 0,
        InvalidRequest = 1,
        NewerSchemaUnsupported = 2,
        MissingEdge = 3,
        StepLimitExceeded = 4
    }
}
