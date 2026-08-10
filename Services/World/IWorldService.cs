using Godot;

namespace DoveDraft;

public interface IWorldService : IService
{
    void AddNode(Node node);

    void RemoveNode(Node node);
}
