using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUIContextAndSelectionTests
    {
        private GameObject rootObject;
        private GameObject eventSystemObject;
        private EchoUIRoot root;
        private EchoUIRoot previousActiveRoot;

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
            if (eventSystemObject != null)
            {
                UnityEngine.Object.DestroyImmediate(
                    eventSystemObject);
            }
            if (rootObject != null)
            {
                UnityEngine.Object.DestroyImmediate(
                    rootObject);
            }

            SetActiveRootForTest(
                previousActiveRoot);
        }

        [Test]
        public void ContextIdsAreProjectDefinedAndStable()
        {
            UIContextId first =
                new UIContextId("  WeirdProjectPauseName  ");
            UIContextId second =
                new UIContextId("WeirdProjectPauseName");
            UIContextId differentCase =
                new UIContextId("weirdprojectpausename");

            Assert.That(first.IsValid, Is.True);
            Assert.That(first.Value, Is.EqualTo("WeirdProjectPauseName"));
            Assert.That(first, Is.EqualTo(second));
            Assert.That(first, Is.Not.EqualTo(differentCase));
        }

        [Test]
        public void MultipleContextsMayBeActiveTogether()
        {
            UIContextState state =
                new UIContextState();

            state.SetActive("pause", true);
            state.SetActive("cinematic", true);

            Assert.That(state.ActiveCount, Is.EqualTo(2));
            Assert.That(state.IsActive("pause"), Is.True);
            Assert.That(state.IsActive("cinematic"), Is.True);
        }

        [Test]
        public void DesignerOrderedRulesResolvePriorityPerDimension()
        {
            UIContextState state =
                ActiveState("pause", "cinematic");
            List<UISurfaceContextRule> rules =
                new List<UISurfaceContextRule>
                {
                    Rule(
                        "cinematic",
                        UISurfaceVisibilityIntent.Hidden,
                        UISurfaceInteractionIntent.NoChange),
                    Rule(
                        "pause",
                        UISurfaceVisibilityIntent.Visible,
                        UISurfaceInteractionIntent.NonInteractable)
                };

            UISurfaceContextResponse result =
                UISurfaceContextResolver.Resolve(
                    rules,
                    state,
                    UISurfaceRuntimeOverride.None);

            Assert.That(
                result.Visibility,
                Is.EqualTo(UISurfaceVisibilityIntent.Hidden));
            Assert.That(
                result.Interaction,
                Is.EqualTo(UISurfaceInteractionIntent.NonInteractable));
        }

        [Test]
        public void UnspecifiedRuleDimensionLeavesCurrentStateUnchanged()
        {
            UISurface surface =
                CreateSurface(
                    "Panel_Hud",
                    "hud",
                    UISurfaceRole.Hud,
                    string.Empty,
                    true);
            CanvasGroup group =
                surface.gameObject.AddComponent<CanvasGroup>();
            group.interactable = true;
            SetRules(
                surface,
                Rule(
                    "pause",
                    UISurfaceVisibilityIntent.NoChange,
                    UISurfaceInteractionIntent.NonInteractable));

            Assert.That(root.Initialize().Succeeded, Is.True);
            Assert.That(root.SetContextActive("pause", true).Succeeded, Is.True);

            Assert.That(surface.IsVisible, Is.True);
            Assert.That(group.interactable, Is.False);
        }

        [Test]
        public void SurfaceCanOptOutOfAutomaticExternalContextHandling()
        {
            UISurface surface =
                CreateSurface(
                    "Panel_Debug",
                    "debug",
                    UISurfaceRole.Overlay,
                    string.Empty,
                    true);
            SetPrivateField(
                surface,
                "allowExternalContext",
                false);
            SetRules(
                surface,
                Rule(
                    "pause",
                    UISurfaceVisibilityIntent.Hidden,
                    UISurfaceInteractionIntent.NoChange));

            Assert.That(root.Initialize().Succeeded, Is.True);
            root.SetContextActive("pause", true);

            Assert.That(surface.IsVisible, Is.True);
        }

        [Test]
        public void SurfaceLocalAuthoredRulesMayDifferWithoutMutatingAnotherSurface()
        {
            UISurface normal =
                CreateSurface(
                    "Panel_Inventory_Normal",
                    "inventory-normal",
                    UISurfaceRole.Window,
                    string.Empty,
                    true);
            UISurface boss =
                CreateSurface(
                    "Panel_Inventory_Boss",
                    "inventory-boss",
                    UISurfaceRole.Window,
                    string.Empty,
                    true);

            SetRules(
                normal,
                Rule(
                    "pause",
                    UISurfaceVisibilityIntent.Visible,
                    UISurfaceInteractionIntent.NoChange));
            SetRules(
                boss,
                Rule(
                    "pause",
                    UISurfaceVisibilityIntent.Hidden,
                    UISurfaceInteractionIntent.NoChange));

            Assert.That(root.Initialize().Succeeded, Is.True);
            root.SetContextActive("pause", true);

            Assert.That(normal.IsVisible, Is.True);
            Assert.That(boss.IsVisible, Is.False);
            Assert.That(
                normal.ContextRules[0].Response.Visibility,
                Is.EqualTo(UISurfaceVisibilityIntent.Visible));
        }

        [Test]
        public void RuntimeOverrideSupersedesEffectiveAuthoredValueWithoutMutatingAuthoredConfiguration()
        {
            UISurface surface =
                CreateSurface(
                    "Panel_QuestTracker",
                    "quest-tracker",
                    UISurfaceRole.Hud,
                    string.Empty,
                    true);
            SetRules(
                surface,
                Rule(
                    "pause",
                    UISurfaceVisibilityIntent.Hidden,
                    UISurfaceInteractionIntent.NoChange));

            Assert.That(root.Initialize().Succeeded, Is.True);
            root.SetContextActive("pause", true);
            Assert.That(surface.IsVisible, Is.False);

            UISurfaceRuntimeOverride runtimeOverride =
                new UISurfaceRuntimeOverride(
                    UISurfaceVisibilityIntent.Visible,
                    UISurfaceInteractionIntent.NoChange,
                    UISurfaceSelectionIntent.NoChange);
            Assert.That(
                root.SetSurfaceRuntimeOverride(
                    "quest-tracker",
                    runtimeOverride).Succeeded,
                Is.True);

            Assert.That(surface.IsVisible, Is.True);
            Assert.That(
                surface.ContextRules[0].Response.Visibility,
                Is.EqualTo(UISurfaceVisibilityIntent.Hidden));
        }

        [Test]
        public void DeactivatingContextRestoresDimensionsPreviouslyControlledByThatContext()
        {
            UISurface surface =
                CreateSurface(
                    "Panel_PauseWindow",
                    "pause-window",
                    UISurfaceRole.Window,
                    string.Empty,
                    false);
            CanvasGroup group =
                surface.gameObject.AddComponent<CanvasGroup>();
            group.interactable = true;
            SetRules(
                surface,
                Rule(
                    "pause",
                    UISurfaceVisibilityIntent.Visible,
                    UISurfaceInteractionIntent.NonInteractable));

            Assert.That(root.Initialize().Succeeded, Is.True);
            Assert.That(surface.IsVisible, Is.False);
            Assert.That(surface.IsInteractable, Is.True);

            Assert.That(root.SetContextActive("pause", true).Succeeded, Is.True);
            Assert.That(surface.IsVisible, Is.True);
            Assert.That(surface.IsInteractable, Is.False);

            Assert.That(root.SetContextActive("pause", false).Succeeded, Is.True);
            Assert.That(surface.IsVisible, Is.False);
            Assert.That(surface.IsInteractable, Is.True);
        }

        [Test]
        public void ClearingRuntimeOverrideRestoresPreOverrideStateWhenNoContextRuleApplies()
        {
            UISurface surface =
                CreateSurface(
                    "Panel_RuntimeOverride",
                    "runtime-override",
                    UISurfaceRole.Hud,
                    string.Empty,
                    true);
            CanvasGroup group =
                surface.gameObject.AddComponent<CanvasGroup>();
            group.interactable = true;

            Assert.That(root.Initialize().Succeeded, Is.True);

            UISurfaceRuntimeOverride runtimeOverride =
                new UISurfaceRuntimeOverride(
                    UISurfaceVisibilityIntent.Hidden,
                    UISurfaceInteractionIntent.NonInteractable,
                    UISurfaceSelectionIntent.NoChange);
            Assert.That(
                root.SetSurfaceRuntimeOverride(
                    "runtime-override",
                    runtimeOverride).Succeeded,
                Is.True);
            Assert.That(surface.IsVisible, Is.False);
            Assert.That(surface.IsInteractable, Is.False);

            Assert.That(
                root.ClearSurfaceRuntimeOverride(
                    "runtime-override").Succeeded,
                Is.True);
            Assert.That(surface.IsVisible, Is.True);
            Assert.That(surface.IsInteractable, Is.True);
        }

        [Test]
        public void DirectCloseWhileContextControlsVisibilityBecomesRestoredBaseline()
        {
            UISurface surface =
                CreateSurface(
                    "Panel_ContextWindow",
                    "context-window",
                    UISurfaceRole.Window,
                    string.Empty,
                    false);
            SetRules(
                surface,
                Rule(
                    "pause",
                    UISurfaceVisibilityIntent.Visible,
                    UISurfaceInteractionIntent.NoChange));

            Assert.That(root.Initialize().Succeeded, Is.True);
            root.SetContextActive("pause", true);
            Assert.That(surface.IsVisible, Is.True);

            Assert.That(root.CloseSurface("context-window").Succeeded, Is.True);
            Assert.That(surface.IsVisible, Is.False);

            root.SetContextActive("pause", false);
            Assert.That(surface.IsVisible, Is.False);
        }

        [Test]
        public void PointerOpenPolicyMayRemainUnselected()
        {
            EnsureEventSystem();
            UISurface surface =
                CreateSurface(
                    "Panel_PointerWindow",
                    "pointer-window",
                    UISurfaceRole.Window,
                    string.Empty,
                    false);
            GameObject target =
                CreateSelectionTarget(
                    surface,
                    "Button_Default");
            SetSelectionPolicy(
                surface,
                new UISurfaceSelectionPolicy(
                    UISelectionOpenBehavior.ClearSelection,
                    UISelectionOpenBehavior.SelectDefault,
                    target));

            Assert.That(root.Initialize().Succeeded, Is.True);
            root.SetInputModality(UIInputModality.Pointer);
            Assert.That(root.OpenSurface("pointer-window").Succeeded, Is.True);

            Assert.That(
                UISelectionCoordinator.CurrentSelectedObject,
                Is.Null);
        }

        [Test]
        public void NavigationModalityCanSelectConfiguredDefault()
        {
            EnsureEventSystem();
            UISurface surface =
                CreateSurface(
                    "Panel_PauseMenu",
                    "pause-menu",
                    UISurfaceRole.Window,
                    string.Empty,
                    false);
            GameObject target =
                CreateSelectionTarget(
                    surface,
                    "Button_Resume");
            SetSelectionPolicy(
                surface,
                new UISurfaceSelectionPolicy(
                    UISelectionOpenBehavior.ClearSelection,
                    UISelectionOpenBehavior.SelectDefault,
                    target));

            Assert.That(root.Initialize().Succeeded, Is.True);
            root.SetInputModality(UIInputModality.Navigation);
            root.OpenSurface("pause-menu");

            Assert.That(
                UISelectionCoordinator.CurrentSelectedObject,
                Is.SameAs(target));
        }

        [Test]
        public void NavigationModalityMayBeConfiguredToRemainUnselected()
        {
            EnsureEventSystem();
            UISurface surface =
                CreateSurface(
                    "Panel_RadialMenu",
                    "radial-menu",
                    UISurfaceRole.Window,
                    string.Empty,
                    false);
            GameObject target =
                CreateSelectionTarget(
                    surface,
                    "Button_Default");
            SetSelectionPolicy(
                surface,
                new UISurfaceSelectionPolicy(
                    UISelectionOpenBehavior.ClearSelection,
                    UISelectionOpenBehavior.ClearSelection,
                    target));

            Assert.That(root.Initialize().Succeeded, Is.True);
            root.SetInputModality(UIInputModality.Navigation);
            root.OpenSurface("radial-menu");

            Assert.That(
                UISelectionCoordinator.CurrentSelectedObject,
                Is.Null);
        }

        [Test]
        public void TemporarySurfaceCloseDoesNotRestoreHistoricalSelectionByDefault()
        {
            EnsureEventSystem();
            UISurface first =
                CreateSurface(
                    "Panel_First",
                    "first",
                    UISurfaceRole.Window,
                    string.Empty,
                    false);
            GameObject firstTarget =
                CreateSelectionTarget(
                    first,
                    "Button_First");
            SetSelectionPolicy(
                first,
                NavigationPolicy(firstTarget));

            UISurface second =
                CreateSurface(
                    "Panel_Second",
                    "second",
                    UISurfaceRole.Window,
                    string.Empty,
                    false);
            GameObject secondTarget =
                CreateSelectionTarget(
                    second,
                    "Button_Second");
            SetSelectionPolicy(
                second,
                NavigationPolicy(secondTarget));

            Assert.That(root.Initialize().Succeeded, Is.True);
            root.SetInputModality(UIInputModality.Navigation);
            root.OpenSurface("first");
            Assert.That(
                UISelectionCoordinator.CurrentSelectedObject,
                Is.SameAs(firstTarget));

            root.OpenSurface("second");
            Assert.That(
                UISelectionCoordinator.CurrentSelectedObject,
                Is.SameAs(secondTarget));

            root.CloseSurface("second");
            Assert.That(
                UISelectionCoordinator.CurrentSelectedObject,
                Is.Null);
        }

        [Test]
        public void ContextAndSelectionChangesPreserveScopedScreenHistoryAndIndependentWindowCoexistence()
        {
            EnsureEventSystem();
            CreateSurface(
                "Panel_MainMenu",
                "main-menu",
                UISurfaceRole.Screen,
                "frontend",
                true);
            CreateSurface(
                "Panel_Settings",
                "settings",
                UISurfaceRole.Screen,
                "frontend",
                false);
            UISurface window =
                CreateSurface(
                    "Panel_DefaultWindow",
                    "default-window",
                    UISurfaceRole.Window,
                    string.Empty,
                    false);
            SetRules(
                window,
                Rule(
                    "pause",
                    UISurfaceVisibilityIntent.Visible,
                    UISurfaceInteractionIntent.NoChange));

            Assert.That(root.Initialize().Succeeded, Is.True);
            Assert.That(root.NavigateTo("settings").Succeeded, Is.True);
            Assert.That(root.OpenSurface("default-window").Succeeded, Is.True);
            root.SetContextActive("pause", true);

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("settings"));
            Assert.That(window.IsVisible, Is.True);

            Assert.That(root.Back("frontend").Succeeded, Is.True);
            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("main-menu"));
            Assert.That(window.IsVisible, Is.True);
        }

        [Test]
        public void InvalidSelectionTargetFailsSafelyWithoutChangingSurfaceState()
        {
            EnsureEventSystem();
            UISurface surface =
                CreateSurface(
                    "Panel_TargetSafety",
                    "target-safety",
                    UISurfaceRole.Window,
                    string.Empty,
                    false);
            GameObject outsideTarget =
                new GameObject(
                    "Button_OutsideSurface");
            outsideTarget.transform.SetParent(
                rootObject.transform,
                false);
            SetSelectionPolicy(
                surface,
                NavigationPolicy(outsideTarget));

            Assert.That(root.Initialize().Succeeded, Is.True);
            root.SetInputModality(UIInputModality.Navigation);
            Assert.That(root.OpenSurface("target-safety").Succeeded, Is.True);

            Assert.That(surface.IsVisible, Is.True);
            Assert.That(
                UISelectionCoordinator.CurrentSelectedObject,
                Is.Null);
        }

        [Test]
        public void RuntimeAssemblyHasNoPeerEchoPackageDependency()
        {
            string[] references =
                typeof(EchoUIRoot)
                    .Assembly
                    .GetReferencedAssemblies()
                    .Select(value => value.Name)
                    .Where(value =>
                        value.StartsWith(
                            "EchoDevGames.",
                            StringComparison.Ordinal) &&
                        !string.Equals(
                            value,
                            "EchoDevGames.EchoUI.Runtime",
                            StringComparison.Ordinal))
                    .ToArray();

            Assert.That(
                references,
                Is.Empty,
                "EchoUI Runtime must not hard-reference a peer Echo package.");
        }

        private UISurface CreateSurface(
            string objectName,
            string surfaceId,
            UISurfaceRole role,
            string scopeId,
            bool startVisible)
        {
            GameObject child =
                new GameObject(objectName);
            child.transform.SetParent(
                rootObject.transform,
                false);

            UISurface surface =
                child.AddComponent<UISurface>();
            SetPrivateField(surface, "surfaceId", surfaceId);
            SetPrivateField(surface, "displayLabel", objectName);
            SetPrivateField(surface, "role", role);
            SetPrivateField(surface, "navigationScopeId", scopeId);
            SetPrivateField(surface, "startVisible", startVisible);
            return surface;
        }

        private static UISurfaceContextRule Rule(
            string contextId,
            UISurfaceVisibilityIntent visibility,
            UISurfaceInteractionIntent interaction) =>
            new UISurfaceContextRule(
                contextId,
                new UISurfaceContextResponse(
                    visibility,
                    interaction,
                    UISurfaceSelectionIntent.NoChange));

        private static UIContextState ActiveState(
            params string[] contextIds)
        {
            UIContextState state =
                new UIContextState();
            for (int index = 0;
                 index < contextIds.Length;
                 index++)
            {
                state.SetActive(
                    contextIds[index],
                    true);
            }
            return state;
        }

        private static void SetRules(
            UISurface surface,
            params UISurfaceContextRule[] rules)
        {
            SetPrivateField(
                surface,
                "contextRules",
                new List<UISurfaceContextRule>(rules));
        }

        private static void SetSelectionPolicy(
            UISurface surface,
            UISurfaceSelectionPolicy policy)
        {
            SetPrivateField(
                surface,
                "selectionPolicy",
                policy);
        }

        private static UISurfaceSelectionPolicy NavigationPolicy(
            GameObject target) =>
            new UISurfaceSelectionPolicy(
                UISelectionOpenBehavior.ClearSelection,
                UISelectionOpenBehavior.SelectDefault,
                target);

        private static GameObject CreateSelectionTarget(
            UISurface surface,
            string name)
        {
            GameObject target =
                new GameObject(name);
            target.transform.SetParent(
                surface.transform,
                false);
            return target;
        }

        private void EnsureEventSystem()
        {
            Type eventSystemType =
                Type.GetType(
                    "UnityEngine.EventSystems.EventSystem, UnityEngine.UI");
            if (eventSystemType == null)
            {
                eventSystemType =
                    AppDomain.CurrentDomain
                        .GetAssemblies()
                        .Select(assembly =>
                            assembly.GetType(
                                "UnityEngine.EventSystems.EventSystem"))
                        .FirstOrDefault(type => type != null);
            }

            Assert.That(
                eventSystemType,
                Is.Not.Null,
                "Unity EventSystem type must be available for Looking Glass selection tests.");

            eventSystemObject =
                new GameObject(
                    "EventSystem_EUI_M1_02_Test");
            eventSystemObject.AddComponent(
                eventSystemType);
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

        private static void SetPrivateField<T>(
            object target,
            string fieldName,
            T value)
        {
            FieldInfo field =
                target.GetType().GetField(
                    fieldName,
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);
            Assert.That(
                field,
                Is.Not.Null,
                $"Expected field '{fieldName}' was not found.");
            field.SetValue(
                target,
                value);
        }
    }
}
