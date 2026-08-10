
using System;
using System.Text;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveUnknownPayloadCarryForwardMergerTests
    {
        private Sha256IntegrityProvider integrity;
        private SaveParticipantRegistry registry;
        private SaveSlotId slotId;
        private SaveGenerationId generationId;

        [SetUp]
        public void SetUp()
        {
            integrity =
                new Sha256IntegrityProvider();

            registry =
                new SaveParticipantRegistry();

            slotId =
                SaveSlotId.NewId();

            generationId =
                SaveGenerationId.NewId();
        }

        [Test]
        public void FreshKnownAndUnknownMergeInDeterministicOrder()
        {
            TrackingParticipant inventory =
                Participant(
                    "com.example.inventory");

            Register(
                inventory);

            string exactUnknown =
                "{ \"future\" : \"  preserve me  \" }";

            SaveUnknownPayloadMergeResult result =
                Merger().Merge(
                    Batch(
                        Pair(
                            "com.example.inventory",
                            "{\"gold\":150}")),
                    Snapshot(
                        Entry(
                            "com.example.future",
                            exactUnknown,
                            7,
                            false)));

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                result.Batch.FreshParticipantCount,
                Is.EqualTo(1));

            Assert.That(
                result.Batch.PreservedUnknownCount,
                Is.EqualTo(1));

            Assert.That(
                result.Batch.PayloadEntries[0]
                    .participantId,
                Is.EqualTo(
                    "com.example.future"));

            Assert.That(
                result.Batch.PayloadEntries[0]
                    .serializedPayload,
                Is.EqualTo(
                    exactUnknown));

            Assert.That(
                result.Batch.PayloadEntries[1]
                    .participantId,
                Is.EqualTo(
                    "com.example.inventory"));

            Assert.That(
                inventory.CaptureCalls,
                Is.Zero);

            Assert.That(
                inventory.ApplyCalls,
                Is.Zero);
        }

        [Test]
        public void MissingSnapshotProvenanceFailsClosed()
        {
            Register(
                Participant(
                    "com.example.inventory"));

            SaveUnknownPayloadSnapshot snapshot =
                new SaveUnknownPayloadSnapshot(
                    new[]
                    {
                        Entry(
                            "com.example.future",
                            "{\"value\":1}")
                    },
                    Entry(
                        "com.example.future",
                        "{\"value\":1}")
                        .byteLength);

            SaveUnknownPayloadMergeResult result =
                Merger().Merge(
                    Batch(
                        Pair(
                            "com.example.inventory",
                            "{\"gold\":1}")),
                    snapshot);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveUnknownPayloadMergeStatus
                        .MissingProvenance));

            Assert.That(
                result.Batch,
                Is.Null);
        }

        [Test]
        public void CanonicallyClaimedUnknownFailsWithCurrentOwner()
        {
            Register(
                Participant(
                    "com.example.inventory"));

            Register(
                Participant(
                    "com.example.future"));

            SaveUnknownPayloadMergeResult result =
                Merger().Merge(
                    Batch(
                        Pair(
                            "com.example.inventory",
                            "{\"gold\":1}")),
                    Snapshot(
                        Entry(
                            "com.example.future",
                            "{\"future\":true}")));

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveUnknownPayloadMergeStatus
                        .OwnershipCollision));

            Assert.That(
                result.FailingPersistedId.Value,
                Is.EqualTo(
                    "com.example.future"));

            Assert.That(
                result.CurrentOwnerId.Value,
                Is.EqualTo(
                    "com.example.future"));

            Assert.That(
                result.Batch,
                Is.Null);
        }

        [Test]
        public void AliasClaimedUnknownFailsWithCanonicalOwner()
        {
            Register(
                Participant(
                    "com.example.inventory"));

            Register(
                Participant(
                    "com.example.future",
                    new SaveParticipantId(
                        "com.example.oldfuture")));

            SaveUnknownPayloadMergeResult result =
                Merger().Merge(
                    Batch(
                        Pair(
                            "com.example.inventory",
                            "{\"gold\":1}")),
                    Snapshot(
                        Entry(
                            "com.example.oldfuture",
                            "{\"future\":true}")));

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveUnknownPayloadMergeStatus
                        .OwnershipCollision));

            Assert.That(
                result.FailingPersistedId.Value,
                Is.EqualTo(
                    "com.example.oldfuture"));

            Assert.That(
                result.CurrentOwnerId.Value,
                Is.EqualTo(
                    "com.example.future"));
        }

        [Test]
        public void FreshCaptureMustStillBelongToCanonicalRegisteredOwner()
        {
            SaveUnknownPayloadMergeResult result =
                Merger().Merge(
                    Batch(
                        Pair(
                            "com.example.inventory",
                            "{\"gold\":1}")),
                    Snapshot(
                        Entry(
                            "com.example.future",
                            "{\"future\":true}")));

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveUnknownPayloadMergeStatus
                        .FreshCaptureInvalid));
        }

        [Test]
        public void CorruptUnknownChecksumFailsBeforeMergedBatchExposure()
        {
            Register(
                Participant(
                    "com.example.inventory"));

            SavePayloadEntry unknown =
                Entry(
                    "com.example.future",
                    "{\"future\":true}");

            unknown.checksum =
                new string(
                    '0',
                    64);

            SaveUnknownPayloadMergeResult result =
                Merger().Merge(
                    Batch(
                        Pair(
                            "com.example.inventory",
                            "{\"gold\":1}")),
                    Snapshot(
                        unknown));

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveUnknownPayloadMergeStatus
                        .UnknownPayloadInvalid));

            Assert.That(
                result.Batch,
                Is.Null);
        }

        [Test]
        public void DuplicateUnknownIdsFailBeforeMergedBatchExposure()
        {
            Register(
                Participant(
                    "com.example.inventory"));

            SavePayloadEntry first =
                Entry(
                    "com.example.future",
                    "{\"future\":1}");

            SavePayloadEntry second =
                Entry(
                    "com.example.future",
                    "{\"future\":2}");

            SaveUnknownPayloadSnapshot snapshot =
                Snapshot(
                    first,
                    second);

            SaveUnknownPayloadMergeResult result =
                Merger().Merge(
                    Batch(
                        Pair(
                            "com.example.inventory",
                            "{\"gold\":1}")),
                    snapshot);

            Assert.That(
                result.Succeeded,
                Is.False);

            Assert.That(
                result.Batch,
                Is.Null);
        }

        [Test]
        public void MergedBatchAccessIsDefensivelyCopied()
        {
            Register(
                Participant(
                    "com.example.inventory"));

            SaveUnknownPayloadMergeResult result =
                Merger().Merge(
                    Batch(
                        Pair(
                            "com.example.inventory",
                            "{\"gold\":1}")),
                    Snapshot(
                        Entry(
                            "com.example.future",
                            "{\"future\":true}")));

            Assert.That(
                result.Succeeded,
                Is.True);

            SavePayloadEntry firstRead =
                result.Batch.PayloadEntries[0];

            string originalId =
                firstRead.participantId;

            firstRead.participantId =
                "com.example.mutated";

            Assert.That(
                result.Batch.PayloadEntries[0]
                    .participantId,
                Is.EqualTo(
                    originalId));
        }

        [Test]
        public void UnknownMetadataIsCarriedForwardWithoutRecomputation()
        {
            Register(
                Participant(
                    "com.example.inventory"));

            SavePayloadEntry unknown =
                Entry(
                    "com.example.future",
                    "{ \"future\" : 42 }",
                    9,
                    false);

            SaveUnknownPayloadMergeResult result =
                Merger().Merge(
                    Batch(
                        Pair(
                            "com.example.inventory",
                            "{\"gold\":1}")),
                    Snapshot(
                        unknown));

            Assert.That(
                result.Succeeded,
                Is.True);

            SavePayloadEntry preserved =
                result.Batch.PayloadEntries[0];

            Assert.That(
                preserved.participantId,
                Is.EqualTo(
                    unknown.participantId));

            Assert.That(
                preserved.participantSchemaVersion,
                Is.EqualTo(
                    unknown.participantSchemaVersion));

            Assert.That(
                preserved.serializerId,
                Is.EqualTo(
                    unknown.serializerId));

            Assert.That(
                preserved.required,
                Is.EqualTo(
                    unknown.required));

            Assert.That(
                preserved.serializedPayload,
                Is.EqualTo(
                    unknown.serializedPayload));

            Assert.That(
                preserved.byteLength,
                Is.EqualTo(
                    unknown.byteLength));

            Assert.That(
                preserved.checksum,
                Is.EqualTo(
                    unknown.checksum));

            Assert.That(
                preserved.flags,
                Is.EqualTo(
                    unknown.flags));
        }

        private SaveUnknownPayloadCarryForwardMerger
            Merger() =>
            new SaveUnknownPayloadCarryForwardMerger(
                integrity,
                registry);

        private void Register(
            TrackingParticipant participant)
        {
            Assert.That(
                registry.Register(
                    participant)
                    .Succeeded,
                Is.True);
        }

        private TrackingParticipant Participant(
            string id,
            params SaveParticipantId[] aliases) =>
            new TrackingParticipant(
                new SaveParticipantDescriptor(
                    new SaveParticipantId(
                        id),
                    1,
                    SaveParticipantCriticality.Required,
                    SaveMissingPayloadPolicy
                        .InitializeDefault,
                    default,
                    aliases));

        private SaveUnknownPayloadSnapshot Snapshot(
            params SavePayloadEntry[] entries)
        {
            long total =
                0L;

            for (int i = 0;
                 i < entries.Length;
                 i++)
            {
                total +=
                    entries[i].byteLength;
            }

            return new SaveUnknownPayloadSnapshot(
                entries,
                total,
                slotId,
                generationId,
                true);
        }

        private SaveParticipantCaptureBatchResult Batch(
            params EntryPair[] pairs)
        {
            Array.Sort(
                pairs,
                (left, right) =>
                    string.Compare(
                        left.Payload.participantId,
                        right.Payload.participantId,
                        StringComparison.Ordinal));

            SavePayloadEntry[] payload =
                new SavePayloadEntry[
                    pairs.Length];

            SavePayloadInventoryEntry[] inventory =
                new SavePayloadInventoryEntry[
                    pairs.Length];

            long total =
                0L;

            for (int i = 0;
                 i < pairs.Length;
                 i++)
            {
                payload[i] =
                    pairs[i].Payload;

                inventory[i] =
                    pairs[i].Inventory;

                total +=
                    payload[i].byteLength;
            }

            return SaveParticipantCaptureBatchResult
                .Success(
                    payload,
                    inventory,
                    total);
        }

        private EntryPair Pair(
            string participantId,
            string serializedPayload)
        {
            SavePayloadEntry payload =
                Entry(
                    participantId,
                    serializedPayload);

            return new EntryPair(
                payload,
                Inventory(
                    payload));
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

        private static SavePayloadInventoryEntry Inventory(
            SavePayloadEntry payload) =>
            new SavePayloadInventoryEntry
            {
                participantId =
                    payload.participantId,
                participantSchemaVersion =
                    payload.participantSchemaVersion,
                serializerId =
                    payload.serializerId,
                required =
                    payload.required,
                byteLength =
                    payload.byteLength,
                checksum =
                    payload.checksum,
                flags =
                    payload.flags
            };

        private readonly struct EntryPair
        {
            internal EntryPair(
                SavePayloadEntry payload,
                SavePayloadInventoryEntry inventory)
            {
                Payload =
                    payload;

                Inventory =
                    inventory;
            }

            internal SavePayloadEntry Payload { get; }

            internal SavePayloadInventoryEntry Inventory { get; }
        }

        private sealed class TrackingParticipant :
            ISaveParticipant
        {
            internal TrackingParticipant(
                SaveParticipantDescriptor descriptor)
            {
                Descriptor =
                    descriptor;
            }

            public SaveParticipantDescriptor Descriptor { get; }

            internal int CaptureCalls { get; private set; }

            internal int ApplyCalls { get; private set; }

            public SaveParticipantCaptureResult Capture()
            {
                CaptureCalls++;

                return SaveParticipantCaptureResult
                    .Failure(
                        "Carry-forward merger must not invoke participant capture.");
            }

            public SaveParticipantApplyResult Apply(
                object detachedState)
            {
                ApplyCalls++;

                return SaveParticipantApplyResult
                    .Failure(
                        "Carry-forward merger must not invoke participant apply.");
            }
        }
    }
}
