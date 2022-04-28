// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>Enumerable elements comparer.</summary>
public abstract class EnumerableEqualityComparer : IEqualityComparer
{
    /// <summary>Hash initial value.</summary>
    public const int FNVHashBasis = unchecked((int)2166136261);
    /// <summary>Hash factor.</summary>
    public const int FNVHashPrime = 16777619;

    /// <summary></summary>
    static readonly ConstructorT<object, EnumerableEqualityComparer> constructor = new(typeof(EnumerableEqualityComparer<>));
    /// <summary></summary>
    static readonly ConstructorT2<object, EnumerableEqualityComparer> constructor2 = new(typeof(EnumerableEqualityComparer<,>));
    /// <summary></summary>
    public static EnumerableEqualityComparer Create(Type elementType, object elementComparer) => constructor.Create(elementType, elementComparer);
    /// <summary></summary>
    public static EnumerableEqualityComparer Create(Type listType, Type elementType, object elementComparer) => constructor2.Create(listType, elementType, elementComparer);
    /// <summary></summary>
    public abstract new bool Equals(object? x, object? y);
    /// <summary></summary>
    public abstract int GetHashCode(object obj);
}

/// <summary>Array elements comparer.</summary>
public static class ArrayEqualityComparer
{
    /// <summary></summary>
    static readonly ConstructorT<object, EnumerableEqualityComparer> constructor = new(typeof(ArrayEqualityComparer<>));
    /// <summary></summary>
    public static EnumerableEqualityComparer Create(Type elementType, object elementComparer) => constructor.Create(elementType, elementComparer);
}

/// <summary>Enumerable elements comparer.</summary>
/// <typeparam name="Element"></typeparam>
public class EnumerableEqualityComparer_<Element> : EnumerableEqualityComparer
{
    /// <summary>Element equality comparer.</summary>
    public readonly IEqualityComparer<Element> elementComparer;

    /// <summary>Create comparer.</summary>
    /// <param name="elementComparer"><![CDATA[IEqualityComparer<Element>]]></param>
    public EnumerableEqualityComparer_(object elementComparer)
    {
        this.elementComparer = (IEqualityComparer<Element>)elementComparer ?? throw new ArgumentNullException(nameof(elementComparer));
    }

    /// <summary>Compare arrays.</summary>
    public override bool Equals(object? _x, object? _y)
    {
        // Compare nulls
        if (_x == null && _y == null) return true;
        if (_x == null || _y == null) return false;
        // Cast
        IEnumerable<Element>? x = _x as IEnumerable<Element>, y = _y as IEnumerable<Element>;
        // Assert nulls
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;

        // Mismatching lengths
        if (x is ICollection<Element> xc && y is ICollection<Element> yc && xc.Count != yc.Count) return false;

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
            if (!elementComparer.Equals(xe, ye)) return false;
            // Move next
            xNext = xEtor.MoveNext();
            yNext = yEtor.MoveNext();
        }
        // Excess elements
        if (xNext || yNext) return false;
        // Same
        return true;
    }

    /// <summary>Calculate hashcode.</summary>
    public override int GetHashCode(object obj)
    {
        // Null
        if (obj == null) return 0;
        //
        IEnumerable<Element>? enumr = obj as IEnumerable<Element>;
        //
        if (enumr == null) return obj.GetHashCode();
        // Init
        int result = FNVHashBasis;
        // Hash-in each element
        foreach (Element e in enumr)
        {
            // Hash in
            if (e != null) result ^= elementComparer.GetHashCode(e);
            result *= FNVHashPrime;
        }
        // Return hash
        return result;
    }
}

/// <summary>Enumerable elements comparer.</summary>
/// <typeparam name="Element"></typeparam>
public class EnumerableEqualityComparer<Element> : EnumerableEqualityComparer_<Element>, IEqualityComparer<IEnumerable<Element>>
{
    /// <summary></summary>
    static Lazy<IEqualityComparer<IEnumerable<Element>>> instance = new(() => new EnumerableEqualityComparer<Element>(EqualityComparer<Element>.Default));
    /// <summary></summary>
    public static IEqualityComparer<IEnumerable<Element>> Instance => instance.Value;

    /// <summary>Create comparer.</summary>
    /// <param name="elementComparer"><![CDATA[IEqualityComparer<Element>]]></param>
    public EnumerableEqualityComparer(object elementComparer) : base(elementComparer) { }

    /// <summary>Compare arrays.</summary>
    public bool Equals(IEnumerable<Element>? x, IEnumerable<Element>? y)
    {
        // Assert nulls
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        // Mismatching lengths
        if (x is ICollection<Element> xc && y is ICollection<Element> yc && xc.Count != yc.Count) return false;
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
            if (!elementComparer.Equals(xe, ye)) return false;
            // Move next
            xNext = xEtor.MoveNext();
            yNext = yEtor.MoveNext();
        }
        // Excess elements
        if (xNext || yNext) return false;
        // Same
        return true;
    }

    /// <summary>Calculate hashcode.</summary>
    public int GetHashCode(IEnumerable<Element> enumr)
    {
        // Null
        if (enumr == null) return 0;
        // Init
        int result = FNVHashBasis;
        // Hash-in each element
        foreach (Element e in enumr)
        {
            // Hash in
            if (e != null) result ^= elementComparer.GetHashCode(e);
            result *= FNVHashPrime;
        }
        // Return hash
        return result;
    }
}

/// <summary>Enumerable elements comparer.</summary>
/// <typeparam name="List">List type, e.g <see cref="IEnumerable{T}"/>, <see cref="IList{T}"/>, <see cref="List{T}"/> or <![CDATA[T[]]]> </typeparam>
/// <typeparam name="Element"></typeparam>
public class EnumerableEqualityComparer<List, Element> : EnumerableEqualityComparer<Element>, IEqualityComparer<List> where List : IEnumerable<Element>
{
    /// <summary></summary>
    static Lazy<IEqualityComparer<List>> instance = new(() => new EnumerableEqualityComparer<List, Element>(EqualityComparer<Element>.Default));
    /// <summary></summary>
    public new static IEqualityComparer<List> Instance => instance.Value;

    /// <summary>Create comparer.</summary>
    /// <param name="elementComparer"><![CDATA[IEqualityComparer<Element>]]></param>
    public EnumerableEqualityComparer(object elementComparer) : base(elementComparer) { }

    /// <summary>Compare arrays.</summary>
    public bool Equals(List? x, List? y)
    {
        // Assert nulls
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        // Mismatching lengths
        if (x is ICollection<Element> xc && y is ICollection<Element> yc && xc.Count != yc.Count) return false;
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
            if (!elementComparer.Equals(xe, ye)) return false;
            // Move next
            xNext = xEtor.MoveNext();
            yNext = yEtor.MoveNext();
        }
        // Excess elements
        if (xNext || yNext) return false;
        // Same
        return true;
    }

    /// <summary>Calculate hashcode.</summary>
    public int GetHashCode(List enumr)
    {
        // Null
        if (enumr == null) return 0;
        // Init
        int result = FNVHashBasis;
        // Hash-in each element
        foreach (Element e in enumr)
        {
            // Hash in
            if (e != null) result ^= elementComparer.GetHashCode(e);
            result *= FNVHashPrime;
        }
        // Return hash
        return result;
    }
}

/// <summary>Enumerable elements comparer.</summary>
/// <typeparam name="Element"></typeparam>
public class ArrayEqualityComparer<Element> : EnumerableEqualityComparer<Element[], Element>
{
    /// <summary></summary>
    static Lazy<IEqualityComparer<Element[]>> instance = new(() => new EnumerableEqualityComparer<Element[], Element>(EqualityComparer<Element>.Default));
    /// <summary></summary>
    public new static IEqualityComparer<Element[]> Instance => instance.Value;

    /// <summary>Create comparer.</summary>
    /// <param name="elementComparer"><![CDATA[IEqualityComparer<Element>]]></param>
    public ArrayEqualityComparer(object elementComparer) : base(elementComparer) { }
}
