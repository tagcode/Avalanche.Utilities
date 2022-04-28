// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

/// <summary>Dictionary that can be set read-only at later point</summary>
public abstract class LockableDictionary : IReadOnly, IEnumerable, ICollection
{
    /// <summary></summary>
    static readonly ConstructorT2<IEnumerable, LockableDictionary> constructor = new(typeof(LockableDictionary<,>._));
    /// <summary></summary>
    public static LockableDictionary Create(Type keyType, Type valueType, IEnumerable? internalDictionary = null) => constructor.Create(keyType, valueType, internalDictionary!);

    /// <summary>Is read-only state</summary>
    protected bool @readonly;
    /// <summary>Is read-only state</summary>
    [IgnoreDataMember] bool IReadOnly.ReadOnly { get => @readonly; set { if (@readonly == value) return; if (!value) throw new InvalidOperationException("Read-only"); @readonly = value; } }

    /// <summary></summary>
    protected bool isSynchronized;
    /// <summary></summary>
    protected object syncRoot = null!;

    /// <summary></summary>
    public IEnumerator GetEnumerator() => getEnumerator();
    /// <summary></summary>
    protected abstract IEnumerator getEnumerator();
    /// <summary></summary>
    public abstract void CopyTo(Array array, int index);
    /// <summary></summary>
    public abstract int Count { get; }
    /// <summary></summary>
    public virtual bool IsSynchronized { get; }
    /// <summary></summary>
    public virtual object SyncRoot => syncRoot;
}

/// <summary>Dictionary that can be set read-only at later point</summary>
public class LockableDictionary<Key, Value> : LockableDictionary, IDictionary<Key, Value> where Key : notnull
{
    /// <summary></summary>
    protected IDictionary<Key, Value> map;
    /// <summary></summary>
    protected ReadOnlyDictionary<Key, Value>? readOnlyDictionary;
    /// <summary></summary>
    protected ReadOnlyDictionary<Key, Value> ReadOnlyMap => readOnlyDictionary ??= new ReadOnlyDictionary<Key, Value>(map);
    /// <summary>Writable or readonly dictionary, depending on state</summary>
    public IDictionary<Key, Value> Dictionary => (@readonly ? ReadOnlyMap : map);

    /// <summary>Assert writable</summary>
    LockableDictionary<Key, Value> AssertWritable => @readonly ? throw new InvalidOperationException("Read-only") : this;

    /// <summary></summary>
    public LockableDictionary() 
    { 
        this.map = new Dictionary<Key, Value>();
        this.syncRoot = (this.map as ICollection)?.SyncRoot ?? new object();
        this.isSynchronized = this.map is ICollection collection ? collection.IsSynchronized : false;
    }

    /// <summary></summary>
    public LockableDictionary(IDictionary<Key, Value>? internalDictionary) 
    { 
        this.map = internalDictionary ?? new Dictionary<Key, Value>();
     
        if (TypeUtilities.IsDefinableTo(map.GetType(), typeof(ConcurrentDictionary<,>)))
        {
            this.isSynchronized = true;
            this.syncRoot = new object();
        }
        else if (internalDictionary is ICollection collection)
        {
            this.syncRoot = collection.SyncRoot;
            this.isSynchronized = collection.IsSynchronized;
        } else
        {
            this.syncRoot = new object();
            this.isSynchronized = false;
        }
    }

    /// <summary></summary>
    public ICollection<Key> Keys => Dictionary.Keys;
    /// <summary></summary>
    public ICollection<Value> Values => Dictionary.Values;

    /// <summary></summary>
    public override int Count => map.Count;
    /// <summary></summary>
    public bool IsReadOnly => this.@readonly;
    /// <summary></summary>
    public Value this[Key key] { get => map[key]; set => AssertWritable.map[key] = value; }
    /// <summary></summary>
    public void Add(Key key, Value value) => AssertWritable.map.Add(key, value);
    /// <summary></summary>
    public bool ContainsKey(Key key) => map.ContainsKey(key);
    /// <summary></summary>
    public bool Remove(Key key) => AssertWritable.map.Remove(key);
    /// <summary></summary>
    public bool TryGetValue(Key key, [MaybeNullWhen(false)] out Value value) => map.TryGetValue(key, out value);
    /// <summary></summary>
    public void Add(KeyValuePair<Key, Value> item) => AssertWritable.map.Add(item);
    /// <summary></summary>
    public void Clear() => AssertWritable.map.Clear();
    /// <summary></summary>
    public bool Contains(KeyValuePair<Key, Value> item) => map.Contains(item);
    /// <summary></summary>
    public void CopyTo(KeyValuePair<Key, Value>[] array, int index) => map.CopyTo(array, index);
    /// <summary></summary>
    public override void CopyTo(Array array, int index) => map.CopyTo((KeyValuePair<Key, Value>[])array, index);
    /// <summary></summary>
    public bool Remove(KeyValuePair<Key, Value> item) => AssertWritable.map.Remove(item);
    /// <summary></summary>
    public new IEnumerator<KeyValuePair<Key, Value>> GetEnumerator() => map.GetEnumerator();
    /// <summary></summary>
    protected override IEnumerator getEnumerator() => ((IEnumerable)map).GetEnumerator();

    /// <summary>Workaround</summary>
    public class _ : LockableDictionary<Key, Value>
    {
        /// <summary></summary>
        public _(IEnumerable? internalDictionary) :
            base(internalDictionary != null ? (IDictionary<Key, Value>)internalDictionary : new Dictionary<Key, Value>())
        {
        }
    }

}

