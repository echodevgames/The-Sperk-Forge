using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    public enum UIMotifServiceState
    {
        Unavailable = 0,
        Ready = 1,
        Shutdown = 2
    }

    public enum UIMotifSwitchStatus
    {
        Applied = 0,
        FallbackApplied = 1,
        Unchanged = 2,
        Unavailable = 3,
        Shutdown = 4
    }

    public readonly struct UIMotifServiceSnapshot
    {
        public UIMotifServiceSnapshot(
            UIMotifServiceState state,
            UIMotifId effectiveMotifId = default,
            long revision = 0)
        {
            State = state;
            EffectiveMotifId = effectiveMotifId;
            Revision = revision;
        }

        public UIMotifServiceState State { get; }
        public UIMotifId EffectiveMotifId { get; }
        public long Revision { get; }
        public bool IsReady => State == UIMotifServiceState.Ready;
    }

    public readonly struct UIMotifSwitchResult
    {
        public UIMotifSwitchResult(
            UIMotifSwitchStatus status,
            UIMotifId requestedMotifId = default,
            UIMotifId effectiveMotifId = default,
            long revision = 0,
            int appliedTargetCount = 0,
            int failedTargetCount = 0,
            string message = "")
        {
            Status = status;
            RequestedMotifId = requestedMotifId;
            EffectiveMotifId = effectiveMotifId;
            Revision = revision;
            AppliedTargetCount = appliedTargetCount;
            FailedTargetCount = failedTargetCount;
            Message = message ?? string.Empty;
        }

        public UIMotifSwitchStatus Status { get; }
        public UIMotifId RequestedMotifId { get; }
        public UIMotifId EffectiveMotifId { get; }
        public long Revision { get; }
        public int AppliedTargetCount { get; }
        public int FailedTargetCount { get; }
        public string Message { get; }
        public bool Succeeded =>
            Status == UIMotifSwitchStatus.Applied ||
            Status == UIMotifSwitchStatus.FallbackApplied ||
            Status == UIMotifSwitchStatus.Unchanged;
    }

    /// <summary>
    /// Root-local session truth for one resolved Motif catalog. It owns only
    /// effective selection; visual targets and field application are separate.
    /// </summary>
    public sealed class UIMotifService
    {
        private readonly UIMotifCatalogSnapshot catalog;
        private readonly List<TargetRegistration> targets =
            new List<TargetRegistration>();
        private UIMotifSnapshot effectiveMotif;
        private bool mutationInProgress;
        private bool shutdown;
        private long revision;
        private long nextRegistrationGeneration;

        private sealed class TargetRegistration
        {
            public TargetRegistration(
                IUIMotifTarget target,
                UnityEngine.Object owner,
                long generation,
                UIMotifRegistrationHandle handle)
            {
                Target = target;
                Owner = owner;
                Generation = generation;
                Handle = handle;
            }

            public IUIMotifTarget Target { get; }
            public UnityEngine.Object Owner { get; }
            public long Generation { get; }
            public UIMotifRegistrationHandle Handle { get; }
            public bool IsLost =>
                UIMotifUnityObjectUtility.IsDestroyed(Target) ||
                (!ReferenceEquals(Owner, null) && Owner == null);
        }

        public UIMotifService(UIMotifCatalogSnapshot catalog)
        {
            this.catalog = catalog;
            if (catalog == null)
                return;

            UIMotifResolutionResult initial = catalog.ResolveDefault();
            if (!initial.Succeeded)
                return;

            effectiveMotif = initial.Snapshot;
            revision = 1;
            IsValid = true;
        }

        public bool IsValid { get; private set; }
        public bool IsShutdown => shutdown;
        public UIMotifId EffectiveMotifId =>
            effectiveMotif == null ? default : effectiveMotif.MotifId;
        public UIMotifSnapshot EffectiveMotif => effectiveMotif;
        public long Revision => revision;
        public int RegisteredTargetCount => targets.Count;

        /// <summary>
        /// Publishes identifier-only committed session truth. No token values,
        /// history buffer, or presentation payload are exposed.
        /// </summary>
        public event Action<UIMotifServiceSnapshot> Changed;

        public UIMotifRegistrationHandle RegisterTarget(
            IUIMotifTarget target,
            UnityEngine.Object owner = null)
        {
            long generation = ++nextRegistrationGeneration;
            if (shutdown)
                return RejectedRegistration(UIMotifRegistrationStatus.Shutdown, generation);

            if (!IsValid || mutationInProgress)
                return RejectedRegistration(UIMotifRegistrationStatus.Unavailable, generation);

            if (target == null || UIMotifUnityObjectUtility.IsDestroyed(target))
                return RejectedRegistration(UIMotifRegistrationStatus.InvalidTarget, generation);

            if (!ReferenceEquals(owner, null) && owner == null)
                return RejectedRegistration(UIMotifRegistrationStatus.InvalidTarget, generation);

            UIMotifTargetApplyResult applyResult;
            mutationInProgress = true;
            try
            {
                applyResult = ApplyTarget(target, effectiveMotif);
            }
            finally
            {
                mutationInProgress = false;
            }
            UIMotifRegistrationStatus status = applyResult.Succeeded
                ? UIMotifRegistrationStatus.Registered
                : UIMotifRegistrationStatus.RegisteredWithApplyFailure;
            UIMotifRegistrationResult registrationResult =
                new UIMotifRegistrationResult(status, generation, applyResult);
            UIMotifRegistrationHandle handle =
                new UIMotifRegistrationHandle(this, generation, registrationResult);
            targets.Add(new TargetRegistration(target, owner, generation, handle));
            return handle;
        }

        public int RefreshDestroyedTargets()
        {
            if (shutdown || !IsValid || mutationInProgress)
                return 0;

            int removed = 0;
            for (int i = targets.Count - 1; i >= 0; i--)
            {
                if (!targets[i].IsLost)
                    continue;

                targets.RemoveAt(i);
                removed++;
            }

            return removed;
        }

        public UIMotifServiceSnapshot GetSnapshot() =>
            new UIMotifServiceSnapshot(
                shutdown
                    ? UIMotifServiceState.Shutdown
                    : IsValid
                        ? UIMotifServiceState.Ready
                        : UIMotifServiceState.Unavailable,
                EffectiveMotifId,
                revision);

        public UIMotifSwitchResult Switch(UIMotifId requestedMotifId)
        {
            if (shutdown)
                return Failure(UIMotifSwitchStatus.Shutdown, requestedMotifId, "Motif service is shut down.");

            if (!IsValid || mutationInProgress)
                return Failure(UIMotifSwitchStatus.Unavailable, requestedMotifId, "Motif service is unavailable.");

            UIMotifResolutionResult resolved = catalog.Resolve(requestedMotifId);
            if (!resolved.Succeeded)
                return Failure(UIMotifSwitchStatus.Unavailable, requestedMotifId, "Requested Motif and fallback are unavailable.");

            UIMotifSwitchStatus appliedStatus =
                resolved.Status == UIMotifResolutionStatus.FallbackApplied
                    ? UIMotifSwitchStatus.FallbackApplied
                    : UIMotifSwitchStatus.Applied;

            return Commit(requestedMotifId, resolved.Snapshot, appliedStatus);
        }

        public UIMotifSwitchResult Reset()
        {
            if (shutdown)
                return Failure(UIMotifSwitchStatus.Shutdown, message: "Motif service is shut down.");

            if (!IsValid || mutationInProgress)
                return Failure(UIMotifSwitchStatus.Unavailable, message: "Motif service is unavailable.");

            UIMotifResolutionResult resolved = catalog.ResolveDefault();
            return Commit(catalog.DefaultMotifId, resolved.Snapshot, UIMotifSwitchStatus.Applied);
        }

        public bool Shutdown()
        {
            if (shutdown || !IsValid || mutationInProgress)
                return false;

            shutdown = true;
            IsValid = false;
            effectiveMotif = null;
            targets.Clear();
            revision++;
            mutationInProgress = true;
            try
            {
                Publish();
                Changed = null;
            }
            finally
            {
                mutationInProgress = false;
            }

            return true;
        }

        private UIMotifSwitchResult Commit(
            UIMotifId requestedMotifId,
            UIMotifSnapshot snapshot,
            UIMotifSwitchStatus appliedStatus)
        {
            if (snapshot.MotifId == EffectiveMotifId)
            {
                return new UIMotifSwitchResult(
                    UIMotifSwitchStatus.Unchanged,
                    requestedMotifId,
                    EffectiveMotifId,
                    revision);
            }

            effectiveMotif = snapshot;
            revision++;
            mutationInProgress = true;
            try
            {
                ApplyTargets(
                    snapshot,
                    out int appliedTargetCount,
                    out int failedTargetCount);
                Publish();

                return new UIMotifSwitchResult(
                    appliedStatus,
                    requestedMotifId,
                    EffectiveMotifId,
                    revision,
                    appliedTargetCount,
                    failedTargetCount);
            }
            finally
            {
                mutationInProgress = false;
            }
        }

        internal UIMotifRegistrationReleaseResult Release(
            UIMotifRegistrationHandle handle)
        {
            if (shutdown)
                return new UIMotifRegistrationReleaseResult(
                    UIMotifRegistrationReleaseStatus.Shutdown,
                    handle == null ? 0 : handle.Generation);

            if (!IsValid || mutationInProgress || handle == null)
                return new UIMotifRegistrationReleaseResult(
                    UIMotifRegistrationReleaseStatus.Unavailable,
                    handle == null ? 0 : handle.Generation);

            for (int i = 0; i < targets.Count; i++)
            {
                TargetRegistration registration = targets[i];
                if (registration.Generation != handle.Generation ||
                    !ReferenceEquals(registration.Handle, handle))
                {
                    continue;
                }

                targets.RemoveAt(i);
                return new UIMotifRegistrationReleaseResult(
                    UIMotifRegistrationReleaseStatus.Released,
                    handle.Generation);
            }

            return new UIMotifRegistrationReleaseResult(
                UIMotifRegistrationReleaseStatus.Stale,
                handle.Generation);
        }

        private UIMotifRegistrationHandle RejectedRegistration(
            UIMotifRegistrationStatus status,
            long generation)
        {
            UIMotifRegistrationResult result =
                new UIMotifRegistrationResult(status, generation);
            return new UIMotifRegistrationHandle(null, generation, result);
        }

        private void ApplyTargets(
            UIMotifSnapshot snapshot,
            out int appliedCount,
            out int failedCount)
        {
            appliedCount = 0;
            failedCount = 0;
            int index = 0;
            while (index < targets.Count)
            {
                TargetRegistration registration = targets[index];
                if (registration.IsLost)
                {
                    targets.RemoveAt(index);
                    continue;
                }

                UIMotifTargetApplyResult result =
                    ApplyTarget(registration.Target, snapshot);
                if (result.Succeeded)
                    appliedCount++;
                else
                    failedCount++;

                index++;
            }
        }

        private static UIMotifTargetApplyResult ApplyTarget(
            IUIMotifTarget target,
            UIMotifSnapshot snapshot)
        {
            try
            {
                return target.ApplyMotif(snapshot);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return new UIMotifTargetApplyResult(
                    UIMotifTargetApplyStatus.Failed,
                    failedBindingCount: 1,
                    message: exception.Message);
            }
        }

        private UIMotifSwitchResult Failure(
            UIMotifSwitchStatus status,
            UIMotifId requestedMotifId = default,
            string message = "") =>
            new UIMotifSwitchResult(
                status,
                requestedMotifId,
                EffectiveMotifId,
                revision,
                appliedTargetCount: 0,
                failedTargetCount: 0,
                message: message);

        private void Publish()
        {
            Action<UIMotifServiceSnapshot> handlers = Changed;
            if (handlers == null)
                return;

            UIMotifServiceSnapshot snapshot = GetSnapshot();
            Delegate[] invocationList = handlers.GetInvocationList();
            for (int i = 0; i < invocationList.Length; i++)
            {
                try
                {
                    ((Action<UIMotifServiceSnapshot>)invocationList[i])(snapshot);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }
    }
}
