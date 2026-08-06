using System.Threading;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoLaunch.Editor.Simulation
{
    internal static class LaunchSimulationEditorTick
    {
        internal static Awaitable NextAsync(
            CancellationToken cancellationToken)
        {
            PendingTick pending =
                new PendingTick(cancellationToken);

            return pending.Awaitable;
        }

        private sealed class PendingTick
        {
            private readonly AwaitableCompletionSource source =
                new AwaitableCompletionSource();

            private CancellationTokenRegistration registration;
            private int settled;

            internal PendingTick(
                CancellationToken cancellationToken)
            {
                EditorApplication.update += Complete;

                if (cancellationToken.CanBeCanceled)
                {
                    registration =
                        cancellationToken.Register(Cancel);
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    Cancel();
                }
            }

            internal Awaitable Awaitable => source.Awaitable;

            private void Complete()
            {
                if (Interlocked.Exchange(
                        ref settled,
                        1) != 0)
                {
                    return;
                }

                EditorApplication.update -= Complete;
                registration.Dispose();
                source.SetResult();
            }

            private void Cancel()
            {
                if (Interlocked.Exchange(
                        ref settled,
                        1) != 0)
                {
                    return;
                }

                EditorApplication.update -= Complete;
                source.TrySetCanceled();
            }
        }
    }
}
