// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>Comparer that forwards comparison to <see cref="IGraphComparable{T}"/> and then to <see cref="IComparable{T}"/>.</summary>
public class GraphComparer : ReadOnlyAssignableClass, IComparer, IGraphComparer, ICyclical
{
    /// <summary></summary>
    static readonly ConstructorT<GraphComparer> constructor = new(typeof(GraphComparer<>));
    /// <summary></summary>
    public static GraphComparer Create(Type type) => constructor.Create(type);

    /// <summary>Singleton</summary>
    static GraphComparer instance = new();
    /// <summary>Singleton</summary>
    public static GraphComparer Instance => instance;

    /// <summary>Explicitly assigned <see cref="IsCyclical"/> value.</summary>
    protected bool isCyclical = true;
    /// <summary></summary>
    public virtual bool IsCyclical { get => isCyclical; set => this.AssertWritable().isCyclical = value; }

    /// <summary></summary>
    public virtual int Compare(object? x, object? y)
    {
        // Reference equal
        if (x == y) return 0;
        // Check nulls
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        // Graph compare
        if (x is IGraphComparable xgc) return xgc.CompareTo(y, new GraphComparerContext2());
        if (y is IGraphComparable ygc) return ygc.CompareTo(x, new GraphComparerContext2());
        // Regular compare
        if (x is IComparable xc) return xc.CompareTo(y);
        if (y is IComparable yc) return yc.CompareTo(x);
        // Cannot discern
        return 0;
    }
    /// <summary></summary>
    public virtual int Compare(object? x, object? y, IGraphComparerContext2 context)
    {
        // Reference equal
        if (x == y) return 0;
        // Check nulls
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        // Graph compare
        if (x is IGraphComparable xgc) return xgc.CompareTo(y, context);
        if (y is IGraphComparable ygc) return ygc.CompareTo(x, context);
        // Regular compare
        if (x is IComparable xc) return xc.CompareTo(y);
        if (y is IComparable yc) return yc.CompareTo(x);
        // Cannot discern
        return 0;
    }
}

/// <summary>Comparer that forwards comparison to <see cref="IGraphComparable{T}"/> and then to <see cref="IComparable{T}"/>.</summary>
public class GraphComparer_<T> : GraphComparer
{
    /// <summary></summary>
    public override int Compare(object? x, object? y)
    {
        // Reference equal
        if (x == y) return 0;
        // Check nulls
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        // Graph compare T
        if (x is IGraphComparable<T> xgct && y is T yt) return xgct.CompareTo(yt, new GraphComparerContext2());
        if (y is IGraphComparable<T> ygct && x is T xt) return ygct.CompareTo(xt, new GraphComparerContext2());
        // Graph compare
        if (x is IGraphComparable xgc) return xgc.CompareTo(y, new GraphComparerContext2());
        if (y is IGraphComparable ygc) return ygc.CompareTo(x, new GraphComparerContext2());
        // Regular compare T
        if (x is IComparable<T> xct && y is T yt_) return xct.CompareTo(yt_);
        if (y is IComparable<T> yct && x is T xt_) return yct.CompareTo(xt_);
        // Regular compare
        if (x is IComparable xc) return xc.CompareTo(y);
        if (y is IComparable yc) return yc.CompareTo(x);
        // Cannot discern
        return 0;
    }

    /// <summary></summary>
    public override int Compare(object? x, object? y, IGraphComparerContext2 context)
    {
        // Reference equal
        if (x == y) return 0;
        // Check nulls
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        // Graph compare T
        if (x is IGraphComparable<T> xgct && y is T yt) return xgct.CompareTo(yt, context);
        if (y is IGraphComparable<T> ygct && x is T xt) return ygct.CompareTo(xt, context);
        // Graph compare
        if (x is IGraphComparable xgc) return xgc.CompareTo(y, context);
        if (y is IGraphComparable ygc) return ygc.CompareTo(x, context);
        // Regular compare T
        if (x is IComparable<T> xct && y is T yt_) return xct.CompareTo(yt_);
        if (y is IComparable<T> yct && x is T xt_) return yct.CompareTo(xt_);
        // Regular compare
        if (x is IComparable xc) return xc.CompareTo(y);
        if (y is IComparable yc) return yc.CompareTo(x);
        // Cannot discern
        return 0;
    }
}

/// <summary>Comparer that forwards comparison to <see cref="IGraphComparable{T}"/> and then to <see cref="IComparable{T}"/>.</summary>
public class GraphComparer<T> : GraphComparer_<T>, IComparer<T>, IGraphComparer<T>
{
    /// <summary>Singleton</summary>
    static GraphComparer<T> instance = new();
    /// <summary>Singleton</summary>
    public new static GraphComparer<T> Instance => instance;

    /// <summary></summary>
    public int Compare(T? x, T? y)
    {
        // Reference equal
        if (!typeof(T).IsValueType && object.ReferenceEquals(x, y)) return 0;
        // Check nulls
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        // Graph compare T
        if (x is IGraphComparable<T> xgct && y is T yt) return xgct.CompareTo(yt, new GraphComparerContext2());
        if (y is IGraphComparable<T> ygct && x is T xt) return ygct.CompareTo(xt, new GraphComparerContext2());
        // Graph compare
        if (x is IGraphComparable xgc) return xgc.CompareTo(y, new GraphComparerContext2());
        if (y is IGraphComparable ygc) return ygc.CompareTo(x, new GraphComparerContext2());
        // Regular compare T
        if (x is IComparable<T> xct && y is T yt_) return xct.CompareTo(yt_);
        if (y is IComparable<T> yct && x is T xt_) return yct.CompareTo(xt_);
        // Regular compare
        if (x is IComparable xc) return xc.CompareTo(y);
        if (y is IComparable yc) return yc.CompareTo(x);
        // Cannot discern
        return 0;
    }

    /// <summary></summary>
    public int Compare(T? x, T? y, IGraphComparerContext2 context)
    {
        // Reference equal
        if (!typeof(T).IsValueType && object.ReferenceEquals(x, y)) return 0;
        // Check nulls
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        // Graph compare T
        if (x is IGraphComparable<T> xgct && y is T yt) return xgct.CompareTo(yt, context);
        if (y is IGraphComparable<T> ygct && x is T xt) return ygct.CompareTo(xt, context);
        // Graph compare
        if (x is IGraphComparable xgc) return xgc.CompareTo(y, context);
        if (y is IGraphComparable ygc) return ygc.CompareTo(x, context);
        // Regular compare T
        if (x is IComparable<T> xct && y is T yt_) return xct.CompareTo(yt_);
        if (y is IComparable<T> yct && x is T xt_) return yct.CompareTo(xt_);
        // Regular compare
        if (x is IComparable xc) return xc.CompareTo(y);
        if (y is IComparable yc) return yc.CompareTo(x);
        // Cannot discern
        return 0;
    }
}

