// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

/// <summary>Dictionary that can be set read-only at later point</summary>
public record LockableDictionaryRecord<Key, Value> : ReadOnlyAssignableRecord, IDictionary<Key, Value> where Key : notnull
{
    /// <summary></summary>
    protected IDictionary<Key, Value> dictionary;
    /// <summary></summary>
    protected ReadOnlyDictionary<Key, Value>? readOnlyDictionary;
    /// <summary></summary>
    protected ReadOnlyDictionary<Key, Value> ReadOnlyMap => readOnlyDictionary ?? (readOnlyDictionary = new ReadOnlyDictionary<Key, Value>(dictionary));
    /// <summary>Writable or readonly dictionary, depending on state</summary>
    public IDictionary<Key, Value> Dictionary => (@readonly ? ReadOnlyMap : dictionary);

    /// <summary>Assert writable</summary>
    LockableDictionaryRecord<Key, Value> AssertWritable => @readonly ? throw new InvalidOperationException("Read-only") : this;

    /// <summary></summary>
    public LockableDictionaryRecord()
    {
        this.dictionary = new Dictionary<Key, Value>();
    }

    /// <summary></summary>
    public LockableDictionaryRecord(IDictionary<Key, Value> map)
    {
        this.dictionary = map;
    }

    /// <summary></summary>
    [IgnoreDataMember]
    public ICollection<Key> Keys => Dictionary.Keys;
    /// <summary></summary>
    [IgnoreDataMember]
    public ICollection<Value> Values => Dictionary.Values;

    /// <summary></summary>
    [IgnoreDataMember]
    public int Count => dictionary.Count;
    /// <summary></summary>
    [IgnoreDataMember]
    public bool IsReadOnly => this.@readonly;
    /// <summary></summary>
    [IgnoreDataMember]
    public Value this[Key key] { get => dictionary[key]; set => AssertWritable.dictionary[key] = value; }
    /// <summary></summary>
    public void Add(Key key, Value value) => AssertWritable.dictionary.Add(key, value);
    /// <summary></summary>
    public bool ContainsKey(Key key) => dictionary.ContainsKey(key);
    /// <summary></summary>
    public bool Remove(Key key) => AssertWritable.dictionary.Remove(key);
    /// <summary></summary>
    public bool TryGetValue(Key key, [MaybeNullWhen(false)] out Value value) => dictionary.TryGetValue(key, out value);
    /// <summary></summary>
    public void Add(KeyValuePair<Key, Value> item) => AssertWritable.dictionary.Add(item);
    /// <summary></summary>
    public void Clear() => AssertWritable.dictionary.Clear();
    /// <summary></summary>
    public bool Contains(KeyValuePair<Key, Value> item) => dictionary.Contains(item);
    /// <summary></summary>
    public void CopyTo(KeyValuePair<Key, Value>[] array, int arrayIndex) => dictionary.CopyTo(array, arrayIndex);
    /// <summary></summary>
    public bool Remove(KeyValuePair<Key, Value> item) => AssertWritable.dictionary.Remove(item);
    /// <summary></summary>
    public IEnumerator<KeyValuePair<Key, Value>> GetEnumerator() => dictionary.GetEnumerator();
    /// <summary></summary>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)dictionary).GetEnumerator();
}
