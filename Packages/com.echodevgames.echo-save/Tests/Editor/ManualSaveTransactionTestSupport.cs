using System;
using System.Text;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    internal sealed class ManualSaveTransactionTestEnvironment :
        IDisposable
    {
        internal ManualSaveTransactionTestEnvironment()
        {
            Storage =
                new SlotCreationTestEnvironment();

            Registry =
                new SaveParticipantRegistry();

            UnknownStore =
                new SaveUnknownPayloadStore();

            SerializerRegistry =
                new SaveSerializerRegistry();

            Publication =
                Storage.CreatePublicationCoordinator();

            CurrentReader =
                new SaveCurrentGenerationReader(
                    Storage.Backend,
                    Storage.Serializer,
                    Storage.Integrity,
                    Registry,
                    UnknownStore);

            CaptureCoordinator =
                new SaveParticipantCaptureCoordinator(
                    SerializerRegistry,
                    Storage.Integrity);

            CarryForward =
                new SaveUnknownPayloadCarryForwardCoordinator(
                    Storage.Backend,
                    Storage.Serializer,
                    Storage.Integrity,
                    Registry,
                    Publication);

            Coordinator =
                new SaveManualTransactionCoordinator(
                    Storage.Catalog,
                    CurrentReader,
                    CaptureCoordinator,
                    Registry,
                    UnknownStore,
                    CarryForward);
        }

        internal SlotCreationTestEnvironment Storage { get; }

        internal SaveParticipantRegistry Registry { get; }

        internal SaveUnknownPayloadStore UnknownStore { get; }

        internal SaveSerializerRegistry SerializerRegistry { get; }

        internal SaveGenerationPublicationCoordinator Publication { get; }

        internal SaveCurrentGenerationReader CurrentReader { get; }

        internal SaveParticipantCaptureCoordinator CaptureCoordinator { get; }

        internal SaveUnknownPayloadCarryForwardCoordinator CarryForward { get; }

        internal SaveManualTransactionCoordinator Coordinator { get; }

        internal SaveManualTransactionRequest Request(
            string projectId = "com.example.game",
            string projectVersion = "1.1.0",
            string buildId = "build-m4-03") =>
            new SaveManualTransactionRequest(
                projectId,
                projectVersion,
                buildId);

        internal CreatedSlot CreateEmptySlot(
            string displayName = "Manual Save",
            bool select = true)
        {
            SaveSlotId slotId =
                SaveSlotId.NewId();

            SaveTechnicalSlotCreateResult created =
                Storage.CreateSlotCoordinator(
                        capacity: 16,
                        maxIdAttempts: 1,
                        slotIdFactory: () => slotId)
                    .Create(
                        SlotCreationTestEnvironment.Request(
                            displayName));

            Require(
                created.Succeeded,
                "Could not create the M4-03 test slot.");

            if (select)
            {
                Require(
                    Storage.Catalog
                        .SelectActiveSlot(
                            slotId)
                        .Succeeded,
                    "Could not select the M4-03 test slot.");
            }

            return new CreatedSlot(
                slotId,
                created.GenerationId,
                displayName);
        }

        internal CreatedSlot InstallParticipantSource(
            string persistedParticipantId,
            string serializedPayload,
            string displayName = "Preserved Slot",
            bool select = true)
        {
            SaveSlotId slotId =
                SaveSlotId.NewId();

            SaveParticipantCaptureBatchResult batch =
                SyntheticBatch(
                    persistedParticipantId,
                    serializedPayload);

            SaveGenerationPublicationResult published =
                Publication
                    .PublishParticipantTransportGeneration(
                        slotId,
                        "com.example.game",
                        "1.0.0",
                        "source-build",
                        displayName,
                        batch);

            Require(
                published.Succeeded,
                "Could not publish the participant-backed M4-03 source generation.");

            SaveSlotCatalogRefreshResult refresh =
                Storage.Catalog.Refresh();

            Require(
                refresh.Succeeded,
                "Could not refresh the M4-03 source slot into the catalog.");

            if (select)
            {
                Require(
                    Storage.Catalog
                        .SelectActiveSlot(
                            slotId)
                        .Succeeded,
                    "Could not select the participant-backed M4-03 source slot.");
            }

            return new CreatedSlot(
                slotId,
                published.GenerationId,
                displayName);
        }

        internal SaveGenerationPublicationResult
            PublishInterveningGeneration(
                SaveSlotId slotId,
                string displayName)
        {
            return Publication
                .PublishParticipantTransportGeneration(
                    slotId,
                    "com.example.game",
                    "1.0.1",
                    "intervening-build",
                    displayName,
                    SyntheticBatch(
                        "com.example.intervening",
                        "{\"marker\":313}"));
        }

        internal SaveParticipantCaptureBatchResult SyntheticBatch(
            string participantId,
            string serializedPayload)
        {
            byte[] bytes =
                Encoding.UTF8.GetBytes(
                    serializedPayload);

            SaveIntegrityResult integrity =
                Storage.Integrity.Calculate(
                    bytes,
                    out string checksum);

            Require(
                integrity.Succeeded,
                "Could not calculate test payload integrity.");

            SavePayloadEntry payload =
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

            SavePayloadInventoryEntry inventory =
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

            return SaveParticipantCaptureBatchResult
                .Success(
                    new[]
                    {
                        payload
                    },
                    new[]
                    {
                        inventory
                    },
                    payload.byteLength);
        }

        internal SaveHeadPointer ReadHead(
            SaveSlotId slotId)
        {
            SaveStorageReadResult read =
                Storage.ReadHead(
                    slotId);

            Require(
                read.Succeeded,
                "Expected Chronicle head was not readable.");

            SaveSerializerResult deserialize =
                Storage.Serializer.Deserialize(
                    Encoding.UTF8.GetString(
                        read.Data),
                    out SaveHeadPointer head);

            Require(
                deserialize.Succeeded,
                "Expected Chronicle head was not deserializable.");

            return head;
        }

        internal void DeleteHead(
            SaveSlotId slotId)
        {
            SaveStorageKey.TryCreate(
                "slots/" +
                slotId.Value +
                "/head.json",
                out SaveStorageKey headKey);

            Require(
                Storage.Local.Delete(
                    headKey)
                    .Succeeded,
                "Could not delete the M4-03 test head.");
        }

        internal ManualSaveTestParticipant Participant(
            string id = "com.example.inventory",
            int value = 100,
            Action onCapture = null,
            bool failCapture = false)
        {
            return new ManualSaveTestParticipant(
                id,
                value,
                onCapture,
                failCapture);
        }

        internal void Register(
            ISaveParticipant participant)
        {
            Require(
                Registry.Register(
                        participant)
                    .Succeeded,
                "Could not register the M4-03 test participant.");
        }

        public void Dispose()
        {
            Storage.Dispose();
        }

        private static void Require(
            bool condition,
            string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(
                    message);
            }
        }

        internal readonly struct CreatedSlot
        {
            internal CreatedSlot(
                SaveSlotId slotId,
                SaveGenerationId generationId,
                string displayName)
            {
                SlotId =
                    slotId;

                GenerationId =
                    generationId;

                DisplayName =
                    displayName;
            }

            internal SaveSlotId SlotId { get; }

            internal SaveGenerationId GenerationId { get; }

            internal string DisplayName { get; }
        }
    }

    [Serializable]
    internal sealed class ManualSaveTestDto
    {
        public int value;
    }

    internal sealed class ManualSaveTestParticipant :
        ISaveTypedParticipant
    {
        private readonly Action onCapture;
        private readonly bool failCapture;
        private readonly int value;

        internal ManualSaveTestParticipant(
            string id,
            int value,
            Action onCapture,
            bool failCapture)
        {
            Descriptor =
                new SaveParticipantDescriptor(
                    new SaveParticipantId(
                        id),
                    1,
                    SaveParticipantCriticality.Required,
                    SaveMissingPayloadPolicy.InitializeDefault,
                    default);

            DetachedStateType =
                typeof(ManualSaveTestDto);

            this.value =
                value;

            this.onCapture =
                onCapture;

            this.failCapture =
                failCapture;
        }

        public SaveParticipantDescriptor Descriptor { get; }

        public Type DetachedStateType { get; }

        internal int CaptureCalls { get; private set; }

        internal int ApplyCalls { get; private set; }

        public SaveParticipantCaptureResult Capture()
        {
            CaptureCalls++;

            onCapture?.Invoke();

            return failCapture
                ? SaveParticipantCaptureResult
                    .Failure(
                        "Fault-injected M4-03 participant capture failure.")
                : SaveParticipantCaptureResult
                    .Success(
                        new ManualSaveTestDto
                        {
                            value =
                                value
                        });
        }

        public SaveParticipantApplyResult Apply(
            object detachedState)
        {
            ApplyCalls++;

            return SaveParticipantApplyResult.Success();
        }
    }
}
