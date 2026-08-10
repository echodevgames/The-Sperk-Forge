
using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Complete zero-callback preflight for one prepared-load apply attempt.
    /// </summary>
    internal sealed class SaveParticipantApplyPlanner
    {
        private readonly SavePreparedLoadStore preparedLoadStore;
        private readonly SaveParticipantRegistry participantRegistry;

        internal SaveParticipantApplyPlanner(
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

        internal SaveParticipantApplyPlanResult Plan(
            PreparedSaveLoad handle)
        {
            if (handle == null)
            {
                return Failure(
                    SaveParticipantApplyPlanStatus.InvalidRequest,
                    default,
                    EchoSaveDiagnosticCodes.PreparedApplyInvalidRequest,
                    "A Chronicle prepared-load handle is required.");
            }

            if (!preparedLoadStore.TryGetPreparedParticipantBatch(
                    handle,
                    out SavePreparedParticipantBatch preparedBatch) ||
                preparedBatch == null)
            {
                return Failure(
                    SaveParticipantApplyPlanStatus.HandleUnavailable,
                    default,
                    EchoSaveDiagnosticCodes.PreparedApplyHandleUnavailable,
                    "The Chronicle prepared-load handle is not live and owned by this prepared-load store.");
            }

            SortedDictionary<string, SavePreparedParticipantEntry>
                preparedByCanonical =
                    new SortedDictionary<string, SavePreparedParticipantEntry>(
                        StringComparer.Ordinal);

            IReadOnlyList<SavePreparedParticipantEntry>
                preparedEntries =
                    preparedBatch.Entries;

            for (int i = 0;
                 i < preparedEntries.Count;
                 i++)
            {
                SavePreparedParticipantEntry prepared =
                    preparedEntries[i];

                if (prepared == null ||
                    !SaveParticipantId.TryParse(
                        prepared.CanonicalParticipantId.Value,
                        out SaveParticipantId canonicalId))
                {
                    return Failure(
                        SaveParticipantApplyPlanStatus.StateIncompatible,
                        default,
                        EchoSaveDiagnosticCodes.PreparedApplyStateIncompatible,
                        "A Chronicle prepared participant entry has an invalid canonical participant identity.");
                }

                if (preparedByCanonical.ContainsKey(
                        canonicalId.Value))
                {
                    return Failure(
                        SaveParticipantApplyPlanStatus.DuplicatePreparedParticipant,
                        canonicalId,
                        EchoSaveDiagnosticCodes.PreparedApplyDuplicateParticipant,
                        "The Chronicle prepared participant batch contains more than one entry for the same canonical participant.");
                }

                preparedByCanonical.Add(
                    canonicalId.Value,
                    prepared);
            }

            SaveParticipantRegistrySnapshot snapshot =
                participantRegistry.GetSnapshot();

            List<SaveParticipantApplyPlanStep> steps =
                new List<SaveParticipantApplyPlanStep>(
                    snapshot.Count);

            HashSet<string> matchedPrepared =
                new HashSet<string>(
                    StringComparer.Ordinal);

            for (int i = 0;
                 i < snapshot.Participants.Count;
                 i++)
            {
                SaveParticipantDescriptor snapshotDescriptor =
                    snapshot.Participants[i];

                SaveParticipantId participantId =
                    snapshotDescriptor.Id;

                if (!participantRegistry.TryResolveOwned(
                        participantId,
                        out ISaveParticipant participant,
                        out SaveParticipantDescriptor currentDescriptor,
                        out long ownershipToken) ||
                    participant == null ||
                    currentDescriptor.Id != participantId)
                {
                    return Failure(
                        SaveParticipantApplyPlanStatus.ParticipantUnavailable,
                        participantId,
                        EchoSaveDiagnosticCodes.PreparedApplyParticipantUnavailable,
                        "A Chronicle participant changed or became unavailable during prepared-load apply preflight.");
                }

                if (preparedByCanonical.TryGetValue(
                        participantId.Value,
                        out SavePreparedParticipantEntry prepared))
                {
                    if (!TryValidatePreparedCompatibility(
                            participant,
                            currentDescriptor,
                            prepared))
                    {
                        return Failure(
                            SaveParticipantApplyPlanStatus.StateIncompatible,
                            participantId,
                            EchoSaveDiagnosticCodes.PreparedApplyStateIncompatible,
                            "The current Chronicle participant is not compatible with its prepared detached state.");
                    }

                    matchedPrepared.Add(
                        participantId.Value);

                    steps.Add(
                        new SaveParticipantApplyPlanStep(
                            participantId,
                            SaveParticipantApplyActionKind.ApplyPreparedState,
                            participant,
                            ownershipToken,
                            prepared.DetachedState));

                    continue;
                }

                switch (currentDescriptor.MissingPayloadPolicy)
                {
                    case SaveMissingPayloadPolicy.Ignore:
                        steps.Add(
                            new SaveParticipantApplyPlanStep(
                                participantId,
                                SaveParticipantApplyActionKind.Ignore,
                                participant,
                                ownershipToken,
                                null));
                        break;

                    case SaveMissingPayloadPolicy.Fail:
                        return Failure(
                            SaveParticipantApplyPlanStatus.MissingPayloadBlocked,
                            participantId,
                            EchoSaveDiagnosticCodes.PreparedApplyMissingPayloadBlocked,
                            "A currently registered Chronicle participant requires a payload, but the prepared load contains none.");

                    case SaveMissingPayloadPolicy.InitializeDefault:
                        if (!(participant is
                            ISaveDefaultableParticipant))
                        {
                            return Failure(
                                SaveParticipantApplyPlanStatus.DefaultCapabilityMissing,
                                participantId,
                                EchoSaveDiagnosticCodes.PreparedApplyDefaultCapabilityMissing,
                                "A Chronicle participant requests default initialization but does not implement ISaveDefaultableParticipant.");
                        }

                        steps.Add(
                            new SaveParticipantApplyPlanStep(
                                participantId,
                                SaveParticipantApplyActionKind.InitializeDefault,
                                participant,
                                ownershipToken,
                                null));
                        break;

                    default:
                        return Failure(
                            SaveParticipantApplyPlanStatus.InvalidRequest,
                            participantId,
                            EchoSaveDiagnosticCodes.PreparedApplyInvalidRequest,
                            "A Chronicle participant declares an unsupported missing-payload policy.");
                }
            }

            if (matchedPrepared.Count !=
                preparedByCanonical.Count)
            {
                foreach (KeyValuePair<string, SavePreparedParticipantEntry>
                    pair in preparedByCanonical)
                {
                    if (matchedPrepared.Contains(
                            pair.Key))
                    {
                        continue;
                    }

                    return Failure(
                        SaveParticipantApplyPlanStatus.ParticipantUnavailable,
                        pair.Value.CanonicalParticipantId,
                        EchoSaveDiagnosticCodes.PreparedApplyParticipantUnavailable,
                        "A participant represented by prepared Chronicle state is no longer registered.");
                }
            }

            return SaveParticipantApplyPlanResult.Success(
                new SaveParticipantApplyPlan(
                    preparedBatch.SourceSlotId,
                    preparedBatch.SourceGenerationId,
                    steps.ToArray()));
        }

        private static bool TryValidatePreparedCompatibility(
            ISaveParticipant participant,
            SaveParticipantDescriptor descriptor,
            SavePreparedParticipantEntry prepared)
        {
            if (participant == null ||
                prepared == null ||
                descriptor.Id !=
                    prepared.CanonicalParticipantId ||
                descriptor.CurrentSchemaVersion !=
                    prepared.ParticipantSchemaVersion ||
                !(participant is
                    ISaveTypedParticipant typedParticipant))
            {
                return false;
            }

            Type liveType;

            try
            {
                liveType =
                    typedParticipant.DetachedStateType;
            }
            catch
            {
                return false;
            }

            return
                liveType != null &&
                prepared.DetachedStateType != null &&
                liveType ==
                    prepared.DetachedStateType &&
                prepared.DetachedState != null &&
                liveType.IsInstanceOfType(
                    prepared.DetachedState);
        }

        private static SaveParticipantApplyPlanResult
            Failure(
                SaveParticipantApplyPlanStatus status,
                SaveParticipantId participantId,
                string diagnosticCode,
                string message) =>
            SaveParticipantApplyPlanResult.Failure(
                status,
                participantId,
                diagnosticCode,
                message);
    }
}
