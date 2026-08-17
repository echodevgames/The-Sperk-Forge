namespace EchoDevGames.EchoUI
{
    public enum UINotificationAdmissionStatus
    {
        Admitted = 0,
        Coalesced = 1,
        Invalid = 2,
        UnknownChannel = 3,
        CapacityExceeded = 4,
        InsufficientPriority = 5,
        Unavailable = 6,
        Shutdown = 7
    }

    public readonly struct UINotificationAdmissionResult
    {
        public UINotificationAdmissionResult(
            UINotificationAdmissionStatus status,
            UINotificationChannelId channelId = default,
            long generation = 0,
            UINotificationCoalescingKey coalescingKey = default,
            UINotificationCorrelationId correlationId = default,
            string message = "")
        {
            Status = status;
            ChannelId = channelId;
            Generation = generation;
            CoalescingKey = coalescingKey;
            CorrelationId = correlationId;
            Message = message ?? string.Empty;
        }

        public UINotificationAdmissionStatus Status { get; }

        public UINotificationChannelId ChannelId { get; }

        public long Generation { get; }

        public UINotificationCoalescingKey CoalescingKey { get; }

        public UINotificationCorrelationId CorrelationId { get; }

        public string Message { get; }

        public bool Succeeded =>
            Status == UINotificationAdmissionStatus.Admitted ||
            Status == UINotificationAdmissionStatus.Coalesced;
    }

    public enum UINotificationOutcome
    {
        Rejected = 0,
        Expired = 1,
        Dismissed = 2,
        Superseded = 3,
        OverflowEvicted = 4,
        OwnerLost = 5,
        Reset = 6,
        Shutdown = 7,
        PresentationLost = 8
    }

    /// <summary>
    /// Exact-once terminal result for one notification generation.
    /// It deliberately contains no presentation payload or visible text.
    /// </summary>
    public readonly struct UINotificationResult
    {
        public UINotificationResult(
            UINotificationOutcome outcome,
            UINotificationChannelId channelId,
            long generation,
            UINotificationCoalescingKey coalescingKey = default,
            UINotificationCorrelationId correlationId = default,
            string message = "")
        {
            Outcome = outcome;
            ChannelId = channelId;
            Generation = generation;
            CoalescingKey = coalescingKey;
            CorrelationId = correlationId;
            Message = message ?? string.Empty;
        }

        public UINotificationOutcome Outcome { get; }

        public UINotificationChannelId ChannelId { get; }

        public long Generation { get; }

        public UINotificationCoalescingKey CoalescingKey { get; }

        public UINotificationCorrelationId CorrelationId { get; }

        public string Message { get; }

        public bool WasRejected =>
            Outcome == UINotificationOutcome.Rejected;
    }

    public enum UINotificationOperationStatus
    {
        Completed = 0,
        Invalid = 1,
        UnknownChannel = 2,
        Stale = 3,
        AlreadySettled = 4,
        Unavailable = 5,
        Shutdown = 6
    }

    public readonly struct UINotificationOperationResult
    {
        public UINotificationOperationResult(
            UINotificationOperationStatus status,
            UINotificationChannelId channelId = default,
            long generation = 0,
            string message = "")
        {
            Status = status;
            ChannelId = channelId;
            Generation = generation;
            Message = message ?? string.Empty;
        }

        public UINotificationOperationStatus Status { get; }

        public UINotificationChannelId ChannelId { get; }

        public long Generation { get; }

        public string Message { get; }

        public bool Succeeded =>
            Status == UINotificationOperationStatus.Completed;
    }
}
