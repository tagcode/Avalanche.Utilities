// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

// <docs>
/// <summary>Clones objects</summary>
public interface ICloner
{
    /// <summary>Clone <paramref name="src"/></summary>
    /// <param name="src"></param>
    object Clone(object src);
}
// </docs>

// <docsT>
/// <summary>Clones objects</summary>
public interface ICloner<T>
{
    /// <summary>Clone <paramref name="src"/></summary>
    /// <param name="src"></param>
    T Clone(in T src);
}
// </docsT>
