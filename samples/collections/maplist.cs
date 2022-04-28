using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalanche.Utilities;
using static System.Console;

public class maplist
{
    public static void Run()
    {
        {
            var list = new MapList<string, int>();
        }
        {
            // Create maplist with comparer
            var list = new MapList<string, int>(StringComparer.InvariantCultureIgnoreCase);
            // Add to key
            list.Add("key", 10);
            list.Add("KEY", 11);
            list.Add("keY", 12);
            // Print values
            foreach (int value in list["KeY"]) WriteLine(value);
        }
        {
            var list =
                (MapList<string, int>)
                MapList.Create(typeof(string), typeof(int));
        }
        {
            var list =
                (MapList<string, int>)
                MapList.Create(typeof(string), typeof(int), StringComparer.InvariantCultureIgnoreCase);
            list.Add("key", 10);
            list.Add("KEY", 11);
            list.Add("keY", 12);

            // Print values
            foreach (int value in list["KeY"]) WriteLine(value);
        }
        {
            var list = new MapList<string, int>
            {
                { "A", 01 },
                { "A", 02 },
                { "A", 03 },
                { "B", 11 },
                { "B", 12 },
                { "B", 13 },
            };
            list.SetReadOnly();
            try
            {
                list.Add("A", 04);
            }
            catch (InvalidOperationException)
            {
            }
        }
        {
            // Create list
            var list = new MapList<string, int>();
            // Add elements
            list.Add("A", 01);
            list.Add("A", 02);
            list.Add("A", 03);
            // Get values
            IList<int> values = list["A"];
            // Print values
            foreach (int value in values) WriteLine(value);
        }
        {
            // Create list
            var list = new MapList<int, int>();
            // Run 50 tasks in parallel.
            Parallel.For(0, 50,
                (int i) => { lock (list.SyncRoot) list.Add(i % 5, i); }
            );
            // Print values
            foreach (var kv in list) WriteLine($"{kv.Key},{string.Join(',', kv.Value)}");
            // 2,7,37,42,47,32,27,22,17,12,2
            // 3,33,38,43,48,8,3,28,23,18,13
            // 4,34,39,44,49,9,29,24,19,14,4
            // 0,35,40,45,0,10,5,30,25,20,15
            // 1,36,41,46,1,11,31,26,21,16,6
        }
    }
}

