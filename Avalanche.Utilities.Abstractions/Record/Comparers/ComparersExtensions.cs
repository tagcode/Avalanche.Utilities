// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;

/// <summary>Extension methods for <see cref="IComparers"/>.</summary>
public static class ComparersExtensions
{
    /// <summary>Equality comparer for datatype instances</summary>
    public static C SetType<C>(this C comparers, Type value) where C : IComparers { comparers.Type = value; return comparers; }
    /// <summary>Equality comparer for datatype instances</summary>
    public static C SetCyclic<C>(this C comparers, bool value) where C : IComparers { comparers.IsCyclical = value; return comparers; }
    /// <summary>Equality comparer for datatype instances</summary>
    public static C SetEqualityComparer<C>(this C comparers, IEqualityComparer value) where C : IComparers { comparers.EqualityComparer = value; return comparers; }
    /// <summary>Comparer for datatype instances</summary>
    public static C SetComparer<C>(this C comparers, IComparer value) where C : IComparers { comparers.Comparer = value; return comparers; }
    /// <summary></summary>
    public static C SetGraphEqualityComparer<C>(this C comparers, IGraphEqualityComparer value) where C : IComparers { comparers.GraphEqualityComparer = value; return comparers; }
    /// <summary></summary>
    public static C SetGraphComparer<C>(this C comparers, IGraphComparer value) where C : IComparers { comparers.GraphComparer = value; return comparers; }

    /// <summary>Equality comparer for datatype instances</summary>
    public static C SetEqualityComparerT<C>(this C comparers, object value) where C : IComparers { comparers.EqualityComparerT = value; return comparers; }
    /// <summary>Comparer for datatype instances</summary>
    public static C SetComparerT<C>(this C comparers, object value) where C : IComparers { comparers.ComparerT = value; return comparers; }
    /// <summary></summary>
    public static C SetGraphEqualityComparerT<C>(this C comparers, object value) where C : IComparers { comparers.GraphEqualityComparerT = value; return comparers; }
    /// <summary></summary>
    public static C SetGraphComparerT<C>(this C comparers, object value) where C : IComparers { comparers.GraphComparerT = value; return comparers; }
}
