using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Package-local transition authority. It tracks only active operations;
    /// there is no per-frame scene/global scan.
    /// </summary>
    public sealed class UITransitionCoordinator
    {
        private sealed class ActiveTransition
        {
            public string SurfaceId;
            public long Generation;
            public CancellationTokenSource Cancellation;
            public Awaitable<UITransitionResult> DriverAwaitable;
            public bool DriverAwaitableAssigned;
            public CancellationTokenSource TimeoutWatchCancellation;
            public bool CancellationRequested;
            public bool TimedOut;
            public bool Superseded;
            public bool Completed;
        }

        private readonly Dictionary<string, IUITransitionDriver> drivers =
            new Dictionary<string, IUITransitionDriver>(StringComparer.Ordinal);
        private readonly Dictionary<string, long> generationBySurface =
            new Dictionary<string, long>(StringComparer.Ordinal);
        private readonly Dictionary<string, ActiveTransition> activeBySurface =
            new Dictionary<string, ActiveTransition>(StringComparer.Ordinal);

        private UITransitionProfile rootDefault;
        private long nextOperationId;
        private bool shutdown;

        public UITransitionCoordinator(UITransitionProfile rootDefault = null)
        {
            this.rootDefault = (rootDefault ?? UITransitionProfile.CreateDefault()).Snapshot();
            RegisterBuiltIn(new ImmediateUITransitionDriver());
            RegisterBuiltIn(new CanvasGroupFadeTransitionDriver());
        }

        public bool IsValid => !shutdown;
        public int RegisteredDriverCount => drivers.Count;
        public int ActiveCount => activeBySurface.Count;

        public void SetRootDefault(UITransitionProfile profile)
        {
            if (shutdown)
            {
                return;
            }
            rootDefault = (profile ?? UITransitionProfile.CreateDefault()).Snapshot();
        }

        public bool RegisterDriver(IUITransitionDriver driver)
        {
            if (shutdown || driver == null || string.IsNullOrWhiteSpace(driver.DriverId))
            {
                return false;
            }
            string id = driver.DriverId.Trim();
            if (drivers.ContainsKey(id))
            {
                return false;
            }
            drivers.Add(id, driver);
            return true;
        }

        public bool UnregisterDriver(string driverId)
        {
            if (shutdown || string.IsNullOrWhiteSpace(driverId))
            {
                return false;
            }
            string id = driverId.Trim();
            if (id == UITransitionDriverIds.Immediate || id == UITransitionDriverIds.CanvasGroupFade)
            {
                return false;
            }
            return drivers.Remove(id);
        }

        public bool TryGetDriver(string driverId, out IUITransitionDriver driver)
        {
            driver = null;
            return !shutdown &&
                !string.IsNullOrWhiteSpace(driverId) &&
                drivers.TryGetValue(driverId.Trim(), out driver);
        }

        public UITransitionResolvedPolicy ResolvePolicy(
            UITransitionDirection direction,
            UITransitionProfile definitionProfile = null,
            UITransitionProfile transientOverride = null,
            bool reducedMotion = false)
        {
            ResolvedValues values = new ResolvedValues();
            ApplyLayer(values, rootDefault, direction);
            ApplyLayer(values, definitionProfile, direction);
            ApplyLayer(values, transientOverride, direction);

            bool reducedApplied = false;
            if (reducedMotion && values.ReducedMotionMode == UITransitionReducedMotionMode.UseReplacement)
            {
                values.DriverId = string.IsNullOrWhiteSpace(values.ReducedMotionDriverId)
                    ? UITransitionDriverIds.Immediate
                    : values.ReducedMotionDriverId;
                values.Duration = 0f;
                reducedApplied = true;
            }

            if (string.IsNullOrWhiteSpace(values.DriverId))
            {
                values.DriverId = UITransitionDriverIds.Immediate;
            }
            if (values.Duration < 0f)
            {
                values.Duration = 0f;
            }
            if (values.HardTimeout <= 0f)
            {
                values.HardTimeout = 5f;
            }
            if (values.Curve == null)
            {
                values.Curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            }

            return new UITransitionResolvedPolicy(
                values.ProfileId,
                values.DriverId,
                values.Duration,
                values.Curve,
                values.HardTimeout,
                reducedApplied);
        }

        public async Awaitable<UITransitionResult> ExecuteAsync(
            UISurface surface,
            UITransitionDirection direction,
            UITransitionProfile definitionProfile = null,
            UITransitionProfile transientOverride = null,
            bool reducedMotion = false)
        {
            if (shutdown || surface == null)
            {
                return new UITransitionResult(
                    UITransitionStatus.Unavailable,
                    default,
                    0,
                    surface == null ? string.Empty : surface.SurfaceId,
                    direction,
                    string.Empty,
                    string.Empty,
                    0d,
                    shutdown ? "Transition coordinator is shut down." : "Transition surface is missing.");
            }

            UITransitionResolvedPolicy policy = ResolvePolicy(direction, definitionProfile, transientOverride, reducedMotion);
            if (!TryGetDriver(policy.DriverId, out IUITransitionDriver driver))
            {
                return new UITransitionResult(
                    UITransitionStatus.Unavailable,
                    new UITransitionOperationId(++nextOperationId),
                    NextGeneration(surface.SurfaceId),
                    surface.SurfaceId,
                    direction,
                    policy.DriverId,
                    policy.ProfileId,
                    0d,
                    "No registered transition driver matches the resolved stable driver ID.");
            }

            SupersedeSurface(
                surface.SurfaceId);

            long generation = NextGeneration(surface.SurfaceId);
            UITransitionOperationId operationId = new UITransitionOperationId(++nextOperationId);
            CancellationTokenSource cancellation = new CancellationTokenSource();
            ActiveTransition active = new ActiveTransition
            {
                SurfaceId = surface.SurfaceId,
                Generation = generation,
                Cancellation = cancellation
            };
            activeBySurface[surface.SurfaceId] = active;

            UITransitionRequest request = new UITransitionRequest(
                operationId,
                generation,
                surface,
                direction,
                policy,
                () => IsCurrent(surface.SurfaceId, generation));

            double started =
                Time.realtimeSinceStartupAsDouble;

            UITransitionResult result;

            try
            {
                active.DriverAwaitable =
                    driver.ExecuteAsync(
                        request,
                        cancellation.Token);

                active.DriverAwaitableAssigned = true;

                active.TimeoutWatchCancellation =
                    new CancellationTokenSource();

                WatchHardTimeoutAsync(
                    active,
                    request,
                    driver,
                    policy.HardTimeoutSeconds,
                    started,
                    active.TimeoutWatchCancellation.Token);

                // The successful path is intentionally a direct await.
                // Unity Awaitable continuations run synchronously when the
                // awaited operation completes, so a manually-completed or
                // custom driver does not need another PlayerLoop tick before
                // lifecycle settlement can continue.
                result =
                    await active.DriverAwaitable;
            }
            catch (OperationCanceledException)
            {
                double cancelledElapsed =
                    Time.realtimeSinceStartupAsDouble -
                    started;

                UITransitionStatus cancelledStatus =
                    active.TimedOut
                        ? UITransitionStatus.TimedOut
                        : active.Superseded ||
                            !IsCurrent(
                                surface.SurfaceId,
                                generation)
                            ? UITransitionStatus.Stale
                            : UITransitionStatus.Cancelled;

                string cancelledMessage =
                    active.TimedOut
                        ? "Transition exceeded its hard safety bound."
                        : cancelledStatus ==
                            UITransitionStatus.Stale
                            ? "Transition was superseded by newer lifecycle truth."
                            : "Transition was cancelled.";

                CompleteActive(
                    active);

                if (cancelledStatus ==
                    UITransitionStatus.TimedOut)
                {
                    driver.ForceFinalState(
                        request);
                }

                return UITransitionResult.ForRequest(
                    request,
                    cancelledStatus,
                    cancelledElapsed,
                    cancelledMessage);
            }
            catch (Exception exception)
            {
                double failedElapsed =
                    Time.realtimeSinceStartupAsDouble -
                    started;

                bool staleFailure =
                    active.Superseded ||
                    !IsCurrent(
                        surface.SurfaceId,
                        generation);

                CompleteActive(
                    active);

                if (staleFailure)
                {
                    return UITransitionResult.ForRequest(
                        request,
                        UITransitionStatus.Stale,
                        failedElapsed,
                        "Transition faulted after its lifecycle generation became stale.");
                }

                driver.ForceFinalState(
                    request);

                return UITransitionResult.ForRequest(
                    request,
                    UITransitionStatus.Failed,
                    failedElapsed,
                    exception.Message);
            }

            double totalElapsed =
                Time.realtimeSinceStartupAsDouble -
                started;

            bool cancellationRequested =
                active.CancellationRequested;

            bool stillCurrent =
                IsCurrent(
                    surface.SurfaceId,
                    generation);

            bool superseded =
                active.Superseded;

            CompleteActive(
                active);

            if (superseded ||
                !stillCurrent)
            {
                return UITransitionResult.ForRequest(
                    request,
                    UITransitionStatus.Stale,
                    totalElapsed,
                    "Transition completed after its lifecycle generation became stale.");
            }

            if (cancellationRequested)
            {
                return UITransitionResult.ForRequest(
                    request,
                    UITransitionStatus.Cancelled,
                    totalElapsed,
                    "Transition was cancelled.");
            }

            if (!result.Succeeded &&
                result.Status !=
                    UITransitionStatus.Stale &&
                result.Status !=
                    UITransitionStatus.Cancelled)
            {
                driver.ForceFinalState(
                    request);
            }

            return new UITransitionResult(
                result.Status,
                request.OperationId,
                request.Generation,
                request.SurfaceId,
                request.Direction,
                policy.DriverId,
                policy.ProfileId,
                totalElapsed,
                result.Message);
        }

        private async void WatchHardTimeoutAsync(
            ActiveTransition active,
            UITransitionRequest request,
            IUITransitionDriver driver,
            float hardTimeoutSeconds,
            double started,
            CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (active == null ||
                        request == null ||
                        driver == null ||
                        shutdown ||
                        !IsCurrent(
                            request.SurfaceId,
                            request.Generation))
                    {
                        return;
                    }

                    double elapsed =
                        Time.realtimeSinceStartupAsDouble -
                        started;

                    if (elapsed >=
                        hardTimeoutSeconds)
                    {
                        active.TimedOut = true;

                        TryCancel(
                            active);

                        driver.ForceFinalState(
                            request);

                        return;
                    }

                    await Awaitable.NextFrameAsync();
                }
            }
            catch (OperationCanceledException)
            {
                // Normal watchdog shutdown after transition settlement.
            }
            catch
            {
                // The watchdog is safety plumbing only. Driver/lifecycle
                // settlement remains authoritative and must not be replaced
                // by an unhandled fire-and-forget exception.
            }
        }

        private void SupersedeSurface(
            string surfaceId)
        {
            if (shutdown ||
                string.IsNullOrWhiteSpace(
                    surfaceId) ||
                !activeBySurface.TryGetValue(
                    surfaceId.Trim(),
                    out ActiveTransition active))
            {
                return;
            }

            active.Superseded = true;

            TryCancel(
                active);
        }

        public bool CancelSurface(string surfaceId)
        {
            if (shutdown || string.IsNullOrWhiteSpace(surfaceId) || !activeBySurface.TryGetValue(surfaceId.Trim(), out ActiveTransition active))
            {
                return false;
            }
            TryCancel(active);
            return true;
        }

        public void Shutdown()
        {
            if (shutdown)
            {
                return;
            }
            shutdown = true;
            foreach (ActiveTransition active in activeBySurface.Values)
            {
                TryCancel(active);
            }
            activeBySurface.Clear();
            generationBySurface.Clear();
        }

        private bool IsCurrent(string surfaceId, long generation) =>
            !shutdown &&
            activeBySurface.TryGetValue(surfaceId, out ActiveTransition active) &&
            active.Generation == generation;

        private long NextGeneration(string surfaceId)
        {
            generationBySurface.TryGetValue(surfaceId, out long current);
            long next = current + 1;
            generationBySurface[surfaceId] = next;
            return next;
        }

        private void CompleteActive(ActiveTransition active)
        {
            if (active == null)
            {
                return;
            }

            active.Completed = true;

            if (activeBySurface.TryGetValue(active.SurfaceId, out ActiveTransition current) && ReferenceEquals(current, active))
            {
                activeBySurface.Remove(active.SurfaceId);
            }

            try
            {
                active.TimeoutWatchCancellation?.Cancel();
            }
            catch (ObjectDisposedException)
            {
            }

            active.TimeoutWatchCancellation?.Dispose();
            active.Cancellation?.Dispose();
        }

        private static void TryCancel(ActiveTransition active)
        {
            if (active == null || active.CancellationRequested)
            {
                return;
            }
            active.CancellationRequested = true;
            try { active.Cancellation?.Cancel(); } catch (ObjectDisposedException) { }

            // CancellationToken callbacks and Unity Awaitable continuations
            // may settle synchronously. CompleteActive then releases the
            // pooled driver awaitable before CancellationTokenSource.Cancel
            // returns. Only use direct Awaitable cancellation as the fallback
            // for a driver that did not settle through its token.
            if (!active.Completed &&
                active.DriverAwaitableAssigned)
            {
                try { active.DriverAwaitable.Cancel(); } catch (InvalidOperationException) { }
            }
        }

        private void RegisterBuiltIn(IUITransitionDriver driver)
        {
            drivers.Add(driver.DriverId, driver);
        }

        private sealed class ResolvedValues
        {
            public string ProfileId = string.Empty;
            public string DriverId = string.Empty;
            public float Duration = -1f;
            public AnimationCurve Curve;
            public float HardTimeout = -1f;
            public UITransitionReducedMotionMode ReducedMotionMode = UITransitionReducedMotionMode.Inherit;
            public string ReducedMotionDriverId = string.Empty;
        }

        private static void ApplyLayer(ResolvedValues values, UITransitionProfile profile, UITransitionDirection direction)
        {
            if (values == null || profile == null)
            {
                return;
            }
            if (!string.IsNullOrWhiteSpace(profile.ProfileId)) values.ProfileId = profile.ProfileId;
            string driverId = direction == UITransitionDirection.Enter ? profile.EnterDriverId : profile.ExitDriverId;
            if (!string.IsNullOrWhiteSpace(driverId)) values.DriverId = driverId;
            float duration = direction == UITransitionDirection.Enter ? profile.EnterDurationSeconds : profile.ExitDurationSeconds;
            if (duration >= 0f) values.Duration = duration;
            AnimationCurve curve = direction == UITransitionDirection.Enter ? profile.EnterCurve : profile.ExitCurve;
            if (curve != null) values.Curve = curve;
            if (profile.HardTimeoutSeconds > 0f) values.HardTimeout = profile.HardTimeoutSeconds;
            if (profile.ReducedMotionMode != UITransitionReducedMotionMode.Inherit) values.ReducedMotionMode = profile.ReducedMotionMode;
            if (!string.IsNullOrWhiteSpace(profile.ReducedMotionDriverId)) values.ReducedMotionDriverId = profile.ReducedMotionDriverId;
        }
    }
}
