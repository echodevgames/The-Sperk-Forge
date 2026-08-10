
using System;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SavePreparedLoadStoreLifecycleTests
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
                        1,
                        0,
                        0,
                        TimeSpan.Zero));

            store =
                new SavePreparedLoadStore(
                    clock,
                    TimeSpan.FromSeconds(30),
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
        public void DisposeIsIdempotentAndReleasesState()
        {
            PreparedSaveLoad handle =
                CreateHandle(
                    1,
                    1);

            handle.Dispose();
            handle.Dispose();

            Assert.That(
                handle.State,
                Is.EqualTo(
                    PreparedLoadState.Disposed));

            Assert.That(
                handle.IsValid,
                Is.False);

            Assert.That(
                store.LiveCount,
                Is.Zero);

            Assert.That(
                store.LiveSourceTransportBytes,
                Is.Zero);

            Assert.That(
                store.TryGetPreparedParticipantBatch(
                    handle,
                    out _),
                Is.False);

            Assert.That(
                store.TryGetUnknownPayloadSnapshot(
                    handle,
                    out _),
                Is.False);
        }

        [Test]
        public void LazyValidityCheckExpiresHandle()
        {
            PreparedSaveLoad handle =
                CreateHandle(
                    1,
                    0);

            clock.Advance(
                TimeSpan.FromSeconds(30));

            Assert.That(
                handle.IsValid,
                Is.False);

            Assert.That(
                handle.State,
                Is.EqualTo(
                    PreparedLoadState.Expired));

            Assert.That(
                store.LiveCount,
                Is.Zero);
        }

        [Test]
        public void SweepExpiresAllDueHandles()
        {
            PreparedSaveLoad first =
                CreateHandle(
                    1,
                    0);

            PreparedSaveLoad second =
                CreateHandle(
                    1,
                    0);

            clock.Advance(
                TimeSpan.FromMinutes(1));

            Assert.That(
                store.SweepExpired(),
                Is.EqualTo(2));

            Assert.That(
                first.State,
                Is.EqualTo(
                    PreparedLoadState.Expired));

            Assert.That(
                second.State,
                Is.EqualTo(
                    PreparedLoadState.Expired));

            Assert.That(
                store.LiveSourceTransportBytes,
                Is.Zero);
        }

        [Test]
        public void SessionInvalidationInvalidatesAllHandles()
        {
            PreparedSaveLoad first =
                CreateHandle(
                    1,
                    0);

            PreparedSaveLoad second =
                CreateHandle(
                    1,
                    1);

            long priorEpoch =
                store.Epoch;

            store.InvalidateSession();

            Assert.That(
                first.State,
                Is.EqualTo(
                    PreparedLoadState
                        .OwnerInvalidated));

            Assert.That(
                second.State,
                Is.EqualTo(
                    PreparedLoadState
                        .OwnerInvalidated));

            Assert.That(
                store.Epoch,
                Is.GreaterThan(
                    priorEpoch));

            Assert.That(
                store.LiveCount,
                Is.Zero);

            Assert.That(
                store.LiveSourceTransportBytes,
                Is.Zero);
        }

        [Test]
        public void InvalidatedHandleCannotResurrect()
        {
            PreparedSaveLoad oldHandle =
                CreateHandle(
                    1,
                    0);

            store.InvalidateSession();

            PreparedSaveLoad replacement =
                CreateHandle(
                    1,
                    0);

            Assert.That(
                oldHandle.State,
                Is.EqualTo(
                    PreparedLoadState
                        .OwnerInvalidated));

            Assert.That(
                oldHandle.IsValid,
                Is.False);

            Assert.That(
                replacement.IsValid,
                Is.True);
        }

        [Test]
        public void CrossOwnerAccessRejects()
        {
            PreparedSaveLoad handle =
                CreateHandle(
                    1,
                    0);

            using (SavePreparedLoadStore other =
                new SavePreparedLoadStore(
                    clock,
                    TimeSpan.FromSeconds(30),
                    8,
                    1024 * 1024))
            {
                Assert.That(
                    other.TryGetPreparedParticipantBatch(
                        handle,
                        out _),
                    Is.False);

                Assert.That(
                    other.TryGetUnknownPayloadSnapshot(
                        handle,
                        out _),
                    Is.False);
            }

            Assert.That(
                handle.IsValid,
                Is.True);
        }

        [Test]
        public void StaleOwnershipTokenCannotReleaseReplacement()
        {
            PreparedSaveLoad stale =
                CreateHandle(
                    1,
                    0);

            long staleToken =
                stale.OwnershipToken;

            long staleEpoch =
                stale.OwnerEpoch;

            store.InvalidateSession();

            PreparedSaveLoad replacement =
                CreateHandle(
                    1,
                    0);

            bool released =
                store.ReleaseOwned(
                    replacement,
                    staleToken,
                    staleEpoch,
                    PreparedLoadState.Disposed);

            Assert.That(
                released,
                Is.False);

            Assert.That(
                replacement.IsValid,
                Is.True);

            Assert.That(
                store.LiveCount,
                Is.EqualTo(1));
        }

        [Test]
        public void StoreDisposeInvalidatesLiveHandles()
        {
            PreparedSaveLoad handle =
                CreateHandle(
                    1,
                    0);

            store.Dispose();

            Assert.That(
                handle.State,
                Is.EqualTo(
                    PreparedLoadState
                        .OwnerInvalidated));

            Assert.That(
                store.LiveCount,
                Is.Zero);

            Assert.That(
                store.IsAvailable,
                Is.False);
        }

        [Test]
        public void PreparedBatchIsAccessibleOnlyWhileLive()
        {
            PreparedSaveLoad handle =
                CreateHandle(
                    1,
                    0);

            Assert.That(
                store.TryGetPreparedParticipantBatch(
                    handle,
                    out SavePreparedParticipantBatch batch),
                Is.True);

            Assert.That(
                batch.Count,
                Is.EqualTo(1));

            handle.Dispose();

            Assert.That(
                store.TryGetPreparedParticipantBatch(
                    handle,
                    out _),
                Is.False);
        }

        [Test]
        public void UnknownSnapshotAccessReturnsDefensiveCopies()
        {
            PreparedSaveLoad handle =
                CreateHandle(
                    0,
                    1);

            Assert.That(
                store.TryGetUnknownPayloadSnapshot(
                    handle,
                    out SaveUnknownPayloadSnapshot first),
                Is.True);

            SavePayloadEntry mutable =
                first.Entries[0];

            mutable.serializedPayload =
                "mutated";

            Assert.That(
                store.TryGetUnknownPayloadSnapshot(
                    handle,
                    out SaveUnknownPayloadSnapshot second),
                Is.True);

            Assert.That(
                second.Entries[0]
                    .serializedPayload,
                Is.Not.EqualTo(
                    "mutated"));
        }

        private PreparedSaveLoad CreateHandle(
            int known,
            int unknown)
        {
            PreparedLoadArtifacts artifacts =
                PreparedLoadArtifacts.Create(
                    slotId,
                    generationId,
                    known,
                    unknown);

            PreparedLoadCreationResult result =
                store.TryCreate(
                    artifacts.ReadResult,
                    artifacts.PreparationResult,
                    unknown > 0
                        ? artifacts.UnknownSnapshot
                        : null);

            Assert.That(
                result.Succeeded,
                Is.True);

            return result.Handle;
        }
    }
}
