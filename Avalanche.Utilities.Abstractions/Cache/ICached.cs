// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

// <docs>
/// <summary>Interface for classes that may containt cached content.</summary>
public interface ICached
{ 
    /// <summary>Object contains cached content.</summary>
    bool IsCached { get; set; }
    /// <summary>Clear cache</summary>
    /// <param name="deep">Deep recursive cache invalidation.</param>
    void InvalidateCache(bool deep = false);
}
// </docs>
