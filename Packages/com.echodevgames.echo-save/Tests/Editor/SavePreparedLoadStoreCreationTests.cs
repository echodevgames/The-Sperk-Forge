
using System;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SavePreparedLoadStoreCreationTests
    {
        private FakePreparedLoadClock clock;
        private SavePreparedLoadStore store;
        private SaveSlotId slotId;
        private SaveGenerationId generationId;

        [SetUp]
        public void SetUp()
        {
            clock =
                new FakePreparedLoadClock(
                    new DateTimeOffset(
                        2026,
                        8,
                        10,
                        0,
                        0,
                        0,
                        TimeSpan.Zero));

            store =
                new SavePreparedLoadStore(
                    clock,
                    TimeSpan.FromMinutes(5),
                    8,
                    1024 * 1024);

            slotId =
                SaveSlotId.NewId();

            generationId =
                SaveGenerationId.NewId();
        }

        [TearDown]
        public void TearDown()
        {
            store.Dispose();
        }

        [Test]
        public void MatchingArtifactsCreateLiveHandle()
        {
            PreparedLoadArtifacts artifacts =
                PreparedLoadArtifacts.Create(
                    slotId,
                    generationId,
                    1,
                    1);

            PreparedLoadCreationResult result =
                store.TryCreate(
                    artifacts.ReadResult,
                    artifacts.PreparationResult,
                    artifacts.UnknownSnapshot);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.Handle.IsValid,
                Is.True);

            Assert.That(
                result.Handle.State,
                Is.EqualTo(
                    PreparedLoadState.Live));

            Assert.That(
                result.Handle.SourceSlotId.Value,
                Is.EqualTo(
                    slotId.Value));

            Assert.That(
                result.Handle.SourceGenerationId.Value,
                Is.EqualTo(
                    generationId.Value));

            Assert.That(
                result.Handle.PreparedParticipantCount,
                Is.EqualTo(1));

            Assert.That(
                result.Handle.UnknownPayloadCount,
                Is.EqualTo(1));

            Assert.That(
                result.Handle.SourceTransportByteEstimate,
                Is.EqualTo(20L));

            Assert.That(
                result.Handle.CreatedUtc,
                Is.EqualTo(
                    clock.UtcNow));

            Assert.That(
                result.Handle.ExpiresUtc,
                Is.EqualTo(
                    clock.UtcNow.AddMinutes(5)));
        }

        [Test]
        public void SourceSlotMismatchRejects()
        {
            PreparedLoadArtifacts artifacts =
                PreparedLoadArtifacts.Create(
                    slotId,
                    generationId,
                    1,
                    0);

            SavePreparedParticipantBatch mismatched =
                PreparedLoadArtifacts
                    .CreatePreparedBatch(
                        SaveSlotId.NewId(),
                        generationId,
                        1);

            PreparedLoadCreationResult result =
                store.TryCreate(
                    artifacts.ReadResult,
                    SaveParticipantPreparationResult
                        .Success(
                            mismatched),
                    null);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    PreparedLoadCreationStatus
                        .SourceProvenanceMismatch));

            Assert.That(
                result.Handle,
                Is.Null);

            Assert.That(
                store.LiveCount,
                Is.Zero);
        }

        [Test]
        public void SourceGenerationMismatchRejects()
        {
            PreparedLoadArtifacts artifacts =
                PreparedLoadArtifacts.Create(
                    slotId,
                    generationId,
                    1,
                    0);

            SavePreparedParticipantBatch mismatched =
                PreparedLoadArtifacts
                    .CreatePreparedBatch(
                        slotId,
                        SaveGenerationId.NewId(),
                        1);

            PreparedLoadCreationResult result =
                store.TryCreate(
                    artifacts.ReadResult,
                    SaveParticipantPreparationResult
                        .Success(
                            mismatched),
                    null);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    PreparedLoadCreationStatus
                        .SourceProvenanceMismatch));

            Assert.That(
                store.LiveCount,
                Is.Zero);
        }

        [Test]
        public void UnknownSourceMismatchRejects()
        {
            PreparedLoadArtifacts artifacts =
                PreparedLoadArtifacts.Create(
                    slotId,
                    generationId,
                    1,
                    1);

            SaveUnknownPayloadSnapshot wrong =
                PreparedLoadArtifacts
                    .CreateUnknownSnapshot(
                        slotId,
                        SaveGenerationId.NewId(),
                        1);

            PreparedLoadCreationResult result =
                store.TryCreate(
                    artifacts.ReadResult,
                    artifacts.PreparationResult,
                    wrong);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    PreparedLoadCreationStatus
                        .UnknownPayloadProvenanceMismatch));

            Assert.That(
                store.LiveCount,
                Is.Zero);
        }

        [Test]
        public void UnknownEntriesWithoutProvenanceReject()
        {
            PreparedLoadArtifacts artifacts =
                PreparedLoadArtifacts.Create(
                    slotId,
                    generationId,
                    0,
                    1);

            SaveUnknownPayloadSnapshot unprovenanced =
                new SaveUnknownPayloadSnapshot(
                    SaveUnknownPayloadSnapshot
                        .CloneEntries(
                            artifacts
                                .UnknownSnapshot
                                .Entries),
                    artifacts
                        .UnknownSnapshot
                        .TotalPayloadBytes);

            PreparedLoadCreationResult result =
                store.TryCreate(
                    artifacts.ReadResult,
                    artifacts.PreparationResult,
                    unprovenanced);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    PreparedLoadCreationStatus
                        .UnknownPayloadProvenanceMismatch));

            Assert.That(
                store.LiveCount,
                Is.Zero);
        }

        [Test]
        public void ZeroUnknownEntriesMayUseNullSnapshot()
        {
            PreparedLoadArtifacts artifacts =
                PreparedLoadArtifacts.Create(
                    slotId,
                    generationId,
                    1,
                    0);

            PreparedLoadCreationResult result =
                store.TryCreate(
                    artifacts.ReadResult,
                    artifacts.PreparationResult,
                    null);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.Handle.UnknownPayloadCount,
                Is.Zero);
        }

        [Test]
        public void ClassificationMismatchRejects()
        {
            PreparedLoadArtifacts artifacts =
                PreparedLoadArtifacts.Create(
                    slotId,
                    generationId,
                    1,
                    1);

            SaveCurrentGenerationReadResult mismatched =
                new SaveCurrentGenerationReadResult(
                    SaveCurrentGenerationReadStatus.Succeeded,
                    string.Empty,
                    "test",
                    slotId,
                    generationId,
                    2,
                    0,
                    artifacts
                        .ReadResult
                        .ValidatedParticipants);

            PreparedLoadCreationResult result =
                store.TryCreate(
                    mismatched,
                    artifacts.PreparationResult,
                    null);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    PreparedLoadCreationStatus
                        .SourceProvenanceMismatch));

            Assert.That(
                store.LiveCount,
                Is.Zero);
        }

        [Test]
        public void FailedReadRejectsWithoutPartialHandle()
        {
            PreparedLoadArtifacts artifacts =
                PreparedLoadArtifacts.Create(
                    slotId,
                    generationId,
                    1,
                    0);

            SaveCurrentGenerationReadResult failedRead =
                new SaveCurrentGenerationReadResult(
                    SaveCurrentGenerationReadStatus
                        .GenerationInvalid,
                    "TEST",
                    "failed",
                    slotId,
                    generationId,
                    0,
                    0);

            PreparedLoadCreationResult result =
                store.TryCreate(
                    failedRead,
                    artifacts.PreparationResult,
                    null);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    PreparedLoadCreationStatus
                        .InvalidRequest));

            Assert.That(
                result.Handle,
                Is.Null);

            Assert.That(
                store.LiveCount,
                Is.Zero);

            Assert.That(
                store.LiveSourceTransportBytes,
                Is.Zero);
        }

        [Test]
        public void FailedPreparationRejectsWithoutPartialHandle()
        {
            PreparedLoadArtifacts artifacts =
                PreparedLoadArtifacts.Create(
                    slotId,
                    generationId,
                    1,
                    0);

            SaveParticipantPreparationResult failed =
                SaveParticipantPreparationResult
                    .Failure(
                        SaveParticipantPreparationStatus
                            .DeserializationFailed,
                        default,
                        default,
                        "TEST",
                        "failed");

            PreparedLoadCreationResult result =
                store.TryCreate(
                    artifacts.ReadResult,
                    failed,
                    null);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    PreparedLoadCreationStatus
                        .InvalidRequest));

            Assert.That(
                result.Handle,
                Is.Null);

            Assert.That(
                store.LiveCount,
                Is.Zero);
        }

        [Test]
        public void DisposedOwnerRejectsNewAdmission()
        {
            PreparedLoadArtifacts artifacts =
                PreparedLoadArtifacts.Create(
                    slotId,
                    generationId,
                    1,
                    0);

            store.Dispose();

            PreparedLoadCreationResult result =
                store.TryCreate(
                    artifacts.ReadResult,
                    artifacts.PreparationResult,
                    null);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    PreparedLoadCreationStatus
                        .OwnerUnavailable));

            Assert.That(
                result.Handle,
                Is.Null);
        }
    }
}
