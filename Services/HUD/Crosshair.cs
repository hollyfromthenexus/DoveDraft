using Godot;

namespace DoveDraft;

public partial class Crosshair : Control
{
    //
    //  Exports
    //

    [Export]
    public Control Outline { get; set; }

    [Export]
    public Control Center { get; set; }

    //
    //  Public Data
    //

    public bool IsHoveringInteractable
    {
        get => Center.Visible;
        set => Center.Visible = value;
    }
}
