// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Collections;

/// <summary>
/// This stack allocable enumerator returns an enumeration of 8-bit unicode codepoints.
/// 
/// It can be initialized with 8-bit, 16-bit and 32-bit enumerables, such as byte[], char[], etc.
/// </summary>
public struct UTF8Enumerator : IEnumerator<byte>
{
    EncodingType srcType;
    int count;
    IEnumerator<byte>? utf8;
    IEnumerator<char>? utf16;
    IEnumerator<int>? utf32;
    byte current;
    int q1, q2, q3;

    /// <summary>Construct enumerator from UTF-8 backend.</summary>
    /// <param name="utf8"></param>
    /// <param name="utf8length">number of codepoints to read from stream, or -1 for until end of stream</param>
    public UTF8Enumerator(IEnumerator<byte> utf8, int utf8length = -1)
    {
        srcType = EncodingType.UTF8;
        this.utf8 = utf8 ?? throw new ArgumentNullException(nameof(utf8));
        this.utf16 = null;
        this.utf32 = null;
        current = 0;
        this.count = utf8length >= 0 ? utf8length : int.MaxValue;
        q1 = -1; q2 = -1; q3 = -1;
    }

    /// <summary>Construct enumerator from UTF-16 backend.</summary>
    /// <param name="utf16"></param>
    /// <param name="utf16length">number of codepoints to read from stream, or -1 for until end of stream</param>
    public UTF8Enumerator(IEnumerator<char> utf16, int utf16length = -1)
    {
        srcType = EncodingType.UTF16;
        this.utf8 = null;
        this.utf16 = utf16 ?? throw new ArgumentNullException(nameof(utf16));
        this.utf32 = null;
        current = 0;
        this.count = utf16length >= 0 ? utf16length : int.MaxValue;
        q1 = -1; q2 = -1; q3 = -1;
    }

    /// <summary>Construct enumerator from UTF-32 backend.</summary>
    /// <param name="utf32"></param>
    /// <param name="utf32length">number of characters to read from stream, or -1 for until end of stream</param>
    public UTF8Enumerator(IEnumerator<int> utf32, int utf32length = -1)
    {
        srcType = EncodingType.UTF32;
        this.utf8 = null;
        this.utf16 = null;
        this.utf32 = utf32 ?? throw new ArgumentNullException(nameof(utf32));
        current = 0;
        this.count = utf32length >= 0 ? utf32length : int.MaxValue;
        q1 = -1; q2 = -1; q3 = -1;
    }

    /// <summary></summary>
    public byte Current => current;
    /// <summary></summary>
    object IEnumerator.Current => current;

    /// <summary></summary>
    public void Dispose()
    {
        utf8?.Dispose(); utf8 = null;
        utf16?.Dispose(); utf16 = null;
        utf32?.Dispose(); utf32 = null;
    }

    /// <summary></summary>
    public void Reset()
    {
        utf8?.Reset(); utf16?.Reset(); utf32?.Reset(); current = 0;
    }

    private int ReadUTF16()
    {
        // Read char
        if (utf16 == null || --count < 0 || !utf16.MoveNext()) return -1;
        char c1 = utf16.Current;

        // High surrogate
        if (c1 >= 0xd800 && c1 <= 0xdbff)
        {
            // Read low surrogate
            if (--count < 0 || !utf16.MoveNext()) return -1;
            char c2 = utf16.Current;
            return (int)((c1 - 0xd800) * 1024 + (c2 - 0xdc00)) + 0x10000;
        }

        // Low surrogate
        else if (c1 >= 0xdc00 && c1 <= 0xdfff)
        {
            // Encoding error, we missed the high surrogate. Return something.
            return c1 - 0xdc00 + 0x10000;
        }

        // No surrogate
        return c1;
    }

    /// <summary></summary>
    public bool MoveNext()
    {
        // Passthrough
        if (srcType == EncodingType.UTF8)
        {
            // Get next char.
            if (utf8 == null || --count < 0 || !utf8.MoveNext()) return false;

            // Return
            current = utf8.Current;
            return true;
        }

        // Empty queue
        if (q1 > 0) { current = (byte)q1; q1 = -1; return true; }
        if (q2 > 0) { current = (byte)q2; q2 = -1; return true; }
        if (q3 > 0) { current = (byte)q3; q3 = -1; return true; }

        int code = -1;
        if (srcType == EncodingType.UTF16)
        {
            code = ReadUTF16();
            if (code < 0) return false;
        }
        else if (srcType == EncodingType.UTF32)
        {
            // Read char
            if (utf32 == null || --count < 0 || !utf32.MoveNext()) return false;
            code = utf32.Current;
        }
        else return false;

        // Encoding error. 
        if (code < 0 || code > 0x10ffff) code = (int)((uint)code) % 0x110000;

        // One byte code
        if (code <= 0x7f) { current = (byte)code; return true; }

        // Two byte code
        if (code <= 0x7ff) { current = (byte)(0xC0 | (code >> 6)); q1 = 0x80 | (code & 0x3f); return true; }

        // Three byte code
        if (code <= 0xffff) { current = (byte)(0xE0 | (code >> 12)); q1 = 0x80 | ((code >> 6) & 0x3f); q2 = 0x80 | (code & 0x3f); return true; }

        // Four byte code
        current = (byte)(0xF0 | ((code >> 18) & 0x07));
        q1 = 0x80 | ((code >> 12) & 0x3f);
        q2 = 0x80 | ((code >> 6) & 0x3f);
        q3 = 0x80 | (code & 0x3f);
        return true;
    }
}

