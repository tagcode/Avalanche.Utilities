// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Collections;
using System.Globalization;
using System.Numerics;

/// <summary>Utilities for converting value types and <![CDATA[byte[]]]> to and from hexadecimal <see cref="string"/>.</summary>
public static class Hex
{
    /// <summary>Print <paramref name="in"/> as hex string.</summary>
    public static String Print(byte[] @in) => Print(@in.AsSpan());
    /// <summary>Print <paramref name="in"/> as hex string.</summary>
    public static String Print(ReadOnlyMemory<byte> @in) => Print(@in.Span);
    /// <summary>Print <paramref name="in"/> as hex string.</summary>
    public static String Print(ReadOnlySpan<byte> @in)
    {
        // Allocate result
        char[] result = new char[@in.Length << 1];
        // Result cursor index
        int ix = 0;
        // Iterate in bytes
        for (int i = 0; i < @in.Length; i++)
        {
            byte b = @in[i], lo = (byte)(b & 0xf), hi = (byte)(b >> 4);
            result[ix++] = (char)(hi < 10 ? 48 + hi : 55 + hi);
            result[ix++] = (char)(lo < 10 ? 48 + lo : 55 + lo);
        }
        // Formulate string
        string resultString = new string(result);
        // Return
        return resultString;
    }

    /// <summary>Print <paramref name="value"/> as hex string.</summary>
    public static String Print(decimal value) => value.ToString("X", CultureInfo.InvariantCulture);
    /// <summary>Print <paramref name="value"/> as hex string.</summary>
    public static String Print(ulong value) => value.ToString("X", CultureInfo.InvariantCulture);
    /// <summary>Print <paramref name="value"/> as hex string.</summary>
    public static String Print(uint value) => value.ToString("X", CultureInfo.InvariantCulture);
    /// <summary>Print <paramref name="value"/> as hex string.</summary>
    public static String Print(BigInteger value) => value.ToString("X", CultureInfo.InvariantCulture);

    /// <summary>Parse <paramref name="str"/> to data.</summary>
    public static BigInteger ToBigInteger(String str) => BigInteger.Parse(str);

    /// <summary>Parse <paramref name="str"/> to data. <paramref name="str"/> must not start with '0x'.</summary>
    /// <param name="str">string of hexadecimal characters [0-9a-fA-F]</param>
    public static byte[] Parse(String str) => Parse(str.AsSpan());

    /// <summary>Parse <paramref name="str"/> to data. <paramref name="str"/> must not start with '0x'.</summary>
    /// <param name="str">string of hexadecimal characters [0-9a-fA-F]</param>
    public static byte[] Parse(Memory<char> str) => Parse(str.Span);

    /// <summary>Parse <paramref name="str"/> to data. <paramref name="str"/> must not start with '0x'.</summary>
    /// <param name="str">string of hexadecimal characters [0-9a-fA-F]</param>
    public static byte[] Parse(ReadOnlyMemory<char> str) => Parse(str.Span);

    /// <summary>Parse <paramref name="str"/> to data. <paramref name="str"/> must not start with '0x'.</summary>
    /// <param name="str">string of hexadecimal characters [0-9a-fA-F]</param>
    public static byte[] Parse(ReadOnlySpan<char> str)
    {
        // Create enumerator
        HexSpanEnumerator stream = new HexSpanEnumerator(str);

        // Hex char count
        int count = 0;
        // Calculate hex character count
        for (int i = 0; stream.MoveNext(); i++)
        {
            // Got non-hex value.
            if (stream.Current < 0 || stream.Error) throw new Exception(str.ToString());
            // Total character count.
            count++;
        }
        // Restart
        stream.Reset();

        // Allocate
        byte[] value = new byte[(count + 1) / 2];
        // Byte array index
        int ix = 0;
        // Toggle for low/high 4bits. 
        bool low = (count & 1) == 1;
        // Re-iterate
        while (count > 0 && stream.MoveNext())
        {
            int hex = stream.Current;
            if (low)
            {
                value[ix] |= (byte)hex;
                ix++;
            }
            else
            {
                value[ix] |= (byte)(hex << 4);
            }
            low = !low;
            count--;
        }
        // Return value
        return value;
    }

    /// <summary>Try parse <paramref name="str"/> to <paramref name="value"/>. <paramref name="str"/> must not start with '0x'.</summary>
    /// <param name="str">string of hexadecimal characters [0-9a-fA-F]</param>
    /// <param name="value">result value</param>
    /// <returns>false if contains non-hex characters, or if number of meaningful hexnumbers is over 24.</returns>
    /// <exception cref="System.FormatException"></exception>
    public static bool TryParseToBytes(String str, out byte[]? value)
    {
        HexEnumerator stream = new HexEnumerator(str.GetEnumerator());

        // Calculate hex character count
        int count = 0;
        for (int i = 0; stream.MoveNext(); i++)
        {
            // Got non-hex value.
            if (stream.Current < 0 || stream.Error) { value = default; return false; }
            // Total character count.
            count++;
        }
        // Restart
        stream.Reset();

        // Allocate
        value = new byte[(count + 1) / 2];

        // Re-Read
        int ix = 0;
        bool low = (count & 1) == 1;
        while (count > 0 && stream.MoveNext())
        {
            int hex = stream.Current;
            if (low)
            {
                value[ix] |= (byte)hex;
                ix++;
            }
            else
            {
                value[ix] |= (byte)(hex << 4);
            }
            low = !low;
            count--;
        }

        return true;
    }

    /// <summary>
    /// Parse a string to uint. String must not start with '0x'.
    /// UInt can accomodate only 8 meaningful hex-decimals.
    /// </summary>
    /// <param name="str">string of hexadecimal characters [0-9a-fA-F]</param>
    /// <returns>value</returns>
    /// <exception cref="System.FormatException"></exception>        
    public static uint ToUInt(String str)
    {
        HexEnumerator stream = new HexEnumerator(str.GetEnumerator());

        // Calculate hex character count
        int count = 0;
        // Calculate meaningful number characters
        int meaningfulDigits = 0;
        for (int i = 0; i < 999 && stream.MoveNext(); i++)
        {
            // Got non-hex value.
            if (stream.Current < 0) throw new System.FormatException("Unexpected character in hex stream.");
            // Start calculating after first non-zero.
            if (meaningfulDigits > 0 || stream.Current != 0) meaningfulDigits++;
            // Total character count.
            count++;
        }

        // Found non-zero value.
        if (stream.Error) if (stream.Current < 0) throw new System.FormatException("Unexpected character in hex stream.");

        // Restart
        stream.Reset();

        // Too many characters
        if (meaningfulDigits >= 9) throw new System.FormatException("Hex value too large to fit into decimal.");

        uint value = 0U;
        while (count > 8 && stream.MoveNext())
        {
            // long can fit 64bit value, any non-zero over 64bits cannot be parsed.
            if (stream.Current != 0) throw new System.FormatException("Hex value too large to fit into decimal.");
            count--;
        }
        while (count > 0 && stream.MoveNext())
        {
            uint hex = (uint)stream.Current;
            value = value << 4 | hex;
            count--;
        }

        return value;
    }

    /// <summary>
    /// Parse a string to long. String must not start with '0x'.
    /// UInt can accomodate only 8 meaningful hex-decimals.
    /// </summary>
    /// <param name="str">string of hexadecimal characters [0-9a-fA-F]</param>
    /// <param name="value">result value</param>
    /// <returns>false if contains non-hex characters, or if number of meaningful hexnumbers is over 24.</returns>
    /// <exception cref="System.FormatException"></exception>
    public static bool TryParseToUInt(String str, out uint value)
    {
        HexEnumerator stream = new HexEnumerator(str.GetEnumerator());

        // Calculate hex character count
        int count = 0;
        // Calculate meaningful number characters
        int meaningfulDigits = 0;
        for (int i = 0; i < 999 && stream.MoveNext(); i++)
        {
            // Got non-hex value.
            if (stream.Current < 0) { value = default(uint); return false; }
            // Start calculating after first non-zero.
            if (meaningfulDigits > 0 || stream.Current != 0) meaningfulDigits++;
            // Total character count.
            count++;
        }

        // Found non-zero value.
        if (stream.Error) { value = default(uint); return false; }

        // Restart
        stream.Reset();

        // Too many characters
        if (meaningfulDigits >= 9) { value = default(uint); return false; }

        value = 0U;
        while (count > 8 && stream.MoveNext())
        {
            // long can fit 64bit value, any non-zero over 64bits cannot be parsed.
            if (stream.Current != 0) { value = default(uint); return false; }
            count--;
        }
        while (count > 0 && stream.MoveNext())
        {
            uint hex = (uint)stream.Current;
            value = value << 4 | hex;
            count--;
        }

        return true;
    }

    /// <summary>
    /// Parse a string to long. String must not start with '0x'.
    /// Long can accomodate only 16 meaningful hex-decimals.
    /// </summary>
    /// <param name="str">string of hexadecimal characters [0-9a-fA-F]</param>
    /// <returns>value</returns>
    /// <exception cref="System.FormatException"></exception>
    public static ulong ToULong(String str)
    {
        HexEnumerator stream = new HexEnumerator(str.GetEnumerator());

        // Calculate hex character count
        int count = 0;
        // Calculate meaningful number characters
        int meaningfulDigits = 0;
        for (int i = 0; i < 999 && stream.MoveNext(); i++)
        {
            // Got non-hex value.
            if (stream.Current < 0) throw new System.FormatException("Unexpected character in hex stream.");
            // Start calculating after first non-zero.
            if (meaningfulDigits > 0 || stream.Current != 0) meaningfulDigits++;
            // Total character count.
            count++;
        }

        // Found non-zero value.
        if (stream.Error) if (stream.Current < 0) throw new System.FormatException("Unexpected character in hex stream.");

        // Restart
        stream.Reset();

        // Too many characters
        if (meaningfulDigits >= 17) throw new System.FormatException("Hex value too large to fit into decimal.");

        ulong value = 0UL;
        while (count > 16 && stream.MoveNext())
        {
            // long can fit 64bit value, any non-zero over 64bits cannot be parsed.
            if (stream.Current != 0) throw new System.FormatException("Hex value too large to fit into decimal.");
            count--;
        }
        while (count > 0 && stream.MoveNext())
        {
            ulong hex = (ulong)stream.Current;
            value = value << 4 | hex;
            count--;
        }

        return value;
    }

    /// <summary>
    /// Parse a string to long. String must not start with '0x'.
    /// Long can accomodate only 16 meaningful hex-decimals.
    /// </summary>
    /// <param name="str">string of hexadecimal characters [0-9a-fA-F]</param>
    /// <param name="value">result value</param>
    /// <returns>false if contains non-hex characters, or if number of meaningful hexnumbers is over 24.</returns>
    public static bool TryParseToULong(String str, out ulong value)
    {
        HexEnumerator stream = new HexEnumerator(str.GetEnumerator());

        // Calculate hex character count
        int count = 0;
        // Calculate meaningful number characters
        int meaningfulDigits = 0;
        for (int i = 0; i < 999 && stream.MoveNext(); i++)
        {
            // Got non-hex value.
            if (stream.Current < 0) { value = default(ulong); return false; }
            // Start calculating after first non-zero.
            if (meaningfulDigits > 0 || stream.Current != 0) meaningfulDigits++;
            // Total character count.
            count++;
        }

        // Found non-zero value.
        if (stream.Error) { value = default(ulong); return false; }

        // Restart
        stream.Reset();

        // Too many characters
        if (meaningfulDigits >= 17) { value = default(ulong); return false; }

        value = 0UL;
        while (count > 16 && stream.MoveNext())
        {
            // long can fit 64bit value, any non-zero over 64bits cannot be parsed.
            if (stream.Current != 0) { value = default(ulong); return false; }
            count--;
        }
        while (count > 0 && stream.MoveNext())
        {
            ulong hex = (ulong)stream.Current;
            value = value << 4 | hex;
            count--;
        }

        return true;
    }

    /// <summary>
    /// Parse string to decimal number.
    /// String must not start with '0x'.
    /// Decimal can accomodate only 24 meaningful hexnumbers. 
    /// </summary>
    /// <param name="str">string of hexadecimal characters [0-9a-fA-F]</param>
    /// <returns>decimal</returns>
    /// <exception cref="System.FormatException"></exception>
    public static decimal ToDecimal(String str)
    {
        HexEnumerator stream = new HexEnumerator(str.GetEnumerator());

        // Calculate hex character count
        int count = 0;
        // Calculate meaningful number characters
        int meaningfulDigits = 0;
        for (int i = 0; i < 999 && stream.MoveNext(); i++)
        {
            // Got non-hex value.
            if (stream.Current < 0) throw new System.FormatException("Unexpected character in hex stream.");
            // Start calculating after first non-zero.
            if (meaningfulDigits > 0 || stream.Current != 0) meaningfulDigits++;
            // Total character count.
            count++;
        }

        // Found non-zero value.
        if (stream.Error) if (stream.Current < 0) throw new System.FormatException("Unexpected character in hex stream.");

        // Restart
        stream.Reset();

        // Too many characters
        if (meaningfulDigits >= 25) throw new System.FormatException("Hex value too large to fit into decimal.");

        int lo = 0, mid = 0, hi = 0;
        while (count > 24 && stream.MoveNext())
        {
            // decimal can fit 96bit value, any non-zero over 96bits cannot be parsed.
            if (stream.Current != 0) throw new System.FormatException("Hex value too large to fit into decimal.");
            count--;
        }
        while (count > 16 && stream.MoveNext())
        {
            hi = (hi << 4) | stream.Current;
            count--;
        }
        while (count > 8 && stream.MoveNext())
        {
            mid = (mid << 4) | stream.Current;
            count--;
        }
        while (count > 0 && stream.MoveNext())
        {
            lo = (lo << 4) | stream.Current;
            count--;
        }

        return new decimal(lo, mid, hi, false, 0);
    }

    /// <summary>
    /// Try to parse a string to decimal number.
    /// String must not start with '0x'.
    /// Decimal can accomodate only 24 meaningful hexnumbers. 
    /// Although, the string can precede any number of zeroes, e.g. "0000000000000000000000000000000000000000000000000000000000000123123abff".
    /// </summary>
    /// <param name="str">string of hexadecimal characters [0-9a-fA-F]</param>
    /// <param name="value">result value</param>
    /// <returns>false if contains non-hex characters, or if number of meaningful hexnumbers is over 24.</returns>
    public static bool TryParseToDecimal(String str, out decimal value)
    {
        HexEnumerator stream = new HexEnumerator(str.GetEnumerator());

        // Calculate hex character count
        int count = 0;
        // Calculate meaningful number characters
        int meaningfulDigits = 0;
        for (int i = 0; i < 999 && stream.MoveNext(); i++)
        {
            // Got non-hex value.
            if (stream.Current < 0) { value = 0; return false; }
            // Start calculating after first non-zero.
            if (meaningfulDigits > 0 || stream.Current != 0) meaningfulDigits++;
            // Total character count.
            count++;
        }

        // Found non-zero value.
        if (stream.Error) { value = 0; return false; }

        // Restart
        stream.Reset();

        // Too many characters
        if (meaningfulDigits >= 25) { value = 0; return false; }

        int lo = 0, mid = 0, hi = 0;
        while (count > 24 && stream.MoveNext())
        {
            // decimal can fit 96bit value, any non-zero over 96bits cannot be parsed.
            if (stream.Current != 0) { value = 0; return false; }
            count--;
        }
        while (count > 16 && stream.MoveNext())
        {
            hi = (hi << 4) | stream.Current;
            count--;
        }
        while (count > 8 && stream.MoveNext())
        {
            mid = (mid << 4) | stream.Current;
            count--;
        }
        while (count > 0 && stream.MoveNext())
        {
            lo = (lo << 4) | stream.Current;
            count--;
        }

        value = new decimal(lo, mid, hi, false, 0);
        return !stream.Error;
    }

    /// <summary>
    /// Try to parse a string to big integer number.
    /// String must not start with '0x'.
    /// </summary>
    /// <param name="str">string of hexadecimal characters [0-9a-fA-F]</param>
    /// <param name="value">result value</param>
    /// <returns>false if contains non-hex characters, or if number of meaningful hexnumbers is over 24.</returns>
    public static bool TryParseToBigInteger(String str, out BigInteger value)
    {
        return BigInteger.TryParse(str, out value);
    }
}

/// <summary>String as a stream of parsed hex decimals (0..15).</summary>
public struct HexEnumerable : IEnumerable<int>
{
    /// <summary></summary>
    public readonly IEnumerable<char> charStream;
    /// <summary></summary>
    public HexEnumerable(IEnumerable<char> charStream)
    {
        this.charStream = charStream;
    }

    /// <summary>Get enumerator. If charStream is String, no heap objects are allocated.</summary>
    /// <returns>Hex enumerator</returns>
    public HexEnumerator GetEnumerator() => new HexEnumerator(charStream is String str ? str.GetEnumerator() : charStream.GetEnumerator());
    IEnumerator IEnumerable.GetEnumerator() => new HexEnumerator(charStream is String str ? str.GetEnumerator() : charStream.GetEnumerator());
    IEnumerator<int> IEnumerable<int>.GetEnumerator() => new HexEnumerator(charStream is String str ? str.GetEnumerator() : charStream.GetEnumerator());
}

/// <summary>
/// String as a stream of parsed hex decimals (0..15).
/// 
/// Returns -1 for any character that is not hexadecimal (0..9, a..f, A..F).
/// 
/// Error flag is raised to true if encountered a non-hexadecimal character.
/// </summary>
public struct HexEnumerator : IEnumerator<int>
{
    /// <summary></summary>
    public int Current => current;
    /// <summary></summary>
    public bool Error => error;
    /// <summary></summary>
    bool error;
    /// <summary></summary>
    IEnumerator<char>? charStream;
    /// <summary></summary>
    CharEnumerator? charStream2;
    /// <summary></summary>
    object IEnumerator.Current => current;
    /// <summary></summary>
    int current;
    /// <summary></summary>
    bool eos;

    /// <summary></summary>
    public HexEnumerator(IEnumerator<char> charStream)
    {
        this.charStream = charStream ?? throw new ArgumentNullException(nameof(charStream));
        this.charStream2 = null;
        current = 0;
        error = false;
        eos = false;
    }

    /// <summary></summary>
    public HexEnumerator(CharEnumerator charStream)
    {
        this.charStream = null;
        this.charStream2 = charStream;
        current = 0;
        error = false;
        eos = false;
    }

    /// <summary></summary>
    public void Dispose()
    {
        if (charStream != null) charStream.Dispose();
        else charStream2?.Dispose();
    }

    /// <summary></summary>
    public bool MoveNext()
    {
        if (eos) return false;
        int value;
        if (charStream != null)
        {
            if (charStream != null && !charStream.MoveNext()) { current = -1; eos = true; return false; }
            value = (int)charStream!.Current;
        }
        else
        {
            if (charStream2 != null && !charStream2.MoveNext()) { current = -1; eos = true; return false; }
            value = (int)charStream2!.Current;
        }
        current = (value >= 48 && value <= 57) ? value - 48 : value >= 65 && value <= 70 ? value - 55 : value >= 97 && value <= 102 ? value - 87 : -1;
        error |= current < 0;
        return true;
    }

    /// <summary></summary>
    public void Reset()
    {
        if (charStream != null) charStream.Reset(); else charStream2!.Reset();
        current = 0;
        error = false;
        eos = false;
    }
}

/// <summary>
/// String as a stream of parsed hex decimals (0..15).
/// 
/// Returns -1 for any character that is not hexadecimal (0..9, a..f, A..F).
/// 
/// Error flag is raised to true if encountered a non-hexadecimal character.
/// </summary>
public ref struct HexSpanEnumerator
{
    /// <summary></summary>
    public int Current;
    /// <summary></summary>
    public bool Error => error;
    /// <summary></summary>
    bool error;
    /// <summary></summary>
    ReadOnlySpan<char> charSpan;
    /// <summary></summary>
    int index;
    /// <summary></summary>
    bool eos => index >= charSpan.Length;

    /// <summary></summary>
    public HexSpanEnumerator(ReadOnlySpan<char> charSpan)
    {
        this.charSpan = charSpan;
        Current = -1;
        error = false;
        index = 0;
    }

    /// <summary></summary>
    public bool MoveNext()
    {
        if (eos) return false;
        int value;

        if (eos) { Current = -1; return false; }
        value = (int)charSpan[index++];
        Current = (value >= 48 && value <= 57) ? value - 48 : value >= 65 && value <= 70 ? value - 55 : value >= 97 && value <= 102 ? value - 87 : -1;
        error |= Current < 0;
        return true;
    }

    /// <summary></summary>
    public void Reset()
    {
        Current = -1;
        error = false;
        index = 0;
    }
}

