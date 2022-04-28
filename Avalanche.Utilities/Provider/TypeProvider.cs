// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider;
using System;
using System.Reflection;

/// <summary>Loads type with <see cref="Type.GetType(string)"/>.</summary>
/// <remarks>May not detect a <see cref="Type"/> that has not been loaded into <see cref="AppDomain"/>. The caller may need to preload an <see cref="Assembly"/> beforehand.</remarks>
public class TypeProvider : ProviderBase<string?, Type[]>
{
    /// <summary>Singleton</summary>
    static readonly IProvider<string?, Type[]> instance = new TypeProvider(AppDomain.CurrentDomain);
    /// <summary>Singleton, cached</summary>
    static readonly IProvider<string?, Type[]> cached = CreateCached();
    /// <summary>Singleton, non-cached</summary>
    public static IProvider<string?, Type[]> Instance => instance;
    /// <summary>Singleton, cached</summary>
    public static IProvider<string?, Type[]> Cached => cached;
    /// <summary>Create type provider with new cache.</summary>
    public static IProvider<string?, Type[]> CreateCached() => Instance.ValueResultCaptured().CachedNullableKey().ValueResultOpened();

    /// <summary>Assemblies to use for trying to load type</summary>
    protected Assembly[]? assemblies;
    /// <summary>AppDomain to read type from</summary>
    protected AppDomain? appDomain;
    /// <summary>Assemblies to use for trying to load type</summary>
    public virtual Assembly[]? Assemblies => assemblies;
    /// <summary>AppDomain to read type from</summary>
    public virtual AppDomain? AppDomain => appDomain;

    /// <summary></summary>
    public TypeProvider()
    {
        this.appDomain = AppDomain.CurrentDomain;
    }

    /// <summary></summary>
    /// <param name="assemblies">Assemblies to use for trying to load type</param>
    public TypeProvider(Assembly[] assemblies)
    {
        this.assemblies = assemblies;
    }

    /// <summary></summary>
    /// <param name="appDomain">AppDomain to read type from</param>
    public TypeProvider(AppDomain appDomain)
    {
        this.appDomain = appDomain;
    }

    /// <summary></summary>
    /// <param name="appDomain">AppDomain to read type from</param>
    /// <param name="assemblies">Assemblies to use for trying to load type</param>
    public TypeProvider(AppDomain appDomain, Assembly[] assemblies)
    {
        this.assemblies = assemblies;
        this.appDomain = appDomain;
    }

    /// <summary>Get a snapshot of applicable assemblies.</summary>
    protected virtual Assembly[] GetAssemblies()
    {
        // Get snapshots
        var _assemblies = this.Assemblies;
        var _appdomain = this.AppDomain;
        // No assemblies
        if (_assemblies == null && _appdomain == null) return Array.Empty<Assembly>();
        // Only appdomain
        if (_assemblies == null && _appdomain != null) return _appdomain.GetAssemblies();
        // Only assemblies
        if (_assemblies != null && _appdomain == null) return _assemblies;
        // Combine assemblies
        var _domainassemblies = _appdomain!.GetAssemblies();
        HashSet<Assembly> result = new HashSet<Assembly>(_assemblies!.Length + _domainassemblies.Length);
        result.AddRange(_assemblies);
        result.AddRange(_domainassemblies);
        return result.ToArray();
    }

    /// <summary>Try load <paramref name="typeName"/>.</summary>
    public override bool TryGetValue(string? typeName, out Type[] types)
    {
        // Get a snapshot of assemblies
        Assembly[] _assemblies = GetAssemblies();
        // Query all types
        if (typeName == null)
        {
            // Create result list
            List<Type> result = new List<Type>(8192);
            // Add types from each assembly
            foreach (Assembly assembly in _assemblies)
            {
                Type[] _types = assembly.GetTypes();
                result.AddRange(_types);
            }
            // Return
            types = result.ToArray();
            return true;
        }
        // Initialize
        types = null!;
        // Get a snapshot of assemblies
        foreach (Assembly assembly in _assemblies)
        {
            // Try load type
            Type? type = assembly.GetType(typeName, throwOnError: false, ignoreCase: false)!;
            // Got type
            if (type != null) { types = new Type[] { type }; return true; }
        }
        // Return
        return false;
    }
}
