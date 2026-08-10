#if TOOLS
using Godot;

namespace DoveDraft;

[Tool]
public partial class DoveDraftPlugin : EditorPlugin
{
    public override void _EnterTree()
    {
        // Initialization of the plugin goes here.
    }

    public override void _ExitTree()
    {
        // Clean-up of the plugin goes here.
    }
}
#endif
