using System;

namespace EchoDevGames.EchoUI
{
    public sealed class UIHudWidgetHandle : IDisposable
    {
        private UIHudRegionService service;

        internal UIHudWidgetHandle(
            UIHudRegionService service,
            UIHudRegionId regionId,
            UIHudWidgetId widgetId,
            long generation,
            bool accepted,
            UIHudOperationResult initialResult)
        {
            this.service = service;
            RegionId = regionId;
            WidgetId = widgetId;
            Generation = generation;
            Accepted = accepted;
            LastResult = initialResult;
        }

        public UIHudRegionId RegionId { get; }
        public UIHudWidgetId WidgetId { get; }
        public long Generation { get; }
        public bool Accepted { get; }
        public bool IsReleased { get; private set; }
        public UIHudOperationResult LastResult { get; private set; }

        public UIHudOperationResult Release()
        {
            if (IsReleased)
            {
                LastResult = new UIHudOperationResult(
                    UIHudOperationStatus.AlreadyReleased,
                    RegionId,
                    WidgetId,
                    Generation,
                    "HUD widget lease was already released.");
                return LastResult;
            }

            LastResult =
                service == null
                    ? new UIHudOperationResult(
                        UIHudOperationStatus.Unavailable,
                        RegionId,
                        WidgetId,
                        Generation,
                        "HUD widget lease service is unavailable.")
                    : service.ReleaseWidget(
                        RegionId,
                        WidgetId,
                        Generation);

            IsReleased = true;
            service = null;
            return LastResult;
        }

        public void Dispose() => Release();

        internal static UIHudWidgetHandle Rejected(
            UIHudRegionId regionId,
            UIHudWidgetId widgetId,
            UIHudOperationStatus status,
            string message)
        {
            UIHudOperationResult result =
                new UIHudOperationResult(
                    status,
                    regionId,
                    widgetId,
                    0,
                    message);

            return new UIHudWidgetHandle(
                null,
                regionId,
                widgetId,
                0,
                false,
                result);
        }
    }

    public sealed class UIHudVisibilityLease : IDisposable
    {
        private UIHudRegionService service;

        internal UIHudVisibilityLease(
            UIHudRegionService service,
            UIHudRegionId regionId,
            string reasonId,
            long generation,
            bool accepted,
            UIHudOperationResult initialResult)
        {
            this.service = service;
            RegionId = regionId;
            ReasonId = reasonId ?? string.Empty;
            Generation = generation;
            Accepted = accepted;
            LastResult = initialResult;
        }

        public UIHudRegionId RegionId { get; }
        public string ReasonId { get; }
        public long Generation { get; }
        public bool Accepted { get; }
        public bool IsReleased { get; private set; }
        public UIHudOperationResult LastResult { get; private set; }

        public UIHudOperationResult Release()
        {
            if (IsReleased)
            {
                LastResult = new UIHudOperationResult(
                    UIHudOperationStatus.AlreadyReleased,
                    RegionId,
                    generation: Generation,
                    message: "HUD visibility lease was already released.");
                return LastResult;
            }

            LastResult =
                service == null
                    ? new UIHudOperationResult(
                        UIHudOperationStatus.Unavailable,
                        RegionId,
                        generation: Generation,
                        message: "HUD visibility lease service is unavailable.")
                    : service.ReleaseVisibility(
                        RegionId,
                        Generation);

            IsReleased = true;
            service = null;
            return LastResult;
        }

        public void Dispose() => Release();

        internal static UIHudVisibilityLease Rejected(
            UIHudRegionId regionId,
            string reasonId,
            UIHudOperationStatus status,
            string message)
        {
            UIHudOperationResult result =
                new UIHudOperationResult(
                    status,
                    regionId,
                    message: message);

            return new UIHudVisibilityLease(
                null,
                regionId,
                reasonId,
                0,
                false,
                result);
        }
    }
}
