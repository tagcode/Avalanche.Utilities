// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Avalanche.Utilities.Provider.Internal;
using Avalanche.Utilities.Provider;

/// <summary>Extension methods for <see cref="IProvider"/>.</summary>
public static class ProviderExtensions_
{
    /// <summary>Decorate <paramref name="provider"/> to apply <paramref name="valueFunc"/> on result value.</summary>
    public static IProvider<Key, DstValue> ValueFunc<Key, SrcValue, DstValue>(this IProvider<Key, SrcValue> provider, Func<SrcValue, DstValue> valueFunc) => new ValueFuncProvider<Key, SrcValue, DstValue>(provider, valueFunc);

    /// <summary>Decorates <paramref name="provider"/> to lock value if it implements <see cref="IReadOnly"/>.</summary>
    /// <returns>Decoration that locks results</returns>
    public static IProvider AsReadOnly(this IProvider provider) => LockerProvider.Create(provider);
    /// <summary>Decorates <paramref name="provider"/> to lock value if it implements <see cref="IReadOnly"/>.</summary>
    /// <returns>Decoration that locks results</returns>
    public static IProvider<Key, Value> AsReadOnly<Key, Value>(this IProvider<Key, Value> provider) where Key : notnull
        => TypeUtilities.TryGetTypeArgumentOfCorrespondingDefinedType(typeof(Value), typeof(IResult<>), 0, out Type resultValueType) ?
            (IProvider<Key, Value>)LockerProvider.Create(typeof(Key), typeof(Value), provider) :
            new LockerProvider<Key, Value>(provider);

    /// <summary>Supplies cache to <paramref name="provider"/>.</summary>
    /// <param name="toCacheCriteria">Evaluates whether key is to be cached or not.</param>
    /// <returns>Provider supplied with <see cref="ConcurrentDictionary{TKey, TValue}"/> cache</returns>
    /// <remarks>The caller can check with <see cref="ProviderExtensions.IsCache"/> if <paramref name="provider"/> is already cached.</remarks>
    public static ConcurrentDictionaryCacheProvider<Key, Value> Cached<Key, Value>(this IProvider<Key, Value> provider, Func<Key, bool>? toCacheCriteria = default) where Key : notnull 
        => new ConcurrentDictionaryCacheProvider<Key, Value>(provider, toCacheCriteria);

    /// <summary>Supplies cache to <paramref name="provider"/>.</summary>
    /// <param name="toCacheCriteria">Evaluates whether key is to be cached or not.</param>
    /// <returns>Provider supplied with <see cref="ConcurrentDictionary{TKey, TValue}"/> cache</returns>
    /// <remarks>The caller can check with <see cref="ProviderExtensions.IsCache"/> if <paramref name="provider"/> is already cached.</remarks>
    public static ConcurrentDictionaryCacheProvider<Key, Value> Cached<Key, Value>(this IProvider<Key, Value> provider, IEqualityComparer<Key> comparer, Func<Key, bool>? toCacheCriteria = default) where Key : notnull => new ConcurrentDictionaryCacheProvider<Key, Value>(provider, comparer, toCacheCriteria);

    /// <summary>Supplies cache to <paramref name="provider"/>.</summary>
    /// <param name="toCacheCriteria">Evaluates whether key is to be cached or not.</param>
    /// <returns>Provider supplied with <see cref="ConcurrentDictionary{TKey, TValue}"/> cache</returns>
    /// <remarks>The caller can check with <see cref="ProviderExtensions.IsCache"/> if <paramref name="provider"/> is already cached.</remarks>
    public static ConcurrentDictionaryCacheProvider<Key, Value> Cached<Key, Value>(this IProvider<Key, Value> provider, ConcurrentDictionary<Key, Value> cacheMap, Func<Key, bool>? toCacheCriteria = default) where Key : notnull => new ConcurrentDictionaryCacheProvider<Key, Value>(provider, cacheMap, toCacheCriteria);

    /// <summary>Supplies cache to <paramref name="provider"/>.</summary>
    /// <param name="toCacheCriteria">Evaluates whether key is to be cached or not.</param>
    /// <returns>Provider supplied with <see cref="ConcurrentDictionary{TKey, TValue}"/> cache</returns>
    /// <remarks>The caller can check with <see cref="ProviderExtensions.IsCache"/> if <paramref name="provider"/> is already cached.</remarks>
    public static ConcurrentDictionaryCacheProvider Cached(this IProvider provider, Delegate? toCacheCriteria = default) => ConcurrentDictionaryCacheProvider.Create(provider, toCacheCriteria);
    
    /// <summary>Supplies cache to <paramref name="provider"/>.</summary>
    /// <param name="toCacheCriteria">Evaluates whether key is to be cached or not.</param>
    /// <returns>Provider supplied with <see cref="ConcurrentDictionary{TKey, TValue}"/> cache</returns>
    /// <remarks>The caller can check with <see cref="ProviderExtensions.IsCache"/> if <paramref name="provider"/> is already cached.</remarks>
    public static ConcurrentDictionaryCacheProviderNullableKey<Key, Value> CachedNullableKey<Key, Value>(this IProvider<Key, Value> provider, Func<Key, bool>? toCacheCriteria = default) where Key : notnull 
        => new ConcurrentDictionaryCacheProviderNullableKey<Key, Value>(provider, toCacheCriteria);

    /// <summary>Supplies cache to <paramref name="provider"/>.</summary>
    /// <param name="toCacheCriteria">Evaluates whether key is to be cached or not.</param>
    /// <returns>Provider supplied with <see cref="ConcurrentDictionary{TKey, TValue}"/> cache</returns>
    /// <remarks>The caller can check with <see cref="ProviderExtensions.IsCache"/> if <paramref name="provider"/> is already cached.</remarks>
    public static ConcurrentDictionaryCacheProviderNullableKey<Key, Value> CachedNullableKey<Key, Value>(this IProvider<Key, Value> provider, IEqualityComparer<ValueTuple<Key>> comparer, Func<Key, bool>? toCacheCriteria = default) where Key : notnull => new ConcurrentDictionaryCacheProviderNullableKey<Key, Value>(provider, comparer, toCacheCriteria);

    /// <summary>Supplies cache to <paramref name="provider"/>.</summary>
    /// <param name="toCacheCriteria">Evaluates whether key is to be cached or not.</param>
    /// <returns>Provider supplied with <see cref="ConcurrentDictionary{TKey, TValue}"/> cache</returns>
    /// <remarks>The caller can check with <see cref="ProviderExtensions.IsCache"/> if <paramref name="provider"/> is already cached.</remarks>
    public static ConcurrentDictionaryCacheProviderNullableKey<Key, Value> CachedNullableKey<Key, Value>(this IProvider<Key, Value> provider, ConcurrentDictionary<ValueTuple<Key>, Value> cacheMap, Func<Key, bool>? toCacheCriteria = default) where Key : notnull => new ConcurrentDictionaryCacheProviderNullableKey<Key, Value>(provider, cacheMap, toCacheCriteria);

    /// <summary>Supplies cache to <paramref name="provider"/>.</summary>
    /// <param name="toCacheCriteria">Evaluates whether key is to be cached or not.</param>
    /// <returns>Provider supplied with <see cref="ConcurrentDictionary{TKey, TValue}"/> cache</returns>
    /// <remarks>The caller can check with <see cref="ProviderExtensions.IsCache"/> if <paramref name="provider"/> is already cached.</remarks>
    public static ConcurrentDictionaryCacheProviderNullableKey CachedNullableKey(this IProvider provider, Delegate? toCacheCriteria = default) => ConcurrentDictionaryCacheProviderNullableKey.Create(provider, toCacheCriteria);

    /// <summary>Supplies cache to <paramref name="provider"/>.</summary>
    /// <param name="toCacheCriteria">Evaluates whether key is to be cached or not.</param>
    /// <returns>Provider supplied with <see cref="ConditionalWeakTable{TKey, TValue}"/> cache</returns>
    /// <remarks>Uses <see cref="ConditionalWeakTable{Key,Value}"/> where keys are object reference compared, not hash-equals.</remarks>
    /// <remarks>The caller can check with <see cref="ProviderExtensions.IsCache"/> if <paramref name="provider"/> is already cached.</remarks>
    public static WeakTableCacheProvider<Key, Value> WeakCached<Key, Value>(this IProvider<Key, Value> provider, Func<Key, bool>? toCacheCriteria = default) where Key : class where Value : class? => new WeakTableCacheProvider<Key, Value>(provider, toCacheCriteria);

    /// <summary>Supplies cache to <paramref name="provider"/>.</summary>
    /// <param name="toCacheCriteria">Evaluates whether key is to be cached or not.</param>
    /// <returns>Provider supplied with <see cref="ConditionalWeakTable{TKey, TValue}"/> cache</returns>
    /// <remarks>Uses <see cref="ConditionalWeakTable{Key,Value}"/> where keys are object reference compared, not hash-equals.</remarks>
    /// <remarks>The caller can check with <see cref="ProviderExtensions.IsCache"/> if <paramref name="provider"/> is already cached.</remarks>
    public static WeakTableCacheProvider WeakCached(this IProvider provider, Delegate? toCacheCriteria = default) => WeakTableCacheProvider.Create(provider, toCacheCriteria);

    /// <summary>Decorates <paramref name="provider"/> to capture error state into <see cref="IResult{T}"/>. This is required for caching to store error states.</summary>
    public static IProvider ResultCaptured(this IProvider provider) => ResultCaptureProvider.Create(provider);
    /// <summary>Decorates <paramref name="provider"/> to capture error state into <see cref="IResult{T}"/>. This is required for caching to store error states.</summary>
    public static IProvider<Key, IResult<Value>> ResultCaptured<Key, Value>(this IProvider<Key, Value> provider) => new ResultCaptureProvider<Key, Value>(provider);
    /// <summary>Decorates <paramref name="provider"/> to capture error state into <see cref="IResult{T}"/>. This is required for caching to store error states.</summary>
    public static IProvider<Key, ValueResult<Value>> ValueResultCaptured<Key, Value>(this IProvider<Key, Value> provider) => new ValueResultCaptureProvider<Key, Value>(provider);

    /// <summary>Extracts captured value from <see cref="IResult{T}"/>.</summary>
    /// <exception cref="Exception">Captured exceptions are rethrown.</exception>
    public static IProvider ResultOpened(this IProvider provider) => ResultOpenedProvider.Create(provider);
    /// <summary>Extracts value from <see cref="IResult{T}"/> and returns <typeparamref name="Value"/>.</summary>
    /// <exception cref="Exception">Captured exceptions are rethrown.</exception>
    public static IProvider<Key, Value> ResultOpened<Key, Value>(this IProvider<Key, IResult<Value>> provider) => new ResultOpenedProvider<Key, Value>(provider);
    /// <summary>Extracts value from <see cref="IResult{T}"/> and returns <typeparamref name="Value"/>.</summary>
    /// <exception cref="Exception">Captured exceptions are rethrown.</exception>
    public static IProvider<Key, Value> ValueResultOpened<Key, Value>(this IProvider<Key, ValueResult<Value>> provider) => new ValueResultOpenedProvider<Key, Value>(provider);

    /// <summary></summary>
    public static IProvider<A, IResult<C>> Concat<A, B, C>(this IProvider<A, IResult<B>> providerAB, IProvider<B, IResult<C>> providerBC) => new ResultProviderConcat<A, B, C>(providerAB, providerBC);
    /// <summary></summary>
    public static IProvider<A, C> Concat<A, B, C>(this IProvider<A, B> providerAB, IProvider<B, C> providerBC) => new ProviderConcat<A, B, C>(providerAB, providerBC);
    /// <summary></summary>
    public static IProvider Concat(this IProvider providerAB, IProvider providerBC) =>
        TypeUtilities.TryGetTypeArgumentOfCorrespondingDefinedType(providerAB.Value, typeof(IResult<>), 0, out Type b) && TypeUtilities.TryGetTypeArgumentOfCorrespondingDefinedType(providerBC.Value, typeof(IResult<>), 0, out Type c) ?
        ResultProviderConcat.Constructor.Create(providerAB.Key, b, c, providerAB, providerBC) :
        ProviderConcat.Constructor.Create(providerAB.Key, providerBC.Key, providerBC.Value, providerAB, providerBC);
}
