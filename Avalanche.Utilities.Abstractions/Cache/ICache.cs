// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

// <docs>
/// <summary>Interface for objects that are cache objects. Used with decoration stacks to indicate which levels are cache objects.</summary>
public interface ICache : ICached
{ 
    /// <summary>Is this object reference a cache object.</summary>
    bool IsCache { get; set; }
}
// </docs>
