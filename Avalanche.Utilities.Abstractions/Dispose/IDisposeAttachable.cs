// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections.Generic;

// <docs>
/// <summary>
/// Object where <see cref="IDisposable"/>s can be attached. Attachments will be disposed along with the object.
/// 
/// There are four stages of dispose: NotDisposed, DisposeCalled, Disposing, Disposed.
/// If dispose is belated, then dispose call won't start internal dispose until belatables are cleared.
/// </summary>
public interface IDisposeAttachable : IDisposable
{
    /// <summary>Has <see cref="IDisposable.Dispose"/> been called whether or not internal dispose has started. Internal dispose starts when all belatables are disposed.</summary>
    bool IsDisposeCalled { get; }
    /// <summary>Has internal dispose started or completed.</summary>
    bool IsDisposing { get; }
    /// <summary>Has dispose completed.</summary>
    bool IsDisposed { get; }

    /// <summary>
    /// Attach <paramref name="disposable"/> to be disposed along with this object.
    /// 
    /// If the implementing object has already been disposed, this method immediately disposes the <paramref name="disposable"/>.
    /// </summary>
    /// <param name="disposable"></param>
    /// <returns>true if was added to list, false if wasn't but was disposed immediately</returns>
    bool AttachDisposable(object disposable);

    /// <summary>
    /// Attach <paramref name="disposableObjects"/> to be disposed along with this object.
    /// 
    /// If the implementing object has already been disposed, this method immediately disposes the <paramref name="disposableObjects"/>.
    /// </summary>
    /// <param name="disposableObjects"></param>
    /// <returns>true if were added to list, false if were disposed immediately</returns>
    bool AttachDisposables(IEnumerable<object> disposableObjects);

    /// <summary>
    /// Remove <paramref name="disposableObject"/> from the attachables. 
    /// </summary>
    /// <param name="disposableObject"></param>
    /// <returns>true if was removed, false if it wasn't in the list.</returns>
    bool RemoveDisposable(object disposableObject);

    /// <summary>
    /// Remove <paramref name="disposableObjects"/> from the attachables 
    /// </summary>
    /// <param name="disposableObjects"></param>
    /// <returns>true if was removed, false if it wasn't in the list.</returns>
    bool RemoveDisposables(IEnumerable<object> disposableObjects);

    /// <summary>
    /// Attach <paramref name="disposeAction"/> to be executed on dispose of this object.
    /// 
    /// If parent object is disposed or being disposed, the <paramref name="disposeAction"/> is executed immediately.
    /// </summary>
    /// <param name="disposeAction"></param>
    /// <param name="state"></param>
    /// <returns>true if was added to list, false if was disposed right away</returns>
    bool AttachDisposeAction(Action<object> disposeAction, object? state);
}
// </docs>


