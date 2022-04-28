// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

/// <summary>Extension methods for <see cref="IDisposeAttachable"/>.</summary>
public static class DisposeAttachableExtensions
{
    /// <summary>Has <see cref="IDisposable.Dispose"/> been called whether or not internal dispose has started. Internal dispose starts when all belatables are disposed.</summary>
    public static bool IsDisposeCalled<T>(this T instance) where T : IDisposeAttachable => instance.IsDisposeCalled;
    /// <summary>Has internal dispose started or completed.</summary>
    public static bool IsDisposing<T>(this T instance) where T : IDisposeAttachable => instance.IsDisposing;
    /// <summary>Has dispose completed.</summary>
    public static bool IsDisposed<T>(this T instance) where T : IDisposeAttachable => instance.IsDisposed;

    /// <summary>
    /// Attach <paramref name="disposable"/> to be disposed along with this object.
    /// 
    /// If the implementing object has already been disposed, this method immediately disposes the <paramref name="disposable"/>.
    /// </summary>
    /// <param name="disposable"></param>
    public static T AttachDisposable<T>(this T instance, object disposable) where T : IDisposeAttachable { instance.AttachDisposable(disposable); return instance; }

    /// <summary>
    /// Attach <paramref name="disposableObjects"/> to be disposed along with this object.
    /// 
    /// If the implementing object has already been disposed, this method immediately disposes the <paramref name="disposableObjects"/>.
    /// </summary>
    /// <param name="disposableObjects"></param>
    public static T AttachDisposables<T>(this T instance, IEnumerable<object> disposableObjects) where T : IDisposeAttachable { instance.AttachDisposables(disposableObjects); return instance; }

    /// <summary>
    /// Remove <paramref name="disposableObject"/> from the attachables. 
    /// </summary>
    /// <param name="disposableObject"></param>
    public static T RemoveDisposable<T>(this T instance, object disposableObject) where T : IDisposeAttachable { instance.RemoveDisposable(disposableObject); return instance; }

    /// <summary>
    /// Remove <paramref name="disposableObjects"/> from the attachables 
    /// </summary>
    /// <param name="disposableObjects"></param>
    public static T RemoveDisposables<T>(this T instance, IEnumerable<object> disposableObjects) where T : IDisposeAttachable { instance.RemoveDisposables(disposableObjects); return instance; }

    /// <summary>
    /// Attach <paramref name="disposeAction"/> to be executed on dispose of this object.
    /// 
    /// If parent object is disposed or being disposed, the <paramref name="disposeAction"/> is executed immediately.
    /// </summary>
    /// <param name="disposeAction"></param>
    /// <param name="state"></param>
    public static T AttachDisposeAction<T>(this T instance, Action<object> disposeAction, object state) where T : IDisposeAttachable { instance.AttachDisposeAction(disposeAction, state); return instance; }


    /// <summary>
    /// Invoke <paramref name="disposeAction"/> on the dispose of the object.
    /// 
    /// If parent object is disposed or being disposed, the disposable will be disposed immedialy.
    /// </summary>
    /// <param name="disposeAction"></param>
    /// <returns>self</returns>
    public static T AddDisposeAction<T>(this T instanceT, Action<T> disposeAction) where T : IDisposeAttachable
    {
        // Adapt to IDisposable
        IDisposable disposable = new DisposeAction<T>(disposeAction, instanceT);
        // Add to list
        instanceT.AttachDisposable(disposable);
        // OK
        return instanceT;
    }

    /// <summary>Adapts <see cref="Action"/> into <see cref="IDisposable"/>.</summary>
    internal sealed class DisposeAction<T> : IDisposable
    {
        /// <summary>Action</summary>
        Action<T> action;
        /// <summary>Disposed object</summary>
        T disposeObject;

        /// <summary>Create dispose action.</summary>
        /// <param name="a"></param>
        /// <param name="disposeObject"></param>
        public DisposeAction(Action<T> a, T disposeObject)
        {
            this.action = a ?? throw new ArgumentNullException(nameof(a));
            this.disposeObject = disposeObject;
        }

        /// <summary>Run delegate with the attached object.</summary>
        public void Dispose() => action(disposeObject);
    }

}
