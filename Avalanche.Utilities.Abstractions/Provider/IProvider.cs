// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider;
using System;
using System.Collections.Generic;

// <docs>
/// <summary></summary>
public interface IProvider
{
    /// <summary>Key type</summary>
    Type Key { get; }
    /// <summary>Value type</summary>
    Type Value { get; }
    /// <summary>Get value that corresponds <paramref name="key"/></summary>
    /// <exception cref="InvalidCastException">If key is wrong type.</exception>
    bool TryGetValue(object key, out object value);
}
// </docs>

// <docsT>
/// <summary></summary>
public interface IProvider<Key> : IProvider { }
/// <summary></summary>
public interface IProviderOf<Value> : IProvider { }
/// <summary></summary>
public interface IProvider<Key, Value> : IProvider<Key>, IProviderOf<Value>
{
    /// <summary>Key to value indexer</summary>
    /// <exception cref="KeyNotFoundException">If <paramref name="key"/> is not found.</exception>
    Value this[Key key] { get; }

    /// <summary>Try get <typeparamref name="Value"/> that corresponds <paramref name="key"/></summary>
    bool TryGetValue(Key key, out Value value);
}
// </docsT>
