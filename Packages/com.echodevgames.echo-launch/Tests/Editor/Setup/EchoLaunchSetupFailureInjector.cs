using System;
using EchoDevGames.EchoLaunch.Editor.Setup;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Setup
{
    internal sealed class EchoLaunchSetupFailureInjector :
        IEchoLaunchSetupFailureInjector
    {
        internal EchoLaunchSetupOperationKind? FailureKind { get; set; }
        internal int CallCount { get; private set; }

        public void ThrowIfRequested(
            EchoLaunchSetupOperationKind operationKind)
        {
            CallCount++;

            if (FailureKind.HasValue &&
                FailureKind.Value == operationKind)
            {
                throw new InvalidOperationException(
                    "Injected failure at " + operationKind + ".");
            }
        }
    }
}
