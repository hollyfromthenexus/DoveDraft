using Godot;

namespace DoveDraft;

/// <inheritdoc/>
[GlobalClass]
public partial class DoveDraftConfigService : Node, IDoveDraftConfigService
{
    /// <inheritdoc/>
    [Export]
    public DoveDraftConfig Config { get; set; }

    //
    //  Godot Methods
    //

    /// <inheritdoc/>
    public override void _EnterTree()
    {
        Services.Register<IDoveDraftConfigService>(this);
    }

    /// <inheritdoc/>
    public override void _ExitTree()
    {
        Services.Unregister<IDoveDraftConfigService>();
    }
}
