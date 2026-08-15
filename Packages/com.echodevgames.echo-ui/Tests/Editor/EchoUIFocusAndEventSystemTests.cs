using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUIFocusAndEventSystemTests
    {
        private readonly List<GameObject> created =
            new List<GameObject>();

        [TearDown]
        public void TearDown()
        {
            for (int index = created.Count - 1;
                 index >= 0;
                 index--)
            {
                if (created[index] != null)
                {
                    UnityEngine.Object.DestroyImmediate(
                        created[index]);
                }
            }

            created.Clear();

            EventSystem[] remaining =
                UnityEngine.Object.FindObjectsByType<EventSystem>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.InstanceID);

            for (int index = 0;
                 index < remaining.Length;
                 index++)
            {
                if (remaining[index] != null)
                {
                    UnityEngine.Object.DestroyImmediate(
                        remaining[index].gameObject);
                }
            }
        }

        [Test]
        public void AdoptAssignedUsesExplicitEventSystem()
        {
            EventSystem assigned =
                CreateEventSystem("Assigned");

            CreateEventSystem("Other");

            UIEventSystemCoordinator coordinator =
                new UIEventSystemCoordinator();

            UIEventSystemCoordinationResult result =
                coordinator.Coordinate(
                    UIEventSystemCoordinationMode.AdoptAssigned,
                    assigned);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.EventSystem, Is.SameAs(assigned));
            Assert.That(result.CreatedByLookingGlass, Is.False);
        }

        [Test]
        public void AdoptExistingRequiresUnambiguousEligibleSystem()
        {
            EventSystem first =
                CreateEventSystem("First");

            UIEventSystemCoordinator coordinator =
                new UIEventSystemCoordinator();

            UIEventSystemCoordinationResult ready =
                coordinator.Coordinate(
                    UIEventSystemCoordinationMode.AdoptExisting,
                    null);

            Assert.That(ready.Succeeded, Is.True);
            Assert.That(ready.EventSystem, Is.SameAs(first));

            CreateEventSystem("Second");

            UIEventSystemCoordinationResult ambiguous =
                coordinator.Coordinate(
                    UIEventSystemCoordinationMode.AdoptExisting,
                    null);

            Assert.That(
                ambiguous.Status,
                Is.EqualTo(
                    UIEventSystemCoordinationStatus.Ambiguous));

            Assert.That(ambiguous.EventSystem, Is.Null);
        }

        [Test]
        public void CreateIfMissingCreatesOnlyWhenConfiguredAndMissing()
        {
            UIEventSystemCoordinator coordinator =
                new UIEventSystemCoordinator();

            UIEventSystemCoordinationResult createdResult =
                coordinator.Coordinate(
                    UIEventSystemCoordinationMode.CreateIfMissing,
                    null);

            Assert.That(createdResult.Succeeded, Is.True);
            Assert.That(createdResult.CreatedByLookingGlass, Is.True);
            Assert.That(coordinator.OwnsCreatedEventSystem, Is.True);

            coordinator.Shutdown();

            EventSystem external =
                CreateEventSystem("External");

            UIEventSystemCoordinationResult adopted =
                coordinator.Coordinate(
                    UIEventSystemCoordinationMode.CreateIfMissing,
                    null);

            Assert.That(adopted.Succeeded, Is.True);
            Assert.That(adopted.EventSystem, Is.SameAs(external));
            Assert.That(adopted.CreatedByLookingGlass, Is.False);
        }

        [Test]
        public void RequireExternalNeverCreatesEventSystem()
        {
            UIEventSystemCoordinator coordinator =
                new UIEventSystemCoordinator();

            UIEventSystemCoordinationResult result =
                coordinator.Coordinate(
                    UIEventSystemCoordinationMode.RequireExternal,
                    null);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    UIEventSystemCoordinationStatus.Missing));

            Assert.That(
                UnityEngine.Object.FindObjectsByType<EventSystem>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.InstanceID).Length,
                Is.EqualTo(0));
        }

        [Test]
        public void MultipleActiveEventSystemsEnterDegradedBlockedFocusStateWithoutDeletion()
        {
            EventSystem first =
                CreateEventSystem("First");

            EventSystem second =
                CreateEventSystem("Second");

            UIEventSystemCoordinator coordinator =
                new UIEventSystemCoordinator();

            UIEventSystemCoordinationResult result =
                coordinator.Coordinate(
                    UIEventSystemCoordinationMode.AdoptExisting,
                    null);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    UIEventSystemCoordinationStatus.Ambiguous));

            Assert.That(first, Is.Not.Null);
            Assert.That(second, Is.Not.Null);
            Assert.That(
                UnityEngine.Object.FindObjectsByType<EventSystem>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.InstanceID).Length,
                Is.EqualTo(2));
        }

        [Test]
        public void LiveEntryRemembersLastValidFocus()
        {
            EventSystem eventSystem =
                CreateEventSystem("EventSystem");

            UISurface surface =
                CreateSurface(
                    "screen",
                    UISurfaceRole.Screen,
                    UIFocusReopenBehavior.Fresh,
                    out GameObject authoredDefault,
                    out GameObject alternate);

            UISelectionCoordinator coordinator =
                CreateCoordinator(
                    eventSystem);

            coordinator.ApplyOpenSelection(
                surface,
                UIInputModality.Navigation);

            eventSystem.SetSelectedGameObject(
                alternate);

            coordinator.ClearSelectionForSurface(
                surface);

            Assert.That(
                coordinator.TryGetRememberedFocus(
                    surface,
                    out GameObject remembered),
                Is.True);

            Assert.That(remembered, Is.SameAs(alternate));
            Assert.That(eventSystem.currentSelectedGameObject, Is.Null);
        }

        [Test]
        public void SurfaceMayOptIntoSessionLevelReopenMemory()
        {
            EventSystem eventSystem =
                CreateEventSystem("EventSystem");

            UISurface surface =
                CreateSurface(
                    "inventory",
                    UISurfaceRole.Window,
                    UIFocusReopenBehavior.RememberThisSession,
                    out _,
                    out GameObject alternate);

            UISelectionCoordinator coordinator =
                CreateCoordinator(
                    eventSystem);

            coordinator.ApplyOpenSelection(
                surface,
                UIInputModality.Navigation);

            eventSystem.SetSelectedGameObject(
                alternate);

            coordinator.CloseSurface(
                surface);

            coordinator.ApplyOpenSelection(
                surface,
                UIInputModality.Navigation);

            Assert.That(
                eventSystem.currentSelectedGameObject,
                Is.SameAs(alternate));
        }

        [Test]
        public void FreshReopenPolicyIgnoresSessionMemory()
        {
            EventSystem eventSystem =
                CreateEventSystem("EventSystem");

            UISurface surface =
                CreateSurface(
                    "menu",
                    UISurfaceRole.Window,
                    UIFocusReopenBehavior.RememberThisSession,
                    out GameObject authoredDefault,
                    out GameObject alternate);

            UISelectionCoordinator coordinator =
                CreateCoordinator(
                    eventSystem);

            coordinator.ApplyOpenSelection(
                surface,
                UIInputModality.Navigation);

            eventSystem.SetSelectedGameObject(
                alternate);

            coordinator.CloseSurface(
                surface);

            SetSelectionPolicy(
                surface,
                new UISurfaceSelectionPolicy(
                    UISelectionOpenBehavior.ClearSelection,
                    UISelectionOpenBehavior.SelectDefault,
                    authoredDefault,
                    UIFocusReopenBehavior.Fresh,
                    true));

            coordinator.ApplyOpenSelection(
                surface,
                UIInputModality.Navigation);

            Assert.That(
                eventSystem.currentSelectedGameObject,
                Is.SameAs(authoredDefault));
        }

        [Test]
        public void ModalCloseRestoresUnderlyingRememberedFocusWhenPolicyAllows()
        {
            EventSystem eventSystem =
                CreateEventSystem("EventSystem");

            UISurface screen =
                CreateSurface(
                    "screen",
                    UISurfaceRole.Screen,
                    UIFocusReopenBehavior.Fresh,
                    out _,
                    out GameObject screenAlternate);

            UISurface modal =
                CreateSurface(
                    "confirm",
                    UISurfaceRole.Modal,
                    UIFocusReopenBehavior.Fresh,
                    out GameObject modalDefault,
                    out _);

            UISelectionCoordinator coordinator =
                CreateCoordinator(
                    eventSystem);

            coordinator.ApplyOpenSelection(
                screen,
                UIInputModality.Navigation);

            eventSystem.SetSelectedGameObject(
                screenAlternate);

            coordinator.ApplyOpenSelection(
                modal,
                UIInputModality.Navigation);

            coordinator.ApplyModalStackChanged(
                modal,
                1,
                UIInputModality.Navigation,
                new[] { screen, modal });

            Assert.That(
                eventSystem.currentSelectedGameObject,
                Is.SameAs(modalDefault));

            coordinator.CloseSurface(
                modal);

            coordinator.ApplyModalStackChanged(
                null,
                0,
                UIInputModality.Navigation,
                new[] { screen, modal });

            Assert.That(
                eventSystem.currentSelectedGameObject,
                Is.SameAs(screenAlternate));
        }

        [Test]
        public void ScreenBackRestoresPreviousEntryFocusWhenPolicyAllows()
        {
            EventSystem eventSystem =
                CreateEventSystem("EventSystem");

            UISurface first =
                CreateSurface(
                    "first",
                    UISurfaceRole.Screen,
                    UIFocusReopenBehavior.Fresh,
                    out _,
                    out GameObject alternate);

            UISurface second =
                CreateSurface(
                    "second",
                    UISurfaceRole.Screen,
                    UIFocusReopenBehavior.Fresh,
                    out _,
                    out _);

            UISelectionCoordinator coordinator =
                CreateCoordinator(
                    eventSystem);

            coordinator.ApplyOpenSelection(
                first,
                UIInputModality.Navigation);

            eventSystem.SetSelectedGameObject(
                alternate);

            coordinator.SuspendSurface(
                first);

            coordinator.ApplyOpenSelection(
                second,
                UIInputModality.Navigation);

            coordinator.CloseSurface(
                second);

            coordinator.ApplyOpenSelection(
                first,
                UIInputModality.Navigation);

            Assert.That(
                eventSystem.currentSelectedGameObject,
                Is.SameAs(alternate));
        }

        [Test]
        public void InvalidRememberedTargetFallsThroughFallbackChain()
        {
            EventSystem eventSystem =
                CreateEventSystem("EventSystem");

            UISurface surface =
                CreateSurface(
                    "screen",
                    UISurfaceRole.Screen,
                    UIFocusReopenBehavior.Fresh,
                    out GameObject authoredDefault,
                    out GameObject alternate);

            UISelectionCoordinator coordinator =
                CreateCoordinator(
                    eventSystem);

            coordinator.ApplyOpenSelection(
                surface,
                UIInputModality.Navigation);

            eventSystem.SetSelectedGameObject(
                alternate);

            coordinator.SuspendSurface(
                surface);

            alternate.SetActive(false);

            coordinator.ApplyOpenSelection(
                surface,
                UIInputModality.Navigation);

            Assert.That(
                eventSystem.currentSelectedGameObject,
                Is.SameAs(authoredDefault));
        }

        [Test]
        public void PointerPolicyMayResolveToNoFocus()
        {
            EventSystem eventSystem =
                CreateEventSystem("EventSystem");

            UISurface surface =
                CreateSurface(
                    "pointer",
                    UISurfaceRole.Window,
                    UIFocusReopenBehavior.Fresh,
                    out GameObject authoredDefault,
                    out _);

            UISelectionCoordinator coordinator =
                CreateCoordinator(
                    eventSystem);

            eventSystem.SetSelectedGameObject(
                authoredDefault);

            coordinator.ApplyOpenSelection(
                surface,
                UIInputModality.Pointer);

            Assert.That(
                eventSystem.currentSelectedGameObject,
                Is.Null);
        }

        [Test]
        public void NavigationPolicyCanResolveConfiguredDefault()
        {
            EventSystem eventSystem =
                CreateEventSystem("EventSystem");

            UISurface surface =
                CreateSurface(
                    "navigation",
                    UISurfaceRole.Window,
                    UIFocusReopenBehavior.Fresh,
                    out GameObject authoredDefault,
                    out _);

            UISelectionCoordinator coordinator =
                CreateCoordinator(
                    eventSystem);

            coordinator.ApplyOpenSelection(
                surface,
                UIInputModality.Navigation);

            Assert.That(
                eventSystem.currentSelectedGameObject,
                Is.SameAs(authoredDefault));
        }

        [Test]
        public void BlockingModalContainsFocusToTopModal()
        {
            EventSystem eventSystem =
                CreateEventSystem("EventSystem");

            UISurface screen =
                CreateSurface(
                    "screen",
                    UISurfaceRole.Screen,
                    UIFocusReopenBehavior.Fresh,
                    out GameObject screenDefault,
                    out _);

            UISurface modal =
                CreateSurface(
                    "modal",
                    UISurfaceRole.Modal,
                    UIFocusReopenBehavior.Fresh,
                    out GameObject modalDefault,
                    out _);

            UISelectionCoordinator coordinator =
                CreateCoordinator(
                    eventSystem);

            coordinator.ApplyOpenSelection(
                screen,
                UIInputModality.Navigation);

            coordinator.ApplyOpenSelection(
                modal,
                UIInputModality.Navigation);

            coordinator.ApplyModalStackChanged(
                modal,
                1,
                UIInputModality.Navigation,
                new[] { screen, modal });

            eventSystem.SetSelectedGameObject(
                screenDefault);

            UIFocusRequestResult result =
                coordinator.Revalidate(
                    UIInputModality.Navigation,
                    modal,
                    new[] { screen, modal },
                    coordinator.Generation);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(
                eventSystem.currentSelectedGameObject,
                Is.SameAs(modalDefault));
        }

        [Test]
        public void LowerEntryFocusMemorySurvivesModalContainment()
        {
            EventSystem eventSystem =
                CreateEventSystem("EventSystem");

            UISurface screen =
                CreateSurface(
                    "screen",
                    UISurfaceRole.Screen,
                    UIFocusReopenBehavior.Fresh,
                    out _,
                    out GameObject alternate);

            UISurface modal =
                CreateSurface(
                    "modal",
                    UISurfaceRole.Modal,
                    UIFocusReopenBehavior.Fresh,
                    out _,
                    out _);

            UISelectionCoordinator coordinator =
                CreateCoordinator(
                    eventSystem);

            coordinator.ApplyOpenSelection(
                screen,
                UIInputModality.Navigation);

            eventSystem.SetSelectedGameObject(
                alternate);

            coordinator.ApplyOpenSelection(
                modal,
                UIInputModality.Navigation);

            Assert.That(
                coordinator.TryGetRememberedFocus(
                    screen,
                    out GameObject remembered),
                Is.True);

            Assert.That(remembered, Is.SameAs(alternate));
        }

        [Test]
        public void ExplicitRevalidationRepairsDynamicInvalidFocus()
        {
            EventSystem eventSystem =
                CreateEventSystem("EventSystem");

            UISurface surface =
                CreateSurface(
                    "dynamic",
                    UISurfaceRole.Window,
                    UIFocusReopenBehavior.Fresh,
                    out GameObject authoredDefault,
                    out GameObject alternate);

            UISelectionCoordinator coordinator =
                CreateCoordinator(
                    eventSystem);

            coordinator.ApplyOpenSelection(
                surface,
                UIInputModality.Navigation);

            eventSystem.SetSelectedGameObject(
                alternate);

            coordinator.RememberCurrentFocus();

            alternate.SetActive(false);

            UIFocusRequestResult result =
                coordinator.Revalidate(
                    UIInputModality.Navigation,
                    null,
                    new[] { surface },
                    coordinator.Generation);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(
                eventSystem.currentSelectedGameObject,
                Is.SameAs(authoredDefault));
        }

        [Test]
        public void StaleFocusRequestCannotOverrideNewerState()
        {
            EventSystem eventSystem =
                CreateEventSystem("EventSystem");

            UISurface first =
                CreateSurface(
                    "first",
                    UISurfaceRole.Window,
                    UIFocusReopenBehavior.Fresh,
                    out GameObject firstDefault,
                    out _);

            UISurface second =
                CreateSurface(
                    "second",
                    UISurfaceRole.Window,
                    UIFocusReopenBehavior.Fresh,
                    out GameObject secondDefault,
                    out _);

            UISelectionCoordinator coordinator =
                CreateCoordinator(
                    eventSystem);

            coordinator.ApplyOpenSelection(
                first,
                UIInputModality.Navigation);

            long stale =
                coordinator.Generation;

            coordinator.ApplyOpenSelection(
                second,
                UIInputModality.Navigation);

            UIFocusRequestResult result =
                coordinator.RequestFocus(
                    first,
                    firstDefault,
                    UIInputModality.Navigation,
                    stale);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    UIFocusRequestStatus.Stale));

            Assert.That(
                eventSystem.currentSelectedGameObject,
                Is.SameAs(secondDefault));
        }

        [Test]
        public void IndependentWindowsRetainDistinctFocusMemoryWithoutWindowManager()
        {
            EventSystem eventSystem =
                CreateEventSystem("EventSystem");

            UISurface first =
                CreateSurface(
                    "inventory",
                    UISurfaceRole.Window,
                    UIFocusReopenBehavior.Fresh,
                    out _,
                    out GameObject firstAlternate);

            UISurface second =
                CreateSurface(
                    "character",
                    UISurfaceRole.Window,
                    UIFocusReopenBehavior.Fresh,
                    out _,
                    out GameObject secondAlternate);

            UISelectionCoordinator coordinator =
                CreateCoordinator(
                    eventSystem);

            coordinator.ApplyOpenSelection(
                first,
                UIInputModality.Navigation);

            eventSystem.SetSelectedGameObject(
                firstAlternate);

            coordinator.RememberCurrentFocus();

            coordinator.ApplyOpenSelection(
                second,
                UIInputModality.Navigation);

            eventSystem.SetSelectedGameObject(
                secondAlternate);

            coordinator.RememberCurrentFocus();

            Assert.That(
                coordinator.TryGetRememberedFocus(
                    first,
                    out GameObject firstRemembered),
                Is.True);

            Assert.That(
                coordinator.TryGetRememberedFocus(
                    second,
                    out GameObject secondRemembered),
                Is.True);

            Assert.That(firstRemembered, Is.SameAs(firstAlternate));
            Assert.That(secondRemembered, Is.SameAs(secondAlternate));
        }

        [Test]
        public void RuntimeAssemblyHasNoPeerEchoPackageDependency()
        {
            string[] references =
                typeof(EchoUIRoot)
                    .Assembly
                    .GetReferencedAssemblies()
                    .Select(item => item.Name)
                    .Where(item =>
                        item.StartsWith(
                            "EchoDevGames.",
                            StringComparison.Ordinal) &&
                        !string.Equals(
                            item,
                            "EchoDevGames.EchoUI.Runtime",
                            StringComparison.Ordinal))
                    .ToArray();

            Assert.That(references, Is.Empty);
        }

        [Test]
        public void CoreHasNoHardInputSystemGeneratedWrapperDependency()
        {
            Type[] types =
                typeof(EchoUIRoot)
                    .Assembly
                    .GetTypes();

            bool wrapperReference =
                types.Any(type =>
                    type.GetFields(
                            BindingFlags.Instance |
                            BindingFlags.Static |
                            BindingFlags.Public |
                            BindingFlags.NonPublic)
                        .Any(field =>
                            string.Equals(
                                field.FieldType.Name,
                                "InputSystem_Actions",
                                StringComparison.Ordinal)) ||
                    type.GetProperties(
                            BindingFlags.Instance |
                            BindingFlags.Static |
                            BindingFlags.Public |
                            BindingFlags.NonPublic)
                        .Any(property =>
                            string.Equals(
                                property.PropertyType.Name,
                                "InputSystem_Actions",
                                StringComparison.Ordinal)));

            Assert.That(wrapperReference, Is.False);
        }

        [Test]
        public void FocusCoordinatorHasNoPerFrameUpdateLoop()
        {
            MethodInfo update =
                typeof(UISelectionCoordinator)
                    .GetMethod(
                        "Update",
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.NonPublic);

            MethodInfo lateUpdate =
                typeof(UISelectionCoordinator)
                    .GetMethod(
                        "LateUpdate",
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.NonPublic);

            Assert.That(update, Is.Null);
            Assert.That(lateUpdate, Is.Null);
        }

        [Test]
        public void PointerModalityChangeDoesNotClearExistingFocus()
        {
            EventSystem eventSystem =
                CreateEventSystem("EventSystem");

            UISurface surface =
                CreateSurface(
                    "window",
                    UISurfaceRole.Window,
                    UIFocusReopenBehavior.Fresh,
                    out GameObject authoredDefault,
                    out _);

            UISelectionCoordinator coordinator =
                CreateCoordinator(
                    eventSystem);

            coordinator.ApplyOpenSelection(
                surface,
                UIInputModality.Navigation);

            coordinator.OnInputModalityChanged(
                UIInputModality.Pointer,
                new[] { surface });

            Assert.That(
                eventSystem.currentSelectedGameObject,
                Is.SameAs(authoredDefault));
        }

        [Test]
        public void EntryResolverParticipatesBeforeLegalNoFocus()
        {
            EventSystem eventSystem =
                CreateEventSystem("EventSystem");

            UISurface surface =
                CreateSurface(
                    "resolver",
                    UISurfaceRole.Window,
                    UIFocusReopenBehavior.Fresh,
                    out _,
                    out GameObject alternate);

            SetSelectionPolicy(
                surface,
                new UISurfaceSelectionPolicy(
                    UISelectionOpenBehavior.ClearSelection,
                    UISelectionOpenBehavior.SelectDefault,
                    null,
                    UIFocusReopenBehavior.Fresh,
                    true));

            TestFocusResolver resolver =
                surface.gameObject.AddComponent<TestFocusResolver>();

            resolver.Target =
                alternate;

            UISelectionCoordinator coordinator =
                CreateCoordinator(
                    eventSystem);

            coordinator.ApplyOpenSelection(
                surface,
                UIInputModality.Navigation);

            Assert.That(
                eventSystem.currentSelectedGameObject,
                Is.SameAs(alternate));
        }

        [Test]
        public void RememberThisSessionUpdatesAsSoonAsAValidFocusTargetWins()
        {
            EventSystem eventSystem =
                CreateEventSystem("EventSystem");

            UISurface surface =
                CreateSurface(
                    "inventory",
                    UISurfaceRole.Window,
                    UIFocusReopenBehavior.RememberThisSession,
                    out _,
                    out GameObject alternate);

            UISelectionCoordinator coordinator =
                CreateCoordinator(
                    eventSystem);

            coordinator.ApplyOpenSelection(
                surface,
                UIInputModality.Navigation);

            UIFocusRequestResult request =
                coordinator.RequestFocus(
                    surface,
                    alternate,
                    UIInputModality.Navigation,
                    coordinator.Generation);

            Assert.That(request.Succeeded, Is.True);

            Assert.That(
                coordinator.TryGetSessionRememberedFocus(
                    surface.SurfaceId,
                    out GameObject remembered),
                Is.True);

            Assert.That(remembered, Is.SameAs(alternate));

            coordinator.CloseSurface(
                surface);

            coordinator.ApplyOpenSelection(
                surface,
                UIInputModality.Navigation);

            Assert.That(
                eventSystem.currentSelectedGameObject,
                Is.SameAs(alternate));
        }

        private UISelectionCoordinator CreateCoordinator(
            EventSystem eventSystem)
        {
            UISelectionCoordinator coordinator =
                new UISelectionCoordinator();

            coordinator.ConfigureEventSystem(
                eventSystem,
                coordinationConfigured: true);

            return coordinator;
        }

        private EventSystem CreateEventSystem(
            string name)
        {
            GameObject objectInstance =
                CreateObject(name);

            return objectInstance.AddComponent<EventSystem>();
        }

        private UISurface CreateSurface(
            string surfaceId,
            UISurfaceRole role,
            UIFocusReopenBehavior reopenBehavior,
            out GameObject authoredDefault,
            out GameObject alternate)
        {
            GameObject root =
                CreateObject(
                    surfaceId);

            root.AddComponent<CanvasGroup>();

            UISurface surface =
                root.AddComponent<UISurface>();

            SetPrivate(
                surface,
                "surfaceId",
                surfaceId);

            SetPrivate(
                surface,
                "role",
                role);

            authoredDefault =
                CreateChild(
                    root,
                    "Default");

            alternate =
                CreateChild(
                    root,
                    "Alternate");

            SetSelectionPolicy(
                surface,
                new UISurfaceSelectionPolicy(
                    UISelectionOpenBehavior.ClearSelection,
                    UISelectionOpenBehavior.SelectDefault,
                    authoredDefault,
                    reopenBehavior,
                    true));

            return surface;
        }

        private static void SetSelectionPolicy(
            UISurface surface,
            UISurfaceSelectionPolicy policy)
        {
            SetPrivate(
                surface,
                "selectionPolicy",
                policy);
        }

        private GameObject CreateObject(
            string name)
        {
            GameObject objectInstance =
                new GameObject(name);

            created.Add(
                objectInstance);

            return objectInstance;
        }

        private GameObject CreateChild(
            GameObject parent,
            string name)
        {
            GameObject child =
                new GameObject(name);

            child.transform.SetParent(
                parent.transform,
                false);

            return child;
        }

        private static void SetPrivate(
            object instance,
            string fieldName,
            object value)
        {
            FieldInfo field =
                instance.GetType()
                    .GetField(
                        fieldName,
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

            Assert.That(
                field,
                Is.Not.Null,
                "Missing private field " + fieldName);

            field.SetValue(
                instance,
                value);
        }

        private sealed class TestFocusResolver :
            MonoBehaviour,
            IUIFocusTargetResolver
        {
            public GameObject Target { get; set; }

            public GameObject ResolveFocusTarget(
                UISurface surface) =>
                Target;
        }
    }
}
