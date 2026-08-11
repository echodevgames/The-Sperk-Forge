
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveManualRetentionIntegrationTests
    {
        [Test]
        public void RetentionRunsOnlyAfterSuccessfulHeadPublication()
        {
            using (ManualSaveTransactionTestEnvironment env =
                new ManualSaveTransactionTestEnvironment())
            {
                env.CreateEmptySlot();

                env.Register(
                    env.Participant());

                RecordingRetentionExecutor retention =
                    new RecordingRetentionExecutor(
                        SaveRetentionResult.NotRequired(
                            "No cleanup required."));

                SaveManualTransactionCoordinator coordinator =
                    new SaveManualTransactionCoordinator(
                        env.Storage.Catalog,
                        env.CurrentReader,
                        env.CaptureCoordinator,
                        env.Registry,
                        env.UnknownStore,
                        env.CarryForward,
                        retention,
                        new SaveRetentionPolicy(2));

                SaveManualTransactionResult result =
                    coordinator.Save(
                        env.Request());

                Assert.That(result.Succeeded, Is.True);
                Assert.That(result.HeadPublished, Is.True);
                Assert.That(retention.Calls, Is.EqualTo(1));
            }
        }

        [Test]
        public void FailedPublicationNeverInvokesRetention()
        {
            using (ManualSaveTransactionTestEnvironment env =
                new ManualSaveTransactionTestEnvironment())
            {
                env.CreateEmptySlot();

                env.Register(
                    env.Participant());

                RecordingRetentionExecutor retention =
                    new RecordingRetentionExecutor(
                        SaveRetentionResult.NotRequired(
                            "No cleanup required."));

                SaveManualTransactionCoordinator coordinator =
                    new SaveManualTransactionCoordinator(
                        env.Storage.Catalog,
                        env.CurrentReader,
                        env.CaptureCoordinator,
                        env.Registry,
                        env.UnknownStore,
                        env.CarryForward,
                        retention,
                        new SaveRetentionPolicy(2));

                env.Storage.Backend.Fault =
                    SlotCreationFaultPoint.HeadPublication;

                SaveManualTransactionResult result =
                    coordinator.Save(
                        env.Request());

                Assert.That(result.Succeeded, Is.False);
                Assert.That(result.HeadPublished, Is.False);
                Assert.That(retention.Calls, Is.Zero);
            }
        }

        [Test]
        public void RetentionFailureNeverFabricatesSaveRollback()
        {
            using (ManualSaveTransactionTestEnvironment env =
                new ManualSaveTransactionTestEnvironment())
            {
                env.CreateEmptySlot();

                env.Register(
                    env.Participant());

                SaveRetentionResult maintenanceFailure =
                    new SaveRetentionResult(
                        SaveRetentionStatus.Failed,
                        EchoSaveDiagnosticCodes
                            .RetentionDeleteFailed,
                        "Injected post-publication cleanup failure.",
                        3,
                        3,
                        1,
                        0,
                        SaveGenerationId.NewId());

                RecordingRetentionExecutor retention =
                    new RecordingRetentionExecutor(
                        maintenanceFailure);

                SaveManualTransactionCoordinator coordinator =
                    new SaveManualTransactionCoordinator(
                        env.Storage.Catalog,
                        env.CurrentReader,
                        env.CaptureCoordinator,
                        env.Registry,
                        env.UnknownStore,
                        env.CarryForward,
                        retention,
                        new SaveRetentionPolicy(2));

                SaveManualTransactionResult result =
                    coordinator.Save(
                        env.Request());

                Assert.That(result.Succeeded, Is.True);
                Assert.That(result.GenerationPublished, Is.True);
                Assert.That(result.HeadPublished, Is.True);
                Assert.That(
                    result.RetentionResult.Status,
                    Is.EqualTo(
                        SaveRetentionStatus.Failed));
                Assert.That(
                    result.DiagnosticCode,
                    Is.EqualTo(
                        EchoSaveDiagnosticCodes
                            .RetentionDeleteFailed));
            }
        }

        private sealed class RecordingRetentionExecutor :
            ISaveGenerationRetentionExecutor
        {
            private readonly SaveRetentionResult result;

            internal RecordingRetentionExecutor(
                SaveRetentionResult result)
            {
                this.result =
                    result;
            }

            internal int Calls { get; private set; }

            public SaveRetentionResult Apply(
                SaveSlotId slotId,
                SaveRetentionPolicy policy)
            {
                Calls++;
                return result;
            }
        }
    }
}
