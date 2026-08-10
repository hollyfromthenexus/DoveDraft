using Godot;

public interface IWorldService : IService
{
    void AddNode(Node node);

    void RemoveNode(Node node);
}
