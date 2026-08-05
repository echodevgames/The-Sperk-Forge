
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
        public void WindowExposesApplyButNoRepairOrMigrateMethod()
        {
            MethodInfo[] methods =
                typeof(EchoLaunchSetupWindow).GetMethods(
                    BindingFlags.Instance |
                    BindingFlags.Static |
                    BindingFlags.Public |
                    BindingFlags.NonPublic);

            bool foundApply = false;

            for (int methodIndex = 0;
                 methodIndex < methods.Length;
                 methodIndex++)
            {
                string name = methods[methodIndex].Name;

                if (name.IndexOf(
                        "Apply",
                        StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    foundApply = true;
                }

                Assert.That(
                    name.IndexOf(
                        "Repair",
                        StringComparison.OrdinalIgnoreCase),
                    Is.LessThan(0),
                    "Forbidden repair method found: " + name);

                Assert.That(
                    name.IndexOf(
                        "Migrate",
                        StringComparison.OrdinalIgnoreCase),
                    Is.LessThan(0),
                    "Forbidden migration method found: " + name);
            }

            Assert.That(foundApply, Is.True);
        }

        [Test]
        public void ApplyBoundaryMessageForbidsOverwrite()
        {
            Assert.That(
                EchoLaunchSetupWindow.ApplyBoundaryMessage,
                Does.Contain("never overwritten"));
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
