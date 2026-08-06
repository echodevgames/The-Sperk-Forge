using EchoDevGames.EchoLaunch.Editor.Validation;
using NUnit.Framework;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Validation
{
    public sealed class EchoLaunchValidatorWindowTests
    {
        [Test]
        public void CreatingWindowDoesNotRunValidation()
        {
            EchoLaunchValidatorWindow window =
                ScriptableObject.CreateInstance<
                    EchoLaunchValidatorWindow>();

            try
            {
                Assert.That(window.LastReport, Is.Null);
            }
            finally
            {
                ScriptableObject.DestroyImmediate(window);
            }
        }

        [Test]
        public void WindowDefaultsToCanonicalProjectRoot()
        {
            EchoLaunchValidatorWindow window =
                ScriptableObject.CreateInstance<
                    EchoLaunchValidatorWindow>();

            try
            {
                Assert.That(
                    window.ProjectRootPath,
                    Is.EqualTo(
                        "Assets/EchoDevGames/FirstLight"));
            }
            finally
            {
                ScriptableObject.DestroyImmediate(window);
            }
        }
    }
}
