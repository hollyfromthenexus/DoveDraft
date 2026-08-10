using Godot;
using Godot.Collections;

namespace DoveDraft;

[GlobalClass]
public partial class SaveFileData : BaseSaveData
{
    [Export]
    public Array<SaveFileMappingData> AllData { get; set; }
}
