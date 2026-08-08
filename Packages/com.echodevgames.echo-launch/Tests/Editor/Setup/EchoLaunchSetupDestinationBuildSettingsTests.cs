//----- EchoLaunchSetupDestinationBuildSettingsTests.cs START -----

using EchoDevGames.EchoLaunch.Editor.Setup;
using NUnit.Framework;
using UnityEditor;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Setup
{
    public sealed class EchoLaunchSetupDestinationBuildSettingsTests
    {
        private EditorBuildSettingsScene[] originalBuildSettings;

        [SetUp]
        public void SetUp()
        {
            originalBuildSettings =
                EchoLaunchSetupBuildSettingsWriter.Clone(
                    EditorBuildSettings.scenes);
        }

        [TearDown]
        public void TearDown()
        {
            EditorBuildSettings.scenes =
                EchoLaunchSetupBuildSettingsWriter.Clone(
                    originalBuildSettings);
        }

        [Test]
        public void MissingBootAndDestinationPlanBootThenDestinationCreates()
        {
            EchoLaunchSetupPlan plan =
                CreatePlan();

            EchoLaunchSetupOperation boot =
                Find(
                    plan,
                    EchoLaunchSetupOperationKind.ResolveBuildSettings);

            EchoLaunchSetupOperation destination =
                Find(
                    plan,
                    EchoLaunchSetupOperationKind
                        .ResolveDestinationBuildSettings);

            Assert.That(
                boot.Disposition,
                Is.EqualTo(
                    EchoLaunchSetupOperationDisposition.Create));

            Assert.That(
                destination.Disposition,
                Is.EqualTo(
                    EchoLaunchSetupOperationDisposition.Create));

            Assert.That(
                IndexOf(plan, boot),
                Is.LessThan(
                    IndexOf(plan, destination)));
        }

        [Test]
        public void ExistingEnabledDestinationPlansOnlyBootCreate()
        {
            EchoLaunchSetupPlan plan =
                CreatePlan(
                    new[]
                    {
                        new EchoLaunchBuildSettingsSceneFact(
                            EchoLaunchSetupTestFactory
                                .DestinationScenePath,
                            true,
                            0)
                    });

            Assert.That(
                Find(
                    plan,
                    EchoLaunchSetupOperationKind
                        .ResolveBuildSettings)
                    .Disposition,
                Is.EqualTo(
                    EchoLaunchSetupOperationDisposition.Create));

            Assert.That(
                Find(
                    plan,
                    EchoLaunchSetupOperationKind
                        .ResolveDestinationBuildSettings)
                    .Disposition,
                Is.EqualTo(
                    EchoLaunchSetupOperationDisposition.NoChange));
        }

        [Test]
        public void ExistingEnabledBootPlansOnlyDestinationCreate()
        {
            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            EchoLaunchSetupPlan plan =
                CreatePlan(
                    new[]
                    {
                        new EchoLaunchBuildSettingsSceneFact(
                            paths.BootScenePath,
                            true,
                            0)
                    });

            Assert.That(
                Find(
                    plan,
                    EchoLaunchSetupOperationKind
                        .ResolveBuildSettings)
                    .Disposition,
                Is.EqualTo(
                    EchoLaunchSetupOperationDisposition.NoChange));

            Assert.That(
                Find(
                    plan,
                    EchoLaunchSetupOperationKind
                        .ResolveDestinationBuildSettings)
                    .Disposition,
                Is.EqualTo(
                    EchoLaunchSetupOperationDisposition.Create));
        }

        [Test]
        public void ExistingEnabledBootAndDestinationPlanNoBuildSettingsCreates()
        {
            EchoLaunchSetupPathSet paths =
                EchoLaunchSetupPathSet.CreateDefault();

            EchoLaunchSetupPlan plan =
                CreatePlan(
                    new[]
                    {
                        new EchoLaunchBuildSettingsSceneFact(
                            paths.BootScenePath,
                            true,
                            0),
                        new EchoLaunchBuildSettingsSceneFact(
                            EchoLaunchSetupTestFactory
                                .DestinationScenePath,
                            true,
                            1)
                    });

            Assert.That(
                Find(
                    plan,
                    EchoLaunchSetupOperationKind
                        .ResolveBuildSettings)
                    .Disposition,
                Is.EqualTo(
                    EchoLaunchSetupOperationDisposition.NoChange));

            Assert.That(
                Find(
                    plan,
                    EchoLaunchSetupOperationKind
                        .ResolveDestinationBuildSettings)
                    .Disposition,
                Is.EqualTo(
                    EchoLaunchSetupOperationDisposition.NoChange));
        }

        [Test]
        public void AppendWriterPreservesUnrelatedOrderAndAddsBootThenDestination()
        {
            EditorBuildSettings.scenes =
                new[]
                {
                    new EditorBuildSettingsScene(
                        "Assets/A.unity",
                        false),
                    new EditorBuildSettingsScene(
                        "Assets/B.unity",
                        true)
                };

            EchoLaunchSetupBuildSettingsWriter writer =
                new EchoLaunchSetupBuildSettingsWriter();

            EchoLaunchSetupRollbackJournal journal =
                new EchoLaunchSetupRollbackJournal();

            EchoLaunchSetupExecutionLog log =
                new EchoLaunchSetupExecutionLog();

            writer.Apply(
                EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd,
                "Assets/Boot.unity",
                false,
                journal,
                log);

            writer.Apply(
                EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd,
                "Assets/MainMenu.unity",
                false,
                journal,
                log);

            EditorBuildSettingsScene[] scenes =
                EditorBuildSettings.scenes;

            Assert.That(
                scenes.Length,
                Is.EqualTo(4));

            Assert.That(
                scenes[0].path,
                Is.EqualTo("Assets/A.unity"));

            Assert.That(
                scenes[0].enabled,
                Is.False);

            Assert.That(
                scenes[1].path,
                Is.EqualTo("Assets/B.unity"));

            Assert.That(
                scenes[1].enabled,
                Is.True);

            Assert.That(
                scenes[2].path,
                Is.EqualTo("Assets/Boot.unity"));

            Assert.That(
                scenes[2].enabled,
                Is.True);

            Assert.That(
                scenes[3].path,
                Is.EqualTo("Assets/MainMenu.unity"));

            Assert.That(
                scenes[3].enabled,
                Is.True);
        }

        [Test]
        public void PlanTextExplicitlyIncludesDestinationBuildSettingsOperation()
        {
            EchoLaunchSetupPlan plan =
                CreatePlan();

            string text =
                new EchoLaunchSetupPlanTextFormatter()
                    .Format(plan);

            Assert.That(
                text,
                Does.Contain(
                    "ResolveDestinationBuildSettings"));

            Assert.That(
                text,
                Does.Contain(
                    EchoLaunchSetupTestFactory
                        .DestinationScenePath));
        }

        [Test]
        public void DoNotChangeBlocksWhenRequiredScenesAreMissing()
        {
            EchoLaunchSetupRequest request =
                EchoLaunchSetupTestFactory.CreateRequest(
                    policy:
                    EchoLaunchBuildSettingsPolicy.DoNotChange);

            EchoLaunchSetupPlan plan =
                new EchoLaunchSetupPlanner()
                    .CreatePlan(
                        request,
                        EchoLaunchSetupTestFactory
                            .CreateSnapshot());

            Assert.That(
                plan.Status,
                Is.EqualTo(
                    EchoLaunchSetupPlanStatus.Blocked));
        }

        private static EchoLaunchSetupPlan CreatePlan(
            EchoLaunchBuildSettingsSceneFact[] buildScenes = null)
        {
            EchoLaunchSetupRequest request =
                EchoLaunchSetupTestFactory.CreateRequest();

            return new EchoLaunchSetupPlanner()
                .CreatePlan(
                    request,
                    EchoLaunchSetupTestFactory.CreateSnapshot(
                        buildScenes: buildScenes));
        }

        private static EchoLaunchSetupOperation Find(
            EchoLaunchSetupPlan plan,
            EchoLaunchSetupOperationKind kind)
        {
            EchoLaunchSetupOperation operation =
                EchoLaunchSetupTestFactory.FindOperation(
                    plan,
                    kind);

            Assert.That(
                operation,
                Is.Not.Null);

            return operation;
        }

        private static int IndexOf(
            EchoLaunchSetupPlan plan,
            EchoLaunchSetupOperation operation)
        {
            for (int index = 0;
                 index < plan.Operations.Count;
                 index++)
            {
                if (ReferenceEquals(
                        plan.Operations[index],
                        operation))
                {
                    return index;
                }
            }

            return -1;
        }
    }
}

//----- EchoLaunchSetupDestinationBuildSettingsTests.cs END -----
