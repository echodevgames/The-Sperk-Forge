
using System;
using System.Text;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveUnknownPayloadStoreTests
    {
        private Sha256IntegrityProvider integrity;

        [SetUp]
        public void SetUp()
        {
            integrity =
                new Sha256IntegrityProvider();
        }

        [Test]
        public void ReplacePreservesEveryOpaqueField()
        {
            SaveUnknownPayloadStore store =
                new SaveUnknownPayloadStore();

            SavePayloadEntry source =
                Entry(
                    "com.example.future",
                    "{\"exact\":\"  spaced  \"}",
                    9,
                    false);

            Assert.That(
                store.TryReplace(
                    new[]
                    {
                        source
                    })
                    .Succeeded,
                Is.True);

            SavePayloadEntry preserved =
                store.GetSnapshot()
                    .Entries[0];

            Assert.That(
                preserved.participantId,
                Is.EqualTo(
                    source.participantId));

            Assert.That(
                preserved.participantSchemaVersion,
                Is.EqualTo(
                    source.participantSchemaVersion));

            Assert.That(
                preserved.serializerId,
                Is.EqualTo(
                    source.serializerId));

            Assert.That(
                preserved.required,
                Is.EqualTo(
                    source.required));

            Assert.That(
                preserved.serializedPayload,
                Is.EqualTo(
                    source.serializedPayload));

            Assert.That(
                preserved.byteProviderReference,
                Is.EqualTo(
                    source.byteProviderReference));

            Assert.That(
                preserved.byteLength,
                Is.EqualTo(
                    source.byteLength));

            Assert.That(
                preserved.checksum,
                Is.EqualTo(
                    source.checksum));

            Assert.That(
                preserved.flags,
                Is.EqualTo(
                    source.flags));
        }

        [Test]
        public void SnapshotMutationCannotMutateStore()
        {
            SaveUnknownPayloadStore store =
                new SaveUnknownPayloadStore();

            Assert.That(
                store.TryReplace(
                    new[]
                    {
                        Entry(
                            "com.example.future",
                            "{\"value\":1}")
                    })
                    .Succeeded,
                Is.True);

            SavePayloadEntry external =
                store.GetSnapshot()
                    .Entries[0];

            external.participantId =
                "com.example.changed";

            external.serializedPayload =
                "{}";

            SavePayloadEntry authoritative =
                store.GetSnapshot()
                    .Entries[0];

            Assert.That(
                authoritative.participantId,
                Is.EqualTo(
                    "com.example.future"));

            Assert.That(
                authoritative.serializedPayload,
                Is.EqualTo(
                    "{\"value\":1}"));
        }

        [Test]
        public void StoreOrdersUnknownEntriesCanonically()
        {
            SaveUnknownPayloadStore store =
                new SaveUnknownPayloadStore();

            Assert.That(
                store.TryReplace(
                    new[]
                    {
                        Entry(
                            "com.example.zeta",
                            "{\"value\":3}"),
                        Entry(
                            "com.example.alpha",
                            "{\"value\":1}"),
                        Entry(
                            "com.example.middle",
                            "{\"value\":2}")
                    })
                    .Succeeded,
                Is.True);

            SaveUnknownPayloadSnapshot snapshot =
                store.GetSnapshot();

            Assert.That(
                snapshot.Entries[0]
                    .participantId,
                Is.EqualTo(
                    "com.example.alpha"));

            Assert.That(
                snapshot.Entries[1]
                    .participantId,
                Is.EqualTo(
                    "com.example.middle"));

            Assert.That(
                snapshot.Entries[2]
                    .participantId,
                Is.EqualTo(
                    "com.example.zeta"));
        }

        [Test]
        public void DuplicateCandidateDoesNotReplacePriorStore()
        {
            SaveUnknownPayloadStore store =
                new SaveUnknownPayloadStore();

            Assert.That(
                store.TryReplace(
                    new[]
                    {
                        Entry(
                            "com.example.prior",
                            "{\"value\":1}")
                    })
                    .Succeeded,
                Is.True);

            SaveUnknownPayloadStoreResult result =
                store.TryReplace(
                    new[]
                    {
                        Entry(
                            "com.example.future",
                            "{\"value\":2}"),
                        Entry(
                            "com.example.future",
                            "{\"value\":3}")
                    });

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveUnknownPayloadStoreStatus
                        .DuplicateId));

            Assert.That(
                store.GetSnapshot()
                    .Entries[0]
                    .participantId,
                Is.EqualTo(
                    "com.example.prior"));
        }

        [Test]
        public void EntryCountLimitDoesNotReplacePriorStore()
        {
            SaveUnknownPayloadStore store =
                new SaveUnknownPayloadStore(
                    1,
                    1024);

            Assert.That(
                store.TryReplace(
                    new[]
                    {
                        Entry(
                            "com.example.prior",
                            "{\"value\":1}")
                    })
                    .Succeeded,
                Is.True);

            SaveUnknownPayloadStoreResult result =
                store.TryReplace(
                    new[]
                    {
                        Entry(
                            "com.example.alpha",
                            "{\"value\":1}"),
                        Entry(
                            "com.example.beta",
                            "{\"value\":2}")
                    });

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveUnknownPayloadStoreStatus
                        .LimitExceeded));

            Assert.That(
                store.GetSnapshot()
                    .Entries[0]
                    .participantId,
                Is.EqualTo(
                    "com.example.prior"));
        }

        [Test]
        public void AggregateByteLimitDoesNotReplacePriorStore()
        {
            SaveUnknownPayloadStore store =
                new SaveUnknownPayloadStore(
                    10,
                    4);

            Assert.That(
                store.Count,
                Is.Zero);

            SaveUnknownPayloadStoreResult result =
                store.TryReplace(
                    new[]
                    {
                        Entry(
                            "com.example.future",
                            "{\"value\":123}")
                    });

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveUnknownPayloadStoreStatus
                        .LimitExceeded));

            Assert.That(
                store.Count,
                Is.Zero);
        }

        [Test]
        public void ClearResetsSessionStore()
        {
            SaveUnknownPayloadStore store =
                new SaveUnknownPayloadStore();

            Assert.That(
                store.TryReplace(
                    new[]
                    {
                        Entry(
                            "com.example.future",
                            "{\"value\":1}")
                    })
                    .Succeeded,
                Is.True);

            store.Clear();

            Assert.That(
                store.Count,
                Is.Zero);

            Assert.That(
                store.TotalPayloadBytes,
                Is.Zero);

            Assert.That(
                store.GetSnapshot()
                    .Entries,
                Is.Empty);
        }

        private SavePayloadEntry Entry(
            string participantId,
            string serializedPayload,
            int schemaVersion = 1,
            bool required = true)
        {
            byte[] bytes =
                Encoding.UTF8.GetBytes(
                    serializedPayload);

            Assert.That(
                integrity.Calculate(
                    bytes,
                    out string checksum)
                    .Succeeded,
                Is.True);

            return new SavePayloadEntry
            {
                participantId =
                    participantId,
                participantSchemaVersion =
                    schemaVersion,
                serializerId =
                    UnityJsonSaveSerializer
                        .StableId,
                required =
                    required,
                serializedPayload =
                    serializedPayload,
                byteProviderReference =
                    string.Empty,
                byteLength =
                    bytes.LongLength,
                checksum =
                    checksum,
                flags =
                    0
            };
        }
    }
}
