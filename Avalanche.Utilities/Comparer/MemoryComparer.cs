// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

/// <summary></summary>
public abstract class MemoryComparer
{
    /// <summary></summary>
    static readonly ConstructorT<MemoryComparer> constructor = new(typeof(MemoryComparer<>));
    /// <summary></summary>
    public static MemoryComparer Create(Type elementType) => constructor.Create(elementType);
}

/// <summary></summary>
public class MemoryComparer<T> : MemoryComparer, IComparer<ReadOnlyMemory<T>>, IComparer<Memory<T>> where T : IComparable<T>
{
    /// <summary></summary>
    static MemoryComparer<T> instance = new();
    /// <summary></summary>
    public static MemoryComparer<T> Instance => instance;
    /// <summary>Hash initial value.</summary>
    public const int FNVHashBasis = unchecked((int)2166136261);
    /// <summary>Hash factor.</summary>
    public const int FNVHashPrime = 16777619;

    /// <summary></summary>
    int IComparer<Memory<T>>.Compare(Memory<T> x, Memory<T> y) => System.MemoryExtensions.SequenceCompareTo<T>(x.Span, y.Span);
    /// <summary></summary>
    int IComparer<ReadOnlyMemory<T>>.Compare(ReadOnlyMemory<T> x, ReadOnlyMemory<T> y) => System.MemoryExtensions.SequenceCompareTo<T>(x.Span, y.Span);
}

