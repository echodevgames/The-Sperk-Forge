using System.Threading;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    public sealed class ImmediateUITransitionDriver : IUITransitionDriver
    {
        public string DriverId => UITransitionDriverIds.Immediate;
        public bool SupportsCancellation => true;

        public Awaitable<UITransitionResult> ExecuteAsync(
            UITransitionRequest request,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            UITransitionResult result;

            if (request == null || request.Surface == null || request.Policy == null)
            {
                result =
                    UITransitionResult.ForRequest(
                        request,
                        UITransitionStatus.Unavailable,
                        message: "Immediate transition request is incomplete.");

                return CreateCompletedAwaitable(
                    result);
            }

            if (!request.IsCurrent)
            {
                result =
                    UITransitionResult.ForRequest(
                        request,
                        UITransitionStatus.Stale,
                        message: "Transition generation is stale.");

                return CreateCompletedAwaitable(
                    result);
            }

            ForceFinalState(
                request);

            result =
                UITransitionResult.ForRequest(
                    request,
                    UITransitionStatus.Completed,
                    message: "Immediate transition completed.");

            return CreateCompletedAwaitable(
                result);
        }

        private static Awaitable<UITransitionResult> CreateCompletedAwaitable(
            UITransitionResult result)
        {
            AwaitableCompletionSource<UITransitionResult> completion =
                new AwaitableCompletionSource<UITransitionResult>();

            completion.SetResult(
                result);

            return completion.Awaitable;
        }

        public void ForceFinalState(UITransitionRequest request)
        {
            if (request == null || request.Surface == null)
            {
                return;
            }

            CanvasGroup group = request.Surface.TransitionCanvasGroup;
            if (group != null)
            {
                group.alpha = request.Direction == UITransitionDirection.Enter ? 1f : 0f;
            }
        }
    }
}
