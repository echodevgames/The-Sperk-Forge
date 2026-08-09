
using System;

namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Optional serializer capability for a trusted runtime DTO Type.
    ///
    /// Type authority comes from currently running registration code. Chronicle
    /// never reads a CLR type name from save data and never uses save data to
    /// request arbitrary type activation.
    /// </summary>
    public interface IRuntimeTypeSaveSerializer :
        ISaveSerializer
    {
        SaveSerializerResult Serialize(
            object value,
            Type valueType,
            out string serialized);

        SaveSerializerResult Deserialize(
            string serialized,
            Type valueType,
            out object value);
    }
}
