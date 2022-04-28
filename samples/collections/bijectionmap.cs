using System;
using System.Collections.Generic;
using Avalanche.Utilities;
using static System.Console;

public class bijectionmap
{
    public static void Run()
    {
        {
            var map = new BijectionMap<string, int>();
        }
        {
            var map =
                (BijectionMap<string, int>)
                BijectionMap.Create(typeof(string), typeof(int));
        }

        {
            IEqualityComparer<string> leftComparer = StringComparer.InvariantCultureIgnoreCase;
            IEqualityComparer<int> rightComparer = EqualityComparer<int>.Default;
            var map = new BijectionMap<string, int>(leftComparer, rightComparer);
        }

        {
            IEqualityComparer<string> leftComparer = StringComparer.InvariantCultureIgnoreCase;
            IEqualityComparer<int> rightComparer = EqualityComparer<int>.Default;
            var map = BijectionMap.Create(typeof(string), typeof(int), leftComparer, rightComparer);
        }
        {
            var dictionary = new Dictionary<string, int> { { "Hello", 123 } };
            var map = new BijectionMap<string, int>(dictionary);
        }
        {
            var dictionary = new Dictionary<string, int> { { "Hello", 123 } };
            var map = new BijectionMap<string, int>()
                .AddAll(dictionary);
        }

        {
            var map = new BijectionMap<string, int>();
            map.Put("A", 1);
            map.Put("B", 2);
            map.Put("C", 3);

            int x = map["A"];
            string y = map.GetLeft(2);
            int z = map.GetRight("B");
            if (map.TryGetRight("C", out int value0)) WriteLine(value0); // 3
            if (map.TryGetLeft(2, out string? value1)) WriteLine(value1); // "B"

            map.SetReadOnly();

            // Write
            try
            {
                map.Put("Key3", 99);
            }
            catch (InvalidOperationException)
            {
            }
        }

        {
            var map = new BijectionMap<string, int>();
            map.Put("A", 1);
            map.Put("B", 2);
            map.Put("C", 3);
            WriteLine(map.Contains("A", 1)); // True
            WriteLine(map.Contains("A", 2)); // False
            WriteLine(map.ContainsLeft("A")); // True
            WriteLine(map.ContainsRight(3)); // True
        }

        {
            var map = new BijectionMap<string, int>();
            map.Put("A", 1);
            map.Put("B", 2);
            map.Put("C", 3);
            map.RemoveWithLeft("A");
            map.RemoveWithRight(2);
        }

        {
            var map = new BijectionMap<string, int>();
            map.Put("A", 1);
            map.Put("B", 2);
            map.Put("C", 3);
            map.RetainAllLeft(new string[] { "B", "C" });
            map.RetainAllRight(new int[] { 2, 3 });
            foreach (var pair in map) WriteLine(pair);
        }

        {
            var map = new BijectionMap<string, int>();
            map.Put("A", 1);
            map.Put("B", 2);
            map.Put("C", 3);
            ICollection<int> rightSet = map.GetRightSet();
            ICollection<string> leftSet = map.GetLeftSet();
            IDictionary<string, int> rightMap = map.GetLeftToRightDictionary();
            IDictionary<int, string> leftMap = map.GetRightToLeftDictionary();
        }
    }
}

