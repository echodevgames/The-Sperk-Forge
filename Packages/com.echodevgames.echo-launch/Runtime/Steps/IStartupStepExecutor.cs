//----- IStartupStepExecutor.cs START -----

using UnityEngine;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Defines one fresh single-use runtime executor for a startup-step
    /// definition.
    ///
    /// A future sequence runner creates the executor and invokes it once.
    /// Active execution state belongs to the executor instance, never to
    /// the shared ScriptableObject definition.
    /// </summary>
    public interface IStartupStepExecutor
    {
        /// <summary>
        /// Executes one startup-step attempt using immutable runtime
        /// context supplied by the future sequence runner.
        /// </summary>
        Awaitable<StartupStepResult> ExecuteAsync(
            StartupStepContext context);
    }
}

//----- IStartupStepExecutor.cs END -----
