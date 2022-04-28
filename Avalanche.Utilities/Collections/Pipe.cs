// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

/// <summary></summary>
public abstract class Pipe : IEnumerable, IDisposable
{
    /// <summary>Constructor</summary>
    static readonly ConstructorT<Pipe> constructor = new(typeof(Pipe<>));
    /// <summary>Create pipe</summary>
    public static Pipe Create(Type elementType) => constructor.Create(elementType);

    /// <summary>Dispose state, 0-not disposed, 1-disposing, 2-disposed</summary>
    protected long disposed = 0L;
    /// <summary>Test if is disposing or disposed.</summary>
    protected bool IsDisposing => Interlocked.Read(ref disposed) >= 1L;
    /// <summary>Test if is disposed.</summary>
    protected bool IsDisposed => Interlocked.Read(ref disposed) >= 2L;

    /// <summary>Element Type</summary>
    public abstract Type ElementType { get; }

    /// <summary></summary>
    public abstract void Dispose();
    /// <summary></summary>
    public IEnumerator GetEnumerator() => getEnumerator();
    /// <summary></summary>
    protected abstract IEnumerator getEnumerator();
}

/// <summary>
/// Pipe is a concurrent stream of elements that can be simultaneously populated with
/// elements, can be enumerated, and be subscribed. 
/// 
/// Enumeration ends once <see cref="Dispose"/> is called. The remaning, non-retrieved elements
/// are returned. 
/// </summary>
/// <typeparam name="T"></typeparam>
public class Pipe<T> : Pipe, IEnumerable<T>, IObservable<T>, IProducerConsumerCollection<T>
{
    /// <summary>The backed object.</summary>
    ICollection collection;
    /// <summary>The backed object casted to <![CDATA[ICollection<T>]]>, if castable.</summary>
    ICollection<T>? collection_T;
    /// <summary>The backed object casted to <![CDATA[IEnumerable<T>]]>, if castable</summary>
    IEnumerable<T> enumr;
    /// <summary>The backed object casted to <![CDATA[IList<T>]]>, if castable.</summary>
    IList<T>? list;
    /// <summary>List of observers</summary>
    ArrayList<IObserver<T>> observers = new ArrayList<IObserver<T>>();
    /// <summary>Delegate the observers call when they unsubscribe.</summary>
    Action<IObserver<T>> unsubscribeAction;
    /// <summary>lock object</summary>
    object monitor = new object();

    /// <summary>Element Type</summary>
    public override Type ElementType => typeof(T);
    /// <summary>Count</summary>
    public int Count => collection.Count;
    /// <summary>Is synchronizable</summary>
    public bool IsSynchronized => true;
    /// <summary>Synchronize object</summary>
    public object SyncRoot => monitor;

    /// <summary>Create a new pipe.</summary>
    public Pipe()
    {
        Queue<T> queue = new Queue<T>();
        this.collection = queue;
        this.collection_T = null;
        this.enumr = queue;
        this.unsubscribeAction = Unsubscribe;
    }

    /// <summary>Create a new pipe where elements are added to <paramref name="collection"/>.</summary>
    /// <param name="collection">backend object where elements are added</param>
    public Pipe(ICollection<T> collection)
    {
        this.enumr = collection;
        this.collection = (ICollection)collection;
        this.collection_T = collection;
        this.list = collection as IList<T>;
        this.unsubscribeAction = Unsubscribe;
    }

    /// <summary>Subscribe to receive new elements.</summary>
    /// <param name="observer"></param>
    /// <returns></returns>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        observers.Add(observer);
        return new _observerHandle<T>(unsubscribeAction, observer);
    }

    /// <summary>Unsubscribe observer.</summary>
    /// <param name="observer"></param>
    void Unsubscribe(IObserver<T> observer)
    {
        bool removed;
        removed = observers.Remove(observer);
        if (removed) observer.OnCompleted();
    }

    /// <summary>
    /// Close the pipe. 
    /// 
    /// Doesn't make it completely unusable, but closes observers.
    /// </summary>
    /// <exception cref="AggregateException">If observer throws an exception</exception>
    public override void Dispose()
    {
        // Is disposing
        Interlocked.CompareExchange(ref disposed, 1L, 0L);

        // Clear and notify observers
        IObserver<T>[] _observers;
        lock (observers.SyncRoot)
        {
            _observers = observers.Array;
            observers.Clear();
        }
        StructList1<Exception> errors = new StructList1<Exception>();
        foreach (IObserver<T> o in _observers)
        {
            try
            {
                o.OnCompleted();
            }
            catch (Exception e)
            {
                errors.Add(e);
            }
        }

        // Is disposed
        Interlocked.CompareExchange(ref disposed, 2L, 1L);
        // Suppress further finalize
        GC.SuppressFinalize(this);
        // Rethrow
        if (errors.Count > 0) throw new AggregateException(errors);
    }

    /// <summary>Copy elements to <paramref name="array"/>.</summary>
    /// <param name="array"></param>
    /// <param name="index"></param>
    public void CopyTo(T[] array, int index)
    {
        lock (collection) collection.CopyTo(array, index);
    }

    /// <summary>Copy elements to <paramref name="array"/>.</summary>
    /// <param name="array"></param>
    /// <param name="index"></param>
    public void CopyTo(Array array, int index)
    {
        lock (collection) ((ICollection)collection).CopyTo(array, index);
    }

    /// <summary>Get snapshot of elements.</summary>
    public T[] ToArray()
    {
        lock (collection)
        {
            if (collection is IProducerConsumerCollection<T> _coll)
                return _coll.ToArray();
            return enumr.ToArray();
        }
    }

    /// <summary></summary>
    public new IEnumerator<T> GetEnumerator()
    {
        _pipeEnumerator<T> etor = new _pipeEnumerator<T>();
        lock (collection)
        {
            if (IsDisposing)
            {
                etor.AddRange(enumr);
                etor.OnCompleted();
            }
            else
            {
                etor.subscriptionHandle = Subscribe(etor);
                etor.AddRange(enumr);
            }
        }
        return etor;
    }

    /// <summary></summary>
    protected override IEnumerator getEnumerator()
    {
        _pipeEnumerator<T> etor = new _pipeEnumerator<T>();
        lock (collection)
        {
            if (IsDisposing)
            {
                etor.AddRange(enumr);
                etor.OnCompleted();
            }
            else
            {
                etor.subscriptionHandle = Subscribe(etor);
                etor.AddRange(enumr);
            }
        }
        return etor;
    }

    /// <summary>Try add new element.</summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool TryAdd(T item)
    {
        lock (collection)
        {
            if (collection_T != null)
            {
                collection_T.Add(item);
            }

            else if (collection is Queue<T> queue)
            {
                queue.Enqueue(item);
            }

            else if (collection is IProducerConsumerCollection<T> queue_)
            {
                bool ok = queue_.TryAdd(item);
                if (!ok) return ok;
            }

            else throw new InvalidOperationException($"Cannot add to {collection.GetType().FullName}.");
        }

        // Notify observers
        IObserver<T>[] _observers = observers.Array;
        StructList1<Exception> errors = new StructList1<Exception>();
        if (_observers.Length > 0)
        {
            foreach (IObserver<T> observer in _observers)
            {
                try
                {
                    observer.OnNext(item);
                }
                catch (Exception e)
                {
                    errors.Add(e);
                }
            }
        }

        if (errors.Count > 0) throw new AggregateException(errors);
        return true;
    }

    /// <summary>Try remove one element.</summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool TryTake([MaybeNullWhen(false)] out T item)
    {
        lock (collection)
        {
            int count = collection.Count;
            if (count == 0) { item = default!; return false; }

            if (collection is Queue<T> queue)
            {
                item = queue.Dequeue();
                return true;
            }

            if (collection is IProducerConsumerCollection<T> queue_)
            {
                bool ok = queue_.TryTake(out item);
                return ok;
            }

            if (list != null)
            {
                item = list[0];
                list.RemoveAt(0);
                return true;
            }

            if (collection_T != null)
            {
                using (var etor = collection_T.GetEnumerator())
                {
                    bool ok = etor.MoveNext();
                    if (!ok) { item = default!; return false; }
                    T _item = etor.Current;
                    item = _item;
                    collection_T.Remove(_item);
                    return true;
                }
            }

            throw new InvalidOperationException($"Could not remove element from {collection.GetType().FullName}.");
        }
    }

}

internal class _pipeEnumerator<T> : IEnumerator<T>, IObserver<T>
{
    T current;
    public T Current => current;
    object? IEnumerator.Current => current;
    object monitor = new object();
    int ix = -1;
    internal IDisposable? subscriptionHandle;
    List<T> list;
    long disposed = 0L; // 0-streaming, 1-stream closed, 2-isdisposing, 3-isdisposed

    bool IsClosed => Interlocked.Read(ref disposed) >= 1L;
    bool IsDisposing => Interlocked.Read(ref disposed) >= 2L;
    bool IsDisposed => Interlocked.Read(ref disposed) >= 3L;

    public _pipeEnumerator()
    {
        this.list = new List<T>();
        this.current = default!;
    }

    /// <summary></summary>
    public void AddRange(IEnumerable<T> values)
    {
        lock (monitor)
            this.list.AddRange(values);
    }

    /// <summary>Dispose subscription handle. Dispose only once.</summary>
    public void Dispose()
    {
        // IsClosed
        Interlocked.CompareExchange(ref disposed, 1L, 0L);
        // IsDisposing
        Interlocked.CompareExchange(ref disposed, 2L, 1L);

        // Cancel subscription
        try
        {
            Interlocked.CompareExchange(ref subscriptionHandle, null, subscriptionHandle)?.Dispose();
        }
        catch (Exception) { }

        // Clear list and wakeup
        lock (monitor)
        {
            list.Clear();
            Monitor.PulseAll(monitor);
        }

        // IsDisposed
        Interlocked.CompareExchange(ref disposed, 3L, 2L);
    }

    /// <summary></summary>
    public bool MoveNext()
    {
        lock (monitor)
        {
            while (true)
            {
                // Is enumerator disposed
                if (IsDisposing) return false;

                if (ix < list.Count - 1)
                {
                    ix++;
                    current = list[ix];
                    return true;
                }

                // Is parent disposed
                if (IsClosed) return false;

                // Sleep and release monitor
                Monitor.Wait(monitor);
            }
        }
    }

    /// <summary></summary>
    public void OnCompleted()
    {
        // No more entries.
        Interlocked.CompareExchange(ref disposed, 1L, 0L);

        // Wakeup threads
        lock (monitor) Monitor.PulseAll(monitor);
    }

    /// <summary></summary>
    public void OnError(Exception error) { }

    /// <summary></summary>
    public void OnNext(T value)
    {
        if (IsDisposing) return;
        lock (monitor)
        {
            if (IsDisposing) return;
            list.Add(value);
            Monitor.Pulse(monitor);
        }
    }

    /// <summary></summary>
    public void Reset()
    {
        lock (monitor) ix = -1;
    }
}

/// <summary>A disposable handle that represents one subscription of <typeparamref name="T"/> from <see cref="IObservable{T}"/>.</summary>
internal class _observerHandle<T> : IDisposable
{
    /// <summary>Unsubscribe action</summary>
    Action<IObserver<T>>? unsubscribeAction;

    /// <summary>Subscribed observer</summary>
    IObserver<T>? observer;

    /// <summary>
    /// 0 - not disposed
    /// 1 - is disposed
    /// </summary>
    long disposing = 0L;

    /// <summary>Tests if handle has been disposed.</summary>
    public bool IsDisposed => Interlocked.Read(ref disposing) != 0L;

    /// <summary>Create handle</summary>
    public _observerHandle(Action<IObserver<T>> unsubscribeAction, IObserver<T> observer)
    {
        this.unsubscribeAction = unsubscribeAction;
        this.observer = observer ?? throw new ArgumentNullException(nameof(observer));
    }

    /// <summary>Dispose handle which will unsubscribe the observer.</summary>
    public void Dispose()
    {
        // Only one thread can unsubscribe, and only once
        if (Interlocked.CompareExchange(ref disposing, 1L, 0L) != 0L) return;

        // Clear referenes
        var _unsubscribeAction = unsubscribeAction;
        var _observer = observer;
        unsubscribeAction = null;
        observer = null;

        // Run action
        _unsubscribeAction!(_observer!);
    }
}
