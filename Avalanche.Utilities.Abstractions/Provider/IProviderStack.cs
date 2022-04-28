// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider;

/// <summary>Provider that is stacked upon other providers.</summary>
public interface IProviderStack : IProvider
{
    /// <summary>Source provider that is decorated.</summary>
    IProvider[]? Sources { get; }
}

