// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Avalanche.Utilities.Provider;

/// <summary>Array utils.</summary>
public static class ArrayUtilities
{
    /// <summary>Get snapshot of <paramref name="list"/>.</summary>
    /// <param name="list">List that may be modifiable</param>
    /// <returns>Thread-safe non-null snapshot of <paramref name="list"/>.</returns>    
    /// <remarks>Best used with <see cref="Avalanche.Utilities.ArrayList{T}"/>.</remarks>
    /// <exception cref="InvalidOperationException">On concurrent modification (after 10 retries).</exception>
    public static IList<T> GetSnapshot<T>(IEnumerable<T> list, int retryCount = 10)
    {
        // Null
        if (list == null) return Array.Empty<T>();
        // ArrayList
        if (list is ISnapshotProvider<T> snapshotProvider) return snapshotProvider.Snapshot;
        // T[]
        if (list is T[] array) return array;
        // ReadOnly IList<T>
        if (list is IList<T> _list && _list is IReadOnly @readonly && @readonly.ReadOnly) return _list;
        // Initial element count
        int initCount = 10;
        // ICollection
        if (list is ICollection collection)
        {
            // Get snapshot under lock
            if (TryGetSyncRoot(collection, out object? syncRoot)) lock (syncRoot) return list.ToList();
            // Set initial count
            initCount = collection.Count;
        }
        // Init result
        List<T> result = new List<T>(initCount);
        // Iterations
        for (int i=1; i<=retryCount; i++)
        {
            //
            result.Clear();
            // 
            try
            {
                result.AddRange(list);
                return result;
            }
            // Catch concurrent modification of enumerable and retry a few times
            catch (InvalidOperationException) when (i<retryCount) {
            }
        }
        // 
        throw new InvalidOperationException();
    }

    /// <summary>Get snapshot of <paramref name="list"/>.</summary>
    /// <param name="list">List that may be modifiable</param>
    /// <returns>Thread-safe non-null snapshot of <paramref name="list"/>.</returns>    
    /// <remarks>Best used with <see cref="Avalanche.Utilities.ArrayList{T}"/>.</remarks>
    /// <exception cref="InvalidOperationException">On concurrent modification (after 10 retries).</exception>
    public static T[] GetSnapshotArray<T>(IEnumerable<T> list, int retryCount = 10)
    {
        // Null
        if (list == null) return Array.Empty<T>();
        // ArrayList
        if (list is ISnapshotProvider<T> snapshotProvider) return snapshotProvider.Snapshot;
        // T[]
        if (list is T[] array) return array;
        // Initial element count
        int initCount = 10;
        // ICollection
        if (list is ICollection collection)
        {
            // Get snapshot under lock
            if (TryGetSyncRoot(collection, out object? syncRoot)) lock (syncRoot) return list.ToArray();
            // Set initial count
            initCount = collection.Count;
        }
        // Init result
        List<T> result = new List<T>(initCount);
        // Iterations
        for (int i=1; i<=retryCount; i++)
        {
            //
            result.Clear();
            // 
            try
            {
                result.AddRange(list);
                return result.ToArray();
            }
            // Catch concurrent modification of enumerable and retry a few times
            catch (InvalidOperationException) when (i<retryCount) {
            }
        }
        // 
        throw new InvalidOperationException();
    }

    /// <summary>Try get sync root.</summary>
    public static bool TryGetSyncRoot(this ICollection collection, [NotNullWhen(true)] out object? syncRoot)
    {
        // null
        if (collection == null) { syncRoot = null!; return false; }
        // ConcurrentDictionary.SyncRoot throws exception
        if (TypeUtilities.IsDefinableTo(collection.GetType(), typeof(ConcurrentDictionary<,>))) { syncRoot = null!; return false; }
        // 
        try
        {
            syncRoot = collection.SyncRoot;
            return true;
        } catch(Exception)
        {
            syncRoot = null!;
            return false;
        }
    }

    /// <summary>Add <paramref name="element"/> to <paramref name="array"/>.</summary>
    public static void Add<T>(ref T[]? array, T element)
    {
        // Create new array
        if (array == null) { array = new T[] { element }; return; }
        // Allocate new
        T[] newArray = new T[array.Length + 1];
        // Copy
        Array.Copy(array, newArray, array.Length);
        // Add
        newArray[array.Length] = element;
        // Assign
        array = newArray;
    }

    /// <summary>Create a clone of <paramref name="array"/> with <paramref name="element"/> added.</summary>
    public static T[] Append<T>(this T[]? array, T element)
    {
        // Get array count
        int c = (array == null ? 0 : array.Length);
        // Allocate new
        T[] newArray = new T[c + 1];
        // Copy
        if (array != null) Array.Copy(array, newArray, array.Length);
        // Add
        newArray[c] = element;
        // return
        return newArray;
    }

    /// <summary>Create a clone of <paramref name="array"/> with  <paramref name="element"/> added.</summary>
    public static T[] AppendIfNotNull<T>(this T[]? array, T? element)
    {
        // Got null
        if (element == null) return array ?? EmptyArray<T>();
        // Get array count
        int c = (array == null ? 0 : array.Length);
        // Allocate new
        T[] newArray = new T[c + 1];
        // Copy
        if (array != null) Array.Copy(array, newArray, array.Length);
        // Add
        newArray[c] = element;
        // return
        return newArray;
    }

    /// <summary>Remove <paramref name="element"/> from <paramref name="array"/>. Nulls <paramref name="array"/> if becomes empty.</summary>
    /// <param name="assignNullIfEmptied">If true and <paramref name="array"/> becomes empty, then assigns null value</param>
    public static void Remove<T>(ref T[]? array, T element, IEqualityComparer<T>? comparer = default, bool assignNullIfEmptied = false)
    {
        // No array
        if (array == null) return;
        // Choose comparer
        if (comparer == null) comparer = EqualityComparer<T>.Default;
        // Index
        int ix = -1;
        // Find by index
        for (int i = 0; i < array.Length; i++)
        {
            // Get element
            T e = array[i];
            // Compare
            if (e != null && comparer.Equals(e, element)) { ix = i; break; }
        }
        // Not found
        if (ix < 0) return;
        // Null
        if (assignNullIfEmptied && array.Length == 1 && ix == 0) { array = null; return; }
        // Allocate new
        T[] newArray = new T[array.Length - 1];
        // Copy
        if (ix > 0) Array.Copy(array, 0, newArray, 0, ix);
        if (ix < array.Length - 1) Array.Copy(array, ix + 1, newArray, ix, array.Length - ix - 1);
        // Assign
        array = newArray;
    }

    /// <summary>Create array without element</summary>
    /// <param name="assignNullIfEmptied">If true and <paramref name="array"/> becomes empty, then returns null value</param>
    public static T[] Without<T>(this T[]? array, T element, IEqualityComparer<T>? comparer = default, bool assignNullIfEmptied = false)
    {
        // No array
        if (array == null) return EmptyArray<T>();
        // Choose comparer
        if (comparer == null) comparer = EqualityComparer<T>.Default;
        // Index
        int ix = -1;
        // Find by index
        for (int i = 0; i < array.Length; i++)
        {
            // Get element
            T e = array[i];
            // Compare
            if (e != null && comparer.Equals(e, element)) { ix = i; break; }
        }
        // Not found
        if (ix < 0) return array;
        // Emptyr array
        if (array.Length == 1 && ix == 0) return assignNullIfEmptied ? null! : EmptyArray<T>();
        // Allocate new
        T[] newArray = new T[array.Length - 1];
        // Copy
        if (ix > 0) Array.Copy(array, 0, newArray, 0, ix);
        if (ix < array.Length - 1) Array.Copy(array, ix + 1, newArray, ix, array.Length - ix - 1);
        // Return
        return newArray;
    }

    /// <summary>Remove element at <paramref name="ix"/></summary>
    public static void RemoveAt<T>(ref T[]? array, int ix)
    {
        // Create new array
        if (array == null) return;
        // Not found
        if (ix < 0) return;
        // Null
        if (array.Length == 1 && ix == 0) { array = null; return; }
        // Allocate new
        T[] newArray = new T[array.Length - 1];
        // Copy
        if (ix > 0) Array.Copy(array, 0, newArray, 0, ix);
        if (ix < array.Length - 1) Array.Copy(array, ix + 1, newArray, ix, array.Length - ix - 1);
        // Assign
        array = newArray;
    }

    /// <summary>Create array without element at <paramref name="index"/></summary>
    /// <param name="index">element index, of less than 0 to do nothing.</param>
    /// <returns>An array without element</returns>
    public static T[] WithoutAt<T>(this T[] array, int index)
    {
        // Create new array
        if (array == null) return array!;
        // Not found
        if (index < 0) return array;
        // Emptyr array
        if (array.Length == 1 && index == 0) return EmptyArray<T>();
        // Allocate new
        T[] newArray = new T[array.Length - 1];
        // Copy
        if (index > 0) Array.Copy(array, 0, newArray, 0, index);
        if (index < array.Length - 1) Array.Copy(array, index + 1, newArray, index, array.Length - index - 1);
        // Return
        return newArray;
    }

    /// <summary>Empty array provider</summary>
    static readonly IProvider<Type, Array> emptyArrayProvider = Providers.Func((Type elementType) => Array.CreateInstance(elementType, 0)).WeakCached();
    /// <summary>Return static empty array for <paramref name="elementType"/>.</summary>
    public static Array EmptyArray(Type elementType) => emptyArrayProvider[elementType];
    /// <summary>Return static empty array for <typeparamref name="T"/>.</summary>
    public static T[] EmptyArray<T>() => Array.Empty<T>();

    /// <summary>Slice a sub array.</summary>
    /// <param name="array"></param>
    /// <param name="ix"></param>
    /// <param name="len"></param>
    /// <returns>New array or static for empty</returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    /// <exception cref="System.IndexOutOfRangeException"></exception>
    public static T[] Slice<T>(this T[]? array, int ix, int len)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (len == 0) return EmptyArray<T>();
        if (ix + len > array.Length) throw new IndexOutOfRangeException();
        T[] dst = new T[len];
        Array.Copy(array, ix, dst, 0, len);
        return dst;
    }

    /// <summary>Searches for a <paramref name="sequence"/> in <paramref name="list1"/>. Returns index of first element of <paramref name="sequence"/> in array or -1.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list1"></param>
    /// <param name="sequence"></param>
    /// <param name="comparer">(optional) comparer</param>
    /// <returns>index or -1</returns>
    public static int IndexOfSequence<T>(this IList<T> list1, IList<T> sequence, IEqualityComparer<T>? comparer = null)
    {
        // Assert not null
        if (list1 == null || sequence == null) return -1;
        // Length difference
        int c = list1.Count - sequence.Count, c2 = sequence.Count;
        // Subarray is longer
        if (c < 0) return -1;
        // Assign default comparer
        if (comparer == null) comparer = EqualityComparer<T>.Default;
        // Iterate
        for (int i = 0; i <= c; i++)
        {
            // Place here if every element matched
            bool match = true;
            // Compare subarray
            for (int j = 0; j < c2; j++)
            {
                match = comparer.Equals(list1[i + j], sequence[j]);
                if (!match) break;
            }
            // Every element matched
            if (match) return i;
        }
        return -1;
    }

    /// <summary>Determine whether <paramref name="part1"/> starts with elements of <paramref name="part2"/>.</summary>
    /// <param name="part1">The byte[] to compare.</param>
    /// <param name="part2"></param>
    /// <exception cref="System.ArgumentNullException">value is null.</exception>
    /// <returns>true if value matches the beginning of this byte[]; otherwise, false.</returns>
    public static bool StartsWith<T>(this IList<T>? part1, IList<T>? part2, IEqualityComparer<T>? comparer = default)
    {
        if (part1 == null) throw new ArgumentNullException(nameof(part1));
        if (part2 == null) throw new ArgumentNullException(nameof(part2));
        if (comparer == null) comparer = EqualityComparer<T>.Default;
        if (part2.Count > part1.Count) return false;
        for (int i = 0; i < part2.Count; i++)
            if (!comparer.Equals(part1[i], part2[i])) return false;
        return true;
    }

    /// <summary>Select each element of multi-dimension array.</summary>
    /// <param name="array">array or matrix (any number of dimensions)</param>
    /// <param name="selector">selector function to apply on each element</param>
    /// <returns>A new array of type <![CDATA[TResult[,]]]> with same dimensions</returns>
    public static Array ArraySelect<TSource, TResult>(this Array array, Func<TSource, TResult> selector)
    {
        int rank = array.Rank;
        if (rank == 0) return EmptyArray<TResult>();
        int[] lens = new int[rank];
        int[] indices = new int[rank];
        for (int i = 0; i < rank; i++) lens[i] = array.GetLength(i);

        Array result = Array.CreateInstance(typeof(TResult), lens);

        while (true)
        {
            int r = 0;
            while (indices[r] >= lens[r])
            {
                indices[r] = 0;
                r++;
                if (r >= rank) return result;
                indices[r]++;
            }
            TSource src = (TSource)array.GetValue(indices)!;
            TResult dst = selector(src!);
            result.SetValue(dst, indices);
            indices[0]++;
        }
    }

    /// <summary>Enumerable for every element of multi-rank array.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    public static IEnumerable<T> Flatten<T>(Array array)
    {
        IList list = array;
        foreach (T value in list) yield return value;
    }

    /// <summary>Visit each element of an multi-dimensional array.</summary>
    /// <param name="array"></param>
    public static IEnumerable<(int[], T)> VisitArray<T>(Array array)
    {
        int rank = array.Rank;
        if (rank == 0) yield break;
        int[] lens = new int[rank];
        int[] indices = new int[rank];
        for (int i = 0; i < rank; i++) lens[i] = array.GetLength(i);
        while (true)
        {
            int r = 0;
            while (indices[r] >= lens[r])
            {
                indices[r] = 0;
                r++;
                if (r >= rank) yield break;
                indices[r]++;
            }
            T element = (T)array.GetValue(indices)!;
            yield return (indices, element!);
            indices[0]++;
        }
    }

    /// <summary>Visit each element of two multi-dimensional arrays.</summary>
    /// <param name="array1"></param>
    /// <param name="array2"></param>
    /// <returns>Arrays matched</returns>
    public static IEnumerable<(int[], T, T)> VisitArrays<T>(Array array1, Array array2)
    {
        int rank = array1.Rank;
        if (rank == 0) yield break;
        if (array2.Rank != rank) yield break;
        int[] lens = new int[rank];
        int[] indices = new int[rank];
        for (int i = 0; i < rank; i++)
        {
            int len = array1.GetLength(i);
            lens[i] = len;
            if (array2.GetLength(i) != len) yield break;
        }
        while (true)
        {
            int r = 0;
            while (indices[r] >= lens[r])
            {
                indices[r] = 0;
                r++;
                if (r >= rank) yield break;
                indices[r]++;
            }
            T element1 = (T)array1.GetValue(indices)!;
            T element2 = (T)array2.GetValue(indices)!;
            yield return (indices, element1, element2);
            indices[0]++;
        }
    }

    /// <summary>Get instances that implement <typeparamref name="T"/>.</summary>
    public static V[] GetInstancesOf<T, V>(this IList<T>? array)
    {
        // No array
        if (array == null || array.Count == 0) return Array.Empty<V>();
        // Count
        int c = 0;
        foreach (T value in array) if (value is V) c++;
        // Allocate
        V[] result = new V[c];
        int ix = 0;
        foreach (T value in array) if (value is V vvalue) result[ix++] = vvalue;
        // Return array
        return result;
    }

    /// <summary>Get first instance of <typeparamref name="T"/>.</summary>
    public static V GetFirstInstanceOf<T, V>(this IList<T>? array)
    {
        // No array
        if (array == null || array.Count == 0) return default!;
        // 
        foreach (T value in array) if (value is V _value) return _value;
        // No value
        return default!;
    }

    /// <summary>Try get first instance of <typeparamref name="T"/>.</summary>
    public static bool TryGetFirstInstanceOf<T, V>(this IList<T>? array, out V value)
    {
        // No array
        if (array == null || array.Count == 0) { value = default!; return false; }
        // 
        foreach (T tvalue in array) if (tvalue is V vvalue) { value = vvalue; return true; }
        // No value
        value = default!;
        return false;
    }

    /// <summary>Concatenate <paramref name="lists"/> into one array with one pass.</summary>
    public static T[] ToArrayAll<T>(params IList<T>[] lists)
    {
        // Total element count
        int count = 0;
        // Add up elements
        foreach (var list in lists) count += list.Count;
        // Allocate
        T[] result = new T[count];
        // Index
        int ix = 0;
        // Add elements
        foreach (var list in lists) foreach (var element in list) result[ix++] = element;
        // Return
        return result;
    }

}
