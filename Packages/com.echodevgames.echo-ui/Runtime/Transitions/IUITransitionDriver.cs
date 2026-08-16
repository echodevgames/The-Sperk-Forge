using System.Threading;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    public interface IUITransitionDriver
    {
        string DriverId { get; }
        bool SupportsCancellation { get; }
        Awaitable<UITransitionResult> ExecuteAsync(UITransitionRequest request, CancellationToken cancellationToken);
        void ForceFinalState(UITransitionRequest request);
    }
}
