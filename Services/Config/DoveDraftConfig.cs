using Godot;
using YarnSpinnerGodot;

namespace DoveDraft;

/// <summary>
/// Configuration data specific to DoveDraft. Provided via <see cref="IDoveDraftConfigService"/>.
/// </summary>
[GlobalClass]
public partial class DoveDraftConfig : Resource
{
    /// <summary>
    /// Gets or sets the <see cref="YarnProject"/> to use for this game.
    /// </summary>
    [Export]
    public YarnProject GameYarnProject { get; set; }
}
