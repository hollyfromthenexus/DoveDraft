using Godot;
using Godot.Collections;

[GlobalClass]
public partial class SaveFileData : BaseSaveData
{
    [Export]
    public Array<SaveFileMappingData> AllData { get; set; }
}
