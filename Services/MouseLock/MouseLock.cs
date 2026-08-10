using Godot;

namespace DoveDraft;

public partial class MouseLock : Node
{
    //
    //  Public Data
    //

    public static MouseLock Instance { get; private set; }

    //
    //  Private Data
    //

    private bool IsMouseLocked
    {
        get => Input.MouseMode == Input.MouseModeEnum.Captured;
        set => Input.MouseMode = value ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
    }

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Process(double delta)
    {
        // Unlock the mouse when we're in a dialog sequence.
        if (Services.Dialog.IsInSequence && IsMouseLocked == true)
        {
            IsMouseLocked = false;
        }
    }

    public override void _Input(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventMouseButton clickEvent:
                if (
                    clickEvent.Pressed
                    && clickEvent.ButtonIndex == MouseButton.Left
                    && Services.Dialog.IsInSequence == false
                )
                {
                    IsMouseLocked = true;
                }
                break;

            case InputEventKey keyEvent:
                if (keyEvent.Pressed && keyEvent.Keycode == Key.Escape)
                {
                    IsMouseLocked = false;
                }
                break;
        }
    }
}
