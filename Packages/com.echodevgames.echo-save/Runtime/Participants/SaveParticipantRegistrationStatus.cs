
namespace EchoDevGames.EchoSave
{
    public enum SaveParticipantRegistrationStatus
    {
        Succeeded = 0,
        InvalidParticipant = 1,
        InvalidDescriptor = 2,
        DuplicateId = 3,
        AliasCollision = 4,
        ServiceNotReady = 5,
        AdmissionClosed = 6
    }
}
