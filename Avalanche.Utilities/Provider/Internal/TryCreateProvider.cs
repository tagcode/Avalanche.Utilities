// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider.Internal;
using System.Diagnostics;
using System.Runtime.CompilerServices;

/// <summary>Adapts <see cref="TryCreate{T, TResult}"/> to <see cref="IProvider{Key, Value}"/>.</summary>
public class TryCreateProvider<TKey, TValue> : ProviderBase<TKey, TValue>
{
    /// <summary></summary>
    protected Avalanche.Utilities.Provider.TryCreate<TKey, TValue> func;
    /// <summary></summary>
    public TryCreateProvider(Avalanche.Utilities.Provider.TryCreate<TKey, TValue> func) => this.func = func ?? throw new ArgumentNullException(nameof(func));
    /// <summary></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public override bool TryGetValue(TKey key, out TValue value) => func(key, out value!);
}

/// <summary>Adapts <![CDATA[TryCreate<T, TResult?>]]> to <see cref="IProvider{Key, Value}"/>.</summary>
/// <remarks>This version is used when try-get delegate returns nullable</remarks>
public class NullableTryCreateProvider<TKey, TValue> : ProviderBase<TKey, TValue> where TValue : struct
{
    /// <summary></summary>
    protected Avalanche.Utilities.Provider.TryCreate<TKey, TValue?> func;
    /// <summary></summary>
    public NullableTryCreateProvider(Avalanche.Utilities.Provider.TryCreate<TKey, TValue?> func) => this.func = func ?? throw new ArgumentNullException(nameof(func));
    /// <summary></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public override bool TryGetValue(TKey key, out TValue value)
    {
        if (!func(key, out Nullable<TValue> _value) || !_value.HasValue) { value = default!; return false; }
        value = _value!.Value;
        return true;
    }
}

