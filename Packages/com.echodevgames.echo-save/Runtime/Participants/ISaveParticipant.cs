
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Public open-ended persistence participation contract.
    ///
    /// Chronicle contains no compile-time catalog of known participants.
    /// Future project systems or optional package bridges use this same
    /// contract without editing Chronicle core.
    ///
    /// M3-01 defines capture/apply-facing methods only. The registry never
    /// invokes them and performs no durable I/O.
    /// </summary>
    public interface ISaveParticipant
    {
        SaveParticipantDescriptor Descriptor { get; }

        SaveParticipantCaptureResult Capture();

        SaveParticipantApplyResult Apply(
            object detachedState);
    }
}
