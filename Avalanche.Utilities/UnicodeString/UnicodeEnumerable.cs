// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Collections;

/// <summary></summary>
public struct UnicodeEnumerable : IEnumerable<int>, IEnumerable<char>, IEnumerable<byte>
{
    /// <summary></summary>
    public readonly EncodingType srcType;
    /// <summary></summary>
    public int srcLength;
    /// <summary></summary>
    public readonly IEnumerable<byte>? utf8;
    /// <summary></summary>
    public readonly IEnumerable<char>? utf16;
    /// <summary></summary>
    public readonly IEnumerable<int>? utf32;

    /// <summary>Get source object.</summary>
    /// <returns></returns>
    public object GetSource() =>
        srcType == EncodingType.UTF8 ? (object)utf8! :
        srcType == EncodingType.UTF16 ? (object)utf16! :
        (object)utf32!;

    /// <summary>Get UTF-8 enumerator into stack.</summary>
    public UTF8Enumerator GetEnumeratorUTF8() => srcType == EncodingType.UTF8 ? new UTF8Enumerator(utf8!.GetEnumerator(), srcLength) : srcType == EncodingType.UTF16 ? new UTF8Enumerator(utf16!.GetEnumerator(), srcLength) : new UTF8Enumerator(utf32!.GetEnumerator(), srcLength);
    /// <summary>Get UTF-16 enumerator into stack.</summary>
    public UTF16Enumerator GetEnumeratorUTF16() => srcType == EncodingType.UTF8 ? new UTF16Enumerator(utf8!.GetEnumerator(), srcLength) : srcType == EncodingType.UTF16 ? new UTF16Enumerator(utf16!.GetEnumerator(), srcLength) : new UTF16Enumerator(utf32!.GetEnumerator(), srcLength);
    /// <summary>Get UTF-32 enumerator into stack.</summary>
    public UTF32Enumerator GetEnumerator() => srcType == EncodingType.UTF8 ? new UTF32Enumerator(utf8!.GetEnumerator(), srcLength) : srcType == EncodingType.UTF16 ? new UTF32Enumerator(utf16!.GetEnumerator(), srcLength) : new UTF32Enumerator(utf32!.GetEnumerator(), srcLength);
    /// <summary>Get enumerator into heap.</summary>
    IEnumerator IEnumerable.GetEnumerator() => srcType == EncodingType.UTF8 ? new UTF32Enumerator(utf8!.GetEnumerator(), srcLength) : srcType == EncodingType.UTF16 ? new UTF32Enumerator(utf16!.GetEnumerator(), srcLength) : new UTF32Enumerator(utf32!.GetEnumerator(), srcLength);
    /// <summary>Get UTF-32 enumerator into heap.</summary>
    IEnumerator<int> IEnumerable<int>.GetEnumerator() => srcType == EncodingType.UTF8 ? new UTF32Enumerator(utf8!.GetEnumerator(), srcLength) : srcType == EncodingType.UTF16 ? new UTF32Enumerator(utf16!.GetEnumerator(), srcLength) : new UTF32Enumerator(utf32!.GetEnumerator(), srcLength);
    /// <summary>Get UTF-16 enumerator into heap.</summary>
    IEnumerator<char> IEnumerable<char>.GetEnumerator() => srcType == EncodingType.UTF8 ? new UTF16Enumerator(utf8!.GetEnumerator(), srcLength) : srcType == EncodingType.UTF16 ? new UTF16Enumerator(utf16!.GetEnumerator(), srcLength) : new UTF16Enumerator(utf32!.GetEnumerator(), srcLength);
    /// <summary>Get UTF-8 enumerator into heap.</summary>
    IEnumerator<byte> IEnumerable<byte>.GetEnumerator() => srcType == EncodingType.UTF8 ? new UTF8Enumerator(utf8!.GetEnumerator(), srcLength) : srcType == EncodingType.UTF16 ? new UTF8Enumerator(utf16!.GetEnumerator(), srcLength) : new UTF8Enumerator(utf32!.GetEnumerator(), srcLength);

    /// <summary>Construct enumerable from UTF-8 backend.</summary>
    /// <param name="utf8"></param>
    /// <param name="utf8length">optional source length</param>
    public UnicodeEnumerable(IEnumerable<byte> utf8, int utf8length = -1)
    {
        srcType = EncodingType.UTF8;
        this.utf8 = utf8 ?? throw new ArgumentNullException(nameof(utf8));
        this.utf16 = null;
        this.utf32 = null;
        this.srcLength = utf8length;
    }

    /// <summary>Construct enumerable from UTF-16 backend.</summary>
    /// <param name="utf16"></param>
    /// <param name="utf16length">optional source length</param>
    public UnicodeEnumerable(IEnumerable<char> utf16, int utf16length = -1)
    {
        srcType = EncodingType.UTF16;
        this.utf8 = null;
        this.utf16 = utf16 ?? throw new ArgumentNullException(nameof(utf16));
        this.utf32 = null;
        this.srcLength = utf16length;
    }

    /// <summary>Construct enumerable from UTF-32 backend.</summary>
    /// <param name="utf32"></param>
    /// <param name="utf32length">optional source length</param>
    public UnicodeEnumerable(IEnumerable<int> utf32, int utf32length = -1)
    {
        srcType = EncodingType.UTF32;
        this.utf8 = null;
        this.utf16 = null;
        this.utf32 = utf32 ?? throw new ArgumentNullException(nameof(utf32));
        this.srcLength = utf32length;
    }
}

