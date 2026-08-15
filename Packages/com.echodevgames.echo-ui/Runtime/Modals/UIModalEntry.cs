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
        }

        public UIModalDefinition Definition { get; }

        public UIModalId ModalId =>
            Definition.ModalId;

        public UISurface View { get; internal set; }

        public bool LookingGlassOwnsView { get; }

        public UIModalHandle Handle { get; }

        public long Generation =>
            Handle.Generation;
    }
}
