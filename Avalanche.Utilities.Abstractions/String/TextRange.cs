// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Text;

// <docs>
/// <summary>Text range in a file</summary>
public readonly struct TextRange
{
    /// <summary>Optional source file name. May be relative or absolute. For diagnostics purposes</summary>
    public readonly string? FileName;
    /// <summary>Start position</summary>
    public readonly TextPosition Start;
    /// <summary>End position</summary>
    public readonly TextPosition End;

    /// <summary>Count length between indices</summary>
    public int Length =>
        End.Index >= 0 && Start.Index >= 0 ?
        End.Index - Start.Index :
        Start.Line > 0 && End.Line > 0 && Start.Line == End.Line ? End.Column - Start.Column :
        0;        

    /// <summary>Create no file mark</summary>
    public TextRange()
    {
        this.FileName = null;
        Start = new TextPosition();
        End = new TextPosition();
    }

    /// <summary>Create file mark</summary>
    public TextRange(string? filename, TextPosition start, TextPosition end)
    {
        this.FileName = filename;
        Start = start;
        End = end;
    }

    /// <summary>Create file mark</summary>
    public TextRange(string? filename, int startLine, int startColumn, int endLine, int endColumn)
    {
        this.FileName = filename;
        Start = new TextPosition(startLine, startColumn);
        End = new TextPosition(endLine, endColumn);
    }

    /// <summary>Return new <see cref="TextRange"/> with <paramref name="newFilename"/></summary>
    public TextRange SetFileName(string? newFilename) => new TextRange(newFilename, Start, End);

    /// <summary></summary>
    public TextRange Slice(int index, int length)
    {
        // No start and end
        if (!HasValue) return this;
        // Estimate text length
        int textLength = Length;
        // Crop index
        if (textLength > 0) index = Math.Min(textLength, index);
        // Crop length
        if (textLength > 0) length = Math.Min(textLength, index+length)-index;
        //
        TextPosition start = new TextPosition(Start.Line, Start.Column + index, Start.Index < 0 ? Start.Index : Start.Index + index);
        TextPosition end = new TextPosition(Start.Line, Start.Column + index + length, Start.Index < 0 ? Start.Index : Start.Index + index + length);
        // 
        return new TextRange(FileName, start, end);
    }

    /// <summary>Has <see cref="TextPosition.Line"/> and <see cref="TextPosition.Column"/> for <see cref="Start"/> and <see cref="End"/></summary>
    public bool HasValue => Start.HasValue && End.HasValue;

    /// <summary>Append to <paramref name="sb"/>.</summary>
    public StringBuilder AppendTo(StringBuilder sb)
    {
        int pos = sb.Length;
        if (FileName != null) sb.Append(FileName);
        if (Start.HasValue || Start.Index >= 0)
        {
            if (sb.Length > pos) sb.Append(" ");
            sb.Append('[');
            Start.AppendTo(sb);
            if (End.HasValue || End.Index >= 0)
            {
                sb.Append(" - ");
                End.AppendTo(sb);
            }
            sb.Append(']');
        }
        return sb;
    }

    /// <summary>Print information</summary>
    public override string ToString() => AppendTo(new StringBuilder()).ToString();
}
// </docs>
