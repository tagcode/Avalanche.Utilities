// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Collections;

/// <summary>
/// High-performance implementation to manage unicode strings.
/// 
/// Handles conversions between UTF-8, UTF-16 and UTF-32.
/// 
/// This class is a wrapper to container of characters: String, byte[], int[], char[], IEnumerable&lt;byte&gt; etc.
/// 
/// HashCode produces same value for same Unicode content, regardless of encoding.
/// HashCode value is cached if calculation is requested.
/// 
/// ToString returns either a new string, or a strong-referenced string if UnicodeString was constructed from a string.
/// 
/// Lazy initialization for: hashcode, utf8length, utf16length and utf32length.
/// 
/// IEnumerable and IEnumerators use stack allocations.
///   
/// Notes:
///   Unicode codepoints are between 0x0 - 10FFFF.
///   Codepoints E000 - FFFF are invalid.
///   UTF-16: High surrogate D800-DBFF, low surrogate DC00-DFFF, E000-FFFF Invalid. High(10bits) add D800, low(10bits) add DC00.
///   UTF-8: Max bytes 4. Bits on first surrogate describe the total length.
///   RFC3629, must throw exception on errors.
///   BOM: Byte-order-mark. UTF-16: U+FEFF (Right char); U+FFFE wrong char.
///   BOM: Byte-order-mark. UTF-8: EF BB BF (U+FEFF), aka.zero width space.
/// </summary>
public class UnicodeString :
    IEnumerable<byte>, IList<byte>,
    IEnumerable<char>, IList<char>,
    IEnumerable<int>, IList<int>,
    IEquatable<UnicodeString>, IComparable<UnicodeString>,
    IEquatable<string>, IComparable<string>,
    ICloneable
{
    /// <summary>consts for hashcoding.</summary>
    public const int FNVHashBasis = unchecked((int)2166136261);
    /// <summary>consts for hashcoding.</summary>
    public const int FNVHashPrime = 16777619;
    /// <summary>Emptry string</summary>
    public static UnicodeString Empty = new UnicodeString("");

    enum Flags
    {
        // First character is byte-order-mark.
        BOM = 1,
        BOMLittleEndian = 3,
        BOMBigEndian = 5,
        // Encoding error was detected in src data.
        EncodingError = 8,
        // Initial scan has been ran.
        Initialized = 16
    }

    Flags flags;
    int hashcode;
    int length = -1;
    int utf8length = -1;
    int utf16length = -1;

    /// <summary>Enumerable code.</summary>
    UnicodeEnumerable enumerable;

    /// <summary>Push IEnumerable to stack.</summary>
    public UnicodeEnumerable GetEnumerable() => enumerable;
    /// <summary>The number of bytes in UTF-8 format. </summary>
    public int UTF8Length { get { if (utf8length < 0) init(); return utf8length; } }
    /// <summary>The number of units in UTF-16 format.</summary>
    public int UTF16Length { get { if (utf16length < 0) init(); return utf16length; } }
    /// <summary>The number of unicode characters in the string.</summary>
    public int Length { get { if (length < 0) init(); return length; } }
    /// <summary>Set to true, if the first character is byte-order-mark U+FEFF</summary>
    public bool Bom { get { init(); return (flags & (Flags.BOM)) != 0; } }
    /// <summary>Set to true, if source stream contains encoding error.</summary>
    public bool EncodingError { get { init(); return (flags & Flags.EncodingError) != 0; } }
    /// <summary>Reference to string instance.</summary>
    WeakReference? strRef;

    /// <summary>Source object.</summary>
    public object Source => enumerable.GetSource();

    /// <summary>Create string from UTF-8 bytes.</summary>
    /// <param name="utf8_source">utf8 codepoints</param>
    /// <param name="utf8length">(optional) length of utf8 codepoints if known, -1 if not known</param>
    public UnicodeString(IEnumerable<byte> utf8_source, int utf8length = -1)
    {
        if (utf8length >= 0)
        {
            this.utf8length = utf8length;
        }
        else
        {
            if (utf8_source is IList<byte>) this.utf8length = ((IList<byte>)utf8_source).Count;
        }
        enumerable = new UnicodeEnumerable(utf8_source, this.utf8length);
    }

    /// <summary>Create string from UTF-16 characters, such as string.</summary>
    /// <param name="utf16_source">utf-16 codepoints</param>
    /// <param name="utf16length">number of utf16 codepoints or -1 if unknown</param>
    public UnicodeString(IEnumerable<char> utf16_source, int utf16length = -1)
    {
        if (utf16length >= 0)
        {
            this.utf16length = utf16length;
        }
        else
        {
            if (utf16_source is IList<char>) this.utf16length = ((IList<char>)utf16_source).Count;
            else if (utf16_source is string) this.utf16length = ((string)utf16_source).Length;
        }
        enumerable = new UnicodeEnumerable(utf16_source, this.utf16length);
    }

    /// <summary>Create string from UTF-32 ints.</summary>
    /// <param name="utf32_source">Utf-32 codepoints</param>
    /// <param name="utf32length">number of unicode codepoints to read from stream, or -1 if unknown</param>
    public UnicodeString(IEnumerable<int> utf32_source, int utf32length = -1)
    {
        if (utf32length >= 0)
        {
            length = utf32length;
        }
        else
        {
            if (utf32_source is IList<int>) length = ((IList<int>)utf32_source).Count;
        }
        enumerable = new UnicodeEnumerable(utf32_source, length);
    }

    /// <summary>Initialize iterates the string and calculates hashcode, utf8 length, utf16 and utf32 lengths.</summary>
    private void init()
    {
        Flags f = flags;
        // Init has been ran
        if ((f & Flags.Initialized) != 0) return;

        // Run init.
        int _hashcode = FNVHashBasis, _length = 0, _utf8length = 0, _utf16length = 0;
        int index = -1;
        foreach (int c in enumerable)
        {
            int code = c;

            // Encoding error
            if (code < 0 || code > 0x10ffff) { f |= Flags.EncodingError; code = (int)((uint)code) % 0x110000; }

            // Source index.
            index++;

            // Churn hashcode
            _hashcode = _hashcode * FNVHashPrime + code;

            // byte-order mark. 
            if (index == 0 && code == 0xfeff) flags |= Flags.BOMLittleEndian;
            else if (index == 0 && code == 0xfffe) flags |= Flags.BOMBigEndian;
            // Invalid char -> EncodingError
            else if (code >= 0xe000 && code <= 0xffff) { flags |= Flags.EncodingError; }

            // Add to UTF-8 length
            if (code >= 0 && code <= 0x7f) _utf8length++;
            else if (code >= 0x80 && code <= 0x7ff) _utf8length += 2;
            else if (code >= 0x800 && code <= 0xffff) _utf8length += 3;
            else if (code >= 0x10000 && code <= 0x10ffff) _utf8length += 4;

            // Add to UTF-16 length
            if (code >= 0x0000 && code <= 0xd7ff) _utf16length++;
            else if (code >= 0x10000 && code <= 0x10ffff) _utf16length += 2;
            else if (code >= 0xe000 && code <= 0xffff) _utf16length++; // invalid

            // Add to UTF-32 length
            _length++;
        }

        lock (this)
        {
            this.flags |= f | Flags.Initialized;
            this.hashcode = _hashcode;
            this.utf8length = this.utf8length < 0 ? _utf8length : Math.Min(this.utf8length, _utf8length);
            this.utf16length = this.utf16length < 0 ? _utf16length : Math.Min(this.utf16length, _utf16length);
            this.length = this.length < 0 ? _length : Math.Min(this.length, _length);
            if (enumerable.srcType == EncodingType.UTF8) enumerable.srcLength = this.utf8length;
            if (enumerable.srcType == EncodingType.UTF16) enumerable.srcLength = this.utf16length;
            if (enumerable.srcType == EncodingType.UTF32) enumerable.srcLength = this.length;
        }
    }

    /// <summary></summary>
    public int this[int index] { get => GetChar32At(index); set => throw new NotSupportedException(); }

    /// <summary>
    /// Get character at index. 
    /// 
    /// If constructed from UTF-8 or UTF-16 and has no surrogates, performance is O(1), if has surrogates O(n).
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public int GetChar32At(int index)
    {
        if (index < 0) throw new IndexOutOfRangeException();
        init();
        if (index >= length) throw new IndexOutOfRangeException();
        if (enumerable.srcType == EncodingType.UTF16)
        {
            if (length == utf16length)
            {
                // No surrogates.
                // Try to access by index.
                if (enumerable.utf16 is string) return ((string)enumerable.utf16)[index];
                if (enumerable.utf16 is IList<char>) return ((IList<char>)enumerable.utf16)[index];
            }
        }
        if (enumerable.srcType == EncodingType.UTF8)
        {
            if (length == utf8length)
            {
                // No surrogates. 
                // Try to access by index.
                if (enumerable.utf8 is IList<byte>) return ((IList<byte>)enumerable.utf8)[index];
            }
        }
        if (enumerable.srcType == EncodingType.UTF32)
        {
            // Try to access by index.
            if (enumerable.utf32 is IList<int>) return ((IList<int>)enumerable.utf32)[index];
        }

        // Iterate until index.
        using (UTF32Enumerator etor = enumerable.GetEnumerator())
        {
            for (int i = 0; i <= index; i++) if (!etor.MoveNext()) throw new IndexOutOfRangeException();
            return etor.Current;
        }
    }

    /// <summary>
    /// Get character unit at index. 
    /// 
    /// If constructed from UTF-8 or UTF-16 and has no surrogates, performance is O(1), if has surrogates O(n).
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public char GetChar16At(int index)
    {
        if (index < 0) throw new IndexOutOfRangeException();
        init();
        if (index >= utf16length) throw new IndexOutOfRangeException();
        if (enumerable.srcType == EncodingType.UTF16)
        {
            // No surrogates.
            // Try to access by index.
            if (enumerable.utf16 is string) return ((string)enumerable.utf16)[index];
            if (enumerable.utf16 is IList<char>) return ((IList<char>)enumerable.utf16)[index];
        }
        if (enumerable.srcType == EncodingType.UTF8)
        {
            if (length == utf8length)
            {
                // No surrogates. 
                // Try to access by index.
                if (enumerable.utf8 is IList<byte>) return (char)((IList<byte>)enumerable.utf8)[index];
            }
        }
        if (enumerable.srcType == EncodingType.UTF32)
        {
            if (utf16length == length)
            {
                // No surrogates
                // Try to access by index.
                if (enumerable.utf32 is IList<int>) return (char)((IList<int>)enumerable.utf32)[index];
            }
        }

        // Iterate until index.
        using (UTF16Enumerator etor = enumerable.GetEnumeratorUTF16())
        {
            for (int i = 0; i <= index; i++) if (!etor.MoveNext()) throw new IndexOutOfRangeException();
            return etor.Current;
        }
    }

    /// <summary>
    /// Get character unit at index. 
    /// 
    /// If constructed from UTF-8 or UTF-16 and has no surrogates, performance is O(1), if has surrogates O(n).
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public byte GetChar8At(int index)
    {
        if (index < 0) throw new IndexOutOfRangeException();
        init();
        if (index >= utf8length) throw new IndexOutOfRangeException();
        if (enumerable.srcType == EncodingType.UTF16)
        {
            if (utf8length == utf16length)
            {
                // No surrogates.
                // Try to access by index.
                if (enumerable.utf16 is string) return (byte)((string)enumerable.utf16)[index];
                if (enumerable.utf16 is IList<char>) return (byte)((IList<char>)enumerable.utf16)[index];
            }
        }
        if (enumerable.srcType == EncodingType.UTF8)
        {
            // Try to access by index.
            if (enumerable.utf8 is IList<byte>) return ((IList<byte>)enumerable.utf8)[index];
        }
        if (enumerable.srcType == EncodingType.UTF32)
        {
            if (utf8length == length)
            {
                // No surrogates.
                // Try to access by index.
                if (enumerable.utf32 is IList<int>) return (byte)((IList<int>)enumerable.utf32)[index];
            }
        }

        // Iterate until index.
        using (UTF8Enumerator etor = enumerable.GetEnumeratorUTF8())
        {
            for (int i = 0; i <= index; i++) if (!etor.MoveNext()) throw new IndexOutOfRangeException();
            return etor.Current;
        }
    }

    /// <summary>Create a string, or return a previously created string that is held with a weak reference.</summary>
    public override string ToString()
    {
        // Return original string.
        if (enumerable.srcType == EncodingType.UTF16 && enumerable.utf16 is string) return (string)enumerable.utf16;

        // Return previous reference
        WeakReference? st = strRef;
        string? str = st?.Target as string;
        if (str != null) return str;

        // Create char[]
        char[] chars;
        if (enumerable.srcType == EncodingType.UTF16 && enumerable.utf16 is char[])
        {
            chars = (char[])enumerable.utf16;
        }
        else
        {
            int len = UTF16Length;
            chars = new char[len];
            using (UTF16Enumerator etor = enumerable.GetEnumeratorUTF16())
            {
                for (int i = 0; i < len; i++)
                {
                    if (!etor.MoveNext()) throw new Exception("Unexpected end of stream");
                    chars[i] = etor.Current;
                }
            }
        }

        // Create string
        str = new string(chars);
        strRef = new WeakReference(str);
        return str;
    }

    /// <summary>
    /// Hashcode. Contract: hash = FNVHashBasis; foreach(ch) hashcode = hashcode * FNVHashPrime + ch.
    /// 
    /// This hashcode is calculated differently than string hashcode as String hashcode does not have a contract.
    /// </summary>
    public override int GetHashCode() { init(); return hashcode; }

    IEnumerator IEnumerable.GetEnumerator() => enumerable.GetEnumerator();
    IEnumerator<byte> IEnumerable<byte>.GetEnumerator() => enumerable.GetEnumeratorUTF8();
    IEnumerator<char> IEnumerable<char>.GetEnumerator() => enumerable.GetEnumeratorUTF16();
    IEnumerator<int> IEnumerable<int>.GetEnumerator() => enumerable.GetEnumerator();

    /// <summary>Push UTF-8 enumerator into stack.</summary>
    public UTF8Enumerator GetEnumeratorUTF8() => enumerable.GetEnumeratorUTF8();
    /// <summary>Push UTF-16 enumerator into stack.</summary>
    public UTF16Enumerator GetEnumeratorUTF16() => enumerable.GetEnumeratorUTF16();
    /// <summary>Push UTF-32 enumerator into stack.</summary>
    public UTF32Enumerator GetEnumerator() => enumerable.GetEnumerator();

    /// <summary>Convert to byte array.</summary>
    /// <returns>byte array</returns>
    public byte[] ToUtf8Array()
    {
        int len = UTF8Length;
        byte[] result = new byte[len];
        if (enumerable.srcType == EncodingType.UTF8 && enumerable.utf8 is byte[] srcArray)
            Array.Copy(srcArray, result, len);
        else
        {
            int ix = 0;
            for (UTF8Enumerator enumr = enumerable.GetEnumeratorUTF8(); enumr.MoveNext();)
                result[ix++] = enumr.Current;
        }
        return result;
    }

    /// <summary>Convert to utf 16 array.</summary>
    /// <returns>char array</returns>
    public char[] ToUtf16Array()
    {
        int len = UTF16Length;
        char[] result = new char[len];
        int ix = 0;
        for (UTF16Enumerator enumr = enumerable.GetEnumeratorUTF16(); enumr.MoveNext();)
            result[ix++] = enumr.Current;
        return result;
    }

    /// <summary>Convert to utf 32 array.</summary>
    /// <returns>char array</returns>
    public int[] ToArray()
    {
        int len = Length;
        int[] result = new int[len];
        int ix = 0;
        for (UTF32Enumerator enumr = enumerable.GetEnumerator(); enumr.MoveNext();)
            result[ix++] = enumr.Current;
        return result;
    }

    bool IEquatable<string>.Equals(string? other)
    {
        if (other == null) return false;
        if (other.Length != UTF16Length) return false;
        if (enumerable.srcType == EncodingType.UTF16 && Object.ReferenceEquals(enumerable.utf16, other)) return true;
        using (UTF16Enumerator etor1 = enumerable.GetEnumeratorUTF16())
        using (CharEnumerator etor2 = other.GetEnumerator())
            return CompareServices.CompareEnumerators<UTF16Enumerator, CharEnumerator, char>(etor1, etor2) == 0;
    }

    /// <summary></summary>
    public override bool Equals(object? other)
    {
        if (other == null) return false;
        if (other is string) ((IEquatable<string>)this).Equals((string)other);
        if (other is UnicodeString) Equals((UnicodeString)other);
        return false;
    }

    /// <summary></summary>
    public bool Equals(UnicodeString? other)
    {
        if (other == null) return false;
        if (Object.ReferenceEquals(this, other)) return true;
        if (other.GetHashCode() != GetHashCode()) return false;
        EncodingType t1 = enumerable.srcType, t2 = other.enumerable.srcType;
        if (t1 == EncodingType.UTF8 && t2 == EncodingType.UTF8)
        {
            if (UTF8Length != other.UTF8Length) return false;
            using (UTF8Enumerator etor1 = enumerable.GetEnumeratorUTF8(), etor2 = other.GetEnumeratorUTF8())
                return CompareServices.CompareEnumerators<UTF8Enumerator, byte>(etor1, etor2) == 0;
        }
        else if (t1 == EncodingType.UTF16 && t2 == EncodingType.UTF16)
        {
            if (UTF16Length != other.UTF16Length) return false;
            using (UTF16Enumerator etor1 = enumerable.GetEnumeratorUTF16(), etor2 = other.GetEnumeratorUTF16())
                return CompareServices.CompareEnumerators<UTF16Enumerator, char>(etor1, etor2) == 0;
        }
        else
        {
            if (Length != other.Length) return false;
            using (UTF32Enumerator etor1 = enumerable.GetEnumerator(), etor2 = other.GetEnumerator())
                return CompareServices.CompareEnumerators<UTF32Enumerator, int>(etor1, etor2) == 0;
        }
    }

    int IComparable<string>.CompareTo(string? other)
    {
        if (other == null) return -1;
        if (enumerable.srcType == EncodingType.UTF16 && Object.ReferenceEquals(enumerable.utf16, other)) return 0;
        using (UTF16Enumerator etor1 = enumerable.GetEnumeratorUTF16())
        using (CharEnumerator etor2 = other.GetEnumerator())
            return CompareServices.CompareEnumerators<UTF16Enumerator, CharEnumerator, char>(etor1, etor2);
    }

    /// <summary></summary>
    public int CompareTo(UnicodeString? other)
    {
        if (other == null) return -1;
        if (Object.ReferenceEquals(this, other)) return 0;
        using (UTF32Enumerator etor1 = enumerable.GetEnumerator(), etor2 = other.GetEnumerator())
            return CompareServices.CompareEnumerators<UTF32Enumerator, int>(etor1, etor2);
    }

    /// <summary></summary>
    public object Clone()
    {
        // Construct clone from UTF-16 string as this is native to C#
        //
        string str = ToString();
        UnicodeString result = new UnicodeString(str);

        // If this has been initialized, copy initialized info.
        if ((flags & Flags.Initialized) != 0)
        {
            result.hashcode = this.hashcode;
            result.utf8length = this.utf8length;
            result.utf16length = this.utf16length;
            result.length = this.length;
        }

        // Return clone.
        return result;
    }

    // IList<int> //
    int ICollection<int>.Count => Length;
    bool ICollection<int>.IsReadOnly => true;
    //int IList<int>.this[int index] { get => this[index]; set => throw new NotImplementedException(); }
    void IList<int>.Insert(int index, int item) => throw new NotSupportedException();
    void IList<int>.RemoveAt(int index) => throw new NotSupportedException();
    void ICollection<int>.Add(int item) => throw new NotSupportedException();
    void ICollection<int>.Clear() => throw new NotSupportedException();
    bool ICollection<int>.Remove(int item) => throw new NotSupportedException();
    int IList<int>.IndexOf(int item)
    {
        int index = 0;
        for (UTF32Enumerator etor = GetEnumerator(); etor.MoveNext(); index++)
            if (etor.Current == item) return index;
        return -1;
    }
    bool ICollection<int>.Contains(int item)
    {
        for (UTF32Enumerator etor = GetEnumerator(); etor.MoveNext();)
            if (etor.Current == item) return true;
        return false;
    }
    void ICollection<int>.CopyTo(int[] array, int localIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        init();
        if (localIndex < 0 || localIndex > length) throw new IndexOutOfRangeException();
        int count = length - localIndex;
        if (array.Length < count) throw new ArgumentException("Array is too short.");
        using (UTF32Enumerator etor = GetEnumerator())
        {
            for (int i = 0; i < localIndex; i++)
                if (!etor.MoveNext()) return;
            for (int i = 0; i < count; i++)
            {
                if (!etor.MoveNext()) return;
                array[i] = etor.Current;
            }
        }
    }

    // IList<char> //
    char IList<char>.this[int index] { get => GetChar16At(index); set => throw new NotSupportedException(); }
    void IList<char>.Insert(int index, char item) => throw new NotSupportedException();
    void IList<char>.RemoveAt(int index) => throw new NotSupportedException();
    void ICollection<char>.Add(char item) => throw new NotSupportedException();
    void ICollection<char>.Clear() => throw new NotSupportedException();
    bool ICollection<char>.Remove(char item) => throw new NotSupportedException();
    int ICollection<char>.Count => UTF16Length;
    bool ICollection<char>.IsReadOnly => true;
    int IList<char>.IndexOf(char item)
    {
        int index = 0;
        for (UTF16Enumerator etor = GetEnumeratorUTF16(); etor.MoveNext(); index++)
            if (etor.Current == item) return index;
        return -1;
    }
    bool ICollection<char>.Contains(char item)
    {
        for (UTF16Enumerator etor = GetEnumeratorUTF16(); etor.MoveNext();)
            if (etor.Current == item) return true;
        return false;
    }
    void ICollection<char>.CopyTo(char[] array, int localIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        init();
        if (localIndex < 0 || localIndex > utf16length) throw new IndexOutOfRangeException();
        int count = utf16length - localIndex;
        if (array.Length < count) throw new ArgumentException("Array is too short.");
        using (UTF16Enumerator etor = GetEnumeratorUTF16())
        {
            for (int i = 0; i < localIndex; i++)
                if (!etor.MoveNext()) return;
            for (int i = 0; i < count; i++)
            {
                if (!etor.MoveNext()) return;
                array[i] = etor.Current;
            }
        }
    }

    // IList<byte> //
    byte IList<byte>.this[int index] { get => GetChar8At(index); set => throw new NotSupportedException(); }
    void IList<byte>.Insert(int index, byte item) => throw new NotSupportedException();
    void IList<byte>.RemoveAt(int index) => throw new NotSupportedException();
    void ICollection<byte>.Add(byte item) => throw new NotSupportedException();
    void ICollection<byte>.Clear() => throw new NotSupportedException();
    bool ICollection<byte>.Remove(byte item) => throw new NotSupportedException();
    int ICollection<byte>.Count => UTF8Length;
    bool ICollection<byte>.IsReadOnly => true;
    int IList<byte>.IndexOf(byte item)
    {
        int index = 0;
        for (UTF8Enumerator etor = GetEnumeratorUTF8(); etor.MoveNext(); index++)
            if (etor.Current == item) return index;
        return -1;
    }
    bool ICollection<byte>.Contains(byte item)
    {
        for (UTF8Enumerator etor = GetEnumeratorUTF8(); etor.MoveNext();)
            if (etor.Current == item) return true;
        return false;
    }
    void ICollection<byte>.CopyTo(byte[] array, int localIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        init();
        if (localIndex < 0 || localIndex > length) throw new IndexOutOfRangeException();
        int count = utf8length - localIndex;
        if (array.Length < count) throw new ArgumentException("Array is too short.");
        using (UTF8Enumerator etor = GetEnumeratorUTF8())
        {
            for (int i = 0; i < localIndex; i++)
                if (!etor.MoveNext()) return;
            for (int i = 0; i < count; i++)
            {
                if (!etor.MoveNext()) return;
                array[i] = etor.Current;
            }
        }
    }

    /// <summary></summary>
    public bool StartsWith(UnicodeString str, UnicodeString other)
    {
        using (var enumr1 = ((IEnumerable<int>)str).GetEnumerator())
        using (var enumr2 = ((IEnumerable<int>)other).GetEnumerator())
        {
            while (enumr2.MoveNext())
            {
                if (!enumr1.MoveNext()) return false;
                if (enumr1.Current != enumr2.Current) return false;
            }
            return true;
        }
    }

}

/// <summary></summary>
public enum EncodingType
{
    /// <summary></summary>
    UTF8,
    /// <summary></summary>
    UTF16,
    /// <summary></summary>
    UTF32
}


