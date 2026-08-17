using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUINotificationPresenterTests
    {
        private sealed class RecordingPresenter :
            IUINotificationPresenter
        {
            public readonly List<UINotificationPresentationSnapshot>
                Snapshots =
                    new List<UINotificationPresentationSnapshot>();

            public Action<UINotificationPresentationSnapshot>
                Callback { get; set; }

            public void ApplyChannel(
                UINotificationPresentationSnapshot snapshot)
            {
                Snapshots.Add(snapshot);
                Callback?.Invoke(snapshot);
            }
        }

        private sealed class FailingPresenter :
            IUINotificationPresenter
        {
            public void ApplyChannel(
                UINotificationPresentationSnapshot snapshot)
            {
                throw new InvalidOperationException(
                    "presenter-observer");
            }
        }

        private static readonly FieldInfo ActiveRootField =
            typeof(EchoUIRoot).GetField(
                "active",
                BindingFlags.Static |
                BindingFlags.NonPublic);

        private static readonly MethodInfo TryClaimAuthorityMethod =
            typeof(EchoUIRoot).GetMethod(
                "TryClaimAuthority",
                BindingFlags.Instance |
                BindingFlags.NonPublic);

        private static readonly MethodInfo LateUpdateMethod =
            typeof(EchoUIRoot).GetMethod(
                "LateUpdate",
                BindingFlags.Instance |
                BindingFlags.NonPublic);

        private static readonly MethodInfo OnDestroyMethod =
            typeof(EchoUIRoot).GetMethod(
                "OnDestroy",
                BindingFlags.Instance |
                BindingFlags.NonPublic);

        private readonly List<GameObject> ownedObjects =
            new List<GameObject>();

        private EchoUIRoot previousActiveRoot;

        [SetUp]
        public void SetUp()
        {
            previousActiveRoot =
                EchoUIRoot.Active;
        }

        [TearDown]
        public void TearDown()
        {
            for (int index = ownedObjects.Count - 1;
                 index >= 0;
                 index--)
            {
                if (ownedObjects[index] != null)
                {
                    UnityEngine.Object.DestroyImmediate(
                        ownedObjects[index]);
                }
            }

            ownedObjects.Clear();
            SetActiveRootForTest(previousActiveRoot);
        }

        [Test]
        public void AttachSynchronizesAuthoredChannelOrderAndVisibleData()
        {
            UINotificationService service =
                CreateService(
                    visibleCapacity: 2,
                    pendingCapacity: 1,
                    includeSecondary: true);

            object firstPresentation =
                new object();

            UINotificationHandle first =
                Admit(
                    service,
                    PrimaryChannel,
                    firstPresentation,
                    priority: 2,
                    coalescingKey: "first-key",
                    correlationId: "first-correlation");

            UINotificationHandle second =
                Admit(
                    service,
                    PrimaryChannel,
                    "second",
                    priority: 9);

            Admit(
                service,
                PrimaryChannel,
                "pending",
                priority: 20);

            UINotificationHandle secondary =
                Admit(
                    service,
                    SecondaryChannel,
                    "secondary");

            RecordingPresenter presenter =
                new RecordingPresenter();

            Assert.That(
                service.SetPresenter(presenter),
                Is.True);

            Assert.That(presenter.Snapshots.Count, Is.EqualTo(2));

            UINotificationPresentationSnapshot primary =
                presenter.Snapshots[0];

            Assert.That(
                primary.ChannelId.Value,
                Is.EqualTo(PrimaryChannel));

            Assert.That(primary.VisibleCount, Is.EqualTo(2));
            Assert.That(primary.VisibleEntries[0].Handle, Is.SameAs(first));
            Assert.That(primary.VisibleEntries[1].Handle, Is.SameAs(second));

            Assert.That(
                primary.VisibleEntries[0].Presentation,
                Is.SameAs(firstPresentation));

            Assert.That(primary.VisibleEntries[0].Priority, Is.EqualTo(2));

            Assert.That(
                primary.VisibleEntries[0].CoalescingKey.Value,
                Is.EqualTo("first-key"));

            Assert.That(
                primary.VisibleEntries[0].CorrelationId.Value,
                Is.EqualTo("first-correlation"));

            UINotificationPresentationSnapshot secondarySnapshot =
                presenter.Snapshots[1];

            Assert.That(
                secondarySnapshot.ChannelId.Value,
                Is.EqualTo(SecondaryChannel));

            Assert.That(
                secondarySnapshot.VisibleEntries[0].Handle,
                Is.SameAs(secondary));
        }

        [Test]
        public void PresentationEntriesAreReadOnlyAndBoundedByVisibleCapacity()
        {
            UINotificationService service =
                CreateService(
                    visibleCapacity: 1,
                    pendingCapacity: 2);

            Admit(
                service,
                PrimaryChannel,
                "visible");

            Admit(
                service,
                PrimaryChannel,
                "pending-one");

            Admit(
                service,
                PrimaryChannel,
                "pending-two");

            RecordingPresenter presenter =
                Attach(service);

            UINotificationPresentationSnapshot snapshot =
                presenter.Snapshots[0];

            Assert.That(snapshot.VisibleCount, Is.EqualTo(1));

            IList<UINotificationPresentationEntry> collection =
                snapshot.VisibleEntries as
                    IList<UINotificationPresentationEntry>;

            Assert.That(collection, Is.Not.Null);

            Assert.Throws<NotSupportedException>(() =>
                collection[0] = default);
        }

        [Test]
        public void AcceptedMutationsPublishAfterCommittedChannelTruth()
        {
            UINotificationService service =
                CreateService(
                    pendingCapacity: 1);

            RecordingPresenter presenter =
                Attach(service);

            presenter.Snapshots.Clear();

            bool statusObserved = false;
            bool presenterObservedAfterStatus = false;

            service.ChannelChanged += _ =>
                statusObserved = true;

            presenter.Callback = snapshot =>
            {
                presenterObservedAfterStatus =
                    statusObserved &&
                    snapshot.VisibleCount == 1;
            };

            Admit(
                service,
                PrimaryChannel,
                "visible");

            Assert.That(presenter.Snapshots.Count, Is.EqualTo(1));
            Assert.That(presenterObservedAfterStatus, Is.True);

            presenter.Callback = null;

            Admit(
                service,
                PrimaryChannel,
                "pending");

            Assert.That(presenter.Snapshots.Count, Is.EqualTo(2));
            Assert.That(presenter.Snapshots[1].VisibleCount, Is.EqualTo(1));
        }

        [Test]
        public void VisibleCoalescingPublishesOnlyReplacementGeneration()
        {
            UINotificationService service =
                CreateService();

            UINotificationHandle prior =
                Admit(
                    service,
                    PrimaryChannel,
                    "prior",
                    coalescingKey: "stable");

            RecordingPresenter presenter =
                Attach(service);

            presenter.Snapshots.Clear();

            UINotificationHandle replacement =
                Admit(
                    service,
                    PrimaryChannel,
                    "replacement",
                    coalescingKey: "stable");

            Assert.That(
                prior.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Superseded));

            Assert.That(presenter.Snapshots.Count, Is.EqualTo(1));

            Assert.That(
                presenter.Snapshots[0].VisibleEntries[0].Handle,
                Is.SameAs(replacement));
        }

        [Test]
        public void VisibleDismissalPublishesDeterministicPromotion()
        {
            UINotificationService service =
                CreateService(
                    pendingCapacity: 2);

            UINotificationHandle visible =
                Admit(
                    service,
                    PrimaryChannel,
                    "visible");

            Admit(
                service,
                PrimaryChannel,
                "lower",
                priority: 1);

            UINotificationHandle higher =
                Admit(
                    service,
                    PrimaryChannel,
                    "higher",
                    priority: 10);

            RecordingPresenter presenter =
                Attach(service);

            presenter.Snapshots.Clear();

            Assert.That(
                service.Dismiss(visible).Status,
                Is.EqualTo(
                    UINotificationOperationStatus.Completed));

            Assert.That(presenter.Snapshots.Count, Is.EqualTo(1));

            Assert.That(
                presenter.Snapshots[0].VisibleEntries[0].Handle,
                Is.SameAs(higher));
        }

        [Test]
        public void ReplacementClearsOutgoingThenSynchronizesIncoming()
        {
            UINotificationService service =
                CreateService(
                    includeSecondary: true);

            Admit(
                service,
                PrimaryChannel,
                "primary");

            Admit(
                service,
                SecondaryChannel,
                "secondary");

            RecordingPresenter outgoing =
                Attach(service);

            outgoing.Snapshots.Clear();

            RecordingPresenter incoming =
                new RecordingPresenter();

            Assert.That(
                service.SetPresenter(incoming),
                Is.True);

            Assert.That(outgoing.Snapshots.Count, Is.EqualTo(2));
            Assert.That(outgoing.Snapshots[0].VisibleCount, Is.EqualTo(0));
            Assert.That(outgoing.Snapshots[1].VisibleCount, Is.EqualTo(0));

            Assert.That(incoming.Snapshots.Count, Is.EqualTo(2));
            Assert.That(incoming.Snapshots[0].VisibleCount, Is.EqualTo(1));
            Assert.That(incoming.Snapshots[1].VisibleCount, Is.EqualTo(1));

            Assert.That(
                service.SetPresenter(incoming),
                Is.True);

            Assert.That(incoming.Snapshots.Count, Is.EqualTo(2));

            Assert.That(
                service.SetPresenter(null),
                Is.True);

            Assert.That(incoming.Snapshots.Count, Is.EqualTo(4));
            Assert.That(service.HasPresenter, Is.False);
        }

        [Test]
        public void PresenterFailureDoesNotRollbackCommittedTruth()
        {
            UINotificationService service =
                CreateService();

            UINotificationHandle handle =
                Admit(
                    service,
                    PrimaryChannel,
                    "committed");

            LogAssert.Expect(
                LogType.Exception,
                new Regex(
                    "InvalidOperationException: presenter-observer"));

            Assert.That(
                service.SetPresenter(
                    new FailingPresenter()),
                Is.True);

            AssertState(
                service,
                handle,
                UINotificationEntryState.Visible);
        }

        [Test]
        public void PresenterCallbackCannotReenterMutation()
        {
            UINotificationService service =
                CreateService();

            RecordingPresenter presenter =
                Attach(service);

            UINotificationHandle reentrant =
                null;

            int resetCount = -1;

            presenter.Callback = _ =>
            {
                resetCount =
                    service.Reset();

                reentrant =
                    service.Admit(
                        new UINotificationRequest(
                            PrimaryChannel,
                            "reentrant"));
            };

            UINotificationHandle admitted =
                Admit(
                    service,
                    PrimaryChannel,
                    "admitted");

            Assert.That(resetCount, Is.EqualTo(0));
            Assert.That(reentrant, Is.Not.Null);
            Assert.That(reentrant.Accepted, Is.False);

            Assert.That(
                reentrant.Admission.Status,
                Is.EqualTo(
                    UINotificationAdmissionStatus.Unavailable));

            AssertState(
                service,
                admitted,
                UINotificationEntryState.Visible);
        }

        [Test]
        public void RejectionsReadsAndIdleWorkPublishNothing()
        {
            UINotificationService service =
                CreateService(
                    pendingCapacity: 0);

            Admit(
                service,
                PrimaryChannel,
                "retained");

            RecordingPresenter presenter =
                Attach(service);

            presenter.Snapshots.Clear();

            UINotificationHandle rejected =
                service.Admit(
                    new UINotificationRequest(
                        PrimaryChannel,
                        "rejected"));

            Assert.That(rejected.Accepted, Is.False);

            Assert.That(
                service.TryGetSnapshot(
                    PrimaryChannel,
                    out _),
                Is.True);

            Assert.That(service.Tick(), Is.EqualTo(0));
            Assert.That(service.RefreshDestroyedOwners(), Is.EqualTo(0));

            Assert.That(
                service.RefreshDestroyedPresentations(),
                Is.EqualTo(0));

            Assert.That(presenter.Snapshots, Is.Empty);
        }

        [Test]
        public void ResetPublishesEmptyTruthAfterEveryHandleSettles()
        {
            UINotificationService service =
                CreateService(
                    pendingCapacity: 1);

            UINotificationHandle visible =
                Admit(
                    service,
                    PrimaryChannel,
                    "visible");

            UINotificationHandle pending =
                Admit(
                    service,
                    PrimaryChannel,
                    "pending");

            RecordingPresenter presenter =
                Attach(service);

            presenter.Snapshots.Clear();

            bool settledAtPresentation = false;

            presenter.Callback = snapshot =>
            {
                settledAtPresentation =
                    visible.IsCompleted &&
                    pending.IsCompleted &&
                    snapshot.VisibleCount == 0;
            };

            Assert.That(service.Reset(), Is.EqualTo(2));
            Assert.That(settledAtPresentation, Is.True);
            Assert.That(presenter.Snapshots.Count, Is.EqualTo(1));
        }

        [Test]
        public void ShutdownPublishesFinalEmptyTruthAndReleasesPresenter()
        {
            UINotificationService service =
                CreateService(
                    includeSecondary: true);

            UINotificationHandle primary =
                Admit(
                    service,
                    PrimaryChannel,
                    "primary");

            UINotificationHandle secondary =
                Admit(
                    service,
                    SecondaryChannel,
                    "secondary");

            RecordingPresenter presenter =
                Attach(service);

            presenter.Snapshots.Clear();

            Assert.That(service.Shutdown(), Is.EqualTo(2));
            Assert.That(presenter.Snapshots.Count, Is.EqualTo(2));
            Assert.That(presenter.Snapshots[0].VisibleCount, Is.EqualTo(0));
            Assert.That(presenter.Snapshots[1].VisibleCount, Is.EqualTo(0));
            Assert.That(primary.IsCompleted, Is.True);
            Assert.That(secondary.IsCompleted, Is.True);
            Assert.That(service.HasPresenter, Is.False);
            Assert.That(service.SetPresenter(presenter), Is.False);
        }

        [Test]
        public void DestroyedVisiblePresentationSettlesAndPromotesWinner()
        {
            UINotificationService service =
                CreateService(
                    pendingCapacity: 2);

            GameObject presentation =
                CreateObject(
                    "visible-presentation");

            UINotificationHandle lost =
                Admit(
                    service,
                    PrimaryChannel,
                    presentation);

            Admit(
                service,
                PrimaryChannel,
                "lower",
                priority: 1);

            UINotificationHandle winner =
                Admit(
                    service,
                    PrimaryChannel,
                    "winner",
                    priority: 10);

            RecordingPresenter presenter =
                Attach(service);

            presenter.Snapshots.Clear();

            UnityEngine.Object.DestroyImmediate(
                presentation);

            Assert.That(
                service.RefreshDestroyedPresentations(),
                Is.EqualTo(1));

            Assert.That(
                lost.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.PresentationLost));

            Assert.That(
                presenter.Snapshots[0].VisibleEntries[0].Handle,
                Is.SameAs(winner));
        }

        [Test]
        public void DestroyedPendingPresentationLeavesVisibleGeneration()
        {
            UINotificationService service =
                CreateService(
                    pendingCapacity: 1);

            UINotificationHandle visible =
                Admit(
                    service,
                    PrimaryChannel,
                    "visible");

            GameObject presentation =
                CreateObject(
                    "pending-presentation");

            UINotificationHandle lost =
                Admit(
                    service,
                    PrimaryChannel,
                    presentation);

            RecordingPresenter presenter =
                Attach(service);

            presenter.Snapshots.Clear();

            UnityEngine.Object.DestroyImmediate(
                presentation);

            Assert.That(
                service.RefreshDestroyedPresentations(),
                Is.EqualTo(1));

            Assert.That(
                lost.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.PresentationLost));

            Assert.That(
                presenter.Snapshots[0].VisibleEntries[0].Handle,
                Is.SameAs(visible));
        }

        [Test]
        public void NonUnityPresentationSurvivesPresentationRefresh()
        {
            UINotificationService service =
                CreateService();

            UINotificationHandle handle =
                Admit(
                    service,
                    PrimaryChannel,
                    new object());

            Assert.That(
                service.RefreshDestroyedPresentations(),
                Is.EqualTo(0));

            AssertState(
                service,
                handle,
                UINotificationEntryState.Visible);
        }

        [Test]
        public void SupersededDestroyedPresentationCannotRemoveReplacement()
        {
            UINotificationService service =
                CreateService();

            GameObject presentation =
                CreateObject(
                    "superseded-presentation");

            UINotificationHandle prior =
                Admit(
                    service,
                    PrimaryChannel,
                    presentation,
                    coalescingKey: "stable");

            UINotificationHandle replacement =
                Admit(
                    service,
                    PrimaryChannel,
                    "replacement",
                    coalescingKey: "stable");

            Assert.That(
                prior.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Superseded));

            UnityEngine.Object.DestroyImmediate(
                presentation);

            Assert.That(
                service.RefreshDestroyedPresentations(),
                Is.EqualTo(0));

            AssertState(
                service,
                replacement,
                UINotificationEntryState.Visible);
        }

        [Test]
        public void AlreadyDestroyedUnityPresentationIsRejectedWithoutMutation()
        {
            UINotificationService service =
                CreateService();

            GameObject presentation =
                CreateObject(
                    "already-destroyed");

            UnityEngine.Object.DestroyImmediate(
                presentation);

            UINotificationHandle rejected =
                service.Admit(
                    new UINotificationRequest(
                        PrimaryChannel,
                        presentation));

            Assert.That(rejected.Accepted, Is.False);

            Assert.That(
                rejected.Admission.Status,
                Is.EqualTo(
                    UINotificationAdmissionStatus.Invalid));

            AssertCounts(
                service,
                visible: 0,
                pending: 0);
        }

        [Test]
        public void RootRetainsPresenterAndRefreshesDestroyedPresentation()
        {
            SetActiveRootForTest(null);

            GameObject rootObject =
                CreateObject(
                    "presenter-root");

            EchoUIRoot root =
                rootObject.AddComponent<EchoUIRoot>();

            ClaimAuthorityForTest(root);

            RecordingPresenter presenter =
                new RecordingPresenter();

            Assert.That(
                root.SetNotificationPresenter(
                    presenter),
                Is.True);

            Assert.That(root.Initialize().Succeeded, Is.True);
            Assert.That(presenter.Snapshots.Count, Is.EqualTo(1));
            Assert.That(presenter.Snapshots[0].VisibleCount, Is.EqualTo(0));

            GameObject presentation =
                CreateObject(
                    "root-presentation");

            UINotificationHandle lost =
                root.AdmitNotification(
                    new UINotificationRequest(
                        DefaultChannel,
                        presentation,
                        lifetimeMode:
                            UINotificationLifetimeMode.Manual));

            Assert.That(lost.Accepted, Is.True);

            UnityEngine.Object.DestroyImmediate(
                presentation);

            Assert.That(LateUpdateMethod, Is.Not.Null);
            LateUpdateMethod.Invoke(root, null);

            Assert.That(
                lost.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.PresentationLost));

            UINotificationHandle retained =
                root.AdmitNotification(
                    new UINotificationRequest(
                        DefaultChannel,
                        "retained",
                        lifetimeMode:
                            UINotificationLifetimeMode.Manual));

            Assert.That(retained.Accepted, Is.True);

            Assert.That(OnDestroyMethod, Is.Not.Null);
            OnDestroyMethod.Invoke(root, null);

            Assert.That(
                retained.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Shutdown));

            Assert.That(
                presenter.Snapshots[
                    presenter.Snapshots.Count - 1].VisibleCount,
                Is.EqualTo(0));
        }

        private const string DefaultChannel =
            "notification.default";

        private const string PrimaryChannel =
            "notification.primary";

        private const string SecondaryChannel =
            "notification.secondary";

        private UINotificationService CreateService(
            int visibleCapacity = 1,
            int pendingCapacity = 2,
            bool includeSecondary = false)
        {
            List<UINotificationChannelDefinition> definitions =
                new List<UINotificationChannelDefinition>
                {
                    new UINotificationChannelDefinition(
                        PrimaryChannel,
                        visibleCapacity,
                        pendingCapacity)
                };

            if (includeSecondary)
            {
                definitions.Add(
                    new UINotificationChannelDefinition(
                        SecondaryChannel,
                        visibleCapacity,
                        pendingCapacity));
            }

            UINotificationService service =
                new UINotificationService(
                    definitions,
                    out string validationError);

            Assert.That(validationError, Is.Empty);
            Assert.That(service.IsValid, Is.True);
            return service;
        }

        private static RecordingPresenter Attach(
            UINotificationService service)
        {
            RecordingPresenter presenter =
                new RecordingPresenter();

            Assert.That(
                service.SetPresenter(presenter),
                Is.True);

            return presenter;
        }

        private static UINotificationHandle Admit(
            UINotificationService service,
            string channelId,
            object presentation,
            int priority = 0,
            string coalescingKey = "",
            string correlationId = "")
        {
            UINotificationHandle handle =
                service.Admit(
                    new UINotificationRequest(
                        channelId,
                        presentation,
                        priority,
                        UINotificationLifetimeMode.Manual,
                        coalescingKey: coalescingKey,
                        correlationId: correlationId));

            Assert.That(handle.Accepted, Is.True);
            return handle;
        }

        private GameObject CreateObject(
            string name)
        {
            GameObject value =
                new GameObject(name);

            ownedObjects.Add(value);
            return value;
        }

        private static void AssertState(
            UINotificationService service,
            UINotificationHandle handle,
            UINotificationEntryState expected)
        {
            Assert.That(
                service.TryGetEntryState(
                    handle,
                    out UINotificationEntryState state),
                Is.True);

            Assert.That(state, Is.EqualTo(expected));
        }

        private static void AssertCounts(
            UINotificationService service,
            int visible,
            int pending)
        {
            Assert.That(service.VisibleCount, Is.EqualTo(visible));
            Assert.That(service.PendingCount, Is.EqualTo(pending));
        }

        private static void SetActiveRootForTest(
            EchoUIRoot value)
        {
            Assert.That(ActiveRootField, Is.Not.Null);
            ActiveRootField.SetValue(null, value);
        }

        private static void ClaimAuthorityForTest(
            EchoUIRoot value)
        {
            Assert.That(TryClaimAuthorityMethod, Is.Not.Null);
            TryClaimAuthorityMethod.Invoke(value, null);
        }
    }
}
