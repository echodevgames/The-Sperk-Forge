using System;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    [Serializable]
    public sealed class UIHudRegionDefinition
    {
        [SerializeField]
        private string regionId = "hud.region";

        [SerializeField]
        private string displayLabel = string.Empty;

        [SerializeField]
        private bool startVisible = true;

        [SerializeField, Min(1)]
        private int widgetCapacity = 16;

        public UIHudRegionDefinition()
        {
        }

        public UIHudRegionDefinition(
            string regionId,
            bool startVisible = true,
            int widgetCapacity = 16,
            string displayLabel = "")
        {
            this.regionId = regionId ?? string.Empty;
            this.startVisible = startVisible;
            this.widgetCapacity = Mathf.Max(1, widgetCapacity);
            this.displayLabel = displayLabel ?? string.Empty;
        }

        public UIHudRegionId RegionId => new UIHudRegionId(regionId);
        public string DisplayLabel =>
            string.IsNullOrWhiteSpace(displayLabel)
                ? RegionId.Value
                : displayLabel.Trim();
        public bool StartVisible => startVisible;
        public int WidgetCapacity => Mathf.Max(1, widgetCapacity);

        internal UIHudRegionDefinition Snapshot() =>
            new UIHudRegionDefinition(
                RegionId.Value,
                StartVisible,
                WidgetCapacity,
                DisplayLabel);
    }
}
