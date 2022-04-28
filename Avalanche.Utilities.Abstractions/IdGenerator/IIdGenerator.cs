// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

// <docs>
/// <summary>Generates unique identities</summary>
public interface IIdGenerator
{
    /// <summary>Identity type.</summary>
    Type Type { get; }
    /// <summary>Uniqueness level</summary>
    Scope Uniqueness { get; }

    /// <summary>Uniqueness scope</summary>
    enum Scope
    {
        /// <summary>Unique id for the generator instance</summary>
        Instance,
        /// <summary>Unique id for the process</summary>
        Process,
        /// <summary>Unique id for the computer</summary>
        System,
        /// <summary>Globally unique id</summary>
        Global,
        /// <summary>Universally unique id</summary>
        Universal
    }

    /// <summary>Try create next unique identity.</summary>
    /// <returns>False if out of unique ids.</returns>
    bool TryGetNext(out object value);

    /// <summary>Get next identity</summary>
    /// <exception cref="InvalidOperationException">If no more identities are available.</exception>
    object Next { get; }
}
// </docs>

// <docsT>
/// <summary>Generates unique identities</summary>
public interface IIdGenerator<T> : IIdGenerator
{
    /// <summary>Has more unique ids.</summary>
    bool HasMore { get; }
    /// <summary>Try create next unique identity.</summary>
    /// <returns>False if out of unique ids.</returns>
    bool TryGetNext(out T value);
    /// <summary>Get next identity</summary>
    /// <exception cref="InvalidOperationException">If no more identities are available.</exception>
    new T Next { get; }
}
// </docsT>
