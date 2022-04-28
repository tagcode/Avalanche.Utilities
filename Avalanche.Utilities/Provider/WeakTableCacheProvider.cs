// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider;
using System.Runtime.CompilerServices;

/// <summary>Cache with weak referenced keys</summary>
/// <remarks>Uses <see cref="ConditionalWeakTable{Key,Value}"/> where keys are object reference compared, not hash-equals.</remarks>
public abstract class WeakTableCacheProvider : IProvider, IProviderStack, ICache, IDecoration
{
    /// <summary>Constructor</summary>
    static readonly ConstructorT2<IProvider, Delegate?, WeakTableCacheProvider> ctor = new(typeof(WeakTableCacheProvider<,>));
    /// <summary>Create</summary>
    public static WeakTableCacheProvider Create(Type keyType, Type valueType, IProvider createProvider, Delegate? toCacheCriteria = default) => ctor.Create(keyType, valueType, createProvider, toCacheCriteria);
    /// <summary>Create</summary>
    public static WeakTableCacheProvider Create(IProvider createProvider, Delegate? toCacheCriteria = default) => ctor.Create(createProvider.Key, createProvider.Value, createProvider, toCacheCriteria);

    /// <summary>Create source</summary>
    protected IProvider[]? sources;

    /// <summary></summary>
    public abstract Type Key { get; }
    /// <summary></summary>
    public abstract Type Value { get; }
    /// <summary>Create source</summary>
    public abstract IProvider[]? Sources { get; }
    /// <summary>Delegate that evaluates whether key is to be cached or not.</summary>
    public abstract Delegate? ToCacheCriteria { get; }
    /// <summary></summary>
    bool ICache.IsCache { get => true; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    bool ICached.IsCached { get => true; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    bool IDecoration.IsDecoration { get => true; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    object? IDecoration.Decoree { get => sources == null || sources.Length != 1 ? null : sources[0]; set => throw new NotImplementedException(); }

    /// <summary>Internal dictionary</summary>
    protected abstract object getMap();
    /// <summary>Internal dictionary</summary>
    public object Map => getMap();

    /// <summary>Get value that corresponds <paramref name="key"/></summary>
    /// <exception cref="InvalidCastException">If key is wrong type.</exception>
    public abstract bool TryGetValue(object key, out object value);
    /// <summary>Clear cache</summary>
    public abstract void InvalidateCache(bool deep = false);
}

/// <summary>Cache with weak referenced keys</summary>
/// <remarks>Uses <see cref="ConditionalWeakTable{Key,Value}"/> where keys are object reference compared, not hash-equals.</remarks>
public class WeakTableCacheProvider<TKey, TValue> :
    WeakTableCacheProvider,
    IProvider<TKey, TValue>
    where TKey : class
    where TValue : class?
{
    /// <summary>Evaluates whether key is to be cached or not.</summary>
    protected Func<TKey, bool>? toCacheCriteria;

    /// <summary></summary>
    public override Type Key => typeof(TKey);
    /// <summary></summary>
    public override Type Value => typeof(TValue);
    /// <summary>Create source</summary>
    public override IProvider[]? Sources => new[] { createProvider };
    /// <summary>Delegate that evaluates whether key is to be cached or not.</summary>
    public override Delegate? ToCacheCriteria => toCacheCriteria;
    /// <summary>Key to value indexer</summary>
    public TValue this[TKey key]
    {
        get
        {
            // Key is not to be cached, create new value
            if (toCacheCriteria != null && !toCacheCriteria(key)) return createProvider[key];
            // Get existing
            if (map.TryGetValue(key, out TValue? _value)) return _value;
            // Create value
            if (!createProvider.TryGetValue(key, out _value)) throw new KeyNotFoundException($"{key}");
            // Assign value
            lock (map)
            {
                // Get existing, again in lock
                if (map.TryGetValue(key, out TValue? __value)) return __value;
                // Assign
                map.AddOrUpdate(key, _value);
            }
            //
            return _value;
        }
    }

    /// <summary>Create provider</summary>
    protected IProvider<TKey, TValue> createProvider;
    /// <summary>Map</summary>
    protected ConditionalWeakTable<TKey, TValue> map;
    /// <summary>Map</summary>
    public new ConditionalWeakTable<TKey, TValue> Map => map;
    /// <summary>Map</summary>
    protected override object getMap() => map;

    /// <summary>Create cache</summary>
    /// <param name="toCacheCriteria">Evaluates whether key is to be cached or not.</param>
    /// <remarks>Note that <paramref name="createProvider"/> may be invoked multiple times in concurrent scenario and value let go</remarks>
    public WeakTableCacheProvider(IProvider<TKey, TValue> createProvider, Func<TKey, bool>? toCacheCriteria = default)
    {
        this.createProvider = createProvider ?? throw new ArgumentNullException(nameof(createProvider));
        this.toCacheCriteria = toCacheCriteria;
        this.sources = new[] { createProvider };
        this.map = new();
    }

    /// <summary>Create cache</summary>
    /// <param name="toCacheCriteria">Evaluates whether key is to be cached or not.</param>
    /// <remarks>Note that <paramref name="createProvider"/> may be invoked multiple times in concurrent scenario and value let go</remarks>
    public WeakTableCacheProvider(IProvider<TKey, TValue> createProvider, ConditionalWeakTable<TKey, TValue> map, Func<TKey, bool>? toCacheCriteria = default)
    {
        this.createProvider = createProvider ?? throw new ArgumentNullException(nameof(createProvider));
        this.toCacheCriteria = toCacheCriteria;
        this.sources = new[] { createProvider };
        this.map = map ?? throw new ArgumentNullException(nameof(map));
    }

    /// <summary>Try get value</summary>
    public bool TryGetValue(TKey key, out TValue value)
    {
        // Key is not to be cached, create new value
        if (toCacheCriteria != null && !toCacheCriteria(key)) return createProvider.TryGetValue(key, out value);
        // Try get cached value
        if (map.TryGetValue(key, out TValue? _value)) { value = _value!; return true; }
        // Create value
        if (!createProvider.TryGetValue(key, out _value)) { value = default!; return false; }
        // Assign value to cache
        lock (map)
        {
            // Try get existing, again in lock
            if (map.TryGetValue(key, out TValue? __value)) { value = __value!; return true; }
            // Assign to cache
            map.AddOrUpdate(key, _value);
        }
        //
        value = _value;
        //
        return true;
    }

    /// <summary>Try get value</summary>
    /// <exception cref="InvalidCastException">If key is not <typeparamref name="TKey"/>.</exception>
    public override bool TryGetValue(object keyObject, out object value)
    {
        // Cast 
        if (keyObject is not TKey key) key = default!;
        // Key is not to be cached, create new value
        if (toCacheCriteria != null && !toCacheCriteria(key)) return createProvider.TryGetValue(key, out value);
        // Get existing
        if (map.TryGetValue(key, out TValue? _value)) { value = _value!; return true; }
        // Create value
        if (!createProvider.TryGetValue(key, out _value)) { value = default!; return false; }
        // Assign value
        lock(map)
        {
            // Get existing, again in lock
            if (map.TryGetValue(key, out TValue? __value)) { value = __value!; return true; }
            // Assign
            map.AddOrUpdate(key, _value);
        }
        //
        value = _value!;
        //
        return true;
    }

    /// <summary>Clear cache</summary>
    public override void InvalidateCache(bool deep)
    {
        map.Clear();
        if (deep && createProvider is ICached cached) cached.InvalidateCache(deep);
    }

    /// <summary>Print information</summary>
    public override string ToString() => $"{createProvider}.WeakCached()";
}
