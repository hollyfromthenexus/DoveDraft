using Godot;

namespace DoveDraft;

public partial class HUDService : CanvasLayer, IHUDService
{
    //
    //  Exports
    //

    [Export]
    public Crosshair Crosshair { get; set; }

    //
    //  Godot Methods
    //

    public override void _EnterTree()
    {
        Services.Register<IHUDService>(this);
    }

    public override void _ExitTree()
    {
        Services.Unregister<IHUDService>();
    }
}

public partial class Services
{
    public static IHUDService HUD => Get<IHUDService>();
}
