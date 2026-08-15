namespace EchoDevGames.EchoUI
{
    /// <summary>
    /// Explicit project-authored EventSystem coordination policy.
    /// </summary>
    public enum UIEventSystemCoordinationMode
    {
        AdoptAssigned = 0,
        AdoptExisting = 1,
        CreateIfMissing = 2,
        RequireExternal = 3
    }
}
