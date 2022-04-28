// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections.Generic;
using Avalanche.Utilities.Internal;

/// <summary>
/// Escaper that uses separator.
/// 
/// As special case the following control characters are also escaped:
///     \\t
///     \\0
///     \\a
///     \\b
///     \\v
///     \\t
///     \\f
///     \\n
///     \\r
///     \\xHH
///     \\uHHHH
/// 
/// </summary>
public class Escaper : IEscaper, IEscaperSplitter
{
    /// <summary>Singleton for escaper where '.' is escaped, escape char is '\'</summary>
    static Escaper dotEscape = Escaper.Create('\\', '.');
    /// <summary>Singleton for escaper where ',' is escaped, escape char is '\'</summary>
    static Escaper commaEscape = Escaper.Create('\\', ',');
    /// <summary>Singleton for escaper where \\ is escaped, escape char is '\'</summary>
    static Escaper backslashEscape = Escaper.Create('\\', '\\');
    /// <summary>Singleton for escaper where " is escaped, escape char is '\'</summary>
    static Escaper quotesEscape = Escaper.Create('\\', '\\', '\"');
    /// <summary>Singleton for escaper where ';' is escaped, escape char is '\'</summary>
    static Escaper semicolonEscape = Escaper.Create('\\', ';');
    /// <summary>Singleton for escaper where ':' is escaped, escape char is '\'</summary>
    static Escaper colonEscape = Escaper.Create('\\', ':');
    /// <summary>Default escape map</summary>
    static IDictionary<char, char> controlCharEscapeMap = new LockableDictionary<char, char>
    {
        { '\0', '0' },
        { '\a', 'a' },
        { '\b', 'b' },
        { '\v', 'v' },
        //{ '\t', 't' },
        { '\f', 'f' },
        { '\n', 'n' },
        { '\r', 'r' },
    }.SetReadOnly();
    /// <summary>Default unescape map</summary>
    static IDictionary<char, char> controlCharUnescapeMap = new LockableDictionary<char, char>
    {
        { '0', '\0' },
        { 'a', '\a' },
        { 'b', '\b' },
        { 'v', '\v' },
        { 't', '\t' },
        { 'f', '\f' },
        { 'n', '\n' },
        { 'r', '\r' },
    }.SetReadOnly();

    /// <summary>Singleton for escaper where '.' is escaped, escape char is '\'</summary>
    public static Escaper Dot => dotEscape;
    /// <summary>Singleton for escaper where ',' is escaped, escape char is '\'</summary>
    public static Escaper Comma => commaEscape;
    /// <summary>Singleton for escaper where '\\' is escaped, escape char is '\'</summary>
    public static Escaper Backslash => backslashEscape;
    /// <summary>Singleton for escaper where quotes '"' are escaped, escape char is backslash '\'</summary>
    public static Escaper Quotes => quotesEscape;
    /// <summary>Singleton for escaper where ';' is escaped, escape char is '\'</summary>
    public static Escaper Semicolon => semicolonEscape;
    /// <summary>Singleton for escaper where ':' is escaped, escape char is '\'</summary>
    public static Escaper Colon => colonEscape;
    /// <summary>Singleton for escaper where "{" are escaped as "{{" and "}" as "}}" and has no control character escaping</summary>
    public static IEscaper Brace => BraceEscaper.Instance;

    /// <summary>Default escape map</summary>
    public static IDictionary<char, char> ControlCharEscapeMap => controlCharEscapeMap;
    /// <summary>Default unescape map</summary>
    public static IDictionary<char, char> ControlCharUnescapeMap => controlCharUnescapeMap;

    /// <summary>Character to escape with, e.g. '\'.</summary>
    public readonly char EscapeChar;
    /// <summary>Separator character. If same as <see cref="EscapeChar"/>, then separator is disabled.</summary>
    public readonly char SeparatorChar;
    /// <summary>Separator char</summary>
    public string? Separator { get; init; }

    /// <summary>Indicates whether escaper uses separator</summary>
    public bool CanSplit => EscapeChar != SeparatorChar;

    /// <summary>escape map</summary>
    protected Dictionary<char, char> escapeMap = new Dictionary<char, char>();
    /// <summary>Unescape map, key equals to character after <see cref="EscapeChar"/>.</summary>
    protected Dictionary<char, char> unescapeMap = new Dictionary<char, char>();

    /// <summary>Create escaper</summary>
    /// <param name="escapeChar">Character to escape with, e.g. '\'.</param>
    /// <param name="separatorChar">Separator character. If same as <see cref="EscapeChar"/>, then separator is disabled.</param>
    /// <param name="toEscapeChars">Other chars to escape.</param>
    /// <remarks>If <paramref name="escapeChar"/> is same as <paramref name="separatorChar"/>, then separator is not used.</remarks>
    public static Escaper Create(char escapeChar, char separatorChar, params char[]? toEscapeChars)
    {
        // Create character escape map
        Dictionary<char, char> escapeMap = new Dictionary<char, char>();
        escapeMap['\0'] = '0';
        escapeMap['\a'] = 'a';
        escapeMap['\b'] = 'b';
        escapeMap['\v'] = 'v';
        //escapeMap['\t'] = 't';
        escapeMap['\f'] = 'f';
        escapeMap['\n'] = 'n';
        escapeMap['\r'] = 'r';
        escapeMap[escapeChar] = escapeChar;
        escapeMap[separatorChar] = separatorChar;

        Dictionary<char, char> unescapeMap = new Dictionary<char, char>();
        unescapeMap['0'] = '\0';
        unescapeMap['a'] = '\a';
        unescapeMap['b'] = '\b';
        unescapeMap['v'] = '\v';
        unescapeMap['t'] = '\t';
        unescapeMap['f'] = '\f';
        unescapeMap['n'] = '\n';
        unescapeMap['r'] = '\r';
        unescapeMap[escapeChar] = escapeChar;
        unescapeMap[separatorChar] = separatorChar;

        if (toEscapeChars != null)
            foreach (char ch in toEscapeChars)
            {
                // Add tabulator to escape map
                if (ch == '\t')
                {
                    escapeMap['\t'] = 't';
                    unescapeMap['t'] = '\t';
                }
                else
                {
                    // Add to escape map
                    escapeMap[ch] = ch;
                    unescapeMap[ch] = ch;
                }
            }

        // Create escaper
        Escaper escaper = new Escaper(escapeChar, separatorChar, escapeMap, unescapeMap);
        // Return
        return escaper;
    }

    /// <summary>Create escaper</summary>
    /// <param name="escapeChar">Character to escape with, e.g. '\'.</param>
    /// <param name="separatorChar">Separator character. If same as <see cref="EscapeChar"/>, then separator is disabled.</param>
    /// <param name="escapeMap"></param>
    /// <param name="unescapeMap"></param>
    /// <remarks>If <paramref name="escapeChar"/> is same as <paramref name="separatorChar"/>, then separator is not used.</remarks>
    public Escaper(char escapeChar, char separatorChar, IEnumerable<KeyValuePair<char, char>> escapeMap, IEnumerable<KeyValuePair<char, char>> unescapeMap)
    {
        this.EscapeChar = escapeChar;
        this.SeparatorChar = separatorChar;
        this.Separator = new string(separatorChar, 1);

        // Create character escape map
        this.escapeMap = new Dictionary<char, char>(escapeMap);
        this.escapeMap[EscapeChar] = EscapeChar;
        this.escapeMap[SeparatorChar] = SeparatorChar;
        // Create character unescape map
        this.unescapeMap = new Dictionary<char, char>(unescapeMap);
        this.unescapeMap[EscapeChar] = EscapeChar;
        this.unescapeMap[SeparatorChar] = separatorChar;
    }

    /// <summary>Estimate number of chars required to escaped version of <paramref name="unescapedInput"/>.</summary>
    /// <returns>Chars in escaped version of <paramref name="unescapedInput"/>.</returns>
    public int EstimateEscapedLength(ReadOnlySpan<char> unescapedInput)
    {
        //
        int result = unescapedInput.Length, len = unescapedInput.Length;
        //
        for (int i = 0; i < len; i++)
        {
            // Get char
            char ch = unescapedInput[i];
            // Get to-escape char
            if (!escapeMap.TryGetValue(ch, out char che)) continue;
            // Add up escape char
            result++;
        }
        // Return result
        return result;
    }

    /// <summary>Estimate number of chars required in unescaped version of <paramref name="escapedInput"/>.</summary>
    /// <returns>Chars in unescaped version of <paramref name="escapedInput"/>.</returns>
    public int EstimateUnescapedLength(ReadOnlySpan<char> escapedInput)
    {
        //
        int result = escapedInput.Length, lastIndex = escapedInput.Length - 1;
        //
        for (int i = 0; i <= lastIndex; i++)
        {
            // Get char
            char ch = escapedInput[i];
            // Not escape char
            if (ch != EscapeChar) continue;
            // Reduce escape char
            result--;
            // Escape char is last char, nothing follows
            if (i == lastIndex) break;
            // Proceed to next after escape char
            ch = escapedInput[++i];
            // Number of hex-decimals to consume
            int hexLenToConsume = ch switch { 'x' => 2, 'u' => 4, _ => 0 };
            // No hexes
            if (hexLenToConsume == 0) continue;
            // Reduce 'x' or 'u'
            result--;
            // Consume hex-digits
            for (int j = 0; j < hexLenToConsume; j++)
            {
                // last char
                if (i == lastIndex) break;
                // Proceed and read next
                ch = escapedInput[++i];
                // Is hex digit?
                bool isHex = (ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F');
                // Did not get expected hex digit
                if (!isHex) break;
                // Reduce hex after first one
                if (j > 0) result--;
            }
        }
        // Return result
        return result;
    }

    /// <summary>Escape <paramref name="unescapedInput"/> into <paramref name="escapedOutput"/>.</summary>
    /// <returns>Number of characters written to <paramref name="escapedOutput"/>. -1 if escape failed.</returns>
    public int Escape(ReadOnlySpan<char> unescapedInput, Span<char> escapedOutput)
    {
        //
        int inputLen = unescapedInput.Length, outputLen = escapedOutput.Length, ox = 0;
        //
        for (int i = 0; i < inputLen; i++)
        {
            // Get char
            char ch = unescapedInput[i];
            // Handle non-escape char
            if (!escapeMap.TryGetValue(ch, out char che))
            {
                // Out of range
                if (ox >= outputLen) return -1;
                // Add char
                escapedOutput[ox++] = ch;
            }
            else
            // Handle escaping
            {
                // Out of range
                if (ox + 1 >= outputLen) return -1;
                // Add char
                escapedOutput[ox++] = EscapeChar;
                // Add char
                escapedOutput[ox++] = che;
            }
        }

        // Return
        return ox;
    }

    /// <summary>Unescape <paramref name="escapedInput"/> into <paramref name="unescapedOutput"/>.</summary>
    /// <returns>Number of characters written to <paramref name="unescapedOutput"/>. -1 if unescape failed.</returns>
    /// <remarks>If separator is found, then unescapes the whole input regardless.</remarks>
    public int Unescape(ReadOnlySpan<char> escapedInput, Span<char> unescapedOutput)
    {
        //
        int outputLen = unescapedOutput.Length, ox = 0, lastIndex = escapedInput.Length - 1;
        //
        for (int i = 0; i <= lastIndex; i++)
        {
            // Get char
            char ch = escapedInput[i];
            // Add non-escape char
            if (ch != EscapeChar)
            {
                // Assert output has room for one char
                if (ox >= outputLen) return -1;
                // Add char
                unescapedOutput[ox++] = ch;
                // Proceed to next input char
                continue;
            }

            // Proceed to next after escape char
            if (++i > lastIndex) break;
            // Proceed to next after escape char
            ch = escapedInput[i];
            // Place here next output char (if produced)
            int oc = -1;
            // Choose action
            switch (ch)
            {
                case 'x':
                case 'u':
                    // Number of hex-decimals to consume
                    int hexLenToConsume = ch switch { 'x' => 2, 'u' => 4, _ => 0 };
                    // No hexes
                    if (hexLenToConsume == 0) continue;
                    // Consume hex-digits
                    for (int j = 0; j < hexLenToConsume; j++)
                    {
                        // Out of range
                        if (ox >= outputLen) return ox;
                        // char is last char, nothing follows
                        if (i == lastIndex) break;
                        // Proceed to next after escape char
                        ch = escapedInput[++i];
                        // Convert 4-bit hex
                        int hex = (ch >= '0' && ch <= '9') ? ch - '0' :
                                  (ch >= 'a' && ch <= 'f') ? ch - 'a' + 10 :
                                  (ch >= 'A' && ch <= 'F') ? ch - 'A' + 10 :
                                  -1;
                        // Did not get hex
                        if (hex < 0) break;
                        // Apply hex
                        oc = oc < 0 ? hex : (oc << 4) | hex; // <- TODO verify this
                    }
                    // Case close
                    break;
                default:
                    if (unescapeMap.TryGetValue(ch, out char che)) oc = che; else oc = ch;
                    break;
            }
            // Unescape failed
            if (oc < 0) continue;
            // Assert output has room for one char
            if (ox >= outputLen) return -1;
            // Add char
            unescapedOutput[ox++] = (char)oc;
        }

        // Unescape succeeded
        return ox;
    }

    /// <summary>Seeks until end of <paramref name="escapedInput"/> or until start of <see cref="Separator"/>.</summary>
    /// <returns>Returns number of escaped and unescaped characters</returns>
    public (int escapedLength, int unescapedLength) SeekSeparator(ReadOnlySpan<char> escapedInput)
    {
        //
        int lastIndex = escapedInput.Length - 1, ox = 0, ix = 0;
        //
        for (; ix <= lastIndex; ix++)
        {
            // Get char
            char ch = escapedInput[ix];
            // Got separator (and separator is enabled)
            if (ch == SeparatorChar && EscapeChar != SeparatorChar) return (ix, ox);
            // Not escape char
            if (ch != EscapeChar) { ox++; continue; }
            // Escape char is last char, nothing follows
            if (ix == lastIndex) break;
            // Proceed to next after escape char
            ch = escapedInput[++ix];
            // Number of hex-decimals to consume
            int hexLenToConsume = ch switch { 'x' => 2, 'u' => 4, _ => 0 };
            // No hexes
            if (hexLenToConsume == 0) { ox++; continue; }
            // Consume hex-digits
            for (int j = 0; j < hexLenToConsume; j++)
            {
                // char is last char, nothing follows
                if (ix == lastIndex) break;
                // Proceed to next
                ch = escapedInput[++ix];
                // Is hex digit?
                bool isHex = (ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F');
                // Did not get expected hex digit
                if (!isHex) break;
                // Got atleast one hex
                if (j == 0) ox++;
            }
        }
        // Return result
        return (ix, ox);
    }

    /// <summary>Seek until end of <paramref name="escapedInput"/> to find first unescaped character that should be escaped.</summary>
    /// <returns>Returns number of escaped and unescaped characters</returns>
    public (int escapedLength, int unescapedLength) SeekUnescaped(ReadOnlySpan<char> escapedInput)
    {
        //
        int lastIndex = escapedInput.Length - 1, ox = 0, ix = 0;
        //
        for (; ix <= lastIndex; ix++)
        {
            // Get char
            char ch = escapedInput[ix];
            // Got non-escaped char
            if (EscapeChar != ch && escapeMap.TryGetValue(ch, out char escapedCh)) return (ix, ox);
            // Not escape char
            if (ch != EscapeChar) { ox++; continue; }
            // Escape char is last char, nothing follows
            if (ix == lastIndex) break;
            // Proceed to next after escape char
            ch = escapedInput[++ix];
            // Number of hex-decimals to consume
            int hexLenToConsume = ch switch { 'x' => 2, 'u' => 4, _ => 0 };
            // No hexes
            if (hexLenToConsume == 0) { ox++; continue; }
            // Consume hex-digits
            for (int j = 0; j < hexLenToConsume; j++)
            {
                // char is last char, nothing follows
                if (ix == lastIndex) break;
                // Proceed to next
                ch = escapedInput[++ix];
                // Is hex digit?
                bool isHex = (ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F');
                // Did not get expected hex digit
                if (!isHex) break;
                // Got atleast one hex
                if (j == 0) ox++;
            }
        }
        // Return result
        return (ix, ox);
    }
}
