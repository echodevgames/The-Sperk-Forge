
using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class EchoSaveStorageLifecycleTests
    {
        private GameObject firstObject;
        private GameObject secondObject;
        private EchoSaveConfiguration configuration;
        private string sandboxParent;
        private string backendRoot;

        [SetUp]
        public void SetUp()
        {
            EchoSaveAuthorityClaim.ResetForTesting();

            configuration =
                ScriptableObject.CreateInstance<
                    EchoSaveConfiguration>();

            configuration.SetDefinitionForTesting(
                EchoSaveConfiguration
                    .CurrentSchemaVersion,
                "EchoSave");

            sandboxParent =
                Path.Combine(
                    Path.GetTempPath(),
                    "EchoSave-Lifecycle-" +
                    Guid.NewGuid()
                        .ToString("N"));

            backendRoot =
                Path.Combine(
                    sandboxParent,
                    "Chronicle");
        }

        [TearDown]
        public void TearDown()
        {
            if (secondObject != null)
            {
                UnityEngine.Object.DestroyImmediate(
                    secondObject);
            }

            if (firstObject != null)
            {
                UnityEngine.Object.DestroyImmediate(
                    firstObject);
            }

            if (configuration != null)
            {
                UnityEngine.Object.DestroyImmediate(
                    configuration);
            }

            if (Directory.Exists(
                    sandboxParent))
            {
                Directory.Delete(
                    sandboxParent,
                    true);
            }

            if (File.Exists(
                    sandboxParent))
            {
                File.Delete(
                    sandboxParent);
            }

            EchoSaveAuthorityClaim.ResetForTesting();
        }

        [Test]
        public void ProductionRootResolvesAsConfiguredChild()
        {
            string persistentRoot =
                Path.Combine(
                    Path.GetTempPath(),
                    "EchoSave-Persistent-" +
                    Guid.NewGuid()
                        .ToString("N"));

            SaveStorageResult result =
                SaveStorageRootResolver
                    .TryResolveProductionRoot(
                        configuration,
                        Path.GetFullPath(
                            persistentRoot),
                        out string resolvedRoot);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                resolvedRoot,
                Is.EqualTo(
                    Path.GetFullPath(
                        Path.Combine(
                            persistentRoot,
                            "EchoSave"))));
        }

        [Test]
        public void InjectedSandboxBackendInitializesThroughChronicle()
        {
            EchoSaveRoot root =
                CreateRoot(
                    ref firstObject);

            root.SetConfigurationForTesting(
                configuration);

            root.SetStorageBackendFactoryForTesting(
                new FixedRootBackendFactory(
                    backendRoot));

            Assert.That(
                Directory.Exists(
                    backendRoot),
                Is.False);

            EchoSaveLifecycleResult result =
                root.InitializeSynchronouslyForTesting();

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                Directory.Exists(
                    backendRoot),
                Is.True);

            Assert.That(
                root.StorageBackendForTesting,
                Is.TypeOf<
                    LocalFileSaveStorageBackend>());
        }

        [Test]
        public void DuplicateCannotCreateStorageRoot()
        {
            EchoSaveRoot first =
                CreateRoot(
                    ref firstObject);

            first.SetConfigurationForTesting(
                configuration);

            first.SetStorageBackendFactoryForTesting(
                new FixedRootBackendFactory(
                    backendRoot));

            LogAssert.Expect(
                LogType.Warning,
                new System.Text.RegularExpressions.Regex(
                    @"\[ESV-LIFE-001\].*Duplicate EchoSaveRoot rejected"));

            EchoSaveRoot duplicate =
                CreateRoot(
                    ref secondObject);

            EchoSaveLifecycleResult duplicateResult =
                duplicate
                    .InitializeSynchronouslyForTesting();

            Assert.That(
                duplicateResult.Status,
                Is.EqualTo(
                    EchoSaveLifecycleStatus.Rejected));

            Assert.That(
                Directory.Exists(
                    backendRoot),
                Is.False);
        }

        [Test]
        public void BackendInitializationFailureBlocksChronicle()
        {
            Directory.CreateDirectory(
                sandboxParent);

            File.WriteAllText(
                backendRoot,
                "This file intentionally blocks directory creation.");

            EchoSaveRoot root =
                CreateRoot(
                    ref firstObject);

            root.SetConfigurationForTesting(
                configuration);

            root.SetStorageBackendFactoryForTesting(
                new FixedRootBackendFactory(
                    backendRoot));

            EchoSaveLifecycleResult result =
                root.InitializeSynchronouslyForTesting();

            Assert.That(
                result.Status,
                Is.EqualTo(
                    EchoSaveLifecycleStatus.Blocked));

            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo(
                    "ESV-STORAGE-002"));

            Assert.That(
                root.State,
                Is.EqualTo(
                    EchoSaveServiceState.Blocked));
        }

        private static EchoSaveRoot CreateRoot(
            ref GameObject owner)
        {
            owner =
                new GameObject(
                    "EchoSave Storage Lifecycle Test");

            EchoSaveRoot root =
                owner.AddComponent<
                    EchoSaveRoot>();

            root.EnsureAuthorityClaimedForTesting();

            return root;
        }

        private sealed class FixedRootBackendFactory :
            IEchoSaveStorageBackendFactory
        {
            private readonly string rootPath;

            internal FixedRootBackendFactory(
                string rootPath)
            {
                this.rootPath =
                    rootPath;
            }

            public SaveStorageResult TryCreate(
                EchoSaveConfiguration config,
                out ISaveStorageBackend backend)
            {
                backend =
                    new LocalFileSaveStorageBackend(
                        rootPath);

                return SaveStorageResult.Success(
                    "Sandbox Chronicle backend created.");
            }
        }
    }
}
