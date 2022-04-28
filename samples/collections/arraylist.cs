using System.Collections.Generic;
using System.Threading.Tasks;
using Avalanche.Utilities;
using static System.Console;

public class arraylist
{
    public static void Run()
    {
        {
            ArrayList<int> arrayList = new ArrayList<int>();
        }

        {
            ArrayList<int> arrayList = (ArrayList<int>)ArrayListT.Create(typeof(int));
        }

        {
            // Create list
            ArrayList<int> arrayList = new ArrayList<int>();
            // Add elements
            arrayList.Add(1);
            arrayList.Add(2);
            arrayList.Add(3);
            // Get snapshot
            int[] array = arrayList.Array;
            // Modify
            arrayList.Add(5);
            // Get another snapshot
            int[] array2 = arrayList.Array;
            //
            WriteLine(array == array2); // false, different reference
        }

        {
            // Create comparer
            IComparer<string> comparer = AlphaNumericComparer.InvariantCultureIgnoreCase;
            // Create list
            ArrayList<string> arrayList = new ArrayList<string>.Sorted(comparer);
            // Add elements
            arrayList.Add("A1");
            arrayList.Add("A2");
            arrayList.Add("A100");
            arrayList.Add("A200");
            // Get snapshot
            string[] array = arrayList.Array;
            // Print
            foreach (string line in array) WriteLine(line); // A1, A2, A100, A200
        }
        {
            ArrayList<int> list = new ArrayList<int>();
            Task t1 = Task.Run(() => list.Add(4));
            Task t2 = Task.Run(() => list.Add(5));
            Task.WaitAll(t1, t2);
            foreach (var line in list.Array) WriteLine(line); // 4, 5
        }
    }
}

