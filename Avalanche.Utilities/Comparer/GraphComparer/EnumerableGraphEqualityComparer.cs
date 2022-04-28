// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

/// <summary>Enumerable elements comparer.</summary>
public abstract class EnumerableGraphEqualityComparer : ReadOnlyAssignableClass, IEqualityComparer, IGraphEqualityComparer, ICyclical
{
    /// <summary>Hash initial value.</summary>
    public const int FNVHashBasis = unchecked((int)2166136261);
    /// <summary>Hash factor.</summary>
    public const int FNVHashPrime = 16777619;

    /// <summary></summary>
    static readonly ConstructorT<object, EnumerableGraphEqualityComparer> constructor = new(typeof(EnumerableGraphEqualityComparer<>));
    /// <summary></summary>
    static readonly ConstructorT2<object, EnumerableGraphEqualityComparer> constructor2 = new(typeof(EnumerableGraphEqualityComparer<,>));

    /// <summary>Explicitly assigned <see cref="IsCyclical"/> value.</summary>
    protected bool isCyclical;
    /// <summary></summary>
    public virtual bool IsCyclical { get => isCyclical; set => this.AssertWritable().isCyclical = value; }

    /// <summary></summary>
    /// <param name="elementComparer"><![CDATA[IEqualityComparer<Element>]]> and/or <![CDATA[IGraphEqualityComparer<Element>]]></param>
    public static EnumerableGraphEqualityComparer Create(Type elementType, object elementComparer) => constructor.Create(elementType, elementComparer);
    /// <summary></summary>
    /// <param name="elementComparer"><![CDATA[IEqualityComparer<Element>]]> and/or <![CDATA[IGraphEqualityComparer<Element>]]></param>
    public static EnumerableGraphEqualityComparer Create(Type listType, Type elementType, object elementComparer) => constructor2.Create(listType, elementType, elementComparer);
    /// <summary></summary>
    public abstract new bool Equals(object? x, object? y);
    /// <summary></summary>
    public abstract bool Equals(object? x, object? y, IGraphComparerContext2 context);

    /// <summary></summary>
    public abstract int GetHashCode(object obj);
    /// <summary></summary>
    public abstract int GetHashCode([DisallowNull] object obj, IGraphComparerContext context);

    /// <summary>Assign <paramref name="context"/> to <see cref="IGraphEqualityComparer.Context2"/> and return it.</summary>
    /// <returns><paramref name="context"/></returns>
    protected IGraphComparerContext? setContext(IGraphComparerContext? context) { IGraphEqualityComparer.Context.Value = context; return context; }
    /// <summary>Assign <paramref name="context"/> to <see cref="IGraphEqualityComparer.Context2"/> and return it.</summary>
    /// <returns><paramref name="context"/></returns>
    protected IGraphComparerContext2? setContext(IGraphComparerContext2? context) { IGraphEqualityComparer.Context2.Value = context; return context; }
}

/// <summary>Array elements comparer.</summary>
public static class ArrayGraphEqualityComparer
{
    /// <summary></summary>
    static readonly ConstructorT<object, EnumerableGraphEqualityComparer> constructor = new(typeof(ArrayGraphEqualityComparer<>));
    /// <summary></summary>
    public static EnumerableGraphEqualityComparer Create(Type elementType, object elementComparer) => constructor.Create(elementType, elementComparer);
}

/// <summary>Enumerable elements comparer.</summary>
/// <typeparam name="Element"></typeparam>
public class EnumerableGraphEqualityComparer_<Element> : EnumerableGraphEqualityComparer
{
    /// <summary>Element equality comparer.</summary>
    public readonly IEqualityComparer<Element>? elementComparer;
    /// <summary>Element equality comparer.</summary>
    public readonly IGraphEqualityComparer<Element>? graphElementComparer;

    /// <summary>Create comparer.</summary>
    /// <param name="elementComparer"><![CDATA[IEqualityComparer<Element>]]> and/or <![CDATA[IGraphEqualityComparer<Element>]]></param>
    public EnumerableGraphEqualityComparer_(object elementComparer)
    {
        this.elementComparer = elementComparer as IEqualityComparer<Element>;
        this.graphElementComparer = elementComparer as IGraphEqualityComparer<Element>;
        if (this.elementComparer == null && this.graphElementComparer == null) throw new ArgumentNullException(nameof(elementComparer));
        //this.isCyclical = graphElementComparer != null;
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
        // Graph comparison
        if (IsCyclical || graphElementComparer != null)
        {
            // Get previous context
            IGraphComparerContext2? prevContext = IGraphEqualityComparer.Context2.Value;
            // Place here context
            IGraphComparerContext2 context = prevContext ?? setContext(new GraphComparerContext2())!;
            try
            {
                // Already compared
                if (!context.Add(x, y)) return false;
                // Compare elements
                while (xNext && yNext)
                {
                    // Get elements
                    Element xe = xEtor.Current, ye = yEtor.Current;
                    // Compare
                    if (!graphElementComparer!.Equals(xe, ye, context)) return false;
                    // Move next
                    xNext = xEtor.MoveNext();
                    yNext = yEtor.MoveNext();
                }
            }
            finally
            {
                // Revert thread local
                IGraphEqualityComparer.Context2.Value = prevContext;
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
                if (!elementComparer!.Equals(xe, ye)) return false;
                // Move next
                xNext = xEtor.MoveNext();
                yNext = yEtor.MoveNext();
            }
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
        // Graph comparison
        if (IsCyclical || graphElementComparer != null)
        {
            // Get previous context
            IGraphComparerContext? prevContext = IGraphEqualityComparer.Context.Value;
            // Place here context
            IGraphComparerContext context = prevContext ?? setContext(new GraphComparerContext())!;
            try
            {
                // Already compared
                if (!context.Add(enumr)) return 0;
                // Hash-in each element
                foreach (Element e in enumr)
                {
                    // Hash in
                    if (e != null) result ^= graphElementComparer!.GetHashCode(e, context);
                    result *= FNVHashPrime;
                }
            }
            finally
            {
                // Revert thread local
                IGraphEqualityComparer.Context.Value = prevContext;
            }
        }
        // Regular comparison
        else
        {
            // Hash-in each element
            foreach (Element e in enumr)
            {
                // Hash in
                if (e != null) result ^= elementComparer!.GetHashCode(e);
                result *= FNVHashPrime;
            }
        }
        // Return hash
        return result;
    }

    /// <summary></summary>
    public override bool Equals(object? _x, object? _y, IGraphComparerContext2 context)
    {
        // Compare nulls
        if (_x == null && _y == null) return true;
        if (_x == null || _y == null) return false;
        // Cast
        IEnumerable<Element>? x = _x as IEnumerable<Element>, y = _y as IEnumerable<Element>;
        // Assert nulls
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        // Already compared
        if (!context.Add(x, y)) return false;

        // Mismatching lengths
        if (x is ICollection<Element> xc && y is ICollection<Element> yc && xc.Count != yc.Count) return false;

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
                if (!graphElementComparer!.Equals(xe, ye, context)) return false;
                // Move next
                xNext = xEtor.MoveNext();
                yNext = yEtor.MoveNext();
            }
        }
        // Regular comparison
        else
        {
            // Get previous context
            IGraphComparerContext2? prevContext = IGraphEqualityComparer.Context2.Value;
            // Assign thread local
            IGraphEqualityComparer.Context2.Value = context;
            try
            {
                // Compare elements
                while (xNext && yNext)
                {
                    // Get elements
                    Element xe = xEtor.Current, ye = yEtor.Current;
                    // Compare
                    if (!elementComparer!.Equals(xe, ye)) return false;
                    // Move next
                    xNext = xEtor.MoveNext();
                    yNext = yEtor.MoveNext();
                }
            }
            finally
            {
                // Revert thread-local
                IGraphEqualityComparer.Context2.Value = prevContext;
            }
        }

        // Excess elements
        if (xNext || yNext) return false;
        // Same
        return true;
    }

    /// <summary></summary>
    public override int GetHashCode([DisallowNull] object obj, IGraphComparerContext context)
    {
        // Null
        if (obj == null) return 0;
        //
        IEnumerable<Element>? enumr = obj as IEnumerable<Element>;
        //
        if (enumr == null) return obj.GetHashCode();
        // Already compared
        if (!context.Add(enumr)) return 0;
        // Init
        int result = FNVHashBasis;
        // Graph comparison
        if (IsCyclical || graphElementComparer != null)
        {
            // Hash-in each element
            foreach (Element e in enumr)
            {
                // Hash in
                if (e != null) result ^= graphElementComparer!.GetHashCode(e, context);
                result *= FNVHashPrime;
            }
        }
        // Regular comparison
        else
        {
            // Get previous context
            IGraphComparerContext? prevContext = IGraphEqualityComparer.Context.Value;
            // Assign thread local
            IGraphEqualityComparer.Context.Value = context;
            try
            {
                // Hash-in each element
                foreach (Element e in enumr)
                {
                    // Hash in
                    if (e != null) result ^= elementComparer!.GetHashCode(e);
                    result *= FNVHashPrime;
                }
            }
            finally
            {
                // Revert thread-local
                IGraphEqualityComparer.Context.Value = prevContext;
            }
        }
        // Return hash
        return result;
    }
}

/// <summary>Enumerable elements comparer.</summary>
/// <typeparam name="Element"></typeparam>
public class EnumerableGraphEqualityComparer<Element> : EnumerableGraphEqualityComparer_<Element>, IEqualityComparer<IEnumerable<Element>>, IGraphEqualityComparer<IEnumerable<Element>>
{
    /// <summary></summary>
    static Lazy<IEqualityComparer<IEnumerable<Element>>> instance = new(() => new EnumerableEqualityComparer<Element>(EqualityComparer<Element>.Default));
    /// <summary></summary>
    public static IEqualityComparer<IEnumerable<Element>> Instance => instance.Value;

    /// <summary>Create comparer.</summary>
    /// <param name="elementComparer"><![CDATA[IEqualityComparer<Element>]]></param>
    public EnumerableGraphEqualityComparer(object elementComparer) : base(elementComparer) { }

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
        // Graph comparison
        if (IsCyclical || graphElementComparer != null)
        {
            // Get previous context
            IGraphComparerContext2? prevContext = IGraphEqualityComparer.Context2.Value;
            // Place here context
            IGraphComparerContext2 context = prevContext ?? setContext(new GraphComparerContext2())!;
            try
            {            // Already compared
                if (!context.Add(x, y)) return false;
                // Compare elements
                while (xNext && yNext)
                {
                    // Get elements
                    Element xe = xEtor.Current, ye = yEtor.Current;
                    // Compare
                    if (!graphElementComparer!.Equals(xe, ye, context)) return false;
                    // Move next
                    xNext = xEtor.MoveNext();
                    yNext = yEtor.MoveNext();
                }
            }
            finally
            {
                // Revert thread local
                IGraphEqualityComparer.Context2.Value = prevContext;
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
                if (!elementComparer!.Equals(xe, ye)) return false;
                // Move next
                xNext = xEtor.MoveNext();
                yNext = yEtor.MoveNext();
            }
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
        // Graph comparison
        if (IsCyclical || graphElementComparer == null)
        {
            // Get previous context
            IGraphComparerContext? prevContext = IGraphEqualityComparer.Context.Value;
            // Place here context
            IGraphComparerContext context = prevContext ?? setContext(new GraphComparerContext())!;
            try
            {            // Already compared
                if (!context.Add(enumr)) return 0;
                // Hash-in each element
                foreach (Element e in enumr)
                {
                    // Hash in
                    if (e != null) result ^= graphElementComparer!.GetHashCode(e, context);
                    result *= FNVHashPrime;
                }
            }
            finally
            {
                // Revert thread local
                IGraphEqualityComparer.Context.Value = prevContext;
            }
        }
        // Regular comparison
        else
        {
            // Hash-in each element
            foreach (Element e in enumr)
            {
                // Hash in
                if (e != null) result ^= elementComparer!.GetHashCode(e);
                result *= FNVHashPrime;
            }
        }
        // Return hash
        return result;
    }

    /// <summary></summary>
    public bool Equals(IEnumerable<Element>? x, IEnumerable<Element>? y, IGraphComparerContext2 context)
    {
        // Assert nulls
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        // Already compared
        if (!context.Add(x, y)) return false;

        // Mismatching lengths
        if (x is ICollection<Element> xc && y is ICollection<Element> yc && xc.Count != yc.Count) return false;

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
                if (!graphElementComparer!.Equals(xe, ye, context)) return false;
                // Move next
                xNext = xEtor.MoveNext();
                yNext = yEtor.MoveNext();
            }
        }
        // Regular comparison        
        else
        {
            // Get previous context
            IGraphComparerContext2? prevContext = IGraphEqualityComparer.Context2.Value;
            // Assign thread local
            IGraphEqualityComparer.Context2.Value = context;
            try
            {
                // Compare elements
                while (xNext && yNext)
                {
                    // Get elements
                    Element xe = xEtor.Current, ye = yEtor.Current;
                    // Compare
                    if (!elementComparer!.Equals(xe, ye)) return false;
                    // Move next
                    xNext = xEtor.MoveNext();
                    yNext = yEtor.MoveNext();
                }
            }
            finally
            {
                // Revert thread-local
                IGraphEqualityComparer.Context2.Value = prevContext;
            }
        }
        // Excess elements
        if (xNext || yNext) return false;
        // Same
        return true;
    }

    /// <summary></summary>
    public int GetHashCode([DisallowNull] IEnumerable<Element> enumr, IGraphComparerContext context)
    {
        // Null
        if (enumr == null) return 0;
        // Already compared
        if (!context.Add(enumr)) return 0;
        // Init
        int result = FNVHashBasis;
        // Graph comparison
        if (IsCyclical || graphElementComparer != null)
        {
            // Hash-in each element
            foreach (Element e in enumr)
            {
                // Hash in
                if (e != null) result ^= graphElementComparer!.GetHashCode(e, context);
                result *= FNVHashPrime;
            }
        }
        // Regular comparison
        else
        {
            // Get previous context
            IGraphComparerContext? prevContext = IGraphEqualityComparer.Context.Value;
            // Assign thread local
            IGraphEqualityComparer.Context.Value = context;
            try
            {
                // Hash-in each element
                foreach (Element e in enumr)
                {
                    // Hash in
                    if (e != null) result ^= elementComparer!.GetHashCode(e);
                    result *= FNVHashPrime;
                }
            }
            finally
            {
                // Revert thread-local
                IGraphEqualityComparer.Context.Value = prevContext;
            }
        }
        // Return hash
        return result;
    }
}

/// <summary>Enumerable elements comparer.</summary>
/// <typeparam name="List"></typeparam>
/// <typeparam name="Element"></typeparam>
public class EnumerableGraphEqualityComparer<List, Element> : EnumerableGraphEqualityComparer<Element>, IEqualityComparer<List>, IGraphEqualityComparer<List> where List : IEnumerable<Element>
{
    /// <summary></summary>
    static Lazy<IEqualityComparer<List>> instance = new(() => new EnumerableEqualityComparer<List, Element>(EqualityComparer<Element>.Default));
    /// <summary></summary>
    public new static IEqualityComparer<List> Instance => instance.Value;

    /// <summary>Create comparer.</summary>
    /// <param name="elementComparer"><![CDATA[IEqualityComparer<Element>]]></param>
    public EnumerableGraphEqualityComparer(object elementComparer) : base(elementComparer) { }

    /// <summary>Compare arrays.</summary>
    public bool Equals(List? x, List? y) => base.Equals((IEnumerable<Element>?)x, (IEnumerable<Element>?)y);
    /// <summary>Calculate hashcode.</summary>
    public int GetHashCode(List enumr) => base.GetHashCode((IEnumerable<Element>)enumr);
    /// <summary></summary>
    public bool Equals(List? x, List? y, IGraphComparerContext2 context) => base.Equals((IEnumerable<Element>?)x, (IEnumerable<Element>?)y, context);
    /// <summary></summary>
    public int GetHashCode([DisallowNull] List enumr, IGraphComparerContext context) => base.GetHashCode((IEnumerable<Element>)enumr, context);
}


/// <summary>Enumerable elements comparer.</summary>
/// <typeparam name="Element"></typeparam>
public class ArrayGraphEqualityComparer<Element> : EnumerableGraphEqualityComparer<Element[], Element>
{
    /// <summary></summary>
    static Lazy<IEqualityComparer<Element[]>> instance = new(() => new EnumerableGraphEqualityComparer<Element[], Element>(GraphEqualityComparer<Element>.Instance));
    /// <summary></summary>
    public new static IEqualityComparer<Element[]> Instance => instance.Value;

    /// <summary>Create comparer.</summary>
    /// <param name="elementComparer"><![CDATA[IEqualityComparer<Element>]]></param>
    public ArrayGraphEqualityComparer(object elementComparer) : base(elementComparer) { }
}
