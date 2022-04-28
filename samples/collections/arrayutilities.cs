using System;
using System.Collections.Generic;
using Avalanche.Utilities;
using static System.Console;

public class arrayutilities
{
    public static void Run()
    {
        {
            int[]? array = { 1, 2, 3 };
            ArrayUtilities.Add(ref array, 4);
        }
        {
            int[] array = { 1, 2, 3 };
            array = ArrayUtilities.Append(array, 4);
        }

        {
            string[] array = { "A", "B", "C" };
            array = ArrayUtilities.AppendIfNotNull(array, "D");
            array = ArrayUtilities.AppendIfNotNull(array, null);
        }
        {
            string[]? array = { "A", "B", "C" };
            ArrayUtilities.Remove(ref array, "B");
        }
        {
            object[]? array = { new object(), new object(), new object() };
            object objectToRemove = array[1];
            ArrayUtilities.Remove(ref array, objectToRemove, comparer: ReferenceEqualityComparer<object>.Instance);
        }

        {
            string[]? array = { "A", "B", "C" };
            ArrayUtilities.Remove(ref array, "A");
            ArrayUtilities.Remove(ref array, "B");
            ArrayUtilities.Remove(ref array, "C", assignNullIfEmptied: true);
        }

        {
            string[]? array = { "A", "B", "C" };
            ArrayUtilities.RemoveAt(ref array, 1);
        }

        {
            string[] array = { "A", "B", "C" };
            array = ArrayUtilities.Without(array, "B");
            array = ArrayUtilities.Without(array, "c", comparer: StringComparer.InvariantCultureIgnoreCase);
        }

        {
            string[] array = { "A", "B", "C" };
            array = ArrayUtilities.WithoutAt(array, 1);
        }

        {
            string[] array = { "A", "B", "C" };
            array = ArrayUtilities.Without(array, "A");
            array = ArrayUtilities.Without(array, "B");
            array = ArrayUtilities.Without(array, "C", assignNullIfEmptied: true);
        }

        {
            string[] array = Array.Empty<string>();
            string[] array_ = (string[])ArrayUtilities.EmptyArray(typeof(string));
        }

        {
            string[] array = { "A", "B", "C", "D", "E", "F" };
            string[] slice = array.Slice(3, 3); // "D", "E", "F"
        }

        {
            string[] array = { "A", "B", "C", "D", "E", "F" };
            string[] subarray = { "D", "E", "F" };
            WriteLine(array.IndexOfSequence(subarray)); // 3
        }

        {
            string[] array = { "A", "B", "C", "D", "E", "F" };
            string[] subarray = { "A", "B", "C" };
            WriteLine(array.StartsWith(subarray)); // True
        }

        {
            int[,] matrix = new int[4, 4];
            matrix[0, 0] = 10;
            matrix = (int[,])ArrayUtilities.ArraySelect(matrix, (int x) => x * x);
            WriteLine(matrix[0, 0]); // 100
        }

        {
            int[,,] matrix = new int[3, 3, 3];
            IEnumerable<int> values = ArrayUtilities.Flatten<int>(matrix);
        }

        {
            int[,] matrix = new int[4, 4];
            foreach ((int[] indices, int value) in ArrayUtilities.VisitArray<int>(matrix))
                WriteLine($"[{string.Join(", ", indices)}] = {value}"); // [0, 0] = 0
        }

        {
            int[,] matrix1 = new int[4, 4];
            int[,] matrix2 = new int[4, 4];
            foreach ((int[] indices, int value1, int value2) in ArrayUtilities.VisitArrays<int>(matrix1, matrix2))
                WriteLine($"[{string.Join(", ", indices)}] = {value1} and {value2}"); // [0, 0] = 0 and 0
        }

        {
            object[] array = { new object(), "Hello", "World", 5, true };
            string[] strings = array.GetInstancesOf<object, string>();
            int[] ints = array.GetInstancesOf<object, int>();
            bool[] bools = array.GetInstancesOf<object, bool>();
            object[] objs = array.GetInstancesOf<object, object>(); // object, "Hello", "World", 5, true
        }
        {
            object[] array = { new object(), "Hello", "World", 5, true };
            string firstString = array.GetFirstInstanceOf<object, string>();     // "Hello"
            int firstInt = array.GetFirstInstanceOf<object, int>();              // 5
            bool firstBool = array.GetFirstInstanceOf<object, bool>();           // true
        }
        {
            object[] array = { new object(), "Hello", "World", 5, true };
            if (array.TryGetFirstInstanceOf(out string firstString)) WriteLine(firstString); // "Hello"
        }

    }
}

