namespace DoveDraft;

public interface ISaveLoadable
{
    public BaseSaveData Save();

    public void Load(BaseSaveData saveData);
}
