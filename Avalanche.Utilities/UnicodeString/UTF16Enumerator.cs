// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Collections;

/// <summary>
/// This stack allocable enumerator returns an enumeration of 16-bit unicode codepoints.
/// 
/// It can be initialized with 8-bit, 16-bit and 32-bit enumerables, such as byte[], char[], etc.
/// </summary>
public struct UTF16Enumerator : IEnumerator<char>
{
    EncodingType srcType;
    int count;
    IEnumerator<byte>? utf8;
    IEnumerator<char>? utf16;
    IEnumerator<int>? utf32;
    char current;
    int lowSurrogateQueue;

    /// <summary>Construct enumerator from UTF-8 backend.</summary>
    /// <param name="utf8"></param>
    /// <param name="utf8length">number of utf8 codepoints to read from stream, or -1 for until end of stream</param>
    public UTF16Enumerator(IEnumerator<byte> utf8, int utf8length = -1)
    {
        srcType = EncodingType.UTF8;
        this.utf8 = utf8 ?? throw new ArgumentNullException(nameof(utf8));
        this.utf16 = null;
        this.utf32 = null;
        current = default(char);
        this.count = utf8length >= 0 ? utf8length : int.MaxValue;
        this.lowSurrogateQueue = -1;
    }

    /// <summary>Construct enumerator from UTF-16 backend.</summary>
    /// <param name="utf16"></param>
    /// <param name="utf16length">number of codepoints to read from stream, or -1 for until end of stream</param>
    public UTF16Enumerator(IEnumerator<char> utf16, int utf16length = -1)
    {
        srcType = EncodingType.UTF16;
        this.utf8 = null;
        this.utf16 = utf16 ?? throw new ArgumentNullException(nameof(utf16));
        this.utf32 = null;
        current = default(char);
        this.count = utf16length >= 0 ? utf16length : int.MaxValue;
        this.lowSurrogateQueue = -1;
    }

    /// <summary>Construct enumerator from UTF-32 backend.</summary>
    /// <param name="utf32"></param>
    /// <param name="utf32length">Number of characters to read from stream, or -1 for until end of stream</param>
    public UTF16Enumerator(IEnumerator<int> utf32, int utf32length = -1)
    {
        srcType = EncodingType.UTF32;
        this.utf8 = null;
        this.utf16 = null;
        this.utf32 = utf32 ?? throw new ArgumentNullException(nameof(utf32));
        current = default(char);
        this.count = utf32length >= 0 ? utf32length : int.MaxValue;
        this.lowSurrogateQueue = -1;
    }

    /// <summary></summary>
    public char Current => current;

    object IEnumerator.Current => throw new NotImplementedException();

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
        utf8?.Reset(); utf16?.Reset(); utf32?.Reset(); current = '\u0000';
    }

    private int ReadUTF8()
    {
        if (utf8 == null) return -1;
        // Read char
        if (--count < 0 || !utf8.MoveNext()) return -1;
        int c1 = utf8.Current;
        if ((c1 & 0x80) == 0x00) return c1;

        // Encoding error. We are reading a char at middle of encoding.. Return something.
        if ((c1 & 0x40) == 0x00) return c1 & 0x3f;

        // Read next char
        if (--count < 0 || !utf8.MoveNext()) return -1;
        int c2 = utf8.Current;

        // Two char encoding
        if ((c1 & 0xE0) == 0xC0) return (c1 & 0x1f) << 6 | (c2 & 0x3f);

        // Read next char
        if (--count < 0 || !utf8.MoveNext()) return -1;
        int c3 = utf8.Current;

        // Three char encoding
        if ((c1 & 0xF0) == 0xE0) return (c1 & 0x0f) << 12 | (c2 & 0x3f) << 6 | (c3 & 0x3f);

        // Read next char
        if (--count < 0 || !utf8.MoveNext()) return -1;
        int c4 = utf8.Current;

        // Four char encoding or Encoding error. return something.
        return (c1 & 0x07) << 18 | (c2 & 0x3f) << 12 | (c3 & 0x3f) << 6 | (c4 & 0x3f);
    }

    /// <summary></summary>
    public bool MoveNext()
    {
        // Passthrough
        if (srcType == EncodingType.UTF16)
        {
            // Get next char.
            if (utf16 == null || --count < 0 || !utf16.MoveNext()) return false;

            // Return
            current = utf16.Current;
            return true;
        }

        // Return low surrogate from queue.
        if (lowSurrogateQueue >= 0) { current = (char)lowSurrogateQueue; lowSurrogateQueue = -1; return true; }

        int code = -1;
        if (srcType == EncodingType.UTF8)
        {
            code = ReadUTF8();
            if (code < 0) return false;
        }
        else if (srcType == EncodingType.UTF32)
        {
            // Get next char.
            if (utf32 == null || --count < 0 || !utf32.MoveNext()) return false;
            // Read code.
            code = utf32.Current;
            // Invalid char.
            if (code < 0 || code > 0x10ffff) code = (int)((uint)code) % 0x110000;
        }
        else return false;

        // One char code or invalid code
        if (code >= 0xd800 && code <= 0xdfff) code = 0xfffd;
        if (code < 0xffff) { current = (char)code; lowSurrogateQueue = -1; return true; }

        // Write high surrogate            
        code -= 0x10000;
        current = (char)(code / 1024 + 0xd800);
        lowSurrogateQueue = (char)(code % 1024 + 0xdc00);
        //current = (int)((c1 - '\ud800') * 'Ѐ' + (c2 - '\udc00')) + 65536;
        //current = (char) (((code >> 10) & 0x03ff - 64) | 0xd800);
        //lowSurrogateQueue = (code & 0x03ff) | 0xdc00;
        return true;
    }

}

