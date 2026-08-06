using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using EchoDevGames.EchoLaunch.Editor.Simulation;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Simulation
{
    public sealed class LaunchSimulationServiceTests
    {
        [Test]
        public async Task ImmediateSuccessCompletes()
        {
            LaunchSimulationReport report =
                await Run(
                    LaunchSimulationPreset.ImmediateSuccess);

            Assert.That(
                report.Status,
                Is.EqualTo(
                    LaunchSimulationStatus.Completed));

            Assert.That(report.StepCount, Is.EqualTo(1));

            Assert.That(
                report.GetStep(0).Status,
                Is.EqualTo(StartupStepStatus.Succeeded));

            Assert.That(report.UnvisitedEntryCount, Is.Zero);
        }

        [Test]
        public async Task TimedProgressIsOrderedAndEndsAtOne()
        {
            LaunchSimulationReport report =
                await Run(
                    LaunchSimulationPreset.TimedProgressSuccess);

            Assert.That(
                report.ProgressSampleCount,
                Is.EqualTo(4));

            float previous = 0f;
            double previousTime = 0d;

            for (int index = 0;
                 index < report.ProgressSampleCount;
                 index++)
            {
                LaunchSimulationProgressSample sample =
                    report.GetProgressSample(index);

                Assert.That(
                    sample.Progress01,
                    Is.GreaterThanOrEqualTo(previous));

                Assert.That(
                    sample.LogicalSeconds,
                    Is.GreaterThanOrEqualTo(previousTime));

                previous = sample.Progress01;
                previousTime = sample.LogicalSeconds;
            }

            Assert.That(previous, Is.EqualTo(1f));
            Assert.That(previousTime, Is.EqualTo(1d));
        }

        [Test]
        public async Task WarningContinuesToSecondStep()
        {
            LaunchSimulationReport report =
                await Run(
                    LaunchSimulationPreset.WarningContinues);

            Assert.That(report.StepCount, Is.EqualTo(2));

            Assert.That(
                report.GetStep(0).Status,
                Is.EqualTo(StartupStepStatus.Warning));

            Assert.That(
                report.GetStep(1).Status,
                Is.EqualTo(StartupStepStatus.Succeeded));
        }

        [Test]
        public async Task RecoverableFailureConvertsAndContinues()
        {
            LaunchSimulationReport report =
                await Run(
                    LaunchSimulationPreset
                        .RecoverableFailureContinues);

            Assert.That(report.StepCount, Is.EqualTo(2));

            Assert.That(
                report.GetStep(0).Status,
                Is.EqualTo(StartupStepStatus.Warning));

            Assert.That(
                report.GetStep(0).Code,
                Is.EqualTo(
                    LaunchSimulationDiagnosticCodes
                        .SimulatedRecoverableFailure));

            Assert.That(
                report.GetStep(1).Status,
                Is.EqualTo(StartupStepStatus.Succeeded));
        }

        [Test]
        public async Task BlockingFailureStopsTraversal()
        {
            LaunchSimulationReport report =
                await Run(
                    LaunchSimulationPreset
                        .BlockingFailureStops);

            Assert.That(report.StepCount, Is.EqualTo(1));

            Assert.That(
                report.UnvisitedEntryCount,
                Is.EqualTo(1));

            Assert.That(
                report.GetStep(0).Status,
                Is.EqualTo(
                    StartupStepStatus.BlockingFailure));
        }

        [Test]
        public async Task TimeoutUsesCanonicalDiagnosticAndStops()
        {
            LaunchSimulationReport report =
                await Run(
                    LaunchSimulationPreset.TimeoutStops);

            Assert.That(report.StepCount, Is.EqualTo(1));

            Assert.That(
                report.UnvisitedEntryCount,
                Is.EqualTo(1));

            Assert.That(
                report.GetStep(0).Code,
                Is.EqualTo("ELAUNCH-STEP-003"));
        }

        [Test]
        public async Task ExecutorExceptionUsesCanonicalConversion()
        {
            LaunchSimulationReport report =
                await Run(
                    LaunchSimulationPreset
                        .ExecutorExceptionStops);

            Assert.That(report.StepCount, Is.EqualTo(1));

            Assert.That(
                report.UnvisitedEntryCount,
                Is.EqualTo(1));

            Assert.That(
                report.GetStep(0).Code,
                Is.EqualTo("ELAUNCH-STEP-004"));
        }

        [Test]
        public async Task PreCancelledCallerSettlesAsCancelled()
        {
            LaunchSimulationService service =
                new LaunchSimulationService();

            LaunchSimulationRequest request =
                CreateRequest(
                    LaunchSimulationPreset.Cancellation);

            using (
                CancellationTokenSource source =
                    new CancellationTokenSource())
            {
                source.Cancel();

                LaunchSimulationReport report =
                    await service.RunAsync(
                        request,
                        source.Token);

                Assert.That(
                    report.Status,
                    Is.EqualTo(
                        LaunchSimulationStatus.Cancelled));

                Assert.That(report.WasCancelled, Is.True);

                Assert.That(
                    report.DiagnosticCode,
                    Is.EqualTo(
                        LaunchSimulationDiagnosticCodes
                            .Cancelled));
            }
        }

        [Test]
        public async Task ActiveRunRejectsReentryThenCancelsCleanly()
        {
            LaunchSimulationService service =
                new LaunchSimulationService();

            using (
                CancellationTokenSource source =
                    new CancellationTokenSource())
            {
                Awaitable<LaunchSimulationReport> active =
                    service.RunAsync(
                        CreateRequest(
                            LaunchSimulationPreset.Cancellation),
                        source.Token);

                Assert.That(service.IsRunning, Is.True);

                LaunchSimulationReport busy =
                    await service.RunAsync(
                        CreateRequest(
                            LaunchSimulationPreset
                                .ImmediateSuccess),
                        CancellationToken.None);

                Assert.That(
                    busy.Status,
                    Is.EqualTo(
                        LaunchSimulationStatus.Busy));

                Assert.That(
                    busy.DiagnosticCode,
                    Is.EqualTo(
                        LaunchSimulationDiagnosticCodes.Busy));

                source.Cancel();

                LaunchSimulationReport cancelled =
                    await active;

                Assert.That(
                    cancelled.Status,
                    Is.EqualTo(
                        LaunchSimulationStatus.Cancelled));

                Assert.That(
                    cancelled.StepCount,
                    Is.EqualTo(1));

                LaunchSimulationStepReport cancelledStep =
                    cancelled.GetStep(0);

                Assert.That(
                    cancelledStep.LogicalElapsedSeconds,
                    Is.Zero);

                Assert.That(
                    cancelledStep.Details,
                    Is.EqualTo(
                        "ExecutorCompletedWithoutException: False"));

                Assert.That(
                    cancelled.Text,
                    Does.Not.Contain("ElapsedSeconds:"));

                Assert.That(service.IsRunning, Is.False);
            }
        }

        [Test]
        public async Task IdenticalRunsProduceIdenticalText()
        {
            LaunchSimulationRequest request =
                CreateRequest(
                    LaunchSimulationPreset.WarningContinues);

            LaunchSimulationService service =
                new LaunchSimulationService();

            LaunchSimulationReport first =
                await service.RunAsync(
                    request,
                    CancellationToken.None);

            LaunchSimulationReport second =
                await service.RunAsync(
                    request,
                    CancellationToken.None);

            Assert.That(
                second.ReportFingerprint,
                Is.EqualTo(first.ReportFingerprint));

            Assert.That(
                second.Text,
                Is.EqualTo(first.Text));
        }

        [Test]
        public async Task InvalidRequestCreatesNoTransientObjects()
        {
            int before =
                LaunchSimulationTransientPlanBuilder.LiveObjectCount;

            LaunchSimulationService service =
                new LaunchSimulationService();

            LaunchSimulationRequest invalid =
                new LaunchSimulationRequest(
                    LaunchSimulationPreset
                        .TimedProgressSuccess,
                    0d,
                    0,
                    0d,
                    string.Empty);

            LaunchSimulationReport report =
                await service.RunAsync(
                    invalid,
                    CancellationToken.None);

            Assert.That(
                report.Status,
                Is.EqualTo(
                    LaunchSimulationStatus.InvalidRequest));

            Assert.That(
                report.DiagnosticCode,
                Is.EqualTo(
                    LaunchSimulationDiagnosticCodes
                        .InvalidRequest));

            Assert.That(
                LaunchSimulationTransientPlanBuilder.LiveObjectCount,
                Is.EqualTo(before));
        }

        private static async Task<LaunchSimulationReport> Run(
            LaunchSimulationPreset preset)
        {
            int before =
                LaunchSimulationTransientPlanBuilder.LiveObjectCount;

            LaunchSimulationService service =
                new LaunchSimulationService();

            LaunchSimulationReport report =
                await service.RunAsync(
                    CreateRequest(preset),
                    CancellationToken.None);

            Assert.That(
                LaunchSimulationTransientPlanBuilder.LiveObjectCount,
                Is.EqualTo(before));

            return report;
        }

        private static LaunchSimulationRequest CreateRequest(
            LaunchSimulationPreset preset)
        {
            return new LaunchSimulationRequest(
                preset,
                preset ==
                    LaunchSimulationPreset
                        .TimedProgressSuccess
                    ? 1d
                    : 0d,
                preset ==
                    LaunchSimulationPreset
                        .TimedProgressSuccess
                    ? 4
                    : 0,
                preset ==
                    LaunchSimulationPreset.TimeoutStops
                    ? 0.5d
                    : 0d,
                string.Empty);
        }
    }
}
