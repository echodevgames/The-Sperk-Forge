//----- IInitialDestinationLoader.cs START -----

using System;
using System.Threading;
using UnityEngine;

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Package-owned seam that performs the one initial destination handoff.
    ///
    /// Implementations load only the startup destination. Normal mid-game
    /// scene travel remains outside First Light.
    /// </summary>
    public interface IInitialDestinationLoader
    {
        Awaitable<InitialDestinationLoadResult>
            LoadAsync(
                LaunchDestination destination,
                IProgress<float> progress,
                CancellationToken cancellationToken);
    }

    /// <summary>
    /// Optional internal preflight seam used by loaders that can validate
    /// their platform-specific destination requirements before startup-step
    /// side effects begin.
    /// </summary>
    internal interface
        IInitialDestinationPreflightValidator
    {
        bool TryValidate(
            LaunchDestination destination,
            out string failureMessage);
    }
}

//----- IInitialDestinationLoader.cs END -----
