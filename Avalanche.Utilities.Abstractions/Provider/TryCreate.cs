// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider;

// <docs>
/// <summary>Try create pattern.</summary>
public delegate bool TryCreate<Key, Value>(Key key, out Value value);
// </docs>
