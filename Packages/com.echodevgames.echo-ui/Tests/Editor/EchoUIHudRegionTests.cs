using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUIHudRegionTests
    {
        private GameObject rootObject;
        private UIHudRegionService service;

        [SetUp]
        public void SetUp()
        {
            rootObject = new GameObject("hud-test-root");
            service = new UIHudRegionService(8, 8);
        }

        [TearDown]
        public void TearDown()
        {
            service?.Shutdown();
            if (rootObject != null)
            {
                Object.DestroyImmediate(rootObject);
            }
        }

        [Test]
        public void RegionRegistrationCreatesStableAddressableSnapshot()
        {
            UIHudRegionHost host = CreateRegion("hud.status", true, 4);
            Assert.That(service.RegisterRegion(host, host, UIHudOwnershipMode.SceneOwned).Succeeded, Is.True);
            Assert.That(service.TryGetSnapshot(new UIHudRegionId("hud.status"), out UIHudRegionSnapshot snapshot), Is.True);
            Assert.That(snapshot.EffectiveVisibility, Is.True);
            Assert.That(snapshot.OwnershipMode, Is.EqualTo(UIHudOwnershipMode.SceneOwned));
        }

        [Test]
        public void DuplicateRegionRejectsWithoutPartialMutation()
        {
            UIHudRegionHost first = CreateRegion("hud.status", true, 4);
            UIHudRegionHost second = CreateRegion("hud.status", false, 4);
            Assert.That(service.RegisterRegion(first).Succeeded, Is.True);
            UIHudOperationResult result = service.RegisterRegion(second);
            Assert.That(result.Status, Is.EqualTo(UIHudOperationStatus.Duplicate));
            Assert.That(service.RegionCount, Is.EqualTo(1));
        }

        [Test]
        public void WidgetRegistrationRequiresHudSurfaceRole()
        {
            UIHudRegionHost host = CreateRegion("hud.status", true, 4);
            service.RegisterRegion(host);
            UISurface screen = CreateSurface("screen", UISurfaceRole.Screen);
            UIHudWidgetHandle handle = service.RegisterWidget(new UIHudRegionId("hud.status"), new UIHudWidgetId("widget.health"), screen);
            Assert.That(handle.Accepted, Is.False);
            Assert.That(handle.LastResult.Status, Is.EqualTo(UIHudOperationStatus.Invalid));
        }

        [Test]
        public void WidgetLeaseIsIdempotentlyDisposable()
        {
            UIHudRegionHost host = CreateRegion("hud.status", true, 4);
            service.RegisterRegion(host);
            UIHudWidgetHandle handle = service.RegisterWidget(new UIHudRegionId("hud.status"), new UIHudWidgetId("widget.health"), CreateSurface("health", UISurfaceRole.Hud));
            Assert.That(handle.Accepted, Is.True);
            Assert.That(handle.Release().Status, Is.EqualTo(UIHudOperationStatus.Completed));
            Assert.That(handle.Release().Status, Is.EqualTo(UIHudOperationStatus.AlreadyReleased));
        }

        [Test]
        public void RegionCapacityRejectsAdditionalWidget()
        {
            UIHudRegionHost host = CreateRegion("hud.status", true, 1);
            service.RegisterRegion(host);
            Assert.That(service.RegisterWidget(new UIHudRegionId("hud.status"), new UIHudWidgetId("one"), CreateSurface("one", UISurfaceRole.Hud)).Accepted, Is.True);
            UIHudWidgetHandle second = service.RegisterWidget(new UIHudRegionId("hud.status"), new UIHudWidgetId("two"), CreateSurface("two", UISurfaceRole.Hud));
            Assert.That(second.LastResult.Status, Is.EqualTo(UIHudOperationStatus.CapacityExceeded));
        }

        [Test]
        public void LatestEqualPriorityVisibilityLeaseWins()
        {
            UIHudRegionHost host = CreateRegion("hud.status", true, 4);
            service.RegisterRegion(host);
            UIHudVisibilityLease hide = service.RequestVisibility(new UIHudRegionId("hud.status"), "pause", false, 10);
            UIHudVisibilityLease show = service.RequestVisibility(new UIHudRegionId("hud.status"), "cinematic-override", true, 10);
            AssertVisibility("hud.status", true);
            show.Release();
            AssertVisibility("hud.status", false);
            hide.Release();
            AssertVisibility("hud.status", true);
        }

        [Test]
        public void HigherPriorityVisibilityLeaseWinsRegardlessOfAge()
        {
            UIHudRegionHost host = CreateRegion("hud.status", true, 4);
            service.RegisterRegion(host);
            UIHudVisibilityLease show = service.RequestVisibility(new UIHudRegionId("hud.status"), "show", true, 20);
            UIHudVisibilityLease hide = service.RequestVisibility(new UIHudRegionId("hud.status"), "hide", false, 10);
            AssertVisibility("hud.status", true);
            show.Release();
            AssertVisibility("hud.status", false);
            hide.Release();
        }

        [Test]
        public void FinalVisibilityReleaseRestoresAuthoredBaseline()
        {
            UIHudRegionHost host = CreateRegion("hud.status", false, 4);
            service.RegisterRegion(host);
            UIHudVisibilityLease show = service.RequestVisibility(new UIHudRegionId("hud.status"), "temporary", true);
            AssertVisibility("hud.status", true);
            show.Release();
            AssertVisibility("hud.status", false);
        }

        [Test]
        public void DestroyedWidgetOwnerCleansOnlyOwnedWidget()
        {
            UIHudRegionHost host = CreateRegion("hud.status", true, 4);
            service.RegisterRegion(host);
            GameObject owner = new GameObject("owner");
            owner.transform.SetParent(rootObject.transform);
            service.RegisterWidget(new UIHudRegionId("hud.status"), new UIHudWidgetId("owned"), CreateSurface("owned", UISurfaceRole.Hud), owner);
            service.RegisterWidget(new UIHudRegionId("hud.status"), new UIHudWidgetId("retained"), CreateSurface("retained", UISurfaceRole.Hud));
            Object.DestroyImmediate(owner);
            service.RefreshDestroyedOwners();
            service.TryGetSnapshot(new UIHudRegionId("hud.status"), out UIHudRegionSnapshot snapshot);
            Assert.That(snapshot.WidgetCount, Is.EqualTo(1));
        }

        [Test]
        public void DestroyedVisibilityOwnerRestoresRemainingTruth()
        {
            UIHudRegionHost host = CreateRegion("hud.status", true, 4);
            service.RegisterRegion(host);
            GameObject owner = new GameObject("visibility-owner");
            owner.transform.SetParent(rootObject.transform);
            service.RequestVisibility(new UIHudRegionId("hud.status"), "owned-hide", false, 50, owner);
            AssertVisibility("hud.status", false);
            Object.DestroyImmediate(owner);
            service.RefreshDestroyedOwners();
            AssertVisibility("hud.status", true);
        }

        [Test]
        public void ListenerFailureDoesNotRollbackCommittedTruth()
        {
            UIHudRegionHost host = CreateRegion("hud.status", true, 4);
            service.RegionChanged += _ => throw new System.InvalidOperationException("observer");
            LogAssert.Expect(LogType.Exception, "InvalidOperationException: observer");
            UIHudOperationResult result = service.RegisterRegion(host);
            Assert.That(result.Succeeded, Is.True);
            Assert.That(service.RegionCount, Is.EqualTo(1));
        }

        [Test]
        public void ShutdownInvalidatesServiceAndClearsBoundedState()
        {
            UIHudRegionHost host = CreateRegion("hud.status", true, 4);
            service.RegisterRegion(host);
            service.RegisterWidget(new UIHudRegionId("hud.status"), new UIHudWidgetId("widget"), CreateSurface("widget", UISurfaceRole.Hud));
            service.RequestVisibility(new UIHudRegionId("hud.status"), "hide", false);
            service.Shutdown();
            Assert.That(service.IsValid, Is.False);
            Assert.That(service.RegionCount, Is.EqualTo(0));
            Assert.That(service.WidgetCount, Is.EqualTo(0));
            Assert.That(service.VisibilityLeaseCount, Is.EqualTo(0));
        }

        private void AssertVisibility(string id, bool expected)
        {
            Assert.That(service.TryGetSnapshot(new UIHudRegionId(id), out UIHudRegionSnapshot snapshot), Is.True);
            Assert.That(snapshot.EffectiveVisibility, Is.EqualTo(expected));
        }

        private UIHudRegionHost CreateRegion(string id, bool visible, int capacity)
        {
            GameObject go = new GameObject(id);
            go.transform.SetParent(rootObject.transform, false);
            UIHudRegionHost host = go.AddComponent<UIHudRegionHost>();
            SerializedObject serialized = new SerializedObject(host);
            SerializedProperty definitionProperty = serialized.FindProperty("definition");
            definitionProperty.FindPropertyRelative("regionId").stringValue = id;
            definitionProperty.FindPropertyRelative("startVisible").boolValue = visible;
            definitionProperty.FindPropertyRelative("widgetCapacity").intValue = capacity;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return host;
        }

        private UISurface CreateSurface(string id, UISurfaceRole role)
        {
            GameObject go = new GameObject(id, typeof(CanvasGroup));
            go.transform.SetParent(rootObject.transform, false);
            UISurface surface = go.AddComponent<UISurface>();
            SerializedObject serialized = new SerializedObject(surface);
            serialized.FindProperty("surfaceId").stringValue = id;
            serialized.FindProperty("role").enumValueIndex = (int)role;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return surface;
        }
    }
}
