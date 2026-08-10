using Godot;

public partial class DialogSaveData : BaseSaveData
{
    [Export]
    public DialogVariableSaveData Variables { get; set; }
}
