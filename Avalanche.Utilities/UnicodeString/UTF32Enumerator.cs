// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Collections;

/// <summary>
/// This stack allocable enumerator returns an enumeration of 32-bit unicode codepoints.
/// 
/// It can be initialized with 8-bit, 16-bit and 32-bit enumerables, such as byte[], char[], etc.
/// </summary>
public struct UTF32Enumerator : IEnumerator<int>
{
    EncodingType srcType;
    int count;
    IEnumerator<byte>? utf8;
    IEnumerator<char>? utf16;
    IEnumerator<int>? utf32;
    int current;

    /// <summary>Construct enumerator from UTF-8 backend.</summary>
    /// <param name="utf8"></param>
    /// <param name="utf8length">optional source length</param>
    public UTF32Enumerator(IEnumerator<byte> utf8, int utf8length = -1)
    {
        srcType = EncodingType.UTF8;
        current = -1;
        this.utf8 = utf8 ?? throw new ArgumentNullException(nameof(utf8));
        this.utf16 = null;
        this.utf32 = null;
        this.count = utf8length >= 0 ? utf8length : int.MaxValue;
    }

    /// <summary>Construct enumerator from UTF-16 backend.</summary>
    /// <param name="utf16"></param>
    /// <param name="utf16length">optional source length</param>
    public UTF32Enumerator(IEnumerator<char> utf16, int utf16length = -1)
    {
        srcType = EncodingType.UTF16;
        current = -1;
        this.utf8 = null;
        this.utf16 = utf16 ?? throw new ArgumentNullException(nameof(utf16));
        this.utf32 = null;
        this.count = utf16length >= 0 ? utf16length : int.MaxValue;
    }

    /// <summary>Construct enumerator from UTF-32 backend.</summary>
    /// <param name="utf32"></param>
    /// <param name="utf32length">optional source length</param>
    public UTF32Enumerator(IEnumerator<int> utf32, int utf32length = -1)
    {
        srcType = EncodingType.UTF32;
        current = -1;
        this.utf8 = null;
        this.utf16 = null;
        this.utf32 = utf32 ?? throw new ArgumentNullException(nameof(utf32));
        this.count = utf32length >= 0 ? utf32length : int.MaxValue;
    }

    /// <summary></summary>
    public int Current => current;
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
        utf8?.Reset(); utf16?.Reset(); utf32?.Reset(); current = -1;
    }

    /// <summary></summary>
    public bool MoveNext()
    {
        if (srcType == EncodingType.UTF8)
        {
            // Disposed.
            if (utf8 == null) return false;

            // Read char
            if (--count < 0 || !utf8.MoveNext()) return false;
            int c1 = utf8.Current;
            if ((c1 & 0x80) == 0x00) { current = c1; return true; }

            // Encoding error. We are reading a char at middle of encoding.. Return something.
            if ((c1 & 0x40) == 0x00) { current = c1 & 0x3f; return true; }

            // Read next char
            if (--count < 0 || !utf8.MoveNext()) return false;
            int c2 = utf8.Current;

            // Two char encoding
            if ((c1 & 0xE0) == 0xC0) { current = (c1 & 0x1f) << 6 | (c2 & 0x3f); return true; }

            // Read next char
            if (--count < 0 || !utf8.MoveNext()) return false;
            int c3 = utf8.Current;

            // Three char encoding
            if ((c1 & 0xF0) == 0xE0) { current = (c1 & 0x0f) << 12 | (c2 & 0x3f) << 6 | (c3 & 0x3f); return true; }

            // Read next char
            if (--count < 0 || !utf8.MoveNext()) return false;
            int c4 = utf8.Current;

            // Four char encoding
            if ((c1 & 0xF8) == 0xF0) { current = (c1 & 0x07) << 18 | (c2 & 0x3f) << 12 | (c3 & 0x3f) << 6 | (c4 & 0x3f); return true; }

            // Encoding error. return something.
            current = (c1 & 0x07) << 18 | (c2 & 0x3f) << 12 | (c3 & 0x3f) << 6 | (c4 & 0x3f); return true;
        }
        else if (srcType == EncodingType.UTF16)
        {
            // Read char
            if (utf16 == null || --count < 0 || !utf16.MoveNext()) return false;
            char c1 = utf16.Current;

            // High surrogate
            if (c1 >= '\ud800' && c1 <= '\udbff')
            {
                // Read low surrogate
                if (--count < 0 || !utf16.MoveNext()) return false;
                char c2 = utf16.Current;
                current = (int)((c1 - '\ud800') * 'Ѐ' + (c2 - '\udc00')) + 65536;
                return true;
            }

            // Low surrogate
            else if (c1 >= '\udc00' && c1 <= '\udfff')
            {
                // Encoding error, we missed the high surrogate. Return something.
                current = c1 - '\udc00' + 65536;
                return true;
            }

            // No surrogate
            current = c1; return true;
        }
        else if (srcType == EncodingType.UTF32)
        {
            // Read char
            if (utf32 == null || --count < 0 || !utf32.MoveNext()) return false;

            // Return
            current = utf32.Current;
            return true;
        }
        return false;
    }

}

