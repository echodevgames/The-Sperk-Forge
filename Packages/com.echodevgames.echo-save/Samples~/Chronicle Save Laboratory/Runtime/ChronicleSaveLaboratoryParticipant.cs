
using System;

namespace EchoDevGames.EchoSave.Samples.ChronicleLaboratory
{
    public sealed class ChronicleSaveLaboratoryParticipant :
        ISaveTypedParticipant
    {
        public const string StableParticipantId =
            "com.echodevgames.chronicle-laboratory.sperk-subject";

        private readonly ChronicleSaveLaboratoryState state;

        public ChronicleSaveLaboratoryParticipant(
            ChronicleSaveLaboratoryState state)
        {
            this.state =
                state ??
                throw new ArgumentNullException(
                    nameof(state));

            Descriptor =
                new SaveParticipantDescriptor(
                    new SaveParticipantId(
                        StableParticipantId),
                    1,
                    SaveParticipantCriticality.Required,
                    SaveMissingPayloadPolicy.Fail,
                    default);
        }

        public SaveParticipantDescriptor Descriptor { get; }

        public Type DetachedStateType =>
            typeof(ChronicleSaveLaboratoryState);

        public SaveParticipantCaptureResult Capture() =>
            SaveParticipantCaptureResult.Success(
                state.Clone());

        public SaveParticipantApplyResult Apply(
            object detachedState)
        {
            if (!(detachedState is
                ChronicleSaveLaboratoryState saved))
            {
                return SaveParticipantApplyResult.Failure(
                    "Chronicle Laboratory expected its tiny detached Sperk state.");
            }

            state.CopyFrom(
                saved);

            return SaveParticipantApplyResult.Success();
        }
    }
}
