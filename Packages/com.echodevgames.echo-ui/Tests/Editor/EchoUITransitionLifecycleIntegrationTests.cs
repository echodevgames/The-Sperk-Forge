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
    public sealed class EchoUITransitionLifecycleIntegrationTests
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

            public string DriverId => "test-manual-transition";

            public bool SupportsCancellation => true;

            public int PendingCount =>
                pending.Count;

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
                    "No manual transition is waiting for completion.");

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
        private ManualTransitionDriver driver;
        private UITransitionProfile manualProfile;
        private UISurface main;
        private UISurface settings;
        private UISurface credits;
        private UISurface window;
        private UISurface windowClose;

        [SetUp]
        public void SetUp()
        {
            previousActiveRoot =
                EchoUIRoot.Active;

            SetActiveRootForTest(
                null);

            rootObject =
                new GameObject(
                    "Canvas_M3_02_TransitionIntegration");

            root =
                rootObject.AddComponent<EchoUIRoot>();

            ClaimAuthorityForTest(
                root);

            UILayerHost layer =
                CreateLayer(
                    "screens",
                    0);

            main =
                CreateSurface(
                    rootObject.transform,
                    "Panel_Main",
                    "main",
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

            credits =
                CreateSurface(
                    rootObject.transform,
                    "Panel_Credits",
                    "credits",
                    UISurfaceRole.Screen,
                    "frontend",
                    false);

            window =
                CreateSurface(
                    rootObject.transform,
                    "Panel_Window",
                    "window",
                    UISurfaceRole.Window,
                    string.Empty,
                    false);

            windowClose =
                CreateSurface(
                    rootObject.transform,
                    "Panel_Window_Close",
                    "window-close",
                    UISurfaceRole.Window,
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
                    "manual-profile",
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
                        Definition(
                            "main",
                            main),
                        Definition(
                            "settings",
                            settings),
                        Definition(
                            "credits",
                            credits)
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

            SetActiveRootForTest(
                previousActiveRoot);
        }

        [UnityTest]
        public IEnumerator PushWaitsForEnterAndExitBeforeHandleCompletes()
        {
            UIScreenHandle handle =
                root.PushScreen(
                    "settings");

            Assert.That(
                handle.Accepted,
                Is.True);

            Assert.That(
                handle.IsCompleted,
                Is.False);

            yield return WaitForPendingCount(1);

            driver.CompleteNext();

            yield return WaitForPendingCount(1);

            Assert.That(
                handle.IsCompleted,
                Is.False);

            driver.CompleteNext();

            yield return WaitForHandle(handle);

            Assert.That(
                handle.Result.Status,
                Is.EqualTo(
                    UIScreenOperationStatus.Succeeded));

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("settings"));
        }

        [UnityTest]
        public IEnumerator PushEnterFailureRestoresPriorStableScreen()
        {
            UIScreenHandle handle =
                root.PushScreen(
                    "settings");

            yield return WaitForPendingCount(1);

            driver.CompleteNext(
                UITransitionStatus.Failed,
                "injected-enter-failure");

            yield return WaitForHandle(handle);

            Assert.That(
                handle.Result.Status,
                Is.EqualTo(
                    UIScreenOperationStatus.Failed));

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("main"));

            Assert.That(
                root.GetScreenHistoryDepth("frontend"),
                Is.EqualTo(1));

            Assert.That(
                main.IsVisible,
                Is.True);

            Assert.That(
                settings.IsVisible,
                Is.False);
        }

        [UnityTest]
        public IEnumerator PushExitFailureStillCommitsDeterministicTarget()
        {
            UIScreenHandle handle =
                root.PushScreen(
                    "settings");

            yield return WaitForPendingCount(1);
            driver.CompleteNext();

            yield return WaitForPendingCount(1);
            driver.CompleteNext(
                UITransitionStatus.Failed,
                "injected-exit-failure");

            yield return WaitForHandle(handle);

            Assert.That(
                handle.Result.Status,
                Is.EqualTo(
                    UIScreenOperationStatus.Succeeded));

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("settings"));

            Assert.That(
                handle.Result.Message,
                Does.Contain("deterministic"));
        }

        [UnityTest]
        public IEnumerator ReplaceEnterFailurePreservesCurrentAndHistory()
        {
            UIScreenHandle push =
                root.PushScreen(
                    "settings");

            yield return CompleteSuccessfulTwoPhaseOperation(
                push);

            Assert.That(
                root.GetScreenHistoryDepth("frontend"),
                Is.EqualTo(2));

            UIScreenHandle replace =
                root.ReplaceScreen(
                    "credits");

            yield return WaitForPendingCount(1);

            driver.CompleteNext(
                UITransitionStatus.Failed,
                "replace-enter-failure");

            yield return WaitForHandle(replace);

            Assert.That(
                replace.Result.Status,
                Is.EqualTo(
                    UIScreenOperationStatus.Failed));

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("settings"));

            Assert.That(
                root.GetScreenHistoryDepth("frontend"),
                Is.EqualTo(2));

            Assert.That(
                settings.IsVisible,
                Is.True);
        }

        [UnityTest]
        public IEnumerator ResetUsesExistingHistoryEntryAndSettlesTransitions()
        {
            UIScreenHandle pushSettings =
                root.PushScreen(
                    "settings");

            yield return CompleteSuccessfulTwoPhaseOperation(
                pushSettings);

            UIScreenHandle pushCredits =
                root.PushScreen(
                    "credits");

            yield return CompleteSuccessfulTwoPhaseOperation(
                pushCredits);

            Assert.That(
                root.GetScreenHistoryDepth("frontend"),
                Is.EqualTo(3));

            UIScreenHandle reset =
                root.ResetScreen(
                    "main");

            yield return WaitForPendingCount(1);
            driver.CompleteNext();

            yield return WaitForPendingCount(1);
            driver.CompleteNext();

            yield return WaitForHandle(reset);

            Assert.That(
                reset.Result.Status,
                Is.EqualTo(
                    UIScreenOperationStatus.Succeeded));

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("main"));

            Assert.That(
                root.GetScreenHistoryDepth("frontend"),
                Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator BackEnterFailureLeavesCurrentScreenAuthoritative()
        {
            UIScreenHandle push =
                root.PushScreen(
                    "settings");

            yield return CompleteSuccessfulTwoPhaseOperation(
                push);

            UIScreenHandle back =
                root.BackScreen(
                    "frontend");

            yield return WaitForPendingCount(1);

            driver.CompleteNext(
                UITransitionStatus.Failed,
                "back-enter-failure");

            yield return WaitForHandle(back);

            Assert.That(
                back.Result.Status,
                Is.EqualTo(
                    UIScreenOperationStatus.Failed));

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("settings"));

            Assert.That(
                root.GetScreenHistoryDepth("frontend"),
                Is.EqualTo(2));
        }

        [UnityTest]
        public IEnumerator CloseRestoreFailureLeavesCurrentScreenOpen()
        {
            UIScreenHandle push =
                root.PushScreen(
                    "settings");

            yield return CompleteSuccessfulTwoPhaseOperation(
                push);

            UIScreenHandle close =
                root.CloseScreen(
                    "settings",
                    "frontend");

            yield return WaitForPendingCount(1);

            driver.CompleteNext(
                UITransitionStatus.Failed,
                "close-restore-failure");

            yield return WaitForHandle(close);

            Assert.That(
                close.Result.Status,
                Is.EqualTo(
                    UIScreenOperationStatus.Failed));

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("settings"));

            Assert.That(
                settings.IsVisible,
                Is.True);
        }

        [UnityTest]
        public IEnumerator ScreenQueueRemainsStrictFifoWhileTransitionIsPending()
        {
            UIScreenHandle first =
                root.PushScreen(
                    "settings");

            UIScreenHandle second =
                root.ReplaceScreen(
                    "credits");

            Assert.That(
                first.IsCompleted,
                Is.False);

            Assert.That(
                second.IsCompleted,
                Is.False);

            yield return WaitForPendingCount(1);

            driver.CompleteNext();
            yield return WaitForPendingCount(1);
            driver.CompleteNext();

            yield return WaitForHandle(first);

            Assert.That(
                second.IsCompleted,
                Is.False);

            yield return WaitForPendingCount(1);
            driver.CompleteNext();
            yield return WaitForPendingCount(1);
            driver.CompleteNext();

            yield return WaitForHandle(second);

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("credits"));
        }

        [UnityTest]
        public IEnumerator IndependentWindowEnterFailureRollsVisibilityBack()
        {
            UISurfaceOperationResult result =
                default;

            bool completed =
                false;

            ObserveSurfaceOperation(
                root.OpenSurfaceAsync(
                    "window",
                    manualProfile),
                value =>
                {
                    result = value;
                    completed = true;
                });

            yield return WaitForPendingCount(1);

            driver.CompleteNext(
                UITransitionStatus.Failed,
                "window-enter-failure");

            yield return WaitForCondition(
                () => completed);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    UISurfaceOperationStatus.TransitionFailed));

            Assert.That(
                window.IsVisible,
                Is.False);
        }

        [UnityTest]
        public IEnumerator IndependentWindowExitFailureStillForcesClosedState()
        {
            UISurfaceOperationResult openResult =
                root.OpenSurface(
                    "window-close");

            Assert.That(
                openResult.Succeeded,
                Is.True);

            UISurfaceOperationResult closeResult =
                default;

            bool completed =
                false;

            ObserveSurfaceOperation(
                root.CloseSurfaceAsync(
                    "window-close",
                    manualProfile),
                value =>
                {
                    closeResult = value;
                    completed = true;
                });

            yield return WaitForPendingCount(1);

            driver.CompleteNext(
                UITransitionStatus.Failed,
                "window-exit-failure");

            yield return WaitForCondition(
                () => completed);

            Assert.That(
                closeResult.Status,
                Is.EqualTo(
                    UISurfaceOperationStatus.TransitionFailed));

            Assert.That(
                windowClose.IsVisible,
                Is.False);
        }

        private UIScreenDefinition Definition(
            string id,
            UISurface view) =>
            new UIScreenDefinition(
                id,
                "frontend",
                "screens",
                UIScreenOwnershipMode.SceneOwned,
                UIScreenSuspensionVisibility.Hidden,
                sceneOwnedView: view,
                transitionProfile:
                    manualProfile);

        private IEnumerator CompleteSuccessfulTwoPhaseOperation(
            UIScreenHandle handle)
        {
            yield return WaitForPendingCount(1);
            driver.CompleteNext();

            yield return WaitForPendingCount(1);
            driver.CompleteNext();

            yield return WaitForHandle(
                handle);
        }

        private IEnumerator WaitForPendingCount(
            int count)
        {
            yield return WaitForCondition(
                () => driver.PendingCount >= count);
        }

        private static IEnumerator WaitForHandle(
            UIScreenHandle handle)
        {
            yield return WaitForCondition(
                () => handle != null &&
                    handle.IsCompleted);
        }

        private static IEnumerator WaitForCondition(
            Func<bool> condition)
        {
            const int frameLimit = 120;

            for (int frame = 0;
                 frame < frameLimit;
                 frame++)
            {
                if (condition())
                {
                    yield break;
                }

                yield return null;
            }

            Assert.Fail(
                "Timed out waiting for transition integration condition.");
        }

        private static async void ObserveSurfaceOperation(
            Awaitable<UISurfaceOperationResult> awaitable,
            Action<UISurfaceOperationResult> completed)
        {
            UISurfaceOperationResult result =
                await awaitable;

            completed?.Invoke(
                result);
        }

        private UILayerHost CreateLayer(
            string id,
            int order)
        {
            GameObject child =
                new GameObject(
                    "Layer_" + id);

            child.transform.SetParent(
                rootObject.transform,
                false);

            UILayerHost host =
                child.AddComponent<UILayerHost>();

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

            UISurface surface =
                child.AddComponent<UISurface>();

            SerializedObject serialized =
                new SerializedObject(
                    surface);

            serialized.FindProperty(
                    "surfaceId")
                .stringValue = surfaceId;

            serialized.FindProperty(
                    "displayLabel")
                .stringValue = objectName;

            serialized.FindProperty(
                    "role")
                .enumValueIndex =
                    (int)role;

            serialized.FindProperty(
                    "navigationScopeId")
                .stringValue = scopeId;

            serialized.FindProperty(
                    "startVisible")
                .boolValue = startVisible;

            serialized.ApplyModifiedPropertiesWithoutUndo();

            return surface;
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
        }

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
    }
}
