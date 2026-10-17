using System;
using System.Threading;
using Godot;
using Godot.Collections;
using GodotTask;

namespace DoveDraft;

public partial class MapService : Node, IMapService, ISaveLoadable
{
    public event IMapService.MapLoadStatusChangedDelegate OnMapLoadStatusChanged;

    public int LoadStartTimeDelayMs { get; set; }

    public Map Current { get; private set; }

    //
    //  Godot Methods
    //

    public override void _EnterTree()
    {
        Services.Register<IMapService>(this);
        LocateAndUseExistingMap();
    }

    public override void _Ready()
    {
        LocateAndUseExistingMap();
    }

    public override void _ExitTree()
    {
        SetNewMap(null);
        Services.Unregister<IMapService>();
    }

    //
    //  IMapService Methods
    //

    public async GDTask TransitionTo(string mapScenePath, CancellationToken token)
    {
        // TODO - gracefully handle multiple attempts to call this at once.

        Log.For<MapService>($"Starting map transition to '{mapScenePath}'...");
        OnMapLoadStatusChanged?.Invoke(MapLoadStatus.Starting);
        await GDTask.Delay(LoadStartTimeDelayMs, token);

        OnMapLoadStatusChanged?.Invoke(MapLoadStatus.Loading);
        await GDTask.SwitchToThreadPool();

        // Load the scene from disk and spawn it.
        PackedScene mapScene = await LoadMapScene(mapScenePath, token);
        Log.For<MapService>("...loaded map scene...");

        Map loadedMap =
            mapScene.Instantiate() as Map ?? throw new Exception("Map was wrong node type.");
        Log.For<MapService>($"...spawned map '{loadedMap.Name}'...");

        await GDTask.SwitchToMainThread(token);

        // Apply it!
        SetNewMap(loadedMap);
        Log.For<MapService>("...completed map transition!");

        OnMapLoadStatusChanged?.Invoke(MapLoadStatus.Loaded);
    }

    public void Clear()
    {
        Log.For<MapService>("Cleared map.");
        SetNewMap(null);
    }

    //
    //  ISaveLoadable Methods
    //

    public BaseSaveData Save()
    {
        Log.For<MapService>($"Saving current map as '{Current?.SceneFilePath ?? "NULL"}'.");
        return new MapSaveData() { CurrentMapPath = Current?.SceneFilePath };
    }

    public void Load(BaseSaveData data)
    {
        MapSaveData mapData = (MapSaveData)data;

        if (string.IsNullOrEmpty(mapData.CurrentMapPath))
        {
            Log.For<MapService>("No map to load, skipping.");
            return;
        }

        // TODO - this needs better cancellation... right?
        TransitionTo(mapData.CurrentMapPath, CancellationToken.None).Forget();
    }

    //
    //  Private Methods
    //

    private async GDTask<PackedScene> LoadMapScene(string mapScenePath, CancellationToken token)
    {
        // Start loading the map's assets.
        Error err = ResourceLoader.LoadThreadedRequest(mapScenePath, typeHint: "PackedScene");
        if (err != Error.Ok)
        {
            throw new Exception($"Couldn't start loading map resource: {err}");
        }

        // Wait until the map is no longer loading.
        while (
            ResourceLoader.LoadThreadedGetStatus(mapScenePath)
            == ResourceLoader.ThreadLoadStatus.InProgress
        )
        {
            await GDTask.NextFrame(token);
        }

        // Verify that the load completed correctly.
        ResourceLoader.ThreadLoadStatus finalLoadStatus = ResourceLoader.LoadThreadedGetStatus(
            mapScenePath
        );
        if (finalLoadStatus != ResourceLoader.ThreadLoadStatus.Loaded)
        {
            throw new Exception($"Failed to load map resource: {finalLoadStatus}");
        }

        return ResourceLoader.LoadThreadedGet(mapScenePath) as PackedScene
            ?? throw new Exception("Map wasn't a PackedScene");
    }

    private void SetNewMap(Map newMap)
    {
        // Remove the old map.
        if (Current != null)
        {
            RemoveChild(Current);
            Current.QueueFree();
        }

        // Add the new map.
        Current = newMap;
        if (newMap != null)
        {
            newMap.GetParent()?.RemoveChild(newMap); // If the new map already has a parent, make sure to unparent the new map.
            AddChild(newMap);
        }
    }

    private void LocateAndUseExistingMap()
    {
        Log.For<MapService>("Checking for pre-existing maps...");

        // Check for any pre-existing maps and grab the first one.
        Array<Node> maps = GetTree().GetNodesInGroup(Map.GroupName);
        if (maps.Count > 1)
        {
            Log.For<MapService>($"Multiple maps already exist ({maps.Count}).");
        }
        Map preExistingMap = GetFirstMap(maps);

        if (preExistingMap == null || preExistingMap == Current)
        {
            Log.For<MapService>("...none found.");
            return;
        }

        SetNewMap(preExistingMap);
    }

    private Map GetFirstMap(Array<Node> potentialMaps)
    {
        foreach (Node node in potentialMaps)
        {
            if (node is Map map)
            {
                return map;
            }
        }

        return null;
    }
}

public partial class Services
{
    public static IMapService Map => Get<IMapService>();
}
