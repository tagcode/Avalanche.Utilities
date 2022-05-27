// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

// <docs>
/// <summary>Position marked text segment (for diagnostics purposes)</summary>
public readonly struct MarkedText : IEquatable<MarkedText>
{
    /// <summary>Implicit conversion</summary>
    public static implicit operator String(MarkedText str) => str.AsString;
    /// <summary>Implicit conversion</summary>
    public static implicit operator MarkedText(String str) => new MarkedText(str);

    /// <summary>Text</summary>
    public readonly ReadOnlyMemory<char> Text;
    /// <summary>Text and file position mark</summary>
    public readonly TextRange Position;

    /// <summary>Create text</summary>
    public MarkedText(ReadOnlyMemory<char> text)
    {
        this.Text = text;
        this.Position = new TextRange();
    }

    /// <summary>Create text</summary>
    public MarkedText(string text)
    {
        this.Text = text.AsMemory();
        this.Position = new TextRange();
    }

    /// <summary>Create text</summary>
    public MarkedText(string text, string? filename)
    {
        this.Text = text.AsMemory();
        this.Position = new TextRange(filename, default, default);
    }

    /// <summary>Create text</summary>
    public MarkedText(ReadOnlyMemory<char> text, TextRange position)
    {
        this.Text = text;
        this.Position = position;
    }

    /// <summary>Create text</summary>
    public MarkedText(string text, TextRange range)
    {
        this.Text = text == null ? default : text.AsMemory();
        this.Position = range;
    }

    /// <summary>Create text</summary>
    public MarkedText(ReadOnlyMemory<char> text, string? filename, TextPosition start, TextPosition end)
    {
        this.Text = text;
        this.Position = new TextRange(filename, start, end);
    }

    /// <summary>Create text</summary>
    public MarkedText(string text, string? filename, TextPosition start, TextPosition end)
    {
        this.Text = text == null ? default : text.AsMemory();
        this.Position = new TextRange(filename, start, end);
    }

    /// <summary>Create text</summary>
    public MarkedText(ReadOnlyMemory<char> text, string? filename, int startLine, int startColumn, int endLine, int endColumn)
    {
        this.Text = text;
        this.Position = new TextRange(filename, startLine, startColumn, endLine, endColumn);
    }

    /// <summary>Create text</summary>
    public MarkedText(string text, string? filename, int startLine, int startColumn, int endLine, int endColumn)
    {
        this.Text = text == null ? default : text.AsMemory();
        this.Position = new TextRange(filename, startLine, startColumn, endLine, endColumn);
    }

    /// <summary>Return new <see cref="MarkedText"/> with <paramref name="newFilename"/></summary>
    public MarkedText SetFileName(string? newFilename) => new MarkedText(Text, Position.SetFileName(newFilename));

    /// <summary>Test if <see cref="Text"/> has assigned string. Empty "" passes as true. <![CDATA[default]]> initialized returns false.</summary>
    public bool HasValue => Text.Length > 0 || MemoryMarshal.TryGetString(Text, out string? t2, out int s2, out int l2) && t2 != null;       
    /// <summary></summary>
    public bool HasPosition => Position.HasValue;
    /// <summary></summary>
    public MarkedText Slice(int index, int length) => new MarkedText(Text, Position.Slice(index, length));
    /// <summary>Return original string of <see cref="Text"/> string or create new string</summary>
    public string AsString => Text.IsEmpty ? "" : MemoryMarshal.TryGetString(Text, out string? text, out int start, out int length) && text != null & start == 0 && length == Text.Length ? text! : new string(Text.Span);
    /// <summary>Return original string or null</summary>
    public string? AsPossibleString => Text.IsEmpty ? null : MemoryMarshal.TryGetString(Text, out string? text, out int start, out int length) && text != null & start == 0 && length == Text.Length ? text! : new string(Text.Span);

    /// <summary></summary>
    public override bool Equals([NotNullWhen(true)] object? obj) 
    {
        if (obj is not MarkedText other) return false;
        return System.MemoryExtensions.SequenceEqual(Text.Span, other.Text.Span);
    }
    /// <summary></summary>
    public bool Equals(MarkedText other) => System.MemoryExtensions.SequenceEqual(Text.Span, other.Text.Span);
    /// <summary></summary>
    public override int GetHashCode()
    {
        FNVHash32 hash = new();
        hash.HashIn(Text.Span);
        return hash.Hash;
    }

    /// <summary>Append to <paramref name="sb"/>.</summary>
    public StringBuilder AppendTo(StringBuilder sb)
    {
        int pos = sb.Length;
        if (Text.Length > 0) sb.Append(Text);
        if (sb.Length > pos) sb.Append(" ");
        Position.AppendTo(sb);
        return sb;
    }
    /// <summary>Print information</summary>
    public string Information => AppendTo(new StringBuilder()).ToString();
    /// <summary>Print string</summary>
    //public override string ToString() => AsString;
    public override string ToString() => Information;
}
// </docs>
