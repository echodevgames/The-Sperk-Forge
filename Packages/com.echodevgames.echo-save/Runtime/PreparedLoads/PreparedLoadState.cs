
namespace EchoDevGames.EchoSave
{
    public enum PreparedLoadState
    {
        Live = 0,
        Disposed = 1,
        Expired = 2,
        OwnerInvalidated = 3,
        Consumed = 4
    }
}
