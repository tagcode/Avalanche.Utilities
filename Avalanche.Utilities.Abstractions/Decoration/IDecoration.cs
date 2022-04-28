// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

// <docs>
/// <summary>Interface for decorations.</summary>
public interface IDecoration
{
    /// <summary>Is decoration.</summary>
    bool IsDecoration { get; set; }
    /// <summary>Object that is decorated.</summary>
    object? Decoree { get; set; }
}
// </docs>
