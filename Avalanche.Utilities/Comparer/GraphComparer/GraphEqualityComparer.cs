// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

/// <summary>Comparer that forwards comparison to <see cref="IGraphEqualityComparable{T}"/> and then to <see cref="object.Equals(object?)"/>.</summary>
public class GraphEqualityComparer : ReadOnlyAssignableClass, IEqualityComparer, IGraphEqualityComparer, ICyclical
{
    /// <summary></summary>
    static readonly ConstructorT<GraphEqualityComparer> constructor = new(typeof(GraphEqualityComparer<>));
    /// <summary></summary>
    public static GraphEqualityComparer Create(Type type) => constructor.Create(type);

    /// <summary>Singleton</summary>
    static GraphEqualityComparer instance = new();
    /// <summary>Singleton</summary>
    public static GraphEqualityComparer Instance => instance;

    /// <summary>Explicitly assigned <see cref="IsCyclical"/> value.</summary>
    protected bool isCyclical = true;
    /// <summary></summary>
    public virtual bool IsCyclical { get => isCyclical; set => this.AssertWritable().isCyclical = value; }

    /// <summary></summary>
    public virtual bool Equals(object? x, object? y, IGraphComparerContext2 context)
    {
        // Reference equal
        if (x == y) return true;
        // Check nulls
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        // Graph compare
        if (x is IGraphEqualityComparable xgc) return xgc.EqualTo(y, context);
        if (y is IGraphEqualityComparable ygc) return ygc.EqualTo(x, context);
        // Regular compare
        return x.Equals(y);
    }

    /// <summary></summary>
    public new virtual bool Equals(object? x, object? y)
    {
        // Reference equal
        if (x == y) return true;
        // Check nulls
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        // Graph compare
        if (x is IGraphEqualityComparable xgc) return xgc.EqualTo(y, new GraphComparerContext2());
        if (y is IGraphEqualityComparable ygc) return ygc.EqualTo(x, new GraphComparerContext2());
        // Regular compare
        return x.Equals(y);
    }

    /// <summary></summary>
    public virtual int GetHashCode([DisallowNull] object obj, IGraphComparerContext context)
    {
        // Check nulls
        if (obj == null) return 0;
        // 
        if (obj is IGraphEqualityComparable go) return go.GetHashCode(context);
        //
        return obj.GetHashCode();
    }

    /// <summary></summary>
    public virtual int GetHashCode(object obj)
    {
        // Check nulls
        if (obj == null) return 0;
        // 
        if (obj is IGraphEqualityComparable go) return go.GetHashCode(new GraphComparerContext());
        //
        return obj.GetHashCode();
    }

    /// <summary></summary>
    bool IGraphEqualityComparer.Equals(object? x, object? y, IGraphComparerContext2 context)
    {
        // Reference equal
        if (x == y) return true;
        // Check nulls
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        // Graph compare
        if (x is IGraphEqualityComparable xgc) return xgc.EqualTo(y, context);
        if (y is IGraphEqualityComparable ygc) return ygc.EqualTo(x, context);
        // Regular compare
        return x.Equals(y);
    }

    /// <summary></summary>
    int IGraphEqualityComparer.GetHashCode(object obj, IGraphComparerContext context)
    {
        // Check nulls
        if (obj == null) return 0;
        // 
        if (obj is IGraphEqualityComparable go) return go.GetHashCode(context);
        //
        return obj.GetHashCode();
    }
}

/// <summary>Comparer that forwards comparison to <see cref="IGraphComparable{T}"/> and then to <see cref="IComparable{T}"/>.</summary>
public class GraphEqualityComparer_<T> : GraphEqualityComparer
{
    /// <summary></summary>
    public override bool Equals(object? x, object? y, IGraphComparerContext2 context)
    {
        // Reference equal
        if (x == y) return true;
        // Check nulls
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        // Graph compare T
        if (x is IGraphEqualityComparable<T> xgct && y is T yt) return xgct.EqualTo(yt, context);
        if (y is IGraphEqualityComparable<T> ygct && x is T xt) return ygct.EqualTo(xt, context);
        // Graph compare
        if (x is IGraphEqualityComparable xgc) return xgc.EqualTo(y, context);
        if (y is IGraphEqualityComparable ygc) return ygc.EqualTo(x, context);
        // Regular compare
        return x.Equals(y);
    }

    /// <summary></summary>
    public override bool Equals(object? x, object? y)
    {
        // Reference equal
        if (x == y) return true;
        // Check nulls
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        // Graph compare T
        if (x is IGraphEqualityComparable<T> xgct && y is T yt) return xgct.EqualTo(yt, new GraphComparerContext2());
        if (y is IGraphEqualityComparable<T> ygct && x is T xt) return ygct.EqualTo(xt, new GraphComparerContext2());
        // Graph compare
        if (x is IGraphEqualityComparable xgc) return xgc.EqualTo(y, new GraphComparerContext2());
        if (y is IGraphEqualityComparable ygc) return ygc.EqualTo(x, new GraphComparerContext2());
        // Regular compare
        return x.Equals(y);
    }

    /// <summary></summary>
    public override int GetHashCode([DisallowNull] object obj, IGraphComparerContext context)
    {
        // Check nulls
        if (obj == null) return 0;
        // 
        if (obj is IGraphEqualityComparable<T> got) return got.GetHashCode(context);
        if (obj is IGraphEqualityComparable go) return go.GetHashCode(context);
        //
        return obj.GetHashCode();
    }

    /// <summary></summary>
    public override int GetHashCode(object obj)
    {
        // Check nulls
        if (obj == null) return 0;
        // 
        if (obj is IGraphEqualityComparable<T> got) return got.GetHashCode(new GraphComparerContext());
        if (obj is IGraphEqualityComparable go) return go.GetHashCode(new GraphComparerContext());
        //
        return obj.GetHashCode();
    }
}

/// <summary>Comparer that forwards comparison to <see cref="IGraphComparable{T}"/> and then to <see cref="IComparable{T}"/>.</summary>
public class GraphEqualityComparer<T> : GraphEqualityComparer_<T>, IEqualityComparer<T>, IGraphEqualityComparer<T>
{
    /// <summary>Singleton</summary>
    static GraphEqualityComparer<T> instance = new();
    /// <summary>Singleton</summary>
    public new static GraphEqualityComparer<T> Instance => instance;

    /// <summary></summary>
    public bool Equals(T? x, T? y, IGraphComparerContext2 context)
    {
        // Reference equal
        if (!typeof(T).IsValueType && object.ReferenceEquals(x, y)) return true;
        // Check nulls
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        // Graph compare T
        if (x is IGraphEqualityComparable<T> xgct) return xgct.EqualTo(y, context);
        if (y is IGraphEqualityComparable<T> ygct) return ygct.EqualTo(x, context);
        // Graph compare
        if (x is IGraphEqualityComparable xgc) return xgc.EqualTo(y, context);
        if (y is IGraphEqualityComparable ygc) return ygc.EqualTo(x, context);
        // Regular compare
        return x.Equals(y);
    }

    /// <summary></summary>
    public bool Equals(T? x, T? y)
    {
        // Reference equal
        if (!typeof(T).IsValueType && object.ReferenceEquals(x, y)) return true;
        // Check nulls
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        // Graph compare T
        if (x is IGraphEqualityComparable<T> xgct) return xgct.EqualTo(y, new GraphComparerContext2());
        if (y is IGraphEqualityComparable<T> ygct) return ygct.EqualTo(x, new GraphComparerContext2());
        // Graph compare
        if (x is IGraphEqualityComparable xgc) return xgc.EqualTo(y, new GraphComparerContext2());
        if (y is IGraphEqualityComparable ygc) return ygc.EqualTo(x, new GraphComparerContext2());
        // Regular compare
        return x.Equals(y);
    }

    /// <summary></summary>
    public int GetHashCode([DisallowNull] T obj, IGraphComparerContext context)
    {
        // Check nulls
        if (obj == null) return 0;
        // 
        if (obj is IGraphEqualityComparable go) return go.GetHashCode(context);
        if (obj is IGraphEqualityComparable<T> got) return got.GetHashCode(context);
        //
        return obj.GetHashCode();
    }

    /// <summary></summary>
    public int GetHashCode([DisallowNull] T obj)
    {
        // Check nulls
        if (obj == null) return 0;
        // 
        if (obj is IGraphEqualityComparable go) return go.GetHashCode(new GraphComparerContext());
        if (obj is IGraphEqualityComparable<T> got) return got.GetHashCode(new GraphComparerContext());
        //
        return obj.GetHashCode();
    }
}

