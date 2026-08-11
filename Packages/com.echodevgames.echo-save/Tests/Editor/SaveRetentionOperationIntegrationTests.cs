
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveRetentionOperationIntegrationTests
    {
        [Test]
        public void ManualSaveMapsRetentionMaintenanceTruth()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                SaveRetentionResult retention =
                    FailedRetention();

                EchoSaveLifecycleResult initialized =
                    env.Initialize(
                        handler:
                            (request, control) =>
                                TransactionWithRetention(
                                    env.ActiveSlotId,
                                    retention));

                Assert.That(initialized.Succeeded, Is.True);

                SaveOperationResult result =
                    env.Service.SaveSynchronouslyForTesting(
                        new SaveRequest(
                            "com.example.retention",
                            "1.0.0",
                            "manual"));

                Assert.That(result.Succeeded, Is.True);
                Assert.That(
                    result.RetentionResult.Status,
                    Is.EqualTo(
                        SaveRetentionStatus.Failed));
                Assert.That(
                    result.RetentionResult.MaintenanceFailed,
                    Is.True);
            }
        }

        [Test]
        public void AutosaveMapsSameRetentionMaintenanceTruth()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                SaveRetentionResult retention =
                    FailedRetention();

                EchoSaveLifecycleResult initialized =
                    env.Initialize(
                        handler:
                            (request, control) =>
                                TransactionWithRetention(
                                    env.ActiveSlotId,
                                    retention));

                Assert.That(initialized.Succeeded, Is.True);

                AutosaveSubmissionResult submission =
                    env.Service
                        .RequestAutosaveSynchronouslyForTesting(
                            env.Request(
                                buildId:
                                    "autosave-retention"));

                Assert.That(
                    submission.Status,
                    Is.EqualTo(
                        AutosaveSubmissionStatus.Executed));
                Assert.That(submission.HasSaveResult, Is.True);
                Assert.That(submission.SaveResult.Succeeded, Is.True);
                Assert.That(
                    submission.SaveResult.RetentionResult.Status,
                    Is.EqualTo(
                        SaveRetentionStatus.Failed));
            }
        }

        private static SaveRetentionResult FailedRetention() =>
            new SaveRetentionResult(
                SaveRetentionStatus.Failed,
                EchoSaveDiagnosticCodes.RetentionDeleteFailed,
                "Injected post-publication retention maintenance failure.",
                3,
                3,
                1,
                0,
                SaveGenerationId.NewId());

        private static SaveManualTransactionResult
            TransactionWithRetention(
                SaveSlotId slotId,
                SaveRetentionResult retention) =>
            new SaveManualTransactionResult(
                SaveManualTransactionStatus.Succeeded,
                retention.DiagnosticCode,
                "The save committed; retention maintenance failed.",
                slotId,
                SaveGenerationId.NewId(),
                SaveGenerationId.NewId(),
                default,
                default,
                1,
                0,
                10L,
                true,
                true,
                true,
                null,
                retention);
    }
}
