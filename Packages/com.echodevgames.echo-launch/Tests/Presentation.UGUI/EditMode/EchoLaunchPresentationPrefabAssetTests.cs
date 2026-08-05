//----- EchoLaunchPresentationPrefabAssetTests.cs START -----

using System;
using System.Collections.Generic;
using EchoDevGames.EchoLaunch;
using EchoDevGames.EchoLaunch.Presentation.UGUI;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EchoDevGames.EchoLaunch.Tests
    .Presentation.UGUI.EditMode
{
    public sealed class
        EchoLaunchPresentationPrefabAssetTests
    {
        private const string StatusPrefabPath =
            "Packages/com.echodevgames.echo-launch/" +
            "Presentation.UGUI/Prefabs/" +
            "EchoLaunchStatusView.prefab";

        private const string RootPrefabPath =
            "Packages/com.echodevgames.echo-launch/" +
            "Presentation.UGUI/Prefabs/" +
            "EchoLaunchRoot.prefab";

        [Test]
        public void StatusPrefabExists()
        {
            Assert.That(
                LoadPrefab(StatusPrefabPath),
                Is.Not.Null);
        }

        [Test]
        public void RootPrefabExists()
        {
            Assert.That(
                LoadPrefab(RootPrefabPath),
                Is.Not.Null);
        }

        [Test]
        public void PrefabGuidsAreNonemptyAndDistinct()
        {
            string statusGuid =
                AssetDatabase.AssetPathToGUID(
                    StatusPrefabPath);

            string rootGuid =
                AssetDatabase.AssetPathToGUID(
                    RootPrefabPath);

            Assert.That(
                statusGuid,
                Is.Not.Empty);

            Assert.That(
                rootGuid,
                Is.Not.Empty);

            Assert.That(
                rootGuid,
                Is.Not.EqualTo(statusGuid));
        }

        [Test]
        public void StatusRootHasApprovedComponents()
        {
            GameObject root =
                LoadPrefab(StatusPrefabPath);

            Assert.That(
                root.GetComponent<RectTransform>(),
                Is.Not.Null);

            Assert.That(
                root.GetComponent<Canvas>(),
                Is.Not.Null);

            Assert.That(
                root.GetComponent<CanvasScaler>(),
                Is.Not.Null);

            Assert.That(
                root.GetComponent<CanvasGroup>(),
                Is.Not.Null);

            Assert.That(
                root.GetComponent<
                    EchoLaunchStatusView>(),
                Is.Not.Null);
        }

        [Test]
        public void CanvasUsesApprovedDefaults()
        {
            GameObject root =
                LoadPrefab(StatusPrefabPath);

            Canvas canvas =
                root.GetComponent<Canvas>();

            CanvasScaler scaler =
                root.GetComponent<CanvasScaler>();

            Assert.That(
                canvas.renderMode,
                Is.EqualTo(
                    RenderMode.ScreenSpaceOverlay));

            Assert.That(
                canvas.sortingOrder,
                Is.EqualTo(1000));

            Assert.That(
                canvas.pixelPerfect,
                Is.False);

            Assert.That(
                scaler.uiScaleMode,
                Is.EqualTo(
                    CanvasScaler.ScaleMode
                        .ScaleWithScreenSize));

            Assert.That(
                scaler.referenceResolution,
                Is.EqualTo(
                    new Vector2(
                        1920f,
                        1080f)));

            Assert.That(
                scaler.screenMatchMode,
                Is.EqualTo(
                    CanvasScaler.ScreenMatchMode
                        .MatchWidthOrHeight));

            Assert.That(
                scaler.matchWidthOrHeight,
                Is.EqualTo(0.5f)
                    .Within(0.0001f));

            Assert.That(
                scaler.referencePixelsPerUnit,
                Is.EqualTo(100f)
                    .Within(0.0001f));
        }

        [Test]
        public void CanvasGroupStartsHiddenAndNoninteractive()
        {
            CanvasGroup group =
                LoadPrefab(StatusPrefabPath)
                    .GetComponent<CanvasGroup>();

            Assert.That(
                group.alpha,
                Is.EqualTo(0f)
                    .Within(0.0001f));

            Assert.That(
                group.interactable,
                Is.False);

            Assert.That(
                group.blocksRaycasts,
                Is.False);

            Assert.That(
                group.ignoreParentGroups,
                Is.False);
        }

        [Test]
        public void StatusPrefabContainsRequiredHierarchy()
        {
            GameObject root =
                LoadPrefab(StatusPrefabPath);

            string[] names =
            {
                "Backdrop",
                "Splash Root",
                "Splash Image",
                "Splash Label",
                "Status Root",
                "State Text",
                "Message Text",
                "Step Text",
                "Determinate Progress Root",
                "Progress Slider",
                "Progress Text",
                "Indeterminate Progress Root",
                "Indeterminate Text",
                "Elapsed Text"
            };

            foreach (string name in names)
            {
                Assert.That(
                    FindDescendant(
                        root.transform,
                        name),
                    Is.Not.Null,
                    "Missing hierarchy role: " +
                    name);
            }
        }

        [Test]
        public void ViewSerializedReferencesAreAssigned()
        {
            EchoLaunchStatusView view =
                LoadPrefab(StatusPrefabPath)
                    .GetComponent<
                        EchoLaunchStatusView>();

            SerializedObject serialized =
                new SerializedObject(view);

            string[] properties =
            {
                "canvasGroup",
                "stateText",
                "messageText",
                "stepText",
                "progressText",
                "elapsedText",
                "determinateProgress",
                "determinateProgressRoot",
                "indeterminateProgressRoot",
                "splashRoot",
                "splashImage",
                "splashLabelText"
            };

            foreach (string propertyName in properties)
            {
                SerializedProperty property =
                    serialized.FindProperty(
                        propertyName);

                Assert.That(
                    property,
                    Is.Not.Null,
                    "Missing property: " +
                    propertyName);

                Assert.That(
                    property.objectReferenceValue,
                    Is.Not.Null,
                    "Unassigned property: " +
                    propertyName);
            }
        }

        [Test]
        public void SplashRootStartsInactive()
        {
            Transform splashRoot =
                FindDescendant(
                    LoadPrefab(StatusPrefabPath)
                        .transform,
                    "Splash Root");

            Assert.That(
                splashRoot.gameObject.activeSelf,
                Is.False);
        }

        [Test]
        public void ProgressRootsStartInactive()
        {
            GameObject root =
                LoadPrefab(StatusPrefabPath);

            Transform determinate =
                FindDescendant(
                    root.transform,
                    "Determinate Progress Root");

            Transform indeterminate =
                FindDescendant(
                    root.transform,
                    "Indeterminate Progress Root");

            Assert.That(
                determinate.gameObject.activeSelf,
                Is.False);

            Assert.That(
                indeterminate.gameObject.activeSelf,
                Is.False);
        }

        [Test]
        public void ProgressSliderIsNoninteractive()
        {
            Slider slider =
                FindDescendant(
                    LoadPrefab(StatusPrefabPath)
                        .transform,
                    "Progress Slider")
                .GetComponent<Slider>();

            Assert.That(
                slider,
                Is.Not.Null);

            Assert.That(
                slider.interactable,
                Is.False);

            Assert.That(
                slider.navigation.mode,
                Is.EqualTo(
                    Navigation.Mode.None));

            Assert.That(
                slider.handleRect,
                Is.Null);
        }

        [Test]
        public void AllGraphicsIgnoreRaycasts()
        {
            Graphic[] graphics =
                LoadPrefab(StatusPrefabPath)
                    .GetComponentsInChildren<
                        Graphic>(
                            true);

            Assert.That(
                graphics.Length,
                Is.GreaterThan(0));

            foreach (Graphic graphic in graphics)
            {
                Assert.That(
                    graphic.raycastTarget,
                    Is.False,
                    graphic.name +
                    " accepts raycasts.");
            }
        }

        [Test]
        public void StatusPrefabContainsNoInputAuthority()
        {
            GameObject root =
                LoadPrefab(StatusPrefabPath);

            Assert.That(
                root.GetComponentInChildren<
                    EventSystem>(
                        true),
                Is.Null);

            Assert.That(
                root.GetComponentInChildren<
                    BaseInputModule>(
                        true),
                Is.Null);

            Assert.That(
                root.GetComponentInChildren<
                    GraphicRaycaster>(
                        true),
                Is.Null);

            Assert.That(
                root.GetComponentInChildren<
                    Button>(
                        true),
                Is.Null);

            Assert.That(
                root.GetComponentInChildren<
                    Toggle>(
                        true),
                Is.Null);
        }

        [Test]
        public void StatusPrefabContainsNoTextMeshProComponents()
        {
            Component[] components =
                LoadPrefab(StatusPrefabPath)
                    .GetComponentsInChildren<
                        Component>(
                            true);

            foreach (Component component in components)
            {
                if (component == null)
                {
                    continue;
                }

                string fullName =
                    component.GetType().FullName ??
                    string.Empty;

                Assert.That(
                    fullName.StartsWith(
                        "TMPro.",
                        StringComparison.Ordinal),
                    Is.False,
                    "Unexpected TMPro component: " +
                    fullName);
            }
        }

        [Test]
        public void EveryTextHasANonprojectFont()
        {
            Text[] texts =
                LoadPrefab(StatusPrefabPath)
                    .GetComponentsInChildren<Text>(
                        true);

            Assert.That(
                texts.Length,
                Is.GreaterThanOrEqualTo(7));

            foreach (Text text in texts)
            {
                Assert.That(
                    text.font,
                    Is.Not.Null,
                    text.name +
                    " has no font.");

                string fontPath =
                    AssetDatabase.GetAssetPath(
                        text.font);

                Assert.That(
                    fontPath.StartsWith(
                        "Assets/",
                        StringComparison.Ordinal),
                    Is.False,
                    text.name +
                    " references a project font.");
            }
        }

        [Test]
        public void StatusPrefabHasNoProjectAssetDependencies()
        {
            AssertNoProjectDependencies(
                StatusPrefabPath);
        }

        [Test]
        public void RootPrefabContainsOneRootAndOneView()
        {
            GameObject root =
                LoadPrefab(RootPrefabPath);

            Assert.That(
                root.GetComponentsInChildren<
                    EchoLaunchRoot>(
                        true).Length,
                Is.EqualTo(1));

            Assert.That(
                root.GetComponentsInChildren<
                    EchoLaunchStatusView>(
                        true).Length,
                Is.EqualTo(1));
        }

        [Test]
        public void RootPrefabUsesNestedStatusPrefab()
        {
            GameObject contents =
                PrefabUtility.LoadPrefabContents(
                    RootPrefabPath);

            try
            {
                EchoLaunchStatusView view =
                    contents.GetComponentInChildren<
                        EchoLaunchStatusView>(
                            true);

                string nestedPath =
                    PrefabUtility
                        .GetPrefabAssetPathOfNearestInstanceRoot(
                            view.gameObject);

                Assert.That(
                    nestedPath,
                    Is.EqualTo(
                        StatusPrefabPath));
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(
                    contents);
            }
        }

        [Test]
        public void RootPresenterTargetsNestedView()
        {
            GameObject rootObject =
                LoadPrefab(RootPrefabPath);

            EchoLaunchRoot root =
                rootObject.GetComponent<
                    EchoLaunchRoot>();

            EchoLaunchStatusView view =
                rootObject.GetComponentInChildren<
                    EchoLaunchStatusView>(
                        true);

            SerializedObject serialized =
                new SerializedObject(root);

            SerializedProperty presenter =
                serialized.FindProperty(
                    "statusPresenterComponent");

            Assert.That(
                presenter,
                Is.Not.Null);

            Assert.That(
                presenter.objectReferenceValue,
                Is.SameAs(view));
        }

        [Test]
        public void RootConfigurationIsIntentionallyNull()
        {
            EchoLaunchRoot root =
                LoadPrefab(RootPrefabPath)
                    .GetComponent<EchoLaunchRoot>();

            SerializedObject serialized =
                new SerializedObject(root);

            SerializedProperty configuration =
                serialized.FindProperty(
                    "configuration");

            Assert.That(
                configuration,
                Is.Not.Null);

            Assert.That(
                configuration
                    .objectReferenceValue,
                Is.Null);
        }

        [Test]
        public void RootUsesCanonicalBootAndAutomaticStart()
        {
            EchoLaunchRoot root =
                LoadPrefab(RootPrefabPath)
                    .GetComponent<EchoLaunchRoot>();

            SerializedObject serialized =
                new SerializedObject(root);

            SerializedProperty launchMode =
                serialized.FindProperty(
                    "launchMode");

            SerializedProperty autoStart =
                serialized.FindProperty(
                    "startAutomatically");

            Assert.That(
                launchMode.enumNames[
                    launchMode.enumValueIndex],
                Is.EqualTo(
                    nameof(
                        LaunchMode.CanonicalBoot)));

            Assert.That(
                autoStart.boolValue,
                Is.True);
        }

        [Test]
        public void RootPrefabContainsNoInputAuthority()
        {
            GameObject root =
                LoadPrefab(RootPrefabPath);

            Assert.That(
                root.GetComponentInChildren<
                    EventSystem>(
                        true),
                Is.Null);

            Assert.That(
                root.GetComponentInChildren<
                    BaseInputModule>(
                        true),
                Is.Null);

            Assert.That(
                root.GetComponentInChildren<
                    GraphicRaycaster>(
                        true),
                Is.Null);

            Assert.That(
                root.GetComponentInChildren<
                    Button>(
                        true),
                Is.Null);
        }

        [Test]
        public void RootPrefabHasNoProjectAssetDependencies()
        {
            AssertNoProjectDependencies(
                RootPrefabPath);
        }

        [Test]
        public void StatusPrefabHasNoMissingScripts()
        {
            AssertNoMissingScripts(
                StatusPrefabPath);
        }

        [Test]
        public void RootPrefabHasNoMissingScripts()
        {
            AssertNoMissingScripts(
                RootPrefabPath);
        }

        [Test]
        public void StatusPrefabInstantiates()
        {
            AssertPrefabInstantiates(
                StatusPrefabPath);
        }

        [Test]
        public void RootPrefabInstantiates()
        {
            AssertPrefabInstantiates(
                RootPrefabPath);
        }

        private static GameObject LoadPrefab(
            string path)
        {
            GameObject prefab =
                AssetDatabase.LoadAssetAtPath<
                    GameObject>(
                        path);

            Assert.That(
                prefab,
                Is.Not.Null,
                "Missing prefab: " + path);

            return prefab;
        }

        private static Transform FindDescendant(
            Transform root,
            string name)
        {
            Transform[] transforms =
                root.GetComponentsInChildren<
                    Transform>(
                        true);

            foreach (Transform candidate in transforms)
            {
                if (candidate.name == name)
                {
                    return candidate;
                }
            }

            return null;
        }

        private static void AssertNoProjectDependencies(
            string path)
        {
            string[] dependencies =
                AssetDatabase.GetDependencies(
                    path,
                    true);

            foreach (string dependency in dependencies)
            {
                Assert.That(
                    dependency.StartsWith(
                        "Assets/",
                        StringComparison.Ordinal),
                    Is.False,
                    "Project dependency: " +
                    dependency);
            }
        }

        private static void AssertNoMissingScripts(
            string path)
        {
            GameObject contents =
                PrefabUtility.LoadPrefabContents(
                    path);

            try
            {
                Transform[] transforms =
                    contents.GetComponentsInChildren<
                        Transform>(
                            true);

                foreach (Transform transform in transforms)
                {
                    Assert.That(
                        GameObjectUtility
                            .GetMonoBehavioursWithMissingScriptCount(
                                transform.gameObject),
                        Is.EqualTo(0),
                        transform.name +
                        " has a missing script.");
                }
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(
                    contents);
            }
        }

        private static void AssertPrefabInstantiates(
            string path)
        {
            GameObject prefab =
                LoadPrefab(path);

            GameObject instance =
                PrefabUtility.InstantiatePrefab(
                    prefab)
                as GameObject;

            try
            {
                Assert.That(
                    instance,
                    Is.Not.Null);
            }
            finally
            {
                if (instance != null)
                {
                    UnityEngine.Object
                        .DestroyImmediate(
                            instance);
                }
            }
        }
    }
}

//----- EchoLaunchPresentationPrefabAssetTests.cs END -----
