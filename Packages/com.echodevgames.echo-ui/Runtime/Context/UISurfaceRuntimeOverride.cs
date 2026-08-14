using System;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Transient project runtime override for effective surface response dimensions.
    /// This value is never persisted or written back into authored context rules.
    /// </summary>
    [Serializable]
    public struct UISurfaceRuntimeOverride
    {
        [SerializeField]
        private UISurfaceVisibilityIntent visibility;

        [SerializeField]
        private UISurfaceInteractionIntent interaction;

        [SerializeField]
        private UISurfaceSelectionIntent selection;

        public UISurfaceRuntimeOverride(
            UISurfaceVisibilityIntent visibility,
            UISurfaceInteractionIntent interaction,
            UISurfaceSelectionIntent selection)
        {
            this.visibility = visibility;
            this.interaction = interaction;
            this.selection = selection;
        }

        public UISurfaceVisibilityIntent Visibility =>
            visibility;

        public UISurfaceInteractionIntent Interaction =>
            interaction;

        public UISurfaceSelectionIntent Selection =>
            selection;

        public bool HasVisibilityOverride =>
            visibility != UISurfaceVisibilityIntent.NoChange;

        public bool HasInteractionOverride =>
            interaction != UISurfaceInteractionIntent.NoChange;

        public bool HasSelectionOverride =>
            selection != UISurfaceSelectionIntent.NoChange;

        public bool HasAnyOverride =>
            HasVisibilityOverride ||
            HasInteractionOverride ||
            HasSelectionOverride;

        public static UISurfaceRuntimeOverride None =>
            new UISurfaceRuntimeOverride(
                UISurfaceVisibilityIntent.NoChange,
                UISurfaceInteractionIntent.NoChange,
                UISurfaceSelectionIntent.NoChange);
    }
}
