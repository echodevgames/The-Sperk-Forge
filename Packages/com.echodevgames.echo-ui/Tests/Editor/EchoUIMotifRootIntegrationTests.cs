using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUIMotifRootIntegrationTests
    {
        private static readonly FieldInfo ActiveRootField =
            typeof(EchoUIRoot).GetField("active", BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly MethodInfo TryClaimAuthorityMethod =
            typeof(EchoUIRoot).GetMethod("TryClaimAuthority", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly MethodInfo LateUpdateMethod =
            typeof(EchoUIRoot).GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.NonPublic);

        private readonly List<UnityEngine.Object> created =
            new List<UnityEngine.Object>();
        private EchoUIRoot previousActive;
        private GameObject rootObject;
        private EchoUIRoot root;

        [SetUp]
        public void SetUp()
        {
            previousActive = EchoUIRoot.Active;
            ActiveRootField.SetValue(null, null);
            rootObject = new GameObject("motif-root");
            root = rootObject.AddComponent<EchoUIRoot>();
            TryClaimAuthorityMethod.Invoke(root, null);
        }

        [TearDown]
        public void TearDown()
        {
            if (rootObject != null) UnityEngine.Object.DestroyImmediate(rootObject);
            for (int i = created.Count - 1; i >= 0; i--)
                if (created[i] != null) UnityEngine.Object.DestroyImmediate(created[i]);
            created.Clear();
            ActiveRootField.SetValue(null, previousActive);
        }

        [Test]
        public void MissingCatalogPreservesBackwardCompatibleRootInitialization()
        {
            UISurfaceOperationResult result = root.Initialize();

            Assert.That(result.Succeeded, Is.True);
            Assert.That(root.IsInitialized, Is.True);
            Assert.That(root.IsMotifLifecycleInitialized, Is.False);
            Assert.That(root.IsNotificationLifecycleInitialized, Is.True);
            Assert.That(root.IsHudLifecycleInitialized, Is.True);
        }

        [Test]
        public void AuthoredCatalogInitializesAtDefaultMotif()
        {
            ConfigureCatalog(CreateCatalog());

            Assert.That(root.Initialize().Succeeded, Is.True);
            Assert.That(root.IsMotifLifecycleInitialized, Is.True);
            Assert.That(root.EffectiveMotifId.Value, Is.EqualTo("motif.first"));
            Assert.That(root.RegisteredMotifTargetCount, Is.Zero);
            Assert.That(root.TryGetMotifSnapshot(out UIMotifServiceSnapshot snapshot), Is.True);
            Assert.That(snapshot.Revision, Is.EqualTo(1));
        }

        [Test]
        public void InvalidCatalogRejectsBeforeRootLifecycleCommit()
        {
            UIMotifDefinition first = CreateDefinition("motif.same", Color.red);
            UIMotifDefinition duplicate = CreateDefinition(" motif.same ", Color.blue);
            ConfigureCatalog(Track(UIMotifCatalog.CreateTransient(
                "motif.same", "", new[] { first, duplicate })));

            UISurfaceOperationResult result = root.Initialize();

            Assert.That(result.Status, Is.EqualTo(UISurfaceOperationStatus.InvalidDefinition));
            Assert.That(root.IsInitialized, Is.False);
            Assert.That(root.RegisteredSurfaceCount, Is.Zero);
            Assert.That(root.IsMotifLifecycleInitialized, Is.False);
            Assert.That(root.IsNotificationLifecycleInitialized, Is.False);
            Assert.That(root.IsHudLifecycleInitialized, Is.False);
        }

        [Test]
        public void InvalidRootMotifCapacitiesRejectInitialization()
        {
            ConfigureCatalog(CreateCatalog());
            SetPrivateField("motifDefinitionCapacity", 0);

            UISurfaceOperationResult result = root.Initialize();

            Assert.That(result.Status, Is.EqualTo(UISurfaceOperationStatus.InvalidDefinition));
            Assert.That(root.IsInitialized, Is.False);
        }

        [Test]
        public void OperationsRejectBeforeMotifInitialization()
        {
            UIMotifSwitchResult switched = root.SwitchMotif(new UIMotifId("motif.first"));
            UIMotifSwitchResult reset = root.ResetMotif();
            UIMotifRegistrationHandle registration = root.RegisterMotifTarget(new RecordingTarget());

            Assert.That(switched.Status, Is.EqualTo(UIMotifSwitchStatus.Unavailable));
            Assert.That(reset.Status, Is.EqualTo(UIMotifSwitchStatus.Unavailable));
            Assert.That(registration.Result.Status, Is.EqualTo(UIMotifRegistrationStatus.Unavailable));
            Assert.That(root.TryGetMotifSnapshot(out _), Is.False);
        }

        [Test]
        public void RootForwardsSwitchFallbackStatusAndEvents()
        {
            ConfigureCatalog(CreateCatalog());
            Assert.That(root.Initialize().Succeeded, Is.True);
            List<UIMotifServiceSnapshot> observed = new List<UIMotifServiceSnapshot>();
            root.MotifChanged += observed.Add;

            UIMotifSwitchResult switched = root.SwitchMotif(new UIMotifId("motif.missing"));

            Assert.That(switched.Status, Is.EqualTo(UIMotifSwitchStatus.FallbackApplied));
            Assert.That(root.EffectiveMotifId.Value, Is.EqualTo("motif.second"));
            Assert.That(observed.Count, Is.EqualTo(1));
            Assert.That(observed[0].EffectiveMotifId, Is.EqualTo(root.EffectiveMotifId));
        }

        [Test]
        public void RootRegistersTargetAndSwitchesAppliedTruth()
        {
            ConfigureCatalog(CreateCatalog());
            Assert.That(root.Initialize().Succeeded, Is.True);
            RecordingTarget target = new RecordingTarget();

            UIMotifRegistrationHandle handle = root.RegisterMotifTarget(target);
            UIMotifSwitchResult switched = root.SwitchMotif(new UIMotifId("motif.second"));

            Assert.That(handle.Result.Succeeded, Is.True);
            Assert.That(target.Applications, Is.EqualTo(2));
            Assert.That(target.LastMotifId.Value, Is.EqualTo("motif.second"));
            Assert.That(switched.AppliedTargetCount, Is.EqualTo(1));
            Assert.That(root.RegisteredMotifTargetCount, Is.EqualTo(1));
        }

        [Test]
        public void RootResetRestoresAuthoredDefault()
        {
            ConfigureCatalog(CreateCatalog());
            root.Initialize();
            root.SwitchMotif(new UIMotifId("motif.second"));

            UIMotifSwitchResult result = root.ResetMotif();

            Assert.That(result.Status, Is.EqualTo(UIMotifSwitchStatus.Applied));
            Assert.That(root.EffectiveMotifId.Value, Is.EqualTo("motif.first"));
        }

        [Test]
        public void LateUpdatePrunesDestroyedMotifOwners()
        {
            ConfigureCatalog(CreateCatalog());
            root.Initialize();
            GameObject owner = Track(new GameObject("motif-owner"));
            root.RegisterMotifTarget(new RecordingTarget(), owner);
            UnityEngine.Object.DestroyImmediate(owner);

            LateUpdateMethod.Invoke(root, null);

            Assert.That(root.RegisteredMotifTargetCount, Is.Zero);
        }

        [Test]
        public void RootListenerFailureIsIsolatedAfterCommittedTruth()
        {
            ConfigureCatalog(CreateCatalog());
            root.Initialize();
            int healthyCalls = 0;
            root.MotifChanged += _ => throw new InvalidOperationException("root-motif-observer");
            root.MotifChanged += _ => healthyCalls++;
            LogAssert.Expect(LogType.Exception, "InvalidOperationException: root-motif-observer");

            UIMotifSwitchResult result = root.SwitchMotif(new UIMotifId("motif.second"));

            Assert.That(result.Succeeded, Is.True);
            Assert.That(healthyCalls, Is.EqualTo(1));
            Assert.That(root.EffectiveMotifId.Value, Is.EqualTo("motif.second"));
        }

        [Test]
        public void RootDestructionShutsDownMotifRegistrations()
        {
            ConfigureCatalog(CreateCatalog());
            root.Initialize();
            UIMotifRegistrationHandle handle = root.RegisterMotifTarget(new RecordingTarget());

            UnityEngine.Object.DestroyImmediate(rootObject);
            rootObject = null;
            UIMotifRegistrationReleaseResult released = handle.Release();

            Assert.That(released.Status, Is.EqualTo(UIMotifRegistrationReleaseStatus.Shutdown));
            Assert.That(EchoUIRoot.Active, Is.Null);
        }

        [Test]
        public void MotifSwitchPreservesRetainedRootAuthorities()
        {
            ConfigureCatalog(CreateCatalog());
            root.Initialize();
            int notificationChannels = root.NotificationChannelCount;
            int hudRegions = root.HudRegionCount;

            root.SwitchMotif(new UIMotifId("motif.second"));

            Assert.That(root.IsInitialized, Is.True);
            Assert.That(root.IsNotificationLifecycleInitialized, Is.True);
            Assert.That(root.NotificationChannelCount, Is.EqualTo(notificationChannels));
            Assert.That(root.IsHudLifecycleInitialized, Is.True);
            Assert.That(root.HudRegionCount, Is.EqualTo(hudRegions));
            Assert.That(root.RegisteredSurfaceCount, Is.Zero);
        }

        private UIMotifCatalog CreateCatalog()
        {
            UIMotifDefinition first = CreateDefinition("motif.first", Color.red);
            UIMotifDefinition second = CreateDefinition("motif.second", Color.blue);
            return Track(UIMotifCatalog.CreateTransient(
                "motif.first", "motif.second", new[] { first, second }));
        }

        private UIMotifDefinition CreateDefinition(string id, Color color) =>
            Track(UIMotifDefinition.CreateTransient(
                id,
                colorTokens: new[]
                {
                    new UIMotifColorToken("color.surface", color)
                }));

        private void ConfigureCatalog(UIMotifCatalog catalog) =>
            SetPrivateField("motifCatalog", catalog);

        private void SetPrivateField(string name, object value) =>
            typeof(EchoUIRoot).GetField(
                name,
                BindingFlags.Instance | BindingFlags.NonPublic).SetValue(root, value);

        private T Track<T>(T value) where T : UnityEngine.Object
        {
            created.Add(value);
            return value;
        }

        private sealed class RecordingTarget : IUIMotifTarget
        {
            public int Applications { get; private set; }
            public UIMotifId LastMotifId { get; private set; }

            public UIMotifTargetApplyResult ApplyMotif(UIMotifSnapshot snapshot)
            {
                Applications++;
                LastMotifId = snapshot.MotifId;
                return new UIMotifTargetApplyResult(
                    UIMotifTargetApplyStatus.Applied,
                    appliedBindingCount: 1);
            }
        }
    }
}
