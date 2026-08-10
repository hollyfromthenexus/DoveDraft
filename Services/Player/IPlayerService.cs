namespace DoveDraft;

public interface IPlayerService : IService
{
    public Player Current { get; }
}
