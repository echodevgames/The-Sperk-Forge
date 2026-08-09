
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Neutral package-document identity/version surface.
    ///
    /// Gameplay payload meaning remains outside Chronicle.
    /// </summary>
    public interface ISavePackageDocument
    {
        string DocumentKind { get; }

        int FormatMajor { get; }

        int FormatMinor { get; }

        int FormatRevision { get; }
    }
}
