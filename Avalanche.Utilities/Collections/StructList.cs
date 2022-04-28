// -----------------------------------------------------------------
// Copyright:      Toni Kalajainen 
// Date:           19.3.2019
// -----------------------------------------------------------------
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>A list where the first 1 element(s) are stack allocated, and rest are allocated from heap when needed.</summary>
/// <typeparam name="T"></typeparam>
public struct StructList1<T> : IList<T>
{
    /// <summary>The number of elements that are stack allocated.</summary>
    public int StackCount => 1;

    /// <summary>Number of elements in the list</summary>
    public int Count => count;
    /// <summary>Is list readonly</summary>
    public bool IsReadOnly => false;

    /// <summary>Number of elements</summary>
    int count;
    /// <summary>First elements</summary>
    T _0;
    /// <summary>Elements after <see cref="StackCount"/>.</summary>
    List<T>? rest;
    /// <summary>Element comparer</summary>
    IEqualityComparer<T>? elementComparer;
    /// <summary>Element comparer</summary>
    public IEqualityComparer<T> ElementComparer => elementComparer ?? EqualityComparer<T>.Default;

    /// <summary>Create struct list.</summary>
    /// <param name="elementComparer"></param>
    public StructList1(IEqualityComparer<T>? elementComparer = null)
    {
        this.elementComparer = elementComparer ?? EqualityComparer<T>.Default;
        count = 0;

        _0 = default!;
        rest = null;
    }

    /// <summary>Create struct list.</summary>
    /// <param name="enumr"></param>
    public StructList1(IEnumerable<T> enumr) : this(elementComparer: default)
    {
        foreach(T value in enumr) Add(value);
    }

    /// <summary>Gets or sets the element at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>element</returns>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList1`1.</exception>
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: return _0;
                default: return rest![index - StackCount];
            }
        }
        set
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: _0 = value; return;
                default: rest![index - StackCount] = value; return;
            }
        }
    }

    /// <summary>Get reference to element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static ref T GetRef(ref StructList1<T> list, int index)
    {
        if (index < 0 || index >= list.Count) throw new ArgumentOutOfRangeException();
        switch (index)
        {
            case 0: return ref list._0;
            default:
                Span<T> span = CollectionsMarshal.AsSpan<T>(list.rest);
                return ref span[index - 1];
        }
    }

    /// <summary>Add <paramref name="item"/> to the StructList1`1.</summary>
    /// <exception cref="System.NotSupportedException">The StructList1`1 is read-only.</exception>
    public void Add(T item)
    {
        switch(count)
        {
            case 0: _0 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }

        count++;
        return;
    }

    /// <summary>Adds <paramref name="items"/>.</summary>
    /// <exception cref="System.NotSupportedException">The StructList1`1 is read-only.</exception>
    public void AddRange(IEnumerable<T> items)
    {
        foreach(T item in items)
        {
            switch(count)
            {
                case 0: _0 = item; break;
                default:
                    if (rest == null) rest = new List<T>();
                    rest.Add(item);
                    break;
            }
            count++;
        }
    }

    /// <summary>Add <paramref name="item"/>, if the item isn't already in the list.</summary>
    /// <exception cref="System.NotSupportedException">The StructList1`1 is read-only.</exception>
    public void AddIfNew(T item)
    {
        if (Contains(item)) return;
        switch(count)
        {
            case 0: _0 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }
        count++; 
    }

    /// <summary>Removes the first occurrence of <paramref name="item"/>.</summary>
    /// <returns>true if item was successfully removed from the StructList1`1; otherwise, false. This method also returns false if item is not found in the original StructList1`1.</returns>
    public bool Remove(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) { RemoveAt(0); return true; }

        if (rest == null) return false;
        foreach(T e in rest) if (comparer.Equals(e, item)) { bool removed = rest.Remove(item); if (removed) count--; return removed; }
        return false;
    }

    /// <summary>Removes element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList1`1.</exception>
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
        if (index <= 0 && count > 1) { _0 = rest![0]; rest!.RemoveAt(0); }
        if (index >= StackCount) rest!.RemoveAt(index - StackCount);
        count--;
    }

    /// <summary>Removes and returns the element at the end of the list.</summary>
    /// <returns>the last element</returns>
    /// <exception cref="InvalidOperationException">If list is empty</exception>
    public T Dequeue()
    {
        if (count == 0) throw new InvalidOperationException();
        int ix = count - 1;
        T result = this[ix];
        RemoveAt(ix);
        return result;
    }

    /// <summary>Remove all elements.</summary>
    /// <exception cref="System.NotSupportedException">The StructList1`1 is read-only.</exception>
    public void Clear()
    {
        if (count >= 1) _0 = default!;
        if (rest != null) rest.Clear();
        count = 0;
    }

    /// <summary>Determine whether <paramref name="item"/> is in the list.</summary>
    /// <returns>true if item is found in the StructList1`1; otherwise, false.</returns>
    public bool Contains(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return true;
        if (rest != null) return rest.Contains(item);
        return false;
    }

    /// <summary>Determines the index of <paramref name="item"/>.</summary>
    /// <returns>The index of item if found in the list; otherwise, -1.</returns>
    public int IndexOf(T item)
    {
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return 0;
        if (rest != null) return rest.IndexOf(item)-StackCount;
        return -1;
    }

    /// <summary>Inserts an <paramref name="item"/> to the StructList1`1 at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="item">The object to insert.</param>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList1`1.</exception>
    public void Insert(int index, T item)
    {
        if (index < 0 || index > count) throw new ArgumentOutOfRangeException();
        if (index >= 1) { if (rest == null) rest = new List<T>(); rest.Insert(index - StackCount, item); }
        if (index <= 0 && count >= 1) { if (rest == null) rest = new List<T>(); rest.Insert(0, _0); }

        count++;
        this[index] = item;
    }

    /// <summary>Copies the elements to <paramref name="array"/>, starting at <paramref name="arrayIndex"/>.</summary>
    /// <param name="array">The one-dimensional System.Array.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <exception cref="System.ArgumentNullException">array is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
    /// <exception cref="System.ArgumentException">The number of elements in the source StructList1`1 is greater than the available space from arrayIndex to the end of the destination array.</exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
        if (count > array.Length + arrayIndex) throw new ArgumentException();

        if (count >= 1) array[arrayIndex++] = _0;
        if (rest != null) rest.CopyTo(array, arrayIndex);
    }

    /// <summary>Create array.</summary>
    public T[] ToArray()
    {
        // Return empty singleton
        if (count == 0) return Array.Empty<T>();
        // Create array
        T[] result = new T[count];
        if (count >= 1) result[0] = _0;
        if (count > 1)
        {
            for (int i = 1; i < count; i++)
                result[i] = rest![i-1];
        }
        // Return array
        return result;
    }

    /// <summary>Create array with elements reversed.</summary>
    public T[] ToReverseArray()
    {
        T[] result = new T[count];
        if (count >= 1) result[count-1] = _0;
        if (count > 1)
        {
            for (int i = 1; i < count; i++)
                result[count-1-i] = rest![i-1];
        }
        return result;
    }

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)ToArray()).GetEnumerator();

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)ToArray()).GetEnumerator();
}


/// <summary>A list where the first 2 element(s) are stack allocated, and rest are allocated from heap when needed.</summary>
/// <typeparam name="T"></typeparam>
public struct StructList2<T> : IList<T>
{
    /// <summary>The number of elements that are stack allocated.</summary>
    public int StackCount => 2;

    /// <summary>Number of elements in the list</summary>
    public int Count => count;
    /// <summary>Is list readonly</summary>
    public bool IsReadOnly => false;

    /// <summary>Number of elements</summary>
    int count;
    /// <summary>First elements</summary>
    T _0, _1;
    /// <summary>Elements after <see cref="StackCount"/>.</summary>
    List<T>? rest;
    /// <summary>Element comparer</summary>
    IEqualityComparer<T>? elementComparer;
    /// <summary>Element comparer</summary>
    public IEqualityComparer<T> ElementComparer => elementComparer ?? EqualityComparer<T>.Default;

    /// <summary>Create struct list.</summary>
    /// <param name="elementComparer"></param>
    public StructList2(IEqualityComparer<T>? elementComparer = null)
    {
        this.elementComparer = elementComparer ?? EqualityComparer<T>.Default;
        count = 0;

        _0 = default!;
        _1 = default!;
        rest = null;
    }

    /// <summary>Create struct list.</summary>
    /// <param name="enumr"></param>
    public StructList2(IEnumerable<T> enumr) : this(elementComparer: default)
    {
        foreach(T value in enumr) Add(value);
    }

    /// <summary>Gets or sets the element at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>element</returns>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList2`1.</exception>
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: return _0;
                case 1: return _1;
                default: return rest![index - StackCount];
            }
        }
        set
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: _0 = value; return;
                case 1: _1 = value; return;
                default: rest![index - StackCount] = value; return;
            }
        }
    }

    /// <summary>Get reference to element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static ref T GetRef(ref StructList2<T> list, int index)
    {
        if (index < 0 || index >= list.Count) throw new ArgumentOutOfRangeException();
        switch (index)
        {
            case 0: return ref list._0;
            case 1: return ref list._1;
            default:
                Span<T> span = CollectionsMarshal.AsSpan<T>(list.rest);
                return ref span[index - 2];
        }
    }

    /// <summary>Add <paramref name="item"/> to the StructList2`1.</summary>
    /// <exception cref="System.NotSupportedException">The StructList2`1 is read-only.</exception>
    public void Add(T item)
    {
        switch(count)
        {
            case 0: _0 = item; break;
            case 1: _1 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }

        count++;
        return;
    }

    /// <summary>Adds <paramref name="items"/>.</summary>
    /// <exception cref="System.NotSupportedException">The StructList2`1 is read-only.</exception>
    public void AddRange(IEnumerable<T> items)
    {
        foreach(T item in items)
        {
            switch(count)
            {
                case 0: _0 = item; break;
                case 1: _1 = item; break;
                default:
                    if (rest == null) rest = new List<T>();
                    rest.Add(item);
                    break;
            }
            count++;
        }
    }

    /// <summary>Add <paramref name="item"/>, if the item isn't already in the list.</summary>
    /// <exception cref="System.NotSupportedException">The StructList2`1 is read-only.</exception>
    public void AddIfNew(T item)
    {
        if (Contains(item)) return;
        switch(count)
        {
            case 0: _0 = item; break;
            case 1: _1 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }
        count++; 
    }

    /// <summary>Removes the first occurrence of <paramref name="item"/>.</summary>
    /// <returns>true if item was successfully removed from the StructList2`1; otherwise, false. This method also returns false if item is not found in the original StructList2`1.</returns>
    public bool Remove(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) { RemoveAt(0); return true; }
        if (count >= 2 && comparer.Equals(_1, item)) { RemoveAt(1); return true; }

        if (rest == null) return false;
        foreach(T e in rest) if (comparer.Equals(e, item)) { bool removed = rest.Remove(item); if (removed) count--; return removed; }
        return false;
    }

    /// <summary>Removes element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList2`1.</exception>
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
        if (index <= 0 && count > 1) _0 = _1;
        if (index <= 1 && count > 2) { _1 = rest![0]; rest!.RemoveAt(0); }
        if (index >= StackCount) rest!.RemoveAt(index - StackCount);
        count--;
    }

    /// <summary>Removes and returns the element at the end of the list.</summary>
    /// <returns>the last element</returns>
    /// <exception cref="InvalidOperationException">If list is empty</exception>
    public T Dequeue()
    {
        if (count == 0) throw new InvalidOperationException();
        int ix = count - 1;
        T result = this[ix];
        RemoveAt(ix);
        return result;
    }

    /// <summary>Remove all elements.</summary>
    /// <exception cref="System.NotSupportedException">The StructList2`1 is read-only.</exception>
    public void Clear()
    {
        if (count >= 1) _0 = default!;
        if (count >= 2) _1 = default!;
        if (rest != null) rest.Clear();
        count = 0;
    }

    /// <summary>Determine whether <paramref name="item"/> is in the list.</summary>
    /// <returns>true if item is found in the StructList2`1; otherwise, false.</returns>
    public bool Contains(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return true;
        if (count >= 2 && comparer.Equals(_1, item)) return true;
        if (rest != null) return rest.Contains(item);
        return false;
    }

    /// <summary>Determines the index of <paramref name="item"/>.</summary>
    /// <returns>The index of item if found in the list; otherwise, -1.</returns>
    public int IndexOf(T item)
    {
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return 0;
        if (count >= 2 && comparer.Equals(_1, item)) return 1;
        if (rest != null) return rest.IndexOf(item)-StackCount;
        return -1;
    }

    /// <summary>Inserts an <paramref name="item"/> to the StructList2`1 at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="item">The object to insert.</param>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList2`1.</exception>
    public void Insert(int index, T item)
    {
        if (index < 0 || index > count) throw new ArgumentOutOfRangeException();
        if (index >= 2) { if (rest == null) rest = new List<T>(); rest.Insert(index - StackCount, item); }
        if (index <= 1 && count >= 2) { if (rest == null) rest = new List<T>(); rest.Insert(0, _1); }
        if (index <= 0 && count >= 1) _1 = _0;

        count++;
        this[index] = item;
    }

    /// <summary>Copies the elements to <paramref name="array"/>, starting at <paramref name="arrayIndex"/>.</summary>
    /// <param name="array">The one-dimensional System.Array.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <exception cref="System.ArgumentNullException">array is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
    /// <exception cref="System.ArgumentException">The number of elements in the source StructList2`1 is greater than the available space from arrayIndex to the end of the destination array.</exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
        if (count > array.Length + arrayIndex) throw new ArgumentException();

        if (count >= 1) array[arrayIndex++] = _0;
        if (count >= 2) array[arrayIndex++] = _1;
        if (rest != null) rest.CopyTo(array, arrayIndex);
    }

    /// <summary>Create array.</summary>
    public T[] ToArray()
    {
        // Return empty singleton
        if (count == 0) return Array.Empty<T>();
        // Create array
        T[] result = new T[count];
        if (count >= 1) result[0] = _0;
        if (count >= 2) result[1] = _1;
        if (count > 2)
        {
            for (int i = 2; i < count; i++)
                result[i] = rest![i-2];
        }
        // Return array
        return result;
    }

    /// <summary>Create array with elements reversed.</summary>
    public T[] ToReverseArray()
    {
        T[] result = new T[count];
        if (count >= 1) result[count-1] = _0;
        if (count >= 2) result[count-2] = _1;
        if (count > 2)
        {
            for (int i = 2; i < count; i++)
                result[count-1-i] = rest![i-2];
        }
        return result;
    }

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)ToArray()).GetEnumerator();

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)ToArray()).GetEnumerator();
}


/// <summary>A list where the first 3 element(s) are stack allocated, and rest are allocated from heap when needed.</summary>
/// <typeparam name="T"></typeparam>
public struct StructList3<T> : IList<T>
{
    /// <summary>The number of elements that are stack allocated.</summary>
    public int StackCount => 3;

    /// <summary>Number of elements in the list</summary>
    public int Count => count;
    /// <summary>Is list readonly</summary>
    public bool IsReadOnly => false;

    /// <summary>Number of elements</summary>
    int count;
    /// <summary>First elements</summary>
    T _0, _1, _2;
    /// <summary>Elements after <see cref="StackCount"/>.</summary>
    List<T>? rest;
    /// <summary>Element comparer</summary>
    IEqualityComparer<T>? elementComparer;
    /// <summary>Element comparer</summary>
    public IEqualityComparer<T> ElementComparer => elementComparer ?? EqualityComparer<T>.Default;

    /// <summary>Create struct list.</summary>
    /// <param name="elementComparer"></param>
    public StructList3(IEqualityComparer<T>? elementComparer = null)
    {
        this.elementComparer = elementComparer ?? EqualityComparer<T>.Default;
        count = 0;

        _0 = default!;
        _1 = default!;
        _2 = default!;
        rest = null;
    }

    /// <summary>Create struct list.</summary>
    /// <param name="enumr"></param>
    public StructList3(IEnumerable<T> enumr) : this(elementComparer: default)
    {
        foreach(T value in enumr) Add(value);
    }

    /// <summary>Gets or sets the element at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>element</returns>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList3`1.</exception>
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: return _0;
                case 1: return _1;
                case 2: return _2;
                default: return rest![index - StackCount];
            }
        }
        set
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: _0 = value; return;
                case 1: _1 = value; return;
                case 2: _2 = value; return;
                default: rest![index - StackCount] = value; return;
            }
        }
    }

    /// <summary>Get reference to element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static ref T GetRef(ref StructList3<T> list, int index)
    {
        if (index < 0 || index >= list.Count) throw new ArgumentOutOfRangeException();
        switch (index)
        {
            case 0: return ref list._0;
            case 1: return ref list._1;
            case 2: return ref list._2;
            default:
                Span<T> span = CollectionsMarshal.AsSpan<T>(list.rest);
                return ref span[index - 3];
        }
    }

    /// <summary>Add <paramref name="item"/> to the StructList3`1.</summary>
    /// <exception cref="System.NotSupportedException">The StructList3`1 is read-only.</exception>
    public void Add(T item)
    {
        switch(count)
        {
            case 0: _0 = item; break;
            case 1: _1 = item; break;
            case 2: _2 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }

        count++;
        return;
    }

    /// <summary>Adds <paramref name="items"/>.</summary>
    /// <exception cref="System.NotSupportedException">The StructList3`1 is read-only.</exception>
    public void AddRange(IEnumerable<T> items)
    {
        foreach(T item in items)
        {
            switch(count)
            {
                case 0: _0 = item; break;
                case 1: _1 = item; break;
                case 2: _2 = item; break;
                default:
                    if (rest == null) rest = new List<T>();
                    rest.Add(item);
                    break;
            }
            count++;
        }
    }

    /// <summary>Add <paramref name="item"/>, if the item isn't already in the list.</summary>
    /// <exception cref="System.NotSupportedException">The StructList3`1 is read-only.</exception>
    public void AddIfNew(T item)
    {
        if (Contains(item)) return;
        switch(count)
        {
            case 0: _0 = item; break;
            case 1: _1 = item; break;
            case 2: _2 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }
        count++; 
    }

    /// <summary>Removes the first occurrence of <paramref name="item"/>.</summary>
    /// <returns>true if item was successfully removed from the StructList3`1; otherwise, false. This method also returns false if item is not found in the original StructList3`1.</returns>
    public bool Remove(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) { RemoveAt(0); return true; }
        if (count >= 2 && comparer.Equals(_1, item)) { RemoveAt(1); return true; }
        if (count >= 3 && comparer.Equals(_2, item)) { RemoveAt(2); return true; }

        if (rest == null) return false;
        foreach(T e in rest) if (comparer.Equals(e, item)) { bool removed = rest.Remove(item); if (removed) count--; return removed; }
        return false;
    }

    /// <summary>Removes element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList3`1.</exception>
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
        if (index <= 0 && count > 1) _0 = _1;
        if (index <= 1 && count > 2) _1 = _2;
        if (index <= 2 && count > 3) { _2 = rest![0]; rest!.RemoveAt(0); }
        if (index >= StackCount) rest!.RemoveAt(index - StackCount);
        count--;
    }

    /// <summary>Removes and returns the element at the end of the list.</summary>
    /// <returns>the last element</returns>
    /// <exception cref="InvalidOperationException">If list is empty</exception>
    public T Dequeue()
    {
        if (count == 0) throw new InvalidOperationException();
        int ix = count - 1;
        T result = this[ix];
        RemoveAt(ix);
        return result;
    }

    /// <summary>Remove all elements.</summary>
    /// <exception cref="System.NotSupportedException">The StructList3`1 is read-only.</exception>
    public void Clear()
    {
        if (count >= 1) _0 = default!;
        if (count >= 2) _1 = default!;
        if (count >= 3) _2 = default!;
        if (rest != null) rest.Clear();
        count = 0;
    }

    /// <summary>Determine whether <paramref name="item"/> is in the list.</summary>
    /// <returns>true if item is found in the StructList3`1; otherwise, false.</returns>
    public bool Contains(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return true;
        if (count >= 2 && comparer.Equals(_1, item)) return true;
        if (count >= 3 && comparer.Equals(_2, item)) return true;
        if (rest != null) return rest.Contains(item);
        return false;
    }

    /// <summary>Determines the index of <paramref name="item"/>.</summary>
    /// <returns>The index of item if found in the list; otherwise, -1.</returns>
    public int IndexOf(T item)
    {
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return 0;
        if (count >= 2 && comparer.Equals(_1, item)) return 1;
        if (count >= 3 && comparer.Equals(_2, item)) return 2;
        if (rest != null) return rest.IndexOf(item)-StackCount;
        return -1;
    }

    /// <summary>Inserts an <paramref name="item"/> to the StructList3`1 at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="item">The object to insert.</param>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList3`1.</exception>
    public void Insert(int index, T item)
    {
        if (index < 0 || index > count) throw new ArgumentOutOfRangeException();
        if (index >= 3) { if (rest == null) rest = new List<T>(); rest.Insert(index - StackCount, item); }
        if (index <= 2 && count >= 3) { if (rest == null) rest = new List<T>(); rest.Insert(0, _2); }
        if (index <= 1 && count >= 2) _2 = _1;
        if (index <= 0 && count >= 1) _1 = _0;

        count++;
        this[index] = item;
    }

    /// <summary>Copies the elements to <paramref name="array"/>, starting at <paramref name="arrayIndex"/>.</summary>
    /// <param name="array">The one-dimensional System.Array.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <exception cref="System.ArgumentNullException">array is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
    /// <exception cref="System.ArgumentException">The number of elements in the source StructList3`1 is greater than the available space from arrayIndex to the end of the destination array.</exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
        if (count > array.Length + arrayIndex) throw new ArgumentException();

        if (count >= 1) array[arrayIndex++] = _0;
        if (count >= 2) array[arrayIndex++] = _1;
        if (count >= 3) array[arrayIndex++] = _2;
        if (rest != null) rest.CopyTo(array, arrayIndex);
    }

    /// <summary>Create array.</summary>
    public T[] ToArray()
    {
        // Return empty singleton
        if (count == 0) return Array.Empty<T>();
        // Create array
        T[] result = new T[count];
        if (count >= 1) result[0] = _0;
        if (count >= 2) result[1] = _1;
        if (count >= 3) result[2] = _2;
        if (count > 3)
        {
            for (int i = 3; i < count; i++)
                result[i] = rest![i-3];
        }
        // Return array
        return result;
    }

    /// <summary>Create array with elements reversed.</summary>
    public T[] ToReverseArray()
    {
        T[] result = new T[count];
        if (count >= 1) result[count-1] = _0;
        if (count >= 2) result[count-2] = _1;
        if (count >= 3) result[count-3] = _2;
        if (count > 3)
        {
            for (int i = 3; i < count; i++)
                result[count-1-i] = rest![i-3];
        }
        return result;
    }

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)ToArray()).GetEnumerator();

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)ToArray()).GetEnumerator();
}


/// <summary>A list where the first 4 element(s) are stack allocated, and rest are allocated from heap when needed.</summary>
/// <typeparam name="T"></typeparam>
public struct StructList4<T> : IList<T>
{
    /// <summary>The number of elements that are stack allocated.</summary>
    public int StackCount => 4;

    /// <summary>Number of elements in the list</summary>
    public int Count => count;
    /// <summary>Is list readonly</summary>
    public bool IsReadOnly => false;

    /// <summary>Number of elements</summary>
    int count;
    /// <summary>First elements</summary>
    T _0, _1, _2, _3;
    /// <summary>Elements after <see cref="StackCount"/>.</summary>
    List<T>? rest;
    /// <summary>Element comparer</summary>
    IEqualityComparer<T>? elementComparer;
    /// <summary>Element comparer</summary>
    public IEqualityComparer<T> ElementComparer => elementComparer ?? EqualityComparer<T>.Default;

    /// <summary>Create struct list.</summary>
    /// <param name="elementComparer"></param>
    public StructList4(IEqualityComparer<T>? elementComparer = null)
    {
        this.elementComparer = elementComparer ?? EqualityComparer<T>.Default;
        count = 0;

        _0 = default!;
        _1 = default!;
        _2 = default!;
        _3 = default!;
        rest = null;
    }

    /// <summary>Create struct list.</summary>
    /// <param name="enumr"></param>
    public StructList4(IEnumerable<T> enumr) : this(elementComparer: default)
    {
        foreach(T value in enumr) Add(value);
    }

    /// <summary>Gets or sets the element at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>element</returns>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList4`1.</exception>
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: return _0;
                case 1: return _1;
                case 2: return _2;
                case 3: return _3;
                default: return rest![index - StackCount];
            }
        }
        set
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: _0 = value; return;
                case 1: _1 = value; return;
                case 2: _2 = value; return;
                case 3: _3 = value; return;
                default: rest![index - StackCount] = value; return;
            }
        }
    }

    /// <summary>Get reference to element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static ref T GetRef(ref StructList4<T> list, int index)
    {
        if (index < 0 || index >= list.Count) throw new ArgumentOutOfRangeException();
        switch (index)
        {
            case 0: return ref list._0;
            case 1: return ref list._1;
            case 2: return ref list._2;
            case 3: return ref list._3;
            default:
                Span<T> span = CollectionsMarshal.AsSpan<T>(list.rest);
                return ref span[index - 4];
        }
    }

    /// <summary>Add <paramref name="item"/> to the StructList4`1.</summary>
    /// <exception cref="System.NotSupportedException">The StructList4`1 is read-only.</exception>
    public void Add(T item)
    {
        switch(count)
        {
            case 0: _0 = item; break;
            case 1: _1 = item; break;
            case 2: _2 = item; break;
            case 3: _3 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }

        count++;
        return;
    }

    /// <summary>Adds <paramref name="items"/>.</summary>
    /// <exception cref="System.NotSupportedException">The StructList4`1 is read-only.</exception>
    public void AddRange(IEnumerable<T> items)
    {
        foreach(T item in items)
        {
            switch(count)
            {
                case 0: _0 = item; break;
                case 1: _1 = item; break;
                case 2: _2 = item; break;
                case 3: _3 = item; break;
                default:
                    if (rest == null) rest = new List<T>();
                    rest.Add(item);
                    break;
            }
            count++;
        }
    }

    /// <summary>Add <paramref name="item"/>, if the item isn't already in the list.</summary>
    /// <exception cref="System.NotSupportedException">The StructList4`1 is read-only.</exception>
    public void AddIfNew(T item)
    {
        if (Contains(item)) return;
        switch(count)
        {
            case 0: _0 = item; break;
            case 1: _1 = item; break;
            case 2: _2 = item; break;
            case 3: _3 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }
        count++; 
    }

    /// <summary>Removes the first occurrence of <paramref name="item"/>.</summary>
    /// <returns>true if item was successfully removed from the StructList4`1; otherwise, false. This method also returns false if item is not found in the original StructList4`1.</returns>
    public bool Remove(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) { RemoveAt(0); return true; }
        if (count >= 2 && comparer.Equals(_1, item)) { RemoveAt(1); return true; }
        if (count >= 3 && comparer.Equals(_2, item)) { RemoveAt(2); return true; }
        if (count >= 4 && comparer.Equals(_3, item)) { RemoveAt(3); return true; }

        if (rest == null) return false;
        foreach(T e in rest) if (comparer.Equals(e, item)) { bool removed = rest.Remove(item); if (removed) count--; return removed; }
        return false;
    }

    /// <summary>Removes element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList4`1.</exception>
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
        if (index <= 0 && count > 1) _0 = _1;
        if (index <= 1 && count > 2) _1 = _2;
        if (index <= 2 && count > 3) _2 = _3;
        if (index <= 3 && count > 4) { _3 = rest![0]; rest!.RemoveAt(0); }
        if (index >= StackCount) rest!.RemoveAt(index - StackCount);
        count--;
    }

    /// <summary>Removes and returns the element at the end of the list.</summary>
    /// <returns>the last element</returns>
    /// <exception cref="InvalidOperationException">If list is empty</exception>
    public T Dequeue()
    {
        if (count == 0) throw new InvalidOperationException();
        int ix = count - 1;
        T result = this[ix];
        RemoveAt(ix);
        return result;
    }

    /// <summary>Remove all elements.</summary>
    /// <exception cref="System.NotSupportedException">The StructList4`1 is read-only.</exception>
    public void Clear()
    {
        if (count >= 1) _0 = default!;
        if (count >= 2) _1 = default!;
        if (count >= 3) _2 = default!;
        if (count >= 4) _3 = default!;
        if (rest != null) rest.Clear();
        count = 0;
    }

    /// <summary>Determine whether <paramref name="item"/> is in the list.</summary>
    /// <returns>true if item is found in the StructList4`1; otherwise, false.</returns>
    public bool Contains(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return true;
        if (count >= 2 && comparer.Equals(_1, item)) return true;
        if (count >= 3 && comparer.Equals(_2, item)) return true;
        if (count >= 4 && comparer.Equals(_3, item)) return true;
        if (rest != null) return rest.Contains(item);
        return false;
    }

    /// <summary>Determines the index of <paramref name="item"/>.</summary>
    /// <returns>The index of item if found in the list; otherwise, -1.</returns>
    public int IndexOf(T item)
    {
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return 0;
        if (count >= 2 && comparer.Equals(_1, item)) return 1;
        if (count >= 3 && comparer.Equals(_2, item)) return 2;
        if (count >= 4 && comparer.Equals(_3, item)) return 3;
        if (rest != null) return rest.IndexOf(item)-StackCount;
        return -1;
    }

    /// <summary>Inserts an <paramref name="item"/> to the StructList4`1 at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="item">The object to insert.</param>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList4`1.</exception>
    public void Insert(int index, T item)
    {
        if (index < 0 || index > count) throw new ArgumentOutOfRangeException();
        if (index >= 4) { if (rest == null) rest = new List<T>(); rest.Insert(index - StackCount, item); }
        if (index <= 3 && count >= 4) { if (rest == null) rest = new List<T>(); rest.Insert(0, _3); }
        if (index <= 2 && count >= 3) _3 = _2;
        if (index <= 1 && count >= 2) _2 = _1;
        if (index <= 0 && count >= 1) _1 = _0;

        count++;
        this[index] = item;
    }

    /// <summary>Copies the elements to <paramref name="array"/>, starting at <paramref name="arrayIndex"/>.</summary>
    /// <param name="array">The one-dimensional System.Array.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <exception cref="System.ArgumentNullException">array is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
    /// <exception cref="System.ArgumentException">The number of elements in the source StructList4`1 is greater than the available space from arrayIndex to the end of the destination array.</exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
        if (count > array.Length + arrayIndex) throw new ArgumentException();

        if (count >= 1) array[arrayIndex++] = _0;
        if (count >= 2) array[arrayIndex++] = _1;
        if (count >= 3) array[arrayIndex++] = _2;
        if (count >= 4) array[arrayIndex++] = _3;
        if (rest != null) rest.CopyTo(array, arrayIndex);
    }

    /// <summary>Create array.</summary>
    public T[] ToArray()
    {
        // Return empty singleton
        if (count == 0) return Array.Empty<T>();
        // Create array
        T[] result = new T[count];
        if (count >= 1) result[0] = _0;
        if (count >= 2) result[1] = _1;
        if (count >= 3) result[2] = _2;
        if (count >= 4) result[3] = _3;
        if (count > 4)
        {
            for (int i = 4; i < count; i++)
                result[i] = rest![i-4];
        }
        // Return array
        return result;
    }

    /// <summary>Create array with elements reversed.</summary>
    public T[] ToReverseArray()
    {
        T[] result = new T[count];
        if (count >= 1) result[count-1] = _0;
        if (count >= 2) result[count-2] = _1;
        if (count >= 3) result[count-3] = _2;
        if (count >= 4) result[count-4] = _3;
        if (count > 4)
        {
            for (int i = 4; i < count; i++)
                result[count-1-i] = rest![i-4];
        }
        return result;
    }

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)ToArray()).GetEnumerator();

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)ToArray()).GetEnumerator();
}


/// <summary>A list where the first 5 element(s) are stack allocated, and rest are allocated from heap when needed.</summary>
/// <typeparam name="T"></typeparam>
public struct StructList5<T> : IList<T>
{
    /// <summary>The number of elements that are stack allocated.</summary>
    public int StackCount => 5;

    /// <summary>Number of elements in the list</summary>
    public int Count => count;
    /// <summary>Is list readonly</summary>
    public bool IsReadOnly => false;

    /// <summary>Number of elements</summary>
    int count;
    /// <summary>First elements</summary>
    T _0, _1, _2, _3, _4;
    /// <summary>Elements after <see cref="StackCount"/>.</summary>
    List<T>? rest;
    /// <summary>Element comparer</summary>
    IEqualityComparer<T>? elementComparer;
    /// <summary>Element comparer</summary>
    public IEqualityComparer<T> ElementComparer => elementComparer ?? EqualityComparer<T>.Default;

    /// <summary>Create struct list.</summary>
    /// <param name="elementComparer"></param>
    public StructList5(IEqualityComparer<T>? elementComparer = null)
    {
        this.elementComparer = elementComparer ?? EqualityComparer<T>.Default;
        count = 0;

        _0 = default!;
        _1 = default!;
        _2 = default!;
        _3 = default!;
        _4 = default!;
        rest = null;
    }

    /// <summary>Create struct list.</summary>
    /// <param name="enumr"></param>
    public StructList5(IEnumerable<T> enumr) : this(elementComparer: default)
    {
        foreach(T value in enumr) Add(value);
    }

    /// <summary>Gets or sets the element at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>element</returns>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList5`1.</exception>
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: return _0;
                case 1: return _1;
                case 2: return _2;
                case 3: return _3;
                case 4: return _4;
                default: return rest![index - StackCount];
            }
        }
        set
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: _0 = value; return;
                case 1: _1 = value; return;
                case 2: _2 = value; return;
                case 3: _3 = value; return;
                case 4: _4 = value; return;
                default: rest![index - StackCount] = value; return;
            }
        }
    }

    /// <summary>Get reference to element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static ref T GetRef(ref StructList5<T> list, int index)
    {
        if (index < 0 || index >= list.Count) throw new ArgumentOutOfRangeException();
        switch (index)
        {
            case 0: return ref list._0;
            case 1: return ref list._1;
            case 2: return ref list._2;
            case 3: return ref list._3;
            case 4: return ref list._4;
            default:
                Span<T> span = CollectionsMarshal.AsSpan<T>(list.rest);
                return ref span[index - 5];
        }
    }

    /// <summary>Add <paramref name="item"/> to the StructList5`1.</summary>
    /// <exception cref="System.NotSupportedException">The StructList5`1 is read-only.</exception>
    public void Add(T item)
    {
        switch(count)
        {
            case 0: _0 = item; break;
            case 1: _1 = item; break;
            case 2: _2 = item; break;
            case 3: _3 = item; break;
            case 4: _4 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }

        count++;
        return;
    }

    /// <summary>Adds <paramref name="items"/>.</summary>
    /// <exception cref="System.NotSupportedException">The StructList5`1 is read-only.</exception>
    public void AddRange(IEnumerable<T> items)
    {
        foreach(T item in items)
        {
            switch(count)
            {
                case 0: _0 = item; break;
                case 1: _1 = item; break;
                case 2: _2 = item; break;
                case 3: _3 = item; break;
                case 4: _4 = item; break;
                default:
                    if (rest == null) rest = new List<T>();
                    rest.Add(item);
                    break;
            }
            count++;
        }
    }

    /// <summary>Add <paramref name="item"/>, if the item isn't already in the list.</summary>
    /// <exception cref="System.NotSupportedException">The StructList5`1 is read-only.</exception>
    public void AddIfNew(T item)
    {
        if (Contains(item)) return;
        switch(count)
        {
            case 0: _0 = item; break;
            case 1: _1 = item; break;
            case 2: _2 = item; break;
            case 3: _3 = item; break;
            case 4: _4 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }
        count++; 
    }

    /// <summary>Removes the first occurrence of <paramref name="item"/>.</summary>
    /// <returns>true if item was successfully removed from the StructList5`1; otherwise, false. This method also returns false if item is not found in the original StructList5`1.</returns>
    public bool Remove(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) { RemoveAt(0); return true; }
        if (count >= 2 && comparer.Equals(_1, item)) { RemoveAt(1); return true; }
        if (count >= 3 && comparer.Equals(_2, item)) { RemoveAt(2); return true; }
        if (count >= 4 && comparer.Equals(_3, item)) { RemoveAt(3); return true; }
        if (count >= 5 && comparer.Equals(_4, item)) { RemoveAt(4); return true; }

        if (rest == null) return false;
        foreach(T e in rest) if (comparer.Equals(e, item)) { bool removed = rest.Remove(item); if (removed) count--; return removed; }
        return false;
    }

    /// <summary>Removes element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList5`1.</exception>
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
        if (index <= 0 && count > 1) _0 = _1;
        if (index <= 1 && count > 2) _1 = _2;
        if (index <= 2 && count > 3) _2 = _3;
        if (index <= 3 && count > 4) _3 = _4;
        if (index <= 4 && count > 5) { _4 = rest![0]; rest!.RemoveAt(0); }
        if (index >= StackCount) rest!.RemoveAt(index - StackCount);
        count--;
    }

    /// <summary>Removes and returns the element at the end of the list.</summary>
    /// <returns>the last element</returns>
    /// <exception cref="InvalidOperationException">If list is empty</exception>
    public T Dequeue()
    {
        if (count == 0) throw new InvalidOperationException();
        int ix = count - 1;
        T result = this[ix];
        RemoveAt(ix);
        return result;
    }

    /// <summary>Remove all elements.</summary>
    /// <exception cref="System.NotSupportedException">The StructList5`1 is read-only.</exception>
    public void Clear()
    {
        if (count >= 1) _0 = default!;
        if (count >= 2) _1 = default!;
        if (count >= 3) _2 = default!;
        if (count >= 4) _3 = default!;
        if (count >= 5) _4 = default!;
        if (rest != null) rest.Clear();
        count = 0;
    }

    /// <summary>Determine whether <paramref name="item"/> is in the list.</summary>
    /// <returns>true if item is found in the StructList5`1; otherwise, false.</returns>
    public bool Contains(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return true;
        if (count >= 2 && comparer.Equals(_1, item)) return true;
        if (count >= 3 && comparer.Equals(_2, item)) return true;
        if (count >= 4 && comparer.Equals(_3, item)) return true;
        if (count >= 5 && comparer.Equals(_4, item)) return true;
        if (rest != null) return rest.Contains(item);
        return false;
    }

    /// <summary>Determines the index of <paramref name="item"/>.</summary>
    /// <returns>The index of item if found in the list; otherwise, -1.</returns>
    public int IndexOf(T item)
    {
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return 0;
        if (count >= 2 && comparer.Equals(_1, item)) return 1;
        if (count >= 3 && comparer.Equals(_2, item)) return 2;
        if (count >= 4 && comparer.Equals(_3, item)) return 3;
        if (count >= 5 && comparer.Equals(_4, item)) return 4;
        if (rest != null) return rest.IndexOf(item)-StackCount;
        return -1;
    }

    /// <summary>Inserts an <paramref name="item"/> to the StructList5`1 at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="item">The object to insert.</param>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList5`1.</exception>
    public void Insert(int index, T item)
    {
        if (index < 0 || index > count) throw new ArgumentOutOfRangeException();
        if (index >= 5) { if (rest == null) rest = new List<T>(); rest.Insert(index - StackCount, item); }
        if (index <= 4 && count >= 5) { if (rest == null) rest = new List<T>(); rest.Insert(0, _4); }
        if (index <= 3 && count >= 4) _4 = _3;
        if (index <= 2 && count >= 3) _3 = _2;
        if (index <= 1 && count >= 2) _2 = _1;
        if (index <= 0 && count >= 1) _1 = _0;

        count++;
        this[index] = item;
    }

    /// <summary>Copies the elements to <paramref name="array"/>, starting at <paramref name="arrayIndex"/>.</summary>
    /// <param name="array">The one-dimensional System.Array.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <exception cref="System.ArgumentNullException">array is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
    /// <exception cref="System.ArgumentException">The number of elements in the source StructList5`1 is greater than the available space from arrayIndex to the end of the destination array.</exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
        if (count > array.Length + arrayIndex) throw new ArgumentException();

        if (count >= 1) array[arrayIndex++] = _0;
        if (count >= 2) array[arrayIndex++] = _1;
        if (count >= 3) array[arrayIndex++] = _2;
        if (count >= 4) array[arrayIndex++] = _3;
        if (count >= 5) array[arrayIndex++] = _4;
        if (rest != null) rest.CopyTo(array, arrayIndex);
    }

    /// <summary>Create array.</summary>
    public T[] ToArray()
    {
        // Return empty singleton
        if (count == 0) return Array.Empty<T>();
        // Create array
        T[] result = new T[count];
        if (count >= 1) result[0] = _0;
        if (count >= 2) result[1] = _1;
        if (count >= 3) result[2] = _2;
        if (count >= 4) result[3] = _3;
        if (count >= 5) result[4] = _4;
        if (count > 5)
        {
            for (int i = 5; i < count; i++)
                result[i] = rest![i-5];
        }
        // Return array
        return result;
    }

    /// <summary>Create array with elements reversed.</summary>
    public T[] ToReverseArray()
    {
        T[] result = new T[count];
        if (count >= 1) result[count-1] = _0;
        if (count >= 2) result[count-2] = _1;
        if (count >= 3) result[count-3] = _2;
        if (count >= 4) result[count-4] = _3;
        if (count >= 5) result[count-5] = _4;
        if (count > 5)
        {
            for (int i = 5; i < count; i++)
                result[count-1-i] = rest![i-5];
        }
        return result;
    }

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)ToArray()).GetEnumerator();

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)ToArray()).GetEnumerator();
}


/// <summary>A list where the first 6 element(s) are stack allocated, and rest are allocated from heap when needed.</summary>
/// <typeparam name="T"></typeparam>
public struct StructList6<T> : IList<T>
{
    /// <summary>The number of elements that are stack allocated.</summary>
    public int StackCount => 6;

    /// <summary>Number of elements in the list</summary>
    public int Count => count;
    /// <summary>Is list readonly</summary>
    public bool IsReadOnly => false;

    /// <summary>Number of elements</summary>
    int count;
    /// <summary>First elements</summary>
    T _0, _1, _2, _3, _4, _5;
    /// <summary>Elements after <see cref="StackCount"/>.</summary>
    List<T>? rest;
    /// <summary>Element comparer</summary>
    IEqualityComparer<T>? elementComparer;
    /// <summary>Element comparer</summary>
    public IEqualityComparer<T> ElementComparer => elementComparer ?? EqualityComparer<T>.Default;

    /// <summary>Create struct list.</summary>
    /// <param name="elementComparer"></param>
    public StructList6(IEqualityComparer<T>? elementComparer = null)
    {
        this.elementComparer = elementComparer ?? EqualityComparer<T>.Default;
        count = 0;

        _0 = default!;
        _1 = default!;
        _2 = default!;
        _3 = default!;
        _4 = default!;
        _5 = default!;
        rest = null;
    }

    /// <summary>Create struct list.</summary>
    /// <param name="enumr"></param>
    public StructList6(IEnumerable<T> enumr) : this(elementComparer: default)
    {
        foreach(T value in enumr) Add(value);
    }

    /// <summary>Gets or sets the element at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>element</returns>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList6`1.</exception>
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: return _0;
                case 1: return _1;
                case 2: return _2;
                case 3: return _3;
                case 4: return _4;
                case 5: return _5;
                default: return rest![index - StackCount];
            }
        }
        set
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: _0 = value; return;
                case 1: _1 = value; return;
                case 2: _2 = value; return;
                case 3: _3 = value; return;
                case 4: _4 = value; return;
                case 5: _5 = value; return;
                default: rest![index - StackCount] = value; return;
            }
        }
    }

    /// <summary>Get reference to element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static ref T GetRef(ref StructList6<T> list, int index)
    {
        if (index < 0 || index >= list.Count) throw new ArgumentOutOfRangeException();
        switch (index)
        {
            case 0: return ref list._0;
            case 1: return ref list._1;
            case 2: return ref list._2;
            case 3: return ref list._3;
            case 4: return ref list._4;
            case 5: return ref list._5;
            default:
                Span<T> span = CollectionsMarshal.AsSpan<T>(list.rest);
                return ref span[index - 6];
        }
    }

    /// <summary>Add <paramref name="item"/> to the StructList6`1.</summary>
    /// <exception cref="System.NotSupportedException">The StructList6`1 is read-only.</exception>
    public void Add(T item)
    {
        switch(count)
        {
            case 0: _0 = item; break;
            case 1: _1 = item; break;
            case 2: _2 = item; break;
            case 3: _3 = item; break;
            case 4: _4 = item; break;
            case 5: _5 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }

        count++;
        return;
    }

    /// <summary>Adds <paramref name="items"/>.</summary>
    /// <exception cref="System.NotSupportedException">The StructList6`1 is read-only.</exception>
    public void AddRange(IEnumerable<T> items)
    {
        foreach(T item in items)
        {
            switch(count)
            {
                case 0: _0 = item; break;
                case 1: _1 = item; break;
                case 2: _2 = item; break;
                case 3: _3 = item; break;
                case 4: _4 = item; break;
                case 5: _5 = item; break;
                default:
                    if (rest == null) rest = new List<T>();
                    rest.Add(item);
                    break;
            }
            count++;
        }
    }

    /// <summary>Add <paramref name="item"/>, if the item isn't already in the list.</summary>
    /// <exception cref="System.NotSupportedException">The StructList6`1 is read-only.</exception>
    public void AddIfNew(T item)
    {
        if (Contains(item)) return;
        switch(count)
        {
            case 0: _0 = item; break;
            case 1: _1 = item; break;
            case 2: _2 = item; break;
            case 3: _3 = item; break;
            case 4: _4 = item; break;
            case 5: _5 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }
        count++; 
    }

    /// <summary>Removes the first occurrence of <paramref name="item"/>.</summary>
    /// <returns>true if item was successfully removed from the StructList6`1; otherwise, false. This method also returns false if item is not found in the original StructList6`1.</returns>
    public bool Remove(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) { RemoveAt(0); return true; }
        if (count >= 2 && comparer.Equals(_1, item)) { RemoveAt(1); return true; }
        if (count >= 3 && comparer.Equals(_2, item)) { RemoveAt(2); return true; }
        if (count >= 4 && comparer.Equals(_3, item)) { RemoveAt(3); return true; }
        if (count >= 5 && comparer.Equals(_4, item)) { RemoveAt(4); return true; }
        if (count >= 6 && comparer.Equals(_5, item)) { RemoveAt(5); return true; }

        if (rest == null) return false;
        foreach(T e in rest) if (comparer.Equals(e, item)) { bool removed = rest.Remove(item); if (removed) count--; return removed; }
        return false;
    }

    /// <summary>Removes element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList6`1.</exception>
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
        if (index <= 0 && count > 1) _0 = _1;
        if (index <= 1 && count > 2) _1 = _2;
        if (index <= 2 && count > 3) _2 = _3;
        if (index <= 3 && count > 4) _3 = _4;
        if (index <= 4 && count > 5) _4 = _5;
        if (index <= 5 && count > 6) { _5 = rest![0]; rest!.RemoveAt(0); }
        if (index >= StackCount) rest!.RemoveAt(index - StackCount);
        count--;
    }

    /// <summary>Removes and returns the element at the end of the list.</summary>
    /// <returns>the last element</returns>
    /// <exception cref="InvalidOperationException">If list is empty</exception>
    public T Dequeue()
    {
        if (count == 0) throw new InvalidOperationException();
        int ix = count - 1;
        T result = this[ix];
        RemoveAt(ix);
        return result;
    }

    /// <summary>Remove all elements.</summary>
    /// <exception cref="System.NotSupportedException">The StructList6`1 is read-only.</exception>
    public void Clear()
    {
        if (count >= 1) _0 = default!;
        if (count >= 2) _1 = default!;
        if (count >= 3) _2 = default!;
        if (count >= 4) _3 = default!;
        if (count >= 5) _4 = default!;
        if (count >= 6) _5 = default!;
        if (rest != null) rest.Clear();
        count = 0;
    }

    /// <summary>Determine whether <paramref name="item"/> is in the list.</summary>
    /// <returns>true if item is found in the StructList6`1; otherwise, false.</returns>
    public bool Contains(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return true;
        if (count >= 2 && comparer.Equals(_1, item)) return true;
        if (count >= 3 && comparer.Equals(_2, item)) return true;
        if (count >= 4 && comparer.Equals(_3, item)) return true;
        if (count >= 5 && comparer.Equals(_4, item)) return true;
        if (count >= 6 && comparer.Equals(_5, item)) return true;
        if (rest != null) return rest.Contains(item);
        return false;
    }

    /// <summary>Determines the index of <paramref name="item"/>.</summary>
    /// <returns>The index of item if found in the list; otherwise, -1.</returns>
    public int IndexOf(T item)
    {
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return 0;
        if (count >= 2 && comparer.Equals(_1, item)) return 1;
        if (count >= 3 && comparer.Equals(_2, item)) return 2;
        if (count >= 4 && comparer.Equals(_3, item)) return 3;
        if (count >= 5 && comparer.Equals(_4, item)) return 4;
        if (count >= 6 && comparer.Equals(_5, item)) return 5;
        if (rest != null) return rest.IndexOf(item)-StackCount;
        return -1;
    }

    /// <summary>Inserts an <paramref name="item"/> to the StructList6`1 at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="item">The object to insert.</param>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList6`1.</exception>
    public void Insert(int index, T item)
    {
        if (index < 0 || index > count) throw new ArgumentOutOfRangeException();
        if (index >= 6) { if (rest == null) rest = new List<T>(); rest.Insert(index - StackCount, item); }
        if (index <= 5 && count >= 6) { if (rest == null) rest = new List<T>(); rest.Insert(0, _5); }
        if (index <= 4 && count >= 5) _5 = _4;
        if (index <= 3 && count >= 4) _4 = _3;
        if (index <= 2 && count >= 3) _3 = _2;
        if (index <= 1 && count >= 2) _2 = _1;
        if (index <= 0 && count >= 1) _1 = _0;

        count++;
        this[index] = item;
    }

    /// <summary>Copies the elements to <paramref name="array"/>, starting at <paramref name="arrayIndex"/>.</summary>
    /// <param name="array">The one-dimensional System.Array.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <exception cref="System.ArgumentNullException">array is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
    /// <exception cref="System.ArgumentException">The number of elements in the source StructList6`1 is greater than the available space from arrayIndex to the end of the destination array.</exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
        if (count > array.Length + arrayIndex) throw new ArgumentException();

        if (count >= 1) array[arrayIndex++] = _0;
        if (count >= 2) array[arrayIndex++] = _1;
        if (count >= 3) array[arrayIndex++] = _2;
        if (count >= 4) array[arrayIndex++] = _3;
        if (count >= 5) array[arrayIndex++] = _4;
        if (count >= 6) array[arrayIndex++] = _5;
        if (rest != null) rest.CopyTo(array, arrayIndex);
    }

    /// <summary>Create array.</summary>
    public T[] ToArray()
    {
        // Return empty singleton
        if (count == 0) return Array.Empty<T>();
        // Create array
        T[] result = new T[count];
        if (count >= 1) result[0] = _0;
        if (count >= 2) result[1] = _1;
        if (count >= 3) result[2] = _2;
        if (count >= 4) result[3] = _3;
        if (count >= 5) result[4] = _4;
        if (count >= 6) result[5] = _5;
        if (count > 6)
        {
            for (int i = 6; i < count; i++)
                result[i] = rest![i-6];
        }
        // Return array
        return result;
    }

    /// <summary>Create array with elements reversed.</summary>
    public T[] ToReverseArray()
    {
        T[] result = new T[count];
        if (count >= 1) result[count-1] = _0;
        if (count >= 2) result[count-2] = _1;
        if (count >= 3) result[count-3] = _2;
        if (count >= 4) result[count-4] = _3;
        if (count >= 5) result[count-5] = _4;
        if (count >= 6) result[count-6] = _5;
        if (count > 6)
        {
            for (int i = 6; i < count; i++)
                result[count-1-i] = rest![i-6];
        }
        return result;
    }

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)ToArray()).GetEnumerator();

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)ToArray()).GetEnumerator();
}


/// <summary>A list where the first 8 element(s) are stack allocated, and rest are allocated from heap when needed.</summary>
/// <typeparam name="T"></typeparam>
public struct StructList8<T> : IList<T>
{
    /// <summary>The number of elements that are stack allocated.</summary>
    public int StackCount => 8;

    /// <summary>Number of elements in the list</summary>
    public int Count => count;
    /// <summary>Is list readonly</summary>
    public bool IsReadOnly => false;

    /// <summary>Number of elements</summary>
    int count;
    /// <summary>First elements</summary>
    T _0, _1, _2, _3, _4, _5, _6, _7;
    /// <summary>Elements after <see cref="StackCount"/>.</summary>
    List<T>? rest;
    /// <summary>Element comparer</summary>
    IEqualityComparer<T>? elementComparer;
    /// <summary>Element comparer</summary>
    public IEqualityComparer<T> ElementComparer => elementComparer ?? EqualityComparer<T>.Default;

    /// <summary>Create struct list.</summary>
    /// <param name="elementComparer"></param>
    public StructList8(IEqualityComparer<T>? elementComparer = null)
    {
        this.elementComparer = elementComparer ?? EqualityComparer<T>.Default;
        count = 0;

        _0 = default!;
        _1 = default!;
        _2 = default!;
        _3 = default!;
        _4 = default!;
        _5 = default!;
        _6 = default!;
        _7 = default!;
        rest = null;
    }

    /// <summary>Create struct list.</summary>
    /// <param name="enumr"></param>
    public StructList8(IEnumerable<T> enumr) : this(elementComparer: default)
    {
        foreach(T value in enumr) Add(value);
    }

    /// <summary>Gets or sets the element at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>element</returns>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList8`1.</exception>
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: return _0;
                case 1: return _1;
                case 2: return _2;
                case 3: return _3;
                case 4: return _4;
                case 5: return _5;
                case 6: return _6;
                case 7: return _7;
                default: return rest![index - StackCount];
            }
        }
        set
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: _0 = value; return;
                case 1: _1 = value; return;
                case 2: _2 = value; return;
                case 3: _3 = value; return;
                case 4: _4 = value; return;
                case 5: _5 = value; return;
                case 6: _6 = value; return;
                case 7: _7 = value; return;
                default: rest![index - StackCount] = value; return;
            }
        }
    }

    /// <summary>Get reference to element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static ref T GetRef(ref StructList8<T> list, int index)
    {
        if (index < 0 || index >= list.Count) throw new ArgumentOutOfRangeException();
        switch (index)
        {
            case 0: return ref list._0;
            case 1: return ref list._1;
            case 2: return ref list._2;
            case 3: return ref list._3;
            case 4: return ref list._4;
            case 5: return ref list._5;
            case 6: return ref list._6;
            case 7: return ref list._7;
            default:
                Span<T> span = CollectionsMarshal.AsSpan<T>(list.rest);
                return ref span[index - 8];
        }
    }

    /// <summary>Add <paramref name="item"/> to the StructList8`1.</summary>
    /// <exception cref="System.NotSupportedException">The StructList8`1 is read-only.</exception>
    public void Add(T item)
    {
        switch(count)
        {
            case 0: _0 = item; break;
            case 1: _1 = item; break;
            case 2: _2 = item; break;
            case 3: _3 = item; break;
            case 4: _4 = item; break;
            case 5: _5 = item; break;
            case 6: _6 = item; break;
            case 7: _7 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }

        count++;
        return;
    }

    /// <summary>Adds <paramref name="items"/>.</summary>
    /// <exception cref="System.NotSupportedException">The StructList8`1 is read-only.</exception>
    public void AddRange(IEnumerable<T> items)
    {
        foreach(T item in items)
        {
            switch(count)
            {
                case 0: _0 = item; break;
                case 1: _1 = item; break;
                case 2: _2 = item; break;
                case 3: _3 = item; break;
                case 4: _4 = item; break;
                case 5: _5 = item; break;
                case 6: _6 = item; break;
                case 7: _7 = item; break;
                default:
                    if (rest == null) rest = new List<T>();
                    rest.Add(item);
                    break;
            }
            count++;
        }
    }

    /// <summary>Add <paramref name="item"/>, if the item isn't already in the list.</summary>
    /// <exception cref="System.NotSupportedException">The StructList8`1 is read-only.</exception>
    public void AddIfNew(T item)
    {
        if (Contains(item)) return;
        switch(count)
        {
            case 0: _0 = item; break;
            case 1: _1 = item; break;
            case 2: _2 = item; break;
            case 3: _3 = item; break;
            case 4: _4 = item; break;
            case 5: _5 = item; break;
            case 6: _6 = item; break;
            case 7: _7 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }
        count++; 
    }

    /// <summary>Removes the first occurrence of <paramref name="item"/>.</summary>
    /// <returns>true if item was successfully removed from the StructList8`1; otherwise, false. This method also returns false if item is not found in the original StructList8`1.</returns>
    public bool Remove(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) { RemoveAt(0); return true; }
        if (count >= 2 && comparer.Equals(_1, item)) { RemoveAt(1); return true; }
        if (count >= 3 && comparer.Equals(_2, item)) { RemoveAt(2); return true; }
        if (count >= 4 && comparer.Equals(_3, item)) { RemoveAt(3); return true; }
        if (count >= 5 && comparer.Equals(_4, item)) { RemoveAt(4); return true; }
        if (count >= 6 && comparer.Equals(_5, item)) { RemoveAt(5); return true; }
        if (count >= 7 && comparer.Equals(_6, item)) { RemoveAt(6); return true; }
        if (count >= 8 && comparer.Equals(_7, item)) { RemoveAt(7); return true; }

        if (rest == null) return false;
        foreach(T e in rest) if (comparer.Equals(e, item)) { bool removed = rest.Remove(item); if (removed) count--; return removed; }
        return false;
    }

    /// <summary>Removes element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList8`1.</exception>
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
        if (index <= 0 && count > 1) _0 = _1;
        if (index <= 1 && count > 2) _1 = _2;
        if (index <= 2 && count > 3) _2 = _3;
        if (index <= 3 && count > 4) _3 = _4;
        if (index <= 4 && count > 5) _4 = _5;
        if (index <= 5 && count > 6) _5 = _6;
        if (index <= 6 && count > 7) _6 = _7;
        if (index <= 7 && count > 8) { _7 = rest![0]; rest!.RemoveAt(0); }
        if (index >= StackCount) rest!.RemoveAt(index - StackCount);
        count--;
    }

    /// <summary>Removes and returns the element at the end of the list.</summary>
    /// <returns>the last element</returns>
    /// <exception cref="InvalidOperationException">If list is empty</exception>
    public T Dequeue()
    {
        if (count == 0) throw new InvalidOperationException();
        int ix = count - 1;
        T result = this[ix];
        RemoveAt(ix);
        return result;
    }

    /// <summary>Remove all elements.</summary>
    /// <exception cref="System.NotSupportedException">The StructList8`1 is read-only.</exception>
    public void Clear()
    {
        if (count >= 1) _0 = default!;
        if (count >= 2) _1 = default!;
        if (count >= 3) _2 = default!;
        if (count >= 4) _3 = default!;
        if (count >= 5) _4 = default!;
        if (count >= 6) _5 = default!;
        if (count >= 7) _6 = default!;
        if (count >= 8) _7 = default!;
        if (rest != null) rest.Clear();
        count = 0;
    }

    /// <summary>Determine whether <paramref name="item"/> is in the list.</summary>
    /// <returns>true if item is found in the StructList8`1; otherwise, false.</returns>
    public bool Contains(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return true;
        if (count >= 2 && comparer.Equals(_1, item)) return true;
        if (count >= 3 && comparer.Equals(_2, item)) return true;
        if (count >= 4 && comparer.Equals(_3, item)) return true;
        if (count >= 5 && comparer.Equals(_4, item)) return true;
        if (count >= 6 && comparer.Equals(_5, item)) return true;
        if (count >= 7 && comparer.Equals(_6, item)) return true;
        if (count >= 8 && comparer.Equals(_7, item)) return true;
        if (rest != null) return rest.Contains(item);
        return false;
    }

    /// <summary>Determines the index of <paramref name="item"/>.</summary>
    /// <returns>The index of item if found in the list; otherwise, -1.</returns>
    public int IndexOf(T item)
    {
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return 0;
        if (count >= 2 && comparer.Equals(_1, item)) return 1;
        if (count >= 3 && comparer.Equals(_2, item)) return 2;
        if (count >= 4 && comparer.Equals(_3, item)) return 3;
        if (count >= 5 && comparer.Equals(_4, item)) return 4;
        if (count >= 6 && comparer.Equals(_5, item)) return 5;
        if (count >= 7 && comparer.Equals(_6, item)) return 6;
        if (count >= 8 && comparer.Equals(_7, item)) return 7;
        if (rest != null) return rest.IndexOf(item)-StackCount;
        return -1;
    }

    /// <summary>Inserts an <paramref name="item"/> to the StructList8`1 at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="item">The object to insert.</param>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList8`1.</exception>
    public void Insert(int index, T item)
    {
        if (index < 0 || index > count) throw new ArgumentOutOfRangeException();
        if (index >= 8) { if (rest == null) rest = new List<T>(); rest.Insert(index - StackCount, item); }
        if (index <= 7 && count >= 8) { if (rest == null) rest = new List<T>(); rest.Insert(0, _7); }
        if (index <= 6 && count >= 7) _7 = _6;
        if (index <= 5 && count >= 6) _6 = _5;
        if (index <= 4 && count >= 5) _5 = _4;
        if (index <= 3 && count >= 4) _4 = _3;
        if (index <= 2 && count >= 3) _3 = _2;
        if (index <= 1 && count >= 2) _2 = _1;
        if (index <= 0 && count >= 1) _1 = _0;

        count++;
        this[index] = item;
    }

    /// <summary>Copies the elements to <paramref name="array"/>, starting at <paramref name="arrayIndex"/>.</summary>
    /// <param name="array">The one-dimensional System.Array.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <exception cref="System.ArgumentNullException">array is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
    /// <exception cref="System.ArgumentException">The number of elements in the source StructList8`1 is greater than the available space from arrayIndex to the end of the destination array.</exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
        if (count > array.Length + arrayIndex) throw new ArgumentException();

        if (count >= 1) array[arrayIndex++] = _0;
        if (count >= 2) array[arrayIndex++] = _1;
        if (count >= 3) array[arrayIndex++] = _2;
        if (count >= 4) array[arrayIndex++] = _3;
        if (count >= 5) array[arrayIndex++] = _4;
        if (count >= 6) array[arrayIndex++] = _5;
        if (count >= 7) array[arrayIndex++] = _6;
        if (count >= 8) array[arrayIndex++] = _7;
        if (rest != null) rest.CopyTo(array, arrayIndex);
    }

    /// <summary>Create array.</summary>
    public T[] ToArray()
    {
        // Return empty singleton
        if (count == 0) return Array.Empty<T>();
        // Create array
        T[] result = new T[count];
        if (count >= 1) result[0] = _0;
        if (count >= 2) result[1] = _1;
        if (count >= 3) result[2] = _2;
        if (count >= 4) result[3] = _3;
        if (count >= 5) result[4] = _4;
        if (count >= 6) result[5] = _5;
        if (count >= 7) result[6] = _6;
        if (count >= 8) result[7] = _7;
        if (count > 8)
        {
            for (int i = 8; i < count; i++)
                result[i] = rest![i-8];
        }
        // Return array
        return result;
    }

    /// <summary>Create array with elements reversed.</summary>
    public T[] ToReverseArray()
    {
        T[] result = new T[count];
        if (count >= 1) result[count-1] = _0;
        if (count >= 2) result[count-2] = _1;
        if (count >= 3) result[count-3] = _2;
        if (count >= 4) result[count-4] = _3;
        if (count >= 5) result[count-5] = _4;
        if (count >= 6) result[count-6] = _5;
        if (count >= 7) result[count-7] = _6;
        if (count >= 8) result[count-8] = _7;
        if (count > 8)
        {
            for (int i = 8; i < count; i++)
                result[count-1-i] = rest![i-8];
        }
        return result;
    }

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)ToArray()).GetEnumerator();

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)ToArray()).GetEnumerator();
}


/// <summary>A list where the first 10 element(s) are stack allocated, and rest are allocated from heap when needed.</summary>
/// <typeparam name="T"></typeparam>
public struct StructList10<T> : IList<T>
{
    /// <summary>The number of elements that are stack allocated.</summary>
    public int StackCount => 10;

    /// <summary>Number of elements in the list</summary>
    public int Count => count;
    /// <summary>Is list readonly</summary>
    public bool IsReadOnly => false;

    /// <summary>Number of elements</summary>
    int count;
    /// <summary>First elements</summary>
    T _0, _1, _2, _3, _4, _5, _6, _7, _8, _9;
    /// <summary>Elements after <see cref="StackCount"/>.</summary>
    List<T>? rest;
    /// <summary>Element comparer</summary>
    IEqualityComparer<T>? elementComparer;
    /// <summary>Element comparer</summary>
    public IEqualityComparer<T> ElementComparer => elementComparer ?? EqualityComparer<T>.Default;

    /// <summary>Create struct list.</summary>
    /// <param name="elementComparer"></param>
    public StructList10(IEqualityComparer<T>? elementComparer = null)
    {
        this.elementComparer = elementComparer ?? EqualityComparer<T>.Default;
        count = 0;

        _0 = default!;
        _1 = default!;
        _2 = default!;
        _3 = default!;
        _4 = default!;
        _5 = default!;
        _6 = default!;
        _7 = default!;
        _8 = default!;
        _9 = default!;
        rest = null;
    }

    /// <summary>Create struct list.</summary>
    /// <param name="enumr"></param>
    public StructList10(IEnumerable<T> enumr) : this(elementComparer: default)
    {
        foreach(T value in enumr) Add(value);
    }

    /// <summary>Gets or sets the element at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>element</returns>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList10`1.</exception>
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: return _0;
                case 1: return _1;
                case 2: return _2;
                case 3: return _3;
                case 4: return _4;
                case 5: return _5;
                case 6: return _6;
                case 7: return _7;
                case 8: return _8;
                case 9: return _9;
                default: return rest![index - StackCount];
            }
        }
        set
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: _0 = value; return;
                case 1: _1 = value; return;
                case 2: _2 = value; return;
                case 3: _3 = value; return;
                case 4: _4 = value; return;
                case 5: _5 = value; return;
                case 6: _6 = value; return;
                case 7: _7 = value; return;
                case 8: _8 = value; return;
                case 9: _9 = value; return;
                default: rest![index - StackCount] = value; return;
            }
        }
    }

    /// <summary>Get reference to element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static ref T GetRef(ref StructList10<T> list, int index)
    {
        if (index < 0 || index >= list.Count) throw new ArgumentOutOfRangeException();
        switch (index)
        {
            case 0: return ref list._0;
            case 1: return ref list._1;
            case 2: return ref list._2;
            case 3: return ref list._3;
            case 4: return ref list._4;
            case 5: return ref list._5;
            case 6: return ref list._6;
            case 7: return ref list._7;
            case 8: return ref list._8;
            case 9: return ref list._9;
            default:
                Span<T> span = CollectionsMarshal.AsSpan<T>(list.rest);
                return ref span[index - 10];
        }
    }

    /// <summary>Add <paramref name="item"/> to the StructList10`1.</summary>
    /// <exception cref="System.NotSupportedException">The StructList10`1 is read-only.</exception>
    public void Add(T item)
    {
        switch(count)
        {
            case 0: _0 = item; break;
            case 1: _1 = item; break;
            case 2: _2 = item; break;
            case 3: _3 = item; break;
            case 4: _4 = item; break;
            case 5: _5 = item; break;
            case 6: _6 = item; break;
            case 7: _7 = item; break;
            case 8: _8 = item; break;
            case 9: _9 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }

        count++;
        return;
    }

    /// <summary>Adds <paramref name="items"/>.</summary>
    /// <exception cref="System.NotSupportedException">The StructList10`1 is read-only.</exception>
    public void AddRange(IEnumerable<T> items)
    {
        foreach(T item in items)
        {
            switch(count)
            {
                case 0: _0 = item; break;
                case 1: _1 = item; break;
                case 2: _2 = item; break;
                case 3: _3 = item; break;
                case 4: _4 = item; break;
                case 5: _5 = item; break;
                case 6: _6 = item; break;
                case 7: _7 = item; break;
                case 8: _8 = item; break;
                case 9: _9 = item; break;
                default:
                    if (rest == null) rest = new List<T>();
                    rest.Add(item);
                    break;
            }
            count++;
        }
    }

    /// <summary>Add <paramref name="item"/>, if the item isn't already in the list.</summary>
    /// <exception cref="System.NotSupportedException">The StructList10`1 is read-only.</exception>
    public void AddIfNew(T item)
    {
        if (Contains(item)) return;
        switch(count)
        {
            case 0: _0 = item; break;
            case 1: _1 = item; break;
            case 2: _2 = item; break;
            case 3: _3 = item; break;
            case 4: _4 = item; break;
            case 5: _5 = item; break;
            case 6: _6 = item; break;
            case 7: _7 = item; break;
            case 8: _8 = item; break;
            case 9: _9 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }
        count++; 
    }

    /// <summary>Removes the first occurrence of <paramref name="item"/>.</summary>
    /// <returns>true if item was successfully removed from the StructList10`1; otherwise, false. This method also returns false if item is not found in the original StructList10`1.</returns>
    public bool Remove(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) { RemoveAt(0); return true; }
        if (count >= 2 && comparer.Equals(_1, item)) { RemoveAt(1); return true; }
        if (count >= 3 && comparer.Equals(_2, item)) { RemoveAt(2); return true; }
        if (count >= 4 && comparer.Equals(_3, item)) { RemoveAt(3); return true; }
        if (count >= 5 && comparer.Equals(_4, item)) { RemoveAt(4); return true; }
        if (count >= 6 && comparer.Equals(_5, item)) { RemoveAt(5); return true; }
        if (count >= 7 && comparer.Equals(_6, item)) { RemoveAt(6); return true; }
        if (count >= 8 && comparer.Equals(_7, item)) { RemoveAt(7); return true; }
        if (count >= 9 && comparer.Equals(_8, item)) { RemoveAt(8); return true; }
        if (count >= 10 && comparer.Equals(_9, item)) { RemoveAt(9); return true; }

        if (rest == null) return false;
        foreach(T e in rest) if (comparer.Equals(e, item)) { bool removed = rest.Remove(item); if (removed) count--; return removed; }
        return false;
    }

    /// <summary>Removes element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList10`1.</exception>
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
        if (index <= 0 && count > 1) _0 = _1;
        if (index <= 1 && count > 2) _1 = _2;
        if (index <= 2 && count > 3) _2 = _3;
        if (index <= 3 && count > 4) _3 = _4;
        if (index <= 4 && count > 5) _4 = _5;
        if (index <= 5 && count > 6) _5 = _6;
        if (index <= 6 && count > 7) _6 = _7;
        if (index <= 7 && count > 8) _7 = _8;
        if (index <= 8 && count > 9) _8 = _9;
        if (index <= 9 && count > 10) { _9 = rest![0]; rest!.RemoveAt(0); }
        if (index >= StackCount) rest!.RemoveAt(index - StackCount);
        count--;
    }

    /// <summary>Removes and returns the element at the end of the list.</summary>
    /// <returns>the last element</returns>
    /// <exception cref="InvalidOperationException">If list is empty</exception>
    public T Dequeue()
    {
        if (count == 0) throw new InvalidOperationException();
        int ix = count - 1;
        T result = this[ix];
        RemoveAt(ix);
        return result;
    }

    /// <summary>Remove all elements.</summary>
    /// <exception cref="System.NotSupportedException">The StructList10`1 is read-only.</exception>
    public void Clear()
    {
        if (count >= 1) _0 = default!;
        if (count >= 2) _1 = default!;
        if (count >= 3) _2 = default!;
        if (count >= 4) _3 = default!;
        if (count >= 5) _4 = default!;
        if (count >= 6) _5 = default!;
        if (count >= 7) _6 = default!;
        if (count >= 8) _7 = default!;
        if (count >= 9) _8 = default!;
        if (count >= 10) _9 = default!;
        if (rest != null) rest.Clear();
        count = 0;
    }

    /// <summary>Determine whether <paramref name="item"/> is in the list.</summary>
    /// <returns>true if item is found in the StructList10`1; otherwise, false.</returns>
    public bool Contains(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return true;
        if (count >= 2 && comparer.Equals(_1, item)) return true;
        if (count >= 3 && comparer.Equals(_2, item)) return true;
        if (count >= 4 && comparer.Equals(_3, item)) return true;
        if (count >= 5 && comparer.Equals(_4, item)) return true;
        if (count >= 6 && comparer.Equals(_5, item)) return true;
        if (count >= 7 && comparer.Equals(_6, item)) return true;
        if (count >= 8 && comparer.Equals(_7, item)) return true;
        if (count >= 9 && comparer.Equals(_8, item)) return true;
        if (count >= 10 && comparer.Equals(_9, item)) return true;
        if (rest != null) return rest.Contains(item);
        return false;
    }

    /// <summary>Determines the index of <paramref name="item"/>.</summary>
    /// <returns>The index of item if found in the list; otherwise, -1.</returns>
    public int IndexOf(T item)
    {
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return 0;
        if (count >= 2 && comparer.Equals(_1, item)) return 1;
        if (count >= 3 && comparer.Equals(_2, item)) return 2;
        if (count >= 4 && comparer.Equals(_3, item)) return 3;
        if (count >= 5 && comparer.Equals(_4, item)) return 4;
        if (count >= 6 && comparer.Equals(_5, item)) return 5;
        if (count >= 7 && comparer.Equals(_6, item)) return 6;
        if (count >= 8 && comparer.Equals(_7, item)) return 7;
        if (count >= 9 && comparer.Equals(_8, item)) return 8;
        if (count >= 10 && comparer.Equals(_9, item)) return 9;
        if (rest != null) return rest.IndexOf(item)-StackCount;
        return -1;
    }

    /// <summary>Inserts an <paramref name="item"/> to the StructList10`1 at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="item">The object to insert.</param>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList10`1.</exception>
    public void Insert(int index, T item)
    {
        if (index < 0 || index > count) throw new ArgumentOutOfRangeException();
        if (index >= 10) { if (rest == null) rest = new List<T>(); rest.Insert(index - StackCount, item); }
        if (index <= 9 && count >= 10) { if (rest == null) rest = new List<T>(); rest.Insert(0, _9); }
        if (index <= 8 && count >= 9) _9 = _8;
        if (index <= 7 && count >= 8) _8 = _7;
        if (index <= 6 && count >= 7) _7 = _6;
        if (index <= 5 && count >= 6) _6 = _5;
        if (index <= 4 && count >= 5) _5 = _4;
        if (index <= 3 && count >= 4) _4 = _3;
        if (index <= 2 && count >= 3) _3 = _2;
        if (index <= 1 && count >= 2) _2 = _1;
        if (index <= 0 && count >= 1) _1 = _0;

        count++;
        this[index] = item;
    }

    /// <summary>Copies the elements to <paramref name="array"/>, starting at <paramref name="arrayIndex"/>.</summary>
    /// <param name="array">The one-dimensional System.Array.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <exception cref="System.ArgumentNullException">array is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
    /// <exception cref="System.ArgumentException">The number of elements in the source StructList10`1 is greater than the available space from arrayIndex to the end of the destination array.</exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
        if (count > array.Length + arrayIndex) throw new ArgumentException();

        if (count >= 1) array[arrayIndex++] = _0;
        if (count >= 2) array[arrayIndex++] = _1;
        if (count >= 3) array[arrayIndex++] = _2;
        if (count >= 4) array[arrayIndex++] = _3;
        if (count >= 5) array[arrayIndex++] = _4;
        if (count >= 6) array[arrayIndex++] = _5;
        if (count >= 7) array[arrayIndex++] = _6;
        if (count >= 8) array[arrayIndex++] = _7;
        if (count >= 9) array[arrayIndex++] = _8;
        if (count >= 10) array[arrayIndex++] = _9;
        if (rest != null) rest.CopyTo(array, arrayIndex);
    }

    /// <summary>Create array.</summary>
    public T[] ToArray()
    {
        // Return empty singleton
        if (count == 0) return Array.Empty<T>();
        // Create array
        T[] result = new T[count];
        if (count >= 1) result[0] = _0;
        if (count >= 2) result[1] = _1;
        if (count >= 3) result[2] = _2;
        if (count >= 4) result[3] = _3;
        if (count >= 5) result[4] = _4;
        if (count >= 6) result[5] = _5;
        if (count >= 7) result[6] = _6;
        if (count >= 8) result[7] = _7;
        if (count >= 9) result[8] = _8;
        if (count >= 10) result[9] = _9;
        if (count > 10)
        {
            for (int i = 10; i < count; i++)
                result[i] = rest![i-10];
        }
        // Return array
        return result;
    }

    /// <summary>Create array with elements reversed.</summary>
    public T[] ToReverseArray()
    {
        T[] result = new T[count];
        if (count >= 1) result[count-1] = _0;
        if (count >= 2) result[count-2] = _1;
        if (count >= 3) result[count-3] = _2;
        if (count >= 4) result[count-4] = _3;
        if (count >= 5) result[count-5] = _4;
        if (count >= 6) result[count-6] = _5;
        if (count >= 7) result[count-7] = _6;
        if (count >= 8) result[count-8] = _7;
        if (count >= 9) result[count-9] = _8;
        if (count >= 10) result[count-10] = _9;
        if (count > 10)
        {
            for (int i = 10; i < count; i++)
                result[count-1-i] = rest![i-10];
        }
        return result;
    }

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)ToArray()).GetEnumerator();

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)ToArray()).GetEnumerator();
}


/// <summary>A list where the first 12 element(s) are stack allocated, and rest are allocated from heap when needed.</summary>
/// <typeparam name="T"></typeparam>
public struct StructList12<T> : IList<T>
{
    /// <summary>The number of elements that are stack allocated.</summary>
    public int StackCount => 12;

    /// <summary>Number of elements in the list</summary>
    public int Count => count;
    /// <summary>Is list readonly</summary>
    public bool IsReadOnly => false;

    /// <summary>Number of elements</summary>
    int count;
    /// <summary>First elements</summary>
    T _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11;
    /// <summary>Elements after <see cref="StackCount"/>.</summary>
    List<T>? rest;
    /// <summary>Element comparer</summary>
    IEqualityComparer<T>? elementComparer;
    /// <summary>Element comparer</summary>
    public IEqualityComparer<T> ElementComparer => elementComparer ?? EqualityComparer<T>.Default;

    /// <summary>Create struct list.</summary>
    /// <param name="elementComparer"></param>
    public StructList12(IEqualityComparer<T>? elementComparer = null)
    {
        this.elementComparer = elementComparer ?? EqualityComparer<T>.Default;
        count = 0;

        _0 = default!;
        _1 = default!;
        _2 = default!;
        _3 = default!;
        _4 = default!;
        _5 = default!;
        _6 = default!;
        _7 = default!;
        _8 = default!;
        _9 = default!;
        _10 = default!;
        _11 = default!;
        rest = null;
    }

    /// <summary>Create struct list.</summary>
    /// <param name="enumr"></param>
    public StructList12(IEnumerable<T> enumr) : this(elementComparer: default)
    {
        foreach(T value in enumr) Add(value);
    }

    /// <summary>Gets or sets the element at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>element</returns>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList12`1.</exception>
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: return _0;
                case 1: return _1;
                case 2: return _2;
                case 3: return _3;
                case 4: return _4;
                case 5: return _5;
                case 6: return _6;
                case 7: return _7;
                case 8: return _8;
                case 9: return _9;
                case 10: return _10;
                case 11: return _11;
                default: return rest![index - StackCount];
            }
        }
        set
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: _0 = value; return;
                case 1: _1 = value; return;
                case 2: _2 = value; return;
                case 3: _3 = value; return;
                case 4: _4 = value; return;
                case 5: _5 = value; return;
                case 6: _6 = value; return;
                case 7: _7 = value; return;
                case 8: _8 = value; return;
                case 9: _9 = value; return;
                case 10: _10 = value; return;
                case 11: _11 = value; return;
                default: rest![index - StackCount] = value; return;
            }
        }
    }

    /// <summary>Get reference to element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static ref T GetRef(ref StructList12<T> list, int index)
    {
        if (index < 0 || index >= list.Count) throw new ArgumentOutOfRangeException();
        switch (index)
        {
            case 0: return ref list._0;
            case 1: return ref list._1;
            case 2: return ref list._2;
            case 3: return ref list._3;
            case 4: return ref list._4;
            case 5: return ref list._5;
            case 6: return ref list._6;
            case 7: return ref list._7;
            case 8: return ref list._8;
            case 9: return ref list._9;
            case 10: return ref list._10;
            case 11: return ref list._11;
            default:
                Span<T> span = CollectionsMarshal.AsSpan<T>(list.rest);
                return ref span[index - 12];
        }
    }

    /// <summary>Add <paramref name="item"/> to the StructList12`1.</summary>
    /// <exception cref="System.NotSupportedException">The StructList12`1 is read-only.</exception>
    public void Add(T item)
    {
        switch(count)
        {
            case 0: _0 = item; break;
            case 1: _1 = item; break;
            case 2: _2 = item; break;
            case 3: _3 = item; break;
            case 4: _4 = item; break;
            case 5: _5 = item; break;
            case 6: _6 = item; break;
            case 7: _7 = item; break;
            case 8: _8 = item; break;
            case 9: _9 = item; break;
            case 10: _10 = item; break;
            case 11: _11 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }

        count++;
        return;
    }

    /// <summary>Adds <paramref name="items"/>.</summary>
    /// <exception cref="System.NotSupportedException">The StructList12`1 is read-only.</exception>
    public void AddRange(IEnumerable<T> items)
    {
        foreach(T item in items)
        {
            switch(count)
            {
                case 0: _0 = item; break;
                case 1: _1 = item; break;
                case 2: _2 = item; break;
                case 3: _3 = item; break;
                case 4: _4 = item; break;
                case 5: _5 = item; break;
                case 6: _6 = item; break;
                case 7: _7 = item; break;
                case 8: _8 = item; break;
                case 9: _9 = item; break;
                case 10: _10 = item; break;
                case 11: _11 = item; break;
                default:
                    if (rest == null) rest = new List<T>();
                    rest.Add(item);
                    break;
            }
            count++;
        }
    }

    /// <summary>Add <paramref name="item"/>, if the item isn't already in the list.</summary>
    /// <exception cref="System.NotSupportedException">The StructList12`1 is read-only.</exception>
    public void AddIfNew(T item)
    {
        if (Contains(item)) return;
        switch(count)
        {
            case 0: _0 = item; break;
            case 1: _1 = item; break;
            case 2: _2 = item; break;
            case 3: _3 = item; break;
            case 4: _4 = item; break;
            case 5: _5 = item; break;
            case 6: _6 = item; break;
            case 7: _7 = item; break;
            case 8: _8 = item; break;
            case 9: _9 = item; break;
            case 10: _10 = item; break;
            case 11: _11 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }
        count++; 
    }

    /// <summary>Removes the first occurrence of <paramref name="item"/>.</summary>
    /// <returns>true if item was successfully removed from the StructList12`1; otherwise, false. This method also returns false if item is not found in the original StructList12`1.</returns>
    public bool Remove(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) { RemoveAt(0); return true; }
        if (count >= 2 && comparer.Equals(_1, item)) { RemoveAt(1); return true; }
        if (count >= 3 && comparer.Equals(_2, item)) { RemoveAt(2); return true; }
        if (count >= 4 && comparer.Equals(_3, item)) { RemoveAt(3); return true; }
        if (count >= 5 && comparer.Equals(_4, item)) { RemoveAt(4); return true; }
        if (count >= 6 && comparer.Equals(_5, item)) { RemoveAt(5); return true; }
        if (count >= 7 && comparer.Equals(_6, item)) { RemoveAt(6); return true; }
        if (count >= 8 && comparer.Equals(_7, item)) { RemoveAt(7); return true; }
        if (count >= 9 && comparer.Equals(_8, item)) { RemoveAt(8); return true; }
        if (count >= 10 && comparer.Equals(_9, item)) { RemoveAt(9); return true; }
        if (count >= 11 && comparer.Equals(_10, item)) { RemoveAt(10); return true; }
        if (count >= 12 && comparer.Equals(_11, item)) { RemoveAt(11); return true; }

        if (rest == null) return false;
        foreach(T e in rest) if (comparer.Equals(e, item)) { bool removed = rest.Remove(item); if (removed) count--; return removed; }
        return false;
    }

    /// <summary>Removes element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList12`1.</exception>
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
        if (index <= 0 && count > 1) _0 = _1;
        if (index <= 1 && count > 2) _1 = _2;
        if (index <= 2 && count > 3) _2 = _3;
        if (index <= 3 && count > 4) _3 = _4;
        if (index <= 4 && count > 5) _4 = _5;
        if (index <= 5 && count > 6) _5 = _6;
        if (index <= 6 && count > 7) _6 = _7;
        if (index <= 7 && count > 8) _7 = _8;
        if (index <= 8 && count > 9) _8 = _9;
        if (index <= 9 && count > 10) _9 = _10;
        if (index <= 10 && count > 11) _10 = _11;
        if (index <= 11 && count > 12) { _11 = rest![0]; rest!.RemoveAt(0); }
        if (index >= StackCount) rest!.RemoveAt(index - StackCount);
        count--;
    }

    /// <summary>Removes and returns the element at the end of the list.</summary>
    /// <returns>the last element</returns>
    /// <exception cref="InvalidOperationException">If list is empty</exception>
    public T Dequeue()
    {
        if (count == 0) throw new InvalidOperationException();
        int ix = count - 1;
        T result = this[ix];
        RemoveAt(ix);
        return result;
    }

    /// <summary>Remove all elements.</summary>
    /// <exception cref="System.NotSupportedException">The StructList12`1 is read-only.</exception>
    public void Clear()
    {
        if (count >= 1) _0 = default!;
        if (count >= 2) _1 = default!;
        if (count >= 3) _2 = default!;
        if (count >= 4) _3 = default!;
        if (count >= 5) _4 = default!;
        if (count >= 6) _5 = default!;
        if (count >= 7) _6 = default!;
        if (count >= 8) _7 = default!;
        if (count >= 9) _8 = default!;
        if (count >= 10) _9 = default!;
        if (count >= 11) _10 = default!;
        if (count >= 12) _11 = default!;
        if (rest != null) rest.Clear();
        count = 0;
    }

    /// <summary>Determine whether <paramref name="item"/> is in the list.</summary>
    /// <returns>true if item is found in the StructList12`1; otherwise, false.</returns>
    public bool Contains(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return true;
        if (count >= 2 && comparer.Equals(_1, item)) return true;
        if (count >= 3 && comparer.Equals(_2, item)) return true;
        if (count >= 4 && comparer.Equals(_3, item)) return true;
        if (count >= 5 && comparer.Equals(_4, item)) return true;
        if (count >= 6 && comparer.Equals(_5, item)) return true;
        if (count >= 7 && comparer.Equals(_6, item)) return true;
        if (count >= 8 && comparer.Equals(_7, item)) return true;
        if (count >= 9 && comparer.Equals(_8, item)) return true;
        if (count >= 10 && comparer.Equals(_9, item)) return true;
        if (count >= 11 && comparer.Equals(_10, item)) return true;
        if (count >= 12 && comparer.Equals(_11, item)) return true;
        if (rest != null) return rest.Contains(item);
        return false;
    }

    /// <summary>Determines the index of <paramref name="item"/>.</summary>
    /// <returns>The index of item if found in the list; otherwise, -1.</returns>
    public int IndexOf(T item)
    {
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return 0;
        if (count >= 2 && comparer.Equals(_1, item)) return 1;
        if (count >= 3 && comparer.Equals(_2, item)) return 2;
        if (count >= 4 && comparer.Equals(_3, item)) return 3;
        if (count >= 5 && comparer.Equals(_4, item)) return 4;
        if (count >= 6 && comparer.Equals(_5, item)) return 5;
        if (count >= 7 && comparer.Equals(_6, item)) return 6;
        if (count >= 8 && comparer.Equals(_7, item)) return 7;
        if (count >= 9 && comparer.Equals(_8, item)) return 8;
        if (count >= 10 && comparer.Equals(_9, item)) return 9;
        if (count >= 11 && comparer.Equals(_10, item)) return 10;
        if (count >= 12 && comparer.Equals(_11, item)) return 11;
        if (rest != null) return rest.IndexOf(item)-StackCount;
        return -1;
    }

    /// <summary>Inserts an <paramref name="item"/> to the StructList12`1 at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="item">The object to insert.</param>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList12`1.</exception>
    public void Insert(int index, T item)
    {
        if (index < 0 || index > count) throw new ArgumentOutOfRangeException();
        if (index >= 12) { if (rest == null) rest = new List<T>(); rest.Insert(index - StackCount, item); }
        if (index <= 11 && count >= 12) { if (rest == null) rest = new List<T>(); rest.Insert(0, _11); }
        if (index <= 10 && count >= 11) _11 = _10;
        if (index <= 9 && count >= 10) _10 = _9;
        if (index <= 8 && count >= 9) _9 = _8;
        if (index <= 7 && count >= 8) _8 = _7;
        if (index <= 6 && count >= 7) _7 = _6;
        if (index <= 5 && count >= 6) _6 = _5;
        if (index <= 4 && count >= 5) _5 = _4;
        if (index <= 3 && count >= 4) _4 = _3;
        if (index <= 2 && count >= 3) _3 = _2;
        if (index <= 1 && count >= 2) _2 = _1;
        if (index <= 0 && count >= 1) _1 = _0;

        count++;
        this[index] = item;
    }

    /// <summary>Copies the elements to <paramref name="array"/>, starting at <paramref name="arrayIndex"/>.</summary>
    /// <param name="array">The one-dimensional System.Array.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <exception cref="System.ArgumentNullException">array is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
    /// <exception cref="System.ArgumentException">The number of elements in the source StructList12`1 is greater than the available space from arrayIndex to the end of the destination array.</exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
        if (count > array.Length + arrayIndex) throw new ArgumentException();

        if (count >= 1) array[arrayIndex++] = _0;
        if (count >= 2) array[arrayIndex++] = _1;
        if (count >= 3) array[arrayIndex++] = _2;
        if (count >= 4) array[arrayIndex++] = _3;
        if (count >= 5) array[arrayIndex++] = _4;
        if (count >= 6) array[arrayIndex++] = _5;
        if (count >= 7) array[arrayIndex++] = _6;
        if (count >= 8) array[arrayIndex++] = _7;
        if (count >= 9) array[arrayIndex++] = _8;
        if (count >= 10) array[arrayIndex++] = _9;
        if (count >= 11) array[arrayIndex++] = _10;
        if (count >= 12) array[arrayIndex++] = _11;
        if (rest != null) rest.CopyTo(array, arrayIndex);
    }

    /// <summary>Create array.</summary>
    public T[] ToArray()
    {
        // Return empty singleton
        if (count == 0) return Array.Empty<T>();
        // Create array
        T[] result = new T[count];
        if (count >= 1) result[0] = _0;
        if (count >= 2) result[1] = _1;
        if (count >= 3) result[2] = _2;
        if (count >= 4) result[3] = _3;
        if (count >= 5) result[4] = _4;
        if (count >= 6) result[5] = _5;
        if (count >= 7) result[6] = _6;
        if (count >= 8) result[7] = _7;
        if (count >= 9) result[8] = _8;
        if (count >= 10) result[9] = _9;
        if (count >= 11) result[10] = _10;
        if (count >= 12) result[11] = _11;
        if (count > 12)
        {
            for (int i = 12; i < count; i++)
                result[i] = rest![i-12];
        }
        // Return array
        return result;
    }

    /// <summary>Create array with elements reversed.</summary>
    public T[] ToReverseArray()
    {
        T[] result = new T[count];
        if (count >= 1) result[count-1] = _0;
        if (count >= 2) result[count-2] = _1;
        if (count >= 3) result[count-3] = _2;
        if (count >= 4) result[count-4] = _3;
        if (count >= 5) result[count-5] = _4;
        if (count >= 6) result[count-6] = _5;
        if (count >= 7) result[count-7] = _6;
        if (count >= 8) result[count-8] = _7;
        if (count >= 9) result[count-9] = _8;
        if (count >= 10) result[count-10] = _9;
        if (count >= 11) result[count-11] = _10;
        if (count >= 12) result[count-12] = _11;
        if (count > 12)
        {
            for (int i = 12; i < count; i++)
                result[count-1-i] = rest![i-12];
        }
        return result;
    }

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)ToArray()).GetEnumerator();

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)ToArray()).GetEnumerator();
}


/// <summary>A list where the first 16 element(s) are stack allocated, and rest are allocated from heap when needed.</summary>
/// <typeparam name="T"></typeparam>
public struct StructList16<T> : IList<T>
{
    /// <summary>The number of elements that are stack allocated.</summary>
    public int StackCount => 16;

    /// <summary>Number of elements in the list</summary>
    public int Count => count;
    /// <summary>Is list readonly</summary>
    public bool IsReadOnly => false;

    /// <summary>Number of elements</summary>
    int count;
    /// <summary>First elements</summary>
    T _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14, _15;
    /// <summary>Elements after <see cref="StackCount"/>.</summary>
    List<T>? rest;
    /// <summary>Element comparer</summary>
    IEqualityComparer<T>? elementComparer;
    /// <summary>Element comparer</summary>
    public IEqualityComparer<T> ElementComparer => elementComparer ?? EqualityComparer<T>.Default;

    /// <summary>Create struct list.</summary>
    /// <param name="elementComparer"></param>
    public StructList16(IEqualityComparer<T>? elementComparer = null)
    {
        this.elementComparer = elementComparer ?? EqualityComparer<T>.Default;
        count = 0;

        _0 = default!;
        _1 = default!;
        _2 = default!;
        _3 = default!;
        _4 = default!;
        _5 = default!;
        _6 = default!;
        _7 = default!;
        _8 = default!;
        _9 = default!;
        _10 = default!;
        _11 = default!;
        _12 = default!;
        _13 = default!;
        _14 = default!;
        _15 = default!;
        rest = null;
    }

    /// <summary>Create struct list.</summary>
    /// <param name="enumr"></param>
    public StructList16(IEnumerable<T> enumr) : this(elementComparer: default)
    {
        foreach(T value in enumr) Add(value);
    }

    /// <summary>Gets or sets the element at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>element</returns>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList16`1.</exception>
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: return _0;
                case 1: return _1;
                case 2: return _2;
                case 3: return _3;
                case 4: return _4;
                case 5: return _5;
                case 6: return _6;
                case 7: return _7;
                case 8: return _8;
                case 9: return _9;
                case 10: return _10;
                case 11: return _11;
                case 12: return _12;
                case 13: return _13;
                case 14: return _14;
                case 15: return _15;
                default: return rest![index - StackCount];
            }
        }
        set
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: _0 = value; return;
                case 1: _1 = value; return;
                case 2: _2 = value; return;
                case 3: _3 = value; return;
                case 4: _4 = value; return;
                case 5: _5 = value; return;
                case 6: _6 = value; return;
                case 7: _7 = value; return;
                case 8: _8 = value; return;
                case 9: _9 = value; return;
                case 10: _10 = value; return;
                case 11: _11 = value; return;
                case 12: _12 = value; return;
                case 13: _13 = value; return;
                case 14: _14 = value; return;
                case 15: _15 = value; return;
                default: rest![index - StackCount] = value; return;
            }
        }
    }

    /// <summary>Get reference to element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static ref T GetRef(ref StructList16<T> list, int index)
    {
        if (index < 0 || index >= list.Count) throw new ArgumentOutOfRangeException();
        switch (index)
        {
            case 0: return ref list._0;
            case 1: return ref list._1;
            case 2: return ref list._2;
            case 3: return ref list._3;
            case 4: return ref list._4;
            case 5: return ref list._5;
            case 6: return ref list._6;
            case 7: return ref list._7;
            case 8: return ref list._8;
            case 9: return ref list._9;
            case 10: return ref list._10;
            case 11: return ref list._11;
            case 12: return ref list._12;
            case 13: return ref list._13;
            case 14: return ref list._14;
            case 15: return ref list._15;
            default:
                Span<T> span = CollectionsMarshal.AsSpan<T>(list.rest);
                return ref span[index - 16];
        }
    }

    /// <summary>Add <paramref name="item"/> to the StructList16`1.</summary>
    /// <exception cref="System.NotSupportedException">The StructList16`1 is read-only.</exception>
    public void Add(T item)
    {
        switch(count)
        {
            case 0: _0 = item; break;
            case 1: _1 = item; break;
            case 2: _2 = item; break;
            case 3: _3 = item; break;
            case 4: _4 = item; break;
            case 5: _5 = item; break;
            case 6: _6 = item; break;
            case 7: _7 = item; break;
            case 8: _8 = item; break;
            case 9: _9 = item; break;
            case 10: _10 = item; break;
            case 11: _11 = item; break;
            case 12: _12 = item; break;
            case 13: _13 = item; break;
            case 14: _14 = item; break;
            case 15: _15 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }

        count++;
        return;
    }

    /// <summary>Adds <paramref name="items"/>.</summary>
    /// <exception cref="System.NotSupportedException">The StructList16`1 is read-only.</exception>
    public void AddRange(IEnumerable<T> items)
    {
        foreach(T item in items)
        {
            switch(count)
            {
                case 0: _0 = item; break;
                case 1: _1 = item; break;
                case 2: _2 = item; break;
                case 3: _3 = item; break;
                case 4: _4 = item; break;
                case 5: _5 = item; break;
                case 6: _6 = item; break;
                case 7: _7 = item; break;
                case 8: _8 = item; break;
                case 9: _9 = item; break;
                case 10: _10 = item; break;
                case 11: _11 = item; break;
                case 12: _12 = item; break;
                case 13: _13 = item; break;
                case 14: _14 = item; break;
                case 15: _15 = item; break;
                default:
                    if (rest == null) rest = new List<T>();
                    rest.Add(item);
                    break;
            }
            count++;
        }
    }

    /// <summary>Add <paramref name="item"/>, if the item isn't already in the list.</summary>
    /// <exception cref="System.NotSupportedException">The StructList16`1 is read-only.</exception>
    public void AddIfNew(T item)
    {
        if (Contains(item)) return;
        switch(count)
        {
            case 0: _0 = item; break;
            case 1: _1 = item; break;
            case 2: _2 = item; break;
            case 3: _3 = item; break;
            case 4: _4 = item; break;
            case 5: _5 = item; break;
            case 6: _6 = item; break;
            case 7: _7 = item; break;
            case 8: _8 = item; break;
            case 9: _9 = item; break;
            case 10: _10 = item; break;
            case 11: _11 = item; break;
            case 12: _12 = item; break;
            case 13: _13 = item; break;
            case 14: _14 = item; break;
            case 15: _15 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }
        count++; 
    }

    /// <summary>Removes the first occurrence of <paramref name="item"/>.</summary>
    /// <returns>true if item was successfully removed from the StructList16`1; otherwise, false. This method also returns false if item is not found in the original StructList16`1.</returns>
    public bool Remove(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) { RemoveAt(0); return true; }
        if (count >= 2 && comparer.Equals(_1, item)) { RemoveAt(1); return true; }
        if (count >= 3 && comparer.Equals(_2, item)) { RemoveAt(2); return true; }
        if (count >= 4 && comparer.Equals(_3, item)) { RemoveAt(3); return true; }
        if (count >= 5 && comparer.Equals(_4, item)) { RemoveAt(4); return true; }
        if (count >= 6 && comparer.Equals(_5, item)) { RemoveAt(5); return true; }
        if (count >= 7 && comparer.Equals(_6, item)) { RemoveAt(6); return true; }
        if (count >= 8 && comparer.Equals(_7, item)) { RemoveAt(7); return true; }
        if (count >= 9 && comparer.Equals(_8, item)) { RemoveAt(8); return true; }
        if (count >= 10 && comparer.Equals(_9, item)) { RemoveAt(9); return true; }
        if (count >= 11 && comparer.Equals(_10, item)) { RemoveAt(10); return true; }
        if (count >= 12 && comparer.Equals(_11, item)) { RemoveAt(11); return true; }
        if (count >= 13 && comparer.Equals(_12, item)) { RemoveAt(12); return true; }
        if (count >= 14 && comparer.Equals(_13, item)) { RemoveAt(13); return true; }
        if (count >= 15 && comparer.Equals(_14, item)) { RemoveAt(14); return true; }
        if (count >= 16 && comparer.Equals(_15, item)) { RemoveAt(15); return true; }

        if (rest == null) return false;
        foreach(T e in rest) if (comparer.Equals(e, item)) { bool removed = rest.Remove(item); if (removed) count--; return removed; }
        return false;
    }

    /// <summary>Removes element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList16`1.</exception>
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
        if (index <= 0 && count > 1) _0 = _1;
        if (index <= 1 && count > 2) _1 = _2;
        if (index <= 2 && count > 3) _2 = _3;
        if (index <= 3 && count > 4) _3 = _4;
        if (index <= 4 && count > 5) _4 = _5;
        if (index <= 5 && count > 6) _5 = _6;
        if (index <= 6 && count > 7) _6 = _7;
        if (index <= 7 && count > 8) _7 = _8;
        if (index <= 8 && count > 9) _8 = _9;
        if (index <= 9 && count > 10) _9 = _10;
        if (index <= 10 && count > 11) _10 = _11;
        if (index <= 11 && count > 12) _11 = _12;
        if (index <= 12 && count > 13) _12 = _13;
        if (index <= 13 && count > 14) _13 = _14;
        if (index <= 14 && count > 15) _14 = _15;
        if (index <= 15 && count > 16) { _15 = rest![0]; rest!.RemoveAt(0); }
        if (index >= StackCount) rest!.RemoveAt(index - StackCount);
        count--;
    }

    /// <summary>Removes and returns the element at the end of the list.</summary>
    /// <returns>the last element</returns>
    /// <exception cref="InvalidOperationException">If list is empty</exception>
    public T Dequeue()
    {
        if (count == 0) throw new InvalidOperationException();
        int ix = count - 1;
        T result = this[ix];
        RemoveAt(ix);
        return result;
    }

    /// <summary>Remove all elements.</summary>
    /// <exception cref="System.NotSupportedException">The StructList16`1 is read-only.</exception>
    public void Clear()
    {
        if (count >= 1) _0 = default!;
        if (count >= 2) _1 = default!;
        if (count >= 3) _2 = default!;
        if (count >= 4) _3 = default!;
        if (count >= 5) _4 = default!;
        if (count >= 6) _5 = default!;
        if (count >= 7) _6 = default!;
        if (count >= 8) _7 = default!;
        if (count >= 9) _8 = default!;
        if (count >= 10) _9 = default!;
        if (count >= 11) _10 = default!;
        if (count >= 12) _11 = default!;
        if (count >= 13) _12 = default!;
        if (count >= 14) _13 = default!;
        if (count >= 15) _14 = default!;
        if (count >= 16) _15 = default!;
        if (rest != null) rest.Clear();
        count = 0;
    }

    /// <summary>Determine whether <paramref name="item"/> is in the list.</summary>
    /// <returns>true if item is found in the StructList16`1; otherwise, false.</returns>
    public bool Contains(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return true;
        if (count >= 2 && comparer.Equals(_1, item)) return true;
        if (count >= 3 && comparer.Equals(_2, item)) return true;
        if (count >= 4 && comparer.Equals(_3, item)) return true;
        if (count >= 5 && comparer.Equals(_4, item)) return true;
        if (count >= 6 && comparer.Equals(_5, item)) return true;
        if (count >= 7 && comparer.Equals(_6, item)) return true;
        if (count >= 8 && comparer.Equals(_7, item)) return true;
        if (count >= 9 && comparer.Equals(_8, item)) return true;
        if (count >= 10 && comparer.Equals(_9, item)) return true;
        if (count >= 11 && comparer.Equals(_10, item)) return true;
        if (count >= 12 && comparer.Equals(_11, item)) return true;
        if (count >= 13 && comparer.Equals(_12, item)) return true;
        if (count >= 14 && comparer.Equals(_13, item)) return true;
        if (count >= 15 && comparer.Equals(_14, item)) return true;
        if (count >= 16 && comparer.Equals(_15, item)) return true;
        if (rest != null) return rest.Contains(item);
        return false;
    }

    /// <summary>Determines the index of <paramref name="item"/>.</summary>
    /// <returns>The index of item if found in the list; otherwise, -1.</returns>
    public int IndexOf(T item)
    {
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return 0;
        if (count >= 2 && comparer.Equals(_1, item)) return 1;
        if (count >= 3 && comparer.Equals(_2, item)) return 2;
        if (count >= 4 && comparer.Equals(_3, item)) return 3;
        if (count >= 5 && comparer.Equals(_4, item)) return 4;
        if (count >= 6 && comparer.Equals(_5, item)) return 5;
        if (count >= 7 && comparer.Equals(_6, item)) return 6;
        if (count >= 8 && comparer.Equals(_7, item)) return 7;
        if (count >= 9 && comparer.Equals(_8, item)) return 8;
        if (count >= 10 && comparer.Equals(_9, item)) return 9;
        if (count >= 11 && comparer.Equals(_10, item)) return 10;
        if (count >= 12 && comparer.Equals(_11, item)) return 11;
        if (count >= 13 && comparer.Equals(_12, item)) return 12;
        if (count >= 14 && comparer.Equals(_13, item)) return 13;
        if (count >= 15 && comparer.Equals(_14, item)) return 14;
        if (count >= 16 && comparer.Equals(_15, item)) return 15;
        if (rest != null) return rest.IndexOf(item)-StackCount;
        return -1;
    }

    /// <summary>Inserts an <paramref name="item"/> to the StructList16`1 at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="item">The object to insert.</param>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList16`1.</exception>
    public void Insert(int index, T item)
    {
        if (index < 0 || index > count) throw new ArgumentOutOfRangeException();
        if (index >= 16) { if (rest == null) rest = new List<T>(); rest.Insert(index - StackCount, item); }
        if (index <= 15 && count >= 16) { if (rest == null) rest = new List<T>(); rest.Insert(0, _15); }
        if (index <= 14 && count >= 15) _15 = _14;
        if (index <= 13 && count >= 14) _14 = _13;
        if (index <= 12 && count >= 13) _13 = _12;
        if (index <= 11 && count >= 12) _12 = _11;
        if (index <= 10 && count >= 11) _11 = _10;
        if (index <= 9 && count >= 10) _10 = _9;
        if (index <= 8 && count >= 9) _9 = _8;
        if (index <= 7 && count >= 8) _8 = _7;
        if (index <= 6 && count >= 7) _7 = _6;
        if (index <= 5 && count >= 6) _6 = _5;
        if (index <= 4 && count >= 5) _5 = _4;
        if (index <= 3 && count >= 4) _4 = _3;
        if (index <= 2 && count >= 3) _3 = _2;
        if (index <= 1 && count >= 2) _2 = _1;
        if (index <= 0 && count >= 1) _1 = _0;

        count++;
        this[index] = item;
    }

    /// <summary>Copies the elements to <paramref name="array"/>, starting at <paramref name="arrayIndex"/>.</summary>
    /// <param name="array">The one-dimensional System.Array.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <exception cref="System.ArgumentNullException">array is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
    /// <exception cref="System.ArgumentException">The number of elements in the source StructList16`1 is greater than the available space from arrayIndex to the end of the destination array.</exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
        if (count > array.Length + arrayIndex) throw new ArgumentException();

        if (count >= 1) array[arrayIndex++] = _0;
        if (count >= 2) array[arrayIndex++] = _1;
        if (count >= 3) array[arrayIndex++] = _2;
        if (count >= 4) array[arrayIndex++] = _3;
        if (count >= 5) array[arrayIndex++] = _4;
        if (count >= 6) array[arrayIndex++] = _5;
        if (count >= 7) array[arrayIndex++] = _6;
        if (count >= 8) array[arrayIndex++] = _7;
        if (count >= 9) array[arrayIndex++] = _8;
        if (count >= 10) array[arrayIndex++] = _9;
        if (count >= 11) array[arrayIndex++] = _10;
        if (count >= 12) array[arrayIndex++] = _11;
        if (count >= 13) array[arrayIndex++] = _12;
        if (count >= 14) array[arrayIndex++] = _13;
        if (count >= 15) array[arrayIndex++] = _14;
        if (count >= 16) array[arrayIndex++] = _15;
        if (rest != null) rest.CopyTo(array, arrayIndex);
    }

    /// <summary>Create array.</summary>
    public T[] ToArray()
    {
        // Return empty singleton
        if (count == 0) return Array.Empty<T>();
        // Create array
        T[] result = new T[count];
        if (count >= 1) result[0] = _0;
        if (count >= 2) result[1] = _1;
        if (count >= 3) result[2] = _2;
        if (count >= 4) result[3] = _3;
        if (count >= 5) result[4] = _4;
        if (count >= 6) result[5] = _5;
        if (count >= 7) result[6] = _6;
        if (count >= 8) result[7] = _7;
        if (count >= 9) result[8] = _8;
        if (count >= 10) result[9] = _9;
        if (count >= 11) result[10] = _10;
        if (count >= 12) result[11] = _11;
        if (count >= 13) result[12] = _12;
        if (count >= 14) result[13] = _13;
        if (count >= 15) result[14] = _14;
        if (count >= 16) result[15] = _15;
        if (count > 16)
        {
            for (int i = 16; i < count; i++)
                result[i] = rest![i-16];
        }
        // Return array
        return result;
    }

    /// <summary>Create array with elements reversed.</summary>
    public T[] ToReverseArray()
    {
        T[] result = new T[count];
        if (count >= 1) result[count-1] = _0;
        if (count >= 2) result[count-2] = _1;
        if (count >= 3) result[count-3] = _2;
        if (count >= 4) result[count-4] = _3;
        if (count >= 5) result[count-5] = _4;
        if (count >= 6) result[count-6] = _5;
        if (count >= 7) result[count-7] = _6;
        if (count >= 8) result[count-8] = _7;
        if (count >= 9) result[count-9] = _8;
        if (count >= 10) result[count-10] = _9;
        if (count >= 11) result[count-11] = _10;
        if (count >= 12) result[count-12] = _11;
        if (count >= 13) result[count-13] = _12;
        if (count >= 14) result[count-14] = _13;
        if (count >= 15) result[count-15] = _14;
        if (count >= 16) result[count-16] = _15;
        if (count > 16)
        {
            for (int i = 16; i < count; i++)
                result[count-1-i] = rest![i-16];
        }
        return result;
    }

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)ToArray()).GetEnumerator();

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)ToArray()).GetEnumerator();
}


/// <summary>A list where the first 20 element(s) are stack allocated, and rest are allocated from heap when needed.</summary>
/// <typeparam name="T"></typeparam>
public struct StructList20<T> : IList<T>
{
    /// <summary>The number of elements that are stack allocated.</summary>
    public int StackCount => 20;

    /// <summary>Number of elements in the list</summary>
    public int Count => count;
    /// <summary>Is list readonly</summary>
    public bool IsReadOnly => false;

    /// <summary>Number of elements</summary>
    int count;
    /// <summary>First elements</summary>
    T _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14, _15, _16, _17, _18, _19;
    /// <summary>Elements after <see cref="StackCount"/>.</summary>
    List<T>? rest;
    /// <summary>Element comparer</summary>
    IEqualityComparer<T>? elementComparer;
    /// <summary>Element comparer</summary>
    public IEqualityComparer<T> ElementComparer => elementComparer ?? EqualityComparer<T>.Default;

    /// <summary>Create struct list.</summary>
    /// <param name="elementComparer"></param>
    public StructList20(IEqualityComparer<T>? elementComparer = null)
    {
        this.elementComparer = elementComparer ?? EqualityComparer<T>.Default;
        count = 0;

        _0 = default!;
        _1 = default!;
        _2 = default!;
        _3 = default!;
        _4 = default!;
        _5 = default!;
        _6 = default!;
        _7 = default!;
        _8 = default!;
        _9 = default!;
        _10 = default!;
        _11 = default!;
        _12 = default!;
        _13 = default!;
        _14 = default!;
        _15 = default!;
        _16 = default!;
        _17 = default!;
        _18 = default!;
        _19 = default!;
        rest = null;
    }

    /// <summary>Create struct list.</summary>
    /// <param name="enumr"></param>
    public StructList20(IEnumerable<T> enumr) : this(elementComparer: default)
    {
        foreach(T value in enumr) Add(value);
    }

    /// <summary>Gets or sets the element at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>element</returns>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList20`1.</exception>
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: return _0;
                case 1: return _1;
                case 2: return _2;
                case 3: return _3;
                case 4: return _4;
                case 5: return _5;
                case 6: return _6;
                case 7: return _7;
                case 8: return _8;
                case 9: return _9;
                case 10: return _10;
                case 11: return _11;
                case 12: return _12;
                case 13: return _13;
                case 14: return _14;
                case 15: return _15;
                case 16: return _16;
                case 17: return _17;
                case 18: return _18;
                case 19: return _19;
                default: return rest![index - StackCount];
            }
        }
        set
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: _0 = value; return;
                case 1: _1 = value; return;
                case 2: _2 = value; return;
                case 3: _3 = value; return;
                case 4: _4 = value; return;
                case 5: _5 = value; return;
                case 6: _6 = value; return;
                case 7: _7 = value; return;
                case 8: _8 = value; return;
                case 9: _9 = value; return;
                case 10: _10 = value; return;
                case 11: _11 = value; return;
                case 12: _12 = value; return;
                case 13: _13 = value; return;
                case 14: _14 = value; return;
                case 15: _15 = value; return;
                case 16: _16 = value; return;
                case 17: _17 = value; return;
                case 18: _18 = value; return;
                case 19: _19 = value; return;
                default: rest![index - StackCount] = value; return;
            }
        }
    }

    /// <summary>Get reference to element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static ref T GetRef(ref StructList20<T> list, int index)
    {
        if (index < 0 || index >= list.Count) throw new ArgumentOutOfRangeException();
        switch (index)
        {
            case 0: return ref list._0;
            case 1: return ref list._1;
            case 2: return ref list._2;
            case 3: return ref list._3;
            case 4: return ref list._4;
            case 5: return ref list._5;
            case 6: return ref list._6;
            case 7: return ref list._7;
            case 8: return ref list._8;
            case 9: return ref list._9;
            case 10: return ref list._10;
            case 11: return ref list._11;
            case 12: return ref list._12;
            case 13: return ref list._13;
            case 14: return ref list._14;
            case 15: return ref list._15;
            case 16: return ref list._16;
            case 17: return ref list._17;
            case 18: return ref list._18;
            case 19: return ref list._19;
            default:
                Span<T> span = CollectionsMarshal.AsSpan<T>(list.rest);
                return ref span[index - 20];
        }
    }

    /// <summary>Add <paramref name="item"/> to the StructList20`1.</summary>
    /// <exception cref="System.NotSupportedException">The StructList20`1 is read-only.</exception>
    public void Add(T item)
    {
        switch(count)
        {
            case 0: _0 = item; break;
            case 1: _1 = item; break;
            case 2: _2 = item; break;
            case 3: _3 = item; break;
            case 4: _4 = item; break;
            case 5: _5 = item; break;
            case 6: _6 = item; break;
            case 7: _7 = item; break;
            case 8: _8 = item; break;
            case 9: _9 = item; break;
            case 10: _10 = item; break;
            case 11: _11 = item; break;
            case 12: _12 = item; break;
            case 13: _13 = item; break;
            case 14: _14 = item; break;
            case 15: _15 = item; break;
            case 16: _16 = item; break;
            case 17: _17 = item; break;
            case 18: _18 = item; break;
            case 19: _19 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }

        count++;
        return;
    }

    /// <summary>Adds <paramref name="items"/>.</summary>
    /// <exception cref="System.NotSupportedException">The StructList20`1 is read-only.</exception>
    public void AddRange(IEnumerable<T> items)
    {
        foreach(T item in items)
        {
            switch(count)
            {
                case 0: _0 = item; break;
                case 1: _1 = item; break;
                case 2: _2 = item; break;
                case 3: _3 = item; break;
                case 4: _4 = item; break;
                case 5: _5 = item; break;
                case 6: _6 = item; break;
                case 7: _7 = item; break;
                case 8: _8 = item; break;
                case 9: _9 = item; break;
                case 10: _10 = item; break;
                case 11: _11 = item; break;
                case 12: _12 = item; break;
                case 13: _13 = item; break;
                case 14: _14 = item; break;
                case 15: _15 = item; break;
                case 16: _16 = item; break;
                case 17: _17 = item; break;
                case 18: _18 = item; break;
                case 19: _19 = item; break;
                default:
                    if (rest == null) rest = new List<T>();
                    rest.Add(item);
                    break;
            }
            count++;
        }
    }

    /// <summary>Add <paramref name="item"/>, if the item isn't already in the list.</summary>
    /// <exception cref="System.NotSupportedException">The StructList20`1 is read-only.</exception>
    public void AddIfNew(T item)
    {
        if (Contains(item)) return;
        switch(count)
        {
            case 0: _0 = item; break;
            case 1: _1 = item; break;
            case 2: _2 = item; break;
            case 3: _3 = item; break;
            case 4: _4 = item; break;
            case 5: _5 = item; break;
            case 6: _6 = item; break;
            case 7: _7 = item; break;
            case 8: _8 = item; break;
            case 9: _9 = item; break;
            case 10: _10 = item; break;
            case 11: _11 = item; break;
            case 12: _12 = item; break;
            case 13: _13 = item; break;
            case 14: _14 = item; break;
            case 15: _15 = item; break;
            case 16: _16 = item; break;
            case 17: _17 = item; break;
            case 18: _18 = item; break;
            case 19: _19 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }
        count++; 
    }

    /// <summary>Removes the first occurrence of <paramref name="item"/>.</summary>
    /// <returns>true if item was successfully removed from the StructList20`1; otherwise, false. This method also returns false if item is not found in the original StructList20`1.</returns>
    public bool Remove(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) { RemoveAt(0); return true; }
        if (count >= 2 && comparer.Equals(_1, item)) { RemoveAt(1); return true; }
        if (count >= 3 && comparer.Equals(_2, item)) { RemoveAt(2); return true; }
        if (count >= 4 && comparer.Equals(_3, item)) { RemoveAt(3); return true; }
        if (count >= 5 && comparer.Equals(_4, item)) { RemoveAt(4); return true; }
        if (count >= 6 && comparer.Equals(_5, item)) { RemoveAt(5); return true; }
        if (count >= 7 && comparer.Equals(_6, item)) { RemoveAt(6); return true; }
        if (count >= 8 && comparer.Equals(_7, item)) { RemoveAt(7); return true; }
        if (count >= 9 && comparer.Equals(_8, item)) { RemoveAt(8); return true; }
        if (count >= 10 && comparer.Equals(_9, item)) { RemoveAt(9); return true; }
        if (count >= 11 && comparer.Equals(_10, item)) { RemoveAt(10); return true; }
        if (count >= 12 && comparer.Equals(_11, item)) { RemoveAt(11); return true; }
        if (count >= 13 && comparer.Equals(_12, item)) { RemoveAt(12); return true; }
        if (count >= 14 && comparer.Equals(_13, item)) { RemoveAt(13); return true; }
        if (count >= 15 && comparer.Equals(_14, item)) { RemoveAt(14); return true; }
        if (count >= 16 && comparer.Equals(_15, item)) { RemoveAt(15); return true; }
        if (count >= 17 && comparer.Equals(_16, item)) { RemoveAt(16); return true; }
        if (count >= 18 && comparer.Equals(_17, item)) { RemoveAt(17); return true; }
        if (count >= 19 && comparer.Equals(_18, item)) { RemoveAt(18); return true; }
        if (count >= 20 && comparer.Equals(_19, item)) { RemoveAt(19); return true; }

        if (rest == null) return false;
        foreach(T e in rest) if (comparer.Equals(e, item)) { bool removed = rest.Remove(item); if (removed) count--; return removed; }
        return false;
    }

    /// <summary>Removes element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList20`1.</exception>
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
        if (index <= 0 && count > 1) _0 = _1;
        if (index <= 1 && count > 2) _1 = _2;
        if (index <= 2 && count > 3) _2 = _3;
        if (index <= 3 && count > 4) _3 = _4;
        if (index <= 4 && count > 5) _4 = _5;
        if (index <= 5 && count > 6) _5 = _6;
        if (index <= 6 && count > 7) _6 = _7;
        if (index <= 7 && count > 8) _7 = _8;
        if (index <= 8 && count > 9) _8 = _9;
        if (index <= 9 && count > 10) _9 = _10;
        if (index <= 10 && count > 11) _10 = _11;
        if (index <= 11 && count > 12) _11 = _12;
        if (index <= 12 && count > 13) _12 = _13;
        if (index <= 13 && count > 14) _13 = _14;
        if (index <= 14 && count > 15) _14 = _15;
        if (index <= 15 && count > 16) _15 = _16;
        if (index <= 16 && count > 17) _16 = _17;
        if (index <= 17 && count > 18) _17 = _18;
        if (index <= 18 && count > 19) _18 = _19;
        if (index <= 19 && count > 20) { _19 = rest![0]; rest!.RemoveAt(0); }
        if (index >= StackCount) rest!.RemoveAt(index - StackCount);
        count--;
    }

    /// <summary>Removes and returns the element at the end of the list.</summary>
    /// <returns>the last element</returns>
    /// <exception cref="InvalidOperationException">If list is empty</exception>
    public T Dequeue()
    {
        if (count == 0) throw new InvalidOperationException();
        int ix = count - 1;
        T result = this[ix];
        RemoveAt(ix);
        return result;
    }

    /// <summary>Remove all elements.</summary>
    /// <exception cref="System.NotSupportedException">The StructList20`1 is read-only.</exception>
    public void Clear()
    {
        if (count >= 1) _0 = default!;
        if (count >= 2) _1 = default!;
        if (count >= 3) _2 = default!;
        if (count >= 4) _3 = default!;
        if (count >= 5) _4 = default!;
        if (count >= 6) _5 = default!;
        if (count >= 7) _6 = default!;
        if (count >= 8) _7 = default!;
        if (count >= 9) _8 = default!;
        if (count >= 10) _9 = default!;
        if (count >= 11) _10 = default!;
        if (count >= 12) _11 = default!;
        if (count >= 13) _12 = default!;
        if (count >= 14) _13 = default!;
        if (count >= 15) _14 = default!;
        if (count >= 16) _15 = default!;
        if (count >= 17) _16 = default!;
        if (count >= 18) _17 = default!;
        if (count >= 19) _18 = default!;
        if (count >= 20) _19 = default!;
        if (rest != null) rest.Clear();
        count = 0;
    }

    /// <summary>Determine whether <paramref name="item"/> is in the list.</summary>
    /// <returns>true if item is found in the StructList20`1; otherwise, false.</returns>
    public bool Contains(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return true;
        if (count >= 2 && comparer.Equals(_1, item)) return true;
        if (count >= 3 && comparer.Equals(_2, item)) return true;
        if (count >= 4 && comparer.Equals(_3, item)) return true;
        if (count >= 5 && comparer.Equals(_4, item)) return true;
        if (count >= 6 && comparer.Equals(_5, item)) return true;
        if (count >= 7 && comparer.Equals(_6, item)) return true;
        if (count >= 8 && comparer.Equals(_7, item)) return true;
        if (count >= 9 && comparer.Equals(_8, item)) return true;
        if (count >= 10 && comparer.Equals(_9, item)) return true;
        if (count >= 11 && comparer.Equals(_10, item)) return true;
        if (count >= 12 && comparer.Equals(_11, item)) return true;
        if (count >= 13 && comparer.Equals(_12, item)) return true;
        if (count >= 14 && comparer.Equals(_13, item)) return true;
        if (count >= 15 && comparer.Equals(_14, item)) return true;
        if (count >= 16 && comparer.Equals(_15, item)) return true;
        if (count >= 17 && comparer.Equals(_16, item)) return true;
        if (count >= 18 && comparer.Equals(_17, item)) return true;
        if (count >= 19 && comparer.Equals(_18, item)) return true;
        if (count >= 20 && comparer.Equals(_19, item)) return true;
        if (rest != null) return rest.Contains(item);
        return false;
    }

    /// <summary>Determines the index of <paramref name="item"/>.</summary>
    /// <returns>The index of item if found in the list; otherwise, -1.</returns>
    public int IndexOf(T item)
    {
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return 0;
        if (count >= 2 && comparer.Equals(_1, item)) return 1;
        if (count >= 3 && comparer.Equals(_2, item)) return 2;
        if (count >= 4 && comparer.Equals(_3, item)) return 3;
        if (count >= 5 && comparer.Equals(_4, item)) return 4;
        if (count >= 6 && comparer.Equals(_5, item)) return 5;
        if (count >= 7 && comparer.Equals(_6, item)) return 6;
        if (count >= 8 && comparer.Equals(_7, item)) return 7;
        if (count >= 9 && comparer.Equals(_8, item)) return 8;
        if (count >= 10 && comparer.Equals(_9, item)) return 9;
        if (count >= 11 && comparer.Equals(_10, item)) return 10;
        if (count >= 12 && comparer.Equals(_11, item)) return 11;
        if (count >= 13 && comparer.Equals(_12, item)) return 12;
        if (count >= 14 && comparer.Equals(_13, item)) return 13;
        if (count >= 15 && comparer.Equals(_14, item)) return 14;
        if (count >= 16 && comparer.Equals(_15, item)) return 15;
        if (count >= 17 && comparer.Equals(_16, item)) return 16;
        if (count >= 18 && comparer.Equals(_17, item)) return 17;
        if (count >= 19 && comparer.Equals(_18, item)) return 18;
        if (count >= 20 && comparer.Equals(_19, item)) return 19;
        if (rest != null) return rest.IndexOf(item)-StackCount;
        return -1;
    }

    /// <summary>Inserts an <paramref name="item"/> to the StructList20`1 at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="item">The object to insert.</param>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList20`1.</exception>
    public void Insert(int index, T item)
    {
        if (index < 0 || index > count) throw new ArgumentOutOfRangeException();
        if (index >= 20) { if (rest == null) rest = new List<T>(); rest.Insert(index - StackCount, item); }
        if (index <= 19 && count >= 20) { if (rest == null) rest = new List<T>(); rest.Insert(0, _19); }
        if (index <= 18 && count >= 19) _19 = _18;
        if (index <= 17 && count >= 18) _18 = _17;
        if (index <= 16 && count >= 17) _17 = _16;
        if (index <= 15 && count >= 16) _16 = _15;
        if (index <= 14 && count >= 15) _15 = _14;
        if (index <= 13 && count >= 14) _14 = _13;
        if (index <= 12 && count >= 13) _13 = _12;
        if (index <= 11 && count >= 12) _12 = _11;
        if (index <= 10 && count >= 11) _11 = _10;
        if (index <= 9 && count >= 10) _10 = _9;
        if (index <= 8 && count >= 9) _9 = _8;
        if (index <= 7 && count >= 8) _8 = _7;
        if (index <= 6 && count >= 7) _7 = _6;
        if (index <= 5 && count >= 6) _6 = _5;
        if (index <= 4 && count >= 5) _5 = _4;
        if (index <= 3 && count >= 4) _4 = _3;
        if (index <= 2 && count >= 3) _3 = _2;
        if (index <= 1 && count >= 2) _2 = _1;
        if (index <= 0 && count >= 1) _1 = _0;

        count++;
        this[index] = item;
    }

    /// <summary>Copies the elements to <paramref name="array"/>, starting at <paramref name="arrayIndex"/>.</summary>
    /// <param name="array">The one-dimensional System.Array.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <exception cref="System.ArgumentNullException">array is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
    /// <exception cref="System.ArgumentException">The number of elements in the source StructList20`1 is greater than the available space from arrayIndex to the end of the destination array.</exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
        if (count > array.Length + arrayIndex) throw new ArgumentException();

        if (count >= 1) array[arrayIndex++] = _0;
        if (count >= 2) array[arrayIndex++] = _1;
        if (count >= 3) array[arrayIndex++] = _2;
        if (count >= 4) array[arrayIndex++] = _3;
        if (count >= 5) array[arrayIndex++] = _4;
        if (count >= 6) array[arrayIndex++] = _5;
        if (count >= 7) array[arrayIndex++] = _6;
        if (count >= 8) array[arrayIndex++] = _7;
        if (count >= 9) array[arrayIndex++] = _8;
        if (count >= 10) array[arrayIndex++] = _9;
        if (count >= 11) array[arrayIndex++] = _10;
        if (count >= 12) array[arrayIndex++] = _11;
        if (count >= 13) array[arrayIndex++] = _12;
        if (count >= 14) array[arrayIndex++] = _13;
        if (count >= 15) array[arrayIndex++] = _14;
        if (count >= 16) array[arrayIndex++] = _15;
        if (count >= 17) array[arrayIndex++] = _16;
        if (count >= 18) array[arrayIndex++] = _17;
        if (count >= 19) array[arrayIndex++] = _18;
        if (count >= 20) array[arrayIndex++] = _19;
        if (rest != null) rest.CopyTo(array, arrayIndex);
    }

    /// <summary>Create array.</summary>
    public T[] ToArray()
    {
        // Return empty singleton
        if (count == 0) return Array.Empty<T>();
        // Create array
        T[] result = new T[count];
        if (count >= 1) result[0] = _0;
        if (count >= 2) result[1] = _1;
        if (count >= 3) result[2] = _2;
        if (count >= 4) result[3] = _3;
        if (count >= 5) result[4] = _4;
        if (count >= 6) result[5] = _5;
        if (count >= 7) result[6] = _6;
        if (count >= 8) result[7] = _7;
        if (count >= 9) result[8] = _8;
        if (count >= 10) result[9] = _9;
        if (count >= 11) result[10] = _10;
        if (count >= 12) result[11] = _11;
        if (count >= 13) result[12] = _12;
        if (count >= 14) result[13] = _13;
        if (count >= 15) result[14] = _14;
        if (count >= 16) result[15] = _15;
        if (count >= 17) result[16] = _16;
        if (count >= 18) result[17] = _17;
        if (count >= 19) result[18] = _18;
        if (count >= 20) result[19] = _19;
        if (count > 20)
        {
            for (int i = 20; i < count; i++)
                result[i] = rest![i-20];
        }
        // Return array
        return result;
    }

    /// <summary>Create array with elements reversed.</summary>
    public T[] ToReverseArray()
    {
        T[] result = new T[count];
        if (count >= 1) result[count-1] = _0;
        if (count >= 2) result[count-2] = _1;
        if (count >= 3) result[count-3] = _2;
        if (count >= 4) result[count-4] = _3;
        if (count >= 5) result[count-5] = _4;
        if (count >= 6) result[count-6] = _5;
        if (count >= 7) result[count-7] = _6;
        if (count >= 8) result[count-8] = _7;
        if (count >= 9) result[count-9] = _8;
        if (count >= 10) result[count-10] = _9;
        if (count >= 11) result[count-11] = _10;
        if (count >= 12) result[count-12] = _11;
        if (count >= 13) result[count-13] = _12;
        if (count >= 14) result[count-14] = _13;
        if (count >= 15) result[count-15] = _14;
        if (count >= 16) result[count-16] = _15;
        if (count >= 17) result[count-17] = _16;
        if (count >= 18) result[count-18] = _17;
        if (count >= 19) result[count-19] = _18;
        if (count >= 20) result[count-20] = _19;
        if (count > 20)
        {
            for (int i = 20; i < count; i++)
                result[count-1-i] = rest![i-20];
        }
        return result;
    }

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)ToArray()).GetEnumerator();

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)ToArray()).GetEnumerator();
}


/// <summary>A list where the first 24 element(s) are stack allocated, and rest are allocated from heap when needed.</summary>
/// <typeparam name="T"></typeparam>
public struct StructList24<T> : IList<T>
{
    /// <summary>The number of elements that are stack allocated.</summary>
    public int StackCount => 24;

    /// <summary>Number of elements in the list</summary>
    public int Count => count;
    /// <summary>Is list readonly</summary>
    public bool IsReadOnly => false;

    /// <summary>Number of elements</summary>
    int count;
    /// <summary>First elements</summary>
    T _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14, _15, _16, _17, _18, _19, _20, _21, _22, _23;
    /// <summary>Elements after <see cref="StackCount"/>.</summary>
    List<T>? rest;
    /// <summary>Element comparer</summary>
    IEqualityComparer<T>? elementComparer;
    /// <summary>Element comparer</summary>
    public IEqualityComparer<T> ElementComparer => elementComparer ?? EqualityComparer<T>.Default;

    /// <summary>Create struct list.</summary>
    /// <param name="elementComparer"></param>
    public StructList24(IEqualityComparer<T>? elementComparer = null)
    {
        this.elementComparer = elementComparer ?? EqualityComparer<T>.Default;
        count = 0;

        _0 = default!;
        _1 = default!;
        _2 = default!;
        _3 = default!;
        _4 = default!;
        _5 = default!;
        _6 = default!;
        _7 = default!;
        _8 = default!;
        _9 = default!;
        _10 = default!;
        _11 = default!;
        _12 = default!;
        _13 = default!;
        _14 = default!;
        _15 = default!;
        _16 = default!;
        _17 = default!;
        _18 = default!;
        _19 = default!;
        _20 = default!;
        _21 = default!;
        _22 = default!;
        _23 = default!;
        rest = null;
    }

    /// <summary>Create struct list.</summary>
    /// <param name="enumr"></param>
    public StructList24(IEnumerable<T> enumr) : this(elementComparer: default)
    {
        foreach(T value in enumr) Add(value);
    }

    /// <summary>Gets or sets the element at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>element</returns>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList24`1.</exception>
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: return _0;
                case 1: return _1;
                case 2: return _2;
                case 3: return _3;
                case 4: return _4;
                case 5: return _5;
                case 6: return _6;
                case 7: return _7;
                case 8: return _8;
                case 9: return _9;
                case 10: return _10;
                case 11: return _11;
                case 12: return _12;
                case 13: return _13;
                case 14: return _14;
                case 15: return _15;
                case 16: return _16;
                case 17: return _17;
                case 18: return _18;
                case 19: return _19;
                case 20: return _20;
                case 21: return _21;
                case 22: return _22;
                case 23: return _23;
                default: return rest![index - StackCount];
            }
        }
        set
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: _0 = value; return;
                case 1: _1 = value; return;
                case 2: _2 = value; return;
                case 3: _3 = value; return;
                case 4: _4 = value; return;
                case 5: _5 = value; return;
                case 6: _6 = value; return;
                case 7: _7 = value; return;
                case 8: _8 = value; return;
                case 9: _9 = value; return;
                case 10: _10 = value; return;
                case 11: _11 = value; return;
                case 12: _12 = value; return;
                case 13: _13 = value; return;
                case 14: _14 = value; return;
                case 15: _15 = value; return;
                case 16: _16 = value; return;
                case 17: _17 = value; return;
                case 18: _18 = value; return;
                case 19: _19 = value; return;
                case 20: _20 = value; return;
                case 21: _21 = value; return;
                case 22: _22 = value; return;
                case 23: _23 = value; return;
                default: rest![index - StackCount] = value; return;
            }
        }
    }

    /// <summary>Get reference to element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static ref T GetRef(ref StructList24<T> list, int index)
    {
        if (index < 0 || index >= list.Count) throw new ArgumentOutOfRangeException();
        switch (index)
        {
            case 0: return ref list._0;
            case 1: return ref list._1;
            case 2: return ref list._2;
            case 3: return ref list._3;
            case 4: return ref list._4;
            case 5: return ref list._5;
            case 6: return ref list._6;
            case 7: return ref list._7;
            case 8: return ref list._8;
            case 9: return ref list._9;
            case 10: return ref list._10;
            case 11: return ref list._11;
            case 12: return ref list._12;
            case 13: return ref list._13;
            case 14: return ref list._14;
            case 15: return ref list._15;
            case 16: return ref list._16;
            case 17: return ref list._17;
            case 18: return ref list._18;
            case 19: return ref list._19;
            case 20: return ref list._20;
            case 21: return ref list._21;
            case 22: return ref list._22;
            case 23: return ref list._23;
            default:
                Span<T> span = CollectionsMarshal.AsSpan<T>(list.rest);
                return ref span[index - 24];
        }
    }

    /// <summary>Add <paramref name="item"/> to the StructList24`1.</summary>
    /// <exception cref="System.NotSupportedException">The StructList24`1 is read-only.</exception>
    public void Add(T item)
    {
        switch(count)
        {
            case 0: _0 = item; break;
            case 1: _1 = item; break;
            case 2: _2 = item; break;
            case 3: _3 = item; break;
            case 4: _4 = item; break;
            case 5: _5 = item; break;
            case 6: _6 = item; break;
            case 7: _7 = item; break;
            case 8: _8 = item; break;
            case 9: _9 = item; break;
            case 10: _10 = item; break;
            case 11: _11 = item; break;
            case 12: _12 = item; break;
            case 13: _13 = item; break;
            case 14: _14 = item; break;
            case 15: _15 = item; break;
            case 16: _16 = item; break;
            case 17: _17 = item; break;
            case 18: _18 = item; break;
            case 19: _19 = item; break;
            case 20: _20 = item; break;
            case 21: _21 = item; break;
            case 22: _22 = item; break;
            case 23: _23 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }

        count++;
        return;
    }

    /// <summary>Adds <paramref name="items"/>.</summary>
    /// <exception cref="System.NotSupportedException">The StructList24`1 is read-only.</exception>
    public void AddRange(IEnumerable<T> items)
    {
        foreach(T item in items)
        {
            switch(count)
            {
                case 0: _0 = item; break;
                case 1: _1 = item; break;
                case 2: _2 = item; break;
                case 3: _3 = item; break;
                case 4: _4 = item; break;
                case 5: _5 = item; break;
                case 6: _6 = item; break;
                case 7: _7 = item; break;
                case 8: _8 = item; break;
                case 9: _9 = item; break;
                case 10: _10 = item; break;
                case 11: _11 = item; break;
                case 12: _12 = item; break;
                case 13: _13 = item; break;
                case 14: _14 = item; break;
                case 15: _15 = item; break;
                case 16: _16 = item; break;
                case 17: _17 = item; break;
                case 18: _18 = item; break;
                case 19: _19 = item; break;
                case 20: _20 = item; break;
                case 21: _21 = item; break;
                case 22: _22 = item; break;
                case 23: _23 = item; break;
                default:
                    if (rest == null) rest = new List<T>();
                    rest.Add(item);
                    break;
            }
            count++;
        }
    }

    /// <summary>Add <paramref name="item"/>, if the item isn't already in the list.</summary>
    /// <exception cref="System.NotSupportedException">The StructList24`1 is read-only.</exception>
    public void AddIfNew(T item)
    {
        if (Contains(item)) return;
        switch(count)
        {
            case 0: _0 = item; break;
            case 1: _1 = item; break;
            case 2: _2 = item; break;
            case 3: _3 = item; break;
            case 4: _4 = item; break;
            case 5: _5 = item; break;
            case 6: _6 = item; break;
            case 7: _7 = item; break;
            case 8: _8 = item; break;
            case 9: _9 = item; break;
            case 10: _10 = item; break;
            case 11: _11 = item; break;
            case 12: _12 = item; break;
            case 13: _13 = item; break;
            case 14: _14 = item; break;
            case 15: _15 = item; break;
            case 16: _16 = item; break;
            case 17: _17 = item; break;
            case 18: _18 = item; break;
            case 19: _19 = item; break;
            case 20: _20 = item; break;
            case 21: _21 = item; break;
            case 22: _22 = item; break;
            case 23: _23 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }
        count++; 
    }

    /// <summary>Removes the first occurrence of <paramref name="item"/>.</summary>
    /// <returns>true if item was successfully removed from the StructList24`1; otherwise, false. This method also returns false if item is not found in the original StructList24`1.</returns>
    public bool Remove(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) { RemoveAt(0); return true; }
        if (count >= 2 && comparer.Equals(_1, item)) { RemoveAt(1); return true; }
        if (count >= 3 && comparer.Equals(_2, item)) { RemoveAt(2); return true; }
        if (count >= 4 && comparer.Equals(_3, item)) { RemoveAt(3); return true; }
        if (count >= 5 && comparer.Equals(_4, item)) { RemoveAt(4); return true; }
        if (count >= 6 && comparer.Equals(_5, item)) { RemoveAt(5); return true; }
        if (count >= 7 && comparer.Equals(_6, item)) { RemoveAt(6); return true; }
        if (count >= 8 && comparer.Equals(_7, item)) { RemoveAt(7); return true; }
        if (count >= 9 && comparer.Equals(_8, item)) { RemoveAt(8); return true; }
        if (count >= 10 && comparer.Equals(_9, item)) { RemoveAt(9); return true; }
        if (count >= 11 && comparer.Equals(_10, item)) { RemoveAt(10); return true; }
        if (count >= 12 && comparer.Equals(_11, item)) { RemoveAt(11); return true; }
        if (count >= 13 && comparer.Equals(_12, item)) { RemoveAt(12); return true; }
        if (count >= 14 && comparer.Equals(_13, item)) { RemoveAt(13); return true; }
        if (count >= 15 && comparer.Equals(_14, item)) { RemoveAt(14); return true; }
        if (count >= 16 && comparer.Equals(_15, item)) { RemoveAt(15); return true; }
        if (count >= 17 && comparer.Equals(_16, item)) { RemoveAt(16); return true; }
        if (count >= 18 && comparer.Equals(_17, item)) { RemoveAt(17); return true; }
        if (count >= 19 && comparer.Equals(_18, item)) { RemoveAt(18); return true; }
        if (count >= 20 && comparer.Equals(_19, item)) { RemoveAt(19); return true; }
        if (count >= 21 && comparer.Equals(_20, item)) { RemoveAt(20); return true; }
        if (count >= 22 && comparer.Equals(_21, item)) { RemoveAt(21); return true; }
        if (count >= 23 && comparer.Equals(_22, item)) { RemoveAt(22); return true; }
        if (count >= 24 && comparer.Equals(_23, item)) { RemoveAt(23); return true; }

        if (rest == null) return false;
        foreach(T e in rest) if (comparer.Equals(e, item)) { bool removed = rest.Remove(item); if (removed) count--; return removed; }
        return false;
    }

    /// <summary>Removes element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList24`1.</exception>
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
        if (index <= 0 && count > 1) _0 = _1;
        if (index <= 1 && count > 2) _1 = _2;
        if (index <= 2 && count > 3) _2 = _3;
        if (index <= 3 && count > 4) _3 = _4;
        if (index <= 4 && count > 5) _4 = _5;
        if (index <= 5 && count > 6) _5 = _6;
        if (index <= 6 && count > 7) _6 = _7;
        if (index <= 7 && count > 8) _7 = _8;
        if (index <= 8 && count > 9) _8 = _9;
        if (index <= 9 && count > 10) _9 = _10;
        if (index <= 10 && count > 11) _10 = _11;
        if (index <= 11 && count > 12) _11 = _12;
        if (index <= 12 && count > 13) _12 = _13;
        if (index <= 13 && count > 14) _13 = _14;
        if (index <= 14 && count > 15) _14 = _15;
        if (index <= 15 && count > 16) _15 = _16;
        if (index <= 16 && count > 17) _16 = _17;
        if (index <= 17 && count > 18) _17 = _18;
        if (index <= 18 && count > 19) _18 = _19;
        if (index <= 19 && count > 20) _19 = _20;
        if (index <= 20 && count > 21) _20 = _21;
        if (index <= 21 && count > 22) _21 = _22;
        if (index <= 22 && count > 23) _22 = _23;
        if (index <= 23 && count > 24) { _23 = rest![0]; rest!.RemoveAt(0); }
        if (index >= StackCount) rest!.RemoveAt(index - StackCount);
        count--;
    }

    /// <summary>Removes and returns the element at the end of the list.</summary>
    /// <returns>the last element</returns>
    /// <exception cref="InvalidOperationException">If list is empty</exception>
    public T Dequeue()
    {
        if (count == 0) throw new InvalidOperationException();
        int ix = count - 1;
        T result = this[ix];
        RemoveAt(ix);
        return result;
    }

    /// <summary>Remove all elements.</summary>
    /// <exception cref="System.NotSupportedException">The StructList24`1 is read-only.</exception>
    public void Clear()
    {
        if (count >= 1) _0 = default!;
        if (count >= 2) _1 = default!;
        if (count >= 3) _2 = default!;
        if (count >= 4) _3 = default!;
        if (count >= 5) _4 = default!;
        if (count >= 6) _5 = default!;
        if (count >= 7) _6 = default!;
        if (count >= 8) _7 = default!;
        if (count >= 9) _8 = default!;
        if (count >= 10) _9 = default!;
        if (count >= 11) _10 = default!;
        if (count >= 12) _11 = default!;
        if (count >= 13) _12 = default!;
        if (count >= 14) _13 = default!;
        if (count >= 15) _14 = default!;
        if (count >= 16) _15 = default!;
        if (count >= 17) _16 = default!;
        if (count >= 18) _17 = default!;
        if (count >= 19) _18 = default!;
        if (count >= 20) _19 = default!;
        if (count >= 21) _20 = default!;
        if (count >= 22) _21 = default!;
        if (count >= 23) _22 = default!;
        if (count >= 24) _23 = default!;
        if (rest != null) rest.Clear();
        count = 0;
    }

    /// <summary>Determine whether <paramref name="item"/> is in the list.</summary>
    /// <returns>true if item is found in the StructList24`1; otherwise, false.</returns>
    public bool Contains(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return true;
        if (count >= 2 && comparer.Equals(_1, item)) return true;
        if (count >= 3 && comparer.Equals(_2, item)) return true;
        if (count >= 4 && comparer.Equals(_3, item)) return true;
        if (count >= 5 && comparer.Equals(_4, item)) return true;
        if (count >= 6 && comparer.Equals(_5, item)) return true;
        if (count >= 7 && comparer.Equals(_6, item)) return true;
        if (count >= 8 && comparer.Equals(_7, item)) return true;
        if (count >= 9 && comparer.Equals(_8, item)) return true;
        if (count >= 10 && comparer.Equals(_9, item)) return true;
        if (count >= 11 && comparer.Equals(_10, item)) return true;
        if (count >= 12 && comparer.Equals(_11, item)) return true;
        if (count >= 13 && comparer.Equals(_12, item)) return true;
        if (count >= 14 && comparer.Equals(_13, item)) return true;
        if (count >= 15 && comparer.Equals(_14, item)) return true;
        if (count >= 16 && comparer.Equals(_15, item)) return true;
        if (count >= 17 && comparer.Equals(_16, item)) return true;
        if (count >= 18 && comparer.Equals(_17, item)) return true;
        if (count >= 19 && comparer.Equals(_18, item)) return true;
        if (count >= 20 && comparer.Equals(_19, item)) return true;
        if (count >= 21 && comparer.Equals(_20, item)) return true;
        if (count >= 22 && comparer.Equals(_21, item)) return true;
        if (count >= 23 && comparer.Equals(_22, item)) return true;
        if (count >= 24 && comparer.Equals(_23, item)) return true;
        if (rest != null) return rest.Contains(item);
        return false;
    }

    /// <summary>Determines the index of <paramref name="item"/>.</summary>
    /// <returns>The index of item if found in the list; otherwise, -1.</returns>
    public int IndexOf(T item)
    {
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return 0;
        if (count >= 2 && comparer.Equals(_1, item)) return 1;
        if (count >= 3 && comparer.Equals(_2, item)) return 2;
        if (count >= 4 && comparer.Equals(_3, item)) return 3;
        if (count >= 5 && comparer.Equals(_4, item)) return 4;
        if (count >= 6 && comparer.Equals(_5, item)) return 5;
        if (count >= 7 && comparer.Equals(_6, item)) return 6;
        if (count >= 8 && comparer.Equals(_7, item)) return 7;
        if (count >= 9 && comparer.Equals(_8, item)) return 8;
        if (count >= 10 && comparer.Equals(_9, item)) return 9;
        if (count >= 11 && comparer.Equals(_10, item)) return 10;
        if (count >= 12 && comparer.Equals(_11, item)) return 11;
        if (count >= 13 && comparer.Equals(_12, item)) return 12;
        if (count >= 14 && comparer.Equals(_13, item)) return 13;
        if (count >= 15 && comparer.Equals(_14, item)) return 14;
        if (count >= 16 && comparer.Equals(_15, item)) return 15;
        if (count >= 17 && comparer.Equals(_16, item)) return 16;
        if (count >= 18 && comparer.Equals(_17, item)) return 17;
        if (count >= 19 && comparer.Equals(_18, item)) return 18;
        if (count >= 20 && comparer.Equals(_19, item)) return 19;
        if (count >= 21 && comparer.Equals(_20, item)) return 20;
        if (count >= 22 && comparer.Equals(_21, item)) return 21;
        if (count >= 23 && comparer.Equals(_22, item)) return 22;
        if (count >= 24 && comparer.Equals(_23, item)) return 23;
        if (rest != null) return rest.IndexOf(item)-StackCount;
        return -1;
    }

    /// <summary>Inserts an <paramref name="item"/> to the StructList24`1 at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="item">The object to insert.</param>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList24`1.</exception>
    public void Insert(int index, T item)
    {
        if (index < 0 || index > count) throw new ArgumentOutOfRangeException();
        if (index >= 24) { if (rest == null) rest = new List<T>(); rest.Insert(index - StackCount, item); }
        if (index <= 23 && count >= 24) { if (rest == null) rest = new List<T>(); rest.Insert(0, _23); }
        if (index <= 22 && count >= 23) _23 = _22;
        if (index <= 21 && count >= 22) _22 = _21;
        if (index <= 20 && count >= 21) _21 = _20;
        if (index <= 19 && count >= 20) _20 = _19;
        if (index <= 18 && count >= 19) _19 = _18;
        if (index <= 17 && count >= 18) _18 = _17;
        if (index <= 16 && count >= 17) _17 = _16;
        if (index <= 15 && count >= 16) _16 = _15;
        if (index <= 14 && count >= 15) _15 = _14;
        if (index <= 13 && count >= 14) _14 = _13;
        if (index <= 12 && count >= 13) _13 = _12;
        if (index <= 11 && count >= 12) _12 = _11;
        if (index <= 10 && count >= 11) _11 = _10;
        if (index <= 9 && count >= 10) _10 = _9;
        if (index <= 8 && count >= 9) _9 = _8;
        if (index <= 7 && count >= 8) _8 = _7;
        if (index <= 6 && count >= 7) _7 = _6;
        if (index <= 5 && count >= 6) _6 = _5;
        if (index <= 4 && count >= 5) _5 = _4;
        if (index <= 3 && count >= 4) _4 = _3;
        if (index <= 2 && count >= 3) _3 = _2;
        if (index <= 1 && count >= 2) _2 = _1;
        if (index <= 0 && count >= 1) _1 = _0;

        count++;
        this[index] = item;
    }

    /// <summary>Copies the elements to <paramref name="array"/>, starting at <paramref name="arrayIndex"/>.</summary>
    /// <param name="array">The one-dimensional System.Array.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <exception cref="System.ArgumentNullException">array is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
    /// <exception cref="System.ArgumentException">The number of elements in the source StructList24`1 is greater than the available space from arrayIndex to the end of the destination array.</exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
        if (count > array.Length + arrayIndex) throw new ArgumentException();

        if (count >= 1) array[arrayIndex++] = _0;
        if (count >= 2) array[arrayIndex++] = _1;
        if (count >= 3) array[arrayIndex++] = _2;
        if (count >= 4) array[arrayIndex++] = _3;
        if (count >= 5) array[arrayIndex++] = _4;
        if (count >= 6) array[arrayIndex++] = _5;
        if (count >= 7) array[arrayIndex++] = _6;
        if (count >= 8) array[arrayIndex++] = _7;
        if (count >= 9) array[arrayIndex++] = _8;
        if (count >= 10) array[arrayIndex++] = _9;
        if (count >= 11) array[arrayIndex++] = _10;
        if (count >= 12) array[arrayIndex++] = _11;
        if (count >= 13) array[arrayIndex++] = _12;
        if (count >= 14) array[arrayIndex++] = _13;
        if (count >= 15) array[arrayIndex++] = _14;
        if (count >= 16) array[arrayIndex++] = _15;
        if (count >= 17) array[arrayIndex++] = _16;
        if (count >= 18) array[arrayIndex++] = _17;
        if (count >= 19) array[arrayIndex++] = _18;
        if (count >= 20) array[arrayIndex++] = _19;
        if (count >= 21) array[arrayIndex++] = _20;
        if (count >= 22) array[arrayIndex++] = _21;
        if (count >= 23) array[arrayIndex++] = _22;
        if (count >= 24) array[arrayIndex++] = _23;
        if (rest != null) rest.CopyTo(array, arrayIndex);
    }

    /// <summary>Create array.</summary>
    public T[] ToArray()
    {
        // Return empty singleton
        if (count == 0) return Array.Empty<T>();
        // Create array
        T[] result = new T[count];
        if (count >= 1) result[0] = _0;
        if (count >= 2) result[1] = _1;
        if (count >= 3) result[2] = _2;
        if (count >= 4) result[3] = _3;
        if (count >= 5) result[4] = _4;
        if (count >= 6) result[5] = _5;
        if (count >= 7) result[6] = _6;
        if (count >= 8) result[7] = _7;
        if (count >= 9) result[8] = _8;
        if (count >= 10) result[9] = _9;
        if (count >= 11) result[10] = _10;
        if (count >= 12) result[11] = _11;
        if (count >= 13) result[12] = _12;
        if (count >= 14) result[13] = _13;
        if (count >= 15) result[14] = _14;
        if (count >= 16) result[15] = _15;
        if (count >= 17) result[16] = _16;
        if (count >= 18) result[17] = _17;
        if (count >= 19) result[18] = _18;
        if (count >= 20) result[19] = _19;
        if (count >= 21) result[20] = _20;
        if (count >= 22) result[21] = _21;
        if (count >= 23) result[22] = _22;
        if (count >= 24) result[23] = _23;
        if (count > 24)
        {
            for (int i = 24; i < count; i++)
                result[i] = rest![i-24];
        }
        // Return array
        return result;
    }

    /// <summary>Create array with elements reversed.</summary>
    public T[] ToReverseArray()
    {
        T[] result = new T[count];
        if (count >= 1) result[count-1] = _0;
        if (count >= 2) result[count-2] = _1;
        if (count >= 3) result[count-3] = _2;
        if (count >= 4) result[count-4] = _3;
        if (count >= 5) result[count-5] = _4;
        if (count >= 6) result[count-6] = _5;
        if (count >= 7) result[count-7] = _6;
        if (count >= 8) result[count-8] = _7;
        if (count >= 9) result[count-9] = _8;
        if (count >= 10) result[count-10] = _9;
        if (count >= 11) result[count-11] = _10;
        if (count >= 12) result[count-12] = _11;
        if (count >= 13) result[count-13] = _12;
        if (count >= 14) result[count-14] = _13;
        if (count >= 15) result[count-15] = _14;
        if (count >= 16) result[count-16] = _15;
        if (count >= 17) result[count-17] = _16;
        if (count >= 18) result[count-18] = _17;
        if (count >= 19) result[count-19] = _18;
        if (count >= 20) result[count-20] = _19;
        if (count >= 21) result[count-21] = _20;
        if (count >= 22) result[count-22] = _21;
        if (count >= 23) result[count-23] = _22;
        if (count >= 24) result[count-24] = _23;
        if (count > 24)
        {
            for (int i = 24; i < count; i++)
                result[count-1-i] = rest![i-24];
        }
        return result;
    }

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)ToArray()).GetEnumerator();

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)ToArray()).GetEnumerator();
}


/// <summary>A list where the first 32 element(s) are stack allocated, and rest are allocated from heap when needed.</summary>
/// <typeparam name="T"></typeparam>
public struct StructList32<T> : IList<T>
{
    /// <summary>The number of elements that are stack allocated.</summary>
    public int StackCount => 32;

    /// <summary>Number of elements in the list</summary>
    public int Count => count;
    /// <summary>Is list readonly</summary>
    public bool IsReadOnly => false;

    /// <summary>Number of elements</summary>
    int count;
    /// <summary>First elements</summary>
    T _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14, _15, _16, _17, _18, _19, _20, _21, _22, _23, _24, _25, _26, _27, _28, _29, _30, _31;
    /// <summary>Elements after <see cref="StackCount"/>.</summary>
    List<T>? rest;
    /// <summary>Element comparer</summary>
    IEqualityComparer<T>? elementComparer;
    /// <summary>Element comparer</summary>
    public IEqualityComparer<T> ElementComparer => elementComparer ?? EqualityComparer<T>.Default;

    /// <summary>Create struct list.</summary>
    /// <param name="elementComparer"></param>
    public StructList32(IEqualityComparer<T>? elementComparer = null)
    {
        this.elementComparer = elementComparer ?? EqualityComparer<T>.Default;
        count = 0;

        _0 = default!;
        _1 = default!;
        _2 = default!;
        _3 = default!;
        _4 = default!;
        _5 = default!;
        _6 = default!;
        _7 = default!;
        _8 = default!;
        _9 = default!;
        _10 = default!;
        _11 = default!;
        _12 = default!;
        _13 = default!;
        _14 = default!;
        _15 = default!;
        _16 = default!;
        _17 = default!;
        _18 = default!;
        _19 = default!;
        _20 = default!;
        _21 = default!;
        _22 = default!;
        _23 = default!;
        _24 = default!;
        _25 = default!;
        _26 = default!;
        _27 = default!;
        _28 = default!;
        _29 = default!;
        _30 = default!;
        _31 = default!;
        rest = null;
    }

    /// <summary>Create struct list.</summary>
    /// <param name="enumr"></param>
    public StructList32(IEnumerable<T> enumr) : this(elementComparer: default)
    {
        foreach(T value in enumr) Add(value);
    }

    /// <summary>Gets or sets the element at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>element</returns>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList32`1.</exception>
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: return _0;
                case 1: return _1;
                case 2: return _2;
                case 3: return _3;
                case 4: return _4;
                case 5: return _5;
                case 6: return _6;
                case 7: return _7;
                case 8: return _8;
                case 9: return _9;
                case 10: return _10;
                case 11: return _11;
                case 12: return _12;
                case 13: return _13;
                case 14: return _14;
                case 15: return _15;
                case 16: return _16;
                case 17: return _17;
                case 18: return _18;
                case 19: return _19;
                case 20: return _20;
                case 21: return _21;
                case 22: return _22;
                case 23: return _23;
                case 24: return _24;
                case 25: return _25;
                case 26: return _26;
                case 27: return _27;
                case 28: return _28;
                case 29: return _29;
                case 30: return _30;
                case 31: return _31;
                default: return rest![index - StackCount];
            }
        }
        set
        {
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
            switch(index)
            {
                case 0: _0 = value; return;
                case 1: _1 = value; return;
                case 2: _2 = value; return;
                case 3: _3 = value; return;
                case 4: _4 = value; return;
                case 5: _5 = value; return;
                case 6: _6 = value; return;
                case 7: _7 = value; return;
                case 8: _8 = value; return;
                case 9: _9 = value; return;
                case 10: _10 = value; return;
                case 11: _11 = value; return;
                case 12: _12 = value; return;
                case 13: _13 = value; return;
                case 14: _14 = value; return;
                case 15: _15 = value; return;
                case 16: _16 = value; return;
                case 17: _17 = value; return;
                case 18: _18 = value; return;
                case 19: _19 = value; return;
                case 20: _20 = value; return;
                case 21: _21 = value; return;
                case 22: _22 = value; return;
                case 23: _23 = value; return;
                case 24: _24 = value; return;
                case 25: _25 = value; return;
                case 26: _26 = value; return;
                case 27: _27 = value; return;
                case 28: _28 = value; return;
                case 29: _29 = value; return;
                case 30: _30 = value; return;
                case 31: _31 = value; return;
                default: rest![index - StackCount] = value; return;
            }
        }
    }

    /// <summary>Get reference to element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static ref T GetRef(ref StructList32<T> list, int index)
    {
        if (index < 0 || index >= list.Count) throw new ArgumentOutOfRangeException();
        switch (index)
        {
            case 0: return ref list._0;
            case 1: return ref list._1;
            case 2: return ref list._2;
            case 3: return ref list._3;
            case 4: return ref list._4;
            case 5: return ref list._5;
            case 6: return ref list._6;
            case 7: return ref list._7;
            case 8: return ref list._8;
            case 9: return ref list._9;
            case 10: return ref list._10;
            case 11: return ref list._11;
            case 12: return ref list._12;
            case 13: return ref list._13;
            case 14: return ref list._14;
            case 15: return ref list._15;
            case 16: return ref list._16;
            case 17: return ref list._17;
            case 18: return ref list._18;
            case 19: return ref list._19;
            case 20: return ref list._20;
            case 21: return ref list._21;
            case 22: return ref list._22;
            case 23: return ref list._23;
            case 24: return ref list._24;
            case 25: return ref list._25;
            case 26: return ref list._26;
            case 27: return ref list._27;
            case 28: return ref list._28;
            case 29: return ref list._29;
            case 30: return ref list._30;
            case 31: return ref list._31;
            default:
                Span<T> span = CollectionsMarshal.AsSpan<T>(list.rest);
                return ref span[index - 32];
        }
    }

    /// <summary>Add <paramref name="item"/> to the StructList32`1.</summary>
    /// <exception cref="System.NotSupportedException">The StructList32`1 is read-only.</exception>
    public void Add(T item)
    {
        switch(count)
        {
            case 0: _0 = item; break;
            case 1: _1 = item; break;
            case 2: _2 = item; break;
            case 3: _3 = item; break;
            case 4: _4 = item; break;
            case 5: _5 = item; break;
            case 6: _6 = item; break;
            case 7: _7 = item; break;
            case 8: _8 = item; break;
            case 9: _9 = item; break;
            case 10: _10 = item; break;
            case 11: _11 = item; break;
            case 12: _12 = item; break;
            case 13: _13 = item; break;
            case 14: _14 = item; break;
            case 15: _15 = item; break;
            case 16: _16 = item; break;
            case 17: _17 = item; break;
            case 18: _18 = item; break;
            case 19: _19 = item; break;
            case 20: _20 = item; break;
            case 21: _21 = item; break;
            case 22: _22 = item; break;
            case 23: _23 = item; break;
            case 24: _24 = item; break;
            case 25: _25 = item; break;
            case 26: _26 = item; break;
            case 27: _27 = item; break;
            case 28: _28 = item; break;
            case 29: _29 = item; break;
            case 30: _30 = item; break;
            case 31: _31 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }

        count++;
        return;
    }

    /// <summary>Adds <paramref name="items"/>.</summary>
    /// <exception cref="System.NotSupportedException">The StructList32`1 is read-only.</exception>
    public void AddRange(IEnumerable<T> items)
    {
        foreach(T item in items)
        {
            switch(count)
            {
                case 0: _0 = item; break;
                case 1: _1 = item; break;
                case 2: _2 = item; break;
                case 3: _3 = item; break;
                case 4: _4 = item; break;
                case 5: _5 = item; break;
                case 6: _6 = item; break;
                case 7: _7 = item; break;
                case 8: _8 = item; break;
                case 9: _9 = item; break;
                case 10: _10 = item; break;
                case 11: _11 = item; break;
                case 12: _12 = item; break;
                case 13: _13 = item; break;
                case 14: _14 = item; break;
                case 15: _15 = item; break;
                case 16: _16 = item; break;
                case 17: _17 = item; break;
                case 18: _18 = item; break;
                case 19: _19 = item; break;
                case 20: _20 = item; break;
                case 21: _21 = item; break;
                case 22: _22 = item; break;
                case 23: _23 = item; break;
                case 24: _24 = item; break;
                case 25: _25 = item; break;
                case 26: _26 = item; break;
                case 27: _27 = item; break;
                case 28: _28 = item; break;
                case 29: _29 = item; break;
                case 30: _30 = item; break;
                case 31: _31 = item; break;
                default:
                    if (rest == null) rest = new List<T>();
                    rest.Add(item);
                    break;
            }
            count++;
        }
    }

    /// <summary>Add <paramref name="item"/>, if the item isn't already in the list.</summary>
    /// <exception cref="System.NotSupportedException">The StructList32`1 is read-only.</exception>
    public void AddIfNew(T item)
    {
        if (Contains(item)) return;
        switch(count)
        {
            case 0: _0 = item; break;
            case 1: _1 = item; break;
            case 2: _2 = item; break;
            case 3: _3 = item; break;
            case 4: _4 = item; break;
            case 5: _5 = item; break;
            case 6: _6 = item; break;
            case 7: _7 = item; break;
            case 8: _8 = item; break;
            case 9: _9 = item; break;
            case 10: _10 = item; break;
            case 11: _11 = item; break;
            case 12: _12 = item; break;
            case 13: _13 = item; break;
            case 14: _14 = item; break;
            case 15: _15 = item; break;
            case 16: _16 = item; break;
            case 17: _17 = item; break;
            case 18: _18 = item; break;
            case 19: _19 = item; break;
            case 20: _20 = item; break;
            case 21: _21 = item; break;
            case 22: _22 = item; break;
            case 23: _23 = item; break;
            case 24: _24 = item; break;
            case 25: _25 = item; break;
            case 26: _26 = item; break;
            case 27: _27 = item; break;
            case 28: _28 = item; break;
            case 29: _29 = item; break;
            case 30: _30 = item; break;
            case 31: _31 = item; break;
            default:
                if (rest == null) rest = new List<T>();
                rest.Add(item);
                break;
        }
        count++; 
    }

    /// <summary>Removes the first occurrence of <paramref name="item"/>.</summary>
    /// <returns>true if item was successfully removed from the StructList32`1; otherwise, false. This method also returns false if item is not found in the original StructList32`1.</returns>
    public bool Remove(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) { RemoveAt(0); return true; }
        if (count >= 2 && comparer.Equals(_1, item)) { RemoveAt(1); return true; }
        if (count >= 3 && comparer.Equals(_2, item)) { RemoveAt(2); return true; }
        if (count >= 4 && comparer.Equals(_3, item)) { RemoveAt(3); return true; }
        if (count >= 5 && comparer.Equals(_4, item)) { RemoveAt(4); return true; }
        if (count >= 6 && comparer.Equals(_5, item)) { RemoveAt(5); return true; }
        if (count >= 7 && comparer.Equals(_6, item)) { RemoveAt(6); return true; }
        if (count >= 8 && comparer.Equals(_7, item)) { RemoveAt(7); return true; }
        if (count >= 9 && comparer.Equals(_8, item)) { RemoveAt(8); return true; }
        if (count >= 10 && comparer.Equals(_9, item)) { RemoveAt(9); return true; }
        if (count >= 11 && comparer.Equals(_10, item)) { RemoveAt(10); return true; }
        if (count >= 12 && comparer.Equals(_11, item)) { RemoveAt(11); return true; }
        if (count >= 13 && comparer.Equals(_12, item)) { RemoveAt(12); return true; }
        if (count >= 14 && comparer.Equals(_13, item)) { RemoveAt(13); return true; }
        if (count >= 15 && comparer.Equals(_14, item)) { RemoveAt(14); return true; }
        if (count >= 16 && comparer.Equals(_15, item)) { RemoveAt(15); return true; }
        if (count >= 17 && comparer.Equals(_16, item)) { RemoveAt(16); return true; }
        if (count >= 18 && comparer.Equals(_17, item)) { RemoveAt(17); return true; }
        if (count >= 19 && comparer.Equals(_18, item)) { RemoveAt(18); return true; }
        if (count >= 20 && comparer.Equals(_19, item)) { RemoveAt(19); return true; }
        if (count >= 21 && comparer.Equals(_20, item)) { RemoveAt(20); return true; }
        if (count >= 22 && comparer.Equals(_21, item)) { RemoveAt(21); return true; }
        if (count >= 23 && comparer.Equals(_22, item)) { RemoveAt(22); return true; }
        if (count >= 24 && comparer.Equals(_23, item)) { RemoveAt(23); return true; }
        if (count >= 25 && comparer.Equals(_24, item)) { RemoveAt(24); return true; }
        if (count >= 26 && comparer.Equals(_25, item)) { RemoveAt(25); return true; }
        if (count >= 27 && comparer.Equals(_26, item)) { RemoveAt(26); return true; }
        if (count >= 28 && comparer.Equals(_27, item)) { RemoveAt(27); return true; }
        if (count >= 29 && comparer.Equals(_28, item)) { RemoveAt(28); return true; }
        if (count >= 30 && comparer.Equals(_29, item)) { RemoveAt(29); return true; }
        if (count >= 31 && comparer.Equals(_30, item)) { RemoveAt(30); return true; }
        if (count >= 32 && comparer.Equals(_31, item)) { RemoveAt(31); return true; }

        if (rest == null) return false;
        foreach(T e in rest) if (comparer.Equals(e, item)) { bool removed = rest.Remove(item); if (removed) count--; return removed; }
        return false;
    }

    /// <summary>Removes element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList32`1.</exception>
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= count) throw new ArgumentOutOfRangeException();
        if (index <= 0 && count > 1) _0 = _1;
        if (index <= 1 && count > 2) _1 = _2;
        if (index <= 2 && count > 3) _2 = _3;
        if (index <= 3 && count > 4) _3 = _4;
        if (index <= 4 && count > 5) _4 = _5;
        if (index <= 5 && count > 6) _5 = _6;
        if (index <= 6 && count > 7) _6 = _7;
        if (index <= 7 && count > 8) _7 = _8;
        if (index <= 8 && count > 9) _8 = _9;
        if (index <= 9 && count > 10) _9 = _10;
        if (index <= 10 && count > 11) _10 = _11;
        if (index <= 11 && count > 12) _11 = _12;
        if (index <= 12 && count > 13) _12 = _13;
        if (index <= 13 && count > 14) _13 = _14;
        if (index <= 14 && count > 15) _14 = _15;
        if (index <= 15 && count > 16) _15 = _16;
        if (index <= 16 && count > 17) _16 = _17;
        if (index <= 17 && count > 18) _17 = _18;
        if (index <= 18 && count > 19) _18 = _19;
        if (index <= 19 && count > 20) _19 = _20;
        if (index <= 20 && count > 21) _20 = _21;
        if (index <= 21 && count > 22) _21 = _22;
        if (index <= 22 && count > 23) _22 = _23;
        if (index <= 23 && count > 24) _23 = _24;
        if (index <= 24 && count > 25) _24 = _25;
        if (index <= 25 && count > 26) _25 = _26;
        if (index <= 26 && count > 27) _26 = _27;
        if (index <= 27 && count > 28) _27 = _28;
        if (index <= 28 && count > 29) _28 = _29;
        if (index <= 29 && count > 30) _29 = _30;
        if (index <= 30 && count > 31) _30 = _31;
        if (index <= 31 && count > 32) { _31 = rest![0]; rest!.RemoveAt(0); }
        if (index >= StackCount) rest!.RemoveAt(index - StackCount);
        count--;
    }

    /// <summary>Removes and returns the element at the end of the list.</summary>
    /// <returns>the last element</returns>
    /// <exception cref="InvalidOperationException">If list is empty</exception>
    public T Dequeue()
    {
        if (count == 0) throw new InvalidOperationException();
        int ix = count - 1;
        T result = this[ix];
        RemoveAt(ix);
        return result;
    }

    /// <summary>Remove all elements.</summary>
    /// <exception cref="System.NotSupportedException">The StructList32`1 is read-only.</exception>
    public void Clear()
    {
        if (count >= 1) _0 = default!;
        if (count >= 2) _1 = default!;
        if (count >= 3) _2 = default!;
        if (count >= 4) _3 = default!;
        if (count >= 5) _4 = default!;
        if (count >= 6) _5 = default!;
        if (count >= 7) _6 = default!;
        if (count >= 8) _7 = default!;
        if (count >= 9) _8 = default!;
        if (count >= 10) _9 = default!;
        if (count >= 11) _10 = default!;
        if (count >= 12) _11 = default!;
        if (count >= 13) _12 = default!;
        if (count >= 14) _13 = default!;
        if (count >= 15) _14 = default!;
        if (count >= 16) _15 = default!;
        if (count >= 17) _16 = default!;
        if (count >= 18) _17 = default!;
        if (count >= 19) _18 = default!;
        if (count >= 20) _19 = default!;
        if (count >= 21) _20 = default!;
        if (count >= 22) _21 = default!;
        if (count >= 23) _22 = default!;
        if (count >= 24) _23 = default!;
        if (count >= 25) _24 = default!;
        if (count >= 26) _25 = default!;
        if (count >= 27) _26 = default!;
        if (count >= 28) _27 = default!;
        if (count >= 29) _28 = default!;
        if (count >= 30) _29 = default!;
        if (count >= 31) _30 = default!;
        if (count >= 32) _31 = default!;
        if (rest != null) rest.Clear();
        count = 0;
    }

    /// <summary>Determine whether <paramref name="item"/> is in the list.</summary>
    /// <returns>true if item is found in the StructList32`1; otherwise, false.</returns>
    public bool Contains(T item)
    {
        if (count == 0) return false;
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return true;
        if (count >= 2 && comparer.Equals(_1, item)) return true;
        if (count >= 3 && comparer.Equals(_2, item)) return true;
        if (count >= 4 && comparer.Equals(_3, item)) return true;
        if (count >= 5 && comparer.Equals(_4, item)) return true;
        if (count >= 6 && comparer.Equals(_5, item)) return true;
        if (count >= 7 && comparer.Equals(_6, item)) return true;
        if (count >= 8 && comparer.Equals(_7, item)) return true;
        if (count >= 9 && comparer.Equals(_8, item)) return true;
        if (count >= 10 && comparer.Equals(_9, item)) return true;
        if (count >= 11 && comparer.Equals(_10, item)) return true;
        if (count >= 12 && comparer.Equals(_11, item)) return true;
        if (count >= 13 && comparer.Equals(_12, item)) return true;
        if (count >= 14 && comparer.Equals(_13, item)) return true;
        if (count >= 15 && comparer.Equals(_14, item)) return true;
        if (count >= 16 && comparer.Equals(_15, item)) return true;
        if (count >= 17 && comparer.Equals(_16, item)) return true;
        if (count >= 18 && comparer.Equals(_17, item)) return true;
        if (count >= 19 && comparer.Equals(_18, item)) return true;
        if (count >= 20 && comparer.Equals(_19, item)) return true;
        if (count >= 21 && comparer.Equals(_20, item)) return true;
        if (count >= 22 && comparer.Equals(_21, item)) return true;
        if (count >= 23 && comparer.Equals(_22, item)) return true;
        if (count >= 24 && comparer.Equals(_23, item)) return true;
        if (count >= 25 && comparer.Equals(_24, item)) return true;
        if (count >= 26 && comparer.Equals(_25, item)) return true;
        if (count >= 27 && comparer.Equals(_26, item)) return true;
        if (count >= 28 && comparer.Equals(_27, item)) return true;
        if (count >= 29 && comparer.Equals(_28, item)) return true;
        if (count >= 30 && comparer.Equals(_29, item)) return true;
        if (count >= 31 && comparer.Equals(_30, item)) return true;
        if (count >= 32 && comparer.Equals(_31, item)) return true;
        if (rest != null) return rest.Contains(item);
        return false;
    }

    /// <summary>Determines the index of <paramref name="item"/>.</summary>
    /// <returns>The index of item if found in the list; otherwise, -1.</returns>
    public int IndexOf(T item)
    {
        IEqualityComparer<T> comparer = ElementComparer;
        if (count >= 1 && comparer.Equals(_0, item)) return 0;
        if (count >= 2 && comparer.Equals(_1, item)) return 1;
        if (count >= 3 && comparer.Equals(_2, item)) return 2;
        if (count >= 4 && comparer.Equals(_3, item)) return 3;
        if (count >= 5 && comparer.Equals(_4, item)) return 4;
        if (count >= 6 && comparer.Equals(_5, item)) return 5;
        if (count >= 7 && comparer.Equals(_6, item)) return 6;
        if (count >= 8 && comparer.Equals(_7, item)) return 7;
        if (count >= 9 && comparer.Equals(_8, item)) return 8;
        if (count >= 10 && comparer.Equals(_9, item)) return 9;
        if (count >= 11 && comparer.Equals(_10, item)) return 10;
        if (count >= 12 && comparer.Equals(_11, item)) return 11;
        if (count >= 13 && comparer.Equals(_12, item)) return 12;
        if (count >= 14 && comparer.Equals(_13, item)) return 13;
        if (count >= 15 && comparer.Equals(_14, item)) return 14;
        if (count >= 16 && comparer.Equals(_15, item)) return 15;
        if (count >= 17 && comparer.Equals(_16, item)) return 16;
        if (count >= 18 && comparer.Equals(_17, item)) return 17;
        if (count >= 19 && comparer.Equals(_18, item)) return 18;
        if (count >= 20 && comparer.Equals(_19, item)) return 19;
        if (count >= 21 && comparer.Equals(_20, item)) return 20;
        if (count >= 22 && comparer.Equals(_21, item)) return 21;
        if (count >= 23 && comparer.Equals(_22, item)) return 22;
        if (count >= 24 && comparer.Equals(_23, item)) return 23;
        if (count >= 25 && comparer.Equals(_24, item)) return 24;
        if (count >= 26 && comparer.Equals(_25, item)) return 25;
        if (count >= 27 && comparer.Equals(_26, item)) return 26;
        if (count >= 28 && comparer.Equals(_27, item)) return 27;
        if (count >= 29 && comparer.Equals(_28, item)) return 28;
        if (count >= 30 && comparer.Equals(_29, item)) return 29;
        if (count >= 31 && comparer.Equals(_30, item)) return 30;
        if (count >= 32 && comparer.Equals(_31, item)) return 31;
        if (rest != null) return rest.IndexOf(item)-StackCount;
        return -1;
    }

    /// <summary>Inserts an <paramref name="item"/> to the StructList32`1 at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="item">The object to insert.</param>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the StructList32`1.</exception>
    public void Insert(int index, T item)
    {
        if (index < 0 || index > count) throw new ArgumentOutOfRangeException();
        if (index >= 32) { if (rest == null) rest = new List<T>(); rest.Insert(index - StackCount, item); }
        if (index <= 31 && count >= 32) { if (rest == null) rest = new List<T>(); rest.Insert(0, _31); }
        if (index <= 30 && count >= 31) _31 = _30;
        if (index <= 29 && count >= 30) _30 = _29;
        if (index <= 28 && count >= 29) _29 = _28;
        if (index <= 27 && count >= 28) _28 = _27;
        if (index <= 26 && count >= 27) _27 = _26;
        if (index <= 25 && count >= 26) _26 = _25;
        if (index <= 24 && count >= 25) _25 = _24;
        if (index <= 23 && count >= 24) _24 = _23;
        if (index <= 22 && count >= 23) _23 = _22;
        if (index <= 21 && count >= 22) _22 = _21;
        if (index <= 20 && count >= 21) _21 = _20;
        if (index <= 19 && count >= 20) _20 = _19;
        if (index <= 18 && count >= 19) _19 = _18;
        if (index <= 17 && count >= 18) _18 = _17;
        if (index <= 16 && count >= 17) _17 = _16;
        if (index <= 15 && count >= 16) _16 = _15;
        if (index <= 14 && count >= 15) _15 = _14;
        if (index <= 13 && count >= 14) _14 = _13;
        if (index <= 12 && count >= 13) _13 = _12;
        if (index <= 11 && count >= 12) _12 = _11;
        if (index <= 10 && count >= 11) _11 = _10;
        if (index <= 9 && count >= 10) _10 = _9;
        if (index <= 8 && count >= 9) _9 = _8;
        if (index <= 7 && count >= 8) _8 = _7;
        if (index <= 6 && count >= 7) _7 = _6;
        if (index <= 5 && count >= 6) _6 = _5;
        if (index <= 4 && count >= 5) _5 = _4;
        if (index <= 3 && count >= 4) _4 = _3;
        if (index <= 2 && count >= 3) _3 = _2;
        if (index <= 1 && count >= 2) _2 = _1;
        if (index <= 0 && count >= 1) _1 = _0;

        count++;
        this[index] = item;
    }

    /// <summary>Copies the elements to <paramref name="array"/>, starting at <paramref name="arrayIndex"/>.</summary>
    /// <param name="array">The one-dimensional System.Array.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <exception cref="System.ArgumentNullException">array is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
    /// <exception cref="System.ArgumentException">The number of elements in the source StructList32`1 is greater than the available space from arrayIndex to the end of the destination array.</exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
        if (count > array.Length + arrayIndex) throw new ArgumentException();

        if (count >= 1) array[arrayIndex++] = _0;
        if (count >= 2) array[arrayIndex++] = _1;
        if (count >= 3) array[arrayIndex++] = _2;
        if (count >= 4) array[arrayIndex++] = _3;
        if (count >= 5) array[arrayIndex++] = _4;
        if (count >= 6) array[arrayIndex++] = _5;
        if (count >= 7) array[arrayIndex++] = _6;
        if (count >= 8) array[arrayIndex++] = _7;
        if (count >= 9) array[arrayIndex++] = _8;
        if (count >= 10) array[arrayIndex++] = _9;
        if (count >= 11) array[arrayIndex++] = _10;
        if (count >= 12) array[arrayIndex++] = _11;
        if (count >= 13) array[arrayIndex++] = _12;
        if (count >= 14) array[arrayIndex++] = _13;
        if (count >= 15) array[arrayIndex++] = _14;
        if (count >= 16) array[arrayIndex++] = _15;
        if (count >= 17) array[arrayIndex++] = _16;
        if (count >= 18) array[arrayIndex++] = _17;
        if (count >= 19) array[arrayIndex++] = _18;
        if (count >= 20) array[arrayIndex++] = _19;
        if (count >= 21) array[arrayIndex++] = _20;
        if (count >= 22) array[arrayIndex++] = _21;
        if (count >= 23) array[arrayIndex++] = _22;
        if (count >= 24) array[arrayIndex++] = _23;
        if (count >= 25) array[arrayIndex++] = _24;
        if (count >= 26) array[arrayIndex++] = _25;
        if (count >= 27) array[arrayIndex++] = _26;
        if (count >= 28) array[arrayIndex++] = _27;
        if (count >= 29) array[arrayIndex++] = _28;
        if (count >= 30) array[arrayIndex++] = _29;
        if (count >= 31) array[arrayIndex++] = _30;
        if (count >= 32) array[arrayIndex++] = _31;
        if (rest != null) rest.CopyTo(array, arrayIndex);
    }

    /// <summary>Create array.</summary>
    public T[] ToArray()
    {
        // Return empty singleton
        if (count == 0) return Array.Empty<T>();
        // Create array
        T[] result = new T[count];
        if (count >= 1) result[0] = _0;
        if (count >= 2) result[1] = _1;
        if (count >= 3) result[2] = _2;
        if (count >= 4) result[3] = _3;
        if (count >= 5) result[4] = _4;
        if (count >= 6) result[5] = _5;
        if (count >= 7) result[6] = _6;
        if (count >= 8) result[7] = _7;
        if (count >= 9) result[8] = _8;
        if (count >= 10) result[9] = _9;
        if (count >= 11) result[10] = _10;
        if (count >= 12) result[11] = _11;
        if (count >= 13) result[12] = _12;
        if (count >= 14) result[13] = _13;
        if (count >= 15) result[14] = _14;
        if (count >= 16) result[15] = _15;
        if (count >= 17) result[16] = _16;
        if (count >= 18) result[17] = _17;
        if (count >= 19) result[18] = _18;
        if (count >= 20) result[19] = _19;
        if (count >= 21) result[20] = _20;
        if (count >= 22) result[21] = _21;
        if (count >= 23) result[22] = _22;
        if (count >= 24) result[23] = _23;
        if (count >= 25) result[24] = _24;
        if (count >= 26) result[25] = _25;
        if (count >= 27) result[26] = _26;
        if (count >= 28) result[27] = _27;
        if (count >= 29) result[28] = _28;
        if (count >= 30) result[29] = _29;
        if (count >= 31) result[30] = _30;
        if (count >= 32) result[31] = _31;
        if (count > 32)
        {
            for (int i = 32; i < count; i++)
                result[i] = rest![i-32];
        }
        // Return array
        return result;
    }

    /// <summary>Create array with elements reversed.</summary>
    public T[] ToReverseArray()
    {
        T[] result = new T[count];
        if (count >= 1) result[count-1] = _0;
        if (count >= 2) result[count-2] = _1;
        if (count >= 3) result[count-3] = _2;
        if (count >= 4) result[count-4] = _3;
        if (count >= 5) result[count-5] = _4;
        if (count >= 6) result[count-6] = _5;
        if (count >= 7) result[count-7] = _6;
        if (count >= 8) result[count-8] = _7;
        if (count >= 9) result[count-9] = _8;
        if (count >= 10) result[count-10] = _9;
        if (count >= 11) result[count-11] = _10;
        if (count >= 12) result[count-12] = _11;
        if (count >= 13) result[count-13] = _12;
        if (count >= 14) result[count-14] = _13;
        if (count >= 15) result[count-15] = _14;
        if (count >= 16) result[count-16] = _15;
        if (count >= 17) result[count-17] = _16;
        if (count >= 18) result[count-18] = _17;
        if (count >= 19) result[count-19] = _18;
        if (count >= 20) result[count-20] = _19;
        if (count >= 21) result[count-21] = _20;
        if (count >= 22) result[count-22] = _21;
        if (count >= 23) result[count-23] = _22;
        if (count >= 24) result[count-24] = _23;
        if (count >= 25) result[count-25] = _24;
        if (count >= 26) result[count-26] = _25;
        if (count >= 27) result[count-27] = _26;
        if (count >= 28) result[count-28] = _27;
        if (count >= 29) result[count-29] = _28;
        if (count >= 30) result[count-30] = _29;
        if (count >= 31) result[count-31] = _30;
        if (count >= 32) result[count-32] = _31;
        if (count > 32)
        {
            for (int i = 32; i < count; i++)
                result[count-1-i] = rest![i-32];
        }
        return result;
    }

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)ToArray()).GetEnumerator();

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)ToArray()).GetEnumerator();
}


