using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUIRootFoundationTests
    {
        private GameObject rootObject;
        private EchoUIRoot root;
        private EchoUIRoot previousActiveRoot;

        private static readonly System.Reflection.FieldInfo ActiveRootField =
            typeof(EchoUIRoot).GetField(
                "active",
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.NonPublic);

        private static readonly System.Reflection.MethodInfo TryClaimAuthorityMethod =
            typeof(EchoUIRoot).GetMethod(
                "TryClaimAuthority",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic);

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

            SetActiveRootForTest(
                previousActiveRoot);
        }

        [Test]
        public void FirstRootClaimsAuthority()
        {
            Assert.That(
                root.IsAuthoritative,
                Is.True);

            Assert.That(
                EchoUIRoot.Active,
                Is.SameAs(root));
        }

        [Test]
        public void DuplicateRootIsRejectedBeforeInitialization()
        {
            GameObject duplicateObject =
                new GameObject(
                    "Canvas_DuplicateMasterCanvas");

            try
            {
                EchoUIRoot duplicate =
                    duplicateObject.AddComponent<EchoUIRoot>();

                ClaimAuthorityForTest(duplicate);

                Assert.That(
                    duplicate.IsAuthoritative,
                    Is.False);

                UISurfaceOperationResult result =
                    duplicate.Initialize();

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        UISurfaceOperationStatus.NotAuthoritative));

                Assert.That(
                    duplicate.IsInitialized,
                    Is.False);
            }
            finally
            {
                Object.DestroyImmediate(
                    duplicateObject);
            }
        }

        [Test]
        public void DuplicateSurfaceIdsAreRejectedWithoutInitialization()
        {
            CreateSurface(
                "Panel_MainMenu",
                "main-menu",
                UISurfaceRole.Screen,
                "frontend",
                true);

            CreateSurface(
                "Panel_SecondMainMenu",
                "main-menu",
                UISurfaceRole.Screen,
                "frontend",
                false);

            UISurfaceOperationResult result =
                root.Initialize();

            Assert.That(
                result.Status,
                Is.EqualTo(
                    UISurfaceOperationStatus.DuplicateSurfaceId));

            Assert.That(
                root.IsInitialized,
                Is.False);
        }

        [Test]
        public void NavigateToExclusiveScreenReplacesCurrentScreen()
        {
            UISurface main =
                CreateSurface(
                    "Panel_MainMenu",
                    "main-menu",
                    UISurfaceRole.Screen,
                    "frontend",
                    true);

            UISurface settings =
                CreateSurface(
                    "Panel_SettingsMenu",
                    "settings",
                    UISurfaceRole.Screen,
                    "frontend",
                    false);

            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            UISurfaceOperationResult result =
                root.NavigateTo(
                    "settings");

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                main.IsVisible,
                Is.False);

            Assert.That(
                settings.IsVisible,
                Is.True);

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("settings"));
        }

        [Test]
        public void BackRestoresPreviousScreen()
        {
            CreateSurface(
                "Panel_MainMenu",
                "main-menu",
                UISurfaceRole.Screen,
                "frontend",
                true);

            CreateSurface(
                "Panel_SettingsMenu",
                "settings",
                UISurfaceRole.Screen,
                "frontend",
                false);

            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            Assert.That(
                root.NavigateTo("settings").Succeeded,
                Is.True);

            UISurfaceOperationResult back =
                root.Back(
                    "frontend");

            Assert.That(
                back.Succeeded,
                Is.True);

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("main-menu"));

            Assert.That(
                root.IsSurfaceVisible("main-menu"),
                Is.True);

            Assert.That(
                root.IsSurfaceVisible("settings"),
                Is.False);
        }

        [Test]
        public void IndependentWindowCoexistsWithCurrentScreen()
        {
            CreateSurface(
                "Panel_MainMenu",
                "main-menu",
                UISurfaceRole.Screen,
                "frontend",
                true);

            CreateSurface(
                "Panel_DefaultWindow",
                "default-window",
                UISurfaceRole.Window,
                string.Empty,
                false);

            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            Assert.That(
                root.OpenSurface("default-window").Succeeded,
                Is.True);

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("main-menu"));

            Assert.That(
                root.IsSurfaceVisible("main-menu"),
                Is.True);

            Assert.That(
                root.IsSurfaceVisible("default-window"),
                Is.True);

            Assert.That(
                root.ToggleSurface("default-window").Succeeded,
                Is.True);

            Assert.That(
                root.IsSurfaceVisible("default-window"),
                Is.False);

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("main-menu"));
        }

        [Test]
        public void UnknownSurfaceFailsWithoutChangingCurrentScreen()
        {
            CreateSurface(
                "Panel_MainMenu",
                "main-menu",
                UISurfaceRole.Screen,
                "frontend",
                true);

            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            UISurfaceOperationResult result =
                root.NavigateTo(
                    "missing");

            Assert.That(
                result.Status,
                Is.EqualTo(
                    UISurfaceOperationStatus.UnknownSurface));

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("main-menu"));

            Assert.That(
                root.IsSurfaceVisible("main-menu"),
                Is.True);
        }

        private static void ClaimAuthorityForTest(
            EchoUIRoot value)
        {
            Assert.That(
                TryClaimAuthorityMethod,
                Is.Not.Null,
                "EchoUIRoot authority-claim method must remain available to the isolated Editor fixture.");

            TryClaimAuthorityMethod.Invoke(
                value,
                null);
        }

        private static void SetActiveRootForTest(
            EchoUIRoot value)
        {
            Assert.That(
                ActiveRootField,
                Is.Not.Null,
                "EchoUIRoot active authority field must remain available to the isolated Editor fixture.");

            ActiveRootField.SetValue(
                null,
                value);
        }

        private UISurface CreateSurface(
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
                rootObject.transform,
                false);

            UISurface surface =
                child.AddComponent<UISurface>();

            SerializedObject serialized =
                new SerializedObject(surface);

            serialized.FindProperty("surfaceId")
                .stringValue = surfaceId;

            serialized.FindProperty("displayLabel")
                .stringValue = objectName;

            serialized.FindProperty("role")
                .enumValueIndex = (int)role;

            serialized.FindProperty("navigationScopeId")
                .stringValue = scopeId;

            serialized.FindProperty("startVisible")
                .boolValue = startVisible;

            serialized.ApplyModifiedPropertiesWithoutUndo();

            return surface;
        }
    }
}
