// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

// <docs>
/// <summary>Graph comparer context</summary>
public interface IGraphComparerContext
{
    /// <summary>Add visitation of <paramref name="x"/>.</summary>
    /// <returns>true if was added, false if has already been added.</returns>
    bool Add<T>(in T x);

    /// <summary>Check previous visitation of <paramref name="x"/>.</summary>
    /// <returns>true if previous visitation exists</returns>
    bool Contains<T>(in T x);
}
// </docs>

// <docs2>
/// <summary>Graph comparer context</summary>
public interface IGraphComparerContext2
{
    /// <summary>Add visitation of <paramref name="x"/> to <paramref name="y"/>.</summary>
    /// <returns>true if was added, false if has already been added.</returns>
    bool Add<T>(in T x, in T y);

    /// <summary>
    /// Check if equality of <paramref name="x"/> to <paramref name="y"/> has been compared.
    /// </summary>
    /// <returns>true if previous visitation exists</returns>
    bool Contains<T>(in T x, in T y);
}
// </docs2>
