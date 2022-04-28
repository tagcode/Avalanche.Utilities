// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Threading;

/// <summary>Extension methods for <see cref="IGraphEqualityComparer"/> and <see cref="IGraphEqualityComparer{T}"/>.</summary>
public static class GraphEqualityComparerExtensions
{
    /// <summary>Thread local</summary>
    static readonly ThreadLocal<IGraphComparerContext?> graphComparerContext = new();
    /// <summary>Thread local</summary>
    static readonly ThreadLocal<IGraphComparerContext2?> graphComparerContext2 = new();

    /// <summary>
    /// Thread local for <see cref="IGraphEqualityComparer"/> and <see cref="IGraphEqualityComparer{T}"/> implementations for storing traverse context 
    /// while visiting non-graph comparers.
    /// </summary>
    public static ThreadLocal<IGraphComparerContext?> GraphComparerContext => graphComparerContext;

    /// <summary>
    /// Thread local for <see cref="IGraphEqualityComparer"/> and <see cref="IGraphEqualityComparer{T}"/> implementations for storing traverse context 
    /// while visiting non-graph comparers.
    /// </summary>
    public static ThreadLocal<IGraphComparerContext2?> GraphComparerContext2 => graphComparerContext2;
}
