
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Capability description for provider-neutral publication primitives.
    ///
    /// These flags describe which primitive the provider uses. They do not
    /// promise universal crash/power-loss atomicity.
    /// </summary>
    public readonly struct SaveStoragePublicationCapabilities
    {
        public SaveStoragePublicationCapabilities(
            bool supportsNewTreePublication,
            bool supportsCurrentObjectPublication,
            bool usesSameRootDirectoryMove,
            bool usesNativeReplaceForExistingCurrent,
            bool claimsPowerLossAtomicity)
        {
            SupportsNewTreePublication =
                supportsNewTreePublication;
            SupportsCurrentObjectPublication =
                supportsCurrentObjectPublication;
            UsesSameRootDirectoryMove =
                usesSameRootDirectoryMove;
            UsesNativeReplaceForExistingCurrent =
                usesNativeReplaceForExistingCurrent;
            ClaimsPowerLossAtomicity =
                claimsPowerLossAtomicity;
        }

        public bool SupportsNewTreePublication { get; }

        public bool SupportsCurrentObjectPublication { get; }

        public bool UsesSameRootDirectoryMove { get; }

        public bool UsesNativeReplaceForExistingCurrent { get; }

        public bool ClaimsPowerLossAtomicity { get; }
    }
}
