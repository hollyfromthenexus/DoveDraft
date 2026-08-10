using System.Collections.Generic;
using Godot;

namespace DoveDraft;

public partial class GameSaveService : Node, IGameSaveService
{
    //
    //  Godot Methods
    //

    public override void _EnterTree()
    {
        Services.Register<IGameSaveService>(this);
    }

    public override void _Process(double _)
    {
        if (Input.IsActionJustPressed("debug_saveload_save"))
        {
            SaveState();
        }
        if (Input.IsActionJustPressed("debug_saveload_load"))
        {
            LoadState();
        }
    }

    public override void _ExitTree()
    {
        Services.Unregister<IGameSaveService>();
    }

    //
    //  IGameSaveService Methods
    //

    public void SaveState()
    {
        const string saveLocation = "res://temp_save.tres";
        Log.For<GameSaveService>($"Starting save to '{saveLocation}'...");

        Dictionary<string, ISaveLoadable> statefulServicesByType = GetStatefulServicesByTypeName();
        List<SaveFileMappingData> dataMappings = new(statefulServicesByType.Count);

        foreach ((string typeName, ISaveLoadable service) in statefulServicesByType)
        {
            Log.For<GameSaveService>($"Saving {service}...");

            // Get the save data from this service.
            BaseSaveData saveData = service.Save();

            // Map it by the service's type name and store it.
            dataMappings.Add(new() { TypeKey = typeName, Data = saveData });
        }

        // Once all save data is generated, produce a save file from the mappings
        SaveFileData saveFile = new() { AllData = new(dataMappings) };

        Log.For<GameSaveService>($"All save data created, writing file...");
        ResourceSaver.Save(saveFile, saveLocation);
        Log.For<GameSaveService>($"...done!");
    }

    public void LoadState()
    {
        const string saveLocation = "res://temp_save.tres";
        Log.For<GameSaveService>($"Starting load from {saveLocation}...");

        SaveFileData saveFile = ResourceLoader.Load<SaveFileData>(
            saveLocation,
            cacheMode: ResourceLoader.CacheMode.IgnoreDeep
        );
        Dictionary<string, ISaveLoadable> statefulServicesByType = GetStatefulServicesByTypeName();

        foreach (SaveFileMappingData mapping in saveFile.AllData)
        {
            Log.For<GameSaveService>($"Loading {mapping.TypeKey}...");

            ISaveLoadable service;
            bool couldFind = statefulServicesByType.TryGetValue(mapping.TypeKey, out service);

            if (couldFind == false)
            {
                Log.For<GameSaveService>(
                    $"...NO SERVICE FOR '{mapping.TypeKey}' FOUND, skipping..."
                );
                continue;
            }

            service.Load(mapping.Data);
        }

        Log.For<GameSaveService>($"...load complete!");
    }

    //
    //  Private Methods
    //

    private IEnumerable<ISaveLoadable> GetStatefulServices()
    {
        foreach (IService service in Services.GetAll())
        {
            if (service is ISaveLoadable statefulService)
            {
                yield return statefulService;
            }
        }
    }

    private Dictionary<string, ISaveLoadable> GetStatefulServicesByTypeName()
    {
        Dictionary<string, ISaveLoadable> data = new(32);

        foreach (ISaveLoadable statefulService in GetStatefulServices())
        {
            data[statefulService.GetType().Name] = statefulService;
        }

        return data;
    }
}

public partial class Services
{
    public IGameSaveService GameSave => Get<IGameSaveService>();
}
