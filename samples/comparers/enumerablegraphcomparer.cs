using System;
using System.Collections.Generic;
using System.Linq;
using Avalanche.Utilities;

public class enumerablegraphcomparer
{
    public record Node
    {
        public Node[] Children = new Node[0];
    }

    public static void Run()
    {
        Node node1 = new Node(), node2 = new Node(), node3 = new Node();
        node1.Children = new[] { node2 };
        node2.Children = new[] { node3 };
        node3.Children = new[] { node1 };



        {
            // Init enumerables
            IEnumerable<string> strings1 = new String[] { "a100", "B1", "a5", "A1" };
            IEnumerable<string> strings2 = new String[] { "a100", "B1", "a5", "A1", "_00" };
            IEnumerable<string>[] strarrays = new IEnumerable<string>[] { strings1, strings2 };
            // Create comparer
            IGraphComparer<IEnumerable<string>> comparer =
                new EnumerableGraphComparer<string>(StringComparer.InvariantCultureIgnoreCase);
            // Compare
            Array.Sort(strarrays, (IComparer<IEnumerable<string>>)comparer); // [a100, B1, a5, A1], [a100, B1, a5, A1, _00]
            PrintArrays(strarrays);
        }
        {
            // Init enumerables
            IEnumerable<string> strings1 = new String[] { "a100", "B1", "a5", "A1" };
            IEnumerable<string> strings2 = new String[] { "a100", "B1", "a5", "A1", "_00" };
            IEnumerable<string>[] strarrays = new IEnumerable<string>[] { strings1, strings2 };
            // Create comparer
            IGraphComparer<IEnumerable<string>> comparer =
                (EnumerableGraphComparer<string>)
                EnumerableGraphComparer.Create(typeof(string), StringComparer.InvariantCultureIgnoreCase);
            // Compare
            Array.Sort(strarrays, (IComparer<IEnumerable<string>>)comparer); // [a100, B1, a5, A1], [a100, B1, a5, A1, _00]
            PrintArrays(strarrays);
        }
        {
            // Init enumerables
            List<string> strings1 = new List<String> { "a100", "B1", "a5", "A1" };
            List<string> strings2 = new List<String> { "a100", "B1", "a5", "A1", "_00" };
            List<string>[] strlists = new List<string>[] { strings1, strings2 };
            // Create comparer
            IGraphComparer<List<string>> comparer =
                new EnumerableGraphComparer<List<string>, string>(StringComparer.InvariantCultureIgnoreCase);
            // Compare
            Array.Sort(strlists, (IComparer<List<string>>)comparer); // [a100, B1, a5, A1], [a100, B1, a5, A1, _00]
            PrintArrays(strlists);
        }
        {
            // Init enumerables
            List<string> strings1 = new List<String> { "a100", "B1", "a5", "A1" };
            List<string> strings2 = new List<String> { "a100", "B1", "a5", "A1", "_00" };
            List<string>[] strlists = new List<string>[] { strings1, strings2 };
            // Create comparer
            IGraphComparer<List<string>> comparer =
                (EnumerableGraphComparer<string>)
                EnumerableGraphComparer.Create(typeof(List<string>), typeof(string), StringComparer.InvariantCultureIgnoreCase);
            // Compare
            Array.Sort(strlists, (IComparer<List<string>>)comparer); // [a100, B1, a5, A1], [a100, B1, a5, A1, _00]
            PrintArrays(strlists);
        }
        {
            String[] strings1 = { "a100", "B1", "a5", "A1" };
            String[] strings2 = { "a100", "B1", "a5", "A1", "_00" };
            string[][] strarrays = { strings1, strings2 };
            IGraphComparer<string[]> comparer = new ArrayGraphComparer<string>(StringComparer.InvariantCultureIgnoreCase);
            Array.Sort(strarrays, (IComparer<string[]>)comparer); // [a100, B1, a5, A1], [a100, B1, a5, A1, _00]
            PrintArrays(strarrays);
        }
        {
            // Init arrays
            String[] strings1 = { "a100", "B1", "a5", "A1" };
            String[] strings2 = { "a100", "B1", "a5", "A1", "_00" };
            string[][] strarrays = { strings1, strings2 };
            // Create comparer
            IGraphComparer<string[]> comparer =
                (ArrayGraphComparer<string>)
                ArrayGraphComparer.Create(typeof(string), StringComparer.InvariantCultureIgnoreCase);
            // Compare
            Array.Sort(strarrays, (IComparer<string[]>)comparer); // [a100, B1, a5, A1], [a100, B1, a5, A1, _00]
            PrintArrays(strarrays);
        }


        {
            // Init enumerables
            IEnumerable<string> strings1 = new String[] { "a100", "B1", "a5", "A1" };
            IEnumerable<string> strings2 = new String[] { "a100", "B1", "a5", "A1", "_00" };
            // Create comparer
            IGraphEqualityComparer<IEnumerable<string>> comparer =
                new EnumerableGraphEqualityComparer<string>(StringComparer.InvariantCultureIgnoreCase);
            // Compare
            ((IEqualityComparer<IEnumerable<string>>)comparer).Equals(strings1, strings2);
        }
        {
            // Init enumerables
            IEnumerable<string> strings1 = new String[] { "a100", "B1", "a5", "A1" };
            IEnumerable<string> strings2 = new String[] { "a100", "B1", "a5", "A1", "_00" };
            // Create comparer
            IGraphEqualityComparer<IEnumerable<string>> comparer =
                (EnumerableGraphEqualityComparer<string>)
                EnumerableGraphEqualityComparer.Create(typeof(string), StringComparer.InvariantCultureIgnoreCase);
            // Compare
            ((IEqualityComparer<IEnumerable<string>>)comparer).Equals(strings1, strings2);
        }
        {
            // Init enumerables
            List<string> strings1 = new List<String> { "a100", "B1", "a5", "A1" };
            List<string> strings2 = new List<String> { "a100", "B1", "a5", "A1", "_00" };
            // Create comparer
            IGraphEqualityComparer<List<string>> comparer =
                new EnumerableGraphEqualityComparer<List<string>, string>(StringComparer.InvariantCultureIgnoreCase);
            // Compare
            ((IEqualityComparer<List<string>>)comparer).Equals(strings1, strings2);
        }
        {
            // Init enumerables
            List<string> strings1 = new List<String> { "a100", "B1", "a5", "A1" };
            List<string> strings2 = new List<String> { "a100", "B1", "a5", "A1", "_00" };
            // Create comparer
            IGraphEqualityComparer<List<string>> comparer =
                (EnumerableGraphEqualityComparer<List<string>, string>)
                EnumerableGraphEqualityComparer.Create(typeof(List<string>), typeof(string), StringComparer.InvariantCultureIgnoreCase);
            // Compare
            ((IEqualityComparer<List<string>>)comparer).Equals(strings1, strings2);
        }
        {
            // Init arrays
            String[] strings1 = { "a100", "B1", "a5", "A1" };
            String[] strings2 = { "a100", "B1", "a5", "A1", "_00" };
            // Create comparer
            IGraphEqualityComparer<string[]> comparer =
                new ArrayGraphEqualityComparer<string>(StringComparer.InvariantCultureIgnoreCase);
            // Compare
            ((IEqualityComparer<string[]>)comparer).Equals(strings1, strings2);
        }
        {
            // Init arrays
            String[] strings1 = { "a100", "B1", "a5", "A1" };
            String[] strings2 = { "a100", "B1", "a5", "A1", "_00" };
            // Create comparer
            IGraphEqualityComparer<string[]> comparer =
                (ArrayGraphEqualityComparer<string>)
                ArrayGraphEqualityComparer.Create(typeof(string), StringComparer.InvariantCultureIgnoreCase);
            // Compare
            ((IEqualityComparer<string[]>)comparer).Equals(strings1, strings2);
        }

    }

    static void PrintArrays<T>(IEnumerable<IEnumerable<T>> enumr) => Console.WriteLine($"[{String.Join("], [", enumr.Select(array => String.Join(", ", array)))}]");
}
