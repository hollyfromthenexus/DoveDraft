using System;
using System.Collections.Generic;
using Godot;

namespace DoveDraft;

public partial class MouseLook3D : Node3D
{
    //
    //  Exports
    //

    /// <summary>
    /// Gets or sets the horizontal mouse sensitivity.
    /// </summary>
    [Export]
    public float SensitivityX { get; set; } = 0.2f;

    /// <summary>
    /// Gets or sets the vertical mouse sensitivity.
    /// </summary>
    [Export]
    public float SensitivityY { get; set; } = 0.2f;

    //
    //  Private Data
    //

    private bool CanLook
    {
        get => Services.Dialog.IsInSequence == false;
    }

    private Queue<Vector2> lookEvents = new();

    //
    //  Godot Methods
    //

    public override void _Input(InputEvent @event)
    {
        // If we can't look, EXIT EARLY.
        if (CanLook == false)
        {
            return;
        }

        // If the mouse isn't locked, EXIT EARLY.
        if (Input.MouseMode != Input.MouseModeEnum.Captured)
        {
            return;
        }

        // If this event is a mouse motion event, add it to the queue to process later.
        switch (@event)
        {
            case InputEventMouseMotion mouseMotionEvent:
                lookEvents.Enqueue(
                    mouseMotionEvent.Relative * new Vector2(SensitivityX, SensitivityY)
                );
                break;
        }
    }

    public override void _Process(double delta)
    {
        // Apply all queue'd look events
        while (lookEvents.Count > 0)
        {
            Vector2 mouseMotion = lookEvents.Dequeue();

            Vector3 newRotation = RotationDegrees;
            newRotation.X = Mathf.Clamp(newRotation.X - mouseMotion.Y, -90, 90);
            newRotation.Y -= mouseMotion.X;
            RotationDegrees = newRotation;
        }
    }
}
