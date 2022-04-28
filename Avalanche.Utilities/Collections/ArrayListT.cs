// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Thread-safe <see cref="IList{T}"/> collection. 
/// 
/// List is modified under mutually exclusive lock <see cref="SyncRoot"/>.
/// 
/// <see cref="Array"/> creates a snapshot array, which won't throw <see cref="InvalidOperationException"/> 
/// if list is modified while being enumerated.
/// 
/// Modification can be detected by comparing the reference returned by <see cref="Array"/> property, as 
/// a new reference is created (lazily) when content has changed.
/// </summary>
/// <remarks><see cref="SnapshotProviderDecorator{T}"/> can be used for decorating <see cref="ArrayList{T}"/></remarks>
public abstract class ArrayListT : IList, ICollection
{
    /// <summary>constructor.</summary>
    static readonly ConstructorT<ArrayListT> constructor = new ConstructorT<ArrayListT>(typeof(ArrayList<>));
    /// <summary>Creates <see cref="ArrayList{T}"/>.</summary>
    public static ArrayListT Create(Type elementType) => constructor.Create(elementType);

    /// <summary>object that can be used to synchronize access</summary>
    public object SyncRoot => mLock;
    /// <summary>Sync root</summary>
    protected object mLock = new object();

    /// <inheritdoc/>
    object? IList.this[int index] { get => GetObject(index); set => SetObject(index, value); }
    /// <summary>Get element at <paramref name="index"/>.</summary>
    protected abstract object? GetObject(int index);
    /// <summary>Set element at <paramref name="index"/>.</summary>
    protected abstract void SetObject(int index, Object? value);

    /// <inheritdoc/>
    public abstract int Count { get; }
    /// <inheritdoc/>
    public abstract bool IsSynchronized { get; }
    /// <inheritdoc/>
    public abstract bool IsFixedSize { get; }
    /// <inheritdoc/>
    public abstract bool IsReadOnly { get; }
    /// <inheritdoc/>
    public abstract int Add(object? value);
    /// <inheritdoc/>
    public abstract void Clear();
    /// <inheritdoc/>
    public abstract bool Contains(object? value);
    /// <inheritdoc/>
    public abstract void CopyTo(Array array, int index);
    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => getEnumerator();
    /// <inheritdoc/>
    protected abstract IEnumerator getEnumerator();
    /// <inheritdoc/>
    public abstract int IndexOf(object? value);
    /// <inheritdoc/>
    public abstract void Insert(int index, object? value);
    /// <inheritdoc/>
    public abstract void Remove(object? value);
    /// <inheritdoc/>
    public abstract void RemoveAt(int index);
}

/// <summary>
/// Thread-safe <see cref="IList{T}"/> collection. 
/// 
/// List is modified under mutually exclusive lock <see cref="ArrayListT.SyncRoot"/>.
/// 
/// <see cref="Array"/> and <see cref="GetEnumerator"/> creates a snapshot array, which won't throw <see cref="InvalidOperationException"/> 
/// if list is modified while being enumerated.
/// </summary>
/// <remarks><see cref="SnapshotProviderDecorator{T}"/> can be used for decorating <see cref="ArrayList{T}"/></remarks>
public class ArrayList<T> : ArrayListT, IList<T>, IReadOnlyList<T>, ISnapshotProvider<T>, ICollection
{
    /// <summary>Get or set an element at <paramref name="index"/>.</summary>
    public T this[int index]
    {
        get => Array[index];
        set { lock (SyncRoot) { list[index] = value; ClearSnapshot(); } }
    }

    /// <summary>Get element at <paramref name="index"/>.</summary>
    protected override object? GetObject(int index) => Array[index];
    /// <summary>Set element at <paramref name="index"/>.</summary>
    protected override void SetObject(int index, object? value) { list[index] = (T)value!; ClearSnapshot(); }

    /// <summary>The number of elements.</summary>
    public override int Count
    {
        get
        {
            var _snaphost = snapshot;
            if (_snaphost != null) return _snaphost.Length;
            lock (SyncRoot) return list.Count;
        }
    }

    /// <summary>Is in readonly state</summary>
    public override bool IsReadOnly => false;
    /// <summary>Array snapshot of current contents. Makes a snapshot if retrieved after content is modified.</summary>
    public T[] Array { get => snapshot ?? BuildArray(); set { if (value == snapshot) return; lock (SyncRoot) { Clear(); AddRange(value); } } }
    /// <summary>Snapshot</summary>
    T[] ISnapshotProvider<T>.Snapshot { get => snapshot ?? BuildArray(); set { if (value == snapshot) return; lock (SyncRoot) { Clear(); AddRange(value); } } }
    /// <summary>Is synchronized</summary>
    public override bool IsSynchronized => true;

    /// <summary>Last snapshot. This snapshot is cleared when internal <see cref="list"/> is modified.</summary>
    protected T[]? snapshot;
    /// <summary>Internal list. Lazy allocation.</summary>
    protected List<T>? _list;
    /// <summary>Internal list. Lazy allocation.</summary>
    protected List<T> list { get { lock (SyncRoot) return _list ?? (_list = new List<T>()); } }

    /// <summary></summary>
    public override bool IsFixedSize => false;

    /// <summary>Create copy-on-write list</summary>
    public ArrayList() { }
    /// <summary>Create copy-on-write list with initial values from <paramref name="enumr"/>.</summary>
    public ArrayList(IEnumerable<T> enumr)
    {
        this._list = new List<T>(enumr);
    }

    /// <summary>Construct array snapshot</summary>
    protected virtual T[] BuildArray()
    {
        lock (SyncRoot)
        {
            return snapshot = list.Count == 0 ? System.Array.Empty<T>() : list.ToArray();
        }
    }

    /// <summary>Clear last array snapshot</summary>
    protected virtual void ClearSnapshot()
    {
        this.snapshot = null;
    }

    /// <summary>Add <paramref name="item"/>.</summary>
    public void Add(T item)
    {
        lock (SyncRoot)
        {
            list.Add(item);
            ClearSnapshot();
        }
    }

    /// <summary>Add <paramref name="item"/>.</summary>
    public override int Add(object? item)
    {
        lock (SyncRoot)
        {
            int ix = list.Count;
            list.Add((T)item!);
            ClearSnapshot();
            return ix;
        }
    }

    /// <summary>Add <paramref name="items"/>.</summary>
    public void AddRange(IEnumerable<T> items)
    {
        lock (SyncRoot)
        {
            list.AddRange(items);
            ClearSnapshot();
        }
    }

    /// <summary>Clear elements</summary>
    public override void Clear()
    {
        lock (SyncRoot)
        {
            if (list.Count == 0) return;
            list.Clear();
            ClearSnapshot();
        }
    }

    /// <summary>Test if contains <paramref name="item"/>.</summary>
    public bool Contains(T item)
    {
        // Get snapshot
        T[] snapshot = Array;
        // Try each
        for (int ix = 0; ix < snapshot.Length; ix++)
        {
            // Get element
            T element = snapshot[ix];
            // 
            if (element == null && item == null) return true;
            // 
            if (element == null) continue;
            // Compare
            if (element.Equals(item)) return true;
        }
        // Not found
        return false;
    }

    /// <summary>Test if contains <paramref name="item"/>.</summary>
    public override bool Contains(object? item)
    {
        // Get snapshot
        T[] snapshot = Array;
        // Try each
        for (int ix = 0; ix < snapshot.Length; ix++)
        {
            // Get element
            T element = snapshot[ix];
            // 
            if (element == null && item == null) return true;
            // 
            if (element == null) continue;
            // Compare
            if (element.Equals(item)) return true;
        }
        // Not found
        return false;
    }

    /// <summary>Copy elements to <paramref name="array"/> at <paramref name="arrayIndex"/>.</summary>
    public void CopyTo(T[] array, int arrayIndex)
    {
        // Get snapshot
        var snapshot = Array;
        // Copy
        System.Array.Copy(snapshot, 0, array, arrayIndex, snapshot.Length);
    }

    /// <summary>Copy elements to <paramref name="array"/> at <paramref name="arrayIndex"/></summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    public override void CopyTo(Array array, int arrayIndex)
    {
        // Get snapshot
        var snapshot = Array;
        // Copy
        System.Array.Copy(snapshot, 0, array, arrayIndex, snapshot.Length);
    }

    /// <summary>Find index of <paramref name="item"/>.</summary>
    /// <param name="item"></param>
    /// <returns>index or -1</returns>
    public int IndexOf(T item)
    {
        // Get snapshot
        var snapshot = Array;
        // Try each
        for (int ix = 0; ix < snapshot.Length; ix++)
        {
            // Get element
            T element = snapshot[ix];
            // 
            if (element == null && item == null) return ix;
            // 
            if (element == null) continue;
            // Compare
            if (element.Equals(item)) return ix;
        }
        // Not func
        return -1;
    }

    /// <summary>Find index of <paramref name="item"/>.</summary>
    /// <param name="item"></param>
    /// <returns>index or -1</returns>
    public override int IndexOf(object? item)
    {
        // Cast
        if (item is not T value) return -1;
        // Get snapshot
        var snapshot = Array;
        // Try each
        for (int ix = 0; ix < snapshot.Length; ix++)
        {
            // Get element
            T element = snapshot[ix];
            // 
            if (element == null && item == null) return ix;
            // 
            if (element == null) continue;
            // Compare
            if (element.Equals(value)) return ix;
        }
        // Not func
        return -1;
    }

    /// <summary>Insert <paramref name="item"/> at <paramref name="index"/>.</summary>
    public void Insert(int index, T item)
    {
        lock (SyncRoot)
        {
            list.Insert(index, item);
            ClearSnapshot();
        }
    }

    /// <summary>Insert <paramref name="item"/> at <paramref name="index"/>.</summary>
    public override void Insert(int index, object? item)
    {
        lock (SyncRoot)
        {
            list.Insert(index, (T)item!);
            ClearSnapshot();
        }
    }

    /// <summary>Copy elements from <paramref name="srcElements"/>.</summary>
    public void CopyFrom(IEnumerable<T> srcElements)
    {
        lock (SyncRoot)
        {
            list.Clear();
            list.AddRange(srcElements);
            ClearSnapshot();
        }
    }

    /// <summary>Reverse elements.</summary>
    public void Reverse()
    {
        lock (SyncRoot)
        {
            // Nothing to do
            if (list.Count <= 1) return;
            int mid = list.Count / 2;
            for (int i = 0, j = list.Count - 1; i < mid; i++, j--)
            {
                // Swap list[i] and list[j]
                T tmp = list[i];
                list[i] = list[j];
                list[j] = tmp;
            }
            ClearSnapshot();
        }
    }

    /// <summary>Remove <paramref name="item"/></summary>
    /// <returns>true if was found</returns>
    public bool Remove(T item)
    {
        lock (SyncRoot)
        {
            bool wasRemoved = list.Remove(item);
            if (wasRemoved) ClearSnapshot();
            return wasRemoved;
        }
    }

    /// <summary>Remove <paramref name="item"/></summary>
    /// <returns>true if was found</returns>
    public override void Remove(object? item)
    {
        // Cast
        if (item is not T value) return;

        lock (SyncRoot)
        {
            bool wasRemoved = list.Remove(value);
            if (wasRemoved) ClearSnapshot();
        }
    }

    /// <summary>Remove element at <paramref name="index"/>.</summary>
    public override void RemoveAt(int index)
    {
        lock (SyncRoot)
        {
            list.RemoveAt(index);
            ClearSnapshot();
        }
    }

    /// <summary>Get enumerator to a snapsot list. Enumerator will not throw <see cref="InvalidOperationException"/>.</summary>
    protected override IEnumerator getEnumerator() => Array.GetEnumerator();
    /// <summary>Get enumerator to a snapsot list. Enumerator not throw <see cref="InvalidOperationException"/>.</summary>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)Array).GetEnumerator();
    /// <summary>Print information</summary>
    public override string ToString() => $"ArrayList(Count={Count})";

    /// <summary>Version of <see cref="ArrayList"/> where values are sorted after build.</summary>
    public class Sorted : ArrayList<T>
    {
        /// <summary>Comparer used for sorting.</summary>
        public readonly IComparer<T> Comparer;

        /// <summary>Create array where values are sorted with <paramref name="comparer"/>.</summary>
        public Sorted(IComparer<T> comparer) : base()
        {
            this.Comparer = comparer;
        }

        /// <summary>Build array and sort.</summary>
        protected override T[] BuildArray()
        {
            // Build array
            T[] newArray = base.BuildArray();
            // Sort
            System.Array.Sort(newArray, Comparer);
            //
            return newArray;
        }
    }
}
