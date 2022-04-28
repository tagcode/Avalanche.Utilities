// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

/// <summary>Methods and extension methods for <see cref="IServiceProvider"/></summary>
public static class ServiceProviderExtensions
{
    /// <summary>Convert <paramref name="dictionary"/> to <see cref="IServiceProvider"/>.</summary>
    public static IServiceProvider? ToServiceProvider(this IDictionary<Type, object> dictionary)
        => new DictionaryServiceProvider(dictionary).SetReadOnly();

    /// <summary>Append service, overwrites previous service of same <paramref name="serviceType"/></summary>
    public static IServiceProvider? Append(this IServiceProvider? serviceProvider, Type serviceType, object service)
    {
        // Create single line service provider
        if (serviceProvider == null) return new SingleLineServiceProvider(serviceType, service);
        // 
        if (serviceProvider is DictionaryServiceProvider mapAdapter)
        {
            // Return same adapter
            if (mapAdapter.TryGetValue(serviceType, out object? _service) && _service == service) return mapAdapter;
            // Copy elements
            Dictionary<Type, object> map = new Dictionary<Type, object>(mapAdapter);
            // Append
            map[serviceType] = service;
            // Create new provider
            return new DictionaryServiceProvider(map);
        }
        // Return same line or overwrite line
        if (serviceProvider is SingleLineServiceProvider ssp && ssp.ServiceType == serviceType) return ssp.Service == service ? ssp : new SingleLineServiceProvider(serviceType, service);
        // Append line to previous service
        return new SingleLineServiceProvider(serviceType, service, serviceProvider);
    }

    /// <summary>Append service, overwrites previous service of same <paramref name="serviceType"/></summary>
    public static IServiceProvider? AppendIfNew(this IServiceProvider? serviceProvider, Type serviceType, object service)
    {
        // Create single line service provider
        if (serviceProvider == null) return new SingleLineServiceProvider(serviceType, service);
        // 
        if (serviceProvider is DictionaryServiceProvider mapAdapter)
        {
            // Already exists
            if (mapAdapter.ContainsKey(serviceType)) return mapAdapter;
            // Copy elements
            Dictionary<Type, object> map = new Dictionary<Type, object>(mapAdapter);
            // Append
            map[serviceType] = service;
            // Create new provider
            return new DictionaryServiceProvider(map);
        }
        // Return existing line
        if (serviceProvider is SingleLineServiceProvider ssp && ssp.ServiceType == serviceType) return ssp;
        // Append line to previous service
        return new SingleLineServiceProvider(serviceType, service, serviceProvider);
    }

}
