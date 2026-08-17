using System;
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
            string message = "")
        {
            Status = status;
            RequestedMotifId = requestedMotifId;
            EffectiveMotifId = effectiveMotifId;
            Revision = revision;
            Message = message ?? string.Empty;
        }

        public UIMotifSwitchStatus Status { get; }
        public UIMotifId RequestedMotifId { get; }
        public UIMotifId EffectiveMotifId { get; }
        public long Revision { get; }
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
        private UIMotifSnapshot effectiveMotif;
        private bool mutationInProgress;
        private bool shutdown;
        private long revision;

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

        /// <summary>
        /// Publishes identifier-only committed session truth. No token values,
        /// history buffer, or presentation payload are exposed.
        /// </summary>
        public event Action<UIMotifServiceSnapshot> Changed;

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
                Publish();
            }
            finally
            {
                mutationInProgress = false;
            }

            return new UIMotifSwitchResult(
                appliedStatus,
                requestedMotifId,
                EffectiveMotifId,
                revision);
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
                message);

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
