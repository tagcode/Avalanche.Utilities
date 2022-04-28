// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Collections.Generic;

/// <summary>
/// Inplace sorter that is intended specifically for struct based lists, but works on any <see cref="IList{T}"/>.
/// </summary>
/// <typeparam name="List"></typeparam>
/// <typeparam name="Element"></typeparam>
public struct StructListSorter<List, Element> where List : IList<Element>
{
    /// <summary>Element comparer</summary>
    readonly IComparer<Element> comparer;

    /// <summary>Create sorter</summary>
    public StructListSorter(IComparer<Element>? comparer = default)
    {
        this.comparer = comparer ?? Comparer<Element>.Default;
    }

    /// <summary>Reverse elements of <paramref name="list"/>.</summary>
    public static void Reverse(ref List list)
    {
        int mid = list.Count / 2;
        for (int i = 0, j = list.Count - 1; i < mid; i++, j--)
        {
            // Swap list[i] and list[j]
            (list[j], list[i]) = (list[i], list[j]);
        }
    }

    /// <summary>Sort elements of <paramref name="list"/>.</summary>
    public void Sort(ref List list) => QuickSort(ref list, 0, list.Count - 1);

    /// <summary>Internal sort</summary>
    private void QuickSort(ref List list, int left, int right)
    {
        if (left < right)
        {
            int pivot = Partition(ref list, left, right);

            if (pivot > 1)
            {
                QuickSort(ref list, left, pivot - 1);
            }
            if (pivot + 1 < right)
            {
                QuickSort(ref list, pivot + 1, right);
            }
        }
    }

    /// <summary>Internal</summary>
    private int Partition(ref List list, int left, int right)
    {
        if (left > right) return -1;
        int end = left;
        Element pivot = list[right];
        for (int i = left; i < right; i++)
        {
            int c = comparer.Compare(list[i], pivot);
            if (c < 0)
            {
                // Swap list[i] and list[end]
                (list[end], list[i]) = (list[i], list[end]);
                end++;
            }
        }
        // Swap list[end] and list[right]
        (list[right], list[end]) = (list[end], list[right]);
        return end;
    }

    /// <summary>Search for a <paramref name="valueToSearch"/> using using associated comparer.</summary>
    /// <remarks>The content must be in ascending order.</remarks>
    /// <param name="valueToSearch">value to search</param>
    /// <returns>zero or positive: an exact match, negative: -insert index-1 </returns>
    public int BinarySearch(ref List list, Element valueToSearch)
    {
        // Init
        int start = 0, end = list.Count - 1;
        // For each
        while (start <= end)
        {
            // Index
            int index = start + ((end - start) >> 1);
            // Get
            Element value = list[index];
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

    /// <summary>Sort elements of list into inverse order.</summary>
    /// <param name="list"></param>
    /// <param name="left"></param>
    /// <param name="right"></param>
    public void QuickSortInverse(ref List list, int left, int right)
    {
        if (left < right)
        {
            int pivot = PartitionInverse(ref list, left, right);

            if (pivot > 1)
            {
                QuickSortInverse(ref list, left, pivot - 1);
            }
            if (pivot + 1 < right)
            {
                QuickSortInverse(ref list, pivot + 1, right);
            }
        }
    }

    /// <summary>Internal</summary>
    private int PartitionInverse(ref List list, int left, int right)
    {
        if (left > right) return -1;
        int end = left;
        Element pivot = list[right];
        for (int i = left; i < right; i++)
        {
            int c = comparer.Compare(list[i], pivot);
            if (c > 0)
            {
                // Swap list[i] and list[end]
                (list[end], list[i]) = (list[i], list[end]);
                end++;
            }
        }
        // Swap list[end] and list[right]
        (list[right], list[end]) = (list[end], list[right]);
        return end;
    }

}
