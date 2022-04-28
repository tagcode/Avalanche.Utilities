// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Buffers;
using System.Runtime.InteropServices;

/// <summary>Extension methods for <see cref="ReadOnlyMemory{T}"/> and <see cref="Memory{T}"/>.</summary>
public static class MemoryExtensions
{
    /// <summary>Create memory that unifies <paramref name="a"/> and <paramref name="b"/> into memory range that includes both.</summary>
    public static ReadOnlyMemory<char> UnifyStringWith(this ReadOnlyMemory<char> a, ReadOnlyMemory<char> b)
    {
        // Both empty
        if (a.Length == 0 && b.Length == 0) return a;
        // Only a has content
        if (a.Length > 0 && b.Length == 0) return a;
        // Only b has content
        if (a.Length == 0 && b.Length > 0) return b;
        // Get both strings
        if (MemoryMarshal.TryGetString(a, out string? a_str, out int a_start, out int a_length) &&
            MemoryMarshal.TryGetString(b, out string? b_str, out int b_start, out int b_length) &&
            object.Equals(a_str, b_str))
        {
            int _start = Math.Min(a_start, b_start), _end = Math.Max(a_start + a_length, b_start + b_length);
            ReadOnlyMemory<char> result = a_str!.AsMemory(_start, _end - _start);
            return result;
        }
        // Fallback: concat to new string
        else
        {
            int count = a.Length + b.Length;
            char[] arr = ArrayPool<char>.Shared.Rent(count);
            a.CopyTo(arr.AsMemory());
            b.CopyTo(arr.AsMemory(a.Length));
            ArrayPool<char>.Shared.Return(arr);
            string str = new string(arr, 0, count);
            return str.AsMemory();
        }
    }
    
    /// <summary>Get original string and index range in it, or fallback to creating a new string if original could not be retrieved.</summary>
    public static (string text, int start, int length) GetString(this ReadOnlyMemory<char> memory)
    {
        // Get string
        if (MemoryMarshal.TryGetString(memory, out string? text, out int start, out int length) && text != null) return (text, start, length);
        // Allocate new string
        text = new string(memory.Span);
        return (text, 0, text.Length);
    }

    /// <summary>Get-or-create string.</summary>
    public static string AsString(this ReadOnlyMemory<char> memory)
    {
        // Get string
        if (MemoryMarshal.TryGetString(memory, out string? text, out int start, out int length) && text != null) if (start == 0 && length == text.Length) return text;
        // Create string
        text = new string(memory.Span);
        return text;
    }

    /// <summary>Returns the right side slice after the <paramref name="leftSideMemory"/> on broader <paramref name="broadMemory"/>.</summary>
    /// <return>
    ///  broadMemory    = xxxxxxxxxxxxxxxxxxxxxx
    ///  leftSideMemory = llllllll
    ///  return         =         rrrrrrrrrrrrrr
    /// </return>
    /// <example>
    /// var memory = new int[10].AsMemory();
    /// var leftSideSlice = memory.Slice(0, 4);
    /// var rightSideSlice = memory.SliceAfter(leftSideSlice);
    /// </example>
    public static ReadOnlyMemory<char> SliceAfter(this ReadOnlyMemory<char> broadMemory, ReadOnlyMemory<char> leftSideMemory)
    {
        //
        if (MemoryMarshal.TryGetString(leftSideMemory, out string? text, out int start, out int length))
        {
            // Return slice
            return text.AsMemory().Slice(start+length, text.Length-start-length);
        }
        // (Returns fallback array if not empty, so cannot be used)
        if (!leftSideMemory.IsEmpty && MemoryMarshal.TryGetArray<char>(leftSideMemory, out ArraySegment<char> array))
        {
            // Return slice
            return broadMemory.Slice(array.Offset + array.Count, broadMemory.Length - array.Offset - array.Count);
        }
        // Guess that there is no gap.
        return broadMemory.Slice(leftSideMemory.Length);
    }

    /// <summary>Slice after <paramref name="broadMemory"/>.</summary>
    public static ReadOnlyMemory<char> SliceAfter(this ReadOnlyMemory<char> broadMemory)
    {
        //
        if (MemoryMarshal.TryGetString(broadMemory, out string? text, out int start, out int length))
        {
            // Return slice
            return text.AsMemory().Slice(start+length, text.Length-start-length);
        }
        // (Returns fallback array if not empty, so cannot be used)
        if (!broadMemory.IsEmpty && MemoryMarshal.TryGetArray<char>(broadMemory, out ArraySegment<char> array) && array.Array!=null)
        {
            // Return slice
            return array.Array.AsMemory().Slice(array.Offset + array.Count, array.Array.Length - array.Offset - array.Count);
        }
        // Guess that there is no gap.
        return broadMemory.Slice(broadMemory.Length);
    }

    /// <summary>Returns the right side slice after the <paramref name="leftSideMemory"/> on broader <paramref name="broadMemory"/>.</summary>
    /// <return>
    ///  broadMemory    = xxxxxxxxxxxxxxxxxxxxxx
    ///  leftSideMemory = llllllll
    ///  return         =         rrrrrrrrrrrrrr
    /// </return>
    /// <example>
    /// var memory = new int[10].AsMemory();
    /// var leftSideSlice = memory.Slice(0, 4);
    /// var rightSideSlice = memory.SliceAfter(leftSideSlice);
    /// </example>
    public static Memory<char> SliceAfter(this Memory<char> broadMemory, Memory<char> leftSideMemory)
    {
        // (Returns fallback array if not empty, so cannot be used)
        if (!leftSideMemory.IsEmpty && MemoryMarshal.TryGetArray<char>(leftSideMemory, out ArraySegment<char> array) && array.Array != null)
        {
            // Return slice
            return array.Array.AsMemory().Slice(array.Offset + array.Count, array.Array.Length - array.Offset - array.Count);
        }
        // Guess that there is no gap.
        return broadMemory.Slice(leftSideMemory.Length);
    }

    /// <summary>Slice after <paramref name="broadMemory"/>.</summary>
    public static Memory<char> SliceAfter(this Memory<char> broadMemory)
    {
        // (Returns fallback array if not empty, so cannot be used)
        if (!broadMemory.IsEmpty && MemoryMarshal.TryGetArray<char>(broadMemory, out ArraySegment<char> array) && array.Array != null)
        {
            // Return slice
            return array.Array.AsMemory().Slice(array.Offset + array.Count, array.Array.Length - array.Offset - array.Count);
        }
        // Guess that there is no gap.
        return broadMemory.Slice(broadMemory.Length);
    }

    /*
    /// <summary>Returns the right side slice after the <paramref name="leftSideMemory"/> on broader <paramref name="broadMemory"/>.</summary>
    /// <return>
    ///  broadMemory    = xxxxxxxxxxxxxxxxxxxxxx
    ///  leftSideMemory = llllllll
    ///  return         =         rrrrrrrrrrrrrr
    /// </return>
    /// <example>
    /// var memory = new int[10].AsMemory();
    /// var leftSideSlice = memory.Slice(0, 4);
    /// var rightSideSlice = memory.SliceAfter(leftSideSlice);
    /// </example>
    public static ReadOnlyMemory<T> SliceAfter<T>(this ReadOnlyMemory<T> broadMemory, ReadOnlyMemory<T> leftSideMemory)
    {
        //
        if (!leftSideMemory.IsEmpty && MemoryMarshal.TryGetArray(leftSideMemory, out ArraySegment<T> array))
        {
            // Return slice
            return array.Array.AsMemory().Slice(array.Offset + array.Count, broadMemory.Length - array.Offset - array.Count);
        }
        // Guess that there is no gap.
        return broadMemory.Slice(leftSideMemory.Length);
    }

    /// <summary>Slice after <paramref name="broadMemory"/>.</summary>
    public static ReadOnlyMemory<T> SliceAfter<T>(this ReadOnlyMemory<T> broadMemory)
    {
        //
        if (!broadMemory.IsEmpty && MemoryMarshal.TryGetArray(broadMemory, out ArraySegment<T> array) && array.Array != null)
        {
            // Return slice
            return array.Array.AsMemory().Slice(array.Offset + array.Count, array.Array.Length - array.Offset - array.Count);
        }
        // Guess that there is no gap.
        return broadMemory.Slice(broadMemory.Length);
    }

    /// <summary>Returns the right side slice after the <paramref name="leftSideMemory"/> on broader <paramref name="broadMemory"/>.</summary>
    /// <return>
    ///  broadMemory    = xxxxxxxxxxxxxxxxxxxxxx
    ///  leftSideMemory = llllllll
    ///  return         =         rrrrrrrrrrrrrr
    /// </return>
    /// <example>
    /// var memory = new int[10].AsMemory();
    /// var leftSideSlice = memory.Slice(0, 4);
    /// var rightSideSlice = memory.SliceAfter(leftSideSlice);
    /// </example>
    public static Memory<T> SliceAfter<T>(this Memory<T> broadMemory, Memory<T> leftSideMemory)
    {
        //
        if (!leftSideMemory.IsEmpty && MemoryMarshal.TryGetArray(leftSideMemory, out ArraySegment<T> array))
        {
            // Return slice
            return array.Array.AsMemory().Slice(array.Offset + array.Count, broadMemory.Length - array.Offset - array.Count);
        }
        // Guess that there is no gap.
        return broadMemory.Slice(leftSideMemory.Length);
    }

    /// <summary>Slice after <paramref name="broadMemory"/>.</summary>
    public static Memory<T> SliceAfter<T>(this Memory<T> broadMemory)
    {
        //
        if (!broadMemory.IsEmpty && MemoryMarshal.TryGetArray(broadMemory, out ArraySegment<T> array) && array.Array != null)
        {
            // Return slice
            return array.Array.AsMemory().Slice(array.Offset + array.Count, array.Array.Length - array.Offset - array.Count);
        }
        // Guess that there is no gap.
        return broadMemory.Slice(broadMemory.Length);
    }
    */
    /// <summary></summary>
    public static int Index(this ReadOnlyMemory<char> memory)
    {
        if (MemoryMarshal.TryGetString(memory, out string? text, out int start, out int length)) return start;
        if (MemoryMarshal.TryGetArray(memory, out ArraySegment<char> array)) return array.Offset;
        return 0;
    }

    /// <summary></summary>
    public static int Index(this Memory<char> memory)
    {
        if (MemoryMarshal.TryGetString(memory, out string? text, out int start, out int length)) return start;
        if (MemoryMarshal.TryGetArray(memory, out ArraySegment<char> array)) return array.Offset;
        return 0;
    }

}

