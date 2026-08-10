
namespace EchoDevGames.EchoSave
{
    internal sealed class SaveParticipantApplyPlanStep
    {
        internal SaveParticipantApplyPlanStep(
            SaveParticipantId participantId,
            SaveParticipantApplyActionKind action,
            ISaveParticipant participant,
            long ownershipToken,
            object detachedState)
        {
            ParticipantId = participantId;
            Action = action;
            Participant = participant;
            OwnershipToken = ownershipToken;
            DetachedState = detachedState;
        }

        internal SaveParticipantId ParticipantId { get; }

        internal SaveParticipantApplyActionKind Action { get; }

        internal ISaveParticipant Participant { get; }

        internal long OwnershipToken { get; }

        internal object DetachedState { get; }
    }
}
