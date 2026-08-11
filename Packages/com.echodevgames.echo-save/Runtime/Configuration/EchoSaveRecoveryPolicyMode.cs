namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Project-owned recovery behavior that is truthful in the current runtime.
    ///
    /// M5-02 intentionally exposes only ManualOnly. Automatic verified fallback
    /// remains separately gated rather than becoming a decorative setting.
    /// </summary>
    public enum EchoSaveRecoveryPolicyMode
    {
        ManualOnly = 0
    }
}
