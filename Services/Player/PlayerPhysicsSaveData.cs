using Godot;

namespace DoveDraft;

public partial class PlayerPhysicsSaveData : BaseSaveData
{
    [Export]
    public Vector3 GlobalPosition { get; set; }

    [Export]
    public Vector3 GlobalRotation { get; set; }

    [Export]
    public Vector3 WalkVelocity { get; set; }

    [Export]
    public Vector3 AirVelocity { get; set; }
}
