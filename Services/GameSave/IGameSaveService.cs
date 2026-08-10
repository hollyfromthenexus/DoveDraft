public interface IGameSaveService : IService
{
    public void SaveState();

    public void LoadState();
}
