
using System;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveRecoveryPlanBuilderTests
    {
        private static readonly DateTime BaseUtc =
            new DateTime(
                2026,
                8,
                10,
                12,
                0,
                0,
                DateTimeKind.Utc);

        [Test]
        public void HealthyCurrentReportsRecoveryNotRequired()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                SaveGenerationId current =
                    env.PublishGeneration(
                        BaseUtc,
                        1);

                SaveRecoveryPlan plan =
                    env.Builder().Build(
                        env.SlotId);

                Assert.That(plan.Succeeded, Is.True);
                Assert.That(
                    plan.Status,
                    Is.EqualTo(
                        SaveRecoveryPlanStatus
                            .RecoveryNotRequired));
                Assert.That(
                    plan.HeadCondition,
                    Is.EqualTo(
                        SaveRecoveryHeadCondition.Healthy));
                Assert.That(plan.HasPreferredCandidate, Is.False);
                Assert.That(
                    plan.ObservedCurrentGenerationId,
                    Is.EqualTo(current));
                Assert.That(
                    env.ReadOnlyBackend.MutationCalls,
                    Is.Zero);
            }
        }

        [Test]
        public void MissingHeadSelectsNewestValidGeneration()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                SaveGenerationId oldest =
                    env.PublishGeneration(
                        BaseUtc,
                        1);

                SaveGenerationId newest =
                    env.PublishGeneration(
                        BaseUtc.AddMinutes(2),
                        2);

                env.DeleteHead();

                SaveRecoveryPlan plan =
                    env.Builder().Build(
                        env.SlotId);

                Assert.That(
                    plan.Status,
                    Is.EqualTo(
                        SaveRecoveryPlanStatus
                            .RecoveryAvailable));
                Assert.That(
                    plan.HeadCondition,
                    Is.EqualTo(
                        SaveRecoveryHeadCondition.Missing));
                Assert.That(plan.HasPreferredCandidate, Is.True);
                Assert.That(
                    plan.PreferredCandidate.GenerationId,
                    Is.EqualTo(newest));
                Assert.That(
                    plan.Candidates[1].GenerationId,
                    Is.EqualTo(oldest));
                Assert.That(
                    env.ReadOnlyBackend.MutationCalls,
                    Is.Zero);
            }
        }

        [Test]
        public void CorruptCurrentOffersPriorValidGeneration()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                SaveGenerationId prior =
                    env.PublishGeneration(
                        BaseUtc,
                        1);

                SaveGenerationId current =
                    env.PublishGeneration(
                        BaseUtc.AddMinutes(1),
                        2);

                env.CorruptPayloadChecksum(
                    current);

                SaveRecoveryPlan plan =
                    env.Builder().Build(
                        env.SlotId);

                Assert.That(
                    plan.HeadCondition,
                    Is.EqualTo(
                        SaveRecoveryHeadCondition
                            .CurrentInvalid));
                Assert.That(
                    plan.PreferredCandidate.GenerationId,
                    Is.EqualTo(prior));
                Assert.That(
                    plan.RejectedCanonicalCount,
                    Is.EqualTo(1));
            }
        }

        [Test]
        public void MultipleValidCandidatesOrderNewestFirst()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                SaveGenerationId first =
                    env.PublishGeneration(
                        BaseUtc,
                        1);

                SaveGenerationId second =
                    env.PublishGeneration(
                        BaseUtc.AddMinutes(1),
                        2);

                SaveGenerationId third =
                    env.PublishGeneration(
                        BaseUtc.AddMinutes(2),
                        3);

                env.DeleteHead();

                SaveRecoveryPlan plan =
                    env.Builder().Build(
                        env.SlotId);

                Assert.That(plan.Candidates.Count, Is.EqualTo(3));
                Assert.That(
                    plan.Candidates[0].GenerationId,
                    Is.EqualTo(third));
                Assert.That(
                    plan.Candidates[1].GenerationId,
                    Is.EqualTo(second));
                Assert.That(
                    plan.Candidates[2].GenerationId,
                    Is.EqualTo(first));
            }
        }

        [Test]
        public void TimestampTieBreakUsesGenerationIdNewestFirst()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                SaveGenerationId first =
                    env.PublishGeneration(
                        BaseUtc,
                        1);

                SaveGenerationId second =
                    env.PublishGeneration(
                        BaseUtc,
                        2);

                env.DeleteHead();

                SaveRecoveryPlan plan =
                    env.Builder().Build(
                        env.SlotId);

                Assert.That(
                    plan.Candidates[0].GenerationId,
                    Is.EqualTo(second));
                Assert.That(
                    plan.Candidates[1].GenerationId,
                    Is.EqualTo(first));
            }
        }

        [Test]
        public void NoValidCandidatePreservesSource()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                SaveGenerationId only =
                    env.PublishGeneration(
                        BaseUtc,
                        1);

                env.CorruptPayloadChecksum(
                    only);

                env.DeleteHead();

                SaveRecoveryPlan plan =
                    env.Builder().Build(
                        env.SlotId);

                Assert.That(
                    plan.Status,
                    Is.EqualTo(
                        SaveRecoveryPlanStatus
                            .NoValidCandidate));
                Assert.That(plan.Candidates.Count, Is.Zero);
                Assert.That(plan.HasPreferredCandidate, Is.False);
                Assert.That(
                    env.ReadOnlyBackend.MutationCalls,
                    Is.Zero);
            }
        }

        [Test]
        public void MalformedHeadStillAllowsVerifiedCandidatePlan()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                SaveGenerationId generation =
                    env.PublishGeneration(
                        BaseUtc,
                        1);

                env.CorruptHead();

                SaveRecoveryPlan plan =
                    env.Builder().Build(
                        env.SlotId);

                Assert.That(
                    plan.HeadCondition,
                    Is.EqualTo(
                        SaveRecoveryHeadCondition.Invalid));
                Assert.That(
                    plan.PreferredCandidate.GenerationId,
                    Is.EqualTo(generation));
            }
        }

        [Test]
        public void UnsupportedHeadStillAllowsVerifiedCandidatePlan()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                env.PublishGeneration(
                    BaseUtc,
                    1);

                env.MakeHeadUnsupported();

                SaveRecoveryPlan plan =
                    env.Builder().Build(
                        env.SlotId);

                Assert.That(
                    plan.HeadCondition,
                    Is.EqualTo(
                        SaveRecoveryHeadCondition.Invalid));
                Assert.That(plan.HasPreferredCandidate, Is.True);
            }
        }

        [Test]
        public void MissingCurrentGenerationOffersPriorValidGeneration()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                SaveGenerationId prior =
                    env.PublishGeneration(
                        BaseUtc,
                        1);

                SaveGenerationId current =
                    env.PublishGeneration(
                        BaseUtc.AddMinutes(1),
                        2);

                env.DeleteGenerationPayload(
                    current);

                SaveRecoveryPlan plan =
                    env.Builder().Build(
                        env.SlotId);

                Assert.That(
                    plan.HeadCondition,
                    Is.EqualTo(
                        SaveRecoveryHeadCondition
                            .CurrentInvalid));
                Assert.That(
                    plan.PreferredCandidate.GenerationId,
                    Is.EqualTo(prior));
            }
        }

        [Test]
        public void HeadPointingAtUndiscoveredGenerationReportsCurrentMissing()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                SaveGenerationId prior =
                    env.PublishGeneration(
                        BaseUtc,
                        1);

                SaveGenerationId missing =
                    SaveGenerationId.CreateForTesting(
                        BaseUtc.AddMinutes(5),
                        99,
                        new Guid(
                            "00000000-0000-0000-0000-000000000099"));

                env.PointHeadAtMissingGeneration(
                    missing);

                SaveRecoveryPlan plan =
                    env.Builder().Build(
                        env.SlotId);

                Assert.That(
                    plan.HeadCondition,
                    Is.EqualTo(
                        SaveRecoveryHeadCondition
                            .CurrentMissing));
                Assert.That(
                    plan.PreferredCandidate.GenerationId,
                    Is.EqualTo(prior));
            }
        }

        [Test]
        public void ChecksumMismatchCandidateIsExcluded()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                SaveGenerationId bad =
                    env.PublishGeneration(
                        BaseUtc,
                        1);

                env.CorruptPayloadChecksum(
                    bad);

                env.DeleteHead();

                SaveRecoveryPlan plan =
                    env.Builder().Build(
                        env.SlotId);

                Assert.That(plan.Candidates.Count, Is.Zero);
                Assert.That(
                    plan.RejectedCanonicalCount,
                    Is.EqualTo(1));
            }
        }

        [Test]
        public void ManifestPayloadInventoryMismatchIsExcluded()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                SaveGenerationId bad =
                    env.PublishGeneration(
                        BaseUtc,
                        1);

                env.MakeInventoryMismatch(
                    bad);

                env.DeleteHead();

                SaveRecoveryPlan plan =
                    env.Builder().Build(
                        env.SlotId);

                Assert.That(plan.Candidates.Count, Is.Zero);
                Assert.That(
                    plan.RejectedCanonicalCount,
                    Is.EqualTo(1));
            }
        }

        [Test]
        public void UnsupportedGenerationIsPreservedAndExcluded()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                SaveGenerationId bad =
                    env.PublishGeneration(
                        BaseUtc,
                        1);

                env.MakeManifestUnsupported(
                    bad);

                env.DeleteHead();

                SaveRecoveryPlan plan =
                    env.Builder().Build(
                        env.SlotId);

                Assert.That(plan.Candidates.Count, Is.Zero);
                Assert.That(
                    env.ReadOnlyBackend.MutationCalls,
                    Is.Zero);
            }
        }

        [Test]
        public void UncommittedGenerationIsPreservedAndExcluded()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                SaveGenerationId bad =
                    env.PublishGeneration(
                        BaseUtc,
                        1);

                env.MakeGenerationUncommitted(
                    bad);

                env.DeleteHead();

                SaveRecoveryPlan plan =
                    env.Builder().Build(
                        env.SlotId);

                Assert.That(plan.Candidates.Count, Is.Zero);
                Assert.That(
                    plan.RejectedCanonicalCount,
                    Is.EqualTo(1));
            }
        }

        [Test]
        public void NonCanonicalGenerationChildIsIgnoredAndPreserved()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                env.PublishGeneration(
                    BaseUtc,
                    1);

                env.CreateNonCanonicalChild(
                    "operator-notes");

                env.DeleteHead();

                SaveRecoveryPlan plan =
                    env.Builder().Build(
                        env.SlotId);

                Assert.That(
                    plan.IgnoredNonCanonicalCount,
                    Is.EqualTo(1));
                Assert.That(
                    plan.VerifiedCandidateCount,
                    Is.EqualTo(1));
                Assert.That(
                    env.ReadOnlyBackend.MutationCalls,
                    Is.Zero);
            }
        }

        [Test]
        public void DiscoveryLimitFailureProducesNoMutation()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                env.PublishGeneration(
                    BaseUtc,
                    1);

                env.PublishGeneration(
                    BaseUtc.AddMinutes(1),
                    2);

                env.DeleteHead();

                SaveRecoveryPlan plan =
                    env.Builder(
                        discoveryLimit: 1)
                    .Build(
                        env.SlotId);

                Assert.That(
                    plan.Status,
                    Is.EqualTo(
                        SaveRecoveryPlanStatus
                            .DiscoveryFailed));
                Assert.That(
                    env.ReadOnlyBackend.MutationCalls,
                    Is.Zero);
            }
        }

        [Test]
        public void ProviderDiscoveryFailureProducesNoMutation()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                env.PublishGeneration(
                    BaseUtc,
                    1);

                env.DeleteHead();

                env.ReadOnlyBackend.FailDiscovery =
                    true;

                SaveRecoveryPlan plan =
                    env.Builder().Build(
                        env.SlotId);

                Assert.That(
                    plan.Status,
                    Is.EqualTo(
                        SaveRecoveryPlanStatus
                            .DiscoveryFailed));
                Assert.That(
                    env.ReadOnlyBackend.MutationCalls,
                    Is.Zero);
            }
        }

        [Test]
        public void ProviderReadFailurePreservesEvidenceAndOffersNoCandidate()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                env.PublishGeneration(
                    BaseUtc,
                    1);

                env.ReadOnlyBackend.FailReads =
                    true;

                SaveRecoveryPlan plan =
                    env.Builder().Build(
                        env.SlotId);

                Assert.That(
                    plan.HeadCondition,
                    Is.EqualTo(
                        SaveRecoveryHeadCondition.Unreadable));
                Assert.That(
                    plan.Status,
                    Is.EqualTo(
                        SaveRecoveryPlanStatus
                            .NoValidCandidate));
                Assert.That(
                    env.ReadOnlyBackend.MutationCalls,
                    Is.Zero);
            }
        }

        [Test]
        public void SourceProvenanceIsDeterministicForUnchangedEvidence()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                env.PublishGeneration(
                    BaseUtc,
                    1);

                SaveRecoveryPlan first =
                    env.Builder().Build(
                        env.SlotId);

                SaveRecoveryPlan second =
                    env.Builder().Build(
                        env.SlotId);

                Assert.That(
                    first.SourceProvenanceFingerprint,
                    Is.Not.Empty);
                Assert.That(
                    second.SourceProvenanceFingerprint,
                    Is.EqualTo(
                        first.SourceProvenanceFingerprint));
            }
        }

        [Test]
        public void SourceProvenanceChangesWhenCanonicalGenerationEvidenceChanges()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                env.PublishGeneration(
                    BaseUtc,
                    1);

                byte[] originalHead =
                    env.ReadHeadBytes();

                SaveRecoveryPlan before =
                    env.Builder().Build(
                        env.SlotId);

                env.PublishGeneration(
                    BaseUtc.AddMinutes(1),
                    2);

                env.RestoreHeadBytes(
                    originalHead);

                SaveRecoveryPlan after =
                    env.Builder().Build(
                        env.SlotId);

                Assert.That(
                    after.SourceProvenanceFingerprint,
                    Is.Not.EqualTo(
                        before.SourceProvenanceFingerprint));
            }
        }

        [Test]
        public void InvalidSlotIsRejectedBeforeDiscovery()
        {
            using (SaveRecoveryTestEnvironment env =
                new SaveRecoveryTestEnvironment())
            {
                SaveRecoveryPlan plan =
                    env.Builder().Build(
                        default);

                Assert.That(
                    plan.Status,
                    Is.EqualTo(
                        SaveRecoveryPlanStatus
                            .InvalidRequest));
                Assert.That(
                    env.ReadOnlyBackend.MutationCalls,
                    Is.Zero);
            }
        }
    }
}
