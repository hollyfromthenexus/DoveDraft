using Godot;
using Godot.Collections;

namespace DoveDraft;

public partial class DialogVariableSaveData : Resource
{
    [Export]
    public Dictionary<string, float> Floats { get; set; }

    [Export]
    public Dictionary<string, string> Strings { get; set; }

    [Export]
    public Dictionary<string, bool> Bools { get; set; }
}
