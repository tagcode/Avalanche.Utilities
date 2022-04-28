// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>Compares by object reference.</summary>
public abstract class ReferenceEqualityComparer : IEqualityComparer
{
    /// <summary>Constructor</summary>
    static readonly ConstructorT<IEqualityComparer> constructor = new(typeof(ReferenceEqualityComparer<>));
    /// <summary>Create comparer</summary>
    public static IEqualityComparer Create(Type type) => constructor.Create(type);
    /// <summary>Singleton</summary>
    public static ReferenceEqualityComparer<object> Instance => ReferenceEqualityComparer<object>.Instance;

    /// <summary></summary>
    bool IEqualityComparer.Equals(object? x, object? y) => object.ReferenceEquals(x, y);
    /// <summary></summary>
    int IEqualityComparer.GetHashCode(object obj) => obj == null ? 0 : obj.GetHashCode();
}

/// <summary>Compares by object reference.</summary>
public class ReferenceEqualityComparer<T> : ReferenceEqualityComparer, IEqualityComparer<T>
{
    /// <summary>Singleton</summary>
    static ReferenceEqualityComparer<T> instance = new ReferenceEqualityComparer<T>();
    /// <summary>Singleton</summary>
    public static new ReferenceEqualityComparer<T> Instance => instance;
    /// <summary>Compare by reference</summary>
    public bool Equals(T? x, T? y) => object.ReferenceEquals(x, y);
    /// <summary>Get original hashcode</summary>
    public int GetHashCode(T obj) => obj == null ? 0 : RuntimeHelpers.GetHashCode(obj);
}

