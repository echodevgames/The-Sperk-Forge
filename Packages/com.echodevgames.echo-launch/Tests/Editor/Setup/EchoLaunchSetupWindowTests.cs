
using System;
using System.Reflection;
using EchoDevGames.EchoLaunch.Editor.Setup;
using NUnit.Framework;
using UnityEditor;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Setup
{
    public sealed class EchoLaunchSetupWindowTests
    {
        private EchoLaunchSetupWindow window;

        [TearDown]
        public void TearDown()
        {
            if (window != null)
            {
                window.Close();
                window = null;
            }
        }

        [Test]
        public void ApprovedMenuPathIsStable()
        {
            Assert.That(
                EchoLaunchSetupWindow.MenuPath,
                Is.EqualTo(
                    "Tools/Sperk's Forge/First Light/Setup"));
        }

        [Test]
        public void PreviewOnlyWarningIsStable()
        {
            Assert.That(
                EchoLaunchSetupWindow.PreviewOnlyMessage,
                Does.Contain("changes nothing"));
        }

        [Test]
        public void WindowCanOpen()
        {
            window = EchoLaunchSetupWindow.OpenWindow();

            Assert.That(window, Is.Not.Null);
            Assert.That(
                window.titleContent.text,
                Is.EqualTo("First Light Setup"));
        }

        [Test]
        public void WindowRefreshProducesPlan()
        {
            window = EchoLaunchSetupWindow.OpenWindow();

            EchoLaunchSetupPlan plan =
                window.RefreshPlanForTests(
                    EchoLaunchSetupTestFactory.CreateRequest(
                        destinationPath:
                        "Assets/Scenes/Missing.unity"));

            Assert.That(plan, Is.Not.Null);
        }

        [Test]
        public void WindowRefreshProducesTextReport()
        {
            window = EchoLaunchSetupWindow.OpenWindow();

            window.RefreshPlanForTests(
                EchoLaunchSetupTestFactory.CreateRequest(
                    destinationPath:
                    "Assets/Scenes/Missing.unity"));

            Assert.That(
                window.CurrentReportForTests,
                Does.Contain("First Light Setup Plan"));
        }

        [Test]
        public void WindowExposesNoApplyRepairOrMigrateMethod()
        {
            MethodInfo[] methods =
                typeof(EchoLaunchSetupWindow).GetMethods(
                    BindingFlags.Instance |
                    BindingFlags.Static |
                    BindingFlags.Public |
                    BindingFlags.NonPublic);

            string[] forbiddenTokens =
            {
                "Apply",
                "Repair",
                "Migrate",
                "CreateAssets",
                "ChangeBuildSettings"
            };

            for (int methodIndex = 0;
                 methodIndex < methods.Length;
                 methodIndex++)
            {
                for (int tokenIndex = 0;
                     tokenIndex < forbiddenTokens.Length;
                     tokenIndex++)
                {
                    Assert.That(
                        methods[methodIndex].Name.IndexOf(
                            forbiddenTokens[tokenIndex],
                            StringComparison.OrdinalIgnoreCase),
                        Is.LessThan(0),
                        "Forbidden method found: " +
                        methods[methodIndex].Name);
                }
            }
        }

        [Test]
        public void WindowRefreshDoesNotCreateProjectRoot()
        {
            const string root =
                "Assets/__EchoLaunch_Window_NoWrite";

            window = EchoLaunchSetupWindow.OpenWindow();

            EchoLaunchSetupRequest request =
                new EchoLaunchSetupRequest(
                    root,
                    root + "/Scenes/Boot.unity",
                    root + "/Scenes/Missing.unity",
                    false,
                    EchoLaunchBuildSettingsPolicy.AddIfMissingAtEnd);

            Assert.That(AssetDatabase.IsValidFolder(root), Is.False);

            window.RefreshPlanForTests(request);

            Assert.That(AssetDatabase.IsValidFolder(root), Is.False);
        }
    }
}
