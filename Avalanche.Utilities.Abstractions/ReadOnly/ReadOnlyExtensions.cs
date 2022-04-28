// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

/// <summary>Extension methods for <see cref="IReadOnly"/>.</summary>
public static class ReadOnlyExtensions
{
    /// <summary>Set instance into read-only state.</summary>
    public static T SetReadOnly<T>(this T instance) where T : IReadOnly { instance.ReadOnly = true; return instance; }
    /// <summary>Set instance into read-only state.</summary>
    public static T SetReadOnlyIf<T>(this T instance, bool condition) where T : IReadOnly { if (!condition) return instance; instance.ReadOnly = true; return instance; }
    /// <summary>Assert writable</summary>
    [DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T AssertWritable<T>(this T instance) where T : IReadOnly
    {
        // Assert
        if (instance.ReadOnly) throw new InvalidOperationException($"{instance} read-only");
        // Return T
        return instance;
    }
    
    /// <summary>Set all elements read-only</summary>
    public static void SetElementsReadOnly(IEnumerable enumr) 
    {
        // Assign each
        foreach (object element in enumr)
            if (element is IReadOnly @readonly)
                @readonly.ReadOnly = true;
    }
    
    /// <summary>Set all elements read-only</summary>
    public static E[] SetElementsReadOnly<E>(this E[] array) where E : IReadOnly
    {
        // Assign each
        foreach (E element in array)
            if (element is IReadOnly @readonly)
                @readonly.ReadOnly = true;
        // Return
        return array;
    }
    
    
    /// <summary>Set all elements read-only</summary>
    public static Enumr SetElementsReadOnly<Enumr, E>(this Enumr enumr) where Enumr : IEnumerable<E> where E : IReadOnly
    {
        // Assign each
        foreach (E element in enumr)
            if (element is IReadOnly @readonly)
                @readonly.ReadOnly = true;
        // Return
        return enumr;
    }
}

