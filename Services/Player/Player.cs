using Godot;

namespace DoveDraft;

public interface IPlayer
{
    public IPlayerInteractor Interactor { get; }

    public void Teleport(Vector3 pos, Vector3 rot, Vector3 walkVel, Vector3 airVel);

    public PlayerPhysicsSaveData Save();
    public void Load(PlayerPhysicsSaveData data);
}

public partial class Player : CharacterBody3D, IPlayer
{
    public const string GroupName = "player";

    //
    //  Exports
    //

    /// <summary>
    /// Gets or sets the head of this player.
    /// </summary>
    [Export]
    public Node3D HeadNode { get; set; }

    [Export]
    public VirtualCamera3D HeadCamera { get; set; }

    /// <summary>
    /// Gets or sets the raycast interactor for this player
    /// </summary>
    [Export]
    public PlayerInteractor Interactor { get; set; }
    IPlayerInteractor IPlayer.Interactor => Interactor;

    /// <summary>
    /// Gets or sets how fast the player can walk, while on the ground.
    /// </summary>
    [Export]
    public float GroundWalkSpeed { get; set; } = 8;

    /// <summary>
    /// Gets or sets how quickly the player accelerates towards their target walk direction, while on the ground.
    /// /// </summary>
    [Export]
    public float GroundWalkAccelerateSpeed { get; set; } = 100;

    /// <summary>
    /// Gets or sets how quickly the player decelerates after not walking, while on the ground.
    /// </summary>
    [Export]
    public float GroundWalkDecelerateSpeed { get; set; } = 50;

    /// <summary>
    /// Gets or sets how quickly the player accelerates towards their target walk direction, while in the air.
    /// </summary>
    [Export]
    public float AirWalkAccelerateSpeed { get; set; } = 20;

    /// <summary>
    /// Gets or sets how quickly the player decelerates after not walking, while in the air.
    /// </summary>
    [Export]
    public float AirWalkDecelerateSpeed { get; set; } = 30;

    /// <summary>
    /// Gets or sets how quickly the player accelerates towards the ground, when they are in the air.
    /// </summary>
    [Export]
    public float FallAcceleration { get; set; } = 15;

    /// <summary>
    /// Gets or sets the velocity impulse to apply when the player jumps.
    /// </summary>
    [Export]
    public float JumpImpulse { get; set; } = 6;

    //
    //  Private Data
    //

    private bool CanUseInput
    {
        get => Services.Dialog.IsInSequence == false;
    }

    /// <summary>
    /// The velocity contributed specifically by walking inputs.
    /// </summary>
    private Vector3 walkVelocity;

    /// <summary>
    /// The velocity contributed specifically by air. Typically this is gravity.
    /// </summary>
    private Vector3 airVelocity;

    //
    //  Godot Methods
    //

    public override void _EnterTree()
    {
        AddToGroup(GroupName);
    }

    public override void _ExitTree()
    {
        RemoveFromGroup(GroupName);
    }

    public override void _PhysicsProcess(double delta)
    {
        // Apply all velocities this frame...
        ProcessWalkVelocity(delta);
        ProcessAirVelocity(delta);
        ProcessUsing();

        // APPLY VELOCITY TO THE PLAYER.
        Velocity = walkVelocity + airVelocity;
        MoveAndSlide();
    }

    //
    //  IPlayer Methods
    //

    public void Teleport(Vector3 pos, Vector3 rot, Vector3 walkVel, Vector3 airVel)
    {
        GlobalPosition = pos;
        HeadNode.GlobalRotation = rot;
        walkVelocity = walkVel;
        airVelocity = airVel;

        ResetPhysicsInterpolation();
        Services.Camera.FlagCameraTeleported(HeadCamera);
    }

    public PlayerPhysicsSaveData Save()
    {
        return new()
        {
            GlobalPosition = GlobalPosition,
            GlobalRotation = HeadNode.GlobalRotation,
            WalkVelocity = walkVelocity,
            AirVelocity = airVelocity,
        };
    }

    public void Load(PlayerPhysicsSaveData data)
    {
        Teleport(data.GlobalPosition, data.GlobalRotation, data.WalkVelocity, data.AirVelocity);
    }

    //
    //  Private Methods
    //

    private void ProcessWalkVelocity(double delta)
    {
        Vector3 inputDirection = GetRotatedWalkDirection();
        float accelerateSpeed = IsOnFloor() ? GroundWalkAccelerateSpeed : AirWalkAccelerateSpeed;
        float decelerateSpeed = IsOnFloor() ? GroundWalkDecelerateSpeed : AirWalkDecelerateSpeed;

        // If the player is not trying to move, drift back to no movement and EXIT EARLY.
        if (inputDirection.IsZeroApprox())
        {
            walkVelocity = walkVelocity.MoveToward(Vector3.Zero, decelerateSpeed * (float)delta);
            return;
        }

        // OTHERWISE, move towards the given input direction
        walkVelocity = walkVelocity.MoveToward(
            inputDirection * GroundWalkSpeed,
            accelerateSpeed * (float)delta
        );
    }

    private void ProcessAirVelocity(double delta)
    {
        // Apply gravity if necessary.
        if (IsOnFloor() == false)
        {
            airVelocity.Y -= FallAcceleration * (float)delta;
        }
        else
        {
            airVelocity.Y = -1;
        }

        // Apply jump movement if needed.
        if (IsOnFloor() && GetInputPlayerJump())
        {
            airVelocity.Y += JumpImpulse;
        }
    }

    private void ProcessUsing()
    {
        // Special logic for when we're in a dialog sequence.
        if (Services.Dialog.IsInSequence == true)
        {
            if (Input.IsActionJustPressed("dialog_proceed"))
            {
                Services.Dialog.RequestProceed();
            }

            Interactor.IsUsing = false;
            return;
        }

        if (Input.IsActionJustPressed("player_use"))
        {
            Interactor.IsUsing = true;
        }

        if (Input.IsActionJustReleased("player_use"))
        {
            Interactor.IsUsing = false;
        }
    }

    /// <summary>
    /// Get the direction that the player wants to walk in, normalized.
    /// </summary>
    /// <returns>The direction to walk in relative to the forward look direction.</returns>
    private Vector3 GetInputWalkDirection()
    {
        var direction = Vector3.Zero;

        // If we can't use input, EXIT EARLY.
        if (CanUseInput == false)
        {
            return direction;
        }

        if (Input.IsActionPressed("player_move_forward"))
        {
            direction.Z -= 1;
        }
        if (Input.IsActionPressed("player_move_backward"))
        {
            direction.Z += 1;
        }
        if (Input.IsActionPressed("player_move_left"))
        {
            direction.X -= 1;
        }
        if (Input.IsActionPressed("player_move_right"))
        {
            direction.X += 1;
        }

        return direction.Normalized();
    }

    /// <summary>
    /// Get the direction that the player wants to walk in, globally.
    /// </summary>
    /// <returns>The direction to walk in relative to this object's origin.</returns>
    private Vector3 GetRotatedWalkDirection()
    {
        return GetInputWalkDirection().Rotated(Vector3.Up, HeadNode.Rotation.Y);
    }

    private bool GetInputPlayerJump()
    {
        // If we can't use input, EXIT EARLY.
        if (CanUseInput == false)
        {
            return false;
        }

        return Input.IsActionJustPressed("player_move_jump");
    }
}
