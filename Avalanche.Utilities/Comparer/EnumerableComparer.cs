// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>Enumerable elements comparer.</summary>
public abstract class EnumerableComparer : IComparer
{
    /// <summary></summary>
    static readonly ConstructorT<object, EnumerableComparer> constructor = new(typeof(EnumerableComparer<>));
    /// <summary></summary>
    static readonly ConstructorT2<object, EnumerableComparer> constructor2 = new(typeof(EnumerableComparer<,>));
    /// <summary></summary>
    public static EnumerableComparer Create(Type elementType, object elementComparer) => constructor.Create(elementType, elementComparer);
    /// <summary></summary>
    public static EnumerableComparer Create(Type listType, Type elementType, object elementComparer) => constructor2.Create(listType, elementType, elementComparer);
    /// <summary></summary>
    public abstract int Compare(object? x, object? y);
}

/// <summary>Array elements comparer.</summary>
public static class ArrayComparer
{
    /// <summary></summary>
    static readonly ConstructorT<object, EnumerableComparer> constructor = new(typeof(ArrayComparer<>));
    /// <summary></summary>
    public static EnumerableComparer Create(Type elementType, object elementComparer) => constructor.Create(elementType, elementComparer);
}

/// <summary>Enumerable elements comparer.</summary>
/// <typeparam name="Element"></typeparam>
public class EnumerableComparer_<Element> : EnumerableComparer
{
    /// <summary>Element equality comparer.</summary>
    protected IComparer<Element> elementComparer;

    /// <summary>Create comparer.</summary>
    /// <param name="elementComparer"><![CDATA[IComparer<Element>]]></param>
    public EnumerableComparer_(object elementComparer)
    {
        this.elementComparer = (IComparer<Element>)elementComparer ?? throw new ArgumentNullException(nameof(elementComparer));
    }

    /// <summary></summary>
    public override int Compare(object? _x, object? _y)
    {
        // Compare nulls
        if (_x == null && _y == null) return 0;
        if (_x == null) return -1;
        if (_y == null) return 1;
        // Cast
        IEnumerable<Element>? x = _x as IEnumerable<Element>, y = _y as IEnumerable<Element>;
        // Compare nulls
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        //
        IEnumerator<Element> xEtor = x.GetEnumerator(), yEtor = y.GetEnumerator();
        // Has next
        bool xNext = xEtor.MoveNext(), yNext = yEtor.MoveNext();
        // Compare elements
        while (xNext && yNext)
        {
            // Get elements
            Element xe = xEtor.Current, ye = yEtor.Current;
            // Compare
            int d = elementComparer.Compare(xe, ye);
            // Got difference
            if (d != 0) return d;
            // Move next
            xNext = xEtor.MoveNext();
            yNext = yEtor.MoveNext();
        }
        // Excess elements
        if (xNext) return 1;
        if (yNext) return -1;
        // Same
        return 0;
    }
}

/// <summary>Enumerable elements comparer.</summary>
/// <typeparam name="Element"></typeparam>
public class EnumerableComparer<Element> : EnumerableComparer_<Element>, IComparer<IEnumerable<Element>>, IComparer
{
    /// <summary></summary>
    static Lazy<IComparer<IEnumerable<Element>>> instance = new(() => new EnumerableComparer<Element>(Comparer<Element>.Default));
    /// <summary></summary>
    public static IComparer<IEnumerable<Element>> Instance => instance.Value;

    /// <summary>Create comparer.</summary>
    /// <param name="elementComparer"><![CDATA[IComparer<Element>]]></param>
    public EnumerableComparer(object elementComparer) : base(elementComparer) { }

    /// <summary>Compare order of two Enumerables</summary>
    public int Compare(IEnumerable<Element>? x, IEnumerable<Element>? y)
    {
        // Compare nulls
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        //
        IEnumerator<Element> xEtor = x.GetEnumerator(), yEtor = y.GetEnumerator();
        // Has next
        bool xNext = xEtor.MoveNext(), yNext = yEtor.MoveNext();
        // Compare elements
        while (xNext && yNext)
        {
            // Get elements
            Element xe = xEtor.Current, ye = yEtor.Current;
            // Compare
            int d = elementComparer.Compare(xe, ye);
            // Got difference
            if (d != 0) return d;
            // Move next
            xNext = xEtor.MoveNext();
            yNext = yEtor.MoveNext();
        }
        // Excess elements
        if (xNext) return 1;
        if (yNext) return -1;
        // Same
        return 0;
    }
}

/// <summary>Enumerable elements comparer.</summary>
/// <typeparam name="List">List type, e.g <see cref="IEnumerable{T}"/>, <see cref="IList{T}"/>, <see cref="List{T}"/> or <![CDATA[T[]]]> </typeparam>
/// <typeparam name="Element"></typeparam>
public class EnumerableComparer<List, Element> : EnumerableComparer<Element>, IComparer<List>, IComparer where List : IEnumerable<Element>
{
    /// <summary></summary>
    static Lazy<IComparer<List>> instance = new(() => new EnumerableComparer<List, Element>(Comparer<Element>.Default));
    /// <summary></summary>
    public new static IComparer<List> Instance => instance.Value;

    /// <summary>Create comparer.</summary>
    /// <param name="elementComparer"><![CDATA[IComparer<Element>]]></param>
    public EnumerableComparer(object elementComparer) : base(elementComparer) { }

    /// <summary>Compare order of two Enumerables</summary>
    public int Compare(List? x, List? y)
    {
        // Compare nulls
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        //
        IEnumerator<Element> xEtor = x.GetEnumerator(), yEtor = y.GetEnumerator();
        // Has next
        bool xNext = xEtor.MoveNext(), yNext = yEtor.MoveNext();
        // Compare elements
        while (xNext && yNext)
        {
            // Get elements
            Element xe = xEtor.Current, ye = yEtor.Current;
            // Compare
            int d = elementComparer.Compare(xe, ye);
            // Got difference
            if (d != 0) return d;
            // Move next
            xNext = xEtor.MoveNext();
            yNext = yEtor.MoveNext();
        }
        // Excess elements
        if (xNext) return 1;
        if (yNext) return -1;
        // Same
        return 0;
    }
}

/// <summary>Enumerable elements comparer.</summary>
/// <typeparam name="Element"></typeparam>
public class ArrayComparer<Element> : EnumerableComparer<Element[], Element>
{
    /// <summary></summary>
    static Lazy<IComparer<Element[]>> instance = new(() => new EnumerableComparer<Element[], Element>(Comparer<Element>.Default));
    /// <summary></summary>
    public new static IComparer<Element[]> Instance => instance.Value;

    /// <summary>Create comparer.</summary>
    /// <param name="elementComparer"><![CDATA[IComparer<Element>]]></param>
    public ArrayComparer(object elementComparer) : base(elementComparer) { }
}
