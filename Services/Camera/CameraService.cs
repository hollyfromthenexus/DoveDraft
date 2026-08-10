using System.Collections.Generic;
using System.Linq;
using Godot;

namespace DoveDraft;

public partial class CameraService : Node, ICameraService, ISaveLoadable
{
    //
    //  Exports
    //

    [Export]
    public MainCamera3D MainCamera { get; set; }

    //
    //  Public Data
    //

    public VirtualCamera3D CurrentTarget => MainCamera.Target;

    //
    //  Private Data
    //

    private HashSet<VirtualCamera3D> cameras = new();
    private bool flagRefreshNextFrame;

    //
    //  Godot Methods
    //

    public override void _EnterTree()
    {
        Services.Register<ICameraService>(this);
    }

    public override void _ExitTree()
    {
        Services.Unregister<ICameraService>();
    }

    public override void _Process(double _)
    {
        if (flagRefreshNextFrame)
        {
            Refresh();
            flagRefreshNextFrame = false;
        }
    }

    //
    //  ICameraService Methods
    //

    /// <inheritdoc/>
    public bool Register(VirtualCamera3D virtualCamera)
    {
        bool couldAdd = cameras.Add(virtualCamera);
        if (couldAdd)
        {
            virtualCamera.PriorityChanged += OnVirtualCameraPriorityChanged;
            flagRefreshNextFrame = true;
        }

        return couldAdd;
    }

    /// <inheritdoc/>
    public bool Unregister(VirtualCamera3D virtualCamera)
    {
        bool couldRemove = cameras.Remove(virtualCamera);
        if (couldRemove)
        {
            virtualCamera.PriorityChanged -= OnVirtualCameraPriorityChanged;
            flagRefreshNextFrame = true;
        }

        return couldRemove;
    }

    public void FlagCameraTeleported(VirtualCamera3D camera = null)
    {
        if (camera == null || camera == MainCamera.Target)
        {
            MainCamera.FlagTeleported();
        }
    }

    //
    //  ISaveLoadable Methods
    //

    public BaseSaveData Save()
    {
        return null;
    }

    public void Load(BaseSaveData data)
    {
        Refresh();
        MainCamera.SkipTransition();
    }

    //
    //  Private Methods
    //

    private void Refresh()
    {
        // Calculate what the target camera SHOULD be. If this is the same
        // as our current target... EXIT EARLY.
        VirtualCamera3D nextTarget = ChooseTargetCamera();
        if (nextTarget == MainCamera.Target) return;

        Log.For<CameraService>($"'{MainCamera.Target?.Name ?? "NULL"}' -> '{nextTarget?.Name ?? "NULL"}' .");
        MainCamera.Target = nextTarget;
    }

    private VirtualCamera3D ChooseTargetCamera()
    {
        VirtualCamera3D target = cameras.FirstOrDefault();

        foreach (VirtualCamera3D camera in cameras)
        {
            if (camera.Priority > target.Priority)
            {
                target = camera;
            }
        }

        return target;
    }

    private void OnVirtualCameraPriorityChanged() => flagRefreshNextFrame = true;
}

public partial class Services
{
    public static ICameraService Camera => Get<ICameraService>();
}
