using Godot;

public partial class MapSaveData : BaseSaveData
{
    [Export]
    public string CurrentMapPath { get; set; }
}
