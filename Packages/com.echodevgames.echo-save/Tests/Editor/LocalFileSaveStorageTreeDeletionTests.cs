
using System;
using System.IO;
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class LocalFileSaveStorageTreeDeletionTests
    {
        [Test]
        public void DeleteTreeRemovesExactNestedTreeOnly()
        {
            string parent =
                Path.Combine(
                    Path.GetTempPath(),
                    "EchoSave-TreeDelete-" +
                    Guid.NewGuid().ToString("N"));

            try
            {
                LocalFileSaveStorageBackend backend =
                    new LocalFileSaveStorageBackend(
                        Path.Combine(
                            parent,
                            "Chronicle"));

                Assert.That(
                    backend.Initialize().Succeeded,
                    Is.True);

                SaveStorageKey.TryCreate(
                    "slots/a/generations/delete-me/manifest.json",
                    out SaveStorageKey nestedFile);

                SaveStorageKey.TryCreate(
                    "slots/a/generations/keep-me/manifest.json",
                    out SaveStorageKey siblingFile);

                Assert.That(
                    backend.WriteNew(
                        nestedFile,
                        new byte[] { 1 })
                        .Succeeded,
                    Is.True);

                Assert.That(
                    backend.WriteNew(
                        siblingFile,
                        new byte[] { 2 })
                        .Succeeded,
                    Is.True);

                SaveStorageKey.TryCreate(
                    "slots/a/generations/delete-me",
                    out SaveStorageKey tree);

                Assert.That(
                    backend.DeleteTree(tree).Succeeded,
                    Is.True);

                Assert.That(
                    backend.Read(nestedFile).Succeeded,
                    Is.False);

                Assert.That(
                    backend.Read(siblingFile).Succeeded,
                    Is.True);
            }
            finally
            {
                if (Directory.Exists(parent))
                {
                    Directory.Delete(parent, true);
                }
            }
        }

        [Test]
        public void MissingTreeReturnsNotFound()
        {
            string parent =
                Path.Combine(
                    Path.GetTempPath(),
                    "EchoSave-TreeDelete-" +
                    Guid.NewGuid().ToString("N"));

            try
            {
                LocalFileSaveStorageBackend backend =
                    new LocalFileSaveStorageBackend(
                        Path.Combine(
                            parent,
                            "Chronicle"));

                Assert.That(
                    backend.Initialize().Succeeded,
                    Is.True);

                SaveStorageKey.TryCreate(
                    "slots/a/generations/missing",
                    out SaveStorageKey tree);

                SaveStorageResult result =
                    backend.DeleteTree(tree);

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveStorageStatus.NotFound));
            }
            finally
            {
                if (Directory.Exists(parent))
                {
                    Directory.Delete(parent, true);
                }
            }
        }
    }
}
