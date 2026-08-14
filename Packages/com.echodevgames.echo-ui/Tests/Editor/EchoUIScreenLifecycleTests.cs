using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUIScreenLifecycleTests
    {
        private GameObject rootObject;
        private EchoUIRoot root;
        private EchoUIRoot previousActiveRoot;
        private readonly List<GameObject> externalObjects =
            new List<GameObject>();

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

        [SetUp]
        public void SetUp()
        {
            previousActiveRoot =
                EchoUIRoot.Active;

            SetActiveRootForTest(null);

            rootObject =
                new GameObject(
                    "Canvas_MasterCanvas");

            root =
                rootObject.AddComponent<EchoUIRoot>();

            ClaimAuthorityForTest(root);
        }

        [TearDown]
        public void TearDown()
        {
            if (rootObject != null)
            {
                Object.DestroyImmediate(
                    rootObject);
            }

            for (int index = 0;
                 index < externalObjects.Count;
                 index++)
            {
                if (externalObjects[index] != null)
                {
                    Object.DestroyImmediate(
                        externalObjects[index]);
                }
            }

            externalObjects.Clear();

            SetActiveRootForTest(
                previousActiveRoot);
        }

        [Test]
        public void MissingLayerFailsWithoutScreenMutation()
        {
            UILayerHost layer =
                CreateLayer(
                    "actual-layer",
                    0);

            UISurface main =
                CreateSurface(
                    rootObject.transform,
                    "Panel_MainMenu",
                    "main-menu",
                    "frontend",
                    true,
                    addCanvasGroup: true);

            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            UIScreenDefinition definition =
                SceneDefinition(
                    "main-menu",
                    "frontend",
                    "missing-layer",
                    main,
                    UIScreenSuspensionVisibility.Hidden);

            UISurfaceOperationResult result =
                root.InitializeScreenLifecycle(
                    new[]
                    {
                        layer
                    },
                    new[]
                    {
                        definition
                    });

            Assert.That(
                result.Status,
                Is.EqualTo(
                    UISurfaceOperationStatus.InvalidDefinition));

            Assert.That(
                root.IsScreenLifecycleInitialized,
                Is.False);

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("main-menu"));

            Assert.That(
                main.IsVisible,
                Is.True);
        }

        [Test]
        public void SceneOwnedScreenNeverDestroysSceneView()
        {
            UILayerHost layer =
                CreateLayer(
                    "screens",
                    0);

            UISurface main =
                CreateSurface(
                    rootObject.transform,
                    "Panel_MainMenu",
                    "main-menu",
                    "frontend",
                    true,
                    addCanvasGroup: true);

            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            Assert.That(
                root.InitializeScreenLifecycle(
                    new[]
                    {
                        layer
                    },
                    new[]
                    {
                        SceneDefinition(
                            "main-menu",
                            "frontend",
                            "screens",
                            main,
                            UIScreenSuspensionVisibility.Hidden)
                    }).Succeeded,
                Is.True);

            UIScreenHandle close =
                root.CloseScreen(
                    "main-menu",
                    "frontend");

            AssertTerminalSuccess(close);

            Assert.That(
                main,
                Is.Not.Null);

            Assert.That(
                main.gameObject,
                Is.Not.Null);

            Assert.That(
                main.IsVisible,
                Is.False);
        }

        [Test]
        public void ExternalOwnedScreenNeverDestroysExternalView()
        {
            UILayerHost layer =
                CreateLayer(
                    "external-layer",
                    7);

            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            UIScreenDefinition definition =
                new UIScreenDefinition(
                    "external-screen",
                    "frontend",
                    "external-layer",
                    UIScreenOwnershipMode.ExternalOwned,
                    UIScreenSuspensionVisibility.Hidden);

            Assert.That(
                root.InitializeScreenLifecycle(
                    new[]
                    {
                        layer
                    },
                    new[]
                    {
                        definition
                    }).Succeeded,
                Is.True);

            UISurface external =
                CreateExternalSurface(
                    "External_View",
                    "external-screen",
                    "frontend");

            Assert.That(
                root.RegisterExternalScreenView(
                    "external-screen",
                    external).Succeeded,
                Is.True);

            AssertTerminalSuccess(
                root.PushScreen(
                    "external-screen"));

            AssertTerminalSuccess(
                root.CloseScreen(
                    "external-screen",
                    "frontend"));

            Assert.That(
                external,
                Is.Not.Null);

            Assert.That(
                external.gameObject,
                Is.Not.Null);
        }

        [Test]
        public void RootOwnedScreenCreatesAndReleasesOwnedView()
        {
            UILayerHost layer =
                CreateLayer(
                    "spawned",
                    10);

            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            GameObject prefab =
                new GameObject(
                    "PF_RootOwnedScreen");

            externalObjects.Add(
                prefab);

            UISurface prefabSurface =
                ConfigureSurface(
                    prefab.AddComponent<UISurface>(),
                    "root-owned",
                    UISurfaceRole.Screen,
                    "frontend",
                    false);

            prefab.AddComponent<CanvasGroup>();

            UIScreenDefinition definition =
                new UIScreenDefinition(
                    "root-owned",
                    "frontend",
                    "spawned",
                    UIScreenOwnershipMode.RootOwned,
                    UIScreenSuspensionVisibility.Hidden,
                    rootOwnedPrefab: prefab);

            Assert.That(
                root.InitializeScreenLifecycle(
                    new[]
                    {
                        layer
                    },
                    new[]
                    {
                        definition
                    }).Succeeded,
                Is.True);

            int before =
                root.RegisteredSurfaceCount;

            AssertTerminalSuccess(
                root.PushScreen(
                    "root-owned"));

            Assert.That(
                root.RegisteredSurfaceCount,
                Is.EqualTo(before + 1));

            Assert.That(
                layer.ContentRoot.childCount,
                Is.EqualTo(1));

            AssertTerminalSuccess(
                root.CloseScreen(
                    "root-owned",
                    "frontend"));

            Assert.That(
                root.RegisteredSurfaceCount,
                Is.EqualTo(before));

            Assert.That(
                layer.ContentRoot.childCount,
                Is.EqualTo(0));

            Assert.That(
                prefabSurface,
                Is.Not.Null);
        }

        [Test]
        public void PushSuspendsPriorScreenAndAddsHistory()
        {
            LifecycleFixture fixture =
                CreateTwoScreenLifecycle(
                    UIScreenSuspensionVisibility.Visible);

            Assert.That(
                root.GetScreenHistoryDepth("frontend"),
                Is.EqualTo(1));

            AssertTerminalSuccess(
                root.PushScreen(
                    "settings"));

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("settings"));

            Assert.That(
                root.GetScreenHistoryDepth("frontend"),
                Is.EqualTo(2));

            Assert.That(
                fixture.Main.IsVisible,
                Is.True);

            Assert.That(
                fixture.Main.IsInteractable,
                Is.False);

            Assert.That(
                fixture.Settings.IsVisible,
                Is.True);
        }

        [Test]
        public void SuspendedVisibilityMayHideRemainVisibleOrPreserveVisibility()
        {
            LifecycleFixture visibleFixture =
                CreateTwoScreenLifecycle(
                    UIScreenSuspensionVisibility.Visible);

            AssertTerminalSuccess(
                root.PushScreen(
                    "settings"));

            Assert.That(
                visibleFixture.Main.IsVisible,
                Is.True);

            RebuildRoot();

            LifecycleFixture hiddenFixture =
                CreateTwoScreenLifecycle(
                    UIScreenSuspensionVisibility.Hidden);

            AssertTerminalSuccess(
                root.PushScreen(
                    "settings"));

            Assert.That(
                hiddenFixture.Main.IsVisible,
                Is.False);

            RebuildRoot();

            LifecycleFixture preserveFixture =
                CreateTwoScreenLifecycle(
                    UIScreenSuspensionVisibility.Preserve);

            AssertTerminalSuccess(
                root.PushScreen(
                    "settings"));

            Assert.That(
                preserveFixture.Main.IsVisible,
                Is.True);
        }

        [Test]
        public void SuspendedScreenIsNonInteractableRegardlessOfVisibilityPolicy()
        {
            LifecycleFixture fixture =
                CreateTwoScreenLifecycle(
                    UIScreenSuspensionVisibility.Visible);

            Assert.That(
                fixture.Main.IsInteractable,
                Is.True);

            AssertTerminalSuccess(
                root.PushScreen(
                    "settings"));

            Assert.That(
                fixture.Main.IsVisible,
                Is.True);

            Assert.That(
                fixture.Main.IsInteractable,
                Is.False);

            AssertTerminalSuccess(
                root.BackScreen(
                    "frontend"));

            Assert.That(
                fixture.Main.IsInteractable,
                Is.True);
        }

        [Test]
        public void ReplaceChangesTopWithoutGrowingHistory()
        {
            CreateThreeScreenLifecycle();

            AssertTerminalSuccess(
                root.PushScreen(
                    "settings"));

            Assert.That(
                root.GetScreenHistoryDepth("frontend"),
                Is.EqualTo(2));

            AssertTerminalSuccess(
                root.ReplaceScreen(
                    "credits"));

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("credits"));

            Assert.That(
                root.GetScreenHistoryDepth("frontend"),
                Is.EqualTo(2));
        }

        [Test]
        public void ResetClearsHistoryAndEstablishesOneRootScreen()
        {
            CreateThreeScreenLifecycle();

            AssertTerminalSuccess(
                root.PushScreen(
                    "settings"));

            AssertTerminalSuccess(
                root.PushScreen(
                    "credits"));

            Assert.That(
                root.GetScreenHistoryDepth("frontend"),
                Is.EqualTo(3));

            AssertTerminalSuccess(
                root.ResetScreen(
                    "main-menu"));

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("main-menu"));

            Assert.That(
                root.GetScreenHistoryDepth("frontend"),
                Is.EqualTo(1));
        }

        [Test]
        public void BackRestoresPreviousValidScreenAndPrunesLostEntry()
        {
            UILayerHost layer =
                CreateLayer(
                    "screens",
                    0);

            UISurface main =
                CreateSurface(
                    rootObject.transform,
                    "Panel_MainMenu",
                    "main-menu",
                    "frontend",
                    true,
                    addCanvasGroup: true);

            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            UIScreenDefinition externalDefinition =
                new UIScreenDefinition(
                    "external",
                    "frontend",
                    "screens",
                    UIScreenOwnershipMode.ExternalOwned,
                    UIScreenSuspensionVisibility.Hidden);

            Assert.That(
                root.InitializeScreenLifecycle(
                    new[]
                    {
                        layer
                    },
                    new[]
                    {
                        SceneDefinition(
                            "main-menu",
                            "frontend",
                            "screens",
                            main,
                            UIScreenSuspensionVisibility.Hidden),
                        externalDefinition
                    }).Succeeded,
                Is.True);

            UISurface external =
                CreateExternalSurface(
                    "External",
                    "external",
                    "frontend");

            Assert.That(
                root.RegisterExternalScreenView(
                    "external",
                    external).Succeeded,
                Is.True);

            AssertTerminalSuccess(
                root.PushScreen(
                    "external"));

            Object.DestroyImmediate(
                external.gameObject);

            UIScreenHandle back =
                root.BackScreen(
                    "frontend");

            AssertTerminalSuccess(
                back);

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("main-menu"));

            Assert.That(
                root.GetScreenHistoryDepth("frontend"),
                Is.EqualTo(1));

            Assert.That(
                main.IsVisible,
                Is.True);
        }

        [Test]
        public void CloseCurrentScreenRestoresPreviousWithoutCorruptingHistory()
        {
            LifecycleFixture fixture =
                CreateTwoScreenLifecycle(
                    UIScreenSuspensionVisibility.Hidden);

            AssertTerminalSuccess(
                root.PushScreen(
                    "settings"));

            AssertTerminalSuccess(
                root.CloseScreen(
                    "settings",
                    "frontend"));

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("main-menu"));

            Assert.That(
                root.GetScreenHistoryDepth("frontend"),
                Is.EqualTo(1));

            Assert.That(
                fixture.Main.IsVisible,
                Is.True);
        }

        [Test]
        public void CloseHonorsCurrentEntryPolicyWithoutCorruptingHistory()
        {
            UILayerHost layer =
                CreateLayer(
                    "screens",
                    0);

            UISurface main =
                CreateSurface(
                    rootObject.transform,
                    "Panel_MainMenu",
                    "main-menu",
                    "frontend",
                    true,
                    addCanvasGroup: true);

            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            UIScreenDefinition definition =
                new UIScreenDefinition(
                    "main-menu",
                    "frontend",
                    "screens",
                    UIScreenOwnershipMode.SceneOwned,
                    UIScreenSuspensionVisibility.Hidden,
                    sceneOwnedView: main,
                    allowClose: false);

            Assert.That(
                root.InitializeScreenLifecycle(
                    new[]
                    {
                        layer
                    },
                    new[]
                    {
                        definition
                    }).Succeeded,
                Is.True);

            UIScreenHandle close =
                root.CloseScreen(
                    "main-menu",
                    "frontend");

            Assert.That(
                close.IsCompleted,
                Is.True);

            Assert.That(
                close.Result.Status,
                Is.EqualTo(
                    UIScreenOperationStatus.Rejected));

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("main-menu"));

            Assert.That(
                root.GetScreenHistoryDepth("frontend"),
                Is.EqualTo(1));

            Assert.That(
                main.IsVisible,
                Is.True);
        }

        [UnityTest]
        public IEnumerator RapidAcceptedOperationsExecuteInStrictSubmissionOrder()
        {
            UIScreenOperationQueue queue =
                new UIScreenOperationQueue(
                    4);

            Assert.That(
                queue.TryEnqueue(
                    UIScreenOperationRequest.Push(
                        "a"),
                    out UIScreenHandle first),
                Is.True);

            Assert.That(
                queue.TryEnqueue(
                    UIScreenOperationRequest.Replace(
                        "b"),
                    out UIScreenHandle second),
                Is.True);

            Assert.That(
                queue.TryEnqueue(
                    UIScreenOperationRequest.Back(
                        "frontend"),
                    out UIScreenHandle third),
                Is.True);

            List<UIScreenOperationKind> settled =
                new List<UIScreenOperationKind>();

            UIScreenOperationResult Execute(
                UIScreenOperationRequest request)
            {
                settled.Add(
                    request.Kind);

                return UIScreenOperationResult.Success(
                    request,
                    request.ScreenId,
                    request.ScopeId,
                    "settled");
            }

            Assert.That(
                queue.TryProcessNext(
                    Execute,
                    out UIScreenHandle firstSettled),
                Is.True);

            Assert.That(
                firstSettled,
                Is.SameAs(first));

            yield return null;

            Assert.That(
                queue.TryProcessNext(
                    Execute,
                    out UIScreenHandle secondSettled),
                Is.True);

            Assert.That(
                secondSettled,
                Is.SameAs(second));

            yield return null;

            Assert.That(
                queue.TryProcessNext(
                    Execute,
                    out UIScreenHandle thirdSettled),
                Is.True);

            Assert.That(
                thirdSettled,
                Is.SameAs(third));

            CollectionAssert.AreEqual(
                new[]
                {
                    UIScreenOperationKind.Push,
                    UIScreenOperationKind.Replace,
                    UIScreenOperationKind.Back
                },
                settled);
        }

        [Test]
        public void QueueCapacityRejectsOverflowWithoutTouchingAcceptedRequest()
        {
            UIScreenOperationQueue queue =
                new UIScreenOperationQueue(
                    1);

            Assert.That(
                queue.TryEnqueue(
                    UIScreenOperationRequest.Push(
                        "first"),
                    out UIScreenHandle accepted),
                Is.True);

            Assert.That(
                queue.TryEnqueue(
                    UIScreenOperationRequest.Push(
                        "second"),
                    out UIScreenHandle rejected),
                Is.False);

            Assert.That(
                accepted.Accepted,
                Is.True);

            Assert.That(
                accepted.IsCompleted,
                Is.False);

            Assert.That(
                rejected.Accepted,
                Is.False);

            Assert.That(
                rejected.IsCompleted,
                Is.True);

            Assert.That(
                rejected.Result.Status,
                Is.EqualTo(
                    UIScreenOperationStatus.Rejected));

            Assert.That(
                queue.Count,
                Is.EqualTo(1));
        }

        [Test]
        public void FactoryFailureLeavesHistoryAndCurrentEntryUnchanged()
        {
            UILayerHost layer =
                CreateLayer(
                    "screens",
                    0);

            UISurface main =
                CreateSurface(
                    rootObject.transform,
                    "Panel_MainMenu",
                    "main-menu",
                    "frontend",
                    true,
                    addCanvasGroup: true);

            GameObject prefab =
                new GameObject(
                    "PF_Failing");

            externalObjects.Add(
                prefab);

            ConfigureSurface(
                prefab.AddComponent<UISurface>(),
                "failing",
                UISurfaceRole.Screen,
                "frontend",
                false);

            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            Assert.That(
                root.InitializeScreenLifecycle(
                    new[]
                    {
                        layer
                    },
                    new[]
                    {
                        SceneDefinition(
                            "main-menu",
                            "frontend",
                            "screens",
                            main,
                            UIScreenSuspensionVisibility.Hidden),
                        new UIScreenDefinition(
                            "failing",
                            "frontend",
                            "screens",
                            UIScreenOwnershipMode.RootOwned,
                            UIScreenSuspensionVisibility.Hidden,
                            rootOwnedPrefab: prefab)
                    },
                    new AlwaysFailFactory()).Succeeded,
                Is.True);

            int depth =
                root.GetScreenHistoryDepth(
                    "frontend");

            UIScreenHandle result =
                root.PushScreen(
                    "failing");

            Assert.That(
                result.IsCompleted,
                Is.True);

            Assert.That(
                result.Result.Status,
                Is.EqualTo(
                    UIScreenOperationStatus.Failed));

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("main-menu"));

            Assert.That(
                root.GetScreenHistoryDepth("frontend"),
                Is.EqualTo(depth));

            Assert.That(
                main.IsVisible,
                Is.True);
        }

        [Test]
        public void RuntimeEntriesDoNotMutateScreenDefinitions()
        {
            UILayerHost layer =
                CreateLayer(
                    "screens",
                    0);

            UISurface main =
                CreateSurface(
                    rootObject.transform,
                    "Panel_MainMenu",
                    "main-menu",
                    "frontend",
                    true,
                    addCanvasGroup: true);

            UISurface settings =
                CreateSurface(
                    rootObject.transform,
                    "Panel_Settings",
                    "settings",
                    "frontend",
                    false,
                    addCanvasGroup: true);

            UIScreenDefinition mainDefinition =
                SceneDefinition(
                    "main-menu",
                    "frontend",
                    "screens",
                    main,
                    UIScreenSuspensionVisibility.Hidden);

            UIScreenDefinition settingsDefinition =
                SceneDefinition(
                    "settings",
                    "frontend",
                    "screens",
                    settings,
                    UIScreenSuspensionVisibility.Hidden);

            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            Assert.That(
                root.InitializeScreenLifecycle(
                    new[]
                    {
                        layer
                    },
                    new[]
                    {
                        mainDefinition,
                        settingsDefinition
                    }).Succeeded,
                Is.True);

            FieldInfo layerField =
                typeof(UIScreenDefinition).GetField(
                    "targetLayerId",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

            Assert.That(
                layerField,
                Is.Not.Null);

            layerField.SetValue(
                settingsDefinition,
                "changed-after-initialize");

            UIScreenHandle push =
                root.PushScreen(
                    "settings");

            AssertTerminalSuccess(
                push);

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("settings"));
        }

        [Test]
        public void M2ScreenLifecyclePreservesM1IndependentWindowBehavior()
        {
            UILayerHost layer =
                CreateLayer(
                    "screens",
                    0);

            UISurface main =
                CreateSurface(
                    rootObject.transform,
                    "Panel_MainMenu",
                    "main-menu",
                    "frontend",
                    true,
                    addCanvasGroup: true);

            UISurface settings =
                CreateSurface(
                    rootObject.transform,
                    "Panel_Settings",
                    "settings",
                    "frontend",
                    false,
                    addCanvasGroup: true);

            UISurface window =
                CreateSurface(
                    rootObject.transform,
                    "Panel_DefaultWindow",
                    "default-window",
                    string.Empty,
                    false,
                    addCanvasGroup: true,
                    role: UISurfaceRole.Window);

            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            Assert.That(
                root.InitializeScreenLifecycle(
                    new[]
                    {
                        layer
                    },
                    new[]
                    {
                        SceneDefinition(
                            "main-menu",
                            "frontend",
                            "screens",
                            main,
                            UIScreenSuspensionVisibility.Hidden),
                        SceneDefinition(
                            "settings",
                            "frontend",
                            "screens",
                            settings,
                            UIScreenSuspensionVisibility.Hidden)
                    }).Succeeded,
                Is.True);

            Assert.That(
                root.OpenSurface(
                    "default-window").Succeeded,
                Is.True);

            AssertTerminalSuccess(
                root.PushScreen(
                    "settings"));

            Assert.That(
                window.IsVisible,
                Is.True);

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("settings"));

            Assert.That(
                root.ToggleSurface(
                    "default-window").Succeeded,
                Is.True);

            Assert.That(
                window.IsVisible,
                Is.False);
        }

        [Test]
        public void RuntimeAssemblyHasNoPeerEchoPackageDependency()
        {
            string path =
                Path.Combine(
                    "Packages",
                    "com.echodevgames.echo-ui",
                    "Runtime",
                    "EchoDevGames.EchoUI.Runtime.asmdef");

            string text =
                File.ReadAllText(
                    path);

            StringAssert.DoesNotContain(
                "EchoDevGames.EchoLaunch",
                text);

            StringAssert.DoesNotContain(
                "EchoDevGames.EchoSave",
                text);
        }

        private LifecycleFixture CreateTwoScreenLifecycle(
            UIScreenSuspensionVisibility mainSuspension)
        {
            UILayerHost layer =
                CreateLayer(
                    "screens",
                    0);

            UISurface main =
                CreateSurface(
                    rootObject.transform,
                    "Panel_MainMenu",
                    "main-menu",
                    "frontend",
                    true,
                    addCanvasGroup: true);

            UISurface settings =
                CreateSurface(
                    rootObject.transform,
                    "Panel_Settings",
                    "settings",
                    "frontend",
                    false,
                    addCanvasGroup: true);

            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            Assert.That(
                root.InitializeScreenLifecycle(
                    new[]
                    {
                        layer
                    },
                    new[]
                    {
                        SceneDefinition(
                            "main-menu",
                            "frontend",
                            "screens",
                            main,
                            mainSuspension),
                        SceneDefinition(
                            "settings",
                            "frontend",
                            "screens",
                            settings,
                            UIScreenSuspensionVisibility.Hidden)
                    }).Succeeded,
                Is.True);

            return new LifecycleFixture(
                main,
                settings);
        }

        private void CreateThreeScreenLifecycle()
        {
            UILayerHost layer =
                CreateLayer(
                    "screens",
                    0);

            UISurface main =
                CreateSurface(
                    rootObject.transform,
                    "Panel_MainMenu",
                    "main-menu",
                    "frontend",
                    true,
                    addCanvasGroup: true);

            UISurface settings =
                CreateSurface(
                    rootObject.transform,
                    "Panel_Settings",
                    "settings",
                    "frontend",
                    false,
                    addCanvasGroup: true);

            UISurface credits =
                CreateSurface(
                    rootObject.transform,
                    "Panel_Credits",
                    "credits",
                    "frontend",
                    false,
                    addCanvasGroup: true);

            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            Assert.That(
                root.InitializeScreenLifecycle(
                    new[]
                    {
                        layer
                    },
                    new[]
                    {
                        SceneDefinition(
                            "main-menu",
                            "frontend",
                            "screens",
                            main,
                            UIScreenSuspensionVisibility.Hidden),
                        SceneDefinition(
                            "settings",
                            "frontend",
                            "screens",
                            settings,
                            UIScreenSuspensionVisibility.Hidden),
                        SceneDefinition(
                            "credits",
                            "frontend",
                            "screens",
                            credits,
                            UIScreenSuspensionVisibility.Hidden)
                    }).Succeeded,
                Is.True);
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
                new SerializedObject(host);

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
            string scopeId,
            bool startVisible,
            bool addCanvasGroup,
            UISurfaceRole role = UISurfaceRole.Screen)
        {
            GameObject child =
                new GameObject(
                    objectName);

            child.transform.SetParent(
                parent,
                false);

            if (addCanvasGroup)
            {
                child.AddComponent<CanvasGroup>();
            }

            UISurface surface =
                child.AddComponent<UISurface>();

            return ConfigureSurface(
                surface,
                surfaceId,
                role,
                scopeId,
                startVisible);
        }

        private UISurface CreateExternalSurface(
            string objectName,
            string surfaceId,
            string scopeId)
        {
            GameObject external =
                new GameObject(
                    objectName);

            externalObjects.Add(
                external);

            external.AddComponent<CanvasGroup>();

            return ConfigureSurface(
                external.AddComponent<UISurface>(),
                surfaceId,
                UISurfaceRole.Screen,
                scopeId,
                false);
        }

        private static UISurface ConfigureSurface(
            UISurface surface,
            string surfaceId,
            UISurfaceRole role,
            string scopeId,
            bool startVisible)
        {
            SerializedObject serialized =
                new SerializedObject(surface);

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

        private static UIScreenDefinition SceneDefinition(
            string screenId,
            string scopeId,
            string layerId,
            UISurface view,
            UIScreenSuspensionVisibility suspension) =>
            new UIScreenDefinition(
                screenId,
                scopeId,
                layerId,
                UIScreenOwnershipMode.SceneOwned,
                suspension,
                sceneOwnedView: view);

        private void RebuildRoot()
        {
            if (rootObject != null)
            {
                Object.DestroyImmediate(
                    rootObject);
            }

            SetActiveRootForTest(null);

            rootObject =
                new GameObject(
                    "Canvas_MasterCanvas_Rebuilt");

            root =
                rootObject.AddComponent<EchoUIRoot>();

            ClaimAuthorityForTest(
                root);
        }

        private static void AssertTerminalSuccess(
            UIScreenHandle handle)
        {
            Assert.That(
                handle,
                Is.Not.Null);

            Assert.That(
                handle.Accepted,
                Is.True);

            Assert.That(
                handle.IsCompleted,
                Is.True);

            Assert.That(
                handle.Result.Status,
                Is.EqualTo(
                    UIScreenOperationStatus.Succeeded),
                handle.Result.Message);
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

        private readonly struct LifecycleFixture
        {
            public LifecycleFixture(
                UISurface main,
                UISurface settings)
            {
                Main = main;
                Settings = settings;
            }

            public UISurface Main { get; }

            public UISurface Settings { get; }
        }

        private sealed class AlwaysFailFactory : IUIScreenFactory
        {
            public bool TryCreate(
                UIScreenDefinition definition,
                UILayerHost layerHost,
                out UISurface surface,
                out string error)
            {
                surface = null;
                error = "Injected factory failure.";
                return false;
            }

            public void Release(
                UISurface surface)
            {
            }
        }
    }
}
