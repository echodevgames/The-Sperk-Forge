using System;
using System.Reflection;
using System.Threading;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SavePublicAutosaveServiceTests
    {
        [Test]
        public void PublicServiceExposesAutosaveRequestSurface()
        {
            MethodInfo method =
                typeof(IEchoSaveService)
                    .GetMethod(
                        "RequestAutosave");

            Assert.That(
                method,
                Is.Not.Null);

            Assert.That(
                method.ReturnType,
                Is.EqualTo(
                    typeof(AutosaveSubmissionResult)));

            ParameterInfo[] parameters =
                method.GetParameters();

            Assert.That(
                parameters.Length,
                Is.EqualTo(
                    1));

            Assert.That(
                parameters[0].ParameterType,
                Is.EqualTo(
                    typeof(AutosaveRequest)));
        }

        [Test]
        public void AutosaveBeforeReadyRejectsWithoutPendingWork()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                AutosaveSubmissionResult result =
                    env.Service
                        .RequestAutosaveSynchronouslyForTesting(
                            env.Request());

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        AutosaveSubmissionStatus
                            .RejectedServiceNotReady));

                Assert.That(
                    result.Accepted,
                    Is.False);

                Assert.That(
                    result.Ticket,
                    Is.Null);

                Assert.That(
                    env.Service.PendingAutosaveCountForTesting,
                    Is.Zero);
            }
        }

        [Test]
        public void InvalidAutosaveRequestRejectsWithoutPendingWork()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize().Succeeded,
                    Is.True);

                AutosaveSubmissionResult result =
                    env.Service
                        .RequestAutosaveSynchronouslyForTesting(
                            env.Request(
                                projectId:
                                    new string(
                                        'x',
                                        SaveManualTransactionCoordinator
                                            .MaximumMetadataTextLength +
                                        1)));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        AutosaveSubmissionStatus
                            .RejectedInvalidRequest));

                Assert.That(
                    env.Executor.Calls,
                    Is.Zero);

                Assert.That(
                    env.Service.PendingAutosaveCountForTesting,
                    Is.Zero);
            }
        }

        [Test]
        public void AutosaveWithoutActiveSlotRejectsWithoutTransaction()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize(
                        selectActiveSlot:
                            false)
                        .Succeeded,
                    Is.True);

                AutosaveSubmissionResult result =
                    env.Service
                        .RequestAutosaveSynchronouslyForTesting(
                            env.Request());

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        AutosaveSubmissionStatus
                            .RejectedNoActiveSlot));

                Assert.That(
                    env.Executor.Calls,
                    Is.Zero);

                Assert.That(
                    env.Service.PendingAutosaveCountForTesting,
                    Is.Zero);
            }
        }

        [Test]
        public void PreCanceledAutosaveRejectsWithoutTransaction()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            using (CancellationTokenSource source =
                new CancellationTokenSource())
            {
                Assert.That(
                    env.Initialize().Succeeded,
                    Is.True);

                source.Cancel();

                AutosaveSubmissionResult result =
                    env.Service
                        .RequestAutosaveSynchronouslyForTesting(
                            env.Request(
                                cancellationToken:
                                    source.Token));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        AutosaveSubmissionStatus
                            .RejectedCanceled));

                Assert.That(
                    env.Executor.Calls,
                    Is.Zero);

                Assert.That(
                    env.Service.PendingAutosaveCountForTesting,
                    Is.Zero);
            }
        }

        [Test]
        public void IdleAutosaveExecutesExistingSaveTransactionImmediately()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize().Succeeded,
                    Is.True);

                AutosaveSubmissionResult result =
                    env.Service
                        .RequestAutosaveSynchronouslyForTesting(
                            env.Request(
                                buildId:
                                    "autosave-immediate"));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        AutosaveSubmissionStatus.Executed));

                Assert.That(
                    result.Accepted,
                    Is.True);

                Assert.That(
                    result.HasSaveResult,
                    Is.True);

                Assert.That(
                    result.SaveResult.Succeeded,
                    Is.True);

                Assert.That(
                    result.Ticket,
                    Is.Not.Null);

                Assert.That(
                    result.Ticket.State,
                    Is.EqualTo(
                        AutosaveTicketState.Succeeded));

                Assert.That(
                    result.Ticket.HasSaveResult,
                    Is.True);

                Assert.That(
                    env.Executor.Calls,
                    Is.EqualTo(
                        1));
            }
        }

        [Test]
        public void OccupiedRootRetainsExactlyOnePendingAutosave()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize().Succeeded,
                    Is.True);

                Assert.That(
                    env.Service
                        .SaveOperationAdmissionForTesting
                        .TryAcquire(
                            out SaveOperationAdmissionLease lease),
                    Is.EqualTo(
                        SaveOperationAdmissionStatus.Admitted));

                try
                {
                    AutosaveSubmissionResult result =
                        env.Service
                            .RequestAutosaveSynchronouslyForTesting(
                                env.Request());

                    Assert.That(
                        result.Status,
                        Is.EqualTo(
                            AutosaveSubmissionStatus.Pending));

                    Assert.That(
                        result.Ticket.State,
                        Is.EqualTo(
                            AutosaveTicketState.Pending));

                    Assert.That(
                        env.Service.PendingAutosaveCountForTesting,
                        Is.EqualTo(
                            1));

                    Assert.That(
                        env.Executor.Calls,
                        Is.Zero);
                }
                finally
                {
                    lease.Dispose();
                }

                Assert.That(
                    env.Executor.Calls,
                    Is.EqualTo(
                        1));

                Assert.That(
                    env.Service.PendingAutosaveCountForTesting,
                    Is.Zero);
            }
        }

        [Test]
        public void NewPendingAutosaveSupersedesOlderPendingTicket()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize().Succeeded,
                    Is.True);

                Assert.That(
                    env.Service
                        .SaveOperationAdmissionForTesting
                        .TryAcquire(
                            out SaveOperationAdmissionLease lease),
                    Is.EqualTo(
                        SaveOperationAdmissionStatus.Admitted));

                AutosaveSubmissionResult first;
                AutosaveSubmissionResult second;

                try
                {
                    first =
                        env.Service
                            .RequestAutosaveSynchronouslyForTesting(
                                env.Request(
                                    buildId:
                                        "older"));

                    second =
                        env.Service
                            .RequestAutosaveSynchronouslyForTesting(
                                env.Request(
                                    buildId:
                                        "latest"));

                    Assert.That(
                        first.Status,
                        Is.EqualTo(
                            AutosaveSubmissionStatus.Pending));

                    Assert.That(
                        second.Status,
                        Is.EqualTo(
                            AutosaveSubmissionStatus.Coalesced));

                    Assert.That(
                        second.SupersededTicket,
                        Is.SameAs(
                            first.Ticket));

                    Assert.That(
                        first.Ticket.State,
                        Is.EqualTo(
                            AutosaveTicketState.Superseded));

                    Assert.That(
                        second.Ticket.State,
                        Is.EqualTo(
                            AutosaveTicketState.Pending));

                    Assert.That(
                        env.Service.PendingAutosaveCountForTesting,
                        Is.EqualTo(
                            1));
                }
                finally
                {
                    lease.Dispose();
                }

                Assert.That(
                    second.Ticket.State,
                    Is.EqualTo(
                        AutosaveTicketState.Succeeded));
            }
        }

        [Test]
        public void RapidAutosaveSpamNeverGrowsPastOnePendingRequest()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize().Succeeded,
                    Is.True);

                Assert.That(
                    env.Service
                        .SaveOperationAdmissionForTesting
                        .TryAcquire(
                            out SaveOperationAdmissionLease lease),
                    Is.EqualTo(
                        SaveOperationAdmissionStatus.Admitted));

                AutosaveTicket latest =
                    null;

                try
                {
                    for (int i = 0;
                         i < 32;
                         i++)
                    {
                        AutosaveSubmissionResult result =
                            env.Service
                                .RequestAutosaveSynchronouslyForTesting(
                                    env.Request(
                                        buildId:
                                            "spam-" +
                                            i));

                        Assert.That(
                            result.Accepted,
                            Is.True);

                        Assert.That(
                            env.Service
                                .PendingAutosaveCountForTesting,
                            Is.EqualTo(
                                1));

                        if (latest != null)
                        {
                            Assert.That(
                                latest.State,
                                Is.EqualTo(
                                    AutosaveTicketState
                                        .Superseded));
                        }

                        latest =
                            result.Ticket;
                    }
                }
                finally
                {
                    lease.Dispose();
                }

                Assert.That(
                    env.Executor.Calls,
                    Is.EqualTo(
                        1));

                Assert.That(
                    latest,
                    Is.Not.Null);

                Assert.That(
                    latest.State,
                    Is.EqualTo(
                        AutosaveTicketState.Succeeded));
            }
        }

        [Test]
        public void LatestPendingMetadataIsWhatEventuallyExecutes()
        {
            string executedBuildId =
                string.Empty;

            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize(
                        handler:
                            (request, control) =>
                            {
                                executedBuildId =
                                    request.BuildId;

                                return PublicManualSaveResultFactory
                                    .Succeeded(
                                        env.ActiveSlotId);
                            })
                        .Succeeded,
                    Is.True);

                Assert.That(
                    env.Service
                        .SaveOperationAdmissionForTesting
                        .TryAcquire(
                            out SaveOperationAdmissionLease lease),
                    Is.EqualTo(
                        SaveOperationAdmissionStatus.Admitted));

                try
                {
                    env.Service
                        .RequestAutosaveSynchronouslyForTesting(
                            env.Request(
                                buildId:
                                    "old-build"));

                    env.Service
                        .RequestAutosaveSynchronouslyForTesting(
                            env.Request(
                                buildId:
                                    "latest-build"));

                    Assert.That(
                        executedBuildId,
                        Is.Empty);
                }
                finally
                {
                    lease.Dispose();
                }

                Assert.That(
                    executedBuildId,
                    Is.EqualTo(
                        "latest-build"));

                Assert.That(
                    env.Executor.Calls,
                    Is.EqualTo(
                        1));
            }
        }

        [Test]
        public void ManualSaveBusyContractRemainsUnchanged()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize().Succeeded,
                    Is.True);

                Assert.That(
                    env.Service
                        .SaveOperationAdmissionForTesting
                        .TryAcquire(
                            out SaveOperationAdmissionLease lease),
                    Is.EqualTo(
                        SaveOperationAdmissionStatus.Admitted));

                try
                {
                    AutosaveSubmissionResult autosave =
                        env.Service
                            .RequestAutosaveSynchronouslyForTesting(
                                env.Request());

                    SaveOperationResult manual =
                        env.Service
                            .SaveSynchronouslyForTesting(
                                new SaveRequest(
                                    "com.example.manual",
                                    "1.0.0",
                                    "manual-busy"));

                    Assert.That(
                        autosave.Status,
                        Is.EqualTo(
                            AutosaveSubmissionStatus.Pending));

                    Assert.That(
                        manual.Status,
                        Is.EqualTo(
                            SaveOperationStatus.Busy));

                    Assert.That(
                        env.Executor.Calls,
                        Is.Zero);
                }
                finally
                {
                    lease.Dispose();
                }

                Assert.That(
                    env.Executor.Calls,
                    Is.EqualTo(
                        1));
            }
        }

        [Test]
        public void PendingAutosaveExecutesAtMostOnceAfterAdmissionRelease()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize().Succeeded,
                    Is.True);

                Assert.That(
                    env.Service
                        .SaveOperationAdmissionForTesting
                        .TryAcquire(
                            out SaveOperationAdmissionLease lease),
                    Is.EqualTo(
                        SaveOperationAdmissionStatus.Admitted));

                AutosaveSubmissionResult pending =
                    env.Service
                        .RequestAutosaveSynchronouslyForTesting(
                            env.Request());

                Assert.That(
                    env.Executor.Calls,
                    Is.Zero);

                lease.Dispose();

                Assert.That(
                    env.Executor.Calls,
                    Is.EqualTo(
                        1));

                Assert.That(
                    pending.Ticket.State,
                    Is.EqualTo(
                        AutosaveTicketState.Succeeded));

                Assert.That(
                    env.Service.PendingAutosaveCountForTesting,
                    Is.Zero);

                env.Service
                    .SaveOperationAdmissionForTesting
                    .TryAcquire(
                        out SaveOperationAdmissionLease secondLease);

                secondLease.Dispose();

                Assert.That(
                    env.Executor.Calls,
                    Is.EqualTo(
                        1));
            }
        }

        [Test]
        public void PendingAutosavePreflightFailureClearsWithoutExecution()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize().Succeeded,
                    Is.True);

                Assert.That(
                    env.Service
                        .SaveOperationAdmissionForTesting
                        .TryAcquire(
                            out SaveOperationAdmissionLease lease),
                    Is.EqualTo(
                        SaveOperationAdmissionStatus.Admitted));

                AutosaveSubmissionResult pending =
                    env.Service
                        .RequestAutosaveSynchronouslyForTesting(
                            env.Request());

                Assert.That(
                    env.Service
                        .SlotCatalogForTesting
                        .ClearActiveSlot()
                        .Succeeded,
                    Is.True);

                lease.Dispose();

                Assert.That(
                    env.Executor.Calls,
                    Is.Zero);

                Assert.That(
                    env.Service.PendingAutosaveCountForTesting,
                    Is.Zero);

                Assert.That(
                    pending.Ticket.State,
                    Is.EqualTo(
                        AutosaveTicketState.Discarded));

                Assert.That(
                    pending.Ticket.DiagnosticCode,
                    Is.EqualTo(
                        EchoSaveDiagnosticCodes
                            .AutosaveNoActiveSlot));
            }
        }

        [Test]
        public void ShutdownDiscardsPendingAutosaveAndNeverExecutesIt()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize().Succeeded,
                    Is.True);

                Assert.That(
                    env.Service
                        .SaveOperationAdmissionForTesting
                        .TryAcquire(
                            out SaveOperationAdmissionLease lease),
                    Is.EqualTo(
                        SaveOperationAdmissionStatus.Admitted));

                AutosaveSubmissionResult pending =
                    env.Service
                        .RequestAutosaveSynchronouslyForTesting(
                            env.Request());

                EchoSaveLifecycleResult shutdown =
                    env.Service.ShutdownCore();

                Assert.That(
                    shutdown.Status,
                    Is.EqualTo(
                        EchoSaveLifecycleStatus.Rejected));

                Assert.That(
                    env.Service.State,
                    Is.EqualTo(
                        EchoSaveServiceState.ShuttingDown));

                Assert.That(
                    env.Service.PendingAutosaveCountForTesting,
                    Is.Zero);

                Assert.That(
                    pending.Ticket.State,
                    Is.EqualTo(
                        AutosaveTicketState.Discarded));

                lease.Dispose();

                Assert.That(
                    env.Executor.Calls,
                    Is.Zero);

                Assert.That(
                    env.Service.ShutdownCore().Succeeded,
                    Is.True);
            }
        }

        [Test]
        public void ShutdownStateRejectsNewAutosaveSubmission()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize().Succeeded,
                    Is.True);

                Assert.That(
                    env.Service.ShutdownCore().Succeeded,
                    Is.True);

                AutosaveSubmissionResult result =
                    env.Service
                        .RequestAutosaveSynchronouslyForTesting(
                            env.Request());

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        AutosaveSubmissionStatus
                            .RejectedAdmissionClosed));

                Assert.That(
                    result.Accepted,
                    Is.False);

                Assert.That(
                    env.Executor.Calls,
                    Is.Zero);
            }
        }

        [Test]
        public void AutosavePreservesTooLateCancellationTruth()
        {
            using (CancellationTokenSource source =
                new CancellationTokenSource())
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize(
                        handler:
                            (request, control) =>
                            {
                                Assert.That(
                                    control.TryBeginPublication(),
                                    Is.True);

                                source.Cancel();

                                return PublicManualSaveResultFactory
                                    .Succeeded(
                                        env.ActiveSlotId);
                            })
                        .Succeeded,
                    Is.True);

                AutosaveSubmissionResult result =
                    env.Service
                        .RequestAutosaveSynchronouslyForTesting(
                            env.Request(
                                cancellationToken:
                                    source.Token));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        AutosaveSubmissionStatus.Executed));

                Assert.That(
                    result.SaveResult.Succeeded,
                    Is.True);

                Assert.That(
                    result.SaveResult.CancellationWasTooLate,
                    Is.True);

                Assert.That(
                    result.Ticket.State,
                    Is.EqualTo(
                        AutosaveTicketState.Succeeded));
            }
        }

        [Test]
        public void AutosavePreservesPartialDurableCatalogTruth()
        {
            using (AutosaveServiceTestEnvironment env =
                new AutosaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize(
                        handler:
                            (request, control) =>
                                PublicManualSaveResultFactory
                                    .Failure(
                                        SaveManualTransactionStatus
                                            .PublishedCatalogReconciliationFailed,
                                        "ESV-SAVE-011",
                                        generationPublished:
                                            true,
                                        headPublished:
                                            true,
                                        catalogReconciled:
                                            false))
                        .Succeeded,
                    Is.True);

                AutosaveSubmissionResult result =
                    env.Service
                        .RequestAutosaveSynchronouslyForTesting(
                            env.Request());

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        AutosaveSubmissionStatus.Executed));

                Assert.That(
                    result.SaveResult.Status,
                    Is.EqualTo(
                        SaveOperationStatus
                            .PublishedCatalogReconciliationFailed));

                Assert.That(
                    result.SaveResult.GenerationPublished,
                    Is.True);

                Assert.That(
                    result.SaveResult.HeadPublished,
                    Is.True);

                Assert.That(
                    result.SaveResult.CatalogReconciled,
                    Is.False);

                Assert.That(
                    result.Ticket.State,
                    Is.EqualTo(
                        AutosaveTicketState.Failed));
            }
        }
    }
}
