// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>Compares by object id.</summary>
public abstract class ReferenceComparer : IComparer
{
    /// <summary>Singleton</summary>
    public static ReferenceComparer<object> Instance => ReferenceComparer<object>.Instance;
    /// <summary>Constructor</summary>
    static readonly ConstructorT<IComparer> constructor = new(typeof(ReferenceComparer<>));
    /// <summary>Create comparer</summary>
    public static IComparer Create(Type type) => constructor.Create(type);

    /// <summary>Compare order by object id</summary>
    public int Compare(object? x, object? y)
    {
        int xi = x == null ? 0 : RuntimeHelpers.GetHashCode(x);
        int yi = y == null ? 0 : RuntimeHelpers.GetHashCode(y);
        if (xi < yi) return -1;
        if (xi > yi) return 1;
        return 0;
    }
}

/// <summary>Compares by object id.</summary>
public class ReferenceComparer<T> : ReferenceComparer, IComparer<T> where T : class
{
    /// <summary>Singleton</summary>
    static ReferenceComparer<T> instance = new ReferenceComparer<T>();
    /// <summary>Singleton</summary>
    public static new ReferenceComparer<T> Instance => instance;
    /// <summary>Compare order by object id</summary>
    public int Compare(T? x, T? y)
    {
        int xi = x == null ? 0 : RuntimeHelpers.GetHashCode(x);
        int yi = y == null ? 0 : RuntimeHelpers.GetHashCode(y);
        if (xi < yi) return -1;
        if (xi > yi) return 1;
        return 0;
    }
}

