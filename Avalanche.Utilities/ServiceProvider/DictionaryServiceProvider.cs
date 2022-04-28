// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

/// <summary>Adapts dictionary to service provider.</summary>
public class DictionaryServiceProvider : LockableDictionary<Type, object>, IServiceProvider, IReadOnly
{
    /// <summary></summary>
    public DictionaryServiceProvider() : base() { }
    /// <summary></summary>
    public DictionaryServiceProvider(IDictionary<Type, object> map) : base(map) { }
    /// <summary>Get service</summary>
    public object? GetService(Type serviceType)
    {
        // Get service
        if (map.TryGetValue(serviceType, out object? service)) return service;
        // No service
        return null;
    }
}

/// <summary></summary>
public static class _DictionaryExtensions
{
    /// <summary></summary>
    public static IServiceProvider AsServiceProvider(this IDictionary<Type, object> map, bool asReadOnly = true)
    {
        // Adapt to IServiceProvider
        DictionaryServiceProvider sp = new DictionaryServiceProvider(map);
        // Make readonly
        if (asReadOnly) sp.SetReadOnly();
        // Return
        return sp;
    }
}
