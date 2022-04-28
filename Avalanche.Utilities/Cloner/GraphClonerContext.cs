// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections.Generic;

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
/// <summary></summary>
public class GraphClonerContext : Dictionary<object, object>, IGraphClonerContext
{
    /// <summary>Maps for value types</summary>
    protected Dictionary<Type, object>? valueTypeMaps = null;

    /// <summary></summary>
    public GraphClonerContext() : base(ReferenceEqualityComparer.Instance) { }

    /// <summary>Associate <paramref name="clone"/> as clone of <paramref name="src"/>.</summary>
    /// <returns>true if was added, false if has already been added.</returns>
    public bool Add<T>(in T src, in T clone)
    {
        // Value type
        if (typeof(T).IsValueType)
        {
            //
            Dictionary<T, T> map = GetValueTypeMap<T>();
            // Associate
            bool ok = map.TryAdd(src, clone);
            // Return 
            return ok;
        }
        // Rreference type
        else
        {
            // Null is implicitely associated
            if (src == null) return true;
            // Associate
            bool ok = base.TryAdd((object)src!, (object)clone!);
            // Return 
            return ok;
        }
    }

    /// <summary>Test whether <paramref name="src"/> has been marked cloned.</summary>
    /// <returns>true if <paramref name="src"/> is cloned</returns>
    public bool Contains<T>(in T src)
    {
        // Value type
        if (typeof(T).IsValueType)
        {
            //
            Dictionary<T, T> map = GetValueTypeMap<T>();
            // 
            bool contains = map.ContainsKey(src);
            // Return 
            return contains;
        }
        // Reference type
        else
        {
            // Null is implicitely associated
            if (src == null) return true;
            // Contains
            bool contains = base.ContainsKey((object)src);
            // Return 
            return contains;
        }
    }

    // vvv XXX TODO: Error might be called with T=object and src=value type. Goes to wrong map. 

    /// <summary>Get cloned counterpart of <paramref name="src"/>.</summary>
    /// <returns>true if <paramref name="clone"/> existed.</returns>
    public bool TryGet<T>(in T src, out T clone)
    {
        // Value type
        if (typeof(T).IsValueType)
        {
            //
            Dictionary<T, T> map = GetValueTypeMap<T>();
            // 
            bool ok = map.TryGetValue(src, out clone!);
            // Return 
            return ok;
        }
        // Rreference type
        else
        {
            // Null is implicitely associated
            if (src == null) { clone = default!; return false; }
            // Contains
            bool ok = base.TryGetValue(src, out object? _clone);
            // Assign 
            clone = ok ? (T)_clone! : default!;
            // Return
            return ok;
        }
    }

    /// <summary>Get-or-create map for value types</summary>
    Dictionary<T, T> GetValueTypeMap<T>()
    {
        // Get or create set
        lock (this)
        {
            // Create value type maps
            if (valueTypeMaps == null) valueTypeMaps = new();
            // Try-get
            else if (valueTypeMaps.TryGetValue(typeof(T), out object? _set) && _set is Dictionary<T, T> __set) return __set;
            // Place set here
            Dictionary<T, T> set = new Dictionary<T, T>();
            // Create new set
            valueTypeMaps[typeof(T)] = set;
            //
            return set;
        }
    }

}
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
