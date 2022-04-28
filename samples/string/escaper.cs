using System.Collections.Generic;
using Avalanche.Utilities;
using static System.Console;

public class escaper
{
    public static void Run()
    {
        {
            // Create escaper
            IEscaper escaper = new Escaper(
                escapeChar: '\\',
                separatorChar: ',',
                escapeMap: Escaper.ControlCharEscapeMap,
                unescapeMap: Escaper.ControlCharUnescapeMap
            );
        }
        {
            // Create escaper
            IEscaper escaper = Escaper.Create(
                escapeChar: '\\',
                separatorChar: ',',
                toEscapeChars: "()".ToCharArray()
            );

            // Escape
            WriteLine(escaper.Escape(@"a(),b()")); // @"a\(\),b\(\)"
            // Unescape
            WriteLine(escaper.Unescape(@"a\(\),b\(\)")); // @"a(),b()"

            List<string> splits = escaper.UnescapeSplit(@"a\(\),b\(\)");

            string join = escaper.EscapeJoin(splits);

            // Reserver 4 slots from stack
            StructList4<string> list = new StructList4<string>();
            // Unescape and split ','
            if (escaper.TryUnescapeSplit(@"a\(\),b\(\)", ref list))
                foreach (var part in list) WriteLine(part); // "a()", "b()"

            // Join with ',' and escape
            if (escaper.TryEscapeJoin(list, out string? joined))
                WriteLine(joined); // @"a\(\),b\(\)"
        }
        {
            IEscaper escaper = Escaper.Backslash;
            WriteLine(escaper.Escape(@"Value\Value\Value"));       // @"Value\\Value\\Value"
            WriteLine(escaper.Unescape(@"Value\\Value\\Value"));   // @"Value\Value\Value"
        }
        {
            IEscaper escaper = Escaper.Dot;
            WriteLine(escaper.Escape(@"Value.Value.Value"));       // @"Value\.Value\.Value"
            WriteLine(escaper.Unescape(@"Value\.Value\.Value"));   // @"Value.Value.Value"

            WriteLine(escaper.Escape(@"Value\.Value\.Value"));     // @"Value\\\.Value\\\.Value"
            WriteLine(escaper.Unescape(@"Value\\\.Value\\\.Value")); // @"Value\.Value\.Value"
        }
        {
            string[] args = { "Arg,0", "Arg1", "Arg2" };
            string join = Escaper.Comma.EscapeJoin(args);
            WriteLine(join); // "Arg\,0,Arg1,Arg2"
        }
        {
            string[] args = { "Arg,0", "Arg1", "Arg2" };
            string csvLine = Escaper.Semicolon.EscapeJoin(args);
            WriteLine(csvLine); // "Arg,0;Arg1;Arg2"
        }
        {
            string[] args = { "Arg,0", "Arg1", "Arg2" };
            string line = Escaper.Colon.EscapeJoin(args);
            WriteLine(line); // "Arg,0:Arg1:Arg2"
        }
        {
            IEscaper escaper = Escaper.Quotes;
            WriteLine(escaper.Escape(@"""Hello world"""));       // "\"Hello world\""
            WriteLine(escaper.Unescape("\"Hello world\""));      // @"""Hello world"""
        }
        {
            IEscaper escaper = Escaper.Dot;
            WriteLine(escaper.Escape("\0\a\b\v\f\n\r")); // @"\0\a\b\v\f\n\r"
            WriteLine(escaper.Unescape(@"\0\a\b\v\f\n\r\t")); // "\0\a\b\v\f\n\r\t"
        }
        {
            IEscaper escaper = Escaper.Create('-', '-');
            WriteLine(escaper.Escape("\0\a\b\v\f\n\r")); // @"-0-a-b-v-f-n-r"
            WriteLine(escaper.Unescape("-0-a-b-v-f-n-r-t")); // "\0\a\b\v\f\n\r\t"
        }
        {
            IEscaper escaper = Escaper.Create('-', '-', '\t');
            WriteLine(escaper.Escape("\0\a\b\v\f\n\r\t")); // @"-0-a-b-v-f-n-r-t"
            WriteLine(escaper.Unescape("-0-a-b-v-f-n-r-t")); // "\0\a\b\v\f\n\r\t"
        }

        {
            IEscaper escaper = Escaper.Brace;
            WriteLine(escaper.Escape("Hello {0}"));       // "Hello {{0}}"
            WriteLine(escaper.Unescape("Hello {{0}}"));   // "Hello {0}"
        }
    }
}

