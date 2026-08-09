
using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Participant capability required by detached capture serialization.
    ///
    /// DetachedStateType is runtime-only authority supplied by trusted live
    /// code. It is never persisted as a CLR/assembly-qualified type name.
    /// </summary>
    public interface ISaveTypedParticipant :
        ISaveParticipant
    {
        Type DetachedStateType { get; }
    }
}
