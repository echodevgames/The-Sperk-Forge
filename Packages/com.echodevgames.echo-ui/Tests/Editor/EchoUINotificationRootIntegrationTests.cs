using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUINotificationRootIntegrationTests
    {
        private sealed class FakeClock :
            IUINotificationClock
        {
            public double NowSeconds { get; set; }
        }

        private static readonly FieldInfo ActiveRootField =
            typeof(EchoUIRoot).GetField(
                "active",
                BindingFlags.Static |
                BindingFlags.NonPublic);

        private static readonly FieldInfo NotificationServiceField =
            typeof(EchoUIRoot).GetField(
                "notificationService",
                BindingFlags.Instance |
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
        private GameObject rootObject;
        private EchoUIRoot root;

        [SetUp]
        public void SetUp()
        {
            previousActiveRoot =
                EchoUIRoot.Active;

            SetActiveRootForTest(null);

            rootObject =
                new GameObject(
                    "notification-root");

            root =
                rootObject.AddComponent<EchoUIRoot>();

            ClaimAuthorityForTest(root);
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

            if (rootObject != null)
            {
                UnityEngine.Object.DestroyImmediate(
                    rootObject);
            }

            SetActiveRootForTest(
                previousActiveRoot);
        }

        [Test]
        public void DefaultChannelInitializesWithRoot()
        {
            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            Assert.That(
                root.IsNotificationLifecycleInitialized,
                Is.True);

            Assert.That(root.NotificationChannelCount, Is.EqualTo(1));
            Assert.That(root.VisibleNotificationCount, Is.EqualTo(0));
            Assert.That(root.PendingNotificationCount, Is.EqualTo(0));

            Assert.That(
                root.TryGetNotificationChannelDefinition(
                    DefaultChannel,
                    out UINotificationChannelDefinition definition),
                Is.True);

            Assert.That(
                definition.ChannelId,
                Is.EqualTo(
                    new UINotificationChannelId(DefaultChannel)));

            AssertSnapshot(
                root,
                DefaultChannel,
                visible: 0,
                pending: 0);
        }

        [Test]
        public void AuthoredChannelsInitializeAsImmutableSnapshots()
        {
            ConfigureDefinitions(
                new UINotificationChannelDefinition(
                    PrimaryChannel,
                    visibleCapacity: 1,
                    pendingCapacity: 2,
                    defaultLifetimeSeconds: 5f),
                new UINotificationChannelDefinition(
                    SecondaryChannel,
                    visibleCapacity: 2,
                    pendingCapacity: 3,
                    defaultLifetimeSeconds: 7f,
                    overflowPolicy:
                        UINotificationOverflowPolicy.DropOldestPending));

            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            Assert.That(root.NotificationChannelCount, Is.EqualTo(2));

            ConfigureDefinitions(
                new UINotificationChannelDefinition(
                    PrimaryChannel,
                    visibleCapacity: 9,
                    pendingCapacity: 9));

            Assert.That(
                root.TryGetNotificationChannelDefinition(
                    PrimaryChannel,
                    out UINotificationChannelDefinition primary),
                Is.True);

            Assert.That(primary.VisibleCapacity, Is.EqualTo(1));
            Assert.That(primary.PendingCapacity, Is.EqualTo(2));
            Assert.That(primary.DefaultLifetimeSeconds, Is.EqualTo(5f));

            Assert.That(
                root.TryGetNotificationChannelDefinition(
                    SecondaryChannel,
                    out UINotificationChannelDefinition secondary),
                Is.True);

            Assert.That(
                secondary.OverflowPolicy,
                Is.EqualTo(
                    UINotificationOverflowPolicy.DropOldestPending));
        }

        [Test]
        public void EmptyDefinitionsRejectBeforeRootStateCommit()
        {
            CreateSurface(
                "main-menu",
                UISurfaceRole.Screen,
                "frontend",
                startVisible: true);

            ConfigureDefinitions();

            UISurfaceOperationResult result =
                root.Initialize();

            Assert.That(
                result.Status,
                Is.EqualTo(
                    UISurfaceOperationStatus.InvalidDefinition));

            Assert.That(root.IsInitialized, Is.False);

            Assert.That(
                root.IsNotificationLifecycleInitialized,
                Is.False);

            Assert.That(root.RegisteredSurfaceCount, Is.EqualTo(0));
            Assert.That(root.IsHudLifecycleInitialized, Is.False);
        }

        [Test]
        public void DuplicateDefinitionsRejectWithoutPartialLifecycle()
        {
            ConfigureDefinitions(
                new UINotificationChannelDefinition(
                    " notification.primary "),
                new UINotificationChannelDefinition(
                    "notification.primary"));

            UISurfaceOperationResult result =
                root.Initialize();

            Assert.That(
                result.Status,
                Is.EqualTo(
                    UISurfaceOperationStatus.InvalidDefinition));

            Assert.That(root.IsInitialized, Is.False);

            Assert.That(
                root.IsNotificationLifecycleInitialized,
                Is.False);

            Assert.That(root.NotificationChannelCount, Is.EqualTo(0));
            Assert.That(root.VisibleNotificationCount, Is.EqualTo(0));
            Assert.That(root.PendingNotificationCount, Is.EqualTo(0));
        }

        [Test]
        public void OperationsRejectBeforeInitialization()
        {
            UINotificationHandle rejected =
                root.AdmitNotification(
                    new UINotificationRequest(
                        DefaultChannel,
                        "unavailable"));

            Assert.That(rejected.Accepted, Is.False);

            Assert.That(
                rejected.Admission.Status,
                Is.EqualTo(
                    UINotificationAdmissionStatus.Unavailable));

            Assert.That(
                root.DismissNotification(rejected).Status,
                Is.EqualTo(
                    UINotificationOperationStatus.Unavailable));

            Assert.That(root.ResetNotifications(), Is.EqualTo(0));

            Assert.That(
                root.TryGetNotificationChannelSnapshot(
                    DefaultChannel,
                    out _),
                Is.False);
        }

        [Test]
        public void RootForwardsAdmissionDismissalStatusAndEvents()
        {
            List<UINotificationChannelSnapshot> observed =
                new List<UINotificationChannelSnapshot>();

            root.NotificationChannelChanged +=
                observed.Add;

            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            Assert.That(observed, Is.Empty);

            UINotificationHandle handle =
                Admit(
                    root,
                    DefaultChannel,
                    "visible");

            Assert.That(observed.Count, Is.EqualTo(1));
            Assert.That(root.VisibleNotificationCount, Is.EqualTo(1));
            Assert.That(root.PendingNotificationCount, Is.EqualTo(0));

            Assert.That(
                root.TryGetNotificationEntryState(
                    handle,
                    out UINotificationEntryState state),
                Is.True);

            Assert.That(
                state,
                Is.EqualTo(
                    UINotificationEntryState.Visible));

            Assert.That(
                root.DismissNotification(handle).Status,
                Is.EqualTo(
                    UINotificationOperationStatus.Completed));

            Assert.That(observed.Count, Is.EqualTo(2));
            Assert.That(handle.IsCompleted, Is.True);

            Assert.That(
                handle.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Dismissed));

            AssertSnapshot(
                root,
                DefaultChannel,
                visible: 0,
                pending: 0);
        }

        [Test]
        public void RootPreservesIndependentAuthoredChannelBounds()
        {
            ConfigureDefinitions(
                new UINotificationChannelDefinition(
                    PrimaryChannel,
                    visibleCapacity: 1,
                    pendingCapacity: 1),
                new UINotificationChannelDefinition(
                    SecondaryChannel,
                    visibleCapacity: 1,
                    pendingCapacity: 1));

            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            Admit(
                root,
                PrimaryChannel,
                "primary-visible");

            Admit(
                root,
                PrimaryChannel,
                "primary-pending");

            Admit(
                root,
                SecondaryChannel,
                "secondary-visible");

            Assert.That(root.VisibleNotificationCount, Is.EqualTo(2));
            Assert.That(root.PendingNotificationCount, Is.EqualTo(1));

            AssertSnapshot(
                root,
                PrimaryChannel,
                visible: 1,
                pending: 1);

            AssertSnapshot(
                root,
                SecondaryChannel,
                visible: 1,
                pending: 0);
        }

        [Test]
        public void RootResetSettlesAllAndPreservesFreshGeneration()
        {
            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            UINotificationHandle visible =
                Admit(
                    root,
                    DefaultChannel,
                    "visible");

            UINotificationHandle pending =
                Admit(
                    root,
                    DefaultChannel,
                    "pending");

            Assert.That(root.ResetNotifications(), Is.EqualTo(2));

            Assert.That(
                visible.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Reset));

            Assert.That(
                pending.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Reset));

            Assert.That(root.VisibleNotificationCount, Is.EqualTo(0));
            Assert.That(root.PendingNotificationCount, Is.EqualTo(0));

            UINotificationHandle replacement =
                Admit(
                    root,
                    DefaultChannel,
                    "replacement");

            Assert.That(
                replacement.Generation,
                Is.GreaterThan(pending.Generation));

            Assert.That(
                root.DismissNotification(visible).Status,
                Is.EqualTo(
                    UINotificationOperationStatus.AlreadySettled));

            Assert.That(
                root.TryGetNotificationEntryState(
                    replacement,
                    out _),
                Is.True);
        }

        [Test]
        public void RejectedRootAdmissionDoesNotPublishOrMutate()
        {
            ConfigureDefinitions(
                new UINotificationChannelDefinition(
                    PrimaryChannel,
                    visibleCapacity: 1,
                    pendingCapacity: 0));

            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            Admit(
                root,
                PrimaryChannel,
                "retained");

            List<UINotificationChannelSnapshot> observed =
                new List<UINotificationChannelSnapshot>();

            root.NotificationChannelChanged +=
                observed.Add;

            UINotificationHandle rejected =
                root.AdmitNotification(
                    new UINotificationRequest(
                        PrimaryChannel,
                        "rejected"));

            Assert.That(rejected.Accepted, Is.False);

            Assert.That(
                rejected.Admission.Status,
                Is.EqualTo(
                    UINotificationAdmissionStatus.CapacityExceeded));

            Assert.That(observed, Is.Empty);
            Assert.That(root.VisibleNotificationCount, Is.EqualTo(1));
            Assert.That(root.PendingNotificationCount, Is.EqualTo(0));
        }

        [Test]
        public void RootListenerFailureCannotRollbackCommittedTruth()
        {
            Action<UINotificationChannelSnapshot> failingListener =
                _ => throw new InvalidOperationException(
                    "root-notification-observer");

            root.NotificationChannelChanged +=
                failingListener;

            int healthyListenerCount = 0;

            root.NotificationChannelChanged += _ =>
                healthyListenerCount++;

            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            LogAssert.Expect(
                LogType.Exception,
                new Regex(
                    "InvalidOperationException: root-notification-observer"));

            UINotificationHandle handle =
                Admit(
                    root,
                    DefaultChannel,
                    "committed");

            Assert.That(healthyListenerCount, Is.EqualTo(1));

            Assert.That(
                root.TryGetNotificationEntryState(
                    handle,
                    out _),
                Is.True);

            root.NotificationChannelChanged -=
                failingListener;
        }

        [Test]
        public void LateUpdateCleansDestroyedOwner()
        {
            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            GameObject owner =
                CreateOwnedObject(
                    "notification-owner");

            UINotificationHandle handle =
                Admit(
                    root,
                    DefaultChannel,
                    "owned",
                    owner: owner);

            UnityEngine.Object.DestroyImmediate(owner);

            InvokeLateUpdate();

            Assert.That(handle.IsCompleted, Is.True);

            Assert.That(
                handle.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.OwnerLost));

            Assert.That(root.VisibleNotificationCount, Is.EqualTo(0));
        }

        [Test]
        public void LateUpdateAdvancesInjectedUnscaledClock()
        {
            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            FakeClock clock =
                new FakeClock();

            ReplaceNotificationService(
                new UINotificationService(
                    new[]
                    {
                        new UINotificationChannelDefinition(
                            DefaultChannel)
                    },
                    clock,
                    out string validationError));

            Assert.That(validationError, Is.Empty);

            UINotificationHandle handle =
                Admit(
                    root,
                    DefaultChannel,
                    "automatic",
                    lifetimeMode:
                        UINotificationLifetimeMode.Automatic,
                    durationSeconds: 1f);

            clock.NowSeconds = 1d;

            InvokeLateUpdate();

            Assert.That(handle.IsCompleted, Is.True);

            Assert.That(
                handle.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Expired));

            Assert.That(root.VisibleNotificationCount, Is.EqualTo(0));
        }

        [Test]
        public void RootDestroyShutsDownEveryGenerationExactlyOnce()
        {
            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            UINotificationHandle visible =
                Admit(
                    root,
                    DefaultChannel,
                    "visible");

            UINotificationHandle pending =
                Admit(
                    root,
                    DefaultChannel,
                    "pending");

            int visibleCompletionCount = 0;
            int pendingCompletionCount = 0;

            visible.Completed += _ => visibleCompletionCount++;
            pending.Completed += _ => pendingCompletionCount++;

            bool finalEventObserved = false;

            root.NotificationChannelChanged += snapshot =>
            {
                finalEventObserved =
                    snapshot.VisibleCount == 0 &&
                    snapshot.PendingCount == 0 &&
                    !root.IsNotificationLifecycleInitialized;
            };

            Assert.That(OnDestroyMethod, Is.Not.Null);
            OnDestroyMethod.Invoke(root, null);

            Assert.That(finalEventObserved, Is.True);
            Assert.That(visibleCompletionCount, Is.EqualTo(1));
            Assert.That(pendingCompletionCount, Is.EqualTo(1));

            Assert.That(
                visible.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Shutdown));

            Assert.That(
                pending.Result.Outcome,
                Is.EqualTo(
                    UINotificationOutcome.Shutdown));

            UnityEngine.Object.DestroyImmediate(
                rootObject);

            rootObject = null;
            root = null;

            Assert.That(visibleCompletionCount, Is.EqualTo(1));
            Assert.That(pendingCompletionCount, Is.EqualTo(1));
        }

        [Test]
        public void NotificationMutationDoesNotChangeStructuralUiTruth()
        {
            UISurface screen =
                CreateSurface(
                    "main-menu",
                    UISurfaceRole.Screen,
                    "frontend",
                    startVisible: true);

            UISurface window =
                CreateSurface(
                    "default-window",
                    UISurfaceRole.Window,
                    string.Empty,
                    startVisible: false);

            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            int registeredSurfaces =
                root.RegisteredSurfaceCount;

            int hudRegions =
                root.HudRegionCount;

            UINotificationHandle first =
                Admit(
                    root,
                    DefaultChannel,
                    "first");

            Admit(
                root,
                DefaultChannel,
                "second");

            root.DismissNotification(first);
            root.ResetNotifications();

            Assert.That(
                root.RegisteredSurfaceCount,
                Is.EqualTo(registeredSurfaces));

            Assert.That(root.HudRegionCount, Is.EqualTo(hudRegions));
            Assert.That(root.ActiveModalCount, Is.EqualTo(0));

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("main-menu"));

            Assert.That(screen.IsVisible, Is.True);
            Assert.That(window.IsVisible, Is.False);
        }

        [Test]
        public void RootEventCallbackCannotReenterMutation()
        {
            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            int eventCount = 0;
            int resetCount = -1;

            UINotificationHandle reentrant =
                null;

            root.NotificationChannelChanged += _ =>
            {
                eventCount++;

                resetCount =
                    root.ResetNotifications();

                reentrant =
                    root.AdmitNotification(
                        new UINotificationRequest(
                            DefaultChannel,
                            "reentrant"));
            };

            UINotificationHandle admitted =
                Admit(
                    root,
                    DefaultChannel,
                    "admitted");

            Assert.That(eventCount, Is.EqualTo(1));
            Assert.That(resetCount, Is.EqualTo(0));

            Assert.That(reentrant, Is.Not.Null);
            Assert.That(reentrant.Accepted, Is.False);

            Assert.That(
                reentrant.Admission.Status,
                Is.EqualTo(
                    UINotificationAdmissionStatus.Unavailable));

            Assert.That(
                root.TryGetNotificationEntryState(
                    admitted,
                    out _),
                Is.True);

            Assert.That(root.VisibleNotificationCount, Is.EqualTo(1));
        }

        private const string DefaultChannel =
            "notification.default";

        private const string PrimaryChannel =
            "notification.primary";

        private const string SecondaryChannel =
            "notification.secondary";

        private void ConfigureDefinitions(
            params UINotificationChannelDefinition[] definitions)
        {
            SerializedObject serialized =
                new SerializedObject(root);

            SerializedProperty collection =
                serialized.FindProperty(
                    "notificationChannelDefinitions");

            Assert.That(collection, Is.Not.Null);

            collection.arraySize =
                definitions.Length;

            for (int index = 0;
                 index < definitions.Length;
                 index++)
            {
                UINotificationChannelDefinition definition =
                    definitions[index];

                SerializedProperty element =
                    collection.GetArrayElementAtIndex(index);

                element.FindPropertyRelative("channelId")
                    .stringValue = definition.ChannelId.Value;

                element.FindPropertyRelative("visibleCapacity")
                    .intValue = definition.VisibleCapacity;

                element.FindPropertyRelative("pendingCapacity")
                    .intValue = definition.PendingCapacity;

                element.FindPropertyRelative("defaultLifetimeSeconds")
                    .floatValue = definition.DefaultLifetimeSeconds;

                element.FindPropertyRelative("overflowPolicy")
                    .enumValueIndex = (int)definition.OverflowPolicy;
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private GameObject CreateOwnedObject(
            string name)
        {
            GameObject value =
                new GameObject(name);

            ownedObjects.Add(value);
            return value;
        }

        private UISurface CreateSurface(
            string surfaceId,
            UISurfaceRole role,
            string scopeId,
            bool startVisible)
        {
            GameObject child =
                new GameObject(surfaceId);

            child.transform.SetParent(
                rootObject.transform,
                false);

            UISurface surface =
                child.AddComponent<UISurface>();

            SerializedObject serialized =
                new SerializedObject(surface);

            serialized.FindProperty("surfaceId")
                .stringValue = surfaceId;

            serialized.FindProperty("displayLabel")
                .stringValue = surfaceId;

            serialized.FindProperty("role")
                .enumValueIndex = (int)role;

            serialized.FindProperty("navigationScopeId")
                .stringValue = scopeId;

            serialized.FindProperty("startVisible")
                .boolValue = startVisible;

            serialized.ApplyModifiedPropertiesWithoutUndo();
            return surface;
        }

        private static UINotificationHandle Admit(
            EchoUIRoot root,
            string channelId,
            string presentation,
            UINotificationLifetimeMode lifetimeMode =
                UINotificationLifetimeMode.Manual,
            float durationSeconds = 0f,
            UnityEngine.Object owner = null)
        {
            UINotificationHandle handle =
                root.AdmitNotification(
                    new UINotificationRequest(
                        channelId,
                        presentation,
                        lifetimeMode: lifetimeMode,
                        durationSeconds: durationSeconds,
                        owner: owner));

            Assert.That(handle.Accepted, Is.True);
            return handle;
        }

        private static void AssertSnapshot(
            EchoUIRoot root,
            string channelId,
            int visible,
            int pending)
        {
            Assert.That(
                root.TryGetNotificationChannelSnapshot(
                    channelId,
                    out UINotificationChannelSnapshot snapshot),
                Is.True);

            Assert.That(snapshot.VisibleCount, Is.EqualTo(visible));
            Assert.That(snapshot.PendingCount, Is.EqualTo(pending));
        }

        private void InvokeLateUpdate()
        {
            Assert.That(LateUpdateMethod, Is.Not.Null);
            LateUpdateMethod.Invoke(root, null);
        }

        private void ReplaceNotificationService(
            UINotificationService replacement)
        {
            Assert.That(NotificationServiceField, Is.Not.Null);

            UINotificationService current =
                (UINotificationService)
                NotificationServiceField.GetValue(root);

            current?.Shutdown();

            NotificationServiceField.SetValue(
                root,
                replacement);
        }

        private static void ClaimAuthorityForTest(
            EchoUIRoot value)
        {
            Assert.That(TryClaimAuthorityMethod, Is.Not.Null);
            TryClaimAuthorityMethod.Invoke(value, null);
        }

        private static void SetActiveRootForTest(
            EchoUIRoot value)
        {
            Assert.That(ActiveRootField, Is.Not.Null);

            ActiveRootField.SetValue(
                null,
                value);
        }
    }
}
