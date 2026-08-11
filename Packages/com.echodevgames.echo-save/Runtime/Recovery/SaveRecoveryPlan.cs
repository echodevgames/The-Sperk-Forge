
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Immutable read-only Chronicle recovery plan.
    ///
    /// A plan is evidence, not mutation authority. M4-07 captures source
    /// provenance for a later execution checkpoint to revalidate.
    /// </summary>
    public sealed class SaveRecoveryPlan
    {
        private readonly ReadOnlyCollection<
            SaveRecoveryCandidate> candidates;

        internal SaveRecoveryPlan(
            SaveRecoveryPlanStatus status,
            string diagnosticCode,
            string message,
            SaveSlotId slotId,
            SaveRecoveryHeadCondition headCondition,
            string observedDiagnosticCode,
            SaveGenerationId observedCurrentGenerationId,
            bool hasObservedCurrentGeneration,
            SaveRecoveryCandidate[] candidates,
            SaveRecoveryCandidate preferredCandidate,
            bool hasPreferredCandidate,
            int rejectedCanonicalCount,
            int ignoredNonCanonicalCount,
            string sourceProvenanceFingerprint)
        {
            Status =
                status;

            DiagnosticCode =
                diagnosticCode ?? string.Empty;

            Message =
                message ?? string.Empty;

            SlotId =
                slotId;

            HeadCondition =
                headCondition;

            ObservedDiagnosticCode =
                observedDiagnosticCode ?? string.Empty;

            ObservedCurrentGenerationId =
                observedCurrentGenerationId;

            HasObservedCurrentGeneration =
                hasObservedCurrentGeneration;

            SaveRecoveryCandidate[] safeCandidates =
                candidates == null
                    ? Array.Empty<SaveRecoveryCandidate>()
                    : (SaveRecoveryCandidate[])
                        candidates.Clone();

            this.candidates =
                Array.AsReadOnly(
                    safeCandidates);

            PreferredCandidate =
                preferredCandidate;

            HasPreferredCandidate =
                hasPreferredCandidate;

            RejectedCanonicalCount =
                rejectedCanonicalCount;

            IgnoredNonCanonicalCount =
                ignoredNonCanonicalCount;

            SourceProvenanceFingerprint =
                sourceProvenanceFingerprint ??
                string.Empty;
        }

        public SaveRecoveryPlanStatus Status { get; }

        public string DiagnosticCode { get; }

        public string Message { get; }

        public SaveSlotId SlotId { get; }

        public SaveRecoveryHeadCondition HeadCondition { get; }

        public string ObservedDiagnosticCode { get; }

        public SaveGenerationId ObservedCurrentGenerationId { get; }

        public bool HasObservedCurrentGeneration { get; }

        public IReadOnlyList<SaveRecoveryCandidate> Candidates =>
            candidates;

        public int VerifiedCandidateCount =>
            candidates.Count;

        public SaveRecoveryCandidate PreferredCandidate { get; }

        public bool HasPreferredCandidate { get; }

        public int RejectedCanonicalCount { get; }

        public int IgnoredNonCanonicalCount { get; }

        public string SourceProvenanceFingerprint { get; }

        public bool Succeeded =>
            Status ==
                SaveRecoveryPlanStatus.RecoveryNotRequired ||
            Status ==
                SaveRecoveryPlanStatus.RecoveryAvailable ||
            Status ==
                SaveRecoveryPlanStatus.NoValidCandidate;

        public bool RecoveryRequired =>
            Status ==
                SaveRecoveryPlanStatus.RecoveryAvailable ||
            Status ==
                SaveRecoveryPlanStatus.NoValidCandidate;

        internal static SaveRecoveryPlan Failure(
            SaveRecoveryPlanStatus status,
            string diagnosticCode,
            string message,
            SaveSlotId slotId) =>
            new SaveRecoveryPlan(
                status,
                diagnosticCode,
                message,
                slotId,
                SaveRecoveryHeadCondition.Invalid,
                string.Empty,
                default,
                false,
                Array.Empty<SaveRecoveryCandidate>(),
                default,
                false,
                0,
                0,
                string.Empty);
    }
}
