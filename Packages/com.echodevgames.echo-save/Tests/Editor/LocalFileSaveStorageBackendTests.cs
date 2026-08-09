
using System;
using System.IO;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class LocalFileSaveStorageBackendTests
    {
        private string sandboxParent;
        private string backendRoot;

        [SetUp]
        public void SetUp()
        {
            sandboxParent =
                Path.Combine(
                    Path.GetTempPath(),
                    "EchoSave-M2-" +
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
        }

        [Test]
        public void InitializeCreatesOnlyConfiguredSandboxRoot()
        {
            LocalFileSaveStorageBackend backend =
                new LocalFileSaveStorageBackend(
                    backendRoot);

            Assert.That(
                Directory.Exists(
                    backendRoot),
                Is.False);

            SaveStorageResult result =
                backend.Initialize();

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                Directory.Exists(
                    backendRoot),
                Is.True);
        }

        [Test]
        public void WriteAndReadRoundTripPreservesExactBytes()
        {
            LocalFileSaveStorageBackend backend =
                CreateInitializedBackend();

            SaveStorageKey.TryCreate(
                "objects/test.bin",
                out SaveStorageKey key);

            byte[] expected =
                {
                    0,
                    1,
                    2,
                    3,
                    127,
                    128,
                    254,
                    255
                };

            SaveStorageResult write =
                backend.WriteNew(
                    key,
                    expected);

            SaveStorageReadResult read =
                backend.Read(
                    key);

            Assert.That(
                write.Succeeded,
                Is.True);

            Assert.That(
                read.Succeeded,
                Is.True);

            Assert.That(
                read.Data,
                Is.EqualTo(
                    expected));
        }

        [Test]
        public void MissingReadReturnsStructuredNotFound()
        {
            LocalFileSaveStorageBackend backend =
                CreateInitializedBackend();

            SaveStorageKey.TryCreate(
                "missing.bin",
                out SaveStorageKey key);

            SaveStorageReadResult read =
                backend.Read(
                    key);

            Assert.That(
                read.Result.Status,
                Is.EqualTo(
                    SaveStorageStatus.NotFound));

            Assert.That(
                read.Result.DiagnosticCode,
                Is.EqualTo(
                    "ESV-STORAGE-003"));

            Assert.That(
                read.Data,
                Is.Empty);
        }

        [Test]
        public void CreateOnlyConflictPreservesExistingBytes()
        {
            LocalFileSaveStorageBackend backend =
                CreateInitializedBackend();

            SaveStorageKey.TryCreate(
                "objects/existing.bin",
                out SaveStorageKey key);

            byte[] first =
                {
                    10,
                    20,
                    30
                };

            byte[] second =
                {
                    99,
                    98,
                    97
                };

            Assert.That(
                backend.WriteNew(
                    key,
                    first)
                    .Succeeded,
                Is.True);

            SaveStorageResult conflict =
                backend.WriteNew(
                    key,
                    second);

            SaveStorageReadResult read =
                backend.Read(
                    key);

            Assert.That(
                conflict.Status,
                Is.EqualTo(
                    SaveStorageStatus.Conflict));

            Assert.That(
                conflict.DiagnosticCode,
                Is.EqualTo(
                    "ESV-STORAGE-004"));

            Assert.That(
                read.Data,
                Is.EqualTo(
                    first));
        }

        [Test]
        public void OperationsBeforeInitializeAreRejected()
        {
            LocalFileSaveStorageBackend backend =
                new LocalFileSaveStorageBackend(
                    backendRoot);

            SaveStorageKey.TryCreate(
                "object.bin",
                out SaveStorageKey key);

            SaveStorageReadResult read =
                backend.Read(
                    key);

            Assert.That(
                read.Result.DiagnosticCode,
                Is.EqualTo(
                    "ESV-STORAGE-006"));

            Assert.That(
                Directory.Exists(
                    backendRoot),
                Is.False);
        }

        [Test]
        public void BackendInitializationFailureIsStructured()
        {
            Directory.CreateDirectory(
                sandboxParent);

            File.WriteAllText(
                backendRoot,
                "This file intentionally blocks directory creation.");

            LocalFileSaveStorageBackend backend =
                new LocalFileSaveStorageBackend(
                    backendRoot);

            SaveStorageResult result =
                backend.Initialize();

            Assert.That(
                result.Succeeded,
                Is.False);

            Assert.That(
                result.DiagnosticCode,
                Is.EqualTo(
                    "ESV-STORAGE-002"));
        }

        private LocalFileSaveStorageBackend
            CreateInitializedBackend()
        {
            LocalFileSaveStorageBackend backend =
                new LocalFileSaveStorageBackend(
                    backendRoot);

            SaveStorageResult initialized =
                backend.Initialize();

            Assert.That(
                initialized.Succeeded,
                Is.True);

            return backend;
        }
    }
}
