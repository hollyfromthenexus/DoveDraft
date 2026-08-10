using Godot;

[GlobalClass]
public partial class VirtualCamera3D : Node3D
{
    //
    //  Signals
    //

    [Signal]
    public delegate void PriorityChangedEventHandler();

    //
    // Exports
    //

    [Export]
    public int Priority
    {
        get => internalPriority;
        set
        {
            internalPriority = value;
            EmitSignalPriorityChanged();
        }
    }
    private int internalPriority = 10;

    [ExportGroup("Transition In")]
    [Export]
    public int TransitionInPriority { get; set; } = 0;

    [Export]
    public long TransitionInDurationMs { get; set; } = 200;

    [Export(PropertyHint.ExpEasing)]
    public float TransitionInCurve { get; set; } = 0.2f;

    [ExportGroup("Transition Out")]
    [Export]
    public int TransitionOutPriority { get; set; } = 0;

    [Export]
    public long TransitionOutDurationMs { get; set; } = 200;

    [Export(PropertyHint.ExpEasing)]
    public float TransitionOutCurve { get; set; } = 0.2f;

    //
    //  Godot Methods
    //

    public override void _EnterTree()
    {
        Services.Camera.Register(this);
    }

    public override void _ExitTree()
    {
        Services.Camera.Unregister(this);
    }
}
