// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;

// <docs>
/// <summary>Escaper</summary>
public interface IEscaper
{
    /// <summary>Estimate number of chars required to escaped version of <paramref name="unescapedInput"/>.</summary>
    /// <returns>Number of chars in escaped version of <paramref name="unescapedInput"/>. -1 if could not estimate</returns>
    int EstimateEscapedLength(ReadOnlySpan<char> unescapedInput);

    /// <summary>Estimate number of chars required in unescaped version of <paramref name="escapedInput"/>.</summary>
    /// <returns>Number of chars in unescaped version of <paramref name="escapedInput"/>. -1 if could not estimate.</returns>
    int EstimateUnescapedLength(ReadOnlySpan<char> escapedInput);

    /// <summary>Escape <paramref name="unescapedInput"/> into <paramref name="escapedOutput"/>.</summary>
    /// <returns>Number of characters written to <paramref name="escapedOutput"/>. -1 if escape failed.</returns>
    int Escape(ReadOnlySpan<char> unescapedInput, Span<char> escapedOutput);

    /// <summary>Unescape <paramref name="escapedInput"/> into <paramref name="unescapedOutput"/>.</summary>
    /// <returns>Number of characters written to <paramref name="unescapedOutput"/>. -1 if unescape failed.</returns>
    /// <remarks>If escaper uses separator, then this method regardless unescapes the whole input and proceed through all separators.</remarks>
    int Unescape(ReadOnlySpan<char> escapedInput, Span<char> unescapedOutput);
}
// </docs>

// <docs2>
/// <summary>Unescapes and escapes with separator char/string.</summary>
public interface IEscaperSplitter : IEscaper
{
    /// <summary>Indicates whether escaper uses separator and can split parts</summary>
    bool CanSplit { get; }
    /// <summary>Separator string</summary>
    string? Separator { get; }

    /// <summary>Seeks until end of <paramref name="escapedInput"/> or until start of <see cref="Separator"/> sequence.</summary>
    /// <returns>Returns count of escaped and unescaped characters that scan forwarded</returns>
    (int escapedLength, int unescapedLength) SeekSeparator(ReadOnlySpan<char> escapedInput);

    /// <summary>Seeks until end of <paramref name="escapedInput"/> or until unescaped character that should be escaped occurs.</summary>
    /// <returns>Returns count of escaped and unescaped characters that scan forwarded</returns>
    (int escapedLength, int unescapedLength) SeekUnescaped(ReadOnlySpan<char> escapedInput);
}
// </docs2>
