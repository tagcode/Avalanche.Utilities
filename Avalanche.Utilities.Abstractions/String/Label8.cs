// Copyright (c) Toni Kalajainen 2022
using System.Runtime.Serialization;
using System.Text;

namespace Avalanche.Utilities
{
    /// <summary>
    /// Short 8-character string that is represented as <see cref="System.UInt64"/> value.
    /// 
    /// The first character becomes most meaningful byte of <see cref="Value"/>, last character is the least. This way value is ordinal sortable.
    /// 
    /// When serializing <see cref="Value"/> it should be written in little-endian order so that characters are in correct order.
    /// </summary>
    public struct Label8 : IComparable<Label8>, IEquatable<Label8>
    {
        /// <summary>Encoder</summary>
        static UTF8Encoding encoder = new UTF8Encoding(false, true);

        /// <summary></summary>
        static Label8 empty = new Label8(0U, "");
        /// <summary></summary>
        public static Label8 Empty => empty;

        /// <summary></summary>
        public const int ByteCount = 8;

        /// <summary>4 character label</summary>
        [DataMember(Name = "Value", Order = 0)]
        public readonly ulong Value;
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
        public static bool operator ==(Label8 a, Label8 b) => a.Value == b.Value;
        /// <summary></summary>
        public static bool operator !=(Label8 a, Label8 b) => a.Value != b.Value;
        /// <summary></summary>
        public static implicit operator (string, string?)(Label8 label) => (label.ShortName, label.LongName);
        /// <summary></summary>
        public static implicit operator Label8((string shortName, string? longName) tuple) => new Label8(tuple.shortName, tuple.longName);
        /// <summary></summary>
        public static implicit operator long(Label8 label) => unchecked((long)label.Value);
        /// <summary></summary>
        public static implicit operator ulong(Label8 label) => label.Value;
        /// <summary></summary>
        public static implicit operator Label8(long value) => new Label8(unchecked((ulong)value));
        /// <summary></summary>
        public static implicit operator Label8(ulong value) => new Label8(value);
        /// <summary></summary>
        public static implicit operator string(Label8 label) => label.ShortName;
        /// <summary></summary>
        public static implicit operator Label8(string value) => new Label8(value);

        /// <summary>Create label</summary>
        public Label8(long shortName)
        {
            this.Value = unchecked((ulong)shortName);
            LongName = null;
            this.shortName = null;
        }

        /// <summary>Create label</summary>
        public Label8(ulong shortName)
        {
            this.Value = shortName;
            LongName = null;
            this.shortName = null;
        }

        /// <summary>Create label</summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public Label8(string value)
        {
            // Assert not null
            if (value == null) throw new ArgumentNullException(nameof(value));
            // Assign
            shortName = value;
            this.Value = ToNumericValue(shortName);
            LongName = null;
        }

        /// <summary>Create label</summary>
        public Label8(ulong shortName, string? longName = null)
        {
            this.Value = shortName;
            LongName = longName;
            this.shortName = null;
        }

        /// <summary>Create label</summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public Label8(string shortName, string? longName = null)
        {
            // Assert not null
            if (shortName == null) throw new ArgumentException(nameof(shortName));
            // Assign
            this.shortName = shortName;
            this.Value = ToNumericValue(this.shortName);
            LongName = longName;
        }

        /// <summary>Convert <paramref name="value"/> to 4-char string.</summary>
        public static string ToStringValue(ulong value)
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
        public static ulong ToNumericValue(string value)
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
            ulong result = 0;
            //
            for (int i = 0; i < ByteCount; i++) result = (result << 8) | buf[i];
            // Return
            return result;
        }

        /// <summary></summary>
        public int CompareTo(Label8 other)
        {
            //
            ulong v1 = this.Value, v2 = other.Value;
            //
            if (v1 == v2) return 0;
            //
            return v1 < v2 ? -1 : 1;
        }
        /// <summary></summary>
        public override bool Equals(object? obj) => obj is Label8 label ? label.Value == Value : false;
        /// <summary></summary>
        public bool Equals(Label8 other) => other.Value == Value;
        /// <summary></summary>
        public override int GetHashCode() => unchecked((int)((Value & 0xffffffff) | (Value >> 32)));
        /// <summary></summary>
        public override string ToString() => LongName ?? ShortName;
    }
}
