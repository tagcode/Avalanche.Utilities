// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

// <docs>
/// <summary>Object graph aware equality comparer. Detects cycles.</summary>
public interface IGraphEqualityComparer
{
    /// <summary>Store for context when transitioning between non-graph and graph comparers.</summary>
    static ThreadLocal<IGraphComparerContext?> context = new();
    /// <summary>Store for context when transitioning between non-graph and graph comparers.</summary>
    static ThreadLocal<IGraphComparerContext2?> context2 = new();
    /// <summary>Store for context when transitioning between non-graph and graph comparers.</summary>
    public static ThreadLocal<IGraphComparerContext?> Context => context;
    /// <summary>Store for context when transitioning between non-graph and graph comparers.</summary>
    public static ThreadLocal<IGraphComparerContext2?> Context2 => context2;

    /// <summary>Is comparer structure cyclical.</summary>
    bool IsCyclical { get; set; }

    /// <summary>Compares for equality of <paramref name="x"/> and <paramref name="y"/>. Detects object cycles.</summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="context">Compare evaluation context.</param>
    /// <returns></returns>
    /// <remarks>Implementation may use <see cref="Context2"/> when visiting non-graph-supported sub-comparer.</remarks>
    bool Equals(object? x, object? y, IGraphComparerContext2 context);

    /// <summary>Return a hash code for <paramref name="obj"/>.</summary>
    /// <param name="obj">Object to hash</param>
    /// <param name="context">Hashing context</param>
    /// <remarks>Implementation may use <see cref="Context"/> when visiting non-graph-supported sub-comparer.</remarks>
    int GetHashCode([DisallowNull] object obj, IGraphComparerContext context);
}
// </docs>

// <docsT>
/// <summary>Object graph aware equality comparer. Detects cycles.</summary>
/// <remarks>
/// Implementation may be start node distinctive or agnostic. 
/// If former, then implementation typically uses order specific hashing, e.g. FNV. 
/// If later, then uses add or xor hashing between objects.
/// </remarks>
public interface IGraphEqualityComparer<in T>
{
    /// <summary>Is comparer structure cyclical.</summary>
    bool IsCyclical { get; set; }

    /// <summary>
    /// Compares for equality of <paramref name="x"/> and <paramref name="y"/>. 
    /// Detects object cycles.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="context">Compare evaluation context.</param>
    /// <returns></returns>
    /// <remarks>
    /// Implementation may use <see cref="IGraphEqualityComparer.Context2"/> 
    /// when visiting non-graph-supported sub-comparer.
    /// </remarks>
    bool Equals(T? x, T? y, IGraphComparerContext2 context);

    /// <summary>Return a hash code for <paramref name="obj"/>.</summary>
    /// <param name="obj">Object to hash</param>
    /// <param name="context">Hashing context</param>
    /// <remarks>
    /// Implementation may use <see cref="IGraphEqualityComparer.Context"/> 
    /// when visiting non-graph-supported sub-comparer.
    /// </remarks>
    int GetHashCode([DisallowNull] T obj, IGraphComparerContext context);
}
// </docsT>
