using Godot;

namespace DoveDraft;

public partial class SaveFileMappingData : Resource
{
    [Export]
    public string TypeKey { get; set; }

    [Export]
    public BaseSaveData Data { get; set; }
}
