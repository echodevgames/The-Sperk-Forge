using NUnit.Framework;
using UnityEngine;
using EchoDevGames.EchoLaunch.Editor.Simulation;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Simulation
{
    public sealed class EchoLaunchSimulatorWindowTests
    {
        [Test]
        public void CreatingWindowDoesNotRunSimulation()
        {
            EchoLaunchSimulatorWindow window =
                ScriptableObject.CreateInstance<
                    EchoLaunchSimulatorWindow>();

            try
            {
                Assert.That(window.IsRunning, Is.False);
                Assert.That(window.LastReport, Is.Null);

                Assert.That(
                    LaunchSimulationService.Shared.IsRunning,
                    Is.False);
            }
            finally
            {
                Object.DestroyImmediate(window);
            }
        }
    }
}
