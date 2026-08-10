using Godot;

public partial class PlayerSaveData : BaseSaveData
{
    [Export]
    public bool PlayerExists { get; set; }

    [Export]
    public PlayerPhysicsSaveData Physics { get; set; }
}
