// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

/// <summary>Order comparer between <see cref="KeyValuePair{TKey, TValue}"/>.</summary>
/// <typeparam name="Key"></typeparam>
/// <typeparam name="Value"></typeparam>
public class KeyValuePairComparer<Key, Value> : IComparer<KeyValuePair<Key, Value>>
{
    /// <summary>Singleton</summary>
    static KeyValuePairComparer<Key, Value> instance = new(Comparer<Key>.Default, Comparer<Value>.Default);
    /// <summary>Singleton</summary>
    public static KeyValuePairComparer<Key, Value> Instance => instance;

    /// <summary>Comparer for key.</summary>
    public readonly IComparer<Key> keyComparer;
    /// <summary>Comparer for value.</summary>
    public readonly IComparer<Value> valueComparer;

    /// <summary>Create comparer.</summary>
    /// <param name="keyComparer"></param>
    /// <param name="valueComparer"></param>
    public KeyValuePairComparer(IComparer<Key> keyComparer, IComparer<Value> valueComparer)
    {
        this.keyComparer = keyComparer ?? throw new ArgumentNullException(nameof(keyComparer));
        this.valueComparer = valueComparer ?? throw new ArgumentNullException(nameof(valueComparer));
    }

    /// <summary>Compare two pairs.</summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int Compare(KeyValuePair<Key, Value> x, KeyValuePair<Key, Value> y)
    {
        int compare = 0;
        compare = keyComparer.Compare(x.Key, y.Key);
        if (compare != 0) return compare;
        compare = valueComparer.Compare(x.Value, y.Value);
        if (compare != 0) return compare;
        return 0;
    }
}

/// <summary>Equality comparer between <see cref="KeyValuePair{Key, Value}"/>.</summary>
/// <typeparam name="Key"></typeparam>
/// <typeparam name="Value"></typeparam>
public class KeyValuePairEqualityComparer<Key, Value> : IEqualityComparer<KeyValuePair<Key, Value>>
{
    /// <summary>Singleton</summary>
    static KeyValuePairEqualityComparer<Key, Value> instance = new(EqualityComparer<Key>.Default, EqualityComparer<Value>.Default);
    /// <summary>Singleton</summary>
    public static KeyValuePairEqualityComparer<Key, Value> Instance => instance;

    /// <summary>Comparer for key.</summary>
    public readonly IEqualityComparer<Key> keyComparer;
    /// <summary>Comparer for value.</summary>
    public readonly IEqualityComparer<Value> valueComparer;

    /// <summary>Create comparer.</summary>
    /// <param name="keyComparer"></param>
    /// <param name="valueComparer"></param>
    public KeyValuePairEqualityComparer(IEqualityComparer<Key> keyComparer, IEqualityComparer<Value> valueComparer)
    {
        this.keyComparer = keyComparer ?? throw new ArgumentNullException(nameof(keyComparer));
        this.valueComparer = valueComparer ?? throw new ArgumentNullException(nameof(valueComparer));
    }

    /// <summary>Compare two parts for equality.</summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Equals(KeyValuePair<Key, Value> x, KeyValuePair<Key, Value> y)
    {
        if (!keyComparer.Equals(x.Key, y.Key)) return false;
        if (!valueComparer.Equals(x.Value, y.Value)) return false;
        return true;
    }

    /// <summary>Calculate hashcode for <paramref name="obj"/>.</summary>
    public int GetHashCode(KeyValuePair<Key, Value> obj)
        => (obj.Key == null ? 0 : 11 * obj.Key.GetHashCode()) + (obj.Value == null ? 0 : 13 * obj.Value.GetHashCode());
}
