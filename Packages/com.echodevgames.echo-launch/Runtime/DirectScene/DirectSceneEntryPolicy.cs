//----- DirectSceneEntryPolicy.cs START -----

namespace EchoDevGames.EchoLaunch
{
    /// <summary>
    /// Project-authored policy controlling whether a missing First Light
    /// authority may be created for direct-scene development entry.
    ///
    /// No value permits creation in a non-development release player.
    /// </summary>
    public enum DirectSceneEntryPolicy
    {
        EditorOnly = 0,
        EditorAndDevelopmentBuilds = 1,
        BootRequired = 2
    }
}

//----- DirectSceneEntryPolicy.cs END -----
