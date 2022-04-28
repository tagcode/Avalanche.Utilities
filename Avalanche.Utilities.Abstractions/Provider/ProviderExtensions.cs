// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider;

/// <summary>Extension methods for <see cref="IProvider"/>.</summary>
public static class ProviderExtensions
{
    /// <summary>Get <paramref name="key"/> or throw <see cref="KeyNotFoundException"/></summary>
    /// <exception cref="KeyNotFoundException">If <paramref name="key"/> is not found.</exception>
    public static object Get(this IProvider provider, object key)
    {
        // Try get
        if (provider.TryGetValue(key, out object value)) return value;
        // Error
        throw new KeyNotFoundException();
    }

    /// <summary>Is cache provider</summary>
    public static bool IsCache(this IProvider provider) => provider is ICached cachedProvider ? cachedProvider.IsCached : false;

    /// <summary>Clear cache</summary>
    public static void InvalidateCache(this IProvider? provider, bool deep = false)
    {
        if (provider is ICached cached) cached.InvalidateCache(deep);

        if (deep && provider is IDecoration decoration)
        {
            for (object? d = decoration; d != null; d = (d as IDecoration)?.Decoree)
                if (d is ICached _cached) _cached.InvalidateCache(deep);
        }
    }

}
