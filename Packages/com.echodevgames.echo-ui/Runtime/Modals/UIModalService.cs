using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Blocking modal stack, ownership, and exact-once result coordinator.
    /// Gameplay input, pause, time, and cursor authority remain external.
    /// </summary>
    public sealed class UIModalService
    {
        private readonly Dictionary<string, UIModalDefinition> definitions =
            new Dictionary<string, UIModalDefinition>(
                StringComparer.Ordinal);

        private readonly List<UIModalEntry> stack =
            new List<UIModalEntry>();

        private readonly UILayerRegistry layerRegistry;
        private readonly IUIModalFactory factory;
        private readonly Func<string, UISurface> externalViewResolver;
        private readonly Func<UISurface, string> registerRuntimeSurface;
        private readonly Action<UISurface> unregisterRuntimeSurface;
        private readonly Action<UISurface> activateSurface;
        private readonly Action<UISurface> deactivateSurface;
        private readonly Action stackChanged;
        private readonly int capacity;

        private Func<UIModalEntry, UITransitionDirection, Awaitable<UITransitionResult>>
            transitionExecutor;
        private Func<string, bool> cancelTransition;
        private Action<UISurface> prepareEnterSurface;
        private Action<UISurface> completeEnterSurface;

        private long nextGeneration;

        public UIModalService(
            IEnumerable<UIModalDefinition> definitions,
            UILayerRegistry layerRegistry,
            IUIModalFactory factory,
            Func<string, UISurface> externalViewResolver,
            Func<UISurface, string> registerRuntimeSurface,
            Action<UISurface> unregisterRuntimeSurface,
            Action<UISurface> activateSurface,
            Action<UISurface> deactivateSurface,
            Action stackChanged,
            int capacity,
            out string validationError)
        {
            this.layerRegistry = layerRegistry;
            this.factory =
                factory ?? new DefaultUIModalPrefabFactory();

            this.externalViewResolver =
                externalViewResolver;

            this.registerRuntimeSurface =
                registerRuntimeSurface;

            this.unregisterRuntimeSurface =
                unregisterRuntimeSurface;

            this.activateSurface =
                activateSurface;

            this.deactivateSurface =
                deactivateSurface;

            this.stackChanged =
                stackChanged;

            this.capacity =
                capacity < 1
                    ? 1
                    : capacity;

            validationError =
                ValidateAndSnapshotDefinitions(
                    definitions);
        }

        public bool IsValid { get; private set; }

        public int DefinitionCount =>
            definitions.Count;

        public int ActiveCount =>
            stack.Count;

        public int Capacity =>
            capacity;

        public bool HasActiveModals =>
            stack.Count > 0;

        public UIModalEntry TopEntry =>
            stack.Count == 0
                ? null
                : stack[stack.Count - 1];

        internal UIModalEntry TopInteractiveEntry
        {
            get
            {
                UIModalEntry top =
                    TopEntry;

                return top != null &&
                    top.AcceptsInteraction
                        ? top
                        : null;
            }
        }

        internal void ConfigureTransitionLifecycle(
            Func<UIModalEntry, UITransitionDirection, Awaitable<UITransitionResult>>
                executor,
            Func<string, bool> cancel,
            Action<UISurface> prepareEnter,
            Action<UISurface> completeEnter)
        {
            transitionExecutor =
                executor;

            cancelTransition =
                cancel;

            prepareEnterSurface =
                prepareEnter;

            completeEnterSurface =
                completeEnter;
        }

        public bool TryGetDefinition(
            string modalId,
            out UIModalDefinition definition)
        {
            definition = null;

            UIModalId normalized =
                new UIModalId(modalId);

            return normalized.IsValid &&
                definitions.TryGetValue(
                    normalized.Value,
                    out definition);
        }

        public UIModalHandle Open(
            string modalId)
        {
            UIModalId normalized =
                new UIModalId(modalId);

            long generation =
                ++nextGeneration;

            if (!IsValid)
            {
                return UIModalHandle.Rejected(
                    normalized,
                    generation,
                    "Modal lifecycle is not valid.");
            }

            if (!normalized.IsValid ||
                !definitions.TryGetValue(
                    normalized.Value,
                    out UIModalDefinition definition))
            {
                return UIModalHandle.Rejected(
                    normalized,
                    generation,
                    "No Modal definition matches the requested stable ID.");
            }

            if (stack.Count >= capacity)
            {
                return UIModalHandle.Rejected(
                    normalized,
                    generation,
                    "Blocking Modal capacity is full.");
            }

            if (FindEntryIndex(
                    normalized,
                    generation: 0,
                    requireGeneration: false) >= 0)
            {
                return UIModalHandle.Rejected(
                    normalized,
                    generation,
                    "The requested Modal definition already has a live generation.");
            }

            if (!TryPrepareEntry(
                    definition,
                    generation,
                    out UIModalEntry entry,
                    out string error))
            {
                return UIModalHandle.Rejected(
                    normalized,
                    generation,
                    error);
            }

            stack.Add(
                entry);

            if (transitionExecutor == null)
            {
                entry.IsInteractive = true;

                activateSurface?.Invoke(
                    entry.View);

                stackChanged?.Invoke();

                return entry.Handle;
            }

            entry.IsEntering = true;
            entry.IsInteractive = false;

            if (prepareEnterSurface != null)
            {
                prepareEnterSurface(
                    entry.View);
            }
            else
            {
                activateSurface?.Invoke(
                    entry.View);
            }

            stackChanged?.Invoke();

            BeginEnterTransition(
                entry);

            return entry.Handle;
        }

        public UIModalCompletionAttemptResult Complete(
            UIModalHandle handle,
            string resultId)
        {
            UIModalResultId normalizedResult =
                new UIModalResultId(resultId);

            if (!normalizedResult.IsValid)
            {
                return new UIModalCompletionAttemptResult(
                    UIModalCompletionStatus.InvalidResult,
                    "Semantic modal completion requires a nonempty project-defined result ID.");
            }

            return ClaimTerminal(
                handle,
                new UIModalResult(
                    UIModalOutcome.Completed,
                    handle == null
                        ? new UIModalId(string.Empty)
                        : handle.ModalId,
                    handle == null
                        ? 0
                        : handle.Generation,
                    normalizedResult),
                UIModalAbortReason.None,
                requireInteractive: true);
        }

        public UIModalCompletionAttemptResult Abort(
            UIModalHandle handle,
            UIModalAbortReason reason)
        {
            if (reason == UIModalAbortReason.None)
            {
                reason =
                    UIModalAbortReason.ExplicitAbort;
            }

            return ClaimTerminal(
                handle,
                new UIModalResult(
                    UIModalOutcome.Aborted,
                    handle == null
                        ? new UIModalId(string.Empty)
                        : handle.ModalId,
                    handle == null
                        ? 0
                        : handle.Generation,
                    abortReason: reason),
                reason,
                requireInteractive: false);
        }

        public UIModalCompletionAttemptResult HandleBack()
        {
            UIModalEntry top =
                TopEntry;

            if (top == null)
            {
                return new UIModalCompletionAttemptResult(
                    UIModalCompletionStatus.NotFound,
                    "No blocking Modal is active.");
            }

            UIModalBackPolicy policy =
                top.Definition.BackPolicy;

            if (policy == null ||
                policy.Behavior ==
                    UIModalBackBehavior.Disabled)
            {
                return new UIModalCompletionAttemptResult(
                    UIModalCompletionStatus.BackDisabled,
                    "The top blocking Modal does not allow Back dismissal.");
            }

            if (!policy.ResultId.IsValid)
            {
                return new UIModalCompletionAttemptResult(
                    UIModalCompletionStatus.InvalidResult,
                    "Back completion policy has no valid project-defined result ID.");
            }

            return Complete(
                top.Handle,
                policy.ResultId.Value);
        }

        public bool AbortByModalId(
            string modalId,
            UIModalAbortReason reason)
        {
            UIModalId normalized =
                new UIModalId(modalId);

            int index =
                FindEntryIndex(
                    normalized,
                    generation: 0,
                    requireGeneration: false);

            if (index < 0)
            {
                return false;
            }

            UIModalEntry entry =
                stack[index];

            Abort(
                entry.Handle,
                reason);

            return true;
        }

        public bool SweepLostViews()
        {
            bool changed = false;

            for (int index = stack.Count - 1;
                 index >= 0;
                 index--)
            {
                UIModalEntry entry =
                    stack[index];

                if (entry.View != null)
                {
                    continue;
                }

                stack.RemoveAt(
                    index);

                cancelTransition?.Invoke(
                    entry.ModalId.Value);

                if (entry.HasTerminalClaim)
                {
                    entry.Handle.TryComplete(
                        entry.ClaimedResult);
                }
                else
                {
                    entry.Handle.TryComplete(
                        new UIModalResult(
                            UIModalOutcome.Aborted,
                            entry.ModalId,
                            entry.Generation,
                            abortReason:
                                UIModalAbortReason.ViewLost,
                            message:
                                "Modal view was lost after admission."));
                }

                changed = true;
            }

            if (changed)
            {
                stackChanged?.Invoke();
            }

            return changed;
        }

        public void Shutdown(
            UIModalAbortReason reason =
                UIModalAbortReason.RootShutdown)
        {
            for (int index = stack.Count - 1;
                 index >= 0;
                 index--)
            {
                UIModalEntry entry =
                    stack[index];

                stack.RemoveAt(
                    index);

                cancelTransition?.Invoke(
                    entry.ModalId.Value);

                if (entry.HasTerminalClaim)
                {
                    entry.Handle.TryComplete(
                        entry.ClaimedResult);
                }
                else
                {
                    entry.Handle.TryComplete(
                        new UIModalResult(
                            UIModalOutcome.Aborted,
                            entry.ModalId,
                            entry.Generation,
                            abortReason: reason,
                            message:
                                "Modal lifecycle shut down before semantic completion."));
                }

                ReleaseEntry(
                    entry);
            }

            stackChanged?.Invoke();
        }

        private string ValidateAndSnapshotDefinitions(
            IEnumerable<UIModalDefinition> source)
        {
            if (layerRegistry == null ||
                layerRegistry.Count == 0)
            {
                return
                    "Blocking Modal lifecycle requires the existing project-authored layer registry.";
            }

            if (source == null)
            {
                return
                    "Blocking Modal definitions are required.";
            }

            foreach (UIModalDefinition authored in source)
            {
                if (authored == null)
                {
                    return
                        "A blocking Modal definition reference is missing.";
                }

                UIModalDefinition definition =
                    authored.Snapshot();

                if (!definition.ModalId.IsValid)
                {
                    return
                        "Modal IDs must be nonempty stable project-authored values.";
                }

                if (!definition.TargetLayerId.IsValid ||
                    !layerRegistry.TryGetHost(
                        definition.TargetLayerId.Value,
                        out _))
                {
                    return
                        "Modal '" +
                        definition.ModalId.Value +
                        "' references a missing target layer.";
                }

                if (definitions.ContainsKey(
                        definition.ModalId.Value))
                {
                    return
                        "Duplicate Modal definition ID: " +
                        definition.ModalId.Value;
                }

                switch (definition.OwnershipMode)
                {
                    case UIScreenOwnershipMode.RootOwned:
                        if (definition.RootOwnedPrefab == null)
                        {
                            return
                                "RootOwned Modal '" +
                                definition.ModalId.Value +
                                "' requires a prefab.";
                        }
                        break;

                    case UIScreenOwnershipMode.SceneOwned:
                        if (definition.SceneOwnedView == null)
                        {
                            return
                                "SceneOwned Modal '" +
                                definition.ModalId.Value +
                                "' requires a scene view.";
                        }

                        string sceneViewError =
                            ValidateView(
                                definition,
                                definition.SceneOwnedView);

                        if (!string.IsNullOrWhiteSpace(
                                sceneViewError))
                        {
                            return sceneViewError;
                        }
                        break;

                    case UIScreenOwnershipMode.ExternalOwned:
                        break;

                    default:
                        return
                            "Modal '" +
                            definition.ModalId.Value +
                            "' uses an unsupported ownership mode.";
                }

                if (definition.BackPolicy != null)
                {
                    switch (definition.BackPolicy.Behavior)
                    {
                        case UIModalBackBehavior.Disabled:
                            break;

                        case UIModalBackBehavior.CompleteWithResultId:
                            if (!definition.BackPolicy.ResultId.IsValid)
                            {
                                return
                                    "Modal '" +
                                    definition.ModalId.Value +
                                    "' maps Back to an invalid result ID.";
                            }
                            break;

                        default:
                            return
                                "Modal '" +
                                definition.ModalId.Value +
                                "' uses an unsupported Back behavior.";
                    }
                }

                definitions.Add(
                    definition.ModalId.Value,
                    definition);
            }

            if (definitions.Count == 0)
            {
                return
                    "At least one blocking Modal definition is required.";
            }

            IsValid = true;
            return string.Empty;
        }

        private bool TryPrepareEntry(
            UIModalDefinition definition,
            long generation,
            out UIModalEntry entry,
            out string error)
        {
            entry = null;
            error = string.Empty;

            if (!layerRegistry.TryGetHost(
                    definition.TargetLayerId.Value,
                    out UILayerHost layerHost) ||
                layerHost == null)
            {
                error =
                    "Modal target layer is unavailable.";
                return false;
            }

            UISurface view = null;
            bool ownsView = false;

            switch (definition.OwnershipMode)
            {
                case UIScreenOwnershipMode.SceneOwned:
                    view =
                        definition.SceneOwnedView;
                    break;

                case UIScreenOwnershipMode.ExternalOwned:
                    view =
                        externalViewResolver == null
                            ? null
                            : externalViewResolver(
                                definition.ModalId.Value);
                    break;

                case UIScreenOwnershipMode.RootOwned:
                    if (!factory.TryCreate(
                            definition,
                            layerHost,
                            out view,
                            out string createError))
                    {
                        error =
                            string.IsNullOrWhiteSpace(
                                createError)
                                ? "RootOwned Modal factory failed."
                                : createError;
                        return false;
                    }

                    ownsView = true;
                    break;

                default:
                    error =
                        "Unsupported Modal ownership mode.";
                    return false;
            }

            string viewError =
                ValidateView(
                    definition,
                    view);

            if (!string.IsNullOrWhiteSpace(
                    viewError))
            {
                if (ownsView)
                {
                    factory.Release(
                        view);
                }

                error =
                    viewError;

                return false;
            }

            if (ownsView)
            {
                string registrationError =
                    registerRuntimeSurface == null
                        ? "No RootOwned Modal runtime surface registrar is available."
                        : registerRuntimeSurface(
                            view);

                if (!string.IsNullOrWhiteSpace(
                        registrationError))
                {
                    factory.Release(
                        view);

                    error =
                        registrationError;

                    return false;
                }
            }

            UIModalHandle handle =
                new UIModalHandle(
                    definition.ModalId,
                    generation,
                    true);

            entry =
                new UIModalEntry(
                    definition,
                    view,
                    ownsView,
                    handle);

            return true;
        }

        private UIModalCompletionAttemptResult ClaimTerminal(
            UIModalHandle handle,
            UIModalResult result,
            UIModalAbortReason abortReason,
            bool requireInteractive)
        {
            if (handle == null)
            {
                return new UIModalCompletionAttemptResult(
                    UIModalCompletionStatus.NotFound,
                    "Modal handle is missing.");
            }

            if (handle.IsCompleted)
            {
                return new UIModalCompletionAttemptResult(
                    UIModalCompletionStatus.AlreadyCompleted,
                    "This Modal generation already has a terminal result.");
            }

            int index =
                FindEntryIndex(
                    handle.ModalId,
                    handle.Generation,
                    requireGeneration: true);

            if (index < 0)
            {
                return new UIModalCompletionAttemptResult(
                    UIModalCompletionStatus.StaleHandle,
                    "The Modal handle does not address a live generation.");
            }

            UIModalEntry entry =
                stack[index];

            if (!ReferenceEquals(
                    entry.Handle,
                    handle))
            {
                return new UIModalCompletionAttemptResult(
                    UIModalCompletionStatus.StaleHandle,
                    "The Modal handle is stale.");
            }

            if (entry.HasTerminalClaim)
            {
                return new UIModalCompletionAttemptResult(
                    UIModalCompletionStatus.AlreadyCompleted,
                    "This Modal generation already has a first terminal claim.");
            }

            if (requireInteractive &&
                !entry.AcceptsInteraction)
            {
                return new UIModalCompletionAttemptResult(
                    UIModalCompletionStatus.NotReady,
                    "Semantic Modal completion is unavailable until enter transition settlement establishes an interactive top Modal.");
            }

            if (result.Outcome ==
                    UIModalOutcome.Aborted &&
                abortReason == UIModalAbortReason.None)
            {
                result =
                    new UIModalResult(
                        UIModalOutcome.Aborted,
                        entry.ModalId,
                        entry.Generation,
                        abortReason:
                            UIModalAbortReason.ExplicitAbort);
            }

            entry.HasTerminalClaim = true;
            entry.ClaimedResult = result;
            entry.IsEntering = false;
            entry.IsInteractive = false;
            entry.IsClosing = true;

            stackChanged?.Invoke();

            if (transitionExecutor == null ||
                entry.View == null)
            {
                FinalizeClaimedSettlement(
                    entry);

                return new UIModalCompletionAttemptResult(
                    UIModalCompletionStatus.Succeeded,
                    "Modal generation settled exactly once.");
            }

            BeginExitTransition(
                entry);

            return new UIModalCompletionAttemptResult(
                UIModalCompletionStatus.Succeeded,
                "Modal first terminal result was claimed exactly once; structural release waits for exit transition settlement.");
        }

        private async void BeginEnterTransition(
            UIModalEntry entry)
        {
            if (!IsLiveEntry(
                    entry) ||
                transitionExecutor == null)
            {
                return;
            }

            UITransitionResult transition;

            try
            {
                transition =
                    await transitionExecutor(
                        entry,
                        UITransitionDirection.Enter);
            }
            catch (Exception exception)
            {
                transition =
                    new UITransitionResult(
                        UITransitionStatus.Failed,
                        default,
                        0,
                        entry.ModalId.Value,
                        UITransitionDirection.Enter,
                        string.Empty,
                        string.Empty,
                        0d,
                        exception.Message);
            }

            if (!IsLiveEntry(
                    entry))
            {
                return;
            }

            if (entry.HasTerminalClaim ||
                entry.IsClosing)
            {
                return;
            }

            entry.IsEntering = false;

            if (!transition.Succeeded)
            {
                RemoveEntry(
                    entry);

                entry.Handle.TryComplete(
                    new UIModalResult(
                        UIModalOutcome.Aborted,
                        entry.ModalId,
                        entry.Generation,
                        abortReason:
                            UIModalAbortReason.TransitionFailed,
                        message:
                            BuildTransitionMessage(
                                "Modal enter transition failed",
                                transition)));

                ReleaseEntry(
                    entry);

                stackChanged?.Invoke();
                return;
            }

            entry.IsInteractive = true;

            stackChanged?.Invoke();

            completeEnterSurface?.Invoke(
                entry.View);
        }

        private async void BeginExitTransition(
            UIModalEntry entry)
        {
            if (!IsLiveEntry(
                    entry))
            {
                return;
            }

            if (transitionExecutor != null &&
                entry.View != null)
            {
                try
                {
                    await transitionExecutor(
                        entry,
                        UITransitionDirection.Exit);
                }
                catch
                {
                    // Exit presentation can fail, time out, cancel, or throw.
                    // The exact-once claimed result remains authoritative and
                    // deterministic structural release still wins.
                }
            }

            if (!IsLiveEntry(
                    entry))
            {
                return;
            }

            FinalizeClaimedSettlement(
                entry);
        }

        private void FinalizeClaimedSettlement(
            UIModalEntry entry)
        {
            if (!IsLiveEntry(
                    entry))
            {
                return;
            }

            RemoveEntry(
                entry);

            ReleaseEntry(
                entry);

            entry.Handle.TryComplete(
                entry.ClaimedResult);

            stackChanged?.Invoke();
        }

        private void RemoveEntry(
            UIModalEntry entry)
        {
            if (entry == null)
            {
                return;
            }

            int index =
                FindEntryIndex(
                    entry.ModalId,
                    entry.Generation,
                    requireGeneration: true);

            if (index >= 0 &&
                ReferenceEquals(
                    stack[index],
                    entry))
            {
                stack.RemoveAt(
                    index);
            }
        }

        private bool IsLiveEntry(
            UIModalEntry entry)
        {
            if (entry == null)
            {
                return false;
            }

            int index =
                FindEntryIndex(
                    entry.ModalId,
                    entry.Generation,
                    requireGeneration: true);

            return index >= 0 &&
                ReferenceEquals(
                    stack[index],
                    entry);
        }

        private static string BuildTransitionMessage(
            string prefix,
            UITransitionResult transition)
        {
            string normalized =
                string.IsNullOrWhiteSpace(
                    prefix)
                    ? "Modal transition failed"
                    : prefix.Trim();

            return normalized +
                " (" +
                transition.Status +
                ")" +
                (string.IsNullOrWhiteSpace(
                    transition.Message)
                    ? "."
                    : ": " +
                        transition.Message);
        }

        private void ReleaseEntry(
            UIModalEntry entry)
        {
            if (entry == null)
            {
                return;
            }

            UISurface view =
                entry.View;

            if (view != null)
            {
                deactivateSurface?.Invoke(
                    view);
            }

            if (!entry.LookingGlassOwnsView)
            {
                return;
            }

            if (view != null)
            {
                unregisterRuntimeSurface?.Invoke(
                    view);

                factory.Release(
                    view);
            }

            entry.View = null;
        }

        private int FindEntryIndex(
            UIModalId modalId,
            long generation,
            bool requireGeneration)
        {
            if (!modalId.IsValid)
            {
                return -1;
            }

            for (int index = stack.Count - 1;
                 index >= 0;
                 index--)
            {
                UIModalEntry entry =
                    stack[index];

                if (entry.ModalId != modalId)
                {
                    continue;
                }

                if (!requireGeneration ||
                    entry.Generation == generation)
                {
                    return index;
                }
            }

            return -1;
        }

        private static string ValidateView(
            UIModalDefinition definition,
            UISurface view)
        {
            if (view == null)
            {
                return
                    "Modal '" +
                    definition.ModalId.Value +
                    "' has no live view for ownership mode " +
                    definition.OwnershipMode +
                    ".";
            }

            if (view.Role != UISurfaceRole.Modal)
            {
                return
                    "Modal '" +
                    definition.ModalId.Value +
                    "' resolved a UISurface that is not configured with Modal role.";
            }

            if (!string.Equals(
                    view.SurfaceId,
                    definition.ModalId.Value,
                    StringComparison.Ordinal))
            {
                return
                    "Modal view stable ID does not match definition '" +
                    definition.ModalId.Value +
                    "'.";
            }

            return string.Empty;
        }
    }
}
