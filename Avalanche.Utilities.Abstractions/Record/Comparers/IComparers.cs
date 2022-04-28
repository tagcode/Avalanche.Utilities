// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>Comparers for a type.</summary>
public interface IComparers
{
    /// <summary>Data type interface or type</summary>
    Type Type { get; set; }
    /// <summary>Does the type have cyclical structure.</summary>
    bool IsCyclical { get; set; }

    /// <summary>Equality comparer for datatype instances</summary>
    IEqualityComparer EqualityComparer { get; set; }
    /// <summary>Comparer for datatype instances</summary>
    IComparer Comparer { get; set; }

    /// <summary>Equality comparer for datatype instances</summary>
    IGraphEqualityComparer GraphEqualityComparer { get; set; }
    /// <summary>Comparer for datatype instances</summary>
    IGraphComparer GraphComparer { get; set; }

    /// <summary><![CDATA[IEqualityComparer<T>]]></summary>
    object EqualityComparerT { get; set; }
    /// <summary><![CDATA[IComparer<T>]]></summary>
    object ComparerT { get; set; }

    /// <summary><![CDATA[IGraphEqualityComparer<T>]]></summary>
    object GraphEqualityComparerT { get; set; }
    /// <summary><![CDATA[IGraphComparer<T>]]></summary>
    object GraphComparerT { get; set; }
}

/// <summary>Comparers for a <typeparamref name="T"/>.</summary>
public interface IComparers<T> : IComparers
{
    /// <summary>Equality comparer for datatype instances</summary>
    new IEqualityComparer<T> EqualityComparer { get; set; }
    /// <summary>Comparer for datatype instances</summary>
    new IComparer<T> Comparer { get; set; }

    /// <summary>Equality comparer for datatype instances</summary>
    new IGraphEqualityComparer<T> GraphEqualityComparer { get; set; }
    /// <summary>Comparer for datatype instances</summary>
    new IGraphComparer<T> GraphComparer { get; set; }
}


