using System;
using System.Collections.Generic;
using Avalanche.Utilities;

public class unicode
{
    // Rename to "Main", or run from Main.
    public static void Run(string[] args)
    {
        // Construct from String, IEnumerable<byte>, IEnumerable<char>, IEnumerable<int>
        UnicodeString str = new UnicodeString("European castle \uD83C\uDFF0");
        UnicodeString str8 = new UnicodeString(new byte[] { 69, 117, 114, 111, 112, 101, 97, 110, 32, 99, 97, 115, 116, 108, 101, 32, 240, 159, 143, 176 });
        UnicodeString str16 = new UnicodeString(new char[] { 'E', 'u', 'r', 'o', 'p', 'e', 'a', 'n', ' ', 'c', 'a', 's', 't', 'l', 'e', ' ', '\uD83C', '\uDFF0' });
        UnicodeString str32 = new UnicodeString(new int[] { 69, 117, 114, 111, 112, 101, 97, 110, 32, 99, 97, 115, 116, 108, 101, 32, 127984 });

        // Convert to utf 8/16/32 codepoints
        Console.WriteLine($"Utf8 [ {String.Join(", ", str.ToUtf8Array())} ]");
        Console.WriteLine($"Utf16 [ {String.Join(", ", str.ToUtf16Array())} ]");
        Console.WriteLine($"Utf32 [ {String.Join(", ", str.ToArray())} ]");

        // Encode to utf-8, utf-16, utf-32 codepoints
        foreach (byte codepoint in str) { }
        foreach (char codepoint in str) { }
        foreach (int codepoint in str) { }

        // Get stack allocated enumerator encoders
        for (UTF8Enumerator enumr = str8.GetEnumeratorUTF8(); enumr.MoveNext();) { }
        for (UTF16Enumerator enumr = str8.GetEnumeratorUTF16(); enumr.MoveNext();) { }
        for (UTF32Enumerator enumr = str8.GetEnumerator(); enumr.MoveNext();) { }

        // Length to different encodings can be calculated
        int utf8length = str8.UTF8Length;
        int utf16length = str8.UTF16Length;
        int utf32length = str8.Length;

        // Compare for unicode equality and hashcode
        var equals1 = str8.Equals(str32);
        var equals2 = str16.Equals(str8);

        var hashcode_equal1 = str8.GetHashCode() == str16.GetHashCode();
        var hashcode_equal2 = str16.GetHashCode() == str8.GetHashCode();

        Console.WriteLine($"Src={str8.Source.GetType().Name},  Str=\"{str8}\", Length={str8.Length}, Hashcode={str8.GetHashCode()}");
        Console.WriteLine($"Src={str16.Source.GetType().Name},  Str=\"{str16}\", Length={str16.Length}, Hashcode={str16.GetHashCode()}");
        Console.WriteLine($"Src={str32.Source.GetType().Name}, Str=\"{str32}\", Length={str32.Length}, Hashcode={str32.GetHashCode()}");

        // Assign UTF8 to ILists of various encodings
        IList<byte> utf8_to_utf8 = str8;
        IList<char> utf8_to_utf16 = str8;
        IList<int> utf8_to_utf32 = str8;

        // Assign UTF16 to ILists of various encodings
        IList<byte> utf16_to_utf8 = str16;
        IList<char> utf16_to_utf16 = str16;
        IList<int> utf16_to_utf32 = str16;

        // Assign UTF32 to ILists of various encodings
        IList<byte> utf32_to_utf8 = str32;
        IList<char> utf32_to_utf16 = str32;
        IList<int> utf32_to_utf32 = str32;
    }
}
