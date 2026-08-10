using System;
using System.Collections.Generic;
using Godot;

namespace DoveDraft;

public partial class Services : Node
{
    public static Services Instance { get; private set; }

    private readonly Dictionary<Type, IService> registeredServices = [];

    public Services()
    {
        Instance = this;
    }

    public static TService Get<TService>() where TService : IService
        => (TService)Instance.registeredServices[typeof(TService)];

    public static bool TryGet<TService>(out TService service)
         where TService : IService
    {
        if (Instance.registeredServices.TryGetValue(typeof(TService), out IService foundService))
        {
            service = (TService)foundService;
            return true;
        }
        else
        {
            service = default;
            return false;
        }
    }

    public static IEnumerable<IService> GetAll() => Instance.registeredServices.Values;

    public static void Register<TService>(TService service)
        where TService : IService
        => Instance.registeredServices[typeof(TService)] = service;

    public static void Unregister<TService>()
        where TService : IService
        => Instance.registeredServices.Remove(typeof(TService));
}
