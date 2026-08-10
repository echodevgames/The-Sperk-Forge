using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave
{
    internal sealed class SaveParticipantMigrationRegistrySnapshot
    {
        private readonly
            SaveParticipantMigrationDescriptor[]
                descriptors;

        internal SaveParticipantMigrationRegistrySnapshot(
            SaveParticipantMigrationDescriptor[] descriptors)
        {
            this.descriptors =
                descriptors == null
                    ? Array.Empty<
                        SaveParticipantMigrationDescriptor>()
                    : (SaveParticipantMigrationDescriptor[])
                        descriptors.Clone();
        }

        internal int Count =>
            descriptors.Length;

        internal IReadOnlyList<
            SaveParticipantMigrationDescriptor>
            Descriptors =>
            Array.AsReadOnly(
                (SaveParticipantMigrationDescriptor[])
                    descriptors.Clone());
    }
}
