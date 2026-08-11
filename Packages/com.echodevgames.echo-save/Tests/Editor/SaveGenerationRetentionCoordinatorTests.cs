
using NUnit.Framework;

namespace EchoDevGames.EchoSave.Tests.Editor
{
    public sealed class SaveGenerationRetentionCoordinatorTests
    {
        [Test]
        public void InvalidPolicyDeletesNothing()
        {
            using (SaveRetentionTestEnvironment env =
                new SaveRetentionTestEnvironment())
            {
                SaveGenerationId first =
                    env.Generation(1);
                SaveGenerationId current =
                    env.Generation(2);
                env.WriteHead(current, first);

                SaveRetentionResult result =
                    env.Coordinator().Apply(
                        env.SlotId,
                        new SaveRetentionPolicy(1));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveRetentionStatus.InvalidPolicy));
                Assert.That(
                    env.GenerationExists(first),
                    Is.True);
            }
        }

        [Test]
        public void HistoryWithinBoundRequiresNoDeletion()
        {
            using (SaveRetentionTestEnvironment env =
                new SaveRetentionTestEnvironment())
            {
                SaveGenerationId previous =
                    env.Generation(1);
                SaveGenerationId current =
                    env.Generation(2);
                env.WriteHead(current, previous);

                SaveRetentionResult result =
                    env.Coordinator().Apply(
                        env.SlotId,
                        new SaveRetentionPolicy(2));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveRetentionStatus.NotRequired));
                Assert.That(result.DeletedCount, Is.Zero);
            }
        }

        [Test]
        public void ExcessHistoryDeletesOldestFirst()
        {
            using (SaveRetentionTestEnvironment env =
                new SaveRetentionTestEnvironment())
            {
                SaveGenerationId oldest =
                    env.Generation(1);
                SaveGenerationId second =
                    env.Generation(2);
                SaveGenerationId previous =
                    env.Generation(3);
                SaveGenerationId current =
                    env.Generation(4);
                env.WriteHead(current, previous);

                SaveRetentionResult result =
                    env.Coordinator().Apply(
                        env.SlotId,
                        new SaveRetentionPolicy(3));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveRetentionStatus.Completed));
                Assert.That(result.DeletedCount, Is.EqualTo(1));
                Assert.That(
                    env.GenerationExists(oldest),
                    Is.False);
                Assert.That(
                    env.GenerationExists(second),
                    Is.True);
            }
        }

        [Test]
        public void MinimumBoundProtectsCurrentAndImmediatePredecessor()
        {
            using (SaveRetentionTestEnvironment env =
                new SaveRetentionTestEnvironment())
            {
                SaveGenerationId oldest =
                    env.Generation(1);
                SaveGenerationId second =
                    env.Generation(2);
                SaveGenerationId previous =
                    env.Generation(3);
                SaveGenerationId current =
                    env.Generation(4);
                env.WriteHead(current, previous);

                SaveRetentionResult result =
                    env.Coordinator().Apply(
                        env.SlotId,
                        new SaveRetentionPolicy(2));

                Assert.That(result.Succeeded, Is.True);
                Assert.That(result.DeletedCount, Is.EqualTo(2));
                Assert.That(env.GenerationExists(oldest), Is.False);
                Assert.That(env.GenerationExists(second), Is.False);
                Assert.That(env.GenerationExists(previous), Is.True);
                Assert.That(env.GenerationExists(current), Is.True);
            }
        }

        [Test]
        public void NonCanonicalGenerationChildIsPreserved()
        {
            using (SaveRetentionTestEnvironment env =
                new SaveRetentionTestEnvironment())
            {
                SaveGenerationId previous =
                    env.Generation(1);
                SaveGenerationId current =
                    env.Generation(2);
                env.CreateNonCanonicalGenerationChild(
                    "support-notes");
                env.WriteHead(current, previous);

                SaveRetentionResult result =
                    env.Coordinator().Apply(
                        env.SlotId,
                        new SaveRetentionPolicy(2));

                Assert.That(result.Succeeded, Is.True);
                Assert.That(
                    System.IO.Directory.Exists(
                        System.IO.Path.Combine(
                            env.Local.RootPath,
                            "slots",
                            env.SlotId.Value,
                            "generations",
                            "support-notes")),
                    Is.True);
            }
        }

        [Test]
        public void MalformedCanonicalManifestFailsClosed()
        {
            using (SaveRetentionTestEnvironment env =
                new SaveRetentionTestEnvironment())
            {
                SaveGenerationId malformed =
                    env.Generation(
                        1,
                        rawManifestOverride:
                            "{ definitely-not-json }");
                SaveGenerationId previous =
                    env.Generation(2);
                SaveGenerationId current =
                    env.Generation(3);
                env.WriteHead(current, previous);

                SaveRetentionResult result =
                    env.Coordinator().Apply(
                        env.SlotId,
                        new SaveRetentionPolicy(2));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveRetentionStatus.Untrustworthy));
                Assert.That(
                    env.GenerationExists(malformed),
                    Is.True);
                Assert.That(
                    env.GenerationExists(previous),
                    Is.True);
            }
        }

        [Test]
        public void UncommittedManifestFailsClosed()
        {
            using (SaveRetentionTestEnvironment env =
                new SaveRetentionTestEnvironment())
            {
                SaveGenerationId candidate =
                    env.Generation(
                        1,
                        SaveGenerationCommitState.Candidate);
                SaveGenerationId previous =
                    env.Generation(2);
                SaveGenerationId current =
                    env.Generation(3);
                env.WriteHead(current, previous);

                SaveRetentionResult result =
                    env.Coordinator().Apply(
                        env.SlotId,
                        new SaveRetentionPolicy(2));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveRetentionStatus.Untrustworthy));
                Assert.That(
                    env.GenerationExists(candidate),
                    Is.True);
            }
        }

        [Test]
        public void MismatchedManifestSlotFailsClosed()
        {
            using (SaveRetentionTestEnvironment env =
                new SaveRetentionTestEnvironment())
            {
                SaveGenerationId mismatch =
                    env.Generation(
                        1,
                        manifestSlotOverride:
                            SaveSlotId.NewId());
                SaveGenerationId previous =
                    env.Generation(2);
                SaveGenerationId current =
                    env.Generation(3);
                env.WriteHead(current, previous);

                SaveRetentionResult result =
                    env.Coordinator().Apply(
                        env.SlotId,
                        new SaveRetentionPolicy(2));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveRetentionStatus.Untrustworthy));
                Assert.That(
                    env.GenerationExists(mismatch),
                    Is.True);
            }
        }

        [Test]
        public void DiscoveryLimitFailureDeletesNothing()
        {
            using (SaveRetentionTestEnvironment env =
                new SaveRetentionTestEnvironment())
            {
                SaveGenerationId first =
                    env.Generation(1);
                SaveGenerationId previous =
                    env.Generation(2);
                SaveGenerationId current =
                    env.Generation(3);
                env.WriteHead(current, previous);

                SaveRetentionResult result =
                    env.Coordinator(
                        discoveryLimit: 2)
                        .Apply(
                            env.SlotId,
                            new SaveRetentionPolicy(2));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveRetentionStatus.Untrustworthy));
                Assert.That(env.GenerationExists(first), Is.True);
            }
        }

        [Test]
        public void MissingTreeDeletionCapabilityReportsUnsupported()
        {
            using (SaveRetentionTestEnvironment env =
                new SaveRetentionTestEnvironment())
            {
                SaveGenerationId oldest =
                    env.Generation(1);
                SaveGenerationId previous =
                    env.Generation(2);
                SaveGenerationId current =
                    env.Generation(3);
                env.WriteHead(current, previous);

                DiscoveryOnlyRetentionBackend backend =
                    new DiscoveryOnlyRetentionBackend(
                        env.Local);

                SaveRetentionResult result =
                    env.Coordinator(backend)
                        .Apply(
                            env.SlotId,
                            new SaveRetentionPolicy(2));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveRetentionStatus.UnsupportedStorage));
                Assert.That(
                    env.GenerationExists(oldest),
                    Is.True);
            }
        }

        [Test]
        public void TreeDeleteFailureNeverTouchesProtectedHistory()
        {
            using (SaveRetentionTestEnvironment env =
                new SaveRetentionTestEnvironment())
            {
                SaveGenerationId oldest =
                    env.Generation(1);
                SaveGenerationId previous =
                    env.Generation(2);
                SaveGenerationId current =
                    env.Generation(3);
                env.WriteHead(current, previous);

                FailingTreeDeletionRetentionBackend backend =
                    new FailingTreeDeletionRetentionBackend(
                        env.Local);

                SaveRetentionResult result =
                    env.Coordinator(backend)
                        .Apply(
                            env.SlotId,
                            new SaveRetentionPolicy(2));

                Assert.That(
                    result.Status,
                    Is.EqualTo(
                        SaveRetentionStatus.Failed));
                Assert.That(backend.DeleteTreeCalls, Is.EqualTo(1));
                Assert.That(env.GenerationExists(oldest), Is.True);
                Assert.That(env.GenerationExists(previous), Is.True);
                Assert.That(env.GenerationExists(current), Is.True);
            }
        }
    }
}
