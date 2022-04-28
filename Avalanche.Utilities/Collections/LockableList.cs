// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

/// <summary>List that can be locked into immutable state.</summary>
public abstract class LockableList : IReadOnly, IList
{
    /// <summary></summary>
    static readonly ConstructorT<LockableList> constructor = new(typeof(LockableList<>));
    /// <summary></summary>
    static readonly ConstructorT<int, LockableList> constructor2 = new(typeof(LockableList<>));
    /// <summary></summary>
    static readonly ConstructorT<IList, LockableList> constructor3 = new(typeof(LockableList<>._));
    /// <summary></summary>
    public static LockableList Create(Type elementType) => constructor.Create(elementType);
    /// <summary></summary>
    public static LockableList Create(Type elementType, int capacity) => constructor2.Create(elementType, capacity);
    /// <summary></summary>
    public static LockableList Create(Type elementType, IList internalList) => constructor3.Create(elementType, internalList);

    /// <summary>Internal list</summary>
    protected IList list = null!;
    /// <summary>Is read-only state</summary>
    protected bool @readonly;
    /// <summary></summary>
    protected bool isSynchronized;
    /// <summary></summary>
    protected object syncRoot = null!;
    /// <summary>Is read-only state</summary>
    [IgnoreDataMember] bool IReadOnly.ReadOnly { get => @readonly; set { if (@readonly == value) return; if (!value) throw new InvalidOperationException("Read-only"); @readonly = value; } }
    /// <summary>Assert writable</summary>
    protected LockableList AssertWritable => @readonly ? throw new InvalidOperationException("Read-only") : this;

    /// <summary></summary>
    public abstract Type ElementType { get; }
    /// <summary></summary>
    public virtual bool IsFixedSize => @readonly || list.IsFixedSize;
    /// <summary></summary>
    public virtual bool IsReadOnly => @readonly || list.IsReadOnly;
    /// <summary></summary>
    public virtual int Count => list.Count;
    /// <summary></summary>
    public virtual bool IsSynchronized => isSynchronized;
    /// <summary></summary>
    public virtual object SyncRoot => syncRoot;
    /// <summary></summary>
    public virtual object? this[int index] { get => AssertWritable.list[index]; set => AssertWritable.list[index] = value; }
    /// <summary></summary>
    public virtual int Add(object? value) => AssertWritable.list.Add(value);
    /// <summary></summary>
    public virtual void Clear() => AssertWritable.list.Clear();
    /// <summary></summary>
    public virtual bool Contains(object? value) => list.Contains(value);
    /// <summary></summary>
    public virtual int IndexOf(object? value) => list.IndexOf(value);
    /// <summary></summary>
    public virtual void Insert(int index, object? value) => AssertWritable.list.Insert(index, value);
    /// <summary></summary>
    public virtual void Remove(object? value) => AssertWritable.list.Remove(value);
    /// <summary></summary>
    public virtual void RemoveAt(int index) => AssertWritable.list.RemoveAt(index);
    /// <summary></summary>
    public virtual void CopyTo(Array array, int index) => list.CopyTo(array, index);
    /// <summary></summary>
    public virtual IEnumerator GetEnumerator() => list.GetEnumerator();
}

/// <summary>List that can be locked into immutable state.</summary>
public class LockableList<T> : LockableList, IList<T>
{
    /// <summary>Internal list</summary>
    protected new IList<T> list;

    /// <summary></summary>
    public override Type ElementType => typeof(T);

    /// <summary></summary>
    public LockableList() 
    { 
        list = new List<T>(); 
        base.list = (list as IList)!;
        base.syncRoot = (list as ICollection)?.SyncRoot ?? new object();
        base.isSynchronized = false;
    }

    /// <summary></summary>
    public LockableList(int capacity) : base() 
    { 
        list = new List<T>(capacity); 
        base.list = (list as IList)!;
        base.syncRoot = (list as ICollection)?.SyncRoot ?? new object();
        base.isSynchronized = false;
    }

    /// <summary></summary>
    public LockableList(IList<T> internalList) : base() 
    { 
        list = internalList ?? throw new ArgumentNullException(nameof(internalList)); 
        base.list = (list as IList)!;
        base.syncRoot = (internalList as ICollection)?.SyncRoot ?? new object();
        base.isSynchronized = internalList is ICollection collection ? collection.IsSynchronized : false;
    }

    /// <summary>Assert writable</summary>
    protected new LockableList<T> AssertWritable => @readonly ? throw new InvalidOperationException("Read-only") : this;

    /// <summary></summary>
    public override int Count => list.Count;
    /// <summary></summary>
    public override bool IsReadOnly => @readonly || list.IsReadOnly;
    /// <summary></summary>
    public override object? this[int index] { get => list[index]; set => AssertWritable.list[index] = (T)value!; }

    /// <summary></summary>
    T IList<T>.this[int index] { get => list[index]; set => AssertWritable.list[index] = value; }
    /// <summary></summary>
    int IList<T>.IndexOf(T item) => list.IndexOf(item);
    /// <summary></summary>
    public void Insert(int index, T item) => AssertWritable.list.Insert(index, item);
    /// <summary></summary>
    void IList<T>.RemoveAt(int index) => AssertWritable.list.RemoveAt(index);
    /// <summary></summary>
    public void Add(T item) => AssertWritable.list.Add(item);
    /// <summary></summary>
    public override void Clear() => AssertWritable.list.Clear();
    /// <summary></summary>
    bool ICollection<T>.Contains(T item) => list.Contains(item);
    /// <summary></summary>
    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
    /// <summary></summary>
    bool ICollection<T>.Remove(T item) => AssertWritable.list.Remove(item);
    /// <summary></summary>
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => list.GetEnumerator();

    /// <summary>Workaround</summary>
    public class _ : LockableList<T>
    {
        /// <summary></summary>
        public _(IList? internalList) : base(internalList != null ? (IList<T>)internalList : new List<T>())
        {
        }
    }

}


