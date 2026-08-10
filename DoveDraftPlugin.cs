#if TOOLS
using Godot;

namespace DoveDraft;

/// <summary>
/// The root plugin script for DoveDraft. See https://docs.godotengine.org/en/stable/tutorials/plugins/editor/making_plugins.html .
/// </summary>
[Tool]
public partial class DoveDraftPlugin : EditorPlugin
{
    /// <inheritdoc/>
    public override void _EnterTree()
    {
        // Initialization of the plugin goes here.
    }

    /// <inheritdoc/>
    public override void _ExitTree()
    {
        // Clean-up of the plugin goes here.
    }
}
#endif
