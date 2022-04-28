// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Runtime.Serialization;
using System.Text;

/// <summary>
/// Short 4-character string that is represented as <see cref="System.UInt32"/> value.
/// 
/// The first character becomes most meaningful byte of <see cref="Value"/>, last character is the least. 
/// This way value is ordinal comparable.
/// 
/// When serializing <see cref="Value"/> it should be written in little-endian order so that characters are in correct order.
/// </summary>
public struct Label4 : IComparable<Label4>, IEquatable<Label4>
{
    /// <summary>Encoder</summary>
    static UTF8Encoding encoder = new UTF8Encoding(false, true);

    /// <summary></summary>
    static Label4 empty = new Label4(0U, "");
    /// <summary></summary>
    public static Label4 Empty => empty;

    /// <summary></summary>
    public const int ByteCount = 4;
    /// <summary>4 character label</summary>
    [DataMember(Name = "Value", Order = 0)]
    public readonly uint Value;
    /// <summary>4 character label</summary>
    [IgnoreDataMember]
    string? shortName;

    /// <summary>Tool tip </summary>
    [IgnoreDataMember]
    public readonly string? LongName;

    /// <summary>4 character label</summary>
    [IgnoreDataMember]
    public string ShortName => shortName ??= ToStringValue(Value);

    /// <summary></summary>
    public static bool operator ==(Label4 a, Label4 b) => a.Value == b.Value;
    /// <summary></summary>
    public static bool operator !=(Label4 a, Label4 b) => a.Value != b.Value;
    /// <summary></summary>
    public static implicit operator (string, string?)(Label4 label) => (label.ShortName, label.LongName);
    /// <summary></summary>
    public static implicit operator Label4((string shortName, string? longName) tuple) => new Label4(tuple.shortName, tuple.longName);
    /// <summary></summary>
    public static implicit operator uint(Label4 label) => label.Value;
    /// <summary></summary>
    public static implicit operator int(Label4 label) => unchecked((int)label.Value);
    /// <summary></summary>
    public static implicit operator Label4(uint value) => new Label4(value);
    /// <summary></summary>
    public static implicit operator Label4(int value) => new Label4(unchecked((uint)value));
    /// <summary></summary>
    public static implicit operator string(Label4 label) => label.ShortName;
    /// <summary></summary>
    public static implicit operator Label4(string value) => new Label4(value);

    /// <summary>Create label</summary>
    public Label4(uint shortName)
    {
        this.Value = shortName;
        LongName = null;
        this.shortName = null;
    }

    /// <summary>Create label</summary>
    public Label4(int shortName)
    {
        this.Value = unchecked((uint)shortName);
        LongName = null;
        this.shortName = null;
    }

    /// <summary>Create label</summary>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Label4(string value)
    {
        // Assert not null
        if (value == null) throw new ArgumentNullException(nameof(value));
        // Assign
        shortName = value;
        this.Value = ToNumericValue(shortName);
        LongName = null;
    }

    /// <summary>Create label</summary>
    public Label4(uint shortName, string? longName = null)
    {
        this.Value = shortName;
        LongName = longName;
        this.shortName = null;
    }

    /// <summary>Create label</summary>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Label4(string shortName, string? longName = null)
    {
        // Assert not null
        if (shortName == null) throw new ArgumentNullException(nameof(shortName));
        // Assign
        this.shortName = shortName;
        this.Value = ToNumericValue(this.shortName);
        LongName = longName;
    }

    /// <summary>Convert <paramref name="value"/> to 4-char string.</summary>
    public static string ToStringValue(uint value)
    {
        //
        Span<byte> bytes = stackalloc byte[ByteCount];
        //
        int byteCount = -1;
        //
        for (int i = 0; i < ByteCount; i++)
        {
            //
            byte b = (byte)(value >> ((ByteCount - 1) * 8));
            //
            bytes[i] = b;
            //
            if (b != 0) byteCount = i;
            //
            value <<= 8;
        }
        //
        bytes = bytes.Slice(0, ++byteCount);
        //
        string result = encoder.GetString(bytes);
        //
        return result;
    }

    /// <summary>Convert <paramref name="value"/> to 32-bit integer</summary>
    public static uint ToNumericValue(string value)
    {
        // Allocate buffer
        Span<byte> buf = stackalloc byte[ByteCount];
        // Write
        int byteCount = encoder.GetBytes(value, buf);
        //
        if (byteCount < 0) throw new ArgumentException("Too short", nameof(value));
        //
        if (byteCount > ByteCount) throw new ArgumentException("Too long", nameof(value));
        //
        uint result = 0;
        //
        for (int i = 0; i < ByteCount; i++) result = (result << 8) | buf[i];
        // Return
        return result;
    }

    /// <summary></summary>
    public int CompareTo(Label4 other)
    {
        //
        uint v1 = this.Value, v2 = other.Value;
        //
        if (v1 == v2) return 0;
        //
        return v1 < v2 ? -1 : 1;
    }
    /// <summary></summary>
    public override bool Equals(object? obj) => obj is Label4 label ? label.Value == Value : false;
    /// <summary></summary>
    public bool Equals(Label4 other) => other.Value == Value;
    /// <summary></summary>
    public override int GetHashCode() => unchecked((int)Value);
    /// <summary></summary>
    public override string ToString() => LongName ?? ShortName;
}
