using System;
using EchoDevGames.EchoLaunch.Editor.Setup;
using NUnit.Framework;
using UnityEditor;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Setup
{
    public sealed class EchoLaunchSetupBuildSettingsWriterTests
    {
        private EditorBuildSettingsScene[] original;

        [SetUp]
        public void SetUp()
        {
            original =
                EchoLaunchSetupBuildSettingsWriter.Clone(
                    EditorBuildSettings.scenes);
        }

        [TearDown]
        public void TearDown()
        {
            EditorBuildSettings.scenes =
                EchoLaunchSetupBuildSettingsWriter.Clone(original);
        }

        [Test]
        public void DoNotChangePerformsNoWrite()
        {
            EditorBuildSettings.scenes =
                new[]
                {
                    new EditorBuildSettingsScene(
                        "Assets/A.unity",
                        true)
                };

            bool changed =
                Apply(
                    EchoLaunchBuildSettingsPolicy.DoNotChange,
                    "Assets/Boot.unity",
                    false);

            Assert.That(changed, Is.False);
            Assert.That(EditorBuildSettings.scenes.Length, Is.EqualTo(1));
        }

        [Test]
        public void AppendAddsOneEnabledEntry()
        {
            EditorBuildSettings.scenes =
                new[]
                {
                    new EditorBuildSettingsScene(
                        "Assets/A.unity",
                        false)
                };

            bool changed =
                Apply(
                    EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd,
                    "Assets/Boot.unity",
                    false);

            Assert.That(changed, Is.True);
            Assert.That(EditorBuildSettings.scenes.Length, Is.EqualTo(2));
            Assert.That(EditorBuildSettings.scenes[1].enabled, Is.True);
        }

        [Test]
        public void AppendPreservesExistingEntry()
        {
            EditorBuildSettings.scenes =
                new[]
                {
                    new EditorBuildSettingsScene(
                        "Assets/A.unity",
                        false)
                };

            Apply(
                EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd,
                "Assets/Boot.unity",
                false);

            Assert.That(
                EditorBuildSettings.scenes[0].path,
                Is.EqualTo("Assets/A.unity"));

            Assert.That(
                EditorBuildSettings.scenes[0].enabled,
                Is.False);
        }

        [Test]
        public void ExistingBootProducesNoChange()
        {
            EditorBuildSettings.scenes =
                new[]
                {
                    new EditorBuildSettingsScene(
                        "Assets/Boot.unity",
                        true)
                };

            bool changed =
                Apply(
                    EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd,
                    "Assets/Boot.unity",
                    false);

            Assert.That(changed, Is.False);
            Assert.That(EditorBuildSettings.scenes.Length, Is.EqualTo(1));
        }

        [Test]
        public void PlaceFirstWithoutApprovalThrows()
        {
            Assert.That(
                delegate
                {
                    Apply(
                        EchoLaunchBuildSettingsPolicy.PlaceFirstAfterApproval,
                        "Assets/Boot.unity",
                        false);
                },
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void PlaceFirstInsertsBootAtZero()
        {
            EditorBuildSettings.scenes =
                new[]
                {
                    new EditorBuildSettingsScene(
                        "Assets/A.unity",
                        true)
                };

            Apply(
                EchoLaunchBuildSettingsPolicy.PlaceFirstAfterApproval,
                "Assets/Boot.unity",
                true);

            Assert.That(
                EditorBuildSettings.scenes[0].path,
                Is.EqualTo("Assets/Boot.unity"));
        }

        [Test]
        public void PlaceFirstRemovesDuplicateBootEntries()
        {
            EditorBuildSettings.scenes =
                new[]
                {
                    new EditorBuildSettingsScene(
                        "Assets/Boot.unity",
                        false),
                    new EditorBuildSettingsScene(
                        "Assets/A.unity",
                        true),
                    new EditorBuildSettingsScene(
                        "Assets/Boot.unity",
                        true)
                };

            Apply(
                EchoLaunchBuildSettingsPolicy.PlaceFirstAfterApproval,
                "Assets/Boot.unity",
                true);

            int count = 0;

            for (int index = 0;
                 index < EditorBuildSettings.scenes.Length;
                 index++)
            {
                if (EditorBuildSettings.scenes[index].path ==
                    "Assets/Boot.unity")
                {
                    count++;
                }
            }

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void PlaceFirstPreservesUnrelatedOrderAndEnabledState()
        {
            EditorBuildSettings.scenes =
                new[]
                {
                    new EditorBuildSettingsScene(
                        "Assets/A.unity",
                        false),
                    new EditorBuildSettingsScene(
                        "Assets/Boot.unity",
                        false),
                    new EditorBuildSettingsScene(
                        "Assets/B.unity",
                        true)
                };

            Apply(
                EchoLaunchBuildSettingsPolicy.PlaceFirstAfterApproval,
                "Assets/Boot.unity",
                true);

            Assert.That(
                EditorBuildSettings.scenes[1].path,
                Is.EqualTo("Assets/A.unity"));

            Assert.That(
                EditorBuildSettings.scenes[1].enabled,
                Is.False);

            Assert.That(
                EditorBuildSettings.scenes[2].path,
                Is.EqualTo("Assets/B.unity"));

            Assert.That(
                EditorBuildSettings.scenes[2].enabled,
                Is.True);
        }

        [Test]
        public void CloneIsDefensive()
        {
            EditorBuildSettingsScene[] source =
            {
                new EditorBuildSettingsScene(
                    "Assets/A.unity",
                    true)
            };

            EditorBuildSettingsScene[] clone =
                EchoLaunchSetupBuildSettingsWriter.Clone(source);

            source[0] =
                new EditorBuildSettingsScene(
                    "Assets/B.unity",
                    false);

            Assert.That(
                clone[0].path,
                Is.EqualTo("Assets/A.unity"));
        }

        [Test]
        public void SummaryIncludesIndexStateAndPath()
        {
            string summary =
                EchoLaunchSetupBuildSettingsWriter.Summarize(
                    new[]
                    {
                        new EditorBuildSettingsScene(
                            "Assets/A.unity",
                            false)
                    });

            Assert.That(
                summary,
                Is.EqualTo("0:Off:Assets/A.unity"));
        }

        private static bool Apply(
            EchoLaunchBuildSettingsPolicy policy,
            string bootPath,
            bool approve)
        {
            return new EchoLaunchSetupBuildSettingsWriter().Apply(
                policy,
                bootPath,
                approve,
                new EchoLaunchSetupRollbackJournal(),
                new EchoLaunchSetupExecutionLog());
        }
    }
}
