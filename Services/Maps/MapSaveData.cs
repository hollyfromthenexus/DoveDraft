using Godot;

namespace DoveDraft;

public partial class MapSaveData : BaseSaveData
{
    [Export]
    public string CurrentMapPath { get; set; }
}
