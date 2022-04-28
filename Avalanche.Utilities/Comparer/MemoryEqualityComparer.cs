// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Diagnostics.CodeAnalysis;

/// <summary></summary>
public abstract class MemoryEqualityComparer
{
    /// <summary></summary>
    static readonly ConstructorT<MemoryEqualityComparer> constructor = new(typeof(MemoryEqualityComparer<>));
    /// <summary></summary>
    public static MemoryEqualityComparer Create(Type elementType) => constructor.Create(elementType);
}

/// <summary></summary>
public class MemoryEqualityComparer<T> : MemoryEqualityComparer, IEqualityComparer<ReadOnlyMemory<T>>, IEqualityComparer<Memory<T>>
{
    /// <summary></summary>
    static MemoryEqualityComparer<T> instance = new();
    /// <summary></summary>
    public static MemoryEqualityComparer<T> Instance => instance;
    /// <summary>Hash initial value.</summary>
    public const int FNVHashBasis = unchecked((int)2166136261);
    /// <summary>Hash factor.</summary>
    public const int FNVHashPrime = 16777619;

    /// <summary></summary>
    bool IEqualityComparer<ReadOnlyMemory<T>>.Equals(ReadOnlyMemory<T> x, ReadOnlyMemory<T> y) => System.MemoryExtensions.SequenceEqual<T>(x.Span, y.Span);
    /// <summary></summary>
    bool IEqualityComparer<Memory<T>>.Equals(Memory<T> x, Memory<T> y) => System.MemoryExtensions.SequenceEqual<T>(x.Span, y.Span);
    /// <summary></summary>
    int IEqualityComparer<ReadOnlyMemory<T>>.GetHashCode([DisallowNull] ReadOnlyMemory<T> mem)
    {
        // Init
        int result = FNVHashBasis;
        // Get span
        ReadOnlySpan<T> span = mem.Span;
        // Hash-in each element
        for (int i=0; i<mem.Length; i++)
        {
            // Get element
            T element = span[i];
            // Hash in
            result ^= element!.GetHashCode();
            result *= FNVHashPrime;
        }
        // Return hash
        return result;
    }
    /// <summary></summary>
    int IEqualityComparer<Memory<T>>.GetHashCode([DisallowNull] Memory<T> mem)
    {
        // Init
        int result = FNVHashBasis;
        // Get span
        ReadOnlySpan<T> span = mem.Span;
        // Hash-in each element
        for (int i=0; i<mem.Length; i++)
        {
            // Get element
            T element = span[i];
            // Hash in
            result ^= element!.GetHashCode();
            result *= FNVHashPrime;
        }
        // Return hash
        return result;
    }
}

