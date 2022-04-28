// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

/// <summary></summary>
public class GraphComparerContext : HashSet<object>, IGraphComparerContext
{
    /// <summary>Maps for value types</summary>
    protected Dictionary<Type, object>? valueTypeMaps = null;
    /// <summary></summary>
    public GraphComparerContext() : base(ReferenceEqualityComparer<object>.Instance) { }

    /// <summary></summary>
    public bool Add<T>(in T x)
    {
        // Got value type
        if (typeof(T).IsValueType) return GetValueTypeMap<T>().Add(x);
        // Got reference type
        else return base.Add((object)x!);
    }

    /// <summary></summary>
    public bool Contains<T>(in T x)
    {
        // Got value type
        if (typeof(T).IsValueType) return GetValueTypeMap<T>().Contains(x);
        // Got reference type
        else return base.Contains((object)x!);
    }

    /// <summary>Get-or-create map for value types</summary>
    HashSet<T> GetValueTypeMap<T>()
    {
        // Get or create set
        lock (this)
        {
            // Create value type maps
            if (valueTypeMaps == null) valueTypeMaps = new();
            // Try-get
            else if (valueTypeMaps.TryGetValue(typeof(T), out object? _set) && _set is HashSet<T> __set) return __set;
            // Place set here
            HashSet<T> set = new HashSet<T>();
            // Create new set
            valueTypeMaps[typeof(T)] = set;
            //
            return set;
        }
    }
}

/// <summary></summary>
public class GraphComparerContext2 : HashSet<PairMutable<object, object>>, IGraphComparerContext2
{
    /// <summary></summary>
    static PairMutable<object, object>.EqualityComparer comparer = new PairMutable<object, object>.EqualityComparer(ReferenceEqualityComparer<object>.Instance, ReferenceEqualityComparer<object>.Instance);
    /// <summary></summary>
    protected Dictionary<Type, object>? valueTypeMaps = null;

    /// <summary></summary>
    public GraphComparerContext2() : base(comparer) { }

    /// <summary></summary>
    public bool Add<T>(in T x, in T y)
    {
        // Got value type
        if (typeof(T).IsValueType) return GetValueTypeMap<T>().Add(new PairMutable<T, T>(x, y));
        // Got reference type
        else return base.Add(new PairMutable<object, object>((object)x!, (object)y!));
    }

    /// <summary></summary>
    public bool Contains<T>(in T x, in T y)
    {
        // Got value type
        if (typeof(T).IsValueType) return GetValueTypeMap<T>().Contains(new PairMutable<T, T>(x, y));
        // Got reference type
        else return base.Contains(new PairMutable<object, object>((object)x!, (object)y!));
    }

    /// <summary>Get-or-create map for value types</summary>
    HashSet<PairMutable<T, T>> GetValueTypeMap<T>()
    {
        // Get or create set
        lock (this)
        {
            // Create value type maps
            if (valueTypeMaps == null) valueTypeMaps = new();
            // Try-get
            else if (valueTypeMaps.TryGetValue(typeof(T), out object? _set) && _set is HashSet<PairMutable<T, T>> __set) return __set;
            // Place set here
            HashSet<PairMutable<T, T>> set = new HashSet<PairMutable<T, T>>();
            // Create new set
            valueTypeMaps[typeof(T)] = set;
            //
            return set;
        }
    }
}
