
using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Deterministic in-memory participant mutation executor.
    ///
    /// No storage write, scene travel, rollback, or compensation authority is
    /// owned here.
    /// </summary>
    internal sealed class SaveParticipantApplyExecutor
    {
        private readonly SavePreparedLoadStore preparedLoadStore;
        private readonly SaveParticipantRegistry participantRegistry;

        internal SaveParticipantApplyExecutor(
            SavePreparedLoadStore preparedLoadStore,
            SaveParticipantRegistry participantRegistry)
        {
            this.preparedLoadStore =
                preparedLoadStore ??
                throw new ArgumentNullException(
                    nameof(preparedLoadStore));

            this.participantRegistry =
                participantRegistry ??
                throw new ArgumentNullException(
                    nameof(participantRegistry));
        }

        internal SavePreparedLoadApplyResult Execute(
            SaveParticipantApplyPlan plan,
            PreparedSaveLoad handle)
        {
            if (plan == null ||
                handle == null ||
                !preparedLoadStore.TryGetPreparedParticipantBatch(
                    handle,
                    out _) ||
                handle.SourceSlotId !=
                    plan.SourceSlotId ||
                handle.SourceGenerationId !=
                    plan.SourceGenerationId)
            {
                return Result(
                    SavePreparedLoadApplyStatus.HandleUnavailable,
                    handle,
                    false,
                    false,
                    default,
                    EchoSaveDiagnosticCodes.PreparedApplyHandleUnavailable,
                    "The Chronicle prepared-load handle is unavailable or does not match the apply plan.",
                    Array.Empty<SaveParticipantApplyReportEntry>());
            }

            if (!TryRevalidateAll(
                    plan,
                    out SaveParticipantId staleParticipant))
            {
                return Result(
                    SavePreparedLoadApplyStatus.RegistryChanged,
                    handle,
                    false,
                    false,
                    staleParticipant,
                    EchoSaveDiagnosticCodes.PreparedApplyRegistryChanged,
                    "Chronicle participant registration ownership changed after apply preflight and before mutation.",
                    NotAttempted(
                        plan,
                        0));
            }

            List<SaveParticipantApplyReportEntry> reports =
                new List<SaveParticipantApplyReportEntry>(
                    plan.Count);

            bool mutationBegan =
                false;

            IReadOnlyList<SaveParticipantApplyPlanStep>
                steps =
                    plan.Steps;

            for (int i = 0;
                 i < steps.Count;
                 i++)
            {
                SaveParticipantApplyPlanStep step =
                    steps[i];

                if (step.Action ==
                    SaveParticipantApplyActionKind.Ignore)
                {
                    reports.Add(
                        Report(
                            step,
                            SaveParticipantApplyOutcome.Ignored,
                            string.Empty,
                            "The Chronicle participant missing payload was ignored by policy."));

                    continue;
                }

                if (!participantRegistry.Owns(
                        step.ParticipantId,
                        step.OwnershipToken))
                {
                    bool consumed =
                        mutationBegan &&
                        Consume(
                            handle);

                    reports.Add(
                        Report(
                            step,
                            SaveParticipantApplyOutcome.Failed,
                            EchoSaveDiagnosticCodes.PreparedApplyRegistryChanged,
                            "Chronicle participant registration ownership changed before this mutating callback."));

                    AppendNotAttemptedTail(
                        reports,
                        steps,
                        i + 1);

                    return Result(
                        SavePreparedLoadApplyStatus.RegistryChanged,
                        handle,
                        mutationBegan,
                        consumed,
                        step.ParticipantId,
                        EchoSaveDiagnosticCodes.PreparedApplyRegistryChanged,
                        "Chronicle participant registration changed during deterministic apply execution.",
                        reports.ToArray());
                }

                mutationBegan =
                    true;

                SaveParticipantApplyResult participantResult;

                try
                {
                    participantResult =
                        step.Action ==
                            SaveParticipantApplyActionKind.ApplyPreparedState
                            ? step.Participant.Apply(
                                step.DetachedState)
                            : ((ISaveDefaultableParticipant)
                                step.Participant)
                                .InitializeDefault();
                }
                catch (Exception exception)
                {
                    bool consumed =
                        Consume(
                            handle);

                    reports.Add(
                        Report(
                            step,
                            SaveParticipantApplyOutcome.Failed,
                            EchoSaveDiagnosticCodes.PreparedApplyParticipantException,
                            $"The Chronicle participant callback threw {exception.GetType().Name}: {exception.Message}"));

                    AppendNotAttemptedTail(
                        reports,
                        steps,
                        i + 1);

                    return Result(
                        SavePreparedLoadApplyStatus.ParticipantException,
                        handle,
                        true,
                        consumed,
                        step.ParticipantId,
                        EchoSaveDiagnosticCodes.PreparedApplyParticipantException,
                        "A Chronicle participant callback threw during deterministic prepared-load apply.",
                        reports.ToArray());
                }

                if (!participantResult.Succeeded)
                {
                    bool consumed =
                        Consume(
                            handle);

                    string diagnosticCode =
                        string.IsNullOrEmpty(
                            participantResult.DiagnosticCode)
                            ? EchoSaveDiagnosticCodes
                                .PreparedApplyParticipantFailed
                            : participantResult
                                .DiagnosticCode;

                    reports.Add(
                        Report(
                            step,
                            SaveParticipantApplyOutcome.Failed,
                            diagnosticCode,
                            participantResult.Message));

                    AppendNotAttemptedTail(
                        reports,
                        steps,
                        i + 1);

                    return Result(
                        SavePreparedLoadApplyStatus.ParticipantFailed,
                        handle,
                        true,
                        consumed,
                        step.ParticipantId,
                        diagnosticCode,
                        "A Chronicle participant reported apply failure.",
                        reports.ToArray());
                }

                reports.Add(
                    Report(
                        step,
                        step.Action ==
                            SaveParticipantApplyActionKind.ApplyPreparedState
                            ? SaveParticipantApplyOutcome.Applied
                            : SaveParticipantApplyOutcome.DefaultInitialized,
                        participantResult.DiagnosticCode,
                        participantResult.Message));
            }

            bool handleConsumed =
                Consume(
                    handle);

            if (!handleConsumed)
            {
                return Result(
                    SavePreparedLoadApplyStatus.HandleUnavailable,
                    handle,
                    mutationBegan,
                    handle.State ==
                        PreparedLoadState.Consumed,
                    default,
                    EchoSaveDiagnosticCodes.PreparedApplyHandleUnavailable,
                    "Chronicle completed the participant apply plan but could not consume the prepared-load handle.",
                    reports.ToArray());
            }

            return Result(
                SavePreparedLoadApplyStatus.Succeeded,
                handle,
                mutationBegan,
                true,
                default,
                string.Empty,
                "The Chronicle prepared load applied successfully.",
                reports.ToArray());
        }

        private bool TryRevalidateAll(
            SaveParticipantApplyPlan plan,
            out SaveParticipantId staleParticipant)
        {
            staleParticipant =
                default;

            IReadOnlyList<SaveParticipantApplyPlanStep>
                steps =
                    plan.Steps;

            for (int i = 0;
                 i < steps.Count;
                 i++)
            {
                SaveParticipantApplyPlanStep step =
                    steps[i];

                if (!participantRegistry.Owns(
                        step.ParticipantId,
                        step.OwnershipToken))
                {
                    staleParticipant =
                        step.ParticipantId;

                    return false;
                }
            }

            return true;
        }

        private bool Consume(
            PreparedSaveLoad handle) =>
            preparedLoadStore.ReleaseOwned(
                handle,
                handle.OwnershipToken,
                handle.OwnerEpoch,
                PreparedLoadState.Consumed);

        private static void AppendNotAttemptedTail(
            List<SaveParticipantApplyReportEntry> reports,
            IReadOnlyList<SaveParticipantApplyPlanStep> steps,
            int startIndex)
        {
            for (int i = startIndex;
                 i < steps.Count;
                 i++)
            {
                reports.Add(
                    Report(
                        steps[i],
                        SaveParticipantApplyOutcome.NotAttempted,
                        string.Empty,
                        "This Chronicle participant action was not attempted after an earlier terminal failure."));
            }
        }

        private static SaveParticipantApplyReportEntry[]
            NotAttempted(
                SaveParticipantApplyPlan plan,
                int startIndex)
        {
            List<SaveParticipantApplyReportEntry> reports =
                new List<SaveParticipantApplyReportEntry>();

            AppendNotAttemptedTail(
                reports,
                plan.Steps,
                startIndex);

            return reports.ToArray();
        }

        private static SaveParticipantApplyReportEntry
            Report(
                SaveParticipantApplyPlanStep step,
                SaveParticipantApplyOutcome outcome,
                string diagnosticCode,
                string message) =>
            new SaveParticipantApplyReportEntry(
                step.ParticipantId,
                step.Action,
                outcome,
                diagnosticCode,
                message);

        private static SavePreparedLoadApplyResult
            Result(
                SavePreparedLoadApplyStatus status,
                PreparedSaveLoad handle,
                bool mutationBegan,
                bool handleConsumed,
                SaveParticipantId failureParticipantId,
                string diagnosticCode,
                string message,
                SaveParticipantApplyReportEntry[] reports) =>
            new SavePreparedLoadApplyResult(
                status,
                handle == null
                    ? default
                    : handle.SourceSlotId,
                handle == null
                    ? default
                    : handle.SourceGenerationId,
                mutationBegan,
                handleConsumed,
                failureParticipantId,
                diagnosticCode,
                message,
                reports);
    }
}
