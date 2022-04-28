using System;
using System.Collections.Generic;
using Avalanche.Utilities;
using static System.Console;

public class enumerableextensions
{
    public static void Run()
    {
        {
            int[] array = { 1, 2, 3 };
        }
        {
            HashSet<int> collection = new HashSet<int>();
            int[] elementsToAdd1 = { 1, 2, 3 };
            collection.AddRange(elementsToAdd1);
        }
        {
            List<string> lines = new List<string> { "Hello", "World" };
            lines.AddIfNew("Hello");
            WriteLine(lines.Count); // 2
        }
        {
            List<string> lines = new List<string> { "Hello", "World" };
            IEnumerable<string> enumr = lines.AppendIfNew("Hello").AppendIfNew("Bye");
            // "Hello", "World", "Bye"
        }
        {
            List<string> lines = new List<string> { "Hello", "World" };
            string[] elementsToAdd = { "Hello", "World", "Bye" };
            IEnumerable<string> enumr = lines.AppendIfNew(elementsToAdd);
            // "Hello", "World", "Bye"
        }
        {
            List<string> lines1 = new List<string> { "Hello", "World" };
            List<string> lines2 = new List<string> { "More", "Lines" };
            string[] concat = lines1.ConcatToArray(lines2);
        }
        {
            List<string> lines1 = new List<string> { "Hello", "World" };
            List<string> lines2 = new List<string> { "More", "Lines" };
            List<string> lines3 = new List<string> { "ABC", "EFG" };
            string[] concat = lines1.ConcatToArray(lines2, lines3);
        }
        {
            string[] lines = new string[] { "Hello", "World" };
            IEnumerable<string> concat = lines.ConcatOptional(null);
        }
        {
            string[] lines = new string[] { "Hello", "World" };
            IEnumerable<string> enumr = lines.ExceptValue("Hello");
        }
        {
            string[] lines1 = new string[0];
            string[] lines2 = new string[] { "Hello", "World" };
            WriteLine(lines1.IsEmpty()); // True
            WriteLine(lines2.IsEmpty()); // False
        }
        {
            string[] lines = new string[] { "Hello", "Hallo" };
            WriteLine(lines.AllTrue((string s) => s[0] == 'H')); // True
        }
        {
            string[] lines1 = new string[] { "Hello", "Hello" };
            string[] lines2 = new string[] { "Hello", "World" };
            WriteLine(lines1.SameValue()); // "Hello"
            WriteLine(lines2.SameValue()); // Null
            WriteLine(lines2.SameValue("Fallback")); // "Fallback"
        }
        {
            string[] lines = new string[] { "Hello", "World" };
            WriteLine(lines.IndexOf((string s) => s == "World")); // 1
        }
        {
            string[] lines = new string[] { "Hello", "World" };
            WriteLine(lines.IndexOf("world", StringComparer.InvariantCultureIgnoreCase)); // 1
        }
        {
            string[] lines = new string[] { "Hello", "World", "world" };
            foreach (int index in lines.IndicesOf("World", StringComparer.InvariantCultureIgnoreCase))
                WriteLine(index); // 1, 2
        }
        {
            string[] lines1 = new string[] { "Hello", "World" };
            string[] lines2 = new string[] { "Hello", "World" };
            WriteLine(lines1.CompareEquality(lines2)); // True
            WriteLine(lines1.CompareEquality(lines2, StringComparer.InvariantCultureIgnoreCase)); // True
        }
        {
            string[] lines1 = new string[] { "Hello", "World" };
            string[] lines2 = new string[] { "Hello", "World" };
            WriteLine(lines1.CompareOrder(lines2)); // 0
            WriteLine(lines1.CompareOrder(lines2, StringComparer.InvariantCultureIgnoreCase)); // 0
        }
        {
            string[] lines = new string[] { "Hello", "World" };
            WriteLine(lines.HashElements());
        }        
        {
            string[] lines = new string[] { "1", "2", "3", "2", "3", "2"  };
            IEnumerable<string> byFrequency = lines.OrderByFrequency();
            WriteLine(String.Join(", ", byFrequency)); // "2, 3, 1"
        }
    }
}

