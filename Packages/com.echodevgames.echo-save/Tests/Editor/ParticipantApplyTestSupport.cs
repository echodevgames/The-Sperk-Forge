
using System;
using System.Collections.Generic;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    internal sealed class ParticipantApplyTestState
    {
        internal ParticipantApplyTestState(
            int value)
        {
            Value = value;
        }

        internal int Value { get; }
    }

    internal class ParticipantApplyTestParticipant :
        ISaveTypedParticipant
    {
        internal ParticipantApplyTestParticipant(
            string participantId,
            SaveMissingPayloadPolicy missingPayloadPolicy =
                SaveMissingPayloadPolicy.Ignore,
            int schemaVersion = 1)
        {
            Descriptor =
                new SaveParticipantDescriptor(
                    new SaveParticipantId(
                        participantId),
                    schemaVersion,
                    SaveParticipantCriticality.Required,
                    missingPayloadPolicy,
                    default);

            DetachedStateType =
                typeof(ParticipantApplyTestState);

            ApplyResult =
                SaveParticipantApplyResult.Success();
        }

        public SaveParticipantDescriptor Descriptor { get; }

        public Type DetachedStateType { get; set; }

        internal int CaptureCalls { get; private set; }

        internal int ApplyCalls { get; private set; }

        internal object LastAppliedState { get; private set; }

        internal bool ThrowOnApply { get; set; }

        internal SaveParticipantApplyResult ApplyResult { get; set; }

        internal Action BeforeApply { get; set; }

        public SaveParticipantCaptureResult Capture()
        {
            CaptureCalls++;

            return SaveParticipantCaptureResult.Success(
                new ParticipantApplyTestState(
                    0));
        }

        public SaveParticipantApplyResult Apply(
            object detachedState)
        {
            ApplyCalls++;
            LastAppliedState =
                detachedState;

            BeforeApply?.Invoke();

            if (ThrowOnApply)
            {
                throw new InvalidOperationException(
                    "apply-boom");
            }

            return ApplyResult;
        }
    }

    internal sealed class DefaultableParticipantApplyTestParticipant :
        ParticipantApplyTestParticipant,
        ISaveDefaultableParticipant
    {
        internal DefaultableParticipantApplyTestParticipant(
            string participantId,
            SaveMissingPayloadPolicy missingPayloadPolicy =
                SaveMissingPayloadPolicy.InitializeDefault,
            int schemaVersion = 1)
            : base(
                participantId,
                missingPayloadPolicy,
                schemaVersion)
        {
            DefaultResult =
                SaveParticipantApplyResult.Success();
        }

        internal int InitializeDefaultCalls { get; private set; }

        internal bool ThrowOnDefault { get; set; }

        internal SaveParticipantApplyResult DefaultResult { get; set; }

        internal Action BeforeDefault { get; set; }

        public SaveParticipantApplyResult InitializeDefault()
        {
            InitializeDefaultCalls++;

            BeforeDefault?.Invoke();

            if (ThrowOnDefault)
            {
                throw new InvalidOperationException(
                    "default-boom");
            }

            return DefaultResult;
        }
    }

    internal static class ParticipantApplyTestSupport
    {
        internal static SavePreparedParticipantEntry PreparedEntry(
            string participantId,
            int value = 1,
            int schemaVersion = 1,
            Type detachedStateType = null,
            object detachedState = null)
        {
            Type stateType =
                detachedStateType ??
                typeof(ParticipantApplyTestState);

            object state =
                detachedState ??
                new ParticipantApplyTestState(
                    value);

            SaveParticipantId id =
                new SaveParticipantId(
                    participantId);

            return new SavePreparedParticipantEntry(
                id,
                id,
                schemaVersion,
                new SaveSerializerId(
                    UnityJsonSaveSerializer.StableId),
                stateType,
                state);
        }

        internal static PreparedSaveLoad CreateHandle(
            SavePreparedLoadStore store,
            SaveSlotId slotId,
            SaveGenerationId generationId,
            params SavePreparedParticipantEntry[] entries)
        {
            SavePreparedParticipantEntry[] safeEntries =
                entries ??
                Array.Empty<SavePreparedParticipantEntry>();

            SavePayloadEntry[] validatedEntries =
                new SavePayloadEntry[
                    safeEntries.Length];

            for (int i = 0;
                 i < safeEntries.Length;
                 i++)
            {
                validatedEntries[i] =
                    new SavePayloadEntry
                    {
                        participantId =
                            safeEntries[i]
                                .CanonicalParticipantId
                                .Value,
                        participantSchemaVersion =
                            safeEntries[i]
                                .ParticipantSchemaVersion,
                        serializerId =
                            safeEntries[i]
                                .SerializerId
                                .Value,
                        required =
                            true,
                        serializedPayload =
                            "{}",
                        byteProviderReference =
                            string.Empty,
                        byteLength =
                            10,
                        checksum =
                            new string(
                                'a',
                                64),
                        flags =
                            0
                    };
            }

            SaveValidatedParticipantSnapshot validated =
                new SaveValidatedParticipantSnapshot(
                    slotId,
                    generationId,
                    validatedEntries);

            SaveCurrentGenerationReadResult read =
                new SaveCurrentGenerationReadResult(
                    SaveCurrentGenerationReadStatus.Succeeded,
                    string.Empty,
                    "test",
                    slotId,
                    generationId,
                    safeEntries.Length,
                    0,
                    validated);

            SavePreparedParticipantBatch batch =
                new SavePreparedParticipantBatch(
                    slotId,
                    generationId,
                    safeEntries);

            PreparedLoadCreationResult result =
                store.TryCreate(
                    read,
                    SaveParticipantPreparationResult.Success(
                        batch),
                    null);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    result.Message);
            }

            return result.Handle;
        }

        internal static PreparedSaveLoad CreateUnknownOnlyHandle(
            SavePreparedLoadStore store,
            SaveSlotId slotId,
            SaveGenerationId generationId)
        {
            SavePayloadEntry unknownEntry =
                new SavePayloadEntry
                {
                    participantId =
                        "com.example.unknown",
                    participantSchemaVersion =
                        1,
                    serializerId =
                        UnityJsonSaveSerializer.StableId,
                    required =
                        false,
                    serializedPayload =
                        "{}",
                    byteProviderReference =
                        string.Empty,
                    byteLength =
                        10,
                    checksum =
                        new string(
                            'a',
                            64),
                    flags =
                        0
                };

            SaveValidatedParticipantSnapshot validated =
                new SaveValidatedParticipantSnapshot(
                    slotId,
                    generationId,
                    new[]
                    {
                        unknownEntry
                    });

            SaveCurrentGenerationReadResult read =
                new SaveCurrentGenerationReadResult(
                    SaveCurrentGenerationReadStatus.Succeeded,
                    string.Empty,
                    "test",
                    slotId,
                    generationId,
                    0,
                    1,
                    validated);

            SavePreparedParticipantBatch batch =
                new SavePreparedParticipantBatch(
                    slotId,
                    generationId,
                    Array.Empty<SavePreparedParticipantEntry>());

            SaveUnknownPayloadSnapshot unknown =
                new SaveUnknownPayloadSnapshot(
                    new[]
                    {
                        SaveUnknownPayloadSnapshot.CloneEntry(
                            unknownEntry)
                    },
                    10L,
                    slotId,
                    generationId,
                    true);

            PreparedLoadCreationResult result =
                store.TryCreate(
                    read,
                    SaveParticipantPreparationResult.Success(
                        batch),
                    unknown);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    result.Message);
            }

            return result.Handle;
        }

        internal static SavePreparedLoadStore Store(
            FakePreparedLoadClock clock) =>
            new SavePreparedLoadStore(
                clock,
                TimeSpan.FromMinutes(5),
                8,
                1024 * 1024);
    }
}
