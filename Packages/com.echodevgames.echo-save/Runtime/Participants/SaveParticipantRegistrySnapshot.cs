
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Immutable bounded view of active participant descriptors.
    /// </summary>
    public sealed class SaveParticipantRegistrySnapshot
    {
        private readonly
            ReadOnlyCollection<SaveParticipantDescriptor>
                participants;

        internal SaveParticipantRegistrySnapshot(
            SaveParticipantDescriptor[] participants)
        {
            SaveParticipantDescriptor[] copy =
                participants == null
                    ? Array.Empty<SaveParticipantDescriptor>()
                    : (SaveParticipantDescriptor[])
                        participants.Clone();

            this.participants =
                Array.AsReadOnly(
                    copy);
        }

        public int Count =>
            participants.Count;

        public IReadOnlyList<SaveParticipantDescriptor>
            Participants =>
            participants;
    }
}
