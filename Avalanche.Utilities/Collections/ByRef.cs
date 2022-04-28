// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Runtime.InteropServices;

/// <summary>Public version of the internal System.ByReference.</summary>
public readonly ref struct _ByReference<T>
{
    /// <summary>Pointer</summary>
    private readonly Span<T> span;

    /// <summary>Create reference of <paramref name="value"/></summary>
    public _ByReference(ref T value)
    {
        span = MemoryMarshal.CreateSpan(ref value, 1);
    }

    /// <summary>Create reference of <paramref name="span"/>'s first element.</summary>
    public _ByReference(Span<T> span)
    {
        if (span.Length != 1) throw new ArgumentException("Length must be 1.");
        this.span = span;
    }

    /// <summary>Get value reference</summary>
    public ref T Value => ref MemoryMarshal.GetReference(span);
}
