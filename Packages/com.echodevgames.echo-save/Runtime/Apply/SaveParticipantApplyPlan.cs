
using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave
{
    internal sealed class SaveParticipantApplyPlan
    {
        private readonly SaveParticipantApplyPlanStep[] steps;

        internal SaveParticipantApplyPlan(
            SaveSlotId sourceSlotId,
            SaveGenerationId sourceGenerationId,
            SaveParticipantApplyPlanStep[] steps)
        {
            SourceSlotId = sourceSlotId;
            SourceGenerationId = sourceGenerationId;
            this.steps =
                steps == null
                    ? Array.Empty<SaveParticipantApplyPlanStep>()
                    : (SaveParticipantApplyPlanStep[])
                        steps.Clone();
        }

        internal SaveSlotId SourceSlotId { get; }

        internal SaveGenerationId SourceGenerationId { get; }

        internal int Count =>
            steps.Length;

        internal IReadOnlyList<SaveParticipantApplyPlanStep>
            Steps =>
            Array.AsReadOnly(
                (SaveParticipantApplyPlanStep[])
                    steps.Clone());
    }
}
