// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

/// <summary>Collection that provides thread-safe snapshot of contents.</summary>
public interface ISnapshotProvider<T>
{
    /// <summary>Content as snapshot</summary>
    T[] Snapshot { get; set; }
}
