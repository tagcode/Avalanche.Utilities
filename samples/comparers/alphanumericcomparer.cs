using System;
using System.Collections.Generic;
using Avalanche.Utilities;

class alphanumericcomparer
{
    public static void Run()
    {
        {
            String[] strings = { "a100", "B1", "a5", "A1" };
            Array.Sort(strings, AlphaNumericComparer.InvariantCultureIgnoreCase); // A1, a5, a100, B1
            Print(strings);
        }
        {
            String[] strings = { "a100", "B1", "a5", "A1" };
            Array.Sort(strings, AlphaNumericComparer.InvariantCulture); // a5, a100, A1, B1
            Print(strings);
        }
        {
            String[] strings = { "a100", "B1", "a5", "A1" };
            Array.Sort(strings, AlphaNumericComparer.CurrentCultureIgnoreCase); // a5, a100, A1, B1
            Print(strings);
        }
        {
            String[] strings = { "a100", "B1", "a5", "A1" };
            Array.Sort(strings, AlphaNumericComparer.CurrentCulture); // a5, a100, A1, B1
            Print(strings);
        }
        {
            String[] strings = { "a100", "B1", "a5", "A1" };
            Array.Sort(strings, AlphaNumericComparer.CurrentUICultureIgnoreCase); // a5, a100, A1, B1
            Print(strings);
        }
        {
            String[] strings = { "a100", "B1", "a5", "A1" };
            Array.Sort(strings, AlphaNumericComparer.CurrentUICulture); // a5, a100, A1, B1
            Print(strings);
        }
    }
    static void Print<T>(IEnumerable<T> enumr) => Console.WriteLine(String.Join(", ", enumr));
}
