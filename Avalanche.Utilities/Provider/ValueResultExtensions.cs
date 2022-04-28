// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider;
using System.Diagnostics;
using System.Runtime.CompilerServices;

/// <summary>Extension methods for <see cref="ValueResult{T}"/>.</summary>
public static class ValueResultExtensions
{
    /// <summary>Try get value</summary>
    [DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetValue<T>(this ValueResult<T> result, out T value)
    {
        // No result
        if (result.Status != ResultStatus.Ok) { value = default!; return false; }
        // Assign result
        value = result.Value!;
        return true;
    }

    /// <summary>Assert status is OK and return value</summary>
    /// <exception cref="Exception"></exception>
    [DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AssertValue<T>(this ValueResult<T> result)
    {
        // Assert
        if (result.Status == ResultStatus.Error && result.Error != null) throw ExceptionUtilities.Wrap(result.Error!);
        // Assert
        if (result.Status != ResultStatus.Ok) throw new InvalidOperationException($"No result for {result.Request}", result.Error);
        // Return value
        return result.Value!;
    }

}
