// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider;
using System.Collections.Concurrent;

/// <summary>Cache concurrent use.</summary>
/// <remarks>Uses <see cref="ConcurrentDictionary{Key,Value}"/></remarks>
public abstract class ConcurrentDictionaryCacheProviderNullableKey : IProvider, IProviderStack, ICache, IDecoration
{
    /// <summary>Constructor</summary>
    static readonly ConstructorT2<Delegate, Delegate?, ConcurrentDictionaryCacheProviderNullableKey> delegateCtor = new(typeof(ConcurrentDictionaryCacheProviderNullableKey<,>));
    /// <summary>Constructor</summary>
    static readonly ConstructorT2<IProvider, Delegate?, ConcurrentDictionaryCacheProviderNullableKey> providerCtor = new(typeof(ConcurrentDictionaryCacheProviderNullableKey<,>));
    /// <summary>Create</summary>
    public static ConcurrentDictionaryCacheProviderNullableKey Create(Type keyType, Type valueType, Delegate createFunc, Delegate? toCacheCriteria = default) => delegateCtor.Create(keyType, valueType, createFunc, toCacheCriteria);
    /// <summary>Create</summary>
    public static ConcurrentDictionaryCacheProviderNullableKey Create(Type keyType, Type valueType, IProvider createProvider, Delegate? toCacheCriteria = default) => providerCtor.Create(keyType, valueType, createProvider, toCacheCriteria);
    /// <summary>Create</summary>
    public static ConcurrentDictionaryCacheProviderNullableKey Create(IProvider createProvider, Delegate? toCacheCriteria = default) => providerCtor.Create(createProvider.Key, createProvider.Value, createProvider, toCacheCriteria);

    /// <summary></summary>
    public abstract Type Key { get; }
    /// <summary></summary>
    public abstract Type Value { get; }
    /// <summary>Create source</summary>
    public virtual IProvider[]? Sources => sources;
    /// <summary>Delegate that evaluates whether key is to be cached or not.</summary>
    public abstract Delegate? ToCacheCriteria { get; }

    /// <summary>Create source</summary>
    protected IProvider[]? sources;
    /// <summary>Internal dictionary</summary>
    protected abstract object getMap();
    /// <summary>Internal dictionary</summary>
    public object Map => getMap();
    /// <summary></summary>
    bool ICache.IsCache { get => true; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    bool ICached.IsCached { get => true; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    bool IDecoration.IsDecoration { get => true; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    object? IDecoration.Decoree { get => sources == null || sources.Length!=1 ? null : sources[0]; set => throw new NotImplementedException(); }

    /// <summary>Get value that corresponds <paramref name="key"/></summary>
    /// <exception cref="InvalidCastException">If key is wrong type.</exception>
    public abstract bool TryGetValue(object key, out object value);
    /// <summary>Clear cache</summary>
    public abstract void InvalidateCache(bool deep = false);
    /// <summary>Print information</summary>
    public override string ToString() => CanonicalName.Print(GetType());
}

/// <summary>Cache concurrent use.</summary>
/// <remarks>Uses <see cref="ConcurrentDictionary{Key,Value}"/></remarks>
public class ConcurrentDictionaryCacheProviderNullableKey<TKey, TValue> :
    ConcurrentDictionaryCacheProviderNullableKey,
    IProvider<TKey, TValue>
    where TKey : notnull
{
    /// <summary>Evaluates whether key is to be cached or not.</summary>
    protected Func<TKey, bool>? toCacheCriteria;
    /// <summary></summary>
    public override Type Key => typeof(TKey);
    /// <summary></summary>
    public override Type Value => typeof(TValue);
    /// <summary>Delegate that evaluates whether key is to be cached or not.</summary>
    public override Delegate? ToCacheCriteria => toCacheCriteria;

    /// <summary>Key to value indexer</summary>
    public TValue this[TKey key]
    {
        get
        {
            // Key is not to be cached, create new value
            if (toCacheCriteria != null && !toCacheCriteria(key)) return createProvider[key];
            // Try getting existing
            if (map.TryGetValue(new ValueTuple<TKey>(key), out TValue? existing)) return existing;
            // Create new
            TValue newValue = createProvider[key];
            // Try cache
            TValue value = map.GetOrAdd(new ValueTuple<TKey>(key), newValue);
            // Return result
            return value;
        }
    }

    /// <summary>Map</summary>
    protected ConcurrentDictionary<ValueTuple<TKey>, TValue> map;
    /// <summary>Map</summary>
    public new ConcurrentDictionary<ValueTuple<TKey>, TValue> Map => map;
    /// <summary>Map</summary>
    protected override object getMap() => map;
    /// <summary>Source provider</summary>
    protected IProvider<TKey, TValue> createProvider;
    /// <summary>Create cache</summary>
    /// <param name="createProvider">Provider that creates new value.</param>
    /// <param name="toCacheCriteria">Evaluates whether key is to be cached or not.</param>
    /// <remarks>Note that <paramref name="toCacheCriteria"/> may be invoked multiple times in concurrent scenario and value let go</remarks>
    public ConcurrentDictionaryCacheProviderNullableKey(IProvider<TKey, TValue> createProvider, Func<TKey, bool>? toCacheCriteria = default)
    {
        this.createProvider = createProvider ?? throw new ArgumentNullException(nameof(createProvider));
        this.sources = new[] { createProvider };
        this.map = new ConcurrentDictionary<ValueTuple<TKey>, TValue>();
        this.toCacheCriteria = toCacheCriteria;
    }

    /// <summary>Create cache</summary>
    /// <param name="createProvider">Provider that creates new value.</param>
    /// <param name="toCacheCriteria">Evaluates whether key is to be cached or not.</param>
    /// <remarks>Note that <paramref name="toCacheCriteria"/> may be invoked multiple times in concurrent scenario and value let go</remarks>
    public ConcurrentDictionaryCacheProviderNullableKey(IProvider<TKey, TValue> createProvider, ConcurrentDictionary<ValueTuple<TKey>, TValue> map, Func<TKey, bool>? toCacheCriteria = default)
    {
        this.createProvider = createProvider ?? throw new ArgumentNullException(nameof(createProvider));
        this.sources = new[] { createProvider };
        this.map = map ?? throw new ArgumentNullException(nameof(map));
        this.toCacheCriteria = toCacheCriteria;
    }

    /// <summary>Create cache</summary>
    /// <param name="createProvider">Provider that creates new value.</param>
    /// <param name="toCacheCriteria">Evaluates whether key is to be cached or not.</param>
    /// <remarks>Note that <paramref name="toCacheCriteria"/> may be invoked multiple times in concurrent scenario and value let go</remarks>
    public ConcurrentDictionaryCacheProviderNullableKey(IProvider<TKey, TValue> createProvider, IEqualityComparer<ValueTuple<TKey>> comparer, Func<TKey, bool>? toCacheCriteria = default)
    {
        this.createProvider = createProvider ?? throw new ArgumentNullException(nameof(createProvider));
        this.sources = new[] { createProvider };
        this.map = new ConcurrentDictionary<ValueTuple<TKey>, TValue>(comparer);
        this.toCacheCriteria = toCacheCriteria;
    }

    /// <summary>Get-or-create value</summary>
    public TValue Get(TKey key)
    {
        // Key is not to be cached, create new value
        if (toCacheCriteria != null && !toCacheCriteria(key)) return createProvider[key];
        // Try getting existing
        if (map.TryGetValue(new ValueTuple<TKey>(key), out TValue? existing)) return existing;
        // Create new
        TValue newValue = createProvider[key];
        // Try cache
        TValue value = map.GetOrAdd(new ValueTuple<TKey>(key), newValue);
        // Return result
        return value;
    }

    /// <summary>Try get value</summary>
    public bool TryGetValue(TKey key, out TValue value)
    {
        // Key is not to be cached, create new value
        if (toCacheCriteria != null && !toCacheCriteria(key)) return createProvider.TryGetValue(key, out value);
        // Try getting existing
        if (map.TryGetValue(new ValueTuple<TKey>(key), out value!)) return true;
        // Create new
        if (!createProvider.TryGetValue(key, out value)) { value = default!; return false; }
        // Try cache
        value = map.GetOrAdd(new ValueTuple<TKey>(key), value);
        // Return result
        return true;
    }

    /// <summary>Try get value</summary>
    /// <exception cref="InvalidCastException">If key is not <typeparamref name="TKey"/>.</exception>
    public override bool TryGetValue(object keyObject, out object value)
    {
        // Cast 
        if (keyObject is not TKey key) key = default!;
        // Key is not to be cached, create new value
        if (keyObject == null || (toCacheCriteria != null && !toCacheCriteria(key))) { bool ok = createProvider.TryGetValue(key!, out TValue value0); value = value0!; return ok; }
        // Try getting existing
        if (map.TryGetValue(new ValueTuple<TKey>(key), out TValue? value1)) { value = value1!; return true; }
        // Create new
        if (!createProvider.TryGetValue(key, out value1)) { value = default!; return false; }
        // Cache
        value1 = map.GetOrAdd(new ValueTuple<TKey>(key), value1);
        // Return result
        value = value1!;
        return true;
    }

    /// <summary>Clear cache</summary>
    public override void InvalidateCache(bool deep)
    {
        map.Clear();
        if (deep && createProvider is ICached cached) cached.InvalidateCache(deep);
    }

    /// <summary>Print information</summary>
    public override string ToString() => $"{createProvider}.Cached()";
}


