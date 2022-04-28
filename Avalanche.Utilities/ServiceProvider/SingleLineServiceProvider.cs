// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

/// <summary>Single line service provider</summary>
public class SingleLineServiceProvider : IServiceProvider
{
    /// <summary></summary>
    public readonly Type ServiceType;
    /// <summary></summary>
    public readonly Object Service;
    /// <summary></summary>
    public readonly IServiceProvider? PreviousServiceProvider;

    /// <summary></summary>
    public SingleLineServiceProvider(Type serviceType, object service, IServiceProvider? previousServiceProvider = null)
    {
        ServiceType = serviceType;
        Service = service;
        PreviousServiceProvider = previousServiceProvider;
    }

    /// <summary>Get service</summary>
    public object? GetService(Type serviceType)
    {
        // Get service
        if (serviceType.Equals(ServiceType)) return Service;
        // Use previous service
        object? service = PreviousServiceProvider?.GetService(serviceType);
        // Return
        return service;
    }

    /// <summary></summary>
    public override string ToString() => $"{ServiceType}={Service}";
}
