// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

/// <summary>Ring is an array with a moving head and tail.</summary>
public abstract class RingQueue : ICollection, IEnumerable, IReadOnly
{
    /// <summary>Constructor</summary>
    static readonly ConstructorT<int, RingQueue> constructor = new(typeof(RingQueue<>));
    /// <summary>Create ring</summary>
    public static RingQueue Create(Type elementType, int initialCount = 32) => constructor.Create(elementType, 32);

    /// <summary>Is read-only state</summary>
    protected bool @readonly;
    /// <summary>Is read-only state</summary>
    [IgnoreDataMember] bool IReadOnly.ReadOnly { get => @readonly; set { if (@readonly == value) return; if (!value) throw new InvalidOperationException("Read-only"); @readonly = value; } }

    /// <summary>Element type</summary>
    public abstract Type ElementType { get; }

    /// <summary>Allow to grow</summary>
    public bool AllowGrow { get; set; } = true;
    /// <summary>Policy whether element slots should be nulled</summary>
    /// <remarks>If true, removed elements are cleared by over-writing the value 'default(T)' on the array element.</remarks>
    public bool NullSlots { get; set; }

    /// <summary></summary>
    public abstract void CopyTo(Array array, int index);
    /// <summary></summary>
    public abstract int Count { get; }
    /// <summary>Is internally synchronized.</summary>
    public bool IsSynchronized => false;
    /// <summary></summary>
    public object SyncRoot { get; set; } = new object();

    /// <summary></summary>
    protected abstract IEnumerator _getEnumerator();
    /// <summary></summary>
    IEnumerator IEnumerable.GetEnumerator() => _getEnumerator();
}

/// <summary>Ring is an array with a moving head and tail.</summary>
/// <typeparam name="T"></typeparam>
public class RingQueue<T> : RingQueue, ICollection<T>, IEnumerable<T>, ICollection, IEnumerable
{
    /// <summary></summary>
    protected int capacity;
    /// <summary></summary>
    protected int count;
    /// <summary></summary>
    protected int head;
    /// <summary></summary>
    internal T[] buffer;
    /// <summary></summary>
    internal int version;

    /// <summary></summary>
    public override int Count => count;
    /// <summary></summary>
    public bool IsReadOnly => false;
    /// <summary>Element type</summary>
    public override Type ElementType => typeof(T);

    /// <summary>Get and set an element at index [0..count-1]</summary>
    public T this[int @index]
    {
        get
        {
            // Assert index
            if (@index < 0 || @index >= count) throw new IndexOutOfRangeException();
            // Get element
            T element = buffer[(head + @index) % capacity];
            // Return element
            return element;
        }
        set
        {
            // Assert writable
            this.AssertWritable();
            // Assert index
            if (@index < 0 || @index >= count) throw new IndexOutOfRangeException();
            // Assign
            buffer[(head + @index) % capacity] = value;
        }
    }

    /// <summary>Create list</summary>
    public RingQueue(int capacity)
    {
        // Assert
        if (capacity < 0) throw new ArgumentException();
        // Assign
        this.capacity = capacity;
        // Create buffer
        buffer = new T[capacity];
    }

    /// <summary>Create list</summary>
    public RingQueue() : this(32)
    {
        NullSlots = !typeof(T).IsValueType;
    }

    /// <summary>Capacity.</summary>
    /// <remarks>If new assigned capacity is smaller than current entry count, entries are removed starting from the head.</remarks>
    public int Capacity
    {
        get => capacity;
        set
        {
            // New capacity
            int newCapacity = value;
            // Create array
            T[] newArray = new T[newCapacity];

            // Move elements to new array
            if (count > 0)
            {
                // Grow
                if (newCapacity >= count)
                {
                    // All entries fit to new capacity
                    CopyTo(newArray, 0);
                }
                // Shrink
                else
                {
                    // Assert writable
                    this.AssertWritable();
                    // Remove entries from head.
                    CopyTo(count - newCapacity, newArray, 0, newCapacity);
                    count = newCapacity;
                }
            }
            buffer = newArray;
            head = 0;
            version++;
            capacity = newCapacity;
        }
    }

    /// <summary>Ensure capacity is atleast <paramref name="capacity"/>.</summary>
    public int EnsureCapacity(int capacity)
    {
        if (Capacity < capacity) Capacity = capacity;
        return Capacity;
    }
    /// <summary>Sets the capacity to the actual number of elements</summary>
    public RingQueue<T> TrimExcess()
    {
        // Set new capacity
        Capacity = Count;
        // Return 
        return this;
    }

    /// <summary>Test if contains <paramref name="item"/></summary>
    public bool Contains(T item) => IndexOf(item, EqualityComparer<T>.Default) >= 0;

    /// <summary>Test if contains <paramref name="item"/> using <paramref name="comparer"/>.</summary>
    public bool Contains(T item, EqualityComparer<T> comparer) => IndexOf(item, comparer) >= 0;

    /// <summary>Find index of <paramref name="item"/> in the collection.</summary>
    /// <returns>index of item or -1</returns>
    public int IndexOf(T item) => IndexOf(item, EqualityComparer<T>.Default);

    /// <summary>Find index of <paramref name="item"/> in collection using <paramref name="comparer"/>.</summary>
    /// <returns>index of item or -1</returns>
    public int IndexOf(T item, EqualityComparer<T> comparer)
    {
        // iterate each
        for (int i = 0, ix = head; i < count; i++)
        {
            // Get element
            T element = buffer[ix];
            // Searched null
            if (item == null && element == null) return i;
            // Compare
            if (element != null && comparer.Equals(element, item)) return i;
            // Next
            ix = (ix + 1) % capacity;
        }
        // Not found
        return -1;
    }

    /// <summary>Find index of <paramref name="item"/> using <paramref name="comparer"/></summary>
    /// <returns>index of item or -1</returns>
    public int IndexOf(object item, IEqualityComparer comparer)
    {
        // iterate each
        for (int i = 0, ix = head; i < count; i++)
        {
            // Get element
            T element = buffer[ix];
            // Searched null
            if (item == null && element == null) return i;
            // Compare
            if (element != null && comparer.Equals(element, item)) return i;
            // Next
            ix = (ix + 1) % capacity;
        }
        // Not found
        return -1;
    }

    /// <summary>Remove element at <paramref name="index"/>.</summary>
    public void RemoveAt(int @index)
    {
        // Assert writable
        this.AssertWritable();
        // Assert index
        if (@index < 0 || @index >= count) throw new IndexOutOfRangeException();
        // Null deleted
        if (NullSlots) buffer[(head + @index) % capacity] = default!;
        // Skip one
        if (@index == 0) head = (head + 1) % capacity;
        // 
        else { for (int i = @index; i < count - 1; i++) this[i] = this[i + 1]; }
        // Modified
        version++;
        count--;
    }

    /// <summary>Clear all items. Nulls all references.</summary>
    public void Clear()
    {
        // Assert writable
        this.AssertWritable();
        // Null slot
        if (NullSlots) for (int i = 0; i < capacity; i++) buffer[i] = default!;
        // Decrement count
        count = head = 0;
        // New version
        version = version * 13 + 0xf012213;
    }

    /// <summary>Put <paramref name="src"/> into the collection.</summary>
    /// <returns>New count</returns>
    public int Enqueue(T[] src) => Enqueue(src, 0, src.Length);

    /// <summary>Put range of <paramref name="src"/> into collection.</summary>
    /// <returns>New count</returns>
    public int Enqueue(T[] src, int srcOffset, int count)
    {
        // Assert writable
        this.AssertWritable();
        //
        if (count > capacity - this.count)
        {
            if (AllowGrow) Capacity = Math.Max(Capacity * 2, count + this.count);
            else count = capacity - this.count;
        }

        //
        int tail = (head + this.count) % capacity;
        //
        int rightsideRemaining = capacity - tail;
        //
        if (count <= rightsideRemaining)
        {
            Array.Copy(src, srcOffset, buffer, tail, count);
        }
        else
        {
            Array.Copy(src, srcOffset, buffer, tail, rightsideRemaining);
            Array.Copy(src, srcOffset + rightsideRemaining, buffer, 0, count - rightsideRemaining);
        }
        // Change count
        this.count += count;
        // New version
        version++;
        // Return count
        return count;
    }

    /// <summary>Put <paramref name="src"/> into <see cref="IEnumerable{T}"/>.</summary>
    /// <returns>New count</returns>
    public int Enqueue(IEnumerable<T> src)
    {
        // Copy
        List<T> copy = src.ToList();
        // Add
        return Enqueue(copy.GetEnumerator(), copy.Count);
    }

    /// <summary>Put a segment of <paramref name="src"/>.</summary>
    /// <returns>New count</returns>
    public int Enqueue(IEnumerator<T> src, int count)
    {
        // Assert writable
        this.AssertWritable();
        //
        if (count > capacity - this.count)
        {
            if (AllowGrow) Capacity = Math.Max(Capacity * 2, count + this.count);
            else count = capacity - this.count;
        }

        //
        int tail = (head + this.count) % capacity;
        //
        for (int i = 0; i < count; i++)
        {
            if (!src.MoveNext()) break;
            buffer[tail] = src.Current;
            if (++tail >= capacity) tail = 0;
        }
        // New count
        this.count += count;
        // Modified
        version++;
        // Return count
        return count;
    }

    /// <summary>Put an <paramref name="item"/> into the buffer.</summary>
    /// <exception cref="InvalidOperationException">If capacity exeeded.</exception>
    public RingQueue<T> Enqueue(T item)
    {
        // Assert writable
        this.AssertWritable();
        // Assert
        if (count >= capacity)
        {
            if (AllowGrow) Capacity = Math.Max(Capacity * 2, count + 10);
            else throw new InvalidOperationException("Buffer is full.");
        }
        // Tail index
        int tail = (head + count) % capacity;
        // Assign
        buffer[tail] = item;
        // New count
        count++;
        // New version
        version++;
        // Return
        return this;
    }

    /// <summary>Take and skip <paramref name="count"/> from the head.</summary>
    public RingQueue<T> Skip(int count)
    {
        // Assert writable
        this.AssertWritable();
        // Nothing to do
        if (count <= 0) return this;
        // Assert
        if (count > this.count) throw new IndexOutOfRangeException();
        // New version
        version++;
        // Assign nulls
        if (NullSlots) for (int i = 0; i < count; i++) buffer[(head + i) % capacity] = default!;
        // Move
        head = (head + count) % capacity;
        // New count
        this.count -= count;
        //
        return this;
    }

    /// <summary>Get <paramref name="count"/> elements from head.</summary>
    public T[] Dequeue(int count)
    {
        // New array
        var dst = new T[count];
        // Get
        Dequeue(dst);
        // Return
        return dst;
    }

    /// <summary>Read into <paramref name="dst"/> the number of elements in <paramref name="dst"/>.</summary>
    /// <returns>new count</returns>
    public int Dequeue(T[] dst) => Dequeue(dst, 0, dst.Length);

    /// <summary>Read into <paramref name="dst"/>.</summary>
    /// <returns>new count</returns>
    public int Dequeue(T[] dst, int offset, int count)
    {
        // Assert writable
        this.AssertWritable();
        // 
        if (count > this.count) count = this.count;
        // 
        int rightsideRemaining = capacity - head;
        // 
        if (count <= rightsideRemaining)
        {
            Array.Copy(buffer, head, dst, offset, count);
            head += count;
        }
        else
        {
            Array.Copy(buffer, head, dst, offset, rightsideRemaining);
            Array.Copy(buffer, 0, dst, offset + rightsideRemaining, count - rightsideRemaining);
            head = count - rightsideRemaining;
        }
        // Modify count
        this.count -= count;
        // Return count
        return count;
    }

    /// <summary>Get item at head.</summary>
    public T Dequeue()
    {
        // Assert writable
        this.AssertWritable();
        // Assert
        if (count == 0) throw new InvalidOperationException("Buffer is empty.");
        // 
        var item = buffer[head];
        // 
        if (NullSlots) buffer[head] = default!;
        // 
        head = (head + 1) % capacity;
        // New count
        count--;
        // Return item
        return item;
    }

    /// <summary>Return value at head.</summary>
    /// <exception cref="InvalidOperationException"></exception>
    public T Peek() => Count == 0 ? throw new InvalidOperationException("Empty") : this[0];

    /// <summary>Get a reference to the internal array, if possible, if not, get a copy of the internal array.</summary>
    /// <param name="offset">logical offset to the array</param>
    /// <param name="count">number of entries requested</param>
    /// <param name="array">result to be written</param>
    /// <param name="arrOffset">offset in the result</param>
    /// <returns>true, a reference to internal array, false a copy of it</returns>
    public bool GetInternalArray(int offset, int count, ref T[] array, ref int arrOffset)
    {
        // Assert
        if (offset + count > Count) throw new IndexOutOfRangeException();

        // Make a copy
        if (offset + count + head > capacity)
        {
            array = (T[])Array.CreateInstance(typeof(T), count);
            Dequeue(array, offset, count);
            arrOffset = 0;
            return false;
        }
        else
        // Get a reference
        {
            array = buffer;
            arrOffset = head + offset;
            return true;
        }
    }

    /// <summary>Search for a <paramref name="valueToSearch"/> using associated comparer.</summary>
    /// <remarks>The content must be in ascending order.</remarks>
    /// <param name="valueToSearch">value to search</param>
    /// <param name="comparer">comparer</param>
    /// <returns>zero or positive: an exact match, negative: -insert index-1 </returns>
    public int BinarySearch(T valueToSearch, IComparer<T> comparer)
    {
        // Init
        int start = 0, end = Count - 1;
        // For each
        while (start <= end)
        {
            // Index
            int index = start + ((end - start) >> 1);
            // Get
            T value = (T)buffer[(head + index) % capacity];
            // Compare
            int comparison = comparer.Compare(value, valueToSearch);
            // Equal, return exact match
            if (comparison == 0) return index;
            // Move
            if (comparison < 0) start = index + 1; else end = index - 1;
        }
        // Index
        return ~start;
    }

    /// <summary>Search for <paramref name="key"/> with function <paramref name="selector"/> and <paramref name="comparer"/>.</summary>
    /// <remarks>The content must be in ascending order.</remarks>
    /// <typeparam name="Key">Key type, such as DateTime</typeparam>
    /// <param name="key">key value, e.g. UTC.now()</param>
    /// <param name="selector">a function that extracts key from T</param>
    /// <param name="comparer">comparer of keys</param>
    /// <returns>zero or positive: an exact match, negative: -insert index-1 </returns>
    public int BinarySearch<Key>(Key key, Func<T, Key> selector, IComparer<Key> comparer)
    {
        // Init
        int start = 0, end = Count - 1;
        // For each
        while (start <= end)
        {
            // Index
            int index = start + ((end - start) >> 1);
            // Get
            T entry = (T)buffer[(head + index) % capacity];
            // Compare
            int comparison = comparer.Compare(selector(entry), key);
            // Equal, return exact match
            if (comparison == 0) return index;
            // Move
            if (comparison < 0) start = index + 1; else end = index - 1;
        }
        // Index
        return ~start;
    }


    /// <summary>Search for <paramref name="valueToSearch"/> using <paramref name="comparer"/>.</summary>
    /// <remarks>The content must be in ascending order.</remarks>
    /// <param name="valueToSearch">value to search</param>
    /// <param name="comparer">comparer</param>
    /// <returns>zero or positive: an exact match, negative: -insert index-1 </returns>
    public int BinarySearch(object valueToSearch, IComparer comparer)
    {
        // Init
        int start = 0, end = Count - 1;
        // For each
        while (start <= end)
        {
            // Inidex
            int index = start + ((end - start) >> 1);
            // Get
            object? value = buffer[(head + index) % capacity];
            // Compare
            int comparison = comparer.Compare(value, valueToSearch);
            // Equal, return exact match
            if (comparison == 0) return index;
            // Move
            if (comparison < 0) start = index + 1; else end = index - 1;
        }
        // Index
        return ~start;
    }

    /// <summary>Copy elements to <paramref name="dst"/></summary>
    /// <param name="ix">start index on this</param>
    /// <param name="dst">destination array</param>
    /// <param name="offset">offset on destination</param>
    /// <param name="count">number of elements to copy</param>
    public void CopyTo(int ix, T[] dst, int offset, int count)
    {
        // Assert
        if (count > this.count) throw new ArgumentOutOfRangeException("Too long", nameof(count));
        // Index
        int rightCount = capacity - ix - head;
        // 
        if (count <= rightCount)
        {
            Array.Copy(buffer, head + ix, dst, offset, count);
        }
        else
        {
            Array.Copy(buffer, head + ix, dst, offset, rightCount);
            Array.Copy(buffer, 0, dst, offset + rightCount, count - rightCount);
        }
    }

    /// <summary>Copy elements to <paramref name="array"/> at <paramref name="arrayIndex"/>.</summary>
    public override void CopyTo(Array array, int arrayIndex) => CopyTo(0, (T[])array, arrayIndex, count);

    /// <summary>Copy elements to <paramref name="array"/> at <paramref name="arrayIndex"/>.</summary>
    public void CopyTo(T[] array, int arrayIndex) => CopyTo(0, array, arrayIndex, count);

    /// <summary></summary>
    public T[] ToArray()
    {
        var dst = new T[count];
        CopyTo(dst, 0);
        return dst;
    }

    /// <summary>Add <paramref name="item"/>.</summary>
    void ICollection<T>.Add(T item)
    {
        Enqueue(item);
    }

    /// <summary>Remove <paramref name="item"/>.</summary>
    /// <returns>If was returned</returns>
    bool ICollection<T>.Remove(T item)
    {
        // No elements
        if (count == 0) return false;
        // Index
        int ix = IndexOf(item);
        // Not found
        if (ix < 0) return false;
        // Remove
        RemoveAt(ix);
        // Removed
        return true;
    }

    /// <summary>Try dequeue value at head.</summary>
    public bool TryDequeue([MaybeNullWhen(false)] out T result)
    {
        // No value
        if (Count == 0) { result = default!; return false; }
        // Return value
        result = Dequeue();
        return true;
    }

    /// <summary>Try peek value at head</summary>
    public bool TryPeek([MaybeNullWhen(false)] out T result)
    {
        // No value
        if (Count == 0) { result = default!; return false; }
        // Return value
        result = this[0];
        return true;
    }


    /// <summary>Enumerator</summary>
    public Enumerator GetEnumerator() => new Enumerator(head, this);
    /// <summary>Enumerator</summary>
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(head, this);
    /// <summary>Enumerator</summary>
    protected override IEnumerator _getEnumerator() => new Enumerator(head, this);

    /// <summary>Enumerator</summary>
    public struct Enumerator : IEnumerator<T>
    {
        /// <summary></summary>
        RingQueue<T> parent;
        /// <summary></summary>
        int revision;
        /// <summary></summary>
        int ix, count;
        /// <summary></summary>
        int startIx;

        /// <summary>Create enumerator</summary>
        public Enumerator(int @index, RingQueue<T> parent)
        {
            this.parent = parent;
            revision = parent.version;
            startIx = @index;
            count = parent.Count;
            ix = -1;
        }

        /// <summary>Dispose</summary>
        public void Dispose()
        {
            parent = null!;
        }

        /// <summary>Value at cursor</summary>
        public T Current
        {
            get
            {
                // Assert
                if (parent == null) throw new InvalidOperationException("Object disposed");
                if (parent.version != revision) throw new InvalidOperationException("RingBuffer concurrent modification.");
                // Get 
                T result = ix < 0 || ix >= count ? default(T)! : parent[ix];
                // Return
                return result!;
            }
        }

        /// <summary>Value at cursor</summary>
        object IEnumerator.Current
        {
            get
            {
                // Assert
                if (parent == null) throw new InvalidOperationException("Object disposed");
                if (parent.version != revision) throw new InvalidOperationException("RingBuffer concurrent modification.");
                // Get
                T result = ix < 0 || ix >= count ? default(T)! : parent.buffer[ix];
                // Return
                return result!;
            }
        }

        /// <summary>Advances to the next element.</summary>
        /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
        /// <exception cref="System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
        public bool MoveNext()
        {
            // Assert
            if (parent == null) throw new InvalidOperationException("Object disposed");
            if (parent.version != revision) throw new InvalidOperationException("RingBuffer concurrent modification.");
            // Move
            return ++ix < count;
        }

        /// <summary>Sets the enumerator to its initial position, which is before the first element in the collection.</summary>
        /// <exception cref="ObjectDisposedException">Object has been disposed.</exception>
        /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
        public void Reset()
        {
            // Assert
            if (parent == null) throw new ObjectDisposedException(nameof(RingQueue<T>));
            if (parent.version != revision) throw new InvalidOperationException("RingBuffer concurrent modification.");
            // Move
            ix = -1;
        }
    }

}

