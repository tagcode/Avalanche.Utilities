// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

/// <summary>Extension methods for <see cref="IEscaper"/>.</summary>
public static class EscaperExtensions
{
    /// <summary>Estimate number of chars required to escaped version of <paramref name="unescapedInput"/>.</summary>
    /// <returns>Number of chars in escaped version of <paramref name="unescapedInput"/>. -1 if could not estimate</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static int EstimateEscapedLength(this IEscaper escaper, ReadOnlyMemory<char> unescapedInput) => escaper.EstimateEscapedLength(unescapedInput.Span);

    /// <summary>Estimate number of chars required in unescaped version of <paramref name="escapedInput"/>.</summary>
    /// <returns>Number of chars in unescaped version of <paramref name="escapedInput"/>. -1 if could not estimate.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static int EstimateUnescapedLength(this IEscaper escaper, ReadOnlyMemory<char> escapedInput) => escaper.EstimateUnescapedLength(escapedInput.Span);

    /// <summary>Escape <paramref name="unescapedInput"/> into <paramref name="escapedOutput"/>.</summary>
    /// <returns>Number of characters written to <paramref name="escapedOutput"/>. -1 if escape failed.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static int Escape(this IEscaper escaper, ReadOnlyMemory<char> unescapedInput, Memory<char> escapedOutput) => escaper.Escape(unescapedInput.Span, escapedOutput.Span);

    /// <summary>Unescape <paramref name="escapedInput"/> into <paramref name="unescapedOutput"/>.</summary>
    /// <returns>Number of characters written to <paramref name="unescapedOutput"/>. -1 if unescape failed.</returns>
    /// <remarks>If escaper uses separator, then this method regardless unescapes the whole input and proceed through all separators.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static int Unescape(this IEscaper escaper, ReadOnlyMemory<char> escapedInput, Memory<char> unescapedOutput) => escaper.Unescape(escapedInput.Span, unescapedOutput.Span);

    /// <summary>Escape <paramref name="inputString"/>.</summary>
    /// <exception cref="NotSupportedException">If <paramref name="escaper"/> could not escape <paramref name="inputString"/>.</exception>
    public static string? Escape(this IEscaper escaper, string? inputString)
    {
        // Handle null
        if (inputString == null) return null;
        // Span it
        ReadOnlySpan<char> input = inputString;
        // Estimate length
        int len = escaper.EstimateEscapedLength(input);
        // Allocate from pool
        char[] output = ArrayPool<char>.Shared.Rent(len);
        //
        try
        {
            // Slice span
            Span<char> outputSpan = output.AsSpan();
            // Escape
            int outputLen = escaper.Escape(input, outputSpan);
            // Did not escape
            if (outputLen < 0) throw new NotSupportedException("Escape not supported");
            // Slice span
            outputSpan = outputSpan.Slice(0, outputLen);
            // Equal sequences
            if (inputString.Length == outputLen && System.MemoryExtensions.SequenceEqual(outputSpan, inputString.AsSpan())) return inputString;
            // Wrape result into string
            string result = new string(outputSpan);
            // Return
            return result;
        } finally
        {
            // Return rental
            ArrayPool<char>.Shared.Return(output, clearArray: true);
        }
    }

    /// <summary>Escape <paramref name="input"/>.</summary>
    /// <exception cref="NotSupportedException">If <paramref name="escaper"/> could not escape <paramref name="input"/>.</exception>
    public static string Escape(this IEscaper escaper, ReadOnlyMemory<char> input)
    {
        // Estimate length
        int len = escaper.EstimateEscapedLength(input);
        // Allocate from pool
        char[] output = ArrayPool<char>.Shared.Rent(len);
        //
        try
        {
            // Slice span
            Span<char> outputSpan = output.AsSpan();
            // Escape
            int outputLen = escaper.Escape(input.Span, outputSpan);
            // Did not escape
            if (outputLen < 0) throw new NotSupportedException("Escape not supported");
            // Slice span
            outputSpan = outputSpan.Slice(0, outputLen);
            // Input same as output
            if (outputLen == input.Length && MemoryMarshal.TryGetString(input, out string? inputString, out int start, out int length) && inputString != null && start == 0 && inputString.Length == outputLen) return inputString;
            // Wrap result into string
            string result = new string(outputSpan);
            // Return
            return result;
        }
        finally
        {
            // Return rental
            ArrayPool<char>.Shared.Return(output, clearArray: true);
        }
    }

    /// <summary>Escape <paramref name="input"/>.</summary>
    /// <exception cref="NotSupportedException">If <paramref name="escaper"/> could not escape <paramref name="input"/>.</exception>
    public static ReadOnlyMemory<char> Escape2(this IEscaper escaper, ReadOnlyMemory<char> input)
    {
        // Estimate length
        int len = escaper.EstimateEscapedLength(input);
        // Allocate from pool
        char[] output = ArrayPool<char>.Shared.Rent(len);
        //
        try
        {
            // Slice span
            Span<char> outputSpan = output.AsSpan();
            // Escape
            int outputLen = escaper.Escape(input.Span, outputSpan);
            // Did not escape
            if (outputLen < 0) throw new NotSupportedException("Escape not supported");
            // Slice span
            outputSpan = outputSpan.Slice(0, outputLen);
            // Input same as output
            if (outputLen == input.Length && System.MemoryExtensions.SequenceEqual(input.Span, outputSpan)) return input;
            // Wrap result into string
            string result = new string(outputSpan);
            // Return
            return result.AsMemory();
        }
        finally
        {
            // Return rental
            ArrayPool<char>.Shared.Return(output, clearArray: true);
        }
    }

    /// <summary>Escape <paramref name="input"/>.</summary>
    /// <exception cref="NotSupportedException">If <paramref name="escaper"/> could not escape <paramref name="input"/>.</exception>
    public static string Escape(this IEscaper escaper, ReadOnlySpan<char> input)
    {
        // Estimate length
        int len = escaper.EstimateEscapedLength(input);
        // Allocate from pool
        char[] output = ArrayPool<char>.Shared.Rent(len);
        //
        try
        {
            // Slice span
            Span<char> outputSpan = output.AsSpan();
            // Escape
            int outputLen = escaper.Escape(input, outputSpan);
            // Did not escape
            if (outputLen < 0) throw new NotSupportedException("Escape not supported");
            // Slice span
            outputSpan = outputSpan.Slice(0, outputLen);
            // Wrape result into string
            string result = new string(outputSpan);
            // Return
            return result;
        }
        finally
        {
            // Return rental
            ArrayPool<char>.Shared.Return(output, clearArray: true);
        }
    }

    /// <summary>Unescape <paramref name="inputString"/>.</summary>
    /// <exception cref="NotSupportedException">If <paramref name="escaper"/> could not unescape <paramref name="inputString"/>.</exception>
    public static string? Unescape(this IEscaper escaper, string? inputString)
    {
        // Handle null
        if (inputString == null) return null;
        // Span it
        ReadOnlySpan<char> input = inputString;        
        // Estimate length
        int len = escaper.EstimateUnescapedLength(input);
        // Allocate from pool
        char[] output = ArrayPool<char>.Shared.Rent(len);
        //
        try
        {
            // Slice span
            Span<char> outputSpan = output.AsSpan();
            // Unescape
            int outputLen = escaper.Unescape(input, output);
            // Did not unescape
            if (outputLen < 0) throw new NotSupportedException("Unescape not supported");
            // Slice span
            outputSpan = outputSpan.Slice(0, outputLen);
            // Equal sequences
            if (inputString.Length == outputLen && System.MemoryExtensions.SequenceEqual(outputSpan, inputString.AsSpan())) return inputString;
            // Wrape result into string
            string result = new string(outputSpan);
            // Return
            return result;
        }
        finally
        {
            // Return rental
            ArrayPool<char>.Shared.Return(output, clearArray: true);
        }
    }

    /// <summary>Unescape <paramref name="input"/>.</summary>
    /// <exception cref="NotSupportedException">If <paramref name="escaper"/> could not unescape <paramref name="input"/>.</exception>
    public static string Unescape(this IEscaper escaper, ReadOnlyMemory<char> input)
    {
        // Estimate length
        int len = escaper.EstimateUnescapedLength(input);
        // Allocate from pool
        char[] output = ArrayPool<char>.Shared.Rent(len);
        //
        try
        {
            // Slice span
            Span<char> outputSpan = output.AsSpan();
            // Unescape
            int outputLen = escaper.Unescape(input.Span, outputSpan);
            // Did not unescape
            if (outputLen < 0) throw new NotSupportedException("Unescape not supported");
            // Slice span
            outputSpan = outputSpan.Slice(0, outputLen);
            // Input same as output
            if (outputLen == input.Length && MemoryMarshal.TryGetString(input, out string? inputString, out int start, out int length) && inputString != null && start == 0 && inputString.Length == outputLen) return inputString;
            // Wrap result into string
            string result = new string(outputSpan);
            // Return
            return result;
        }
        finally
        {
            // Return rental
            ArrayPool<char>.Shared.Return(output, clearArray: true);
        }
    }

    /// <summary>Unescape <paramref name="input"/>.</summary>
    /// <exception cref="NotSupportedException">If <paramref name="escaper"/> could not unescape <paramref name="input"/>.</exception>
    public static ReadOnlyMemory<char> Unescape2(this IEscaper escaper, ReadOnlyMemory<char> input)
    {
        // Estimate length
        int len = escaper.EstimateUnescapedLength(input);
        // Allocate from pool
        char[] output = ArrayPool<char>.Shared.Rent(len);
        //
        try
        {
            // Slice span
            Span<char> outputSpan = output.AsSpan();
            // Unescape
            int outputLen = escaper.Unescape(input.Span, outputSpan);
            // Did not unescape
            if (outputLen < 0) throw new NotSupportedException("Unescape not supported");
            // Slice span
            outputSpan = outputSpan.Slice(0, outputLen);
            // Input same as output
            if (outputLen == input.Length && System.MemoryExtensions.SequenceEqual(input.Span, outputSpan)) return input;
            // Wrap result into string
            string result = new string(outputSpan);
            // Return
            return result.AsMemory();
        }
        finally
        {
            // Return rental
            ArrayPool<char>.Shared.Return(output, clearArray: true);
        }
    }

    /// <summary>Unescape <paramref name="input"/>.</summary>
    /// <exception cref="NotSupportedException">If <paramref name="escaper"/> could not unescape <paramref name="input"/>.</exception>
    public static string Unescape(this IEscaper escaper, ReadOnlySpan<char> input)
    {
        // Estimate length
        int len = escaper.EstimateUnescapedLength(input);
        // Allocate from pool
        char[] output = ArrayPool<char>.Shared.Rent(len);
        //
        try
        {
            // Slice span
            Span<char> outputSpan = output.AsSpan();
            // Unescape
            int outputLen = escaper.Unescape(input, outputSpan);
            // Did not unescape
            if (outputLen < 0) throw new NotSupportedException("Unescape not supported");
            // Slice span
            outputSpan = outputSpan.Slice(0, outputLen);
            // Wrap result into string
            string result = new string(outputSpan);
            // Return
            return result;
        }
        finally
        {
            // Return rental
            ArrayPool<char>.Shared.Return(output, clearArray: true);
        }
    }

    /// <summary>Escapes each string in <paramref name="input"/> and adds separator in between.</summary>
    /// <param name="escaper"></param>
    /// <param name="input">string enumerable. Is enumerated twice.</param>
    /// <returns>True if result was written</returns>
    public static bool TryEscapeJoin(this IEscaper escaper, IEnumerable<string> input, [MaybeNullWhen(false)] out string output)
    {
        // Cast to splitter
        if (escaper is not IEscaperSplitter splitter || !splitter.CanSplit || splitter.Separator == null) { output = null; return false; }
        // Estimate length
        int len = 0;
        // Get separator
        ReadOnlySpan<char> separator = splitter.Separator!;
        // Part index
        int partIx = 0;
        // Calculate total length
        foreach (string part in input)
        {
            // Add separator
            if (partIx++ > 0) len += separator.Length;
            // Estimate escaped length
            int partEscapeLength = escaper.EstimateEscapedLength(part);
            // Add escaped length
            len += partEscapeLength;
        }
        // Allocate from stack / heap
        Span<char> outputSpan = len < 512 ? stackalloc char[len] : new char[len];
        // Work output
        Span<char> workOutput = outputSpan;
        // Part index
        partIx = 0;
        // Append each
        foreach (string part in input)
        {
            // Append separator
            if (partIx++ > 0)
            {
                // No room for separator
                if (workOutput.Length < separator.Length) { output = null; return false; }
                // Append separator
                separator.CopyTo(workOutput);
                // Slice output
                workOutput = workOutput.Slice(separator.Length);
            }
            // Write escaped to output
            int outputLen = splitter.Escape(part, workOutput);
            // Failed
            if (outputLen < 0) { output = null; return false; }
            // Slice off output for the part
            workOutput = workOutput.Slice(outputLen);
        }
        // Wrap result into string
        output = new string(outputSpan);
        // Return
        return true;
    }

    /// <summary>Escapes each string in <paramref name="input"/> and adds separator in between.</summary>
    /// <param name="escaper"></param>
    /// <param name="input">string enumerable. Is enumerated twice.</param>
    /// <returns>Joined <paramref name="input"/></returns>
    /// <exception cref="NotSupportedException">If join was not supported by <paramref name="escaper"/>.</exception>
    public static string EscapeJoin(this IEscaper escaper, IEnumerable<string> input)
    {
        // Try escape
        bool ok = escaper.TryEscapeJoin(input, out string? output);
        // Not ok
        if (!ok || output == null) throw new NotSupportedException();
        // Return result
        return output;
    }

    /// <summary>Try to unescape and separate </summary>
    /// <param name="escaper"></param>
    /// <param name="input">input</param>
    /// <param name="output">Output where to write parts. Caller must have initialized.</param>
    /// <returns>separated parts</returns>
    public static bool TryUnescapeSplit<LIST>(this IEscaper escaper, ReadOnlySpan<char> input, ref LIST output) where LIST : IList<string>
    {
        // Cast to splitter
        if (escaper is not IEscaperSplitter splitter || !splitter.CanSplit) return false;

        // Get separator
        ReadOnlySpan<char> separator = splitter.Separator!;

        // Separator is empty
        if (separator.Length == 0)
        {
            // Unescape whole thing
            string? outputStr = escaper.Unescape(input);
            // Empty
            if (outputStr == null) return false;
            // Allocate one
            output.Add(outputStr);
            // Return 
            return true;
        }

        // Evaluate buffer length
        int bufLen = Math.Min(input.Length + 16, 512);
        // Allocate buffer
        Span<char> buf = stackalloc char[bufLen];
        // Estimate part count
        while (true)
        {
            // Proceed on part
            (int partEscapedLength, int partUnescapedLength) = splitter.SeekSeparator(input);
            // Allocate from stack / heap
            Span<char> part = partUnescapedLength <= bufLen ? buf.Slice(0, partUnescapedLength) : new char[partUnescapedLength];
            // Slice to input part
            ReadOnlySpan<char> inputPart = input.Slice(0, partEscapedLength);
            // Write as unescaped
            int partLen = escaper.Unescape(inputPart, part);
            // Unescape failed
            if (partLen < 0) return false;
            // Slice off part
            input = input.Slice(partEscapedLength);
            // Create part string
            string partStr = new string(part);
            // Add to result
            output.Add(partStr);

            // No separator left
            if (input.Length < separator.Length) break;
            //
            int ixSeparator = 0;
            // Step over separator
            for (; ixSeparator < separator.Length; ixSeparator++)
            {
                // Input matches to separator
                if (input[ixSeparator] != separator[ixSeparator]) break;
            }
            // Forwarded nothing
            if (partEscapedLength == 0 && ixSeparator == 0) break;
            // Slice off separator
            input = input.Slice(separator.Length);
        }

        // Done
        return true;
    }

    /// <summary>Try to unescape and separate </summary>
    /// <param name="escaper"></param>
    /// <param name="input">input</param>
    /// <returns>separated parts</returns>
    /// <exception cref="NotSupportedException">If split is not supported by <paramref name="escaper"/></exception>
    public static List<string> UnescapeSplit(this IEscaper escaper, ReadOnlySpan<char> input)
    {
        // Create list
        List<string> output = new List<string>(8);
        // Split
        bool ok = escaper.TryUnescapeSplit(input, ref output);
        // Failed
        if (!ok || output == null) throw new NotSupportedException();
        // Return
        return output;
    }


}
