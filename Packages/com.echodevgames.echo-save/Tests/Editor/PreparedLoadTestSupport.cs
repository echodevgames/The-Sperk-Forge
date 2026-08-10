
using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    internal sealed class FakePreparedLoadClock : IPreparedLoadClock
    {
        internal FakePreparedLoadClock(
            DateTimeOffset utcNow)
        {
            UtcNow = utcNow;
        }

        public DateTimeOffset UtcNow { get; private set; }

        internal void Advance(
            TimeSpan amount)
        {
            UtcNow =
                UtcNow.Add(
                    amount);
        }
    }

    internal sealed class PreparedLoadArtifacts
    {
        private PreparedLoadArtifacts(
            SaveCurrentGenerationReadResult readResult,
            SaveParticipantPreparationResult preparationResult,
            SaveUnknownPayloadSnapshot unknownSnapshot)
        {
            ReadResult = readResult;
            PreparationResult = preparationResult;
            UnknownSnapshot = unknownSnapshot;
        }

        internal SaveCurrentGenerationReadResult ReadResult { get; }

        internal SaveParticipantPreparationResult PreparationResult { get; }

        internal SaveUnknownPayloadSnapshot UnknownSnapshot { get; }

        internal static PreparedLoadArtifacts Create(
            SaveSlotId slotId,
            SaveGenerationId generationId,
            int knownCount,
            int unknownCount)
        {
            List<SavePayloadEntry> all =
                new List<SavePayloadEntry>();

            List<SavePayloadEntry> unknown =
                new List<SavePayloadEntry>();

            for (int i = 0;
                 i < knownCount;
                 i++)
            {
                all.Add(
                    Entry(
                        "com.example.known" + i,
                        10));
            }

            for (int i = 0;
                 i < unknownCount;
                 i++)
            {
                SavePayloadEntry entry =
                    Entry(
                        "com.example.unknown" + i,
                        10);

                all.Add(
                    entry);

                unknown.Add(
                    SaveUnknownPayloadSnapshot
                        .CloneEntry(
                            entry));
            }

            SaveValidatedParticipantSnapshot validated =
                new SaveValidatedParticipantSnapshot(
                    slotId,
                    generationId,
                    all.ToArray());

            SaveCurrentGenerationReadResult read =
                new SaveCurrentGenerationReadResult(
                    SaveCurrentGenerationReadStatus.Succeeded,
                    string.Empty,
                    "test",
                    slotId,
                    generationId,
                    knownCount,
                    unknownCount,
                    validated);

            SavePreparedParticipantBatch batch =
                CreatePreparedBatch(
                    slotId,
                    generationId,
                    knownCount);

            SaveParticipantPreparationResult preparation =
                SaveParticipantPreparationResult
                    .Success(
                        batch);

            SaveUnknownPayloadSnapshot snapshot =
                new SaveUnknownPayloadSnapshot(
                    unknown.ToArray(),
                    unknownCount * 10L,
                    slotId,
                    generationId,
                    true);

            return new PreparedLoadArtifacts(
                read,
                preparation,
                snapshot);
        }

        internal static SavePreparedParticipantBatch
            CreatePreparedBatch(
                SaveSlotId slotId,
                SaveGenerationId generationId,
                int count)
        {
            SavePreparedParticipantEntry[] entries =
                new SavePreparedParticipantEntry[
                    count];

            for (int i = 0;
                 i < count;
                 i++)
            {
                entries[i] =
                    new SavePreparedParticipantEntry(
                        new SaveParticipantId(
                            "com.example.known" + i),
                        new SaveParticipantId(
                            "com.example.known" + i),
                        1,
                        new SaveSerializerId(
                            UnityJsonSaveSerializer.StableId),
                        typeof(TestDetachedState),
                        new TestDetachedState
                        {
                            value = i
                        });
            }

            return new SavePreparedParticipantBatch(
                slotId,
                generationId,
                entries);
        }

        internal static SaveUnknownPayloadSnapshot
            CreateUnknownSnapshot(
                SaveSlotId slotId,
                SaveGenerationId generationId,
                int count)
        {
            SavePayloadEntry[] entries =
                new SavePayloadEntry[
                    count];

            for (int i = 0;
                 i < count;
                 i++)
            {
                entries[i] =
                    Entry(
                        "com.example.unknown" + i,
                        10);
            }

            return new SaveUnknownPayloadSnapshot(
                entries,
                count * 10L,
                slotId,
                generationId,
                true);
        }

        private static SavePayloadEntry Entry(
            string participantId,
            int byteLength) =>
            new SavePayloadEntry
            {
                participantId =
                    participantId,
                participantSchemaVersion =
                    1,
                serializerId =
                    UnityJsonSaveSerializer.StableId,
                required =
                    true,
                serializedPayload =
                    "{}",
                byteProviderReference =
                    string.Empty,
                byteLength =
                    byteLength,
                checksum =
                    new string(
                        'a',
                        64),
                flags =
                    0
            };

        [Serializable]
        private sealed class TestDetachedState
        {
            public int value;
        }
    }
}
