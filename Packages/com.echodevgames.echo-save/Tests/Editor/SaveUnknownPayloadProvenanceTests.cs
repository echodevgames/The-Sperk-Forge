
using System.Text;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveUnknownPayloadProvenanceTests
    {
        private Sha256IntegrityProvider integrity;
        private SaveUnknownPayloadStore store;
        private SaveSlotId slotId;
        private SaveGenerationId generationId;

        [SetUp]
        public void SetUp()
        {
            integrity =
                new Sha256IntegrityProvider();

            store =
                new SaveUnknownPayloadStore();

            slotId =
                SaveSlotId.NewId();

            generationId =
                SaveGenerationId.NewId();
        }

        [Test]
        public void ProvenanceReplacementBindsExactSource()
        {
            Assert.That(
                store.TryReplace(
                    new[]
                    {
                        Entry(
                            "com.example.future",
                            "{\"value\":1}")
                    },
                    slotId,
                    generationId)
                    .Succeeded,
                Is.True);

            SaveUnknownPayloadSnapshot snapshot =
                store.GetSnapshot();

            Assert.That(
                snapshot.HasSourceProvenance,
                Is.True);

            Assert.That(
                snapshot.SourceSlotId,
                Is.EqualTo(
                    slotId));

            Assert.That(
                snapshot.SourceGenerationId,
                Is.EqualTo(
                    generationId));
        }

        [Test]
        public void EmptySuccessfulReplacementStillCarriesSourceProvenance()
        {
            Assert.That(
                store.TryReplace(
                    System.Array.Empty<SavePayloadEntry>(),
                    slotId,
                    generationId)
                    .Succeeded,
                Is.True);

            SaveUnknownPayloadSnapshot snapshot =
                store.GetSnapshot();

            Assert.That(
                snapshot.Count,
                Is.Zero);

            Assert.That(
                snapshot.HasSourceProvenance,
                Is.True);

            Assert.That(
                snapshot.SourceSlotId,
                Is.EqualTo(
                    slotId));

            Assert.That(
                snapshot.SourceGenerationId,
                Is.EqualTo(
                    generationId));
        }

        [Test]
        public void FailedReplacementPreservesPriorEntriesAndProvenance()
        {
            SavePayloadEntry prior =
                Entry(
                    "com.example.prior",
                    "{\"prior\":true}");

            Assert.That(
                store.TryReplace(
                    new[]
                    {
                        prior
                    },
                    slotId,
                    generationId)
                    .Succeeded,
                Is.True);

            SaveUnknownPayloadStoreResult failed =
                store.TryReplace(
                    new SavePayloadEntry[]
                    {
                        null
                    },
                    SaveSlotId.NewId(),
                    SaveGenerationId.NewId());

            Assert.That(
                failed.Succeeded,
                Is.False);

            SaveUnknownPayloadSnapshot snapshot =
                store.GetSnapshot();

            Assert.That(
                snapshot.Count,
                Is.EqualTo(1));

            Assert.That(
                snapshot.Entries[0]
                    .participantId,
                Is.EqualTo(
                    prior.participantId));

            Assert.That(
                snapshot.SourceSlotId,
                Is.EqualTo(
                    slotId));

            Assert.That(
                snapshot.SourceGenerationId,
                Is.EqualTo(
                    generationId));
        }

        [Test]
        public void ClearRemovesEntriesAndProvenance()
        {
            Assert.That(
                store.TryReplace(
                    new[]
                    {
                        Entry(
                            "com.example.future",
                            "{\"value\":1}")
                    },
                    slotId,
                    generationId)
                    .Succeeded,
                Is.True);

            store.Clear();

            SaveUnknownPayloadSnapshot snapshot =
                store.GetSnapshot();

            Assert.That(
                snapshot.Count,
                Is.Zero);

            Assert.That(
                snapshot.HasSourceProvenance,
                Is.False);

            Assert.That(
                snapshot.SourceSlotId,
                Is.EqualTo(
                    default(SaveSlotId)));

            Assert.That(
                snapshot.SourceGenerationId,
                Is.EqualTo(
                    default(SaveGenerationId)));
        }

        [Test]
        public void LegacyReplacementHasNoCarryForwardProvenance()
        {
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

            Assert.That(
                store.GetSnapshot()
                    .HasSourceProvenance,
                Is.False);
        }

        private SavePayloadEntry Entry(
            string participantId,
            string serializedPayload)
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
                    1,
                serializerId =
                    UnityJsonSaveSerializer
                        .StableId,
                required =
                    false,
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
