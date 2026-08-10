
using System;
using System.IO;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class LocalFileSaveStorageDiscoveryTests
    {
        private string root;
        private LocalFileSaveStorageBackend backend;

        [SetUp]
        public void SetUp()
        {
            root =
                Path.Combine(
                    Path.GetTempPath(),
                    "EchoSaveCatalogDiscovery_" +
                    Guid.NewGuid().ToString("N"));

            backend =
                new LocalFileSaveStorageBackend(
                    root);

            Assert.That(
                backend.Initialize().Succeeded,
                Is.True);
        }

        [TearDown]
        public void TearDown()
        {
            backend?.Shutdown();

            if (!string.IsNullOrEmpty(root) &&
                Directory.Exists(root))
            {
                Directory.Delete(
                    root,
                    true);
            }
        }

        [Test]
        public void MissingParentReturnsParentNotFound()
        {
            SaveStorageKey.TryCreate(
                "slots",
                out SaveStorageKey key);

            SaveStorageDiscoveryResult result =
                backend.DiscoverChildDirectories(
                    key,
                    8);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveStorageDiscoveryStatus.ParentNotFound));

            Assert.That(
                result.ChildNames,
                Is.Empty);
        }

        [Test]
        public void DiscoveryReturnsImmediateDirectoriesOnly()
        {
            Directory.CreateDirectory(
                Path.Combine(
                    root,
                    "slots",
                    "alpha",
                    "nested"));

            Directory.CreateDirectory(
                Path.Combine(
                    root,
                    "slots",
                    "beta"));

            File.WriteAllText(
                Path.Combine(
                    root,
                    "slots",
                    "not-a-directory.txt"),
                "x");

            SaveStorageKey.TryCreate(
                "slots",
                out SaveStorageKey key);

            SaveStorageDiscoveryResult result =
                backend.DiscoverChildDirectories(
                    key,
                    8);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveStorageDiscoveryStatus.Succeeded));

            Assert.That(
                result.ChildNames,
                Is.EquivalentTo(
                    new[]
                    {
                        "alpha",
                        "beta"
                    }));

            Assert.That(
                result.ChildNames,
                Does.Not.Contain(
                    "nested"));
        }

        [Test]
        public void DiscoveryLimitFailsWithoutPartialChildren()
        {
            Directory.CreateDirectory(
                Path.Combine(
                    root,
                    "slots",
                    "a"));

            Directory.CreateDirectory(
                Path.Combine(
                    root,
                    "slots",
                    "b"));

            SaveStorageKey.TryCreate(
                "slots",
                out SaveStorageKey key);

            SaveStorageDiscoveryResult result =
                backend.DiscoverChildDirectories(
                    key,
                    1);

            Assert.That(
                result.Status,
                Is.EqualTo(
                    SaveStorageDiscoveryStatus.LimitExceeded));

            Assert.That(
                result.ChildNames,
                Is.Empty);
        }

        [Test]
        public void InvalidDiscoveryBoundRejects()
        {
            SaveStorageKey.TryCreate(
                "slots",
                out SaveStorageKey key);

            Assert.That(
                backend.DiscoverChildDirectories(
                    key,
                    0)
                    .Status,
                Is.EqualTo(
                    SaveStorageDiscoveryStatus.InvalidRequest));
        }

        [Test]
        public void TraversalCannotBecomeDiscoveryKey()
        {
            SaveStorageResult result =
                SaveStorageKey.TryCreate(
                    "slots/../outside",
                    out _);

            Assert.That(
                result.Succeeded,
                Is.False);
        }
    }
}
