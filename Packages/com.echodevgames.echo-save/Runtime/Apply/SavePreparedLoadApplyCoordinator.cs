
using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Bounded synchronous foundation that joins complete apply preflight with
    /// deterministic participant mutation execution.
    ///
    /// Production async operation admission remains a later checkpoint.
    /// </summary>
    internal sealed class SavePreparedLoadApplyCoordinator
    {
        private readonly SaveParticipantApplyPlanner planner;
        private readonly SaveParticipantApplyExecutor executor;

        internal SavePreparedLoadApplyCoordinator(
            SavePreparedLoadStore preparedLoadStore,
            SaveParticipantRegistry participantRegistry)
        {
            if (preparedLoadStore == null)
            {
                throw new ArgumentNullException(
                    nameof(preparedLoadStore));
            }

            if (participantRegistry == null)
            {
                throw new ArgumentNullException(
                    nameof(participantRegistry));
            }

            planner =
                new SaveParticipantApplyPlanner(
                    preparedLoadStore,
                    participantRegistry);

            executor =
                new SaveParticipantApplyExecutor(
                    preparedLoadStore,
                    participantRegistry);
        }

        internal SavePreparedLoadApplyResult Apply(
            PreparedSaveLoad handle)
        {
            SaveParticipantApplyPlanResult planResult =
                planner.Plan(
                    handle);

            if (!planResult.Succeeded)
            {
                return new SavePreparedLoadApplyResult(
                    planResult.Status ==
                        SaveParticipantApplyPlanStatus.HandleUnavailable
                        ? SavePreparedLoadApplyStatus.HandleUnavailable
                        : SavePreparedLoadApplyStatus.PreflightRejected,
                    handle == null
                        ? default
                        : handle.SourceSlotId,
                    handle == null
                        ? default
                        : handle.SourceGenerationId,
                    false,
                    false,
                    planResult.FailureParticipantId,
                    planResult.DiagnosticCode,
                    planResult.Message,
                    Array.Empty<SaveParticipantApplyReportEntry>());
            }

            return executor.Execute(
                planResult.Plan,
                handle);
        }
    }
}
