// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>Enumerable elements comparer.</summary>
public abstract class EnumerableGraphComparer : ReadOnlyAssignableClass, IComparer, IGraphComparer, ICyclical
{
    /// <summary></summary>
    static readonly ConstructorT<object, EnumerableGraphComparer> constructor = new(typeof(EnumerableGraphComparer<>));
    /// <summary></summary>
    static readonly ConstructorT2<object, EnumerableGraphComparer> constructor2 = new(typeof(EnumerableGraphComparer<,>));
    /// <summary></summary>
    /// <param name="elementComparer"><![CDATA[IComparer<Element>]]> and/or <![CDATA[IGraphComparer<Element>]]></param>
    public static EnumerableGraphComparer Create(Type elementType, object elementComparer) => constructor.Create(elementType, elementComparer);
    /// <summary></summary>
    /// <param name="elementComparer"><![CDATA[IComparer<Element>]]> and/or <![CDATA[IGraphComparer<Element>]]></param>
    public static EnumerableGraphComparer Create(Type listType, Type elementType, object elementComparer) => constructor2.Create(listType, elementType, elementComparer);

    /// <summary>Explicitly assigned <see cref="IsCyclical"/> value.</summary>
    protected bool isCyclical;
    /// <summary></summary>
    public virtual bool IsCyclical { get => isCyclical; set => this.AssertWritable().isCyclical = value; }

    /// <summary></summary>
    public abstract int Compare(object? x, object? y);
    /// <summary></summary>
    public abstract int Compare(object? x, object? y, IGraphComparerContext2 context);

    /// <summary>Assign <paramref name="context"/> to <see cref="IGraphComparer.Context2"/> and return it.</summary>
    /// <returns><paramref name="context"/></returns>
    protected IGraphComparerContext2? setContext(IGraphComparerContext2? context) { IGraphComparer.Context2.Value = context; return context; }
}

/// <summary>Array elements comparer.</summary>
public static class ArrayGraphComparer
{
    /// <summary></summary>
    static readonly ConstructorT<object, EnumerableGraphComparer> constructor = new(typeof(ArrayGraphComparer<>));
    /// <summary></summary>
    public static EnumerableGraphComparer Create(Type elementType, object elementComparer) => constructor.Create(elementType, elementComparer);
}

/// <summary>Enumerable elements comparer.</summary>
/// <typeparam name="Element"></typeparam>
public class EnumerableGraphComparer_<Element> : EnumerableGraphComparer
{
    /// <summary>Element equality comparer.</summary>
    public readonly IComparer<Element>? elementComparer;
    /// <summary>Element equality comparer.</summary>
    public readonly IGraphComparer<Element>? graphElementComparer;

    /// <summary>Create comparer.</summary>
    /// <param name="elementComparer"><![CDATA[IComparer<Element>]]> and/or <![CDATA[IGraphComparer<Element>]]></param>
    public EnumerableGraphComparer_(object elementComparer)
    {
        this.elementComparer = elementComparer as IComparer<Element>;
        this.graphElementComparer = elementComparer as IGraphComparer<Element>;
        if (this.elementComparer == null && this.graphElementComparer == null) throw new ArgumentNullException(nameof(elementComparer));
        //this.isCyclical = graphElementComparer != null;
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
        // Graph comparison
        if (IsCyclical || graphElementComparer != null)
        {
            // Get previous context
            IGraphComparerContext2? prevContext = IGraphComparer.Context2.Value;
            // Place here context
            IGraphComparerContext2 context = prevContext ?? setContext(new GraphComparerContext2())!;
            try
            {
                // Already compared
                if (!context.Add(x, y)) return 0;
                // Compare elements
                while (xNext && yNext)
                {
                    // Get elements
                    Element xe = xEtor.Current, ye = yEtor.Current;
                    // Compare
                    int d = graphElementComparer!.Compare(xe, ye, context);
                    // Got difference
                    if (d != 0) return d;
                    // Move next
                    xNext = xEtor.MoveNext();
                    yNext = yEtor.MoveNext();
                }
            }
            finally
            {
                // Revert thread local
                IGraphComparer.Context2.Value = prevContext;
            }
        }
        // Regular comparison
        else
        {
            // Compare elements
            while (xNext && yNext)
            {
                // Get elements
                Element xe = xEtor.Current, ye = yEtor.Current;
                // Compare
                int d = elementComparer!.Compare(xe, ye);
                // Got difference
                if (d != 0) return d;
                // Move next
                xNext = xEtor.MoveNext();
                yNext = yEtor.MoveNext();
            }
        }
        // Excess elements
        if (xNext) return -1;
        if (yNext) return 1;
        // Same
        return 0;
    }

    /// <summary></summary>
    public override int Compare(object? _x, object? _y, IGraphComparerContext2 context)
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
        // Already compared
        if (!context.Add(x, y)) return 0;
        //
        IEnumerator<Element> xEtor = x.GetEnumerator(), yEtor = y.GetEnumerator();
        // Has next
        bool xNext = xEtor.MoveNext(), yNext = yEtor.MoveNext();
        // Graph comparison
        if (IsCyclical || graphElementComparer != null)
        {
            // Compare elements
            while (xNext && yNext)
            {
                // Get elements
                Element xe = xEtor.Current, ye = yEtor.Current;
                // Compare
                int d = graphElementComparer!.Compare(xe, ye, context);
                // Got difference
                if (d != 0) return d;
                // Move next
                xNext = xEtor.MoveNext();
                yNext = yEtor.MoveNext();
            }
        }
        // Regular comparison
        else
        {
            // Get previous context
            IGraphComparerContext2? prevContext = IGraphComparer.Context2.Value;
            // Assign thread local
            IGraphComparer.Context2.Value = context;
            try
            {
                // Compare elements
                while (xNext && yNext)
                {
                    // Get elements
                    Element xe = xEtor.Current, ye = yEtor.Current;
                    // Compare
                    int d = elementComparer!.Compare(xe, ye);
                    // Got difference
                    if (d != 0) return d;
                    // Move next
                    xNext = xEtor.MoveNext();
                    yNext = yEtor.MoveNext();
                }
            }
            finally
            {
                // Revert thread-local
                IGraphComparer.Context2.Value = prevContext;
            }
        }

        // Excess elements
        if (xNext) return -1;
        if (yNext) return 1;
        // Same
        return 0;
    }
}

/// <summary>Enumerable elements comparer.</summary>
/// <typeparam name="Element"></typeparam>
public class EnumerableGraphComparer<Element> : EnumerableGraphComparer_<Element>, IComparer<IEnumerable<Element>>, IGraphComparer<IEnumerable<Element>>
{
    /// <summary></summary>
    static Lazy<IComparer<IEnumerable<Element>>> instance = new(() => new EnumerableComparer<Element>(Comparer<Element>.Default));
    /// <summary></summary>
    public static IComparer<IEnumerable<Element>> Instance => instance.Value;

    /// <summary>Create comparer.</summary>
    /// <param name="elementComparer"><![CDATA[IComparer<Element>]]></param>
    public EnumerableGraphComparer(object elementComparer) : base(elementComparer) { }

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
        // Graph comparison
        if (IsCyclical || graphElementComparer != null)
        {
            // Get previous context
            IGraphComparerContext2? prevContext = IGraphComparer.Context2.Value;
            // Place here context
            IGraphComparerContext2 context = prevContext ?? setContext(new GraphComparerContext2())!;
            try
            {
                // Already compared
                if (!context.Add(x, y)) return 0;
                // Compare elements
                while (xNext && yNext)
                {
                    // Get elements
                    Element xe = xEtor.Current, ye = yEtor.Current;
                    // Compare
                    int d = graphElementComparer!.Compare(xe, ye, context);
                    // Got difference
                    if (d != 0) return d;
                    // Move next
                    xNext = xEtor.MoveNext();
                    yNext = yEtor.MoveNext();
                }
            }
            finally
            {
                // Revert thread local
                IGraphComparer.Context2.Value = prevContext;
            }
        }
        // Regular comparison
        else
        {
            // Compare elements
            while (xNext && yNext)
            {
                // Get elements
                Element xe = xEtor.Current, ye = yEtor.Current;
                // Compare
                int d = elementComparer!.Compare(xe, ye);
                // Got difference
                if (d != 0) return d;
                // Move next
                xNext = xEtor.MoveNext();
                yNext = yEtor.MoveNext();
            }
        }

        // Excess elements
        if (xNext) return -1;
        if (yNext) return 1;
        // Same
        return 0;
    }

    /// <summary></summary>
    public int Compare(IEnumerable<Element>? x, IEnumerable<Element>? y, IGraphComparerContext2 context)
    {
        // Compare nulls
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        // Already compared
        if (!context.Add(x, y)) return 0;
        //
        IEnumerator<Element> xEtor = x.GetEnumerator(), yEtor = y.GetEnumerator();
        // Has next
        bool xNext = xEtor.MoveNext(), yNext = yEtor.MoveNext();
        // Graph comparison
        if (IsCyclical || graphElementComparer != null)
        {
            // Compare elements
            while (xNext && yNext)
            {
                // Get elements
                Element xe = xEtor.Current, ye = yEtor.Current;
                // Compare
                int d = graphElementComparer!.Compare(xe, ye, context);
                // Got difference
                if (d != 0) return d;
                // Move next
                xNext = xEtor.MoveNext();
                yNext = yEtor.MoveNext();
            }
        }
        // Regular comparison
        else
        {
            // Get previous context
            IGraphComparerContext2? prevContext = IGraphComparer.Context2.Value;
            // Assign thread local
            IGraphComparer.Context2.Value = context;
            try
            {
                // Compare elements
                while (xNext && yNext)
                {
                    // Get elements
                    Element xe = xEtor.Current, ye = yEtor.Current;
                    // Compare
                    int d = elementComparer!.Compare(xe, ye);
                    // Got difference
                    if (d != 0) return d;
                    // Move next
                    xNext = xEtor.MoveNext();
                    yNext = yEtor.MoveNext();
                }
            }
            finally
            {
                // Revert thread-local
                IGraphComparer.Context2.Value = prevContext;
            }
        }
        // Excess elements
        if (xNext) return -1;
        if (yNext) return 1;
        // Same
        return 0;
    }
}

/// <summary>Enumerable elements comparer.</summary>
/// <typeparam name="Element"></typeparam>
/// <typeparam name="List"></typeparam>
public class EnumerableGraphComparer<List, Element> : EnumerableGraphComparer<Element>, IComparer<List>, IGraphComparer<List> where List : IEnumerable<Element>
{
    /// <summary></summary>
    static Lazy<IComparer<List>> instance = new(() => new EnumerableComparer<List, Element>(Comparer<Element>.Default));
    /// <summary></summary>
    public new static IComparer<List> Instance => instance.Value;

    /// <summary></summary>
    public EnumerableGraphComparer(object elementComparer) : base(elementComparer) { }

    /// <summary>Compare order of two Enumerables</summary>
    public int Compare(List? x, List? y) => base.Compare((IEnumerable<Element>?)x, (IEnumerable<Element>?)y);
    /// <summary></summary>
    public int Compare(List? x, List? y, IGraphComparerContext2 context) => base.Compare((IEnumerable<Element>?)x, (IEnumerable<Element>?)y, context);
}

/// <summary>Enumerable elements comparer.</summary>
/// <typeparam name="Element"></typeparam>
public class ArrayGraphComparer<Element> : EnumerableGraphComparer<Element[], Element>
{
    /// <summary></summary>
    static Lazy<IComparer<Element[]>> instance = new(() => new EnumerableGraphComparer<Element[], Element>(GraphComparer<Element>.Instance));
    /// <summary></summary>
    public new static IComparer<Element[]> Instance => instance.Value;

    /// <summary>Create comparer.</summary>
    /// <param name="elementComparer"><![CDATA[IComparer<Element>]]></param>
    public ArrayGraphComparer(object elementComparer) : base(elementComparer) { }
}
