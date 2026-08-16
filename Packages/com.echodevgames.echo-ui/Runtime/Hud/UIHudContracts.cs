using System;

namespace EchoDevGames.EchoUI
{
    [Serializable]
    public readonly struct UIHudRegionId : IEquatable<UIHudRegionId>
    {
        public UIHudRegionId(string value)
        {
            Value = value == null ? string.Empty : value.Trim();
        }

        public string Value { get; }
        public bool IsValid => !string.IsNullOrWhiteSpace(Value);
        public bool Equals(UIHudRegionId other) =>
            string.Equals(Value, other.Value, StringComparison.Ordinal);
        public override bool Equals(object obj) =>
            obj is UIHudRegionId other && Equals(other);
        public override int GetHashCode() =>
            StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        public override string ToString() => Value ?? string.Empty;
    }

    [Serializable]
    public readonly struct UIHudWidgetId : IEquatable<UIHudWidgetId>
    {
        public UIHudWidgetId(string value)
        {
            Value = value == null ? string.Empty : value.Trim();
        }

        public string Value { get; }
        public bool IsValid => !string.IsNullOrWhiteSpace(Value);
        public bool Equals(UIHudWidgetId other) =>
            string.Equals(Value, other.Value, StringComparison.Ordinal);
        public override bool Equals(object obj) =>
            obj is UIHudWidgetId other && Equals(other);
        public override int GetHashCode() =>
            StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        public override string ToString() => Value ?? string.Empty;
    }

    public enum UIHudOwnershipMode
    {
        RootOwned = 0,
        SceneOwned = 1,
        ExternalOwned = 2
    }

    public enum UIHudOperationStatus
    {
        Completed = 0,
        Rejected = 1,
        Invalid = 2,
        Duplicate = 3,
        UnknownRegion = 4,
        CapacityExceeded = 5,
        Stale = 6,
        AlreadyReleased = 7,
        Unavailable = 8,
        Shutdown = 9
    }

    public readonly struct UIHudOperationResult
    {
        public UIHudOperationResult(
            UIHudOperationStatus status,
            UIHudRegionId regionId = default,
            UIHudWidgetId widgetId = default,
            long generation = 0,
            string message = "")
        {
            Status = status;
            RegionId = regionId;
            WidgetId = widgetId;
            Generation = generation;
            Message = message ?? string.Empty;
        }

        public UIHudOperationStatus Status { get; }
        public UIHudRegionId RegionId { get; }
        public UIHudWidgetId WidgetId { get; }
        public long Generation { get; }
        public string Message { get; }
        public bool Succeeded => Status == UIHudOperationStatus.Completed;

        public static UIHudOperationResult Success(
            UIHudRegionId regionId,
            UIHudWidgetId widgetId = default,
            long generation = 0,
            string message = "") =>
            new UIHudOperationResult(
                UIHudOperationStatus.Completed,
                regionId,
                widgetId,
                generation,
                message);
    }

    public readonly struct UIHudRegionSnapshot
    {
        public UIHudRegionSnapshot(
            UIHudRegionId regionId,
            long generation,
            bool effectiveVisibility,
            int widgetCount,
            int visibilityLeaseCount,
            UIHudOwnershipMode ownershipMode)
        {
            RegionId = regionId;
            Generation = generation;
            EffectiveVisibility = effectiveVisibility;
            WidgetCount = widgetCount;
            VisibilityLeaseCount = visibilityLeaseCount;
            OwnershipMode = ownershipMode;
        }

        public UIHudRegionId RegionId { get; }
        public long Generation { get; }
        public bool EffectiveVisibility { get; }
        public int WidgetCount { get; }
        public int VisibilityLeaseCount { get; }
        public UIHudOwnershipMode OwnershipMode { get; }
    }
}
