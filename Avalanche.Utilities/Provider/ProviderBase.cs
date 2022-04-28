// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider;                     

/// <summary>Base class for provider.</summary>
public abstract class ProviderBase : IProvider
{
    /// <summary></summary>
    public abstract Type Key { get; }
    /// <summary></summary>
    public abstract Type Value { get; }
    /// <summary>Get value that corresponds <paramref name="key"/></summary>
    /// <exception cref="InvalidCastException">If key is wrong type.</exception>
    public abstract bool TryGetValue(object key, out object value);
    /// <summary>Print information</summary>
    public override string ToString() => CanonicalName.Print(GetType());
}

/// <summary>Adapts <see cref="Func{T, TResult}"/> to <see cref="IProvider{Key, Value}"/>.</summary>
public abstract class ProviderBase<TKey, TValue> :
    ProviderBase,
    IProvider<TKey, TValue>
{
    /// <summary></summary>
    public override Type Key => typeof(TKey);
    /// <summary></summary>
    public override Type Value => typeof(TValue);

    /// <summary>Key to value indexer</summary>
    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out TValue value)) return value;
            else throw new KeyNotFoundException(key?.ToString() ?? "");
        }
    }

    /// <summary>Try get value</summary>
    public abstract bool TryGetValue(TKey key, out TValue value);

    /// <summary>Try get value</summary>
    /// <exception cref="InvalidCastException">If key is not <typeparamref name="TKey"/>.</exception>
    public override bool TryGetValue(object keyObject, out object value)
    {
        // Cast key
        if (keyObject is not TKey keyT) { value = null!; return false; }
        // Forward
        if (!TryGetValue(keyT, out TValue valueT)) { value = null!; return false; }
        //
        value = valueT!;
        return true;
    }
}
