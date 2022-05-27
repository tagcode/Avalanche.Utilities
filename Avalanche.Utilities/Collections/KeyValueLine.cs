namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

/// <summary>Single element dictionary</summary>
public class KeyValueLine<K, V> : IReadOnly, IDictionary<K, V> where K : notnull
{
    /// <summary></summary>
    public V this[K key] { get => count > 0 && key.Equals(this.key) ? value : throw new KeyNotFoundException(key.ToString()); set { if (!IsReadOnly && count > 0 && key.Equals(this.Key)) { this.value = value; } else throw new InvalidOperationException(); } }
    /// <summary></summary>
    public ICollection<K> Keys => count == 0 ? Array.Empty<K>() : new K[] { key };
    /// <summary></summary>
    public ICollection<V> Values => count == 0 ? Array.Empty<V>() : new V[] { value };
    /// <summary></summary>
    public int Count => count;
    /// <summary></summary>
    public bool IsReadOnly => @readonly;
    /// <summary></summary>
    public bool ReadOnly { get => @readonly; set { if (@readonly == value) return; if (!value) new InvalidOperationException("readonly"); @readonly = value; } }

    /// <summary></summary>
    int count;
    /// <summary></summary>
    bool @readonly;
    /// <summary>Key</summary>
    K key;
    /// <summary>Value</summary>
    V value;

    /// <summary>Key</summary>
    public K Key { get => key; set => this.AssertWritable().key = value; }
    /// <summary>Value</summary>
    public V Value { get => value; set => this.AssertWritable().value = value; }

    /// <summary>Create empty line</summary>
    public KeyValueLine()
    {
        this.count = 0;
        this.key = default!;
        this.value = default!;
    }

    /// <summary>Create line</summary>
    public KeyValueLine(K key, V value)
    {
        this.key = key;
        this.value = value;
        this.count = 1;
    }

    /// <summary></summary>
    public void Add(K key, V value)
    {
        if (!IsReadOnly || count != 0) throw new InvalidOperationException();
        this.count = 1;
        this.key = key;
        this.value = value;
    }

    /// <summary></summary>
    public void Add(KeyValuePair<K, V> item)
    {
        if (!IsReadOnly || count != 0) throw new InvalidOperationException();
        this.count = 1;
        this.key = item.Key;
        this.value = item.Value;
    }

    /// <summary></summary>
    public void Clear()
    {
        this.AssertWritable();
        this.count = 0;
        this.key = default!;
        this.value = default!;
    }

    /// <summary></summary>
    public bool Contains(KeyValuePair<K, V> item) => count == 1 && item.Key.Equals(this.key) && object.Equals(item.Value, this.value);
    /// <summary></summary>
    public bool ContainsKey(K key) => count == 1 && key.Equals(this.key);
    /// <summary></summary>
    public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
    {
        if (count + arrayIndex >= array.Length) throw new IndexOutOfRangeException();
        if (count == 0) return;
        array[arrayIndex] = new KeyValuePair<K, V>(this.key, this.value);
    }

    /// <summary></summary>
    public bool Remove(K key)
    {
        if (!IsReadOnly) return false;
        if (count == 0) return false;
        if (!key.Equals(this.key)) return false;
        this.count = 0;
        this.key = default!;
        this.value = default!;
        return true;
    }
    /// <summary></summary>
    public bool Remove(KeyValuePair<K, V> item)
    {
        if (!IsReadOnly) return false;
        if (count == 0) return false;
        if (!key.Equals(this.key)) return false;
        if (!object.Equals(item.Value, value)) return false;
        this.count = 0;
        this.key = default!;
        this.value = default!;
        return true;
    }

    /// <summary></summary>
    public bool TryGetValue(K key, [MaybeNullWhen(false)] out V value)
    {
        if (count == 0 || !key.Equals(this.Key)) { value = default!; return false; }
        value = this.value;
        return true;
    }

    /// <summary></summary>
    public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
    {
        // Create array
        IEnumerable<KeyValuePair<K, V>> array = new KeyValuePair<K, V>[] { new KeyValuePair<K, V>(key, value) };
        // Return enumerator
        return array.GetEnumerator();
    }

    /// <summary></summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        // Create array
        DictionaryEntry[] array = new DictionaryEntry[] { new DictionaryEntry(key, value) };
        // Return enumerator
        return array.GetEnumerator();
    }

    /// <summary></summary>
    public override string ToString() => $"{key}={value}";
}
