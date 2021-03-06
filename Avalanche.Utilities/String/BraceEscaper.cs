// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Internal;
using System;

/// <summary>Escapes "%" as "%%".</summary>
public class PercentEscaper : IEscaper
{
    /// <summary></summary>
    static PercentEscaper instance = new PercentEscaper();
    /// <summary></summary>
    public static PercentEscaper Instance => instance;

    /// <summary>Estimate length of escape '%' to "%%".</summary>
    public int EstimateEscapedLength(ReadOnlySpan<char> unescapedInput)
    {
        // Place length here
        int length = unescapedInput.Length;
        //
        for (int i = 0; i < unescapedInput.Length; i++)
        {
            // Get char
            char c = unescapedInput[i];
            //
            if (c == '%') length++;
        }
        // Return
        return length;
    }

    /// <summary>Estimate length of unescape string where "%%" is '%'.</summary>
    public int EstimateUnescapedLength(ReadOnlySpan<char> escapedInput)
    {
        // Place length here
        int length = escapedInput.Length;
        //
        char prevChar = '\0';
        //
        for (int i = 0; i < escapedInput.Length; i++)
        {
            // Get char
            char c = escapedInput[i];
            //
            if ((c == '%' && prevChar == '%')) { length--; prevChar = '\0'; }
            //
            else prevChar = c;
        }
        // Return
        return length;
    }

    /// <summary>Escape '%' into "%%".</summary>
    public int Escape(ReadOnlySpan<char> unescapedInput, Span<char> escapedOutput)
    {
        //
        int writtenLength = 0;
        //
        for (int i=0; i<unescapedInput.Length; i++)
        {
            // Get char
            char c = unescapedInput[i];
            // Drop this char
            if (c == '%') escapedOutput[writtenLength++] = c;
            // Assign write
            escapedOutput[writtenLength++] = c;
        }
        //
        return writtenLength;
    }

    /// <summary>Unescape "{{" into '{' and "}}" into '}'.</summary>
    public int Unescape(ReadOnlySpan<char> escapedInput, Span<char> unescapedOutput)
    {
        //
        char prevChar = '\0';
        //
        int writtenLength = 0;
        //
        for (int i = 0; i < escapedInput.Length; i++)
        {
            // Get char
            char c = escapedInput[i];
            // Drop this char
            if ((c == '%' && prevChar == '%')) continue;
            // Assign write
            unescapedOutput[writtenLength++] = c;
            // 
            prevChar = c;
        }
        //
        return writtenLength;
    }
}

