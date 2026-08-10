using System;
using UnityEngine;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    internal sealed class PublicManualSaveServiceTestEnvironment :
        IDisposable
    {
        internal PublicManualSaveServiceTestEnvironment()
        {
            Configuration =
                ScriptableObject.CreateInstance<
                    EchoSaveConfiguration>();

            Configuration.SetDefinitionForTesting(
                EchoSaveConfiguration
                    .CurrentSchemaVersion,
                "EchoSave");

            Backend =
                new PublicManualSaveTestBackend();

            Service =
                new EchoSaveService(
                    Configuration);

            Service.SetStorageBackendFactory(
                new PublicManualSaveTestBackendFactory(
                    Backend));
        }

        internal EchoSaveConfiguration Configuration { get; }

        internal PublicManualSaveTestBackend Backend { get; }

        internal EchoSaveService Service { get; }

        internal FakeManualSaveTransactionExecutor Executor { get; private set; }

        internal EchoSaveLifecycleResult Initialize(
            Func<
                SaveManualTransactionRequest,
                SaveManualTransactionControl,
                SaveManualTransactionResult> handler = null)
        {
            EchoSaveLifecycleResult result =
                Service.InitializeCore();

            if (result.Succeeded)
            {
                Executor =
                    new FakeManualSaveTransactionExecutor(
                        handler ??
                        ((request, control) =>
                            PublicManualSaveResultFactory
                                .Succeeded()));

                Service.SetManualSaveTransactionExecutorForTesting(
                    Executor);
            }

            return result;
        }

        internal SaveRequest Request(
            string projectId = "com.example.game",
            string projectVersion = "1.0.0",
            string buildId = "build-m4-04",
            System.Threading.CancellationToken cancellationToken =
                default) =>
            new SaveRequest(
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

            if (Configuration != null)
            {
                UnityEngine.Object.DestroyImmediate(
                    Configuration);
            }
        }
    }

    internal sealed class FakeManualSaveTransactionExecutor :
        ISaveManualTransactionExecutor
    {
        private readonly Func<
            SaveManualTransactionRequest,
            SaveManualTransactionControl,
            SaveManualTransactionResult> handler;

        internal FakeManualSaveTransactionExecutor(
            Func<
                SaveManualTransactionRequest,
                SaveManualTransactionControl,
                SaveManualTransactionResult> handler)
        {
            this.handler =
                handler ??
                throw new ArgumentNullException(
                    nameof(handler));
        }

        internal int Calls { get; private set; }

        SaveManualTransactionResult
            ISaveManualTransactionExecutor.Save(
                SaveManualTransactionRequest request,
                SaveManualTransactionControl control)
        {
            Calls++;

            return handler(
                request,
                control);
        }
    }

    internal static class PublicManualSaveResultFactory
    {
        internal static SaveManualTransactionResult Succeeded(
            SaveSlotId slotId = default,
            SaveGenerationId sourceGenerationId = default,
            SaveGenerationId publishedGenerationId = default)
        {
            if (string.IsNullOrEmpty(
                    slotId.Value))
            {
                slotId =
                    SaveSlotId.NewId();
            }

            if (string.IsNullOrEmpty(
                    sourceGenerationId.Value))
            {
                sourceGenerationId =
                    SaveGenerationId.NewId();
            }

            if (string.IsNullOrEmpty(
                    publishedGenerationId.Value))
            {
                publishedGenerationId =
                    SaveGenerationId.NewId();
            }

            return new SaveManualTransactionResult(
                SaveManualTransactionStatus.Succeeded,
                string.Empty,
                "Manual save succeeded.",
                slotId,
                sourceGenerationId,
                publishedGenerationId,
                default,
                default,
                2,
                1,
                313L,
                true,
                true,
                true,
                null);
        }

        internal static SaveManualTransactionResult Failure(
            SaveManualTransactionStatus status,
            string diagnosticCode = "ESV-TEST-001",
            bool generationPublished = false,
            bool headPublished = false,
            bool catalogReconciled = false)
        {
            return new SaveManualTransactionResult(
                status,
                diagnosticCode,
                "Manual save failed for the requested test condition.",
                SaveSlotId.NewId(),
                SaveGenerationId.NewId(),
                generationPublished
                    ? SaveGenerationId.NewId()
                    : default,
                default,
                default,
                1,
                0,
                55L,
                generationPublished,
                headPublished,
                catalogReconciled,
                null);
        }
    }

    internal sealed class PublicManualSaveTestBackendFactory :
        IEchoSaveStorageBackendFactory
    {
        private readonly PublicManualSaveTestBackend backend;

        internal PublicManualSaveTestBackendFactory(
            PublicManualSaveTestBackend backend)
        {
            this.backend =
                backend;
        }

        public SaveStorageResult TryCreate(
            EchoSaveConfiguration configuration,
            out ISaveStorageBackend created)
        {
            created =
                backend;

            return SaveStorageResult.Success(
                "Public manual-save test backend created.");
        }
    }

    internal sealed class PublicManualSaveTestBackend :
        ISaveStorageBackend
    {
        private static readonly SaveStorageBackendId BackendId =
            new SaveStorageBackendId(
                "test.public-manual-save");

        internal int InitializeCalls { get; private set; }

        internal int ShutdownCalls { get; private set; }

        public SaveStorageBackendId Id =>
            BackendId;

        public string RootPath =>
            "memory://public-manual-save";

        public SaveStorageResult Initialize()
        {
            InitializeCalls++;

            return SaveStorageResult.Success(
                "Public manual-save test backend initialized.");
        }

        public SaveStorageResult Exists(
            SaveStorageKey key,
            out bool exists)
        {
            exists =
                false;

            return SaveStorageResult.Success(
                "No test storage object exists.");
        }

        public SaveStorageReadResult Read(
            SaveStorageKey key) =>
            new SaveStorageReadResult(
                new SaveStorageResult(
                    SaveStorageStatus.NotFound,
                    string.Empty,
                    "The test storage object does not exist."),
                Array.Empty<byte>());

        public SaveStorageResult WriteNew(
            SaveStorageKey key,
            byte[] data) =>
            SaveStorageResult.Success(
                "Test bytes accepted.");

        public SaveStorageResult Delete(
            SaveStorageKey key) =>
            SaveStorageResult.NoChange(
                "No test bytes required deletion.");

        public SaveStorageResult Shutdown()
        {
            ShutdownCalls++;

            return SaveStorageResult.Success(
                "Public manual-save test backend shut down.");
        }
    }
}
