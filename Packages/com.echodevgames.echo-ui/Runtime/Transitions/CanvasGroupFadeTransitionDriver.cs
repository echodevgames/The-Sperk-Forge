using System;
using System.Threading;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    public sealed class CanvasGroupFadeTransitionDriver : IUITransitionDriver
    {
        public string DriverId => UITransitionDriverIds.CanvasGroupFade;
        public bool SupportsCancellation => true;

        public async Awaitable<UITransitionResult> ExecuteAsync(
            UITransitionRequest request,
            CancellationToken cancellationToken)
        {
            if (request == null || request.Surface == null || request.Policy == null)
            {
                return UITransitionResult.ForRequest(request, UITransitionStatus.Unavailable, message: "CanvasGroup fade request is incomplete.");
            }

            CanvasGroup group = request.Surface.TransitionCanvasGroup;
            if (group == null)
            {
                return UITransitionResult.ForRequest(request, UITransitionStatus.Unavailable, message: "CanvasGroup fade requires a CanvasGroup on the UISurface.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            if (!request.IsCurrent)
            {
                return UITransitionResult.ForRequest(request, UITransitionStatus.Stale, message: "Transition generation is stale.");
            }

            float duration = Mathf.Max(0f, request.Policy.DurationSeconds);
            float startAlpha = request.Direction == UITransitionDirection.Enter ? 0f : group.alpha;
            float endAlpha = request.Direction == UITransitionDirection.Enter ? 1f : 0f;
            group.alpha = startAlpha;

            double started = Time.realtimeSinceStartupAsDouble;
            if (duration <= 0f)
            {
                group.alpha = endAlpha;
                return UITransitionResult.ForRequest(request, UITransitionStatus.Completed, message: "Zero-duration CanvasGroup fade completed.");
            }

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!request.IsCurrent)
                {
                    return UITransitionResult.ForRequest(
                        request,
                        UITransitionStatus.Stale,
                        Time.realtimeSinceStartupAsDouble - started,
                        "Transition generation became stale during CanvasGroup fade.");
                }

                double elapsed = Time.realtimeSinceStartupAsDouble - started;
                float linear = Mathf.Clamp01((float)(elapsed / duration));
                float curved = request.Policy.Curve == null ? linear : request.Policy.Curve.Evaluate(linear);
                group.alpha = Mathf.LerpUnclamped(startAlpha, endAlpha, curved);

                if (linear >= 1f)
                {
                    group.alpha = endAlpha;
                    return UITransitionResult.ForRequest(request, UITransitionStatus.Completed, elapsed, "CanvasGroup fade completed.");
                }

                await Awaitable.NextFrameAsync(cancellationToken);
            }
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
