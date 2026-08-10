using Godot;

namespace DoveDraft;

public partial class WorldService : Node3D, IWorldService
{
    //
    //  Godot Methods
    //

    public override void _EnterTree()
    {
        Services.Register<IWorldService>(this);
    }

    public override void _ExitTree()
    {
        Services.Unregister<IWorldService>();
    }

    //
    //  IWorldService Methods
    //

    public void AddNode(Node node) => AddChild(node);

    public void RemoveNode(Node node) => RemoveNode(node);
}

public partial class Services
{
    public static IWorldService World => Get<IWorldService>();
}
