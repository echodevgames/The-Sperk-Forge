namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Mutable runtime state for one admitted modal generation.
    /// </summary>
    public sealed class UIModalEntry
    {
        internal UIModalEntry(
            UIModalDefinition definition,
            UISurface view,
            bool lookingGlassOwnsView,
            UIModalHandle handle)
        {
            Definition = definition;
            View = view;
            LookingGlassOwnsView = lookingGlassOwnsView;
            Handle = handle;
            IsEntering = false;
            IsInteractive = false;
            IsClosing = false;
            HasTerminalClaim = false;
            ClaimedResult = default;
        }

        public UIModalDefinition Definition { get; }

        public UIModalId ModalId =>
            Definition.ModalId;

        public UISurface View { get; internal set; }

        public bool LookingGlassOwnsView { get; }

        public UIModalHandle Handle { get; }

        public long Generation =>
            Handle.Generation;

        internal bool IsEntering { get; set; }

        internal bool IsInteractive { get; set; }

        internal bool IsClosing { get; set; }

        internal bool HasTerminalClaim { get; set; }

        internal UIModalResult ClaimedResult { get; set; }

        internal bool AcceptsInteraction =>
            IsInteractive &&
            !IsEntering &&
            !IsClosing &&
            !HasTerminalClaim;
    }
}
