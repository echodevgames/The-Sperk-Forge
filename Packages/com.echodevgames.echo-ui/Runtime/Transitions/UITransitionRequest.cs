using System;

namespace EchoDevGames.EchoUI
{
    public sealed class UITransitionRequest
    {
        private readonly Func<bool> currentGenerationProbe;

        internal UITransitionRequest(
            UITransitionOperationId operationId,
            long generation,
            UISurface surface,
            UITransitionDirection direction,
            UITransitionResolvedPolicy policy,
            Func<bool> currentGenerationProbe)
        {
            OperationId = operationId;
            Generation = generation;
            Surface = surface;
            Direction = direction;
            Policy = policy;
            this.currentGenerationProbe = currentGenerationProbe;
        }

        public UITransitionOperationId OperationId { get; }
        public long Generation { get; }
        public UISurface Surface { get; }
        public string SurfaceId => Surface == null ? string.Empty : Surface.SurfaceId;
        public UITransitionDirection Direction { get; }
        public UITransitionResolvedPolicy Policy { get; }
        public bool IsCurrent => currentGenerationProbe == null || currentGenerationProbe();
    }
}
