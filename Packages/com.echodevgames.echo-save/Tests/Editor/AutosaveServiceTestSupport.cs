using System;
using UnityEngine;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    internal sealed class AutosaveServiceTestEnvironment :
        IDisposable
    {
        internal AutosaveServiceTestEnvironment()
        {
            Storage =
                new SlotCreationTestEnvironment();

            Configuration =
                ScriptableObject.CreateInstance<
                    EchoSaveConfiguration>();

            Configuration.SetDefinitionForTesting(
                EchoSaveConfiguration
                    .CurrentSchemaVersion,
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

        internal FakeManualSaveTransactionExecutor Executor
        {
            get;
            private set;
        }

        internal SaveSlotId ActiveSlotId { get; private set; }

        internal EchoSaveLifecycleResult Initialize(
            bool selectActiveSlot = true,
            Func<
                SaveManualTransactionRequest,
                SaveManualTransactionControl,
                SaveManualTransactionResult> handler = null)
        {
            EchoSaveLifecycleResult result =
                Service.InitializeCore();

            if (!result.Succeeded)
            {
                return result;
            }

            if (selectActiveSlot)
            {
                SaveTechnicalSlotCreationCoordinator
                    slotCreation =
                        new SaveTechnicalSlotCreationCoordinator(
                            Service.SlotCatalogForTesting,
                            Storage.Backend,
                            Storage.Serializer,
                            Storage.Integrity,
                            8,
                            4);

                SaveTechnicalSlotCreateResult created =
                    slotCreation.Create(
                        SlotCreationTestEnvironment.Request(
                            "Autosave Test Slot",
                            "com.example.autosave",
                            "1.0.0",
                            "initial"));

                if (!created.Succeeded)
                {
                    throw new InvalidOperationException(
                        "Could not create the autosave test slot. " +
                        created.DiagnosticCode +
                        " " +
                        created.Message);
                }

                SaveActiveSlotSelectionResult selection =
                    Service.SlotCatalogForTesting
                        .SelectActiveSlot(
                            created.SlotId);

                if (!selection.Succeeded ||
                    !selection.HasActiveSlot)
                {
                    throw new InvalidOperationException(
                        "Could not select the autosave test slot. " +
                        selection.DiagnosticCode +
                        " " +
                        selection.Message);
                }

                ActiveSlotId =
                    created.SlotId;
            }

            Executor =
                new FakeManualSaveTransactionExecutor(
                    handler ??
                    ((request, control) =>
                        PublicManualSaveResultFactory
                            .Succeeded(
                                ActiveSlotId)));

            Service.SetManualSaveTransactionExecutorForTesting(
                Executor);

            return result;
        }

        internal AutosaveRequest Request(
            string projectId = "com.example.autosave",
            string projectVersion = "1.0.0",
            string buildId = "autosave-a",
            System.Threading.CancellationToken cancellationToken =
                default) =>
            new AutosaveRequest(
                projectId,
                projectVersion,
                buildId,
                cancellationToken);

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

    internal sealed class AutosaveTestBackendFactory :
        IEchoSaveStorageBackendFactory
    {
        private readonly ISaveStorageBackend backend;

        internal AutosaveTestBackendFactory(
            ISaveStorageBackend backend)
        {
            this.backend =
                backend ??
                throw new ArgumentNullException(
                    nameof(backend));
        }

        public SaveStorageResult TryCreate(
            EchoSaveConfiguration configuration,
            out ISaveStorageBackend created)
        {
            created =
                backend;

            return SaveStorageResult.Success(
                "Autosave test backend created.");
        }
    }
}
