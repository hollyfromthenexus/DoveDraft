using System.Threading;
using GodotTask;

namespace DoveDraft;

public interface IMapService : IService
{
    public delegate void MapLoadStatusChangedDelegate(MapLoadStatus status);

    public event MapLoadStatusChangedDelegate OnMapLoadStatusChanged;

    Map Current { get; }

    public int LoadStartTimeDelayMs { get; set; }

    GDTask TransitionTo(string mapScenePath, CancellationToken token);

    void Clear();
}
