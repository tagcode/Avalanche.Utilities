// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>Extension methods for <see cref="IEnumerable{T}"/>.</summary>
public static class EnumerableExtensions
{
    /// <summary>Add <paramref name="elementsToAdd"/> to <paramref name="list"/>.</summary>
    /// <returns><paramref name="list"/></returns>
    public static ICollection<T> AddRange<T>(this ICollection<T> list, IEnumerable<T> elementsToAdd)
    {
        // List<T>
        if (list is List<T> listT) listT.AddRange(elementsToAdd);
        // Add each element
        else
        {
            foreach (T element in elementsToAdd) list.Add(element);
        }
        // Return
        return list;
    }

    /// <summary>Add <paramref name="element"/> to <paramref name="collection"/> if new.</summary>
    /// <returns>true if element was new and was added.</returns>
    public static bool AddIfNew<E>(this ICollection<E> collection, E element, IEqualityComparer<E>? equalityComparer = default)
    {
        // Assert argument
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        // Find element in collection
        bool containsElement = equalityComparer == null ? collection.Contains(element) : collection.Contains(element, equalityComparer);
        // Existed, not added
        if (containsElement) return false;
        // Add
        collection.Add(element);
        // Was added
        return true;
    }

    /// <summary>Append <paramref name="element"/> to <paramref name="sequence"/> if new.</summary>
    /// <remarks>If <paramref name="element"/> or <paramref name="sequence"/> is null then nothing is added.</remarks>
    /// <typeparam name="T">The type of the elements of the input sequence and the element.</typeparam>
    /// <param name="sequence">The sequence to concatenate to.</param>
    /// <param name="element">An element to concatenate to the sequence.</param>
    /// <returns><see cref="System.Collections.Generic.IEnumerable{T}"/> that contains the concatenated elements.</returns>
    public static IEnumerable<T> AppendIfNew<T>(this IEnumerable<T>? sequence, T? element)
    {
        // Empty array.
        if (sequence == null && element == null) return Array.Empty<T>();
        // Single element
        if (sequence == null) return new T[] { element! };
        // No element
        if (element == null) return sequence;
        // Contains, ignore
        if (sequence.Contains(element)) return sequence;
        // Concat
        return Enumerable.Concat(sequence, Enumerable.Repeat(element, 1));
    }

    /// <summary>Append new elements from <paramref name="s2"/> to <paramref name="s1"/>,</summary>
    /// <typeparam name="T">The type of the elements of the input sequence and the element.</typeparam>
    /// <param name="s1">The sequence to concatenate to.</param>
    /// <param name="s2">The sequence to concatenate with.</param>
    /// <returns><see cref="IEnumerable{T}"/> with concatenated elements.</returns>
    public static IEnumerable<T> AppendIfNew<T>(this IEnumerable<T>? s1, IEnumerable<T>? s2)
    {
        // Are empty
        bool empty1 = s1 == null || s1.IsEmpty(), empty2 = s2 == null || s2.IsEmpty();
        // Both empty
        if (empty1 && empty2) return Enumerable.Empty<T>();
        // Return 2
        if (empty1) return s2!;
        // Return 1
        if (empty2) return s1!;
        // New reuslt
        List<T> result = new List<T>(s1!);
        // Used for testing if new
        HashSet<T> set = new HashSet<T>(result);
        // Iterate
        foreach (T e in s2!)
        {
            // Not new
            if (!set.Add(e)) continue;
            // Add to result
            result.Add(e);
        }
        // New result
        return result;
    }

    /// <summary>Concatenate <paramref name="part1"/> and <paramref name="part2"/> and build into array.</summary>
    /// <param name="part1">array or null</param>
    /// <param name="part2">array or null</param>
    /// <returns>new array</returns>
    public static T[] ConcatToArray<T>(this IEnumerable<T>? part1, IEnumerable<T>? part2)
    {
        // Count
        int count1 = part1 == null ? 0 : part1 is ICollection<T> collection1 ? collection1.Count : part1.Count();
        int count2 = part2 == null ? 0 : part2 is ICollection<T> collection2 ? collection2.Count : part2.Count();
        // Total count
        int totalCount = count1 + count2;
        // No elements
        if (totalCount == 0) return Array.Empty<T>();
        // Allocate result
        T[] result = new T[totalCount];
        // Cursor index
        int ix = 0;
        // Append from part1
        if (count1 > 0)
        {
            // Copy from array
            if (part1 is T[] array1)
            {
                Array.Copy(array1, 0, result, ix, count1);
                ix += count1;
            }
            // Copy from enumerable
            else
            {
                int i = 0;
                foreach (T value in part1!)
                {
                    result[ix++] = value;
                    if (++i >= count1) break;
                }
            }
        }
        // Append from part2
        if (count2 > 0)
        {
            // Copy from array
            if (part2 is T[] array2)
            {
                Array.Copy(array2, 0, result, ix, count2);
                ix += count2;
            }
            // Copy from enumerable
            else
            {
                int i = 0;
                foreach (T value in part2!)
                {
                    result[ix++] = value;
                    if (++i >= count2) break;
                }
            }
        }
        // Return
        return result;
    }

    /// <summary>Concatenate <paramref name="part1"/>, <paramref name="part2"/> and <paramref name="part3"/> and build into array.</summary>
    /// <param name="part1">array or null</param>
    /// <param name="part2">array or null</param>
    /// <param name="part3">array or null</param>
    /// <returns>new array</returns>
    public static T[] ConcatToArray<T>(this IEnumerable<T>? part1, IEnumerable<T>? part2, IEnumerable<T>? part3)
    {
        // Count
        int count1 = part1 == null ? 0 : part1 is ICollection<T> collection1 ? collection1.Count : part1.Count();
        int count2 = part2 == null ? 0 : part2 is ICollection<T> collection2 ? collection2.Count : part2.Count();
        int count3 = part3 == null ? 0 : part3 is ICollection<T> collection3 ? collection3.Count : part3.Count();
        // Total count
        int totalCount = count1 + count2 + count3;
        // No elements
        if (totalCount == 0) return Array.Empty<T>();
        // Allocate result
        T[] result = new T[totalCount];
        // Cursor index
        int ix = 0;
        // Append from part1
        if (count1 > 0)
        {
            // Copy from array
            if (part1 is T[] array1)
            {
                Array.Copy(array1, 0, result, ix, count1);
                ix += count1;
            }
            // Copy from enumerable
            else
            {
                int i = 0;
                foreach (T value in part1!)
                {
                    result[ix++] = value;
                    if (++i >= count1) break;
                }
            }
        }
        // Append from part2
        if (count2 > 0)
        {
            // Copy from array
            if (part2 is T[] array2)
            {
                Array.Copy(array2, 0, result, ix, count2);
                ix += count2;
            }
            // Copy from enumerable
            else
            {
                int i = 0;
                foreach (T value in part2!)
                {
                    result[ix++] = value;
                    if (++i >= count2) break;
                }
            }
        }
        // Append from part3
        if (count3 > 0)
        {
            // Copy from array
            if (part3 is T[] array3)
            {
                Array.Copy(array3, 0, result, ix, count3);
                ix += count3;
            }
            // Copy from enumerable
            else
            {
                int i = 0;
                foreach (T value in part3!)
                {
                    result[ix++] = value;
                    if (++i >= count3) break;
                }
            }
        }
        // Return
        return result;
    }

    /// <summary>Concat <paramref name="second"/> to <paramref name="first"/>.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="first">(optional) enumerable</param>
    /// <param name="second">(optional) enumerable</param>
    /// <returns>null, first, second, or concatenation</returns>
    public static IEnumerable<T> ConcatOptional<T>(this IEnumerable<T>? first, IEnumerable<T>? second)
    {
        // Empty
        if (first == null && second == null) return Enumerable.Empty<T>();
        // Second only
        if (first == null) return second!;
        // First only
        if (second == null) return first;
        // Concat
        return first.Concat(second);
    }

    /// <summary>Exclude <paramref name="valueToExclude"/>.</summary>
    public static IEnumerable<T> ExceptValue<T>(this IEnumerable<T> enumr, T valueToExclude, IEqualityComparer<T>? comparer = default)
    {
        // Choose default comparer
        if (comparer == null) comparer = EqualityComparer<T>.Default;
        //
        foreach (T value in enumr)
        {
            // Exclude this
            if (comparer.Equals(value, valueToExclude)) continue;
            // Yield rest
            yield return value;
        }
    }

    /// <summary>Tests if <paramref name="enumr"/> is empty or null</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumr"></param>
    /// <returns></returns>
    public static bool IsEmpty<T>(this IEnumerable<T>? enumr)
    {
        // null
        if (enumr == null) return true;
        // Collection
        if (enumr is ICollection collection) return collection.Count == 0;
        // Try enumerate
        return !enumr.GetEnumerator().MoveNext();
    }

    /// <summary>Test if all elements are true according to <paramref name="selector"/>.</summary>
    /// <typeparam name="T">source element type or null (results false)</typeparam>
    /// <param name="source">source sequence</param>
    /// <param name="selector">selector function</param>
    /// <returns>true if one element is selected.</returns>
    public static bool AllTrue<T>(this IEnumerable<T>? source, Func<T, bool> selector)
    {
        // Null
        if (source == null) return false;
        // Test each
        foreach (T s in source)
            if (!selector(s)) return false;
        // All true
        return true;
    }

    /// <summary>If all elements have same value, returns it, otherwise returns <paramref name="fallback"/>.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumr"></param>
    /// <param name="fallback">Value to default to</param>
    /// <returns>Same value, or to <paramref name="fallback"/>.</returns>
    public static T SameValue<T>(this IEnumerable<T> enumr, T fallback = default(T)!, IEqualityComparer<T>? comparer = default)
    {
        // Choose comparer
        if (comparer == null) comparer = EqualityComparer<T>.Default;
        // Index
        int i = 0;
        // Place value here
        T first = fallback;
        foreach (T element in enumr)
        {
            // 
            i++;
            // Pick up first
            if (i == 1) { first = element; continue; }
            // On eis null, other is not
            if (first == null && element != null) return fallback;
            if (first != null && element == null) return fallback;
            // Both null
            if (!comparer.Equals(first!, element)) return fallback;
        }
        // Return element
        return first;
    }

    /// <summary>Find element using <paramref name="selector"/>.</summary>
    /// <typeparam name="T">source element type</typeparam>
    /// <param name="source">source sequence</param>
    /// <param name="selector">selector function</param>
    /// <returns>element index or -1</returns>
    public static int IndexOf<T>(this IEnumerable<T>? source, Func<T, bool> selector)
    {
        // No source
        if (source == null) return -1;
        // Element index
        int index = 0;
        // Enumerate
        foreach (T s in source)
        {
            // Select
            if (selector(s)) return index;
            // Next
            index++;
        }
        // No result
        return -1;
    }

    /// <summary>Find <paramref name="element"/> from <paramref name="source"/>.</summary>
    /// <typeparam name="T">source element type</typeparam>
    /// <param name="source">source sequence</param>
    /// <param name="element">element</param>
    /// <returns>element index or -1</returns>
    public static int IndexOf<T>(this IEnumerable<T>? source, T element, IEqualityComparer<T>? comparer = default)
    {
        // No source
        if (source == null) return -1;
        // Get comparer
        if (comparer == null) comparer = EqualityComparer<T>.Default;
        // Index
        int index = 0;
        // Enumerate
        foreach (T s in source)
        {
            // Compare
            if (comparer.Equals(element, s)) return index;
            // Next
            index++;
        }
        // No result
        return -1;
    }

    /// <summary>Searches for all index numbers of <paramref name="element"/> with default comparer.</summary>
    /// <typeparam name="T">source element type or null (results -1)</typeparam>
    /// <param name="source">source sequence</param>
    /// <param name="element">elemnt</param>
    /// <returns>element indices</returns>
    public static IEnumerable<int> IndicesOf<T>(this IEnumerable<T>? source, T element, IEqualityComparer<T>? comparer = default)
    {
        // No source
        if (source == null) yield break;
        // Compare
        if (comparer == null) comparer = EqualityComparer<T>.Default;
        // Index
        int index = 0;
        // For each
        foreach (T s in source)
        {
            // Compare
            if (comparer.Equals(element, s)) yield return index;
            // Next
            index++;
        }
    }

    /// <summary>Compare equal contents of <paramref name="a"/> and <paramref name="b"/>.</summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool CompareEquality<T>(this IEnumerable<T>? a, IEnumerable<T>? b, IEqualityComparer<T>? comparer = default)
    {
        // Same reference
        if (a == b) return true;
        // Assert nulls
        if (a == null && b == null) return true;
        if (a != null && b == null) return false;
        if (a == null && b != null) return false;
        // Compare counts
        if (a is IList<T> alist && b is IList<T> blist && alist.Count != blist.Count) return false;
        // Choose comparer
        if (comparer == null) comparer = EqualityComparer<T>.Default;
        // Get enumerators
        using var a_etor = a!.GetEnumerator();
        using var b_etor = b!.GetEnumerator();
        // Place has more here
        bool a_more, b_more;
        // Compare elements
        while ((a_more = a_etor.MoveNext()) & (b_more = b_etor.MoveNext()))
        {
            // Mismatch
            if (!comparer.Equals(a_etor.Current, b_etor.Current)) return false;
        }
        // Excess elements
        if (a_more != b_more) return false;
        // Equal
        return true;
    }

    /// <summary>Compares two streams of <see cref="IComparable{T}"/>.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>-1 if a_str1 is before a_str2, 0 if they are equal, 1 if a_str1 is after a_str2.</returns>
    public static int CompareOrder<T>(this IEnumerable<T>? a, IEnumerable<T>? b, IComparer<T>? comparer = default)
    {
        // Same reference
        if (a == b) return 0;
        // Assert nulls
        if (a == null && b == null) return 0;
        if (a != null && b == null) return 1;
        if (a == null && b != null) return -1;
        // Choose comparer
        if (comparer == null) comparer = Comparer<T>.Default;
        // Get enumerators
        using var a_etor = a!.GetEnumerator();
        using var b_etor = b!.GetEnumerator();
        // Place has more here
        bool a_more, b_more;
        // Compare elements
        while ((a_more = a_etor.MoveNext()) & (b_more = b_etor.MoveNext()))
        {
            // Get diff
            int d = comparer.Compare(a_etor.Current, b_etor.Current);
            // Got diff
            if (d != 0) return d;
        }
        // Return diff
        return a_more ? (b_more ? 0 : 1) : (b_more ? -1 : 0);
    }

    /// <summary>Order by frequence, highest frequency first.</summary>
    public static IEnumerable<T> OrderByFrequency<T>(this IEnumerable<T> enumr, IEqualityComparer<T>? equalityComparer = default) where T : notnull
    {
        // Empty
        if (enumr == null) return Array.Empty<T>();
        // Choose comparer
        if (equalityComparer == null) equalityComparer = EqualityComparer<T>.Default;
        // Frequencies here
        Dictionary<T, int> frequencies = new Dictionary<T, int>(equalityComparer);
        // Result here
        List<T> result = new();
        // Calculate counts
        foreach (T element in enumr)
            if (frequencies.TryGetValue(element, out int count)) { frequencies[element] = count + 1; } else { result.Add(element); frequencies[element] = 1; }
        // Sort list
        result.Sort(new _FrequenceSorter<T>(frequencies));
        // Return
        return result;
    }
    /// <summary>By frequency sorter</summary>
    record _FrequenceSorter<T>(Dictionary<T, int> frequencies) : IComparer<T> where T : notnull
    {
        int IComparer<T>.Compare(T? x, T? y) => frequencies[y!] - frequencies[x!];
    }

    /// <summary>Sum hashcode of elements</summary>
    /// <param name="enumr"></param>
    /// <returns>hashcode</returns>
    public static int HashElements<T>(this IEnumerable<T> enumr)
    {
        // Init
        int result = FNVHashBasis;
        // Enumerate
        foreach (T value in enumr)
        {
            result ^= value == null ? 0 : value.GetHashCode();
            result *= FNVHashPrime;
        }
        // Return
        return result;
    }

    /// <summary></summary>
    public const int FNVHashBasis = unchecked((int)2166136261);
    /// <summary></summary>
    public const int FNVHashPrime = 16777619;
}

/// <summary></summary>
public struct SingleEnumerable<T> : IEnumerable<T>
{
    /// <summary></summary>
    public T value;
    /// <summary></summary>
    public SingleEnumerable(T a_value) { this.value = a_value; }
    /// <summary></summary>
    public SingleEnumerator<T> GetEnumerator() => new SingleEnumerator<T>(value);
    /// <summary></summary>
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => new SingleEnumerator<T>(value);
    /// <summary></summary>
    IEnumerator IEnumerable.GetEnumerator() => new SingleEnumerator<T>(value);
}

/// <summary></summary>
public struct SingleEnumerator<T> : IEnumerator<T>
{
    /// <summary></summary>
    static SingleEnumerator<T> instance = new SingleEnumerator<T>();
    /// <summary></summary>
    public static SingleEnumerator<T> Instance => instance;
    /// <summary></summary>
    public T value;
    /// <summary></summary>
    int index;
    /// <summary></summary>
    public T Current => index == 1 ? value : default!;
    /// <summary></summary>
    object? IEnumerator.Current => index == 1 ? value : default!;
    /// <summary></summary>
    public SingleEnumerator(T a_value) { this.value = a_value; this.index = 0; }
    /// <summary></summary>
    public void Dispose() { }
    /// <summary></summary>
    public bool MoveNext() => ++index == 1;
    /// <summary></summary>
    public void Reset() { index = 0; }
}

/// <summary></summary>
public struct EmptyEnumerable<T> : IEnumerator<T>, IEnumerable<T>
{
    /// <summary></summary>
    static EmptyEnumerable<T> instance = new EmptyEnumerable<T>();
    /// <summary></summary>
    public static EmptyEnumerable<T> Instance => instance;
    /// <summary></summary>
    public T Current => default!;
    /// <summary></summary>
    object? IEnumerator.Current => default;
    /// <summary></summary>
    public void Dispose() { }
    /// <summary></summary>
    public IEnumerator<T> GetEnumerator() => this;
    /// <summary></summary>
    public bool MoveNext() { return false; }
    /// <summary></summary>
    public void Reset() { }
    /// <summary></summary>
    IEnumerator IEnumerable.GetEnumerator() => this;
}

/// <summary></summary>
public struct EnumeratorToEnumerable<ETOR, T> : IEnumerable<T> where ETOR : IEnumerator<T>
{
    /// <summary></summary>
    IEnumerator<T> etor;
    /// <summary></summary>
    public EnumeratorToEnumerable(IEnumerator<T> a_etor) { etor = a_etor; }
    /// <summary></summary>
    public IEnumerator<T> GetEnumerator() => etor;
    /// <summary></summary>
    IEnumerator IEnumerable.GetEnumerator() => etor;
}

/// <summary>Joins key and value enumerators to one KeyValuePair enumerator.</summary>
/// <typeparam name="Key"></typeparam>
/// <typeparam name="Value"></typeparam>
public struct MapEnumerator<Key, Value> : IEnumerator<KeyValuePair<Key, Value>>
{
    IEnumerator<Key> keyEtor;
    IEnumerator<Value> valueEtor;
    KeyValuePair<Key, Value> current;

    /// <summary></summary>
    public KeyValuePair<Key, Value> Current => current;
    /// <summary></summary>
    object IEnumerator.Current => current;

    /// <summary></summary>
    public MapEnumerator(IEnumerator<Key> a_keyetor, IEnumerator<Value> a_valueetor)
    {
        this.current = default(KeyValuePair<Key, Value>);
        this.keyEtor = a_keyetor;
        this.valueEtor = a_valueetor;
    }

    /// <summary></summary>
    public void Dispose()
    {
        keyEtor = null!;
        valueEtor = null!;
    }

    /// <summary></summary>
    public bool MoveNext()
    {
        if (!keyEtor.MoveNext() || !valueEtor.MoveNext())
        {
            current = default(KeyValuePair<Key, Value>);
            return false;
        }
        current = new KeyValuePair<Key, Value>(keyEtor.Current, valueEtor.Current);
        return true;
    }

    /// <summary></summary>
    public void Reset()
    {
        current = default(KeyValuePair<Key, Value>);
        keyEtor?.Reset();
        valueEtor?.Reset();
    }
}


/// <summary>Heap free enumerator, repeats array's enumerator since its hidden.</summary>
public struct ArrayEnumerator<Value> : IEnumerator<Value>, IEnumerator
{
    /// <summary></summary>
    int ix;
    /// <summary></summary>
    Value[] array;

    /// <summary>Create enumerator</summary>
    /// <param name="array">(optional) array instance</param>
    public ArrayEnumerator(Value[] array)
    {
        this.array = array; ix = -1;
    }

    /// <summary></summary>
    public Value Current => ix < 0 || ix > array.Length ? default! : array[ix];

    /// <summary></summary>
    object? IEnumerator.Current => Current;

    /// <summary></summary>
    public void Dispose()
    {
        array = null!;
        ix = -1;
    }

    /// <summary></summary>
    public bool MoveNext() => array == null ? false : ++ix < array.Length;
    /// <summary></summary>
    public void Reset() => ix = -1;
}



