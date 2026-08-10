using System;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SavePublicManualSaveServiceTests
    {
        [Test]
        public void PublicServiceExposesBoundedManualSaveAndAutosaveOperations()
        {
            MethodInfo saveMethod =
                typeof(IEchoSaveService).GetMethod(
                    "SaveAsync",
                    new[]
                    {
                        typeof(SaveRequest)
                    });

            Assert.That(
                saveMethod,
                Is.Not.Null);

            Assert.That(
                saveMethod.ReturnType,
                Is.EqualTo(
                    typeof(Awaitable<
                        SaveOperationResult>)));

            MethodInfo autosaveMethod =
                typeof(IEchoSaveService).GetMethod(
                    "RequestAutosave",
                    new[]
                    {
                        typeof(AutosaveRequest)
                    });

            Assert.That(
                autosaveMethod,
                Is.Not.Null);

            Assert.That(
                autosaveMethod.ReturnType,
                Is.EqualTo(
                    typeof(AutosaveSubmissionResult)));
        }

        [Test]
        public void PublicSaveRequestOwnsNoPathRenameOrSlotOverride()
        {
            PropertyInfo[] properties =
                typeof(SaveRequest).GetProperties(
                    BindingFlags.Instance |
                    BindingFlags.Public);

            Assert.That(
                Array.Exists(
                    properties,
                    property =>
                        property.Name ==
                        "ProjectId"),
                Is.True);

            Assert.That(
                Array.Exists(
                    properties,
                    property =>
                        property.Name ==
                        "CancellationToken"),
                Is.True);

            Assert.That(
                Array.Exists(
                    properties,
                    property =>
                        property.Name ==
                        "Path" ||
                        property.Name ==
                        "DisplayName" ||
                        property.Name ==
                        "SlotId"),
                Is.False);
        }

        [Test]
        public void SaveBeforeReadyRejectsWithoutTransactionExecution()
        {
            using (PublicManualSaveServiceTestEnvironment env =
                new PublicManualSaveServiceTestEnvironment())
            {
                SaveOperationResult result =
                    env.Service.SaveSynchronouslyForTesting(
                        env.Request());

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveOperationStatus.ServiceNotReady));

                Assert.That(
                    result.GenerationPublished,
                    Is.False);
            }
        }

        [Test]
        public void SuccessfulPublicSaveMapsM403DurableTruth()
        {
            using (PublicManualSaveServiceTestEnvironment env =
                new PublicManualSaveServiceTestEnvironment())
            {
                EchoSaveLifecycleResult initialization =
                    env.Initialize();

                Assert.That(
                    initialization.Succeeded,
                    Is.True);

                SaveOperationResult result =
                    env.Service.SaveSynchronouslyForTesting(
                        env.Request());

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveOperationStatus.Succeeded));

                Assert.That(
                    result.Succeeded,
                    Is.True);

                Assert.That(
                    result.CancellationDisposition,
                    Is.EqualTo(
                        SaveCancellationDisposition.None));

                Assert.That(
                    result.FreshParticipantCount,
                    Is.EqualTo(2));

                Assert.That(
                    result.PreservedUnknownCount,
                    Is.EqualTo(1));

                Assert.That(
                    result.TotalPayloadBytes,
                    Is.EqualTo(313L));

                Assert.That(
                    result.GenerationPublished,
                    Is.True);

                Assert.That(
                    result.HeadPublished,
                    Is.True);

                Assert.That(
                    result.CatalogReconciled,
                    Is.True);

                Assert.That(
                    env.Executor.Calls,
                    Is.EqualTo(1));
            }
        }

        [Test]
        public void OverlappingManualSaveReturnsBusyAndNeverQueues()
        {
            using (PublicManualSaveServiceTestEnvironment env =
                new PublicManualSaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize().Succeeded,
                    Is.True);

                SaveOperationAdmissionStatus admission =
                    env.Service
                        .SaveOperationAdmissionForTesting
                        .TryAcquire(
                            out SaveOperationAdmissionLease lease);

                Assert.That(
                    admission,
                    Is.EqualTo(
                        SaveOperationAdmissionStatus.Admitted));

                SaveOperationResult busy =
                    env.Service.SaveSynchronouslyForTesting(
                        env.Request());

                Assert.That(
                    busy.Status,
                    Is.EqualTo(
                        SaveOperationStatus.Busy));

                Assert.That(
                    env.Executor.Calls,
                    Is.Zero);

                lease.Dispose();

                Assert.That(
                    env.Executor.Calls,
                    Is.Zero);

                SaveOperationResult retry =
                    env.Service.SaveSynchronouslyForTesting(
                        env.Request());

                Assert.That(
                    retry.Status,
                    Is.EqualTo(
                        SaveOperationStatus.Succeeded));

                Assert.That(
                    env.Executor.Calls,
                    Is.EqualTo(1));
            }
        }

        [Test]
        public void PreCanceledRequestNeverExecutesTransaction()
        {
            using (PublicManualSaveServiceTestEnvironment env =
                new PublicManualSaveServiceTestEnvironment())
            using (CancellationTokenSource cancellation =
                new CancellationTokenSource())
            {
                Assert.That(
                    env.Initialize().Succeeded,
                    Is.True);

                cancellation.Cancel();

                SaveOperationResult result =
                    env.Service.SaveSynchronouslyForTesting(
                        env.Request(
                            cancellationToken:
                                cancellation.Token));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveOperationStatus.Canceled));

                Assert.That(
                    result.CancellationDisposition,
                    Is.EqualTo(
                        SaveCancellationDisposition.Canceled));

                Assert.That(
                    env.Executor.Calls,
                    Is.Zero);
            }
        }

        [Test]
        public void CancellationAfterPublicationBoundaryReportsTooLate()
        {
            using (CancellationTokenSource cancellation =
                new CancellationTokenSource())
            using (PublicManualSaveServiceTestEnvironment env =
                new PublicManualSaveServiceTestEnvironment())
            {
                EchoSaveLifecycleResult initialization =
                    env.Initialize(
                        (request, control) =>
                        {
                            Assert.That(
                                control.TryBeginPublication(),
                                Is.True);

                            cancellation.Cancel();

                            return PublicManualSaveResultFactory
                                .Succeeded();
                        });

                Assert.That(
                    initialization.Succeeded,
                    Is.True);

                SaveOperationResult result =
                    env.Service.SaveSynchronouslyForTesting(
                        env.Request(
                            cancellationToken:
                                cancellation.Token));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveOperationStatus.Succeeded));

                Assert.That(
                    result.CancellationDisposition,
                    Is.EqualTo(
                        SaveCancellationDisposition.TooLate));

                Assert.That(
                    result.CancellationWasTooLate,
                    Is.True);

                Assert.That(
                    result.GenerationPublished,
                    Is.True);

                Assert.That(
                    result.HeadPublished,
                    Is.True);

                Assert.That(
                    result.CatalogReconciled,
                    Is.True);
            }
        }

        [Test]
        public void PublishedCatalogFailureMapsWithoutRollbackFiction()
        {
            using (PublicManualSaveServiceTestEnvironment env =
                new PublicManualSaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize(
                        (request, control) =>
                            PublicManualSaveResultFactory
                                .Failure(
                                    SaveManualTransactionStatus
                                        .PublishedCatalogReconciliationFailed,
                                    "ESV-SAVE-011",
                                    generationPublished: true,
                                    headPublished: true,
                                    catalogReconciled: false))
                        .Succeeded,
                    Is.True);

                SaveOperationResult result =
                    env.Service.SaveSynchronouslyForTesting(
                        env.Request());

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveOperationStatus
                            .PublishedCatalogReconciliationFailed));

                Assert.That(
                    result.GenerationPublished,
                    Is.True);

                Assert.That(
                    result.HeadPublished,
                    Is.True);

                Assert.That(
                    result.CatalogReconciled,
                    Is.False);

                Assert.That(
                    result.Succeeded,
                    Is.False);
            }
        }

        [Test]
        public void FailedTransactionReleasesAdmission()
        {
            using (PublicManualSaveServiceTestEnvironment env =
                new PublicManualSaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize(
                        (request, control) =>
                            PublicManualSaveResultFactory
                                .Failure(
                                    SaveManualTransactionStatus
                                        .CaptureFailed))
                        .Succeeded,
                    Is.True);

                SaveOperationResult result =
                    env.Service.SaveSynchronouslyForTesting(
                        env.Request());

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveOperationStatus.TransactionFailed));

                SaveOperationAdmissionStatus admission =
                    env.Service
                        .SaveOperationAdmissionForTesting
                        .TryAcquire(
                            out SaveOperationAdmissionLease lease);

                Assert.That(
                    admission,
                    Is.EqualTo(
                        SaveOperationAdmissionStatus.Admitted));

                lease.Dispose();
            }
        }

        [Test]
        public void ShutdownClosesAdmissionBeforeBackendShutdown()
        {
            using (PublicManualSaveServiceTestEnvironment env =
                new PublicManualSaveServiceTestEnvironment())
            {
                Assert.That(
                    env.Initialize().Succeeded,
                    Is.True);

                SaveOperationAdmissionStatus admission =
                    env.Service
                        .SaveOperationAdmissionForTesting
                        .TryAcquire(
                            out SaveOperationAdmissionLease lease);

                Assert.That(
                    admission,
                    Is.EqualTo(
                        SaveOperationAdmissionStatus.Admitted));

                EchoSaveLifecycleResult firstShutdown =
                    env.Service.ShutdownCore();

                Assert.That(
                    firstShutdown.Status,
                    Is.EqualTo(
                        EchoSaveLifecycleStatus.Rejected));

                Assert.That(
                    firstShutdown.State,
                    Is.EqualTo(
                        EchoSaveServiceState.ShuttingDown));

                Assert.That(
                    env.Backend.ShutdownCalls,
                    Is.Zero);

                SaveOperationResult rejected =
                    env.Service.SaveSynchronouslyForTesting(
                        env.Request());

                Assert.That(
                    rejected.Status,
                    Is.EqualTo(
                        SaveOperationStatus.AdmissionClosed));

                lease.Dispose();

                EchoSaveLifecycleResult settledShutdown =
                    env.Service.ShutdownCore();

                Assert.That(
                    settledShutdown.Succeeded,
                    Is.True);

                Assert.That(
                    settledShutdown.State,
                    Is.EqualTo(
                        EchoSaveServiceState.Shutdown));

                Assert.That(
                    env.Backend.ShutdownCalls,
                    Is.EqualTo(1));
            }
        }

        [Test]
        public void InitializedServiceBuildsOneRootLocalManualSaveRuntime()
        {
            using (PublicManualSaveServiceTestEnvironment first =
                new PublicManualSaveServiceTestEnvironment())
            using (PublicManualSaveServiceTestEnvironment second =
                new PublicManualSaveServiceTestEnvironment())
            {
                Assert.That(
                    first.Initialize().Succeeded,
                    Is.True);

                Assert.That(
                    second.Initialize().Succeeded,
                    Is.True);

                Assert.That(
                    first.Service.ParticipantRegistryForTesting,
                    Is.Not.Null);

                Assert.That(
                    first.Service.SlotCatalogForTesting,
                    Is.Not.Null);

                Assert.That(
                    second.Service.ParticipantRegistryForTesting,
                    Is.Not.SameAs(
                        first.Service.ParticipantRegistryForTesting));

                SaveOperationAdmissionStatus firstAdmission =
                    first.Service
                        .SaveOperationAdmissionForTesting
                        .TryAcquire(
                            out SaveOperationAdmissionLease firstLease);

                SaveOperationAdmissionStatus secondAdmission =
                    second.Service
                        .SaveOperationAdmissionForTesting
                        .TryAcquire(
                            out SaveOperationAdmissionLease secondLease);

                Assert.That(
                    firstAdmission,
                    Is.EqualTo(
                        SaveOperationAdmissionStatus.Admitted));

                Assert.That(
                    secondAdmission,
                    Is.EqualTo(
                        SaveOperationAdmissionStatus.Admitted));

                firstLease.Dispose();
                secondLease.Dispose();
            }
        }
    }

    public sealed class SaveOperationAdmissionCoordinatorTests
    {
        [Test]
        public void ClosedAdmissionRejectsWithoutLease()
        {
            SaveOperationAdmissionCoordinator admission =
                new SaveOperationAdmissionCoordinator();

            SaveOperationAdmissionStatus status =
                admission.TryAcquire(
                    out SaveOperationAdmissionLease lease);

            Assert.That(
                status,
                Is.EqualTo(
                    SaveOperationAdmissionStatus.Closed));

            Assert.That(
                lease,
                Is.Null);
        }

        [Test]
        public void BusyAdmissionDoesNotQueue()
        {
            SaveOperationAdmissionCoordinator admission =
                new SaveOperationAdmissionCoordinator();

            admission.Open();

            Assert.That(
                admission.TryAcquire(
                    out SaveOperationAdmissionLease first),
                Is.EqualTo(
                    SaveOperationAdmissionStatus.Admitted));

            Assert.That(
                admission.TryAcquire(
                    out SaveOperationAdmissionLease busy),
                Is.EqualTo(
                    SaveOperationAdmissionStatus.Busy));

            Assert.That(
                busy,
                Is.Null);

            first.Dispose();

            Assert.That(
                admission.TryAcquire(
                    out SaveOperationAdmissionLease retry),
                Is.EqualTo(
                    SaveOperationAdmissionStatus.Admitted));

            retry.Dispose();
        }

        [Test]
        public void ClosePreservesActiveLeaseButRejectsNewAdmission()
        {
            SaveOperationAdmissionCoordinator admission =
                new SaveOperationAdmissionCoordinator();

            admission.Open();

            Assert.That(
                admission.TryAcquire(
                    out SaveOperationAdmissionLease active),
                Is.EqualTo(
                    SaveOperationAdmissionStatus.Admitted));

            admission.Close();

            Assert.That(
                admission.IsOccupied,
                Is.True);

            Assert.That(
                admission.TryAcquire(
                    out SaveOperationAdmissionLease rejected),
                Is.EqualTo(
                    SaveOperationAdmissionStatus.Closed));

            Assert.That(
                rejected,
                Is.Null);

            active.Dispose();

            Assert.That(
                admission.IsOccupied,
                Is.False);
        }
    }

    public sealed class SaveManualTransactionCancellationTests
    {
        [Test]
        public void PreCanceledControlStopsBeforeParticipantCapture()
        {
            using (ManualSaveTransactionTestEnvironment env =
                new ManualSaveTransactionTestEnvironment())
            using (CancellationTokenSource cancellation =
                new CancellationTokenSource())
            {
                ManualSaveTransactionTestEnvironment.CreatedSlot source =
                    env.CreateEmptySlot();

                ManualSaveTestParticipant participant =
                    env.Participant();

                env.Register(
                    participant);

                cancellation.Cancel();

                SaveManualTransactionResult result =
                    env.Coordinator.Save(
                        env.Request(),
                        new SaveManualTransactionControl(
                            cancellation.Token));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveManualTransactionStatus.Canceled));

                Assert.That(
                    participant.CaptureCalls,
                    Is.Zero);

                Assert.That(
                    env.ReadHead(
                            source.SlotId)
                        .currentGenerationId,
                    Is.EqualTo(
                        source.GenerationId.Value));
            }
        }

        [Test]
        public void CancellationDuringCaptureStopsBeforePublication()
        {
            using (ManualSaveTransactionTestEnvironment env =
                new ManualSaveTransactionTestEnvironment())
            using (CancellationTokenSource cancellation =
                new CancellationTokenSource())
            {
                ManualSaveTransactionTestEnvironment.CreatedSlot source =
                    env.CreateEmptySlot();

                ManualSaveTestParticipant participant =
                    env.Participant(
                        onCapture:
                            cancellation.Cancel);

                env.Register(
                    participant);

                SaveManualTransactionControl control =
                    new SaveManualTransactionControl(
                        cancellation.Token);

                SaveManualTransactionResult result =
                    env.Coordinator.Save(
                        env.Request(),
                        control);

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveManualTransactionStatus.Canceled));

                Assert.That(
                    participant.CaptureCalls,
                    Is.EqualTo(1));

                Assert.That(
                    control.PublicationStarted,
                    Is.False);

                Assert.That(
                    result.GenerationPublished,
                    Is.False);

                Assert.That(
                    result.HeadPublished,
                    Is.False);

                Assert.That(
                    env.ReadHead(
                            source.SlotId)
                        .currentGenerationId,
                    Is.EqualTo(
                        source.GenerationId.Value));
            }
        }

        [Test]
        public void SuccessfulTransactionMarksDurablePublicationBoundary()
        {
            using (ManualSaveTransactionTestEnvironment env =
                new ManualSaveTransactionTestEnvironment())
            {
                env.CreateEmptySlot();

                env.Register(
                    env.Participant());

                SaveManualTransactionControl control =
                    new SaveManualTransactionControl(
                        CancellationToken.None);

                SaveManualTransactionResult result =
                    env.Coordinator.Save(
                        env.Request(),
                        control);

                Assert.That(
                    result.Succeeded,
                    Is.True);

                Assert.That(
                    control.PublicationStarted,
                    Is.True);
            }
        }
    }
}
