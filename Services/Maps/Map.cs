using Godot;

namespace DoveDraft;

[GlobalClass]
public partial class Map : WorldEnvironment
{
    public const string GroupName = "maps";

    public override void _EnterTree()
    {
        AddToGroup(GroupName);
    }

    public override void _ExitTree()
    {
        RemoveFromGroup(GroupName);
    }
}
