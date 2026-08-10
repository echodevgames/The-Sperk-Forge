using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave
{
    internal sealed class SaveParticipantMigrationPlan
    {
        private readonly
            SaveParticipantMigrationPlanStep[]
                steps;

        internal SaveParticipantMigrationPlan(
            SaveParticipantId participantId,
            int sourceSchemaVersion,
            int targetSchemaVersion,
            SaveParticipantMigrationPlanStep[] steps)
        {
            ParticipantId =
                participantId;

            SourceSchemaVersion =
                sourceSchemaVersion;

            TargetSchemaVersion =
                targetSchemaVersion;

            this.steps =
                steps == null
                    ? Array.Empty<
                        SaveParticipantMigrationPlanStep>()
                    : (SaveParticipantMigrationPlanStep[])
                        steps.Clone();
        }

        internal SaveParticipantId ParticipantId { get; }

        internal int SourceSchemaVersion { get; }

        internal int TargetSchemaVersion { get; }

        internal int Count =>
            steps.Length;

        internal IReadOnlyList<
            SaveParticipantMigrationPlanStep>
            Steps =>
            Array.AsReadOnly(
                (SaveParticipantMigrationPlanStep[])
                    steps.Clone());
    }
}
