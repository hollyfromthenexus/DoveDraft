using Godot;

namespace DoveDraft;

[GlobalClass]
public partial class MainCamera3D : Camera3D
{
    private struct TransitionData
    {
        public bool IsActive { get; set; }

        public VirtualCamera3D OldTarget { get; set; }

        public Vector3 OldPosition { get; set; }

        public Basis OldBasis { get; set; }

        public ulong Duration { get; set; }

        public float EaseCurve { get; set; }

        public ulong StartTime { get; set; }

        public ulong TimeSinceStart => Time.GetTicksMsec() - StartTime;

        public bool IsComplete => TimeSinceStart >= Duration;

        public Vector3 StartPosition => OldTarget?.GlobalPosition ?? OldPosition;

        public Basis StartBasis => OldTarget?.GlobalBasis ?? OldBasis;
    }

    //
    //  Public Data
    //

    public VirtualCamera3D Target
    {
        get => targetInternal;
        set => StartTransition(targetInternal, value);
    }
    private VirtualCamera3D targetInternal;

    //
    //  Private Data
    //

    private TransitionData transition;
    private bool wasTeleported;

    //
    //  Godot Methods
    //

    public override void _Process(double _)
    {
        Vector3 targetPos = GlobalPosition;
        Basis targetBasis = GlobalBasis;

        if (Target != null)
        {
            ProcessFollowTarget(ref targetPos, ref targetBasis);
        }

        if (transition.IsActive)
        {
            ProcessTransition(ref targetPos, ref targetBasis);
        }

        // TODO - can use the above structure to implement camera offsets
        // or screenshake, because position/rotation are derived each frame.

        GlobalPosition = targetPos;
        GlobalBasis = targetBasis;

        // If something has teleported the camera since last tick, reset interp!
        if (wasTeleported)
        {
            ResetPhysicsInterpolation();
            wasTeleported = false;
        }
    }

    //
    //  Public Methods
    //

    public void FlagTeleported() => wasTeleported = true;

    public void SkipTransition()
    {
        GlobalPosition = Target.GlobalPosition;
        GlobalBasis = Target.GlobalBasis;
        EndTransition();
        FlagTeleported();
    }

    //
    //  Private Methods
    //

    private void ProcessTransition(ref Vector3 targetPos, ref Basis targetBasis)
    {
        // If the transition has completed since last frame, EXIT EARLY.
        if (transition.IsComplete)
        {
            EndTransition();
            return;
        }

        // Calculate lerp / ease based on transition config.
        float lerpProgress = (float)transition.TimeSinceStart / (float)transition.Duration;
        float easedLerp = Mathf.Ease(lerpProgress, transition.EaseCurve);

        // Apply actual lerp values!
        targetPos = transition.StartPosition.Lerp(targetPos, easedLerp);
        targetBasis = transition.StartBasis.Slerp(targetBasis, easedLerp);
    }

    private void ProcessFollowTarget(ref Vector3 targetPosition, ref Basis targetBasis)
    {
        targetPosition = Target.GlobalPosition;
        targetBasis = Target.GlobalBasis;
    }

    private void StartTransition(VirtualCamera3D oldTarget, VirtualCamera3D newTarget)
    {
        if (transition.IsActive)
        {
            Log.For<CameraService>("Interrupting existing transition!");
        }

        int outPriority = oldTarget?.TransitionOutPriority ?? int.MaxValue;
        int inPriority = newTarget?.TransitionInPriority ?? int.MinValue;

        VirtualCamera3D transitionSettingSource;
        float easeCurve;
        long duration;
        if (outPriority > inPriority)
        {
            easeCurve = oldTarget?.TransitionOutCurve ?? 0;
            duration = oldTarget?.TransitionOutDurationMs ?? 0;
            transitionSettingSource = oldTarget;
        }
        else
        {
            easeCurve = newTarget?.TransitionInCurve ?? 1;
            duration = newTarget?.TransitionInDurationMs ?? 200;
            transitionSettingSource = newTarget;
        }

        transition = new TransitionData()
        {
            IsActive = true,
            OldTarget = oldTarget,
            OldPosition = GlobalPosition,
            OldBasis = GlobalBasis,
            Duration = (ulong)duration,
            EaseCurve = easeCurve,
            StartTime = Time.GetTicksMsec(),
        };

        Log.For<CameraService>($"Using {transitionSettingSource?.Name ?? "NULL"}'s {transition.Duration}ms with curve {transition.EaseCurve}.");
        targetInternal = newTarget;
    }

    private void EndTransition()
    {
        // Fix a visual glitch if using a hard cut while physics
        // interpolation is enabled.
        if (transition.Duration == 0 && Target != null)
        {
            FlagTeleported();
        }

        transition = default;
    }
}
