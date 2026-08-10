using Godot;

namespace DoveDraft;

public partial class DialogSaveData : BaseSaveData
{
    [Export]
    public DialogVariableSaveData Variables { get; set; }
}
