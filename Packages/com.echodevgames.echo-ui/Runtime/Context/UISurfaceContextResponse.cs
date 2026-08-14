using System;
using UnityEngine;

namespace EchoDevGames.EchoUI
{
    public enum UISurfaceVisibilityIntent
    {
        NoChange = 0,
        Visible = 1,
        Hidden = 2
    }

    public enum UISurfaceInteractionIntent
    {
        NoChange = 0,
        Interactable = 1,
        NonInteractable = 2
    }

    public enum UISurfaceSelectionIntent
    {
        NoChange = 0,
        ClearSelection = 1,
        SelectDefault = 2
    }

    /// <summary>
    /// Per-dimension response supplied by one context rule or effective resolver result.
    /// NoChange is a first-class value and must never be interpreted as false.
    /// </summary>
    [Serializable]
    public struct UISurfaceContextResponse
    {
        [SerializeField]
        private UISurfaceVisibilityIntent visibility;

        [SerializeField]
        private UISurfaceInteractionIntent interaction;

        [SerializeField]
        private UISurfaceSelectionIntent selection;

        public UISurfaceContextResponse(
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

        public bool HasAnyIntent =>
            visibility != UISurfaceVisibilityIntent.NoChange ||
            interaction != UISurfaceInteractionIntent.NoChange ||
            selection != UISurfaceSelectionIntent.NoChange;

        public static UISurfaceContextResponse NoChange =>
            new UISurfaceContextResponse(
                UISurfaceVisibilityIntent.NoChange,
                UISurfaceInteractionIntent.NoChange,
                UISurfaceSelectionIntent.NoChange);
    }
}
