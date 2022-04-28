// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider.Internal;
using System.Diagnostics;
using System.Runtime.CompilerServices;

/// <summary>Adapts <see cref="Func{T, TResult}"/> to <see cref="IProvider{Key, Value}"/>.</summary>
public abstract class FuncProvider : IProvider
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
public class FuncProvider<TKey, TValue> :
    FuncProvider,
    IProvider<TKey, TValue>
{
    /// <summary></summary>
    public override Type Key => typeof(TKey);
    /// <summary></summary>
    public override Type Value => typeof(TValue);

    /// <summary>Key to value indexer</summary>
    [DebuggerHidden]
    public TValue this[TKey key] => func(key);

    /// <summary>Function</summary>
    protected Func<TKey, TValue> func;

    /// <summary>Create cache</summary>
    /// <param name="func">Function that creates value.</param>
    /// <remarks>Note that <paramref name="func"/> may be invoked multiple times in concurrent scenario and value let go</remarks>
    public FuncProvider(Func<TKey, TValue> func)
    {
        this.func = func ?? throw new ArgumentNullException(nameof(func));
    }

    /// <summary>Try get value</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public bool TryGetValue(TKey key, out TValue value)
    {
        value = func(key);
        return true;
    }

    /// <summary>Try get value</summary>
    /// <exception cref="InvalidCastException">If key is not <typeparamref name="TKey"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public override bool TryGetValue(object key, out object value)
    {
        value = (object)func((TKey)key)!;
        return true;
    }
}
