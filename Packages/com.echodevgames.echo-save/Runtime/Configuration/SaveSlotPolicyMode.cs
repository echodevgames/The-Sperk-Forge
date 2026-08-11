namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Project-authored Chronicle slot-capacity policy modes.
    /// </summary>
    public enum SaveSlotPolicyMode
    {
        SingleSlot = 0,
        FixedMultiSlot = 1,
        ConfigurableMultiSlot = 2,
        BoundedProfiles = 3
    }
}
