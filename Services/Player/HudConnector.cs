using Godot;

public partial class HudConnector : Node
{
    //
    //  Exports
    //

    [Export]
    public Player Player { get; set; }

    public override void _Ready()
    {
        Player.Interactor.IsHoveringChanged += OnIsHoveringChanged;
    }

    private void OnIsHoveringChanged(bool isHovering)
    {
        Services.HUD.Crosshair.IsHoveringInteractable = isHovering;
    }
}
