namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Mutable session entry corresponding to one immutable Screen definition snapshot.
    /// </summary>
    public sealed class UIScreenEntry
    {
        internal UIScreenEntry(
            UIScreenDefinition definition,
            UISurface view,
            bool lookingGlassOwnsView)
        {
            Definition = definition;
            View = view;
            LookingGlassOwnsView = lookingGlassOwnsView;
        }

        public UIScreenDefinition Definition { get; }

        public UISurface View { get; internal set; }

        public bool LookingGlassOwnsView { get; }

        public bool IsActive { get; internal set; }

        public bool IsSuspended { get; internal set; }

        public string ScreenId =>
            Definition.ScreenId;

        public string NavigationScopeId =>
            Definition.NavigationScopeId;
    }
}
