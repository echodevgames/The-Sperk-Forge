
namespace EchoDevGames.EchoSave
{
    internal enum SaveGenerationPublicationStatus
    {
        Succeeded = 0,
        InvalidRequest = 1,
        BackendUnsupported = 2,
        ExistingHeadInvalid = 3,
        SerializationFailed = 4,
        CandidateWriteFailed = 5,
        CandidateVerificationFailed = 6,
        GenerationPublicationFailed = 7,
        HeadPublicationFailed = 8
    }
}
