// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Runtime.Serialization;

// <docs>
/// <summary>
/// Indicates that the inner contents of the instance may contain a cycle back to the instance.
/// 
/// This is used with graph comparer, equality-comparer and cloner implementations.
/// </summary>
public interface ICyclical
{
    /// <summary>Can instance be cyclic. False = never, True = yes/possibly.</summary>
    [IgnoreDataMember] bool IsCyclical { get; set; }
}
// </docs>
