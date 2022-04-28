// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Runtime.Serialization;

// <docs>
/// <summary>Interface for objects whose state can be locked into read-only state.</summary>
public interface IReadOnly
{
    /// <summary>Is class immutable.</summary>
    /// <exception cref="InvalidOperationException">If writability state change is not allowed.</exception>
    [IgnoreDataMember]
    bool ReadOnly { get; set; }
}
// </docs>
