using Godot;

public partial class NpcInteractable : Interactable
{
    public override bool IsInteractable => Services.Dialog.CoolingDown == false;

    //
    //  Exports
    //

    [Export]
    public NPC ParentNpc { get; set; }

    //
    //  Interactable Methods
    //

    protected override void UseStart()
    {
        ParentNpc.RequestStartTalking();
    }
}
