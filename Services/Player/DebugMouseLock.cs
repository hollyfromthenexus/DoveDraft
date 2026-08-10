using System;
using Godot;

namespace DoveDraft;

public partial class DebugMouseLock : Node
{
    private bool IsMouseLocked
    {
        get => Input.MouseMode == Input.MouseModeEnum.Captured;
        set => Input.MouseMode = value ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
    }

    public override void _Input(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventMouseButton clickEvent:
                if (clickEvent.Pressed && clickEvent.ButtonIndex == MouseButton.Left) IsMouseLocked = true;
                break;

            case InputEventKey keyEvent:
                if (keyEvent.Pressed && keyEvent.Keycode == Key.Escape) IsMouseLocked = false;
                break;
        }
    }
}
