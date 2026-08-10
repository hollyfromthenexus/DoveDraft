using GodotTask;

public interface IMapService : IService
{
    Map Current { get; }

    GDTask TransitionTo(string mapScenePath);
}
