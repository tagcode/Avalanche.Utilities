using System;
using System.Collections.Generic;
using System.Linq;
using Avalanche.Utilities;

class enumerablecomparer
{
    public static void Run()
    {
        {
            // Init enumerables
            IEnumerable<string> strings1 = new String[] { "a100", "B1", "a5", "A1" };
            IEnumerable<string> strings2 = new String[] { "a100", "B1", "a5", "A1", "_00" };
            IEnumerable<string>[] strarrays = new IEnumerable<string>[] { strings1, strings2 };
            // Create comparer
            IComparer<IEnumerable<string>> comparer =
                new EnumerableComparer<string>(StringComparer.InvariantCultureIgnoreCase);
            // Compare
            Array.Sort(strarrays, comparer); // [a100, B1, a5, A1], [a100, B1, a5, A1, _00]
            PrintArray(strarrays);
        }
        {
            // Init enumerables
            IEnumerable<string> strings1 = new String[] { "a100", "B1", "a5", "A1" };
            IEnumerable<string> strings2 = new String[] { "a100", "B1", "a5", "A1", "_00" };
            IEnumerable<string>[] strarrays = new IEnumerable<string>[] { strings1, strings2 };
            // Create comparer
            IComparer<IEnumerable<string>> comparer =
                (EnumerableComparer<string>)
                EnumerableComparer.Create(typeof(string), StringComparer.InvariantCultureIgnoreCase);
            // Compare
            Array.Sort(strarrays, comparer); // [a100, B1, a5, A1], [a100, B1, a5, A1, _00]
            PrintArray(strarrays);
        }
        {
            // Init enumerables
            List<string> strings1 = new List<String> { "a100", "B1", "a5", "A1" };
            List<string> strings2 = new List<String> { "a100", "B1", "a5", "A1", "_00" };
            List<string>[] strlists = new List<string>[] { strings1, strings2 };
            // Create comparer
            IComparer<List<string>> comparer =
                new EnumerableComparer<List<string>, string>(StringComparer.InvariantCultureIgnoreCase);
            // Compare
            Array.Sort(strlists, comparer); // [a100, B1, a5, A1], [a100, B1, a5, A1, _00]
            PrintArray(strlists);
        }
        {
            // Init enumerables
            List<string> strings1 = new List<String> { "a100", "B1", "a5", "A1" };
            List<string> strings2 = new List<String> { "a100", "B1", "a5", "A1", "_00" };
            List<string>[] strlists = new List<string>[] { strings1, strings2 };
            // Create comparer
            IComparer<List<string>> comparer =
                (EnumerableComparer<string>)
                EnumerableComparer.Create(typeof(List<string>), typeof(string), StringComparer.InvariantCultureIgnoreCase);
            // Compare
            Array.Sort(strlists, comparer); // [a100, B1, a5, A1], [a100, B1, a5, A1, _00]
            PrintArray(strlists);
        }
        {
            String[] strings1 = { "a100", "B1", "a5", "A1" };
            String[] strings2 = { "a100", "B1", "a5", "A1", "_00" };
            string[][] strarrays = { strings1, strings2 };
            IComparer<string[]> comparer = new ArrayComparer<string>(StringComparer.InvariantCultureIgnoreCase);
            Array.Sort(strarrays, comparer); // [a100, B1, a5, A1], [a100, B1, a5, A1, _00]
            PrintArray(strarrays);
        }
        {
            // Init arrays
            String[] strings1 = { "a100", "B1", "a5", "A1" };
            String[] strings2 = { "a100", "B1", "a5", "A1", "_00" };
            string[][] strarrays = { strings1, strings2 };
            // Create comparer
            IComparer<string[]> comparer =
                (ArrayComparer<string>)
                ArrayComparer.Create(typeof(string), StringComparer.InvariantCultureIgnoreCase);
            // Compare
            Array.Sort(strarrays, comparer); // [a100, B1, a5, A1], [a100, B1, a5, A1, _00]
            PrintArray(strarrays);
        }


        {
            // Init enumerables
            IEnumerable<string> strings1 = new String[] { "a100", "B1", "a5", "A1" };
            IEnumerable<string> strings2 = new String[] { "a100", "B1", "a5", "A1", "_00" };
            // Create comparer
            IEqualityComparer<IEnumerable<string>> comparer =
                new EnumerableEqualityComparer<string>(StringComparer.InvariantCultureIgnoreCase);
            // Compare
            comparer.Equals(strings1, strings2);
        }
        {
            // Init enumerables
            IEnumerable<string> strings1 = new String[] { "a100", "B1", "a5", "A1" };
            IEnumerable<string> strings2 = new String[] { "a100", "B1", "a5", "A1", "_00" };
            // Create comparer
            IEqualityComparer<IEnumerable<string>> comparer =
                (EnumerableEqualityComparer<string>)
                EnumerableEqualityComparer.Create(typeof(string), StringComparer.InvariantCultureIgnoreCase);
            // Compare
            comparer.Equals(strings1, strings2);
        }
        {
            // Init enumerables
            List<string> strings1 = new List<String> { "a100", "B1", "a5", "A1" };
            List<string> strings2 = new List<String> { "a100", "B1", "a5", "A1", "_00" };
            // Create comparer
            IEqualityComparer<List<string>> comparer =
                new EnumerableEqualityComparer<List<string>, string>(StringComparer.InvariantCultureIgnoreCase);
            // Compare
            comparer.Equals(strings1, strings2);
        }
        {
            // Init enumerables
            List<string> strings1 = new List<String> { "a100", "B1", "a5", "A1" };
            List<string> strings2 = new List<String> { "a100", "B1", "a5", "A1", "_00" };
            // Create comparer
            IEqualityComparer<List<string>> comparer =
                (EnumerableEqualityComparer<List<string>, string>)
                EnumerableEqualityComparer.Create(typeof(List<string>), typeof(string), StringComparer.InvariantCultureIgnoreCase);
            // Compare
            comparer.Equals(strings1, strings2);
        }
        {
            // Init arrays
            String[] strings1 = { "a100", "B1", "a5", "A1" };
            String[] strings2 = { "a100", "B1", "a5", "A1", "_00" };
            // Create comparer
            IEqualityComparer<string[]> comparer =
                new ArrayEqualityComparer<string>(StringComparer.InvariantCultureIgnoreCase);
            // Compare
            comparer.Equals(strings1, strings2);
        }
        {
            // Init arrays
            String[] strings1 = { "a100", "B1", "a5", "A1" };
            String[] strings2 = { "a100", "B1", "a5", "A1", "_00" };
            // Create comparer
            IEqualityComparer<string[]> comparer =
                (ArrayEqualityComparer<string>)
                ArrayEqualityComparer.Create(typeof(string), StringComparer.InvariantCultureIgnoreCase);
            // Compare
            comparer.Equals(strings1, strings2);
        }


    }

    static void PrintArray<T>(IEnumerable<IEnumerable<T>> enumr) => Console.WriteLine($"[{String.Join("], [", enumr.Select(array => String.Join(", ", array)))}]");
}
