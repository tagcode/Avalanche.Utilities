﻿// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

/// <summary>
/// Base implementation for classes where dispose attachments can be added and whose dispose can be belated.
/// 
/// All attached disposables are disposed with object.
/// 
/// The implementing class implements <see cref="InnerDispose"/> for the actual dispose. InnerDispose will be called only once. 
/// A disposable that manages a list of disposable objects.
/// </summary>
public class DisposeAttachable : IDisposeAttachable, IDisposeBelatable, INonDisposeAssignable
{
    /// <summary>Lock for modifying <see cref="attachments"/>.</summary>
    protected object attachmentsLock = new object();
    /// <summary>List of dispose attachments that has been attached with this object.</summary>
    protected StructList2<IDisposable> attachments = new StructList2<IDisposable>();

    /// <summary>
    /// State that is set when disposing starts and finalizes.
    /// Is changed with Interlocked. 
    ///  0 - not disposed
    ///  1 - dispose called, but not started
    ///  2 - disposing started
    ///  3 - disposed
    ///  
    /// When disposing starts, new disposables cannot be attached, instead they are activated right at away by the calling thread.
    /// </summary>
    protected long disposing;

    /// <summary>Has <see cref="IDisposable.Dispose"/> been called whether or not internal dispose has started. Internal dispose starts when all belatables are disposed.</summary>
    public bool IsDisposeCalled => Interlocked.Read(ref disposing) >= 1L;
    /// <summary>Has internal dispose started or completed.</summary>
    public bool IsDisposing => Interlocked.Read(ref disposing) >= 2L;
    /// <summary>Has dispose completed.</summary>
    public bool IsDisposed => Interlocked.Read(ref disposing) == 3L;

    /// <summary>Number of belate handles.</summary>
    protected int belateHandleCount;

    /// <summary>
    /// Non disposable is a flag for objects that cannot be disposed, such as singleton instances.
    /// <see cref="nonDisposable"/> is set at construction.
    /// 
    /// Use method <see cref="SetToNonDisposable"/> to modify the state at constructor.
    /// </summary>
    protected bool nonDisposable;

    /// <summary>Non-disposable is a flag for objects that have dispose barried, such as singleton instances.</summary>
    bool INonDisposeAssignable.NonDisposable { get => nonDisposable; set => this.nonDisposable = value | nonDisposable; }

    /// <summary>
    /// Non-disposable is a flag for objects that cannot be disposed, such as singleton instances.
    /// <see cref="nonDisposable"/> is set at construction.
    /// 
    /// When Dispose() is called for non-disposable object, the attached disposables
    /// are removed and disposed, but the object itself does not go into disposed state.
    /// </summary>
    /// <returns>self</returns>
    protected object SetToNonDisposable() { this.nonDisposable = true; return this; }

    /// <summary>Delay dispose until belate handle is disposed.</summary>
    /// <returns>Belate handle</returns>
    /// <exception cref="ObjectDisposedException">Thrown if object has already been disposed.</exception>
    public bool TryBelateDispose([NotNullWhen(true)] out IDisposable? belateHandle)
    {
        // Create handle
        belateHandle = new BelateHandle(this);

        lock (attachmentsLock)
        {
            // Dispose has already been started
            if (IsDisposing) { belateHandle = null; return false; }
            // Add counter
            belateHandleCount++;
        }
        // Return handle
        return true;
    }

    /// <summary>A handle that postpones dispose of the <see cref="DisposeAttachable"/> object.</summary>
    sealed class BelateHandle : IDisposable
    {
        /// <summary></summary>
        DisposeAttachable? parent;

        /// <summary>Create belate handle</summary>
        /// <param name="parent"></param>
        public BelateHandle(DisposeAttachable parent)
        {
            this.parent = parent;
        }

        /// <summary>Close belate handle. May activate <see cref="parent"/>'s dispose.</summary>
        public void Dispose()
        {
            // Only one thread can dispose
            DisposeAttachable? _parent = Interlocked.CompareExchange(ref parent, null, parent);
            // Handle has already been disposed
            if (_parent == null) return;
            // Should dispose be started
            bool processDispose = false;
            // Decrement handle count
            lock (_parent.attachmentsLock)
            {
                int newCount = --_parent.belateHandleCount;
                // Is not the handle.
                if (newCount > 0) return;
                // Check Dispose() has been called when counter goes to 0
                processDispose = _parent.IsDisposeCalled;
            }
            // Start dispose
            if (processDispose) { if (_parent.nonDisposable) _parent.ProcessNonDispose(); else _parent.ProcessDispose(); }
        }
    }

    /// <summary>
    /// Dispose object. This method is intended to be called by the consumer of the object.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Marks dispose-has-been-called. If there are no belate handles, then proceeds with dispose.
    /// </summary>
    /// <param name="disposing">
    ///     If true, called by Dispose(), and should dispose managed and unmanaged resources.
    ///     If false, called by GC, and should dispose only unmanaged resources.
    /// </param>
    /// <exception cref="AggregateException">thrown if disposing threw errors</exception>
    protected virtual void Dispose(bool disposing)
    {
        // Called by finalized, someone forgot to dispose the object
        if (!disposing)
        {
            // Collection of errors
            StructList4<Exception> errors = new StructList4<Exception>();
            // Dispose unmanaged resources
            InnerDisposeUnmanaged(ref errors);
            // Throw captured errors
            if (errors.Count > 0) throw ExceptionUtilities.Wrap(errors);
        }

        // Dispose unamnaged and managed resources
        if (disposing)
        {
            // Dispose() called
            Interlocked.CompareExchange(ref this.disposing, 1L, 0L);

            // Should dispose be started
            bool processDispose = false;

            lock (attachmentsLock)
            {
                // Post-pone if there are belate handles
                if (belateHandleCount > 0) return;
                // Set state to dispose called
                processDispose = Interlocked.Read(ref this.disposing) <= 1L;
            }

            // Start dispose
            if (processDispose) { if (nonDisposable) ProcessNonDispose(); else ProcessDispose(); }
        }
    }

    /// <summary>
    /// Process the actual dispose. This may be called from Dispose() or from the dispose of the last
    /// belate handle (After Dispose() has been called aswell).
    /// 
    /// Disposes all attached diposables and call <see cref="InnerDispose"/>.
    /// 
    /// Only one thread may process the dispose.
    /// Sets state to 2, and then 3.
    /// 
    /// Unattaches all disposables, disposes them, and calls <see cref="InnerDispose"/>.
    /// </summary>
    /// <exception cref="AggregateException">thrown if disposing threw errors</exception>
    protected virtual void ProcessDispose()
    {
        // Set state IsDisposing=2, but let only one thread continue.
        bool thisThreadChangedStateToDispose = (Interlocked.CompareExchange(ref disposing, 2L, 0L) == 0L) || (Interlocked.CompareExchange(ref disposing, 2L, 1L) == 1L);
        // Not for this thread.
        if (!thisThreadChangedStateToDispose) return;

        // Extract snapshot, clear array
        StructList2<IDisposable> toDispose = default;
        lock (attachmentsLock) { toDispose = attachments; attachments = default; }

        // Captured errors
        StructList4<Exception> errors = new StructList4<Exception>();

        // Dispose disposables
        DisposeAndCapture(ref toDispose, ref errors);

        // Call InnerDispose(). Capture errors to compose it with others.
        try
        {
            InnerDispose(ref errors);
        }
        catch (Exception e)
        {
            // Capture error
            errors.Add(e);
        }

        // Call InnerDisposeUnmanaged(). Capture errors to compose it with others.
        try
        {
            InnerDisposeUnmanaged(ref errors);
        }
        catch (Exception e)
        {
            // Capture error
            errors.Add(e);
        }

        // Is disposed
        Interlocked.CompareExchange(ref disposing, 3L, 2L);

        // Throw captured errors
        if (errors.Count > 0) throw ExceptionUtilities.Wrap(errors);
    }

    /// <summary>
    /// Process the non-dispose. Used when <see cref="nonDisposable"/> is true (singleton instances).
    /// 
    /// This may be called from Dispose() or from the dispose of the last
    /// belate handle (After Dispose() has been called aswell).
    /// 
    /// Only one thread may process the dispose. Returns state back to 0.
    /// 
    /// Unattaches all disposables, disposes them, and calls <see cref="InnerDispose"/>.
    /// Does not set state 
    /// </summary>
    /// <exception cref="AggregateException">thrown if disposing threw errors</exception>
    protected virtual void ProcessNonDispose()
    {
        // Revert state
        Interlocked.CompareExchange(ref disposing, 0L, 1L);

        // Extract snapshot, clear array
        StructList2<IDisposable> toDispose = default;
        lock (attachmentsLock) { toDispose = attachments; attachments = default; }

        // Captured errors
        StructList4<Exception> errors = new StructList4<Exception>();

        // Dispose disposables
        DisposeAndCapture(ref toDispose, ref errors);

        // Call InnerDispose(). Capture errors to compose it with others.
        try
        {
            InnerDispose(ref errors);
        }
        catch (Exception e)
        {
            // Capture error
            errors.Add(e);
        }

        // Call InnerDisposeUnmanaged(). Capture errors to compose it with others.
        try
        {
            InnerDisposeUnmanaged(ref errors);
        }
        catch (Exception e)
        {
            // Capture error
            errors.Add(e);
        }

        // Throw captured errors
        if (errors.Count > 0) throw ExceptionUtilities.Wrap(errors);
    }

    /// <summary>Override this to dispose managed resources</summary>
    /// <param name="errors">list that can be instantiated and where errors can be added</param>
    /// <exception cref="Exception">any exception is captured and aggregated with other errors</exception>
    protected virtual void InnerDispose<Exceptions>(ref Exceptions errors) where Exceptions : IList<Exception>
    {
    }

    /// <summary>Override this to dispose unmanaged resources.</summary>
    /// <param name="errors">list that can be instantiated and where errors can be added</param>
    /// <exception cref="Exception">any exception is captured and aggregated with other errors</exception>
    protected virtual void InnerDisposeUnmanaged<Exceptions>(ref Exceptions errors) where Exceptions : IList<Exception>
    {
    }

    /// <summary>
    /// Add <paramref name="disposableObject"/> to be disposed with the object.
    /// 
    /// If parent object is disposed or being disposed, the disposable will be disposed immedialy.
    /// </summary>
    /// <param name="disposableObject"></param>
    /// <returns>true if was added to list, false if was disposed right away</returns>
    bool IDisposeAttachable.AttachDisposable(object disposableObject)
    {
        // Argument error
        if (disposableObject == null) throw new ArgumentNullException(nameof(disposableObject));
        // Cast to IDisposable
        IDisposable? disposable = disposableObject as IDisposable;
        // Was not disposable, was not added to list
        if (disposable == null) return false;
        // Parent is disposed/ing
        if (IsDisposing) { disposable.Dispose(); return false; }
        // Add to list
        lock (attachmentsLock) attachments.Add(disposable);
        // Check parent again
        if (IsDisposing) { lock (attachmentsLock) attachments.Remove(disposable); disposable.Dispose(); return false; }
        // OK
        return true;
    }

    /// <summary>
    /// Invoke <paramref name="disposeAction"/> on the dispose of the object.
    /// 
    /// If parent object is disposed or being disposed, the <paramref name="disposeAction"/> is executed immediately.
    /// </summary>
    /// <param name="disposeAction"></param>
    /// <param name="state"></param>
    /// <returns>true if was added to list, false if was disposed right away</returns>
    bool IDisposeAttachable.AttachDisposeAction(Action<object> disposeAction, object? state)
    {
        // Argument error
        if (disposeAction == null) throw new ArgumentNullException(nameof(disposeAction));
        // Parent is disposed/ing
        if (IsDisposing) { disposeAction(this); return false; }
        // Adapt to IDisposable
        IDisposable disposable = new DisposeAction<object>(disposeAction, state!);
        // Add to list
        lock (attachmentsLock) attachments.Add(disposable);
        // Check parent again
        if (IsDisposing) { lock (attachmentsLock) attachments.Remove(disposable); disposable.Dispose(); return false; }
        // OK
        return true;
    }

    /// <summary>
    /// Adapts <see cref="Action"/> into <see cref="IDisposable"/>.
    /// </summary>
    public sealed class DisposeAction : IDisposable
    {
        Action<object> action;
        object state;

        /// <summary>
        /// Create dispose action.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="state"></param>
        public DisposeAction(Action<object> a, object state)
        {
            this.action = a ?? throw new ArgumentNullException(nameof(a));
            this.state = state;
        }

        /// <summary>
        /// Run delegate with the attached object.
        /// </summary>
        public void Dispose()
            => action(state);
    }

    /// <summary>
    /// Adapts <see cref="Action"/> into <see cref="IDisposable"/>.
    /// </summary>
    public sealed class DisposeAction<T> : IDisposable
    {
        Action<T> action;
        T disposeObject;

        /// <summary>
        /// Create dispose action.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="disposeObject"></param>
        public DisposeAction(Action<T> a, T disposeObject)
        {
            this.action = a ?? throw new ArgumentNullException(nameof(a));
            this.disposeObject = disposeObject;
        }

        /// <summary>
        /// Run delegate with the attached object.
        /// </summary>
        public void Dispose()
            => action(disposeObject);
    }

    /// <summary>
    /// Add <paramref name="disposableObjects"/> to be disposed with the object.
    /// </summary>
    /// <param name="disposableObjects"></param>
    /// <returns></returns>
    bool IDisposeAttachable.AttachDisposables(IEnumerable<object> disposableObjects)
    {
        // Argument error
        if (disposableObjects == null) throw new ArgumentNullException(nameof(disposableObjects));
        // Parent is disposed/ing
        if (IsDisposing)
        {
            // Captured errors
            StructList4<Exception> errors = new StructList4<Exception>();
            // Dispose now
            DisposeAndCapture(disposableObjects, ref errors);
            // Throw captured errors
            if (errors.Count > 0) throw ExceptionUtilities.Wrap(errors);
            // not ok
            return false;
        }

        // Add to list
        lock (attachmentsLock)
            foreach (object d in disposableObjects)
                if (d is IDisposable disposable)
                    attachments.Add(disposable);

        // Check parent again
        if (IsDisposing)
        {
            // Captured errors
            StructList4<Exception> errors = new StructList4<Exception>();
            // Dispose now
            DisposeAndCapture(disposableObjects, ref errors);
            // Remove
            lock (attachmentsLock) foreach (object d in disposableObjects) if (d is IDisposable disp) attachments.Remove(disp);
            // Throw captured errors
            if (errors.Count > 0) throw ExceptionUtilities.Wrap(errors);
            // not ok
            return false;
        }

        // OK
        return true;
    }

    /// <summary>
    /// Remove <paramref name="disposableObject"/> from list of attached disposables.
    /// </summary>
    /// <param name="disposableObject"></param>
    /// <returns>true if an item of <paramref name="disposableObject"/> was removed, false if it wasn't there</returns>
    bool IDisposeAttachable.RemoveDisposable(object disposableObject)
    {
        // Argument error
        if (disposableObject == null) throw new ArgumentNullException(nameof(disposableObject));
        // Cast to IDisposable
        IDisposable? disposable = disposableObject as IDisposable;
        // Was not IDisposable
        if (disposable == null) return false;
        // Remove from list
        lock (attachmentsLock)
        {
            return attachments.Remove(disposable);
        }
    }

    /// <summary>
    /// Remove <paramref name="disposableObjects"/> from the list. 
    /// </summary>
    /// <param name="disposableObjects"></param>
    /// <returns>true if was removed, false if it wasn't in the list.</returns>
    bool IDisposeAttachable.RemoveDisposables(IEnumerable<object> disposableObjects)
    {
        // Argument error
        if (disposableObjects == null) throw new ArgumentNullException(nameof(disposableObjects));

        bool ok = true;
        lock (this)
        {
            if (disposableObjects == null) return false;
            foreach (object disposableObject in disposableObjects)
                if (disposableObject is IDisposable disposable)
                    ok &= attachments.Remove(disposable);
            return ok;
        }
    }

    /// <summary>
    /// Dispose enumerable and capture errors
    /// </summary>
    /// <param name="disposableObjects">list of disposables</param>
    /// <param name="disposeErrors">list to be created if errors occur</param>
    public static void DisposeAndCapture(IEnumerable<object> disposableObjects, ref StructList4<Exception> disposeErrors)
    {
        if (disposableObjects == null) return;

        // Dispose disposables
        foreach (object disposableObject in disposableObjects)
        {
            if (disposableObject is IDisposable disposable)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (AggregateException ae)
                {
                    foreach (Exception e in ae.InnerExceptions)
                        disposeErrors.Add(e);
                }
                catch (Exception e)
                {
                    // Capture error
                    disposeErrors.Add(e);
                }
            }
        }
    }

    /// <summary>
    /// Dispose enumerable and capture errors
    /// </summary>
    /// <param name="disposableObjects">list of disposables</param>
    /// <param name="disposeErrors">list to be created if errors occur</param>
    public static void DisposeAndCapture<List>(ref List disposableObjects, ref StructList4<Exception> disposeErrors) where List : IList<IDisposable>
    {
        // Dispose disposables
        for (int i = 0; i < disposableObjects.Count; i++)
        {
            IDisposable disposable = disposableObjects[i];
            if (disposable != null)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (AggregateException ae)
                {
                    foreach (Exception e in ae.InnerExceptions)
                        disposeErrors.Add(e);
                }
                catch (Exception e)
                {
                    // Capture error
                    disposeErrors.Add(e);
                }
            }
        }
    }

}

