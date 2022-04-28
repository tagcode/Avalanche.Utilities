// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Text;

// <docs>
/// <summary>Text position</summary>
public readonly struct TextPosition
{
    /// <summary>Line number, starting from 1. If 0 or less, unspecified</summary>
    public readonly int Line;
    /// <summary>Column number, starting from 1. If 0 or less, unspecified</summary>
    public readonly int Column;
    /// <summary>Character index in source file, if less than 0, unspecified</summary>
    public readonly int Index;

    /// <summary>Create no mark</summary>
    public TextPosition()
    {
        Line = 0;
        Column = 0;
        Index = -1;
    }

    /// <summary>Create no mark</summary>
    public TextPosition(int line, int column)
    {
        Line = line;
        Column = column;
        Index = -1;
    }

    /// <summary>Create no mark</summary>
    public TextPosition(int line, int column, int index)
    {
        Line = line;
        Column = column;
        Index = index;
    }

    /// <summary>Has <see cref="Line"/> and <see cref="Column"/></summary>
    public bool HasValue => Line > 0 && Column > 0;

    /// <summary>Append to <paramref name="sb"/>.</summary>
    public StringBuilder AppendTo(StringBuilder sb)
    {
        int pos = sb.Length;
        if (Line>0)
        {
            sb.Append("Ln ");
            sb.Append(Line);
        }
        if (Column>0) 
        {
            if (pos < sb.Length) sb.Append(", ");
            sb.Append("Col ");
            sb.Append(Column);
        }
        if (Index>=0)
        {
            if (pos < sb.Length) sb.Append(", ");
            sb.Append("Ix ");
            sb.Append(Index);
        }
        return sb;
    }

    /// <summary>Print information</summary>
    public override string ToString() => AppendTo(new StringBuilder()).ToString();
}
// </docs>
