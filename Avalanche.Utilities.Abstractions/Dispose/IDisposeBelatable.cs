// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Diagnostics.CodeAnalysis;

// <docs>
/// <summary>Interface for objects whose dispose can be belated. </summary>
public interface IDisposeBelatable : IDisposable
{
    /// <summary>Try creates a handle that postpones the dispose of the object until all the belate-handles have been disposed.</summary>
    /// <returns>belating handle that must be diposed</returns>
    bool TryBelateDispose([NotNullWhen(true)] out IDisposable? belateHandle);
}
// </docs>

