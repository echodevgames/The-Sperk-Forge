using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Authoritative per-scope Screen history and ownership coordinator.
    /// View presentation is delegated back to EchoUIRoot so M1 context/selection
    /// behavior remains the single surface presentation path.
    /// </summary>
    public sealed class UIScreenNavigator
    {
        private readonly Dictionary<string, UIScreenDefinition> definitions;
        private readonly Dictionary<string, List<UIScreenEntry>> historyByScope =
            new Dictionary<string, List<UIScreenEntry>>(
                StringComparer.Ordinal);

        private readonly UILayerRegistry layerRegistry;
        private readonly IUIScreenFactory factory;
        private readonly Func<string, UISurface> externalViewResolver;
        private readonly Func<UISurface, string> registerRuntimeSurface;
        private readonly Action<UISurface> unregisterRuntimeSurface;
        private readonly Action<UIScreenEntry> activateEntry;
        private readonly Action<UIScreenEntry> suspendEntry;
        private readonly Action<UIScreenEntry> resumeEntry;
        private readonly Action<UIScreenEntry> closeEntry;
        private Func<UIScreenEntry, UITransitionDirection, Awaitable<UITransitionResult>>
            transitionExecutor;

        public UIScreenNavigator(
            IEnumerable<UIScreenDefinition> definitions,
            UILayerRegistry layerRegistry,
            IUIScreenFactory factory,
            Func<string, UISurface> externalViewResolver,
            Func<UISurface, string> registerRuntimeSurface,
            Action<UISurface> unregisterRuntimeSurface,
            Action<UIScreenEntry> activateEntry,
            Action<UIScreenEntry> suspendEntry,
            Action<UIScreenEntry> resumeEntry,
            Action<UIScreenEntry> closeEntry,
            out string validationError)
        {
            this.layerRegistry = layerRegistry;
            this.factory =
                factory ?? new DefaultUIScreenPrefabFactory();
            this.externalViewResolver =
                externalViewResolver;
            this.registerRuntimeSurface =
                registerRuntimeSurface;
            this.unregisterRuntimeSurface =
                unregisterRuntimeSurface;
            this.activateEntry =
                activateEntry;
            this.suspendEntry =
                suspendEntry;
            this.resumeEntry =
                resumeEntry;
            this.closeEntry =
                closeEntry;

            this.definitions =
                new Dictionary<string, UIScreenDefinition>(
                    StringComparer.Ordinal);

            validationError =
                ValidateAndSnapshotDefinitions(
                    definitions);
        }

        public int DefinitionCount =>
            definitions.Count;

        public bool IsValid { get; private set; }

        public void SetTransitionExecutor(
            Func<UIScreenEntry, UITransitionDirection, Awaitable<UITransitionResult>>
                executor)
        {
            transitionExecutor = executor;
        }

        public bool TryGetDefinition(
            string screenId,
            out UIScreenDefinition definition)
        {
            definition = null;
            string normalized =
                Normalize(screenId);

            return !string.IsNullOrWhiteSpace(normalized) &&
                definitions.TryGetValue(
                    normalized,
                    out definition);
        }

        public bool HasDefinition(
            string screenId)
        {
            string normalized =
                Normalize(screenId);

            return !string.IsNullOrWhiteSpace(normalized) &&
                definitions.ContainsKey(normalized);
        }

        public bool HasScope(
            string scopeId)
        {
            string normalized =
                Normalize(scopeId);

            if (string.IsNullOrWhiteSpace(normalized))
            {
                return false;
            }

            foreach (UIScreenDefinition definition in definitions.Values)
            {
                if (string.Equals(
                        definition.NavigationScopeId,
                        normalized,
                        StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        public string GetCurrentScreenId(
            string scopeId)
        {
            UIScreenEntry entry =
                GetCurrentEntry(scopeId);

            return entry == null
                ? string.Empty
                : entry.ScreenId;
        }

        public UIScreenEntry GetCurrentEntry(
            string scopeId)
        {
            string normalized =
                Normalize(scopeId);

            if (string.IsNullOrWhiteSpace(normalized) ||
                !historyByScope.TryGetValue(
                    normalized,
                    out List<UIScreenEntry> history) ||
                history.Count == 0)
            {
                return null;
            }

            return history[history.Count - 1];
        }

        public int GetHistoryDepth(
            string scopeId)
        {
            string normalized =
                Normalize(scopeId);

            return historyByScope.TryGetValue(
                    normalized,
                    out List<UIScreenEntry> history)
                ? history.Count
                : 0;
        }

        public UIScreenOperationResult Execute(
            UIScreenOperationRequest request)
        {
            if (request == null)
            {
                return new UIScreenOperationResult(
                    UIScreenOperationStatus.Invalid,
                    UIScreenOperationKind.Push,
                    message: "Screen operation request is missing.");
            }

            if (!IsValid)
            {
                return new UIScreenOperationResult(
                    UIScreenOperationStatus.Failed,
                    request.Kind,
                    request.ScreenId,
                    request.ScopeId,
                    request.Sequence,
                    "Screen navigator is not valid.");
            }

            switch (request.Kind)
            {
                case UIScreenOperationKind.Push:
                    return Push(request);

                case UIScreenOperationKind.Replace:
                    return Replace(request);

                case UIScreenOperationKind.Reset:
                    return Reset(request);

                case UIScreenOperationKind.Back:
                    return Back(request);

                case UIScreenOperationKind.Close:
                    return Close(request);

                default:
                    return new UIScreenOperationResult(
                        UIScreenOperationStatus.Invalid,
                        request.Kind,
                        request.ScreenId,
                        request.ScopeId,
                        request.Sequence,
                        "Unsupported Screen operation kind.");
            }
        }


        public async Awaitable<UIScreenOperationResult> ExecuteAsync(
            UIScreenOperationRequest request)
        {
            if (transitionExecutor == null)
            {
                return Execute(
                    request);
            }

            if (request == null)
            {
                return new UIScreenOperationResult(
                    UIScreenOperationStatus.Invalid,
                    UIScreenOperationKind.Push,
                    message:
                        "Screen operation request is missing.");
            }

            if (!IsValid)
            {
                return new UIScreenOperationResult(
                    UIScreenOperationStatus.Failed,
                    request.Kind,
                    request.ScreenId,
                    request.ScopeId,
                    request.Sequence,
                    "Screen navigator is not valid.");
            }

            switch (request.Kind)
            {
                case UIScreenOperationKind.Push:
                    return await PushAsync(
                        request);

                case UIScreenOperationKind.Replace:
                    return await ReplaceAsync(
                        request);

                case UIScreenOperationKind.Reset:
                    return await ResetAsync(
                        request);

                case UIScreenOperationKind.Back:
                    return await BackAsync(
                        request);

                case UIScreenOperationKind.Close:
                    return await CloseAsync(
                        request);

                default:
                    return new UIScreenOperationResult(
                        UIScreenOperationStatus.Invalid,
                        request.Kind,
                        request.ScreenId,
                        request.ScopeId,
                        request.Sequence,
                        "Unsupported Screen operation kind.");
            }
        }

        private async Awaitable<UIScreenOperationResult> PushAsync(
            UIScreenOperationRequest request)
        {
            if (!TryResolveDefinition(
                    request.ScreenId,
                    request,
                    out UIScreenDefinition definition,
                    out UIScreenOperationResult failure))
            {
                return failure;
            }

            List<UIScreenEntry> history =
                GetOrCreateHistory(
                    definition.NavigationScopeId);

            UIScreenEntry current =
                history.Count == 0
                    ? null
                    : history[history.Count - 1];

            if (current != null &&
                string.Equals(
                    current.ScreenId,
                    definition.ScreenId,
                    StringComparison.Ordinal))
            {
                return new UIScreenOperationResult(
                    UIScreenOperationStatus.NoChange,
                    request.Kind,
                    definition.ScreenId,
                    definition.NavigationScopeId,
                    request.Sequence,
                    "Requested Screen is already current.");
            }

            if (ContainsScreen(
                    history,
                    definition.ScreenId))
            {
                return new UIScreenOperationResult(
                    UIScreenOperationStatus.Invalid,
                    request.Kind,
                    definition.ScreenId,
                    definition.NavigationScopeId,
                    request.Sequence,
                    "Push cannot duplicate an existing Screen entry in the same scope history.");
            }

            if (!TryPrepareEntry(
                    definition,
                    request,
                    out UIScreenEntry target,
                    out failure))
            {
                return failure;
            }

            target.IsActive = true;
            target.IsSuspended = false;

            activateEntry?.Invoke(
                target);

            UITransitionResult enter =
                await RunTransitionAsync(
                    target,
                    UITransitionDirection.Enter);

            if (!enter.Succeeded)
            {
                ReleaseEntry(
                    target,
                    hideExternalOrScene: true);

                RestoreStableEntryAfterFailedEnter(
                    current);

                return TransitionFailure(
                    request,
                    target.ScreenId,
                    target.NavigationScopeId,
                    "Screen Push enter transition failed",
                    enter);
            }

            UITransitionResult exit =
                current == null
                    ? default
                    : await RunTransitionAsync(
                        current,
                        UITransitionDirection.Exit);

            if (current != null)
            {
                current.IsActive = false;
                current.IsSuspended = true;

                suspendEntry?.Invoke(
                    current);
            }

            history.Add(
                target);

            return UIScreenOperationResult.Success(
                request,
                target.ScreenId,
                target.NavigationScopeId,
                AppendExitDiagnostic(
                    "Screen Push completed.",
                    exit,
                    current != null));
        }

        private async Awaitable<UIScreenOperationResult> ReplaceAsync(
            UIScreenOperationRequest request)
        {
            if (!TryResolveDefinition(
                    request.ScreenId,
                    request,
                    out UIScreenDefinition definition,
                    out UIScreenOperationResult failure))
            {
                return failure;
            }

            List<UIScreenEntry> history =
                GetOrCreateHistory(
                    definition.NavigationScopeId);

            UIScreenEntry current =
                history.Count == 0
                    ? null
                    : history[history.Count - 1];

            if (current != null &&
                string.Equals(
                    current.ScreenId,
                    definition.ScreenId,
                    StringComparison.Ordinal))
            {
                return new UIScreenOperationResult(
                    UIScreenOperationStatus.NoChange,
                    request.Kind,
                    definition.ScreenId,
                    definition.NavigationScopeId,
                    request.Sequence,
                    "Requested Screen is already current.");
            }

            if (ContainsScreen(
                    history,
                    definition.ScreenId))
            {
                return new UIScreenOperationResult(
                    UIScreenOperationStatus.Invalid,
                    request.Kind,
                    definition.ScreenId,
                    definition.NavigationScopeId,
                    request.Sequence,
                    "Replace cannot duplicate an existing Screen entry in the same scope history.");
            }

            if (!TryPrepareEntry(
                    definition,
                    request,
                    out UIScreenEntry target,
                    out failure))
            {
                return failure;
            }

            target.IsActive = true;
            target.IsSuspended = false;

            activateEntry?.Invoke(
                target);

            UITransitionResult enter =
                await RunTransitionAsync(
                    target,
                    UITransitionDirection.Enter);

            if (!enter.Succeeded)
            {
                ReleaseEntry(
                    target,
                    hideExternalOrScene: true);

                RestoreStableEntryAfterFailedEnter(
                    current);

                return TransitionFailure(
                    request,
                    target.ScreenId,
                    target.NavigationScopeId,
                    "Screen Replace enter transition failed",
                    enter);
            }

            UITransitionResult exit =
                current == null
                    ? default
                    : await RunTransitionAsync(
                        current,
                        UITransitionDirection.Exit);

            if (current != null)
            {
                history.RemoveAt(
                    history.Count - 1);

                ReleaseEntry(
                    current,
                    hideExternalOrScene: true);
            }

            history.Add(
                target);

            return UIScreenOperationResult.Success(
                request,
                target.ScreenId,
                target.NavigationScopeId,
                AppendExitDiagnostic(
                    "Screen Replace completed.",
                    exit,
                    current != null));
        }

        private async Awaitable<UIScreenOperationResult> ResetAsync(
            UIScreenOperationRequest request)
        {
            if (!TryResolveDefinition(
                    request.ScreenId,
                    request,
                    out UIScreenDefinition definition,
                    out UIScreenOperationResult failure))
            {
                return failure;
            }

            List<UIScreenEntry> history =
                GetOrCreateHistory(
                    definition.NavigationScopeId);

            if (history.Count == 1 &&
                string.Equals(
                    history[0].ScreenId,
                    definition.ScreenId,
                    StringComparison.Ordinal))
            {
                return new UIScreenOperationResult(
                    UIScreenOperationStatus.NoChange,
                    request.Kind,
                    definition.ScreenId,
                    definition.NavigationScopeId,
                    request.Sequence,
                    "Scope is already reset to the requested root Screen.");
            }

            UIScreenEntry current =
                history.Count == 0
                    ? null
                    : history[history.Count - 1];

            int existingIndex =
                FindEntryIndex(
                    history,
                    definition.ScreenId);

            UIScreenEntry target =
                existingIndex >= 0
                    ? history[existingIndex]
                    : null;

            bool targetAlreadyInHistory =
                target != null &&
                target.View != null;

            if (!targetAlreadyInHistory &&
                !TryPrepareEntry(
                    definition,
                    request,
                    out target,
                    out failure))
            {
                return failure;
            }

            target.IsActive = true;
            target.IsSuspended = false;

            if (targetAlreadyInHistory)
            {
                resumeEntry?.Invoke(
                    target);
            }
            else
            {
                activateEntry?.Invoke(
                    target);
            }

            UITransitionResult enter =
                await RunTransitionAsync(
                    target,
                    UITransitionDirection.Enter);

            if (!enter.Succeeded)
            {
                if (targetAlreadyInHistory)
                {
                    target.IsActive = false;
                    target.IsSuspended = true;

                    suspendEntry?.Invoke(
                        target);
                }
                else
                {
                    ReleaseEntry(
                        target,
                        hideExternalOrScene: true);
                }

                RestoreStableEntryAfterFailedEnter(
                    current);

                return TransitionFailure(
                    request,
                    definition.ScreenId,
                    definition.NavigationScopeId,
                    "Screen Reset enter transition failed",
                    enter);
            }

            UITransitionResult exit =
                current == null ||
                ReferenceEquals(
                    current,
                    target)
                    ? default
                    : await RunTransitionAsync(
                        current,
                        UITransitionDirection.Exit);

            List<UIScreenEntry> previous =
                new List<UIScreenEntry>(
                    history);

            history.Clear();

            for (int index = previous.Count - 1;
                 index >= 0;
                 index--)
            {
                UIScreenEntry entry =
                    previous[index];

                if (ReferenceEquals(
                        entry,
                        target))
                {
                    continue;
                }

                ReleaseEntry(
                    entry,
                    hideExternalOrScene: true);
            }

            target.IsActive = true;
            target.IsSuspended = false;

            history.Add(
                target);

            return UIScreenOperationResult.Success(
                request,
                target.ScreenId,
                target.NavigationScopeId,
                AppendExitDiagnostic(
                    "Screen scope Reset completed.",
                    exit,
                    current != null &&
                    !ReferenceEquals(
                        current,
                        target)));
        }

        private async Awaitable<UIScreenOperationResult> BackAsync(
            UIScreenOperationRequest request)
        {
            string scopeId =
                Normalize(
                    request.ScopeId);

            if (string.IsNullOrWhiteSpace(
                    scopeId))
            {
                return new UIScreenOperationResult(
                    UIScreenOperationStatus.Invalid,
                    request.Kind,
                    scopeId: scopeId,
                    sequence: request.Sequence,
                    message:
                        "Back requires a navigation scope ID.");
            }

            if (!historyByScope.TryGetValue(
                    scopeId,
                    out List<UIScreenEntry> history) ||
                history.Count <= 1)
            {
                return new UIScreenOperationResult(
                    UIScreenOperationStatus.NoChange,
                    request.Kind,
                    scopeId: scopeId,
                    sequence: request.Sequence,
                    message:
                        "No previous Screen is available in this scope.");
            }

            int previousIndex =
                FindPreviousValidIndex(
                    history);

            if (previousIndex < 0)
            {
                PruneInvalidBelowCurrent(
                    history);

                return new UIScreenOperationResult(
                    UIScreenOperationStatus.NoChange,
                    request.Kind,
                    scopeId: scopeId,
                    sequence: request.Sequence,
                    message:
                        "No valid previous Screen remains in this scope.");
            }

            UIScreenEntry current =
                history[history.Count - 1];

            UIScreenEntry previous =
                history[previousIndex];

            previous.IsActive = true;
            previous.IsSuspended = false;

            resumeEntry?.Invoke(
                previous);

            UITransitionResult enter =
                await RunTransitionAsync(
                    previous,
                    UITransitionDirection.Enter);

            if (!enter.Succeeded)
            {
                previous.IsActive = false;
                previous.IsSuspended = true;

                suspendEntry?.Invoke(
                    previous);

                RestoreStableEntryAfterFailedEnter(
                    current);

                return TransitionFailure(
                    request,
                    previous.ScreenId,
                    scopeId,
                    "Screen Back restore transition failed",
                    enter);
            }

            UITransitionResult exit =
                await RunTransitionAsync(
                    current,
                    UITransitionDirection.Exit);

            List<UIScreenEntry> removed =
                new List<UIScreenEntry>();

            for (int index = history.Count - 1;
                 index > previousIndex;
                 index--)
            {
                removed.Add(
                    history[index]);

                history.RemoveAt(
                    index);
            }

            for (int index = 0;
                 index < removed.Count;
                 index++)
            {
                ReleaseEntry(
                    removed[index],
                    hideExternalOrScene: true);
            }

            previous.IsActive = true;
            previous.IsSuspended = false;

            return UIScreenOperationResult.Success(
                request,
                previous.ScreenId,
                scopeId,
                AppendExitDiagnostic(
                    "Back restored the previous valid Screen.",
                    exit,
                    true));
        }

        private async Awaitable<UIScreenOperationResult> CloseAsync(
            UIScreenOperationRequest request)
        {
            UIScreenEntry current;

            string scopeId =
                Normalize(
                    request.ScopeId);

            if (!string.IsNullOrWhiteSpace(
                    scopeId))
            {
                current =
                    GetCurrentEntry(
                        scopeId);
            }
            else
            {
                if (!TryResolveDefinition(
                        request.ScreenId,
                        request,
                        out UIScreenDefinition definition,
                        out UIScreenOperationResult failure))
                {
                    return failure;
                }

                scopeId =
                    definition.NavigationScopeId;

                current =
                    GetCurrentEntry(
                        scopeId);
            }

            if (current == null)
            {
                return new UIScreenOperationResult(
                    UIScreenOperationStatus.NoChange,
                    request.Kind,
                    request.ScreenId,
                    scopeId,
                    request.Sequence,
                    "No current Screen exists in the requested scope.");
            }

            if (!current.Definition.AllowClose)
            {
                return new UIScreenOperationResult(
                    UIScreenOperationStatus.Rejected,
                    request.Kind,
                    current.ScreenId,
                    scopeId,
                    request.Sequence,
                    "Current Screen definition does not allow explicit Close.");
            }

            if (!string.IsNullOrWhiteSpace(
                    request.ScreenId) &&
                !string.Equals(
                    current.ScreenId,
                    request.ScreenId,
                    StringComparison.Ordinal))
            {
                return new UIScreenOperationResult(
                    UIScreenOperationStatus.Invalid,
                    request.Kind,
                    request.ScreenId,
                    scopeId,
                    request.Sequence,
                    "Close may only target the current Screen in this checkpoint.");
            }

            List<UIScreenEntry> history =
                historyByScope[scopeId];

            int previousIndex =
                FindPreviousValidIndex(
                    history);

            UIScreenEntry previous =
                previousIndex >= 0
                    ? history[previousIndex]
                    : null;

            if (previous != null)
            {
                previous.IsActive = true;
                previous.IsSuspended = false;

                resumeEntry?.Invoke(
                    previous);

                UITransitionResult enter =
                    await RunTransitionAsync(
                        previous,
                        UITransitionDirection.Enter);

                if (!enter.Succeeded)
                {
                    previous.IsActive = false;
                    previous.IsSuspended = true;

                    suspendEntry?.Invoke(
                        previous);

                    RestoreStableEntryAfterFailedEnter(
                        current);

                    return TransitionFailure(
                        request,
                        previous.ScreenId,
                        scopeId,
                        "Screen Close restore transition failed",
                        enter);
                }
            }

            UITransitionResult exit =
                await RunTransitionAsync(
                    current,
                    UITransitionDirection.Exit);

            history.RemoveAt(
                history.Count - 1);

            ReleaseEntry(
                current,
                hideExternalOrScene: true);

            if (previous != null)
            {
                while (history.Count - 1 >
                       previousIndex)
                {
                    UIScreenEntry invalid =
                        history[history.Count - 1];

                    history.RemoveAt(
                        history.Count - 1);

                    if (!ReferenceEquals(
                            invalid,
                            previous))
                    {
                        ReleaseEntry(
                            invalid,
                            hideExternalOrScene: false);
                    }
                }

                previous.IsActive = true;
                previous.IsSuspended = false;
            }
            else
            {
                history.Clear();
            }

            return UIScreenOperationResult.Success(
                request,
                current.ScreenId,
                scopeId,
                AppendExitDiagnostic(
                    "Current Screen closed.",
                    exit,
                    true));
        }

        private async Awaitable<UITransitionResult> RunTransitionAsync(
            UIScreenEntry entry,
            UITransitionDirection direction)
        {
            if (transitionExecutor == null ||
                entry == null ||
                entry.View == null)
            {
                return new UITransitionResult(
                    UITransitionStatus.Unavailable,
                    default,
                    0,
                    entry == null
                        ? string.Empty
                        : entry.ScreenId,
                    direction,
                    string.Empty,
                    string.Empty,
                    0d,
                    "Screen transition executor or live entry is unavailable.");
            }

            return await transitionExecutor(
                entry,
                direction);
        }

        private void RestoreStableEntryAfterFailedEnter(
            UIScreenEntry stable)
        {
            if (stable == null ||
                stable.View == null)
            {
                return;
            }

            stable.IsActive = true;
            stable.IsSuspended = false;

            resumeEntry?.Invoke(
                stable);
        }

        private static UIScreenOperationResult TransitionFailure(
            UIScreenOperationRequest request,
            string screenId,
            string scopeId,
            string prefix,
            UITransitionResult transition)
        {
            return new UIScreenOperationResult(
                UIScreenOperationStatus.Failed,
                request.Kind,
                screenId,
                scopeId,
                request.Sequence,
                prefix +
                ": " +
                transition.Status +
                (string.IsNullOrWhiteSpace(
                    transition.Message)
                    ? string.Empty
                    : " - " +
                        transition.Message));
        }

        private static string AppendExitDiagnostic(
            string successMessage,
            UITransitionResult exit,
            bool hadExit)
        {
            if (!hadExit ||
                exit.Succeeded)
            {
                return successMessage;
            }

            return successMessage +
                " Exit transition settled as " +
                exit.Status +
                "; deterministic lifecycle state still committed.";
        }

        private static int FindEntryIndex(
            List<UIScreenEntry> history,
            string screenId)
        {
            if (history == null ||
                string.IsNullOrWhiteSpace(
                    screenId))
            {
                return -1;
            }

            for (int index = history.Count - 1;
                 index >= 0;
                 index--)
            {
                UIScreenEntry entry =
                    history[index];

                if (entry != null &&
                    string.Equals(
                        entry.ScreenId,
                        screenId,
                        StringComparison.Ordinal))
                {
                    return index;
                }
            }

            return -1;
        }

        public void Shutdown()
        {
            foreach (List<UIScreenEntry> history in historyByScope.Values)
            {
                for (int index = history.Count - 1;
                     index >= 0;
                     index--)
                {
                    ReleaseEntry(
                        history[index],
                        hideExternalOrScene: false);
                }
            }

            historyByScope.Clear();
        }

        private string ValidateAndSnapshotDefinitions(
            IEnumerable<UIScreenDefinition> source)
        {
            if (layerRegistry == null ||
                layerRegistry.Count == 0)
            {
                return "Screen lifecycle requires a valid project-authored layer registry.";
            }

            if (source == null)
            {
                return "Screen definitions are required.";
            }

            foreach (UIScreenDefinition authored in source)
            {
                if (authored == null)
                {
                    return "A Screen definition reference is missing.";
                }

                UIScreenDefinition definition =
                    authored.Snapshot();

                if (string.IsNullOrWhiteSpace(
                        definition.ScreenId))
                {
                    return "Screen IDs must be nonempty stable project-authored values.";
                }

                if (string.IsNullOrWhiteSpace(
                        definition.NavigationScopeId))
                {
                    return
                        "Screen '" +
                        definition.ScreenId +
                        "' requires a navigation scope ID.";
                }

                if (!definition.TargetLayerId.IsValid ||
                    !layerRegistry.TryGetHost(
                        definition.TargetLayerId.Value,
                        out _))
                {
                    return
                        "Screen '" +
                        definition.ScreenId +
                        "' references missing layer '" +
                        definition.TargetLayerId.Value +
                        "'.";
                }

                if (definitions.ContainsKey(
                        definition.ScreenId))
                {
                    return
                        "Duplicate Screen definition ID: " +
                        definition.ScreenId;
                }

                switch (definition.OwnershipMode)
                {
                    case UIScreenOwnershipMode.RootOwned:
                        if (definition.RootOwnedPrefab == null)
                        {
                            return
                                "RootOwned Screen '" +
                                definition.ScreenId +
                                "' requires a prefab.";
                        }
                        break;

                    case UIScreenOwnershipMode.SceneOwned:
                        if (definition.SceneOwnedView == null)
                        {
                            return
                                "SceneOwned Screen '" +
                                definition.ScreenId +
                                "' requires a scene view.";
                        }

                        string sceneViewError =
                            ValidateView(
                                definition,
                                definition.SceneOwnedView);

                        if (!string.IsNullOrEmpty(
                                sceneViewError))
                        {
                            return sceneViewError;
                        }
                        break;

                    case UIScreenOwnershipMode.ExternalOwned:
                        break;

                    default:
                        return
                            "Screen '" +
                            definition.ScreenId +
                            "' uses an unsupported ownership mode.";
                }

                definitions.Add(
                    definition.ScreenId,
                    definition);
            }

            if (definitions.Count == 0)
            {
                return "At least one Screen definition is required.";
            }

            IsValid = true;
            return string.Empty;
        }

        private UIScreenOperationResult Push(
            UIScreenOperationRequest request)
        {
            if (!TryResolveDefinition(
                    request.ScreenId,
                    request,
                    out UIScreenDefinition definition,
                    out UIScreenOperationResult failure))
            {
                return failure;
            }

            List<UIScreenEntry> history =
                GetOrCreateHistory(
                    definition.NavigationScopeId);

            UIScreenEntry current =
                history.Count == 0
                    ? null
                    : history[history.Count - 1];

            if (current != null &&
                string.Equals(
                    current.ScreenId,
                    definition.ScreenId,
                    StringComparison.Ordinal))
            {
                return new UIScreenOperationResult(
                    UIScreenOperationStatus.NoChange,
                    request.Kind,
                    definition.ScreenId,
                    definition.NavigationScopeId,
                    request.Sequence,
                    "Requested Screen is already current.");
            }

            if (ContainsScreen(
                    history,
                    definition.ScreenId))
            {
                return new UIScreenOperationResult(
                    UIScreenOperationStatus.Invalid,
                    request.Kind,
                    definition.ScreenId,
                    definition.NavigationScopeId,
                    request.Sequence,
                    "Push cannot duplicate an existing Screen entry in the same scope history.");
            }

            if (!TryPrepareEntry(
                    definition,
                    request,
                    out UIScreenEntry target,
                    out failure))
            {
                return failure;
            }

            if (current != null)
            {
                current.IsActive = false;
                current.IsSuspended = true;
            }

            target.IsActive = true;
            target.IsSuspended = false;

            history.Add(
                target);

            if (current != null)
            {
                suspendEntry?.Invoke(
                    current);
            }

            activateEntry?.Invoke(
                target);

            return UIScreenOperationResult.Success(
                request,
                target.ScreenId,
                target.NavigationScopeId,
                "Screen Push completed.");
        }

        private UIScreenOperationResult Replace(
            UIScreenOperationRequest request)
        {
            if (!TryResolveDefinition(
                    request.ScreenId,
                    request,
                    out UIScreenDefinition definition,
                    out UIScreenOperationResult failure))
            {
                return failure;
            }

            List<UIScreenEntry> history =
                GetOrCreateHistory(
                    definition.NavigationScopeId);

            UIScreenEntry current =
                history.Count == 0
                    ? null
                    : history[history.Count - 1];

            if (current != null &&
                string.Equals(
                    current.ScreenId,
                    definition.ScreenId,
                    StringComparison.Ordinal))
            {
                return new UIScreenOperationResult(
                    UIScreenOperationStatus.NoChange,
                    request.Kind,
                    definition.ScreenId,
                    definition.NavigationScopeId,
                    request.Sequence,
                    "Requested Screen is already current.");
            }

            if (ContainsScreen(
                    history,
                    definition.ScreenId))
            {
                return new UIScreenOperationResult(
                    UIScreenOperationStatus.Invalid,
                    request.Kind,
                    definition.ScreenId,
                    definition.NavigationScopeId,
                    request.Sequence,
                    "Replace cannot duplicate an existing Screen entry in the same scope history.");
            }

            if (!TryPrepareEntry(
                    definition,
                    request,
                    out UIScreenEntry target,
                    out failure))
            {
                return failure;
            }

            if (current != null)
            {
                history.RemoveAt(
                    history.Count - 1);
            }

            target.IsActive = true;
            target.IsSuspended = false;
            history.Add(target);

            if (current != null)
            {
                ReleaseEntry(
                    current,
                    hideExternalOrScene: true);
            }

            activateEntry?.Invoke(
                target);

            return UIScreenOperationResult.Success(
                request,
                target.ScreenId,
                target.NavigationScopeId,
                "Screen Replace completed.");
        }

        private UIScreenOperationResult Reset(
            UIScreenOperationRequest request)
        {
            if (!TryResolveDefinition(
                    request.ScreenId,
                    request,
                    out UIScreenDefinition definition,
                    out UIScreenOperationResult failure))
            {
                return failure;
            }

            List<UIScreenEntry> history =
                GetOrCreateHistory(
                    definition.NavigationScopeId);

            if (history.Count == 1 &&
                string.Equals(
                    history[0].ScreenId,
                    definition.ScreenId,
                    StringComparison.Ordinal))
            {
                return new UIScreenOperationResult(
                    UIScreenOperationStatus.NoChange,
                    request.Kind,
                    definition.ScreenId,
                    definition.NavigationScopeId,
                    request.Sequence,
                    "Scope is already reset to the requested root Screen.");
            }

            if (!TryPrepareEntry(
                    definition,
                    request,
                    out UIScreenEntry target,
                    out failure))
            {
                return failure;
            }

            List<UIScreenEntry> previous =
                new List<UIScreenEntry>(
                    history);

            history.Clear();
            target.IsActive = true;
            target.IsSuspended = false;
            history.Add(target);

            for (int index = previous.Count - 1;
                 index >= 0;
                 index--)
            {
                ReleaseEntry(
                    previous[index],
                    hideExternalOrScene: true);
            }

            activateEntry?.Invoke(
                target);

            return UIScreenOperationResult.Success(
                request,
                target.ScreenId,
                target.NavigationScopeId,
                "Screen scope Reset completed.");
        }

        private UIScreenOperationResult Back(
            UIScreenOperationRequest request)
        {
            string scopeId =
                Normalize(request.ScopeId);

            if (string.IsNullOrWhiteSpace(scopeId))
            {
                return new UIScreenOperationResult(
                    UIScreenOperationStatus.Invalid,
                    request.Kind,
                    scopeId: scopeId,
                    sequence: request.Sequence,
                    message: "Back requires a navigation scope ID.");
            }

            if (!historyByScope.TryGetValue(
                    scopeId,
                    out List<UIScreenEntry> history) ||
                history.Count <= 1)
            {
                return new UIScreenOperationResult(
                    UIScreenOperationStatus.NoChange,
                    request.Kind,
                    scopeId: scopeId,
                    sequence: request.Sequence,
                    message: "No previous Screen is available in this scope.");
            }

            int previousIndex =
                FindPreviousValidIndex(
                    history);

            if (previousIndex < 0)
            {
                PruneInvalidBelowCurrent(
                    history);

                return new UIScreenOperationResult(
                    UIScreenOperationStatus.NoChange,
                    request.Kind,
                    scopeId: scopeId,
                    sequence: request.Sequence,
                    message: "No valid previous Screen remains in this scope.");
            }

            UIScreenEntry current =
                history[history.Count - 1];

            List<UIScreenEntry> removed =
                new List<UIScreenEntry>();

            for (int index = history.Count - 1;
                 index > previousIndex;
                 index--)
            {
                removed.Add(
                    history[index]);

                history.RemoveAt(index);
            }

            UIScreenEntry previous =
                history[history.Count - 1];

            previous.IsActive = true;
            previous.IsSuspended = false;

            for (int index = 0;
                 index < removed.Count;
                 index++)
            {
                ReleaseEntry(
                    removed[index],
                    hideExternalOrScene: true);
            }

            resumeEntry?.Invoke(
                previous);

            return UIScreenOperationResult.Success(
                request,
                previous.ScreenId,
                scopeId,
                "Back restored the previous valid Screen.");
        }

        private UIScreenOperationResult Close(
            UIScreenOperationRequest request)
        {
            UIScreenEntry current;
            string scopeId =
                Normalize(request.ScopeId);

            if (!string.IsNullOrWhiteSpace(scopeId))
            {
                current =
                    GetCurrentEntry(
                        scopeId);
            }
            else
            {
                if (!TryResolveDefinition(
                        request.ScreenId,
                        request,
                        out UIScreenDefinition definition,
                        out UIScreenOperationResult failure))
                {
                    return failure;
                }

                scopeId =
                    definition.NavigationScopeId;

                current =
                    GetCurrentEntry(
                        scopeId);
            }

            if (current == null)
            {
                return new UIScreenOperationResult(
                    UIScreenOperationStatus.NoChange,
                    request.Kind,
                    request.ScreenId,
                    scopeId,
                    request.Sequence,
                    "No current Screen exists in the requested scope.");
            }

            if (!current.Definition.AllowClose)
            {
                return new UIScreenOperationResult(
                    UIScreenOperationStatus.Rejected,
                    request.Kind,
                    current.ScreenId,
                    scopeId,
                    request.Sequence,
                    "Current Screen definition does not allow explicit Close.");
            }

            if (!string.IsNullOrWhiteSpace(
                    request.ScreenId) &&
                !string.Equals(
                    current.ScreenId,
                    request.ScreenId,
                    StringComparison.Ordinal))
            {
                return new UIScreenOperationResult(
                    UIScreenOperationStatus.Invalid,
                    request.Kind,
                    request.ScreenId,
                    scopeId,
                    request.Sequence,
                    "Close may only target the current Screen in this checkpoint.");
            }

            List<UIScreenEntry> history =
                historyByScope[scopeId];

            history.RemoveAt(
                history.Count - 1);

            ReleaseEntry(
                current,
                hideExternalOrScene: true);

            int previousIndex =
                FindCurrentValidIndex(
                    history);

            if (previousIndex >= 0)
            {
                while (history.Count - 1 >
                       previousIndex)
                {
                    UIScreenEntry invalid =
                        history[history.Count - 1];

                    history.RemoveAt(
                        history.Count - 1);

                    ReleaseEntry(
                        invalid,
                        hideExternalOrScene: false);
                }

                UIScreenEntry previous =
                    history[history.Count - 1];

                previous.IsActive = true;
                previous.IsSuspended = false;

                resumeEntry?.Invoke(
                    previous);
            }
            else
            {
                history.Clear();
            }

            return UIScreenOperationResult.Success(
                request,
                current.ScreenId,
                scopeId,
                "Current Screen closed.");
        }

        private bool TryResolveDefinition(
            string screenId,
            UIScreenOperationRequest request,
            out UIScreenDefinition definition,
            out UIScreenOperationResult failure)
        {
            definition = null;
            failure = default;

            string normalized =
                Normalize(screenId);

            if (string.IsNullOrWhiteSpace(normalized) ||
                !definitions.TryGetValue(
                    normalized,
                    out definition))
            {
                failure =
                    new UIScreenOperationResult(
                        UIScreenOperationStatus.Invalid,
                        request.Kind,
                        normalized,
                        request.ScopeId,
                        request.Sequence,
                        "No Screen definition matches the requested stable ID.");

                return false;
            }

            if (!layerRegistry.TryGetHost(
                    definition.TargetLayerId.Value,
                    out _))
            {
                failure =
                    new UIScreenOperationResult(
                        UIScreenOperationStatus.Invalid,
                        request.Kind,
                        definition.ScreenId,
                        definition.NavigationScopeId,
                        request.Sequence,
                        "Screen target layer is no longer available.");

                return false;
            }

            return true;
        }

        private bool TryPrepareEntry(
            UIScreenDefinition definition,
            UIScreenOperationRequest request,
            out UIScreenEntry entry,
            out UIScreenOperationResult failure)
        {
            entry = null;
            failure = default;

            if (!layerRegistry.TryGetHost(
                    definition.TargetLayerId.Value,
                    out UILayerHost layerHost) ||
                layerHost == null)
            {
                failure =
                    Failure(
                        request,
                        definition,
                        UIScreenOperationStatus.Invalid,
                        "Screen target layer is unavailable.");

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
                                definition.ScreenId);
                    break;

                case UIScreenOwnershipMode.RootOwned:
                    if (!factory.TryCreate(
                            definition,
                            layerHost,
                            out view,
                            out string createError))
                    {
                        failure =
                            Failure(
                                request,
                                definition,
                                UIScreenOperationStatus.Failed,
                                string.IsNullOrWhiteSpace(
                                    createError)
                                    ? "RootOwned Screen factory failed."
                                    : createError);

                        return false;
                    }

                    ownsView = true;
                    break;

                default:
                    failure =
                        Failure(
                            request,
                            definition,
                            UIScreenOperationStatus.Invalid,
                            "Unsupported Screen ownership mode.");

                    return false;
            }

            string viewError =
                ValidateView(
                    definition,
                    view);

            if (!string.IsNullOrEmpty(
                    viewError))
            {
                if (ownsView)
                {
                    factory.Release(
                        view);
                }

                failure =
                    Failure(
                        request,
                        definition,
                        UIScreenOperationStatus.Invalid,
                        viewError);

                return false;
            }

            if (ownsView)
            {
                string registrationError =
                    registerRuntimeSurface == null
                        ? "No RootOwned runtime surface registrar is available."
                        : registerRuntimeSurface(
                            view);

                if (!string.IsNullOrWhiteSpace(
                        registrationError))
                {
                    factory.Release(
                        view);

                    failure =
                        Failure(
                            request,
                            definition,
                            UIScreenOperationStatus.Failed,
                            registrationError);

                    return false;
                }
            }

            entry =
                new UIScreenEntry(
                    definition,
                    view,
                    ownsView);

            return true;
        }

        private void ReleaseEntry(
            UIScreenEntry entry,
            bool hideExternalOrScene)
        {
            if (entry == null)
            {
                return;
            }

            entry.IsActive = false;
            entry.IsSuspended = false;

            if (entry.View != null &&
                hideExternalOrScene)
            {
                closeEntry?.Invoke(
                    entry);
            }

            if (!entry.LookingGlassOwnsView)
            {
                return;
            }

            UISurface owned =
                entry.View;

            if (owned != null)
            {
                unregisterRuntimeSurface?.Invoke(
                    owned);

                factory.Release(
                    owned);
            }

            entry.View = null;
        }

        private static string ValidateView(
            UIScreenDefinition definition,
            UISurface view)
        {
            if (view == null)
            {
                return
                    "Screen '" +
                    definition.ScreenId +
                    "' has no live view for ownership mode " +
                    definition.OwnershipMode +
                    ".";
            }

            if (view.Role != UISurfaceRole.Screen)
            {
                return
                    "Screen '" +
                    definition.ScreenId +
                    "' resolved a UISurface that is not a Screen.";
            }

            if (!string.Equals(
                    view.SurfaceId,
                    definition.ScreenId,
                    StringComparison.Ordinal))
            {
                return
                    "Screen definition ID '" +
                    definition.ScreenId +
                    "' does not match resolved UISurface ID '" +
                    view.SurfaceId +
                    "'.";
            }

            if (!string.Equals(
                    view.NavigationScopeId,
                    definition.NavigationScopeId,
                    StringComparison.Ordinal))
            {
                return
                    "Screen '" +
                    definition.ScreenId +
                    "' definition scope does not match its UISurface scope.";
            }

            return string.Empty;
        }

        private bool IsEntryValid(
            UIScreenEntry entry)
        {
            if (entry == null ||
                entry.View == null ||
                !definitions.TryGetValue(
                    entry.ScreenId,
                    out UIScreenDefinition definition))
            {
                return false;
            }

            return layerRegistry.TryGetHost(
                definition.TargetLayerId.Value,
                out _);
        }

        private int FindPreviousValidIndex(
            List<UIScreenEntry> history)
        {
            for (int index = history.Count - 2;
                 index >= 0;
                 index--)
            {
                if (IsEntryValid(
                        history[index]))
                {
                    return index;
                }
            }

            return -1;
        }

        private int FindCurrentValidIndex(
            List<UIScreenEntry> history)
        {
            for (int index = history.Count - 1;
                 index >= 0;
                 index--)
            {
                if (IsEntryValid(
                        history[index]))
                {
                    return index;
                }
            }

            return -1;
        }

        private void PruneInvalidBelowCurrent(
            List<UIScreenEntry> history)
        {
            for (int index = history.Count - 2;
                 index >= 0;
                 index--)
            {
                if (IsEntryValid(
                        history[index]))
                {
                    continue;
                }

                UIScreenEntry invalid =
                    history[index];

                history.RemoveAt(index);

                ReleaseEntry(
                    invalid,
                    hideExternalOrScene: false);
            }
        }

        private static bool ContainsScreen(
            List<UIScreenEntry> history,
            string screenId)
        {
            for (int index = 0;
                 index < history.Count;
                 index++)
            {
                if (string.Equals(
                        history[index].ScreenId,
                        screenId,
                        StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private List<UIScreenEntry> GetOrCreateHistory(
            string scopeId)
        {
            if (!historyByScope.TryGetValue(
                    scopeId,
                    out List<UIScreenEntry> history))
            {
                history =
                    new List<UIScreenEntry>();

                historyByScope.Add(
                    scopeId,
                    history);
            }

            return history;
        }

        private static UIScreenOperationResult Failure(
            UIScreenOperationRequest request,
            UIScreenDefinition definition,
            UIScreenOperationStatus status,
            string message) =>
            new UIScreenOperationResult(
                status,
                request.Kind,
                definition.ScreenId,
                definition.NavigationScopeId,
                request.Sequence,
                message);

        private static string Normalize(
            string value) =>
            value == null
                ? string.Empty
                : value.Trim();
    }
}
