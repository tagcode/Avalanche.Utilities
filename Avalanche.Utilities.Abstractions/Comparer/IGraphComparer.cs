// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Threading;

// <docs>
/// <summary>Object graph aware comparer. Detects cycles.</summary>
public interface IGraphComparer
{
    /// <summary>Store for context when transitioning between non-graph and graph comparers.</summary>
    static ThreadLocal<IGraphComparerContext2?> context2 = new();
    /// <summary>Store for context when transitioning between non-graph and graph comparers.</summary>
    public static ThreadLocal<IGraphComparerContext2?> Context2 => context2;

    /// <summary>Is comparer structure cyclical.</summary>
    bool IsCyclical { get; set; }

    /// <summary>Compares order of <paramref name="x"/> to <paramref name="y"/>.</summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="context">Graph traverse context</param>
    /// <returns>
    /// Signed interger that indicates relative value of <paramref name="x"/> to <paramref name="y"/>. 
    ///     <![CDATA[<0]]> for <paramref name="x"/> preceding <paramref name="y"/>.
    ///     <![CDATA[>0]]> for <paramref name="x"/> trailing <paramref name="y"/>.
    ///     <![CDATA[0]]> for <paramref name="x"/> being equal to <paramref name="y"/>.
    /// </returns>
    /// <remarks>
    /// Implementation may use <see cref="Context2"/> when 
    /// visiting non-graph-supported sub-comparer.
    /// </remarks>
    int Compare(object? x, object? y, IGraphComparerContext2 context);
}
// </docs>

// <docsT>
/// <summary>Object graph aware comparer. Detects cycles.</summary>
/// <remarks>
/// Implementation may be start node distinctive or agnostic. 
/// If former, then implementation typically uses order specific hashing, e.g. FNV. 
/// If later, then uses add or xor hashing between objects.
/// </remarks>
public interface IGraphComparer<in T>
{
    /// <summary>Is comparer structure cyclical.</summary>
    bool IsCyclical { get; set; }

    /// <summary>Compares order of <paramref name="x"/> to <paramref name="y"/>.</summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="context">Graph traverse context</param>
    /// <returns>
    /// Signed interger that indicates relative value of <paramref name="x"/> to <paramref name="y"/>. 
    ///     <![CDATA[<0]]> for <paramref name="x"/> preceding <paramref name="y"/>.
    ///     <![CDATA[>0]]> for <paramref name="x"/> trailing <paramref name="y"/>.
    ///     <![CDATA[0]]> for <paramref name="x"/> being equal to <paramref name="y"/>.
    /// </returns>
    /// <remarks>
    /// Implementation may use <see cref="IGraphComparer.Context2"/> when visiting 
    /// non-graph-supported sub-comparer.
    /// </remarks>
    int Compare(T? x, T? y, IGraphComparerContext2 context);
}
// </docsT>
