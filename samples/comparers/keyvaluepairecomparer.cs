using System;
using System.Collections.Generic;
using Avalanche.Utilities;

class keyvaluepairecomparer
{
    public static void Run()
    {
        {
            // Create pairs
            var pairs = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("Key3", "Value1"),
                new KeyValuePair<string, string>("Key2", "Value2"),
                new KeyValuePair<string, string>("Key1", "Value3")
            };
            // Create comparer
            IComparer<KeyValuePair<string, string>> comparer =
                new KeyValuePairComparer<string, string>(
                    Comparer<string>.Default,
                    Comparer<string>.Default
                );
            // Sort
            Array.Sort(pairs, comparer); // Key1, Key2, Key3
            Print(pairs);
        }
        {
            // Create pairs
            var pairs = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("Key3", "Value1"),
                new KeyValuePair<string, string>("Key2", "Value2"),
                new KeyValuePair<string, string>("Key1", "Value3")
            };
            // Use default comparer
            IComparer<KeyValuePair<string, string>> comparer =
                KeyValuePairComparer<string, string>.Instance;
            // Sort
            Array.Sort(pairs, comparer); // Key1, Key2, Key3
            Print(pairs);
        }
        {
            // Create pairs
            var pair1 = new KeyValuePair<string, string>("Key3", "Value1");
            var pair2 = new KeyValuePair<string, string>("Key2", "Value1");
            // Create comparer
            IEqualityComparer<KeyValuePair<string, string>> comparer =
                new KeyValuePairEqualityComparer<string, string>(
                    EqualityComparer<string>.Default,
                    EqualityComparer<string>.Default
                );
            // Compare
            bool equal = comparer.Equals(pair1, pair2);
        }
        {
            // Create pairs
            var pairs = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("Key3", "Value1"),
                new KeyValuePair<string, string>("Key2", "Value2"),
                new KeyValuePair<string, string>("Key1", "Value3")
            };
            // Create pairs
            var pair1 = new KeyValuePair<string, string>("Key3", "Value1");
            var pair2 = new KeyValuePair<string, string>("Key2", "Value1");
            // Create comparer
            IEqualityComparer<KeyValuePair<string, string>> comparer =
                KeyValuePairEqualityComparer<string, string>.Instance;
            // Compare
            bool equal = comparer.Equals(pair1, pair2);
        }
    }

    static void Print<T>(IEnumerable<T> enumr) => Console.WriteLine(String.Join(", ", enumr));
}
