
using System;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SavePreparedLoadStoreBoundsTests
    {
        private FakePreparedLoadClock clock;
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
                        2,
                        0,
                        0,
                        TimeSpan.Zero));

            slotId =
                SaveSlotId.NewId();

            generationId =
                SaveGenerationId.NewId();
        }

        [Test]
        public void CountLimitRejectsWithoutEviction()
        {
            using (SavePreparedLoadStore store =
                new SavePreparedLoadStore(
                    clock,
                    TimeSpan.FromMinutes(5),
                    1,
                    1024))
            {
                PreparedSaveLoad first =
                    Create(
                        store,
                        1,
                        0)
                    .Handle;

                PreparedLoadCreationResult second =
                    Create(
                        store,
                        1,
                        0);

                Assert.That(
                    second.Status,
                    Is.EqualTo(
                        PreparedLoadCreationStatus
                            .CountLimitExceeded));

                Assert.That(
                    first.IsValid,
                    Is.True);

                Assert.That(
                    store.LiveCount,
                    Is.EqualTo(1));
            }
        }

        [Test]
        public void DisposeReleasesCountCapacity()
        {
            using (SavePreparedLoadStore store =
                new SavePreparedLoadStore(
                    clock,
                    TimeSpan.FromMinutes(5),
                    1,
                    1024))
            {
                PreparedSaveLoad first =
                    Create(
                        store,
                        1,
                        0)
                    .Handle;

                first.Dispose();

                Assert.That(
                    Create(
                        store,
                        1,
                        0)
                    .Succeeded,
                    Is.True);
            }
        }

        [Test]
        public void ByteLimitRejectsWithoutEviction()
        {
            using (SavePreparedLoadStore store =
                new SavePreparedLoadStore(
                    clock,
                    TimeSpan.FromMinutes(5),
                    8,
                    15))
            {
                PreparedSaveLoad first =
                    Create(
                        store,
                        1,
                        0)
                    .Handle;

                PreparedLoadCreationResult second =
                    Create(
                        store,
                        1,
                        0);

                Assert.That(
                    first.SourceTransportByteEstimate,
                    Is.EqualTo(10L));

                Assert.That(
                    second.Status,
                    Is.EqualTo(
                        PreparedLoadCreationStatus
                            .ByteLimitExceeded));

                Assert.That(
                    first.IsValid,
                    Is.True);

                Assert.That(
                    store.LiveSourceTransportBytes,
                    Is.EqualTo(10L));
            }
        }

        [Test]
        public void ExpiryReleasesByteCapacityBeforeAdmission()
        {
            using (SavePreparedLoadStore store =
                new SavePreparedLoadStore(
                    clock,
                    TimeSpan.FromSeconds(5),
                    8,
                    15))
            {
                PreparedSaveLoad first =
                    Create(
                        store,
                        1,
                        0)
                    .Handle;

                clock.Advance(
                    TimeSpan.FromSeconds(5));

                PreparedLoadCreationResult second =
                    Create(
                        store,
                        1,
                        0);

                Assert.That(
                    first.State,
                    Is.EqualTo(
                        PreparedLoadState.Expired));

                Assert.That(
                    second.Succeeded,
                    Is.True);

                Assert.That(
                    store.LiveSourceTransportBytes,
                    Is.EqualTo(10L));
            }
        }

        [Test]
        public void FailedAdmissionLeaksNoCapacity()
        {
            using (SavePreparedLoadStore store =
                new SavePreparedLoadStore(
                    clock,
                    TimeSpan.FromMinutes(5),
                    2,
                    15))
            {
                PreparedSaveLoad first =
                    Create(
                        store,
                        1,
                        0)
                    .Handle;

                PreparedLoadCreationResult rejected =
                    Create(
                        store,
                        1,
                        0);

                Assert.That(
                    rejected.Succeeded,
                    Is.False);

                Assert.That(
                    store.LiveCount,
                    Is.EqualTo(1));

                Assert.That(
                    store.LiveSourceTransportBytes,
                    Is.EqualTo(10L));

                first.Dispose();

                Assert.That(
                    Create(
                        store,
                        1,
                        0)
                    .Succeeded,
                    Is.True);
            }
        }

        [Test]
        public void SessionInvalidationReleasesAllCapacity()
        {
            using (SavePreparedLoadStore store =
                new SavePreparedLoadStore(
                    clock,
                    TimeSpan.FromMinutes(5),
                    4,
                    100))
            {
                Create(
                    store,
                    1,
                    0);

                Create(
                    store,
                    1,
                    1);

                Assert.That(
                    store.LiveCount,
                    Is.EqualTo(2));

                store.InvalidateSession();

                Assert.That(
                    store.LiveCount,
                    Is.Zero);

                Assert.That(
                    store.LiveSourceTransportBytes,
                    Is.Zero);
            }
        }

        [TestCase(0, 1L)]
        [TestCase(1, 0L)]
        [TestCase(-1, 1L)]
        [TestCase(1, -1L)]
        public void InvalidBoundsRejectAtConfigurationTime(
            int maxHandles,
            long maxBytes)
        {
            Assert.Throws<
                ArgumentOutOfRangeException>(
                () =>
                    new SavePreparedLoadStore(
                        clock,
                        TimeSpan.FromMinutes(1),
                        maxHandles,
                        maxBytes));
        }

        private PreparedLoadCreationResult Create(
            SavePreparedLoadStore store,
            int known,
            int unknown)
        {
            PreparedLoadArtifacts artifacts =
                PreparedLoadArtifacts.Create(
                    slotId,
                    generationId,
                    known,
                    unknown);

            return store.TryCreate(
                artifacts.ReadResult,
                artifacts.PreparationResult,
                unknown > 0
                    ? artifacts.UnknownSnapshot
                    : null);
        }
    }
}
