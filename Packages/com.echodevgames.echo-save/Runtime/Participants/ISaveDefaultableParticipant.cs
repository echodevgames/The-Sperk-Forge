
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Optional additive capability for a participant that can explicitly
    /// initialize its own default gameplay state when no payload exists.
    ///
    /// Chronicle never encodes default initialization as Apply(null).
    /// </summary>
    public interface ISaveDefaultableParticipant
    {
        SaveParticipantApplyResult InitializeDefault();
    }
}
