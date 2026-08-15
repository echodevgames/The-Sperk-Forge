using UnityEngine.EventSystems;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Structured EventSystem coordination outcome.
    /// </summary>
    public readonly struct UIEventSystemCoordinationResult
    {
        public UIEventSystemCoordinationResult(
            UIEventSystemCoordinationStatus status,
            EventSystem eventSystem,
            bool createdByLookingGlass,
            int eligibleCount,
            string message)
        {
            Status = status;
            EventSystem = eventSystem;
            CreatedByLookingGlass = createdByLookingGlass;
            EligibleCount = eligibleCount;
            Message = message ?? string.Empty;
        }

        public UIEventSystemCoordinationStatus Status { get; }
        public EventSystem EventSystem { get; }
        public bool CreatedByLookingGlass { get; }
        public int EligibleCount { get; }
        public string Message { get; }

        public bool Succeeded =>
            Status == UIEventSystemCoordinationStatus.Ready &&
            EventSystem != null;
    }
}
