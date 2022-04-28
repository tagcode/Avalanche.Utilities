// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

/// <summary></summary>
public interface INonDisposeAssignable
{
    /// <summary>Non-disposable is a flag for objects that have dispose barried, such as singleton instances.</summary>
    bool NonDisposable { get; set; }

}
