using Godot;

namespace DoveDraft;

public interface INPC
{
    public void RequestStartTalking();
}

[GlobalClass]
public partial class NPC : Node3D, INPC
{
    //
    //  Exports
    //

    [Export]
    public string DialogStartNode { get; set; }

    //
    //  INPC Methods
    //

    public void RequestStartTalking()
    {
        Services.Dialog.StartDialog(DialogStartNode);
    }
}
