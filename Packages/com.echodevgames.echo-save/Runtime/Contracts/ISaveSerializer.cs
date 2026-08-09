
namespace EchoDevGames.EchoSave
{
    /// <summary>
    /// Replaceable serializer-provider contract for detached DTOs.
    ///
    /// Serializers operate in memory only. They do not own storage paths,
    /// generation publication, slots, participants, or gameplay state.
    /// </summary>
    public interface ISaveSerializer
    {
        SaveSerializerId Id { get; }

        SaveSerializerResult Serialize<T>(
            T value,
            out string serialized);

        SaveSerializerResult Deserialize<T>(
            string serialized,
            out T value);
    }
}
