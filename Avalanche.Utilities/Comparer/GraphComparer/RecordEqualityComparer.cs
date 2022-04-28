// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Avalanche.Utilities.Provider;

/// <summary>
/// Equality comparer that applies an array of comparers, each of which must pass equality test.
/// 
/// Supports regular <see cref="IEqualityComparer"/>/<see cref="IEqualityComparer{T}"/> and object graph comparison with <see cref="IGraphEqualityComparer"/>/<see cref="IGraphEqualityComparer{T}"/>.
/// 
/// Supports two-phased initialization. <see cref="Comparers"/> can be assigned after initialization, and then can be set immutable with <see cref="IReadOnly"/>.
/// </summary>
public abstract class RecordEqualityComparer : ReadOnlyAssignableClass, IEqualityComparer, IGraphEqualityComparer, ICyclical
{
    /// <summary>Hash initial value.</summary>
    public const int FNVHashBasis = unchecked((int)2166136261);
    /// <summary>Hash factor.</summary>
    public const int FNVHashPrime = 16777619;

    /// <summary></summary>
    static readonly ConstructorT<RecordEqualityComparer> constructor = new(typeof(RecordEqualityComparer<>));
    /// <summary></summary>
    static readonly ConstructorT<IEnumerable, RecordEqualityComparer> constructor2 = new(typeof(RecordEqualityComparer<>));

    /// <summary>Comparer constructor</summary>
    static readonly IProvider<Type, RecordEqualityComparer> created = Providers.Func<Type, RecordEqualityComparer>(RecordEqualityComparer.Create);
    /// <summary>Comparer constructor</summary>
    static readonly IProvider<Type, IResult<RecordEqualityComparer>> createResult = created.ResultCaptured();
    /// <summary>Comparer provider from type</summary>
    static readonly IProvider<Type, IResult<RecordEqualityComparer>> cachedResult = createResult.WeakCached();
    /// <summary>Comparer provider from type, cached</summary>
    static readonly IProvider<Type, RecordEqualityComparer> cached = cachedResult.ResultOpened();

    /// <summary></summary>
    public static RecordEqualityComparer Create(Type elementType) => constructor.Create(elementType);
    /// <summary></summary>
    /// <param name="comparerEnumr">Enumerable of <![CDATA[IEqualityComparer<>]]>, <![CDATA[IGraphEqualityComparer<>]]>></param>
    public static RecordEqualityComparer Create(Type elementType, IEnumerable comparerEnumr) => constructor2.Create(elementType, comparerEnumr);

    /// <summary>Comparer constructor</summary>
    public static IProvider<Type, RecordEqualityComparer> Created => created;
    /// <summary>Comparer constructor</summary>
    public static IProvider<Type, IResult<RecordEqualityComparer>> CreateResult => createResult;
    /// <summary>Comparer provider from type</summary>
    public static IProvider<Type, IResult<RecordEqualityComparer>> CachedResult => cachedResult;
    /// <summary>Comparer provider from type, cached</summary>
    public static IProvider<Type, RecordEqualityComparer> Cached => cached;

    /// <summary>Explicitly assigned <see cref="IsCyclical"/> value.</summary>
    protected bool isCyclical;
    /// <summary></summary>
    public virtual bool IsCyclical { get => isCyclical; set => this.AssertWritable().isCyclical = value; }

    /// <summary></summary>
    public abstract bool Equals(object? x, object? y, IGraphComparerContext2 context);
    /// <summary></summary>
    public abstract int GetHashCode([DisallowNull] object obj, IGraphComparerContext context);
    /// <summary></summary>
    public abstract new bool Equals(object? x, object? y);
    /// <summary></summary>
    public abstract int GetHashCode(object obj);

    /// <summary></summary>
    protected object[] allComparers = null!;
    /// <summary></summary>
    protected abstract void setComparers(IEnumerable comparerEnumr);
    /// <summary></summary>
    public IEnumerable Comparers { get => allComparers; set => setComparers(value); }

    /// <summary>Assign <paramref name="context"/> to <see cref="IGraphEqualityComparer.Context2"/> and return it.</summary>
    /// <returns><paramref name="context"/></returns>
    protected IGraphComparerContext? setContext(IGraphComparerContext? context) { IGraphEqualityComparer.Context.Value = context; return context; }
    /// <summary>Assign <paramref name="context"/> to <see cref="IGraphEqualityComparer.Context2"/> and return it.</summary>
    /// <returns><paramref name="context"/></returns>
    protected IGraphComparerContext2? setContext(IGraphComparerContext2? context) { IGraphEqualityComparer.Context2.Value = context; return context; }
}

/// <summary>Equality comparer that applies an array of comparers, each of which must pass equality test.</summary>
public abstract class RecordEqualityComparer_<T> : RecordEqualityComparer
{
    /// <summary></summary>
    protected IEqualityComparer<T>?[] nonGraphComparers = null!;
    /// <summary></summary>
    protected IGraphEqualityComparer<T>?[] graphComparers = null!;

    /// <summary>Number of comparers that implement <![CDATA[IEqualityComparer<T>]]></summary>
    protected int comparerCount;
    /// <summary>Number of comparers that implement <![CDATA[IGraphEqualityComparer<T>]]></summary>
    protected int graphComparerCount;
    /// <summary>Number of comparers that implement only <![CDATA[IEqualityComparer<T>]]></summary>
    protected int nonGraphComparerOnlyCount;
    /// <summary>Number of comparers that implement only <![CDATA[IGraphEqualityComparer<T>]]></summary>
    protected int graphComparerOnlyCount;

    /// <summary></summary>
    public RecordEqualityComparer_() => setComparers(new object[0]);

    /// <summary></summary>
    /// <param name="comparerEnumr">IEnumerable of <![CDATA[IEqualityComparer<T>]]> or <![CDATA[IGraphEqualityComparer<T>]]></param>
    public RecordEqualityComparer_(IEnumerable comparerEnumr) => setComparers(comparerEnumr);

    /// <summary></summary>
    protected override void setComparers(IEnumerable comparerEnumr)
    {
        this.AssertWritable();
        StructList8<IEqualityComparer<T>?> _comparers = new();
        StructList8<IGraphEqualityComparer<T>?> _graph_comparers = new();
        StructList8<object> _all_comparers = new();

        int ix = 0;
        foreach (object o in comparerEnumr)
        {
            IEqualityComparer<T>? ec = o as IEqualityComparer<T>;
            IGraphEqualityComparer<T>? gec = o as IGraphEqualityComparer<T>;

            // Cast to either failed
            if (ec == null && gec == null) throw new InvalidOperationException($"Element {ix} must implement {CanonicalName.Print(typeof(IEqualityComparer<T>))} and/or {CanonicalName.Print(typeof(IGraphEqualityComparer<T>))}");
            // No need for graph comparison, discard it
            if (ec != null && gec != null && !gec.IsCyclical) gec = null;

            _all_comparers.Add(o);
            _comparers.Add(ec);
            _graph_comparers.Add(gec);

            ix++;
            if (ec != null) comparerCount++;
            if (gec != null) graphComparerCount++;
            if (ec != null && gec == null) nonGraphComparerOnlyCount++;
            if (ec == null && gec != null) graphComparerOnlyCount++;
        }

        this.allComparers = _all_comparers.ToArray();
        this.nonGraphComparers = _comparers.ToArray();
        this.graphComparers = _graph_comparers.ToArray();
        //this.isCyclical |= graphComparerCount > 0;
    }

    /// <summary></summary>
    public override bool Equals(object? _x, object? _y)
    {
        // Same reference
        if (object.ReferenceEquals(_x, _y)) return true;
        // Nulls
        if (_x == null && _y == null) return true;
        if (_x == null || _y == null) return false;
        //
        if (_x is T x && _y is T y)
        {
            // Got mixed comparers. Uses try-catch to store context for non-graph comparers.
            if (IsCyclical || graphComparerCount > 0)
            {
                // Get previous context
                IGraphComparerContext2? prevContext = IGraphEqualityComparer.Context2.Value;
                // Place here context
                IGraphComparerContext2 context = prevContext ?? setContext(new GraphComparerContext2())!;
                try
                {
                    // Already compared
                    if (!context.Add(x, y)) return true;
                    // Compare with each
                    for (int i = 0; i < allComparers.Length; i++)
                    {
                        // Compare
                        bool equals = graphComparers[i] != null ? graphComparers[i]!.Equals(x, y, context) : nonGraphComparers[i]!.Equals(x, y);
                        // Not equal
                        if (!equals) return false;
                    }
                }
                finally
                {
                    // Revert thread-local
                    IGraphEqualityComparer.Context2.Value = prevContext;
                }
            }
            // Only non-graph comparers
            else
            {
                // Compare with each
                for (int i = 0; i < allComparers.Length; i++)
                {
                    // Not equal
                    if (!nonGraphComparers[i]!.Equals(x, y)) return false;
                }
            }
            // Equals
            return true;
        }
        // Revert to equals
        else return _x.Equals(_y);
    }

    /// <summary></summary>
    public override bool Equals(object? _x, object? _y, IGraphComparerContext2 context)
    {
        // Same reference
        if (object.ReferenceEquals(_x, _y)) return true;
        // Nulls
        if (_x == null && _y == null) return true;
        if (_x == null || _y == null) return false;
        //
        if (_x is T x && _y is T y)
        {
            // Got mixed comparers. Uses try-catch to store context for non-graph comparers.
            if (IsCyclical || graphComparerCount > 0)
            {
                // Get previous context
                IGraphComparerContext2? prevContext = IGraphEqualityComparer.Context2.Value;
                // Assign thread local
                IGraphEqualityComparer.Context2.Value = context;
                try
                {
                    // Already compared
                    if (!context.Add(x, y)) return true;
                    // Compare with each
                    for (int i = 0; i < allComparers.Length; i++)
                    {
                        // Compare
                        bool equals = graphComparers[i] != null ? graphComparers[i]!.Equals(x, y, context) : nonGraphComparers[i]!.Equals(x, y);
                        // Not equal
                        if (!equals) return false;
                    }
                }
                finally
                {
                    // Revert thread-local
                    IGraphEqualityComparer.Context2.Value = prevContext;
                }
            }
            // Only non-graph comparers
            else
            {
                // Compare with each
                for (int i = 0; i < allComparers.Length; i++)
                {
                    // Not equal
                    if (!nonGraphComparers[i]!.Equals(x, y)) return false;
                }
            }
            // Equals
            return true;
        }
        // Revert to equals
        else return _x.Equals(_y);
    }

    /// <summary></summary>
    public override int GetHashCode(object obj)
    {
        //
        if (obj == null) return 0;
        //
        if (obj is T t)
        {
            //
            int result = FNVHashBasis;
            // Got mixed comparers. Uses try-catch to store context for non-graph comparers.
            if (IsCyclical || graphComparerCount > 0)
            {
                // Get previous context
                IGraphComparerContext? prevContext = IGraphEqualityComparer.Context.Value;
                // Place here context
                IGraphComparerContext context = prevContext ?? setContext(new GraphComparerContext())!;
                try
                {
                    // Already visited
                    if (!context.Add(obj)) return 0;
                    // Compare with each
                    for (int i = 0; i < allComparers.Length; i++)
                    {
                        // Hash in
                        result ^= (graphComparers[i] != null ? graphComparers[i]!.GetHashCode(t, context) : nonGraphComparers[i]!.GetHashCode(t)) ^ i;
                        // Prime shuffle
                        result *= FNVHashPrime;
                    }
                }
                finally
                {
                    // Clean up thread local
                    IGraphEqualityComparer.Context.Value = prevContext;
                }
            }
            // Only non-graph comparers
            else
            {
                // Get comparer count
                int count = allComparers.Length;
                // Compare with each
                for (int i = 0; i < count; i++)
                {
                    // Hash-in
                    result ^= nonGraphComparers[i]!.GetHashCode(t) ^ i;
                    result *= FNVHashPrime;
                }
            }
            //
            return result;
        }
        //
        return obj.GetHashCode();
    }

    /// <summary></summary>
    public override int GetHashCode([DisallowNull] object obj, IGraphComparerContext context)
    {
        //
        if (obj == null) return 0;
        //
        if (obj is T t)
        {
            //
            int result = FNVHashBasis;
            // Got mixed comparers. Uses try-catch to store context for non-graph comparers.
            if (IsCyclical || graphComparerCount > 0)
            {
                // Get previous context
                IGraphComparerContext? prevContext = IGraphEqualityComparer.Context.Value;
                // Assign thread local
                IGraphEqualityComparer.Context.Value = context;
                try
                {
                    // Already visited
                    if (!context.Add(obj)) return 0;
                    // Compare with each
                    for (int i = 0; i < allComparers.Length; i++)
                    {
                        // Hash in
                        result ^= (graphComparers[i] != null ? graphComparers[i]!.GetHashCode(t, context) : nonGraphComparers[i]!.GetHashCode(t)) ^ i;
                        // Prime shuffle
                        result *= FNVHashPrime;
                    }
                }
                finally
                {
                    // Clean up thread local
                    IGraphEqualityComparer.Context.Value = prevContext;
                }
            }
            // Only non-graph comparers
            else
            {
                // Get comparer count
                int count = allComparers.Length;
                // Compare with each
                for (int i = 0; i < count; i++)
                {
                    // Hash-in
                    result ^= nonGraphComparers[i]!.GetHashCode(t) ^ i;
                    result *= FNVHashPrime;
                }
            }
            //
            return result;
        }
        //
        return obj.GetHashCode();
    }

}

/// <summary>Equality comparer that applies an array of comparers, each of which must pass equality test.</summary>
public class RecordEqualityComparer<T> : RecordEqualityComparer_<T>, IEqualityComparer<T>, IGraphEqualityComparer<T>
{
    /// <summary></summary>
    public RecordEqualityComparer() : base() { }

    /// <summary></summary>
    /// <param name="comparerEnumr">IEnumerable of <![CDATA[IEqualityComparer<T>]]> or <![CDATA[IGraphEqualityComparer<T>]]></param>
    public RecordEqualityComparer(IEnumerable comparerEnumr) : base(comparerEnumr) { }

    /// <summary></summary>
    public bool Equals(T? x, T? y)
    {
        // Same reference
        if (!typeof(T).IsValueType && object.ReferenceEquals(x, y)) return true;
        // Nulls
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        // Got mixed comparers. Uses try-catch to store context for non-graph comparers.
        if (IsCyclical || graphComparerCount > 0)
        {
            // Get previous context
            IGraphComparerContext2? prevContext = IGraphEqualityComparer.Context2.Value;
            // Place here context
            IGraphComparerContext2 context = prevContext ?? setContext(new GraphComparerContext2())!;
            try
            {
                // Already compared
                if (!context.Add(x, y)) return true;
                // Compare with each
                for (int i = 0; i < allComparers.Length; i++)
                {
                    // Compare
                    bool equals = graphComparers[i] != null ? graphComparers[i]!.Equals(x, y, context) : nonGraphComparers[i]!.Equals(x, y);
                    // Not equal
                    if (!equals) return false;
                }
            }
            finally
            {
                // Revert thread-local
                IGraphEqualityComparer.Context2.Value = prevContext;
            }
        }
        // Only non-graph comparers
        else
        {
            // Compare with each
            for (int i = 0; i < allComparers.Length; i++)
            {
                // Not equal
                if (!nonGraphComparers[i]!.Equals(x, y)) return false;
            }
        }
        // Equals
        return true;
    }

    /// <summary></summary>
    public bool Equals(T? x, T? y, IGraphComparerContext2 context)
    {
        // Referable
        if (!typeof(T).IsValueType && object.ReferenceEquals(x, y)) return true;
        // Nulls
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        // Got mixed comparers. Uses try-catch to store context for non-graph comparers.
        if (IsCyclical || graphComparerCount > 0)
        {
            // Get previous context
            IGraphComparerContext2? prevContext = IGraphEqualityComparer.Context2.Value;
            // Assign thread local
            IGraphEqualityComparer.Context2.Value = context;
            try
            {
                // Already compared
                if (!context.Add(x, y)) return true;
                // Compare with each
                for (int i = 0; i < allComparers.Length; i++)
                {
                    // Compare
                    bool equals = graphComparers[i] != null ? graphComparers[i]!.Equals(x, y, context) : nonGraphComparers[i]!.Equals(x, y);
                    // Not equal
                    if (!equals) return false;
                }
            }
            finally
            {
                // Revert thread-local
                IGraphEqualityComparer.Context2.Value = prevContext;
            }
        }
        // Only non-graph comparers
        else
        {
            // Compare with each
            for (int i = 0; i < allComparers.Length; i++)
            {
                // Not equal
                if (!nonGraphComparers[i]!.Equals(x, y)) return false;
            }
        }
        // Equals
        return true;
    }

    /// <summary></summary>
    public int GetHashCode([DisallowNull] T obj)
    {
        //
        if (!typeof(T).IsValueType && obj == null) return 0;
        //
        int result = FNVHashBasis;
        // Got mixed comparers. Uses try-catch to store context for non-graph comparers.
        if (IsCyclical || graphComparerCount > 0)
        {
            // Get previous context
            IGraphComparerContext? prevContext = IGraphEqualityComparer.Context.Value;
            // Place here context
            IGraphComparerContext context = prevContext ?? setContext(new GraphComparerContext())!;
            try
            {
                // Already visited
                if (!context.Add(obj)) return 0;
                // Compare with each
                for (int i = 0; i < allComparers.Length; i++)
                {
                    // Hash in
                    result ^= (graphComparers[i] != null ? graphComparers[i]!.GetHashCode(obj, context) : nonGraphComparers[i]!.GetHashCode(obj)) ^ i;
                    // Prime shuffle
                    result *= FNVHashPrime;
                }
            }
            finally
            {
                // Clean up thread local
                IGraphEqualityComparer.Context.Value = prevContext;
            }
        }
        // Only non-graph comparers
        else
        {
            // Get comparer count
            int count = allComparers.Length;
            // Compare with each
            for (int i = 0; i < count; i++)
            {
                // Hash-in
                result ^= nonGraphComparers[i]!.GetHashCode(obj) ^ i;
                result *= FNVHashPrime;
            }
        }
        //
        return result;
    }

    /// <summary></summary>
    public int GetHashCode([DisallowNull] T obj, IGraphComparerContext context)
    {
        //
        int result = FNVHashBasis;
        // Got mixed comparers. Uses try-catch to store context for non-graph comparers.
        if (IsCyclical || graphComparerCount > 0)
        {
            // Get previous context
            IGraphComparerContext? prevContext = IGraphEqualityComparer.Context.Value;
            // Assign thread local
            IGraphEqualityComparer.Context.Value = context;
            try
            {
                // Already visited
                if (!context.Add(obj)) return 0;
                // Compare with each
                for (int i = 0; i < allComparers.Length; i++)
                {
                    // Hash in
                    result ^= (graphComparers[i] != null ? graphComparers[i]!.GetHashCode(obj, context) : nonGraphComparers[i]!.GetHashCode(obj)) ^ i;
                    // Prime shuffle
                    result *= FNVHashPrime;
                }
            }
            finally
            {
                // Clean up thread local
                IGraphEqualityComparer.Context.Value = prevContext;
            }
        }
        // Only non-graph comparers
        else
        {
            // Get comparer count
            int count = allComparers.Length;
            // Compare with each
            for (int i = 0; i < count; i++)
            {
                // Hash-in
                result ^= nonGraphComparers[i]!.GetHashCode(obj) ^ i;
                result *= FNVHashPrime;
            }
        }
        //
        return result;
    }
}

/// <summary>Extension methods for <see cref="RecordComparer"/>.</summary>
public static class RecordEqualityComparerExtensions
{
    /// <summary>Set comparers</summary>
    public static T SetComparers<T>(this T recordComparer, IEnumerable comparerEnumr) where T : RecordEqualityComparer { recordComparer.Comparers = comparerEnumr; return recordComparer; }
    /// <summary>Set comparers</summary>
    public static T SetComparers<T>(this T recordComparer, params object[] comparerEnumr) where T : RecordEqualityComparer { recordComparer.Comparers = comparerEnumr; return recordComparer; }
}

