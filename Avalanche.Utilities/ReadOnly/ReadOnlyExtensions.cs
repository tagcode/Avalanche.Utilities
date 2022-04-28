// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Diagnostics;
using System.Runtime.CompilerServices;

/// <summary>Extension methods for <see cref="IReadOnly"/>.</summary>
public static class ReadOnlyExtensions_
{
    /// <summary>Assert writable</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static T AssertWritable<T>(this T instance) where T : IReadOnly
    {
        // Assert
        if (instance.ReadOnly) throw new InvalidOperationException("Read-only");
        // Return T
        return instance;
    }

}
