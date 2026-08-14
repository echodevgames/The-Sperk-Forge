using System.Collections.Generic;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Mutable package-local view of externally supplied active/inactive UI contexts.
    /// Looking Glass stores the presentation truth but does not own why a context is active.
    /// </summary>
    public sealed class UIContextState
    {
        private readonly HashSet<UIContextId> activeContexts =
            new HashSet<UIContextId>();

        public int ActiveCount =>
            activeContexts.Count;

        public bool SetActive(
            string contextId,
            bool active) =>
            SetActive(
                new UIContextId(contextId),
                active);

        public bool SetActive(
            UIContextId contextId,
            bool active)
        {
            if (!contextId.IsValid)
            {
                return false;
            }

            return active
                ? activeContexts.Add(contextId)
                : activeContexts.Remove(contextId);
        }

        public bool IsActive(string contextId) =>
            IsActive(
                new UIContextId(contextId));

        public bool IsActive(UIContextId contextId) =>
            contextId.IsValid &&
            activeContexts.Contains(contextId);

        public void Clear()
        {
            activeContexts.Clear();
        }
    }
}
