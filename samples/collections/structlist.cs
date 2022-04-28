using System;
using System.Collections.Generic;
using Avalanche.Utilities;
using static System.Console;

public class structlist
{
    // Rename to "Main", or run from Main.
    public static void Run()
    {
        {
            StructList4<int> list = new StructList4<int>();
            list.Add(5);  // 1st: Allocated on stack
            list.Add(2);  // 2nd:           .. stack
            list.Add(3);  // 3rd:           .. stack
            list.Add(8);  // 4th:           .. stack
            list.Add(10); // 5th: Allocated in heap
        }

        {
            StructList4<int> list = new StructList4<int>();
            int[] array = list.ToArray();
            int[] arrayReverse = list.ToReverseArray();
        }

        {
            StructList10<int> list = new StructList10<int>() { 5, 2, 3, 8 };
            var sorter = new StructListSorter<StructList10<int>, int>(Comparer<int>.Default);
            sorter.Sort(ref list);
        }

        {
            StructList10<int> list = new();
            list.Add(1);
            Process(ref list);
            WriteLine(String.Join(", ", list.ToArray())); // 1, 10, 11, 12
        }


        {
            StructList10<int> list = new StructList10<int>() { 2, 5, 7, 8, 10 };
            var sorter = new StructListSorter<StructList10<int>, int>(Comparer<int>.Default);
            WriteLine(sorter.BinarySearch(ref list, 8)); //3 
        }


        {
            // Allocate list on stack
            StructList1<int> list = new StructList1<int>() { 2, 5, 7, 8, 10 };
            // Get pointer to element [0]
            ref int pointer = ref StructList1<int>.GetRef(ref list, 0);
            // Read from pointer
            WriteLine(pointer); // 2
            // Assign value '4' at pointer 
            pointer = 4;
            // Read from list (value is changed)
            WriteLine(list[0]); // 4
            pointer = ref StructList1<int>.GetRef(ref list, 4);
            WriteLine(pointer); // 10
            pointer = 4;
            WriteLine(list[4]); // 4
        }

    }

    public static void Process<List>(ref List list) where List : IList<int>
    {
        list.Add(10);
        list.Add(12);
        list.Add(13);
    }
}

