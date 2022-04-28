// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections.Generic;

/// <summary></summary>
public static class MapList
{
    /// <summary></summary>
    static readonly ConstructorT2<LockableDictionary> constructor = new(typeof(MapList<,>));
    /// <summary></summary>
    static readonly ConstructorT2<object, LockableDictionary> constructor2 = new(typeof(MapList<,>._));
    /// <summary></summary>
    public static LockableDictionary Create(Type keyType, Type valueType) => constructor.Create(keyType, valueType);
    /// <summary></summary>
    public static LockableDictionary Create(Type keyType, Type valueType, object keyComparer) => constructor2.Create(keyType, valueType, keyComparer);
}

/// <summary>MapList is a dictionary that has multiple values per key.</summary>
/// <typeparam name="Key"></typeparam>
/// <typeparam name="Value"></typeparam>
public class MapList<Key, Value> : LockableDictionary<Key, List<Value>>, IEnumerable<KeyValuePair<Key, Value>>, IReadOnly where Key : notnull
{
    /// <summary>Create map list.</summary>
    public MapList() : base() 
    {
        // Cannot combine synchronization of internal dictionary and List<T>.
        isSynchronized = false;
        // Assrt sync root
        if (this.syncRoot == null) this.syncRoot = new object();
    }

    /// <summary>Create map list with custom <paramref name="comparer"/>.</summary>
    public MapList(IEqualityComparer<Key> comparer) : base(new Dictionary<Key, List<Value>>(comparer)) 
    {
        // Cannot combine synchronization of internal dictionary and List<T>.
        isSynchronized = false;
        // Assrt sync root
        if (this.syncRoot == null) this.syncRoot = new object();
    }

    /// <summary>Create map list with <paramref name="initialValues"/>.</summary>
    public MapList(IEnumerable<KeyValuePair<Key, Value>> initialValues) : base() 
    {
        // Add elements
        AddRange(initialValues);
        // Cannot combine synchronization of internal dictionary and List<T>.
        isSynchronized = false;
        // Assrt sync root
        if (this.syncRoot == null) this.syncRoot = new object();
    }

    /// <summary>Create map list with <paramref name="initialValues"/>.</summary>
    public MapList(IEnumerable<KeyValuePair<Key, List<Value>>> initialValues) : base() 
    { 
        // Add elements
        AddRange(initialValues);
        // Cannot combine synchronization of internal dictionary and List<T>.
        isSynchronized = false;
        // Assrt sync root
        if (this.syncRoot == null) this.syncRoot = new object();
    }

    /// <summary>Add <paramref name="value"/> to <paramref name="key"/>.</summary>
    public MapList<Key, Value> Add(Key key, Value value)
    {
        this.AssertWritable();
        if (!TryGetValue(key, out List<Value>? list)) this[key] = list = new List<Value>(1);
        list.Add(value);
        return this;
    }

    /// <summary>Remove <paramref name="value"/> at <paramref name="key"/>.</summary>
    public MapList<Key, Value> Remove(Key key, Value value)
    {
        this.AssertWritable();
        if (TryGetValue(key, out List<Value>? list))
        {
            list.Remove(value);
            if (list.Count == 0) Remove(key);
        }
        return this;
    }

    /// <summary>Get values at <paramref name="key"/>.</summary>
    public IEnumerable<Value> GetEnumerable(Key key)
    {
        if (TryGetValue(key, out List<Value>? values)) return values!;
        return Array.Empty<Value>();
    }

    /// <summary>Try get internal values list.</summary>
    public List<Value>? TryGetList(Key key)
    {
        if (TryGetValue(key, out List<Value>? list)) return list;
        return null;
    }

    /// <summary>Test if contains <paramref name="value"/> at <paramref name="key"/>.</summary>
    public bool Contains(Key key, Value value)
        => TryGetValue(key, out List<Value>? values) && values.Contains(value);

    /// <summary>Get internal value list for <paramref name="key"/>.</summary>
    public List<Value> GetList(Key key)
        => this[key];

    /// <summary>Get-or-create value list for <paramref name="key"/>.</summary>
    public List<Value> GetOrCreateList(Key key)
    {
        if (!TryGetValue(key, out List<Value>? list)) this[key] = list = new List<Value>(1);
        return list;
    }

    /// <summary>Add <paramref name="items"/></summary>
    public MapList<Key, Value> AddRange(IEnumerable<KeyValuePair<Key, List<Value>>> items)
    {
        this.AssertWritable();
        foreach (var pair in items)
        {
            List<Value> values = GetOrCreateList(pair.Key);
            values.AddRange(pair.Value);
        }
        return this;
    }

    /// <summary>Add <paramref name="values"/></summary>
    public MapList<Key, Value> AddRange(IEnumerable<KeyValuePair<Key, Value>> values)
    {
        this.AssertWritable();
        foreach (var pair in values)
            GetOrCreateList(pair.Key).Add(pair.Value);
        return this;
    }

    /// <summary>Enumerate all lines</summary>
    IEnumerator<KeyValuePair<Key, Value>> IEnumerable<KeyValuePair<Key, Value>>.GetEnumerator()
    {
        foreach (var line in this)
            foreach (var item in line.Value)
                yield return new KeyValuePair<Key, Value>(line.Key, item);
    }

    /// <summary>Get all values</summary>
    public IEnumerable<Value> AllValues()
    {
        foreach (var line in this)
            foreach (var _value in line.Value)
                yield return _value;
    }

    /// <summary>Workaround</summary>
    public new class _ : MapList<Key, Value>
    {
        /// <summary></summary>
        public _(object keyComparer) : base((IEqualityComparer<Key>)keyComparer) { }
    }

}

/// <summary>Other map list related extension methods</summary>
public static class MapListExtensions
{
    /// <summary>Convert <paramref name="enumr"/> to maplist.</summary>
    public static MapList<Key, Value> ToMapList<Key, Value>(this IEnumerable<KeyValuePair<Key, Value>> enumr) where Key : notnull
        => new MapList<Key, Value>(enumr);

    /// <summary>Convert <paramref name="enumr"/> to maplist.</summary>
    public static MapList<Key, Value> ToMapList<Key, Value>(this IEnumerable<KeyValuePair<Key, List<Value>>> enumr) where Key : notnull
        => new MapList<Key, Value>(enumr);
}
