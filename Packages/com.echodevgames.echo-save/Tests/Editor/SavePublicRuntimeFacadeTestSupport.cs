
using System;
using UnityEngine;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    internal sealed class PublicRuntimeFacadeTestEnvironment :
        IDisposable
    {
        internal PublicRuntimeFacadeTestEnvironment()
        {
            Storage =
                new SlotCreationTestEnvironment();

            Configuration =
                ScriptableObject.CreateInstance<
                    EchoSaveConfiguration>();

            Configuration.SetDefinitionForTesting(
                EchoSaveConfiguration.CurrentSchemaVersion,
                "EchoSave");

            Service =
                new EchoSaveService(
                    Configuration);

            Service.SetStorageBackendFactory(
                new AutosaveTestBackendFactory(
                    Storage.Backend));
        }

        internal SlotCreationTestEnvironment Storage { get; }

        internal EchoSaveConfiguration Configuration { get; }

        internal EchoSaveService Service { get; }

        internal EchoSaveLifecycleResult Initialize() =>
            Service.InitializeCore();

        internal SaveSlotCreateRequest CreateRequest(
            string displayName = "R1 Public Slot",
            string projectId = "com.example.r1",
            string projectVersion = "1.0.0",
            string buildId = "r1") =>
            new SaveSlotCreateRequest(
                displayName,
                projectId,
                projectVersion,
                buildId);

        internal SaveSlotCreateResult CreateSlot(
            string displayName = "R1 Public Slot")
        {
            SaveSlotCreateResult result =
                Service.CreateSlotSynchronouslyForTesting(
                    CreateRequest(
                        displayName));

            if (!result.SlotPublished &&
                result.Status !=
                    SaveSlotCreateStatus.CapacityReached)
            {
                throw new InvalidOperationException(
                    "Could not create the R1 public test slot. " +
                    result.DiagnosticCode +
                    " " +
                    result.Message);
            }

            return result;
        }

        internal SaveSlotCreateResult CreateAndSelect(
            string displayName = "R1 Public Slot")
        {
            SaveSlotCreateResult created =
                CreateSlot(
                    displayName);

            if (!created.Succeeded)
            {
                throw new InvalidOperationException(
                    "Could not create a selectable R1 public test slot.");
            }

            SaveActiveSlotSelectionResult selection =
                Service.SelectSlotSynchronouslyForTesting(
                    created.SlotId);

            if (!selection.Succeeded ||
                !selection.HasActiveSlot)
            {
                throw new InvalidOperationException(
                    "Could not select the R1 public test slot. " +
                    selection.DiagnosticCode +
                    " " +
                    selection.Message);
            }

            return created;
        }

        internal SaveOperationResult SaveCurrent(
            string buildId = "r1-save") =>
            Service.SaveSynchronouslyForTesting(
                new SaveRequest(
                    "com.example.r1",
                    "1.0.0",
                    buildId));

        public void Dispose()
        {
            if (Service.State !=
                EchoSaveServiceState.Shutdown)
            {
                Service.ShutdownImmediate();
            }

            Storage.Dispose();

            if (Configuration != null)
            {
                UnityEngine.Object.DestroyImmediate(
                    Configuration);
            }
        }
    }

    [Serializable]
    internal sealed class PublicRuntimeFacadeState
    {
        public int value;
    }

    internal sealed class PublicRuntimeFacadeParticipant :
        ISaveTypedParticipant
    {
        private readonly SaveParticipantDescriptor descriptor;

        internal PublicRuntimeFacadeParticipant(
            string participantId,
            int value = 0,
            SaveMissingPayloadPolicy missingPayloadPolicy =
                SaveMissingPayloadPolicy.Fail,
            bool failApply = false,
            params string[] aliases)
        {
            SaveParticipantId[] aliasIds =
                aliases == null
                    ? Array.Empty<SaveParticipantId>()
                    : new SaveParticipantId[
                        aliases.Length];

            for (int i = 0;
                 i < aliasIds.Length;
                 i++)
            {
                aliasIds[i] =
                    new SaveParticipantId(
                        aliases[i]);
            }

            descriptor =
                new SaveParticipantDescriptor(
                    new SaveParticipantId(
                        participantId),
                    1,
                    SaveParticipantCriticality.Required,
                    missingPayloadPolicy,
                    default,
                    aliasIds);

            Value = value;
            FailApply = failApply;
        }

        public SaveParticipantDescriptor Descriptor =>
            descriptor;

        public Type DetachedStateType =>
            typeof(PublicRuntimeFacadeState);

        internal int Value { get; set; }

        internal bool FailApply { get; set; }

        internal int CaptureCalls { get; private set; }

        internal int ApplyCalls { get; private set; }

        public SaveParticipantCaptureResult Capture()
        {
            CaptureCalls++;

            return SaveParticipantCaptureResult.Success(
                new PublicRuntimeFacadeState
                {
                    value = Value
                });
        }

        public SaveParticipantApplyResult Apply(
            object detachedState)
        {
            ApplyCalls++;

            if (FailApply)
            {
                return SaveParticipantApplyResult.Failure(
                    "Injected R1 participant apply failure.",
                    "ESV-TEST-R1-APPLY");
            }

            PublicRuntimeFacadeState state =
                detachedState as
                    PublicRuntimeFacadeState;

            if (state == null)
            {
                return SaveParticipantApplyResult.Failure(
                    "R1 test participant received incompatible detached state.",
                    "ESV-TEST-R1-TYPE");
            }

            Value =
                state.value;

            return SaveParticipantApplyResult.Success();
        }
    }
}
