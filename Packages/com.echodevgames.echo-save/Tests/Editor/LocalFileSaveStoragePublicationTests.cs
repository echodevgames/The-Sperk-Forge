
using System;
using System.IO;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class LocalFileSaveStoragePublicationTests
    {
        private string sandboxParent;
        private string backendRoot;

        [SetUp]
        public void SetUp()
        {
            sandboxParent =
                Path.Combine(
                    Path.GetTempPath(),
                    "EchoSave-M2-04-Storage-" +
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
        public void CapabilitiesAreExplicitWithoutUniversalAtomicityClaim()
        {
            LocalFileSaveStorageBackend backend =
                CreateBackend();

            SaveStoragePublicationCapabilities capabilities =
                backend.PublicationCapabilities;

            Assert.That(
                capabilities.SupportsNewTreePublication,
                Is.True);

            Assert.That(
                capabilities.SupportsCurrentObjectPublication,
                Is.True);

            Assert.That(
                capabilities.UsesSameRootDirectoryMove,
                Is.True);

            Assert.That(
                capabilities.UsesNativeReplaceForExistingCurrent,
                Is.True);

            Assert.That(
                capabilities.ClaimsPowerLossAtomicity,
                Is.False);
        }

        [Test]
        public void PublishNewTreeMovesCandidateAndPreservesBytes()
        {
            LocalFileSaveStorageBackend backend =
                CreateBackend();

            SaveStorageKey.TryCreate(
                "objects/incomplete/g1/payload.bin",
                out SaveStorageKey candidatePayload);

            SaveStorageKey.TryCreate(
                "objects/incomplete/g1",
                out SaveStorageKey candidateDirectory);

            SaveStorageKey.TryCreate(
                "objects/generations/g1",
                out SaveStorageKey generationDirectory);

            SaveStorageKey.TryCreate(
                "objects/generations/g1/payload.bin",
                out SaveStorageKey finalPayload);

            byte[] expected =
                { 1, 3, 3, 7 };

            Assert.That(
                backend.WriteNew(
                    candidatePayload,
                    expected)
                    .Succeeded,
                Is.True);

            SaveStorageResult published =
                backend.PublishNewTree(
                    candidateDirectory,
                    generationDirectory);

            SaveStorageReadResult read =
                backend.Read(
                    finalPayload);

            Assert.That(
                published.Succeeded,
                Is.True);

            Assert.That(
                read.Data,
                Is.EqualTo(
                    expected));

            Assert.That(
                Directory.Exists(
                    Path.Combine(
                        backendRoot,
                        "objects",
                        "incomplete",
                        "g1")),
                Is.False);
        }

        [Test]
        public void DuplicatePublicationDestinationIsRejected()
        {
            LocalFileSaveStorageBackend backend =
                CreateBackend();

            SaveStorageKey.TryCreate(
                "objects/incomplete/g1/payload.bin",
                out SaveStorageKey firstCandidatePayload);

            SaveStorageKey.TryCreate(
                "objects/incomplete/g1",
                out SaveStorageKey firstCandidateDirectory);

            SaveStorageKey.TryCreate(
                "objects/generations/g1",
                out SaveStorageKey generationDirectory);

            Assert.That(
                backend.WriteNew(
                    firstCandidatePayload,
                    new byte[] { 1 })
                    .Succeeded,
                Is.True);

            Assert.That(
                backend.PublishNewTree(
                    firstCandidateDirectory,
                    generationDirectory)
                    .Succeeded,
                Is.True);

            SaveStorageKey.TryCreate(
                "objects/incomplete/g1/second.bin",
                out SaveStorageKey secondCandidatePayload);

            Assert.That(
                backend.WriteNew(
                    secondCandidatePayload,
                    new byte[] { 2 })
                    .Succeeded,
                Is.True);

            SaveStorageResult conflict =
                backend.PublishNewTree(
                    firstCandidateDirectory,
                    generationDirectory);

            Assert.That(
                conflict.Status,
                Is.EqualTo(
                    SaveStorageStatus.Conflict));
        }

        [Test]
        public void PublishCurrentObjectCreatesFirstCurrentFile()
        {
            LocalFileSaveStorageBackend backend =
                CreateBackend();

            SaveStorageKey.TryCreate(
                "slots/test/head.json",
                out SaveStorageKey head);

            byte[] expected =
                { 10, 20, 30 };

            SaveStorageResult result =
                backend.PublishCurrentObject(
                    head,
                    expected);

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                backend.Read(
                    head)
                    .Data,
                Is.EqualTo(
                    expected));
        }

        [Test]
        public void PublishCurrentObjectReplacesExistingCurrentFile()
        {
            LocalFileSaveStorageBackend backend =
                CreateBackend();

            SaveStorageKey.TryCreate(
                "slots/test/head.json",
                out SaveStorageKey head);

            Assert.That(
                backend.PublishCurrentObject(
                    head,
                    new byte[] { 1 })
                    .Succeeded,
                Is.True);

            SaveStorageResult second =
                backend.PublishCurrentObject(
                    head,
                    new byte[] { 2, 3 });

            Assert.That(
                second.Succeeded,
                Is.True);

            Assert.That(
                backend.Read(
                    head)
                    .Data,
                Is.EqualTo(
                    new byte[] { 2, 3 }));
        }

        private LocalFileSaveStorageBackend CreateBackend()
        {
            LocalFileSaveStorageBackend backend =
                new LocalFileSaveStorageBackend(
                    backendRoot);

            Assert.That(
                backend.Initialize()
                    .Succeeded,
                Is.True);

            return backend;
        }
    }
}
