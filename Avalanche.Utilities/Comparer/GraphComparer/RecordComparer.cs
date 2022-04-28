// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using Avalanche.Utilities.Provider;

/// <summary>
/// Comparer that applies an array of comparers in order.
/// 
/// Supports regular <see cref="IComparer"/>/<see cref="IComparer{T}"/> and object graph comparison with <see cref="IGraphComparer"/>/<see cref="IGraphComparer{T}"/>.
/// 
/// Supports two-phased initialization. <see cref="Comparers"/> can be assigned after initialization, and then can be set immutable with <see cref="IReadOnly"/>.
/// </summary>
public abstract class RecordComparer : ReadOnlyAssignableClass, IComparer, IGraphComparer, ICyclical
{
    /// <summary></summary>
    static readonly ConstructorT<RecordComparer> constructor = new(typeof(RecordComparer<>));
    /// <summary></summary>
    static readonly ConstructorT<IEnumerable, RecordComparer> constructor2 = new(typeof(RecordComparer<>));

    /// <summary>Comparer constructor</summary>
    static readonly IProvider<Type, RecordComparer> created = Providers.Func<Type, RecordComparer>(RecordComparer.Create);
    /// <summary>Comparer constructor</summary>
    static readonly IProvider<Type, IResult<RecordComparer>> createResult = created.ResultCaptured();
    /// <summary>Comparer provider from type</summary>
    static readonly IProvider<Type, IResult<RecordComparer>> cachedResult = createResult.WeakCached();
    /// <summary>Comparer provider from type, cached</summary>
    static readonly IProvider<Type, RecordComparer> cached = cachedResult.ResultOpened();

    /// <summary>Create <see cref="RecordComparer{T}"/></summary>
    public static RecordComparer Create(Type elementType) => constructor.Create(elementType);
    /// <summary>Create <see cref="RecordComparer{T}"/></summary>
    /// <param name="comparerEnumr">IEnumerable of <![CDATA[IComparer<T>]]></param>
    public static RecordComparer Create(Type elementType, IEnumerable comparerEnumr) => constructor2.Create(elementType, comparerEnumr);

    /// <summary>Comparer constructor</summary>
    public static IProvider<Type, RecordComparer> Created => created;
    /// <summary>Comparer constructor</summary>
    public static IProvider<Type, IResult<RecordComparer>> CreateResult => createResult;
    /// <summary>Comparer provider from type</summary>
    public static IProvider<Type, IResult<RecordComparer>> CachedResult => cachedResult;
    /// <summary>Comparer provider from type, cached</summary>
    public static IProvider<Type, RecordComparer> Cached => cached;

    /// <summary>Explicitly assigned <see cref="IsCyclical"/> value.</summary>
    protected bool isCyclical;
    /// <summary></summary>
    public virtual bool IsCyclical { get => isCyclical; set => this.AssertWritable().isCyclical = value; }

    /// <summary></summary>
    public abstract int Compare(object? x, object? y);
    /// <summary></summary>
    public abstract int Compare(object? x, object? y, IGraphComparerContext2 context);

    /// <summary></summary>
    protected object[] allComparers = null!;
    /// <summary></summary>
    protected abstract void setComparers(IEnumerable comparerEnumr);
    /// <summary></summary>
    public IEnumerable Comparers { get => allComparers; set => setComparers(value); }

    /// <summary>Assign <paramref name="context"/> to <see cref="IGraphComparer.Context2"/> and return it.</summary>
    /// <returns><paramref name="context"/></returns>
    protected IGraphComparerContext2? setContext(IGraphComparerContext2? context) { IGraphComparer.Context2.Value = context; return context; }
}

/// <summary>Comparer that applies an array of comparers in order.</summary>
public class RecordComparer_<T> : RecordComparer
{
    /// <summary></summary>
    protected IComparer<T>?[] nonGraphComparers = null!;
    /// <summary></summary>
    protected IGraphComparer<T>?[] graphComparers = null!;

    /// <summary>Number of comparers that implement <![CDATA[IComparer<T>]]></summary>
    protected int comparerCount;
    /// <summary>Number of comparers that implement <![CDATA[IGraphComparer<T>]]></summary>
    protected int graphComparerCount;
    /// <summary>Number of comparers that implement only <![CDATA[IComparer<T>]]></summary>
    protected int nonGraphComparerOnlyCount;
    /// <summary>Number of comparers that implement only <![CDATA[IGraphComparer<T>]]></summary>
    protected int graphComparerOnlyCount;

    /// <summary></summary>
    public RecordComparer_() => setComparers(new object[0]);

    /// <summary></summary>
    /// <param name="comparerEnumr">IEnumerable of <![CDATA[IComparer<T>]]> or <![CDATA[IGraphComparer<T>]]></param>
    public RecordComparer_(IEnumerable comparerEnumr) => setComparers(comparerEnumr);

    /// <summary></summary>
    protected override void setComparers(IEnumerable comparerEnumr)
    {
        this.AssertWritable();

        StructList8<IComparer<T>?> _comparers = new();
        StructList8<IGraphComparer<T>?> _graph_comparers = new();
        StructList8<object> _all_comparers = new();

        int ix = 0;
        foreach (object o in comparerEnumr)
        {
            IComparer<T>? ec = o as IComparer<T>;
            IGraphComparer<T>? gec = o as IGraphComparer<T>;

            // Cast to either failed
            if (ec == null && gec == null) throw new InvalidOperationException($"Element {ix} must implement {CanonicalName.Print(typeof(IComparer<T>))} and/or {CanonicalName.Print(typeof(IGraphComparer<T>))}");
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
    public override int Compare(object? _x, object? _y)
    {
        // Same reference
        if (object.ReferenceEquals(_x, _y)) return 0;
        // Check nulls
        if (_x == null && _y == null) return 0;
        if (_x == null) return -1;
        if (_y == null) return 1;
        //
        if (_x is T x && _y is T y)
        {
            // Got mixed comparers. Uses try-catch to store context for non-graph comparers.
            if (IsCyclical || graphComparerCount > 0)
            {
                // Get previous context
                IGraphComparerContext2? prevContext = IGraphComparer.Context2.Value;
                // Place here context
                IGraphComparerContext2 context = prevContext ?? setContext(new GraphComparerContext2())!;
                try
                {
                    // Already compared
                    if (!context.Add(x, y)) return 0;
                    // Compare with each
                    for (int i = 0; i < allComparers.Length; i++)
                    {
                        // Get graph comparer
                        IGraphComparer<T>? gec = graphComparers[i];
                        // Get diff
                        int d = gec != null ? gec.Compare(x, y, context) : nonGraphComparers[i]!.Compare(x, y);
                        // Got diff
                        if (d != 0) return d;
                    }
                    // Equals
                    return 0;
                }
                finally
                {
                    // Revert to previous context
                    IGraphComparer.Context2.Value = prevContext;
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
                    // Get diff
                    int d = nonGraphComparers[i]!.Compare(x, y);
                    // Got diff
                    if (d != 0) return d;
                }
            }
            // Equals
            return 0;
        }
        // Cannot discern
        else return 0;
    }

    /// <summary></summary>
    public override int Compare(object? _x, object? _y, IGraphComparerContext2 context)
    {
        // Same reference
        if (object.ReferenceEquals(_x, _y)) return 0;
        // Check nulls
        if (_x == null && _y == null) return 0;
        if (_x == null) return -1;
        if (_y == null) return 1;
        //
        if (_x is T x && _y is T y)
        {
            // Got mixed comparers. Uses try-catch to store context for non-graph comparers.
            if (IsCyclical || graphComparerCount > 0)
            {
                // Get previous context
                IGraphComparerContext2? prevContext = IGraphComparer.Context2.Value;
                // No context, create context, assign thread local
                IGraphComparer.Context2.Value = context;
                try
                {
                    // Already compared
                    if (!context.Add(x, y)) return 0;
                    // Compare with each
                    for (int i = 0; i < allComparers.Length; i++)
                    {
                        // Get graph comparer
                        IGraphComparer<T>? gec = graphComparers[i];
                        // Get diff
                        int d = gec != null ? gec.Compare(x, y, context) : nonGraphComparers[i]!.Compare(x, y);
                        // Got diff
                        if (d != 0) return d;
                    }
                    // Equals
                    return 0;
                }
                finally
                {
                    // Revert thread local
                    IGraphComparer.Context2.Value = prevContext;
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
                    // Get diff
                    int d = nonGraphComparers[i]!.Compare(x, y);
                    // Got diff
                    if (d != 0) return d;
                }
            }
            // Equals
            return 0;
        }
        // Cannot discern
        else return 0;
    }
}

/// <summary>Comparer that applies an array of comparers in order.</summary>
public class RecordComparer<T> : RecordComparer_<T>, IComparer<T>, IGraphComparer<T>
{
    /// <summary></summary>
    public RecordComparer() : base() { }

    /// <summary></summary>
    /// <param name="comparerEnumr">IEnumerable of <![CDATA[IComparer<T>]]></param>
    public RecordComparer(IEnumerable comparerEnumr) : base(comparerEnumr) { }

    /// <summary></summary>
    public int Compare(T? x, T? y)
    {
        // Same reference
        if (!typeof(T).IsValueType && object.ReferenceEquals(x, y)) return 0;
        //
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        // Got mixed comparers. Uses try-catch to store context for non-graph comparers.
        if (IsCyclical || graphComparerCount > 0)
        {
            // Get previous context
            IGraphComparerContext2? prevContext = IGraphComparer.Context2.Value;
            // Place here context
            IGraphComparerContext2 context = prevContext ?? setContext(new GraphComparerContext2())!;
            try
            {
                // Already compared
                if (!context.Add(x, y)) return 0;
                // Compare with each
                for (int i = 0; i < allComparers.Length; i++)
                {
                    // Get graph comparer
                    IGraphComparer<T>? gec = graphComparers[i];
                    // Get diff
                    int d = gec != null ? gec.Compare(x, y, context) : nonGraphComparers[i]!.Compare(x, y);
                    // Got diff
                    if (d != 0) return d;
                }
                // Equals
                return 0;
            }
            finally
            {
                // Revert thread local
                IGraphComparer.Context2.Value = prevContext;
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
                // Get diff
                int d = nonGraphComparers[i]!.Compare(x, y);
                // Got diff
                if (d != 0) return d;
            }
        }
        // Equals
        return 0;
    }

    /// <summary></summary>
    public int Compare(T? x, T? y, IGraphComparerContext2 context)
    {
        // Same reference
        if (object.ReferenceEquals(x, y)) return 0;
        // Check nulls
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        // Got mixed comparers. Uses try-catch to store context for non-graph comparers.
        if (IsCyclical || graphComparerCount > 0)
        {
            // Get previous context
            IGraphComparerContext2? prevContext = IGraphComparer.Context2.Value;
            // Assign thread local
            IGraphComparer.Context2.Value = context;
            try
            {
                // Already compared
                if (!context.Add(x, y)) return 0;
                // Compare with each
                for (int i = 0; i < allComparers.Length; i++)
                {
                    // Get graph comparer
                    IGraphComparer<T>? gec = graphComparers[i];
                    // Get diff
                    int d = gec != null ? gec.Compare(x, y, context) : nonGraphComparers[i]!.Compare(x, y);
                    // Got diff
                    if (d != 0) return d;
                }
                // Equals
                return 0;
            }
            finally
            {
                // Revert thread local
                IGraphComparer.Context2.Value = prevContext;
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
                // Get diff
                int d = nonGraphComparers[i]!.Compare(x, y);
                // Got diff
                if (d != 0) return d;
            }
        }
        // Equals
        return 0;
    }
}

/// <summary>Extension methods for <see cref="RecordComparer"/>.</summary>
public static class RecordComparerExtensions
{
    /// <summary>Set comparers</summary>
    public static T SetComparers<T>(this T recordComparer, IEnumerable comparerEnumr) where T : RecordComparer { recordComparer.Comparers = comparerEnumr; return recordComparer; }
    /// <summary>Set comparers</summary>
    public static T SetComparers<T>(this T recordComparer, params object[] comparerEnumr) where T : RecordComparer { recordComparer.Comparers = comparerEnumr; return recordComparer; }
}

