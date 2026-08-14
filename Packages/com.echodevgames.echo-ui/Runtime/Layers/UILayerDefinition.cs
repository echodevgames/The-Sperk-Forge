using System;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Immutable-at-runtime authored description of one Looking Glass layer.
    /// </summary>
    [Serializable]
    public sealed class UILayerDefinition
    {
        [SerializeField]
        private string layerId = "layer";

        [SerializeField]
        private string displayLabel = string.Empty;

        [SerializeField]
        private int order;

        public UILayerDefinition()
        {
        }

        public UILayerDefinition(
            string layerId,
            int order,
            string displayLabel = "")
        {
            this.layerId = layerId ?? string.Empty;
            this.order = order;
            this.displayLabel = displayLabel ?? string.Empty;
        }

        public UILayerId LayerId =>
            new UILayerId(layerId);

        public string DisplayLabel =>
            string.IsNullOrWhiteSpace(displayLabel)
                ? LayerId.Value
                : displayLabel.Trim();

        public int Order =>
            order;

        internal UILayerDefinition Snapshot() =>
            new UILayerDefinition(
                LayerId.Value,
                Order,
                DisplayLabel);
    }
}
