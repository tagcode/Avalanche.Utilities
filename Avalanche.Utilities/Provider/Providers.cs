// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider;
using Avalanche.Utilities.Provider.Internal;
using System;

/// <summary>Provider methods</summary>
public static class Providers
{
    /// <summary>Convert <paramref name="func"/> to provider</summary>
    public static IProvider<Key, Value> Func<Key, Value>(Func<Key, Value> func) => new FuncProvider<Key, Value>(func);
    /// <summary>Convert <paramref name="tryCreate"/> to provider</summary>
    public static IProvider<Key, Value> Func<Key, Value>(Avalanche.Utilities.Provider.TryCreate<Key, Value> tryCreate) => new TryCreateProvider<Key, Value>(tryCreate);
    /// <summary>Convert tryCreate to provider</summary>
    public static IProvider<Key, Value> NullableFunc<Key, Value>(Avalanche.Utilities.Provider.TryCreate<Key, Value?> nullableTryCreate) where Value : struct => new NullableTryCreateProvider<Key, Value>(nullableTryCreate);
    /// <summary>Use provider provided by <paramref name="providerFunc"/>.</summary>
    public static IProvider<Key, Value> Indirect<Key, Value>(Func<IProvider<Key, Value>> providerFunc) => new IndirectProvider<Key, Value>(providerFunc);
    /// <summary>Create decorator that concatenates results</summary>
    public static IProvider<Key, IEnumerable<Value>> EnumerableConcat<Key, Value>(IEnumerable<IProvider<Key, IEnumerable<Value>>> providers, bool distinctValues, IEqualityComparer<Value>? valueEqualityComparer = null) => distinctValues ? new ResultConcatProvider<Key, Value>.Distinct(providers, valueEqualityComparer) : new ResultConcatProvider<Key, Value>(providers);
}
