using UnityEngine;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Optional package-neutral entry-local focus resolver.
    /// Implementations live with project/sample UI and are queried only on explicit lifecycle/revalidation work.
    /// </summary>
    public interface IUIFocusTargetResolver
    {
        GameObject ResolveFocusTarget(
            UISurface surface);
    }
}
