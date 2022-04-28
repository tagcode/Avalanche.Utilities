// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;

// <docs>
/// <summary>Annotable type</summary>
public interface IAnnotable
{
    /// <summary>Annotations, typically <see cref="Attribute"/>s or strings, but always easily serializable.</summary>
    object[] Annotations { get; set; }
}
// </docs>
