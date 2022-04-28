// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Diagnostics.CodeAnalysis;

/// <summary>Extension methods for <see cref="IDisposeBelatable"/>.</summary>
public static class DisposeBelatableExtensions
{
    /// <summary>Try creates a handle that postpones the dispose of the object until all the belate-handles have been disposed.</summary>
    /// <returns>belating handle that must be diposed</returns>
    public static bool TryBelateDispose<T>(this T instance, [NotNullWhen(true)] out IDisposable? belateHandle) where T : IDisposeBelatable
        => instance.TryBelateDispose(out belateHandle);

    /// <summary>Creates a handle that postpones the dispose of the object until all the belate-handles have been disposed.</summary>
    /// <returns>belating handle that must be diposed</returns>
    public static IDisposable BelateDispose<T>(this T disposeBelatable) where T : IDisposeBelatable
    {
        // Could not create belate handle
        if (!disposeBelatable.TryBelateDispose(out IDisposable? belateHandle)) throw new InvalidOperationException($"Could not create belate handle");
        // Return belate handle
        return belateHandle;
    }

}
