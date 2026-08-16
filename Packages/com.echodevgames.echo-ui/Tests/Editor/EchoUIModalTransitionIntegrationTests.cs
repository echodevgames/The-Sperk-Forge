using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUIModalTransitionIntegrationTests
    {
        private sealed class ManualTransitionDriver : IUITransitionDriver
        {
            private sealed class Pending
            {
                public UITransitionRequest Request;
                public AwaitableCompletionSource<UITransitionResult> Completion;
            }

            private readonly Queue<Pending> pending =
                new Queue<Pending>();

            public string DriverId =>
                "test-modal-manual-transition";

            public bool SupportsCancellation =>
                true;

            public int PendingCount =>
                pending.Count;

            public UITransitionRequest PeekNextRequest =>
                pending.Count == 0
                    ? null
                    : pending.Peek().Request;

            public Awaitable<UITransitionResult> ExecuteAsync(
                UITransitionRequest request,
                CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                AwaitableCompletionSource<UITransitionResult> completion =
                    new AwaitableCompletionSource<UITransitionResult>();

                pending.Enqueue(
                    new Pending
                    {
                        Request = request,
                        Completion = completion
                    });

                return completion.Awaitable;
            }

            public void CompleteNext(
                UITransitionStatus status =
                    UITransitionStatus.Completed,
                string message = "manual")
            {
                Assert.That(
                    pending.Count,
                    Is.GreaterThan(0),
                    "No manual Modal transition is waiting for completion.");

                Pending item =
                    pending.Dequeue();

                item.Completion.SetResult(
                    UITransitionResult.ForRequest(
                        item.Request,
                        status,
                        message: message));
            }

            public void ForceFinalState(
                UITransitionRequest request)
            {
                if (request == null ||
                    request.Surface == null)
                {
                    return;
                }

                CanvasGroup group =
                    request.Surface.GetComponent<CanvasGroup>();

                if (group != null)
                {
                    group.alpha =
                        request.Direction ==
                            UITransitionDirection.Enter
                            ? 1f
                            : 0f;
                }
            }
        }

        private sealed class TrackingModalFactory : IUIModalFactory
        {
            public int CreateCount { get; private set; }

            public int ReleaseCount { get; private set; }

            public UISurface LastCreated { get; private set; }

            public bool TryCreate(
                UIModalDefinition definition,
                UILayerHost layerHost,
                out UISurface surface,
                out string error)
            {
                surface = null;
                error = string.Empty;

                if (definition == null ||
                    definition.RootOwnedPrefab == null ||
                    layerHost == null ||
                    layerHost.ContentRoot == null)
                {
                    error =
                        "Tracking Modal factory received incomplete creation input.";
                    return false;
                }

                GameObject instance =
                    UnityEngine.Object.Instantiate(
                        definition.RootOwnedPrefab,
                        layerHost.ContentRoot,
                        false);

                surface =
                    instance.GetComponent<UISurface>();

                if (surface == null)
                {
                    UnityEngine.Object.DestroyImmediate(
                        instance);

                    error =
                        "Tracking Modal prefab has no UISurface.";
                    return false;
                }

                CreateCount++;
                LastCreated = surface;
                return true;
            }

            public void Release(
                UISurface surface)
            {
                ReleaseCount++;

                if (surface != null)
                {
                    UnityEngine.Object.DestroyImmediate(
                        surface.gameObject);
                }
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

        private GameObject rootObject;
        private EchoUIRoot root;
        private EchoUIRoot previousActiveRoot;
        private readonly List<GameObject> externalObjects =
            new List<GameObject>();

        private UILayerHost layer;
        private UISurface main;
        private UISurface settings;
        private UISurface modal;
        private ManualTransitionDriver driver;
        private UITransitionProfile manualProfile;

        [SetUp]
        public void SetUp()
        {
            previousActiveRoot =
                EchoUIRoot.Active;

            SetActiveRootForTest(
                null);

            rootObject =
                new GameObject(
                    "Canvas_M3_02_ModalTransitions");

            root =
                rootObject.AddComponent<EchoUIRoot>();

            ClaimAuthorityForTest(
                root);

            layer =
                CreateLayer(
                    "primary-ui",
                    20);

            main =
                CreateSurface(
                    rootObject.transform,
                    "Panel_Main",
                    "main-menu",
                    UISurfaceRole.Screen,
                    "frontend",
                    true);

            settings =
                CreateSurface(
                    rootObject.transform,
                    "Panel_Settings",
                    "settings",
                    UISurfaceRole.Screen,
                    "frontend",
                    false);

            modal =
                CreateSurface(
                    rootObject.transform,
                    "Panel_Confirm",
                    "confirm",
                    UISurfaceRole.Modal,
                    string.Empty,
                    false);

            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            driver =
                new ManualTransitionDriver();

            Assert.That(
                root.RegisterTransitionDriver(
                    driver),
                Is.True);

            manualProfile =
                new UITransitionProfile(
                    "modal-manual-profile",
                    driver.DriverId,
                    driver.DriverId,
                    0f,
                    0f,
                    hardTimeoutSeconds: 2f);

            Assert.That(
                root.InitializeScreenLifecycle(
                    new[]
                    {
                        layer
                    },
                    new[]
                    {
                        SceneScreenDefinition(
                            "main-menu",
                            main),
                        SceneScreenDefinition(
                            "settings",
                            settings)
                    }).Succeeded,
                Is.True);
        }

        [TearDown]
        public void TearDown()
        {
            if (rootObject != null)
            {
                UnityEngine.Object.DestroyImmediate(
                    rootObject);
            }

            for (int index = 0;
                 index < externalObjects.Count;
                 index++)
            {
                if (externalObjects[index] != null)
                {
                    UnityEngine.Object.DestroyImmediate(
                        externalObjects[index]);
                }
            }

            externalObjects.Clear();

            SetActiveRootForTest(
                previousActiveRoot);
        }

        [UnityTest]
        public IEnumerator OpenBlocksLowerAndTopInteractionUntilEnterSettles()
        {
            InitializeSceneModal();

            CanvasGroup mainGroup =
                main.GetComponent<CanvasGroup>();

            CanvasGroup modalGroup =
                modal.GetComponent<CanvasGroup>();

            UIModalHandle handle =
                root.OpenModal(
                    "confirm");

            Assert.That(
                handle.Accepted,
                Is.True);

            Assert.That(
                root.ActiveModalCount,
                Is.EqualTo(1));

            Assert.That(
                root.TopModalId,
                Is.EqualTo("confirm"));

            Assert.That(
                modal.IsVisible,
                Is.True);

            Assert.That(
                main.IsInteractable,
                Is.False);

            Assert.That(
                mainGroup.blocksRaycasts,
                Is.False);

            Assert.That(
                modal.IsInteractable,
                Is.False);

            Assert.That(
                modalGroup.blocksRaycasts,
                Is.False);

            AssertPending(
                "confirm",
                UITransitionDirection.Enter);

            driver.CompleteNext();

            yield return WaitForCondition(
                () => modal.IsInteractable,
                "Modal did not become interactive after successful enter settlement.");

            Assert.That(
                main.IsInteractable,
                Is.False);

            Assert.That(
                modalGroup.blocksRaycasts,
                Is.True);

            SettleModalSuccessfully(
                handle);
        }

        [UnityTest]
        public IEnumerator SemanticCompletionBeforeEnterSettlementIsNotReady()
        {
            InitializeSceneModal();

            UIModalHandle handle =
                root.OpenModal(
                    "confirm");

            AssertPending(
                "confirm",
                UITransitionDirection.Enter);

            UIModalCompletionAttemptResult early =
                root.CompleteModal(
                    handle,
                    "confirm");

            Assert.That(
                early.Status,
                Is.EqualTo(
                    UIModalCompletionStatus.NotReady));

            Assert.That(
                handle.IsCompleted,
                Is.False);

            driver.CompleteNext();

            yield return WaitForCondition(
                () => modal.IsInteractable,
                "Modal enter did not settle after rejecting premature semantic completion.");

            UIModalCompletionAttemptResult accepted =
                root.CompleteModal(
                    handle,
                    "confirm");

            Assert.That(
                accepted.Succeeded,
                Is.True);

            AssertPending(
                "confirm",
                UITransitionDirection.Exit);

            driver.CompleteNext();

            yield return WaitForCondition(
                () => handle.IsCompleted,
                "Modal did not complete after valid post-enter semantic completion.");

            Assert.That(
                handle.Result.ResultId.Value,
                Is.EqualTo("confirm"));
        }

        [UnityTest]
        public IEnumerator EnterFailureAbortsStructurallyAndKeepsSceneOwnedView()
        {
            InitializeSceneModal();

            UIModalHandle handle =
                root.OpenModal(
                    "confirm");

            AssertPending(
                "confirm",
                UITransitionDirection.Enter);

            driver.CompleteNext(
                UITransitionStatus.Failed,
                "injected enter failure");

            yield return WaitForCondition(
                () => handle.IsCompleted,
                "Modal handle did not structurally abort after failed enter.");

            Assert.That(
                handle.Result.Outcome,
                Is.EqualTo(
                    UIModalOutcome.Aborted));

            Assert.That(
                handle.Result.AbortReason,
                Is.EqualTo(
                    UIModalAbortReason.TransitionFailed));

            Assert.That(
                handle.Result.ResultId.IsValid,
                Is.False);

            Assert.That(
                root.ActiveModalCount,
                Is.EqualTo(0));

            Assert.That(
                modal.IsVisible,
                Is.False);

            Assert.That(
                modal,
                Is.Not.Null);

            Assert.That(
                main.IsInteractable,
                Is.True);
        }

        [UnityTest]
        public IEnumerator RootOwnedEnterFailureReleasesPartialInstance()
        {
            GameObject prefab =
                CreateModalPrefab(
                    "root-confirm");

            TrackingModalFactory factory =
                new TrackingModalFactory();

            UIModalDefinition definition =
                new UIModalDefinition(
                    "root-confirm",
                    "primary-ui",
                    UIScreenOwnershipMode.RootOwned,
                    rootOwnedPrefab: prefab,
                    transitionProfile: manualProfile);

            Assert.That(
                root.InitializeModalLifecycle(
                    new[]
                    {
                        definition
                    },
                    factory).Succeeded,
                Is.True);

            int baselineSurfaceCount =
                root.RegisteredSurfaceCount;

            UIModalHandle handle =
                root.OpenModal(
                    "root-confirm");

            Assert.That(
                factory.CreateCount,
                Is.EqualTo(1));

            Assert.That(
                root.RegisteredSurfaceCount,
                Is.EqualTo(
                    baselineSurfaceCount + 1));

            AssertPending(
                "root-confirm",
                UITransitionDirection.Enter);

            driver.CompleteNext(
                UITransitionStatus.Failed,
                "root-owned enter failure");

            yield return WaitForCondition(
                () => handle.IsCompleted,
                "RootOwned Modal did not abort after enter failure.");

            Assert.That(
                handle.Result.Outcome,
                Is.EqualTo(
                    UIModalOutcome.Aborted));

            Assert.That(
                factory.ReleaseCount,
                Is.EqualTo(1));

            Assert.That(
                factory.LastCreated == null,
                Is.True);

            Assert.That(
                root.RegisteredSurfaceCount,
                Is.EqualTo(
                    baselineSurfaceCount));
        }

        [UnityTest]
        public IEnumerator FirstTerminalClaimWinsWhileExitIsPending()
        {
            InitializeSceneModal();

            UIModalHandle handle =
                root.OpenModal(
                    "confirm");

            CompleteEnter();

            yield return WaitForCondition(
                () => modal.IsInteractable,
                "Modal did not reach interactive state before exact-once test.");

            UIModalCompletionAttemptResult first =
                root.CompleteModal(
                    handle,
                    "confirm");

            Assert.That(
                first.Succeeded,
                Is.True);

            Assert.That(
                handle.IsCompleted,
                Is.False);

            AssertPending(
                "confirm",
                UITransitionDirection.Exit);

            UIModalCompletionAttemptResult second =
                root.CompleteModal(
                    handle,
                    "cancel");

            UIModalCompletionAttemptResult third =
                root.AbortModal(
                    handle);

            Assert.That(
                second.Status,
                Is.EqualTo(
                    UIModalCompletionStatus.AlreadyCompleted));

            Assert.That(
                third.Status,
                Is.EqualTo(
                    UIModalCompletionStatus.AlreadyCompleted));

            driver.CompleteNext();

            yield return WaitForCondition(
                () => handle.IsCompleted,
                "Claimed Modal result did not settle after exit completion.");

            Assert.That(
                handle.Result.Outcome,
                Is.EqualTo(
                    UIModalOutcome.Completed));

            Assert.That(
                handle.Result.ResultId.Value,
                Is.EqualTo("confirm"));

            Assert.That(
                root.ActiveModalCount,
                Is.EqualTo(0));
        }

        [UnityTest]
        public IEnumerator ExitFailurePreservesSemanticResultAndForceCloses()
        {
            InitializeSceneModal();

            UIModalHandle handle =
                root.OpenModal(
                    "confirm");

            CompleteEnter();

            yield return WaitForCondition(
                () => modal.IsInteractable,
                "Modal did not enter before exit-failure test.");

            Assert.That(
                root.CompleteModal(
                    handle,
                    "confirm").Succeeded,
                Is.True);

            AssertPending(
                "confirm",
                UITransitionDirection.Exit);

            driver.CompleteNext(
                UITransitionStatus.Failed,
                "injected exit failure");

            yield return WaitForCondition(
                () => handle.IsCompleted,
                "Modal semantic result did not settle after failed exit.");

            Assert.That(
                handle.Result.Outcome,
                Is.EqualTo(
                    UIModalOutcome.Completed));

            Assert.That(
                handle.Result.ResultId.Value,
                Is.EqualTo("confirm"));

            Assert.That(
                root.ActiveModalCount,
                Is.EqualTo(0));

            Assert.That(
                modal.IsVisible,
                Is.False);

            Assert.That(
                main.IsInteractable,
                Is.True);
        }

        [UnityTest]
        public IEnumerator FailedExitPreservesExactAbortResult()
        {
            InitializeSceneModal();

            UIModalHandle handle =
                root.OpenModal(
                    "confirm");

            CompleteEnter();

            yield return WaitForCondition(
                () => modal.IsInteractable,
                "Modal did not enter before abort exit-failure test.");

            Assert.That(
                root.AbortModal(
                    handle,
                    UIModalAbortReason.ExplicitAbort).Succeeded,
                Is.True);

            AssertPending(
                "confirm",
                UITransitionDirection.Exit);

            Assert.That(
                root.CompleteModal(
                    handle,
                    "confirm").Status,
                Is.EqualTo(
                    UIModalCompletionStatus.AlreadyCompleted));

            driver.CompleteNext(
                UITransitionStatus.TimedOut,
                "injected exit timeout");

            yield return WaitForCondition(
                () => handle.IsCompleted,
                "Modal abort result did not settle after failed exit.");

            Assert.That(
                handle.Result.Outcome,
                Is.EqualTo(
                    UIModalOutcome.Aborted));

            Assert.That(
                handle.Result.AbortReason,
                Is.EqualTo(
                    UIModalAbortReason.ExplicitAbort));

            Assert.That(
                handle.Result.ResultId.IsValid,
                Is.False);
        }

        [UnityTest]
        public IEnumerator ClosingModalKeepsAllInteractionBlockedUntilExitSettles()
        {
            InitializeSceneModal();

            CanvasGroup mainGroup =
                main.GetComponent<CanvasGroup>();

            CanvasGroup modalGroup =
                modal.GetComponent<CanvasGroup>();

            UIModalHandle handle =
                root.OpenModal(
                    "confirm");

            CompleteEnter();

            yield return WaitForCondition(
                () => modal.IsInteractable,
                "Modal did not enter before closing-interaction test.");

            Assert.That(
                root.CompleteModal(
                    handle,
                    "confirm").Succeeded,
                Is.True);

            Assert.That(
                handle.IsCompleted,
                Is.False);

            Assert.That(
                main.IsInteractable,
                Is.False);

            Assert.That(
                mainGroup.blocksRaycasts,
                Is.False);

            Assert.That(
                modal.IsInteractable,
                Is.False);

            Assert.That(
                modalGroup.blocksRaycasts,
                Is.False);

            driver.CompleteNext();

            yield return WaitForCondition(
                () => handle.IsCompleted,
                "Closing Modal did not settle.");

            Assert.That(
                main.IsInteractable,
                Is.True);

            Assert.That(
                mainGroup.blocksRaycasts,
                Is.True);
        }

        [UnityTest]
        public IEnumerator DeferredScreenMutationWaitsForModalExitSettlement()
        {
            InitializeSceneModal(
                UIModalScreenMutationPolicy.DeferUntilModalStackClears);

            UIModalHandle modalHandle =
                root.OpenModal(
                    "confirm");

            CompleteEnter();

            yield return WaitForCondition(
                () => modal.IsInteractable,
                "Modal did not enter before deferred Screen test.");

            UIScreenHandle screenHandle =
                root.PushScreen(
                    "settings");

            Assert.That(
                screenHandle.Accepted,
                Is.True);

            Assert.That(
                screenHandle.IsCompleted,
                Is.False);

            Assert.That(
                root.DeferredScreenOperationQueueDepth,
                Is.EqualTo(1));

            Assert.That(
                root.CompleteModal(
                    modalHandle,
                    "confirm").Succeeded,
                Is.True);

            Assert.That(
                screenHandle.IsCompleted,
                Is.False);

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("main-menu"));

            AssertPending(
                "confirm",
                UITransitionDirection.Exit);

            driver.CompleteNext();

            yield return WaitForCondition(
                () => modalHandle.IsCompleted &&
                    screenHandle.IsCompleted,
                "Deferred Screen operation did not drain after Modal exit settlement.");

            Assert.That(
                screenHandle.Result.Status,
                Is.EqualTo(
                    UIScreenOperationStatus.Succeeded));

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("settings"));
        }

        [UnityTest]
        public IEnumerator BackCompletionClaimsOnceAndWaitsForExit()
        {
            UIModalBackPolicy backPolicy =
                new UIModalBackPolicy(
                    UIModalBackBehavior.CompleteWithResultId,
                    "back-cancel");

            InitializeSceneModal(
                UIModalScreenMutationPolicy.Reject,
                backPolicy);

            UIModalHandle handle =
                root.OpenModal(
                    "confirm");

            CompleteEnter();

            yield return WaitForCondition(
                () => modal.IsInteractable,
                "Modal did not enter before Back completion test.");

            UIModalCompletionAttemptResult first =
                root.HandleModalBack();

            Assert.That(
                first.Succeeded,
                Is.True);

            Assert.That(
                handle.IsCompleted,
                Is.False);

            AssertPending(
                "confirm",
                UITransitionDirection.Exit);

            UIModalCompletionAttemptResult second =
                root.HandleModalBack();

            Assert.That(
                second.Status,
                Is.EqualTo(
                    UIModalCompletionStatus.AlreadyCompleted));

            driver.CompleteNext();

            yield return WaitForCondition(
                () => handle.IsCompleted,
                "Back-claimed Modal did not settle after exit.");

            Assert.That(
                handle.Result.Outcome,
                Is.EqualTo(
                    UIModalOutcome.Completed));

            Assert.That(
                handle.Result.ResultId.Value,
                Is.EqualTo("back-cancel"));
        }

        private void InitializeSceneModal(
            UIModalScreenMutationPolicy policy =
                UIModalScreenMutationPolicy.Reject,
            UIModalBackPolicy backPolicy = null)
        {
            UIModalDefinition definition =
                new UIModalDefinition(
                    "confirm",
                    "primary-ui",
                    UIScreenOwnershipMode.SceneOwned,
                    sceneOwnedView: modal,
                    backPolicy: backPolicy,
                    transitionProfile: manualProfile);

            UISurfaceOperationResult result =
                root.InitializeModalLifecycle(
                    new[]
                    {
                        definition
                    },
                    screenMutationPolicy: policy);

            Assert.That(
                result.Succeeded,
                Is.True,
                result.Message);
        }

        private void CompleteEnter()
        {
            AssertPending(
                "confirm",
                UITransitionDirection.Enter);

            driver.CompleteNext();
        }

        private void SettleModalSuccessfully(
            UIModalHandle handle)
        {
            Assert.That(
                root.CompleteModal(
                    handle,
                    "confirm").Succeeded,
                Is.True);

            AssertPending(
                handle.ModalId.Value,
                UITransitionDirection.Exit);

            driver.CompleteNext();

            Assert.That(
                handle.IsCompleted,
                Is.True);
        }

        private void AssertPending(
            string surfaceId,
            UITransitionDirection direction)
        {
            Assert.That(
                driver.PendingCount,
                Is.EqualTo(1),
                "Expected exactly one pending transition.");

            UITransitionRequest request =
                driver.PeekNextRequest;

            Assert.That(
                request,
                Is.Not.Null);

            Assert.That(
                request.SurfaceId,
                Is.EqualTo(surfaceId));

            Assert.That(
                request.Direction,
                Is.EqualTo(direction));
        }

        private static IEnumerator WaitForCondition(
            Func<bool> predicate,
            string failureMessage)
        {
            const int frameLimit = 120;

            for (int frame = 0;
                 frame < frameLimit;
                 frame++)
            {
                if (predicate())
                {
                    yield break;
                }

                yield return null;
            }

            Assert.Fail(
                failureMessage);
        }

        private UISurface CreateSurface(
            Transform parent,
            string objectName,
            string surfaceId,
            UISurfaceRole role,
            string scopeId,
            bool startVisible)
        {
            GameObject child =
                new GameObject(
                    objectName);

            child.transform.SetParent(
                parent,
                false);

            child.AddComponent<CanvasGroup>();

            return ConfigureSurface(
                child.AddComponent<UISurface>(),
                surfaceId,
                role,
                scopeId,
                startVisible);
        }

        private UILayerHost CreateLayer(
            string id,
            int order)
        {
            GameObject layerObject =
                new GameObject(
                    "Layer_" + id);

            layerObject.transform.SetParent(
                rootObject.transform,
                false);

            UILayerHost host =
                layerObject.AddComponent<UILayerHost>();

            SerializedObject serialized =
                new SerializedObject(
                    host);

            SerializedProperty definition =
                serialized.FindProperty(
                    "definition");

            definition.FindPropertyRelative(
                    "layerId")
                .stringValue = id;

            definition.FindPropertyRelative(
                    "displayLabel")
                .stringValue = id;

            definition.FindPropertyRelative(
                    "order")
                .intValue = order;

            serialized.ApplyModifiedPropertiesWithoutUndo();

            return host;
        }

        private GameObject CreateModalPrefab(
            string modalId)
        {
            GameObject prefab =
                new GameObject(
                    "PF_" + modalId);

            externalObjects.Add(
                prefab);

            prefab.AddComponent<CanvasGroup>();

            ConfigureSurface(
                prefab.AddComponent<UISurface>(),
                modalId,
                UISurfaceRole.Modal,
                string.Empty,
                false);

            return prefab;
        }

        private static UISurface ConfigureSurface(
            UISurface surface,
            string surfaceId,
            UISurfaceRole role,
            string scopeId,
            bool startVisible)
        {
            SerializedObject serialized =
                new SerializedObject(
                    surface);

            serialized.FindProperty(
                    "surfaceId")
                .stringValue = surfaceId;

            serialized.FindProperty(
                    "displayLabel")
                .stringValue = surface.gameObject.name;

            serialized.FindProperty(
                    "role")
                .enumValueIndex = (int)role;

            serialized.FindProperty(
                    "navigationScopeId")
                .stringValue = scopeId;

            serialized.FindProperty(
                    "startVisible")
                .boolValue = startVisible;

            serialized.ApplyModifiedPropertiesWithoutUndo();

            return surface;
        }

        private static UIScreenDefinition SceneScreenDefinition(
            string screenId,
            UISurface view) =>
            new UIScreenDefinition(
                screenId,
                "frontend",
                "primary-ui",
                UIScreenOwnershipMode.SceneOwned,
                UIScreenSuspensionVisibility.Hidden,
                sceneOwnedView: view);

        private static void SetActiveRootForTest(
            EchoUIRoot value)
        {
            Assert.That(
                ActiveRootField,
                Is.Not.Null);

            ActiveRootField.SetValue(
                null,
                value);
        }

        private static void ClaimAuthorityForTest(
            EchoUIRoot value)
        {
            Assert.That(
                TryClaimAuthorityMethod,
                Is.Not.Null);

            TryClaimAuthorityMethod.Invoke(
                value,
                null);

            Assert.That(
                value.IsAuthoritative,
                Is.True);
        }
    }
}
