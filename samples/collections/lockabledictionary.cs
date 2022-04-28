using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalanche.Utilities;
using static System.Console;

public class lockabledictionary
{
    public static void Run()
    {
        {
            // Create dictionary
            IDictionary<string, string> map = new Dictionary<string, string>
            {
                { "Key1", "Value1" },
                { "Key2", "Value2" },
            };
            // Wraps into lockable dictionary
            var wrapedMap = new LockableDictionary<string, string>(map);
        }
        {
            LockableDictionary<string, string> map =
                (LockableDictionary<string, string>)
                LockableDictionary.Create(typeof(string), typeof(string));
            map["A"] = "B";
        }
        {
            // Create map
            var map = new LockableDictionary<string, string>
            {
                { "Key1", "Value1" },
                { "Key2", "Value2" },
            };
            // Lock
            map.SetReadOnly();
            // Write
            try
            {
                map["Key3"] = "Value3";
            }
            catch (InvalidOperationException)
            {
            }
        }
        {
            // Create map
            var map = new LockableDictionaryRecord<string, string>
            {
                { "Key1", "Value1" },
                { "Key2", "Value2" },
            };
        }
        {
            // Create internal map
            var internalMap = new Dictionary<int, int>();
            // Create map
            var map = new LockableDictionary<int, int>(internalMap);
            // Is underlying map internally synchronized
            WriteLine(map.IsSynchronized); // False
            // Run 50 tasks in parallel.
            Parallel.For(0, 50,
                (int i) => { lock (map.SyncRoot) map[i % 5] = i; }
            );
            // Print values
            foreach (var kv in map) WriteLine($"{kv.Key}={string.Join(',', kv.Value)}");
            // 0 = 15
            // 1 = 6
            // 3 = 8
            // 2 = 2
            // 4 = 4
        }
        {
            // Create internal map
            var internalMap = new ConcurrentDictionary<int, int>();
            // Create map
            var map = new LockableDictionary<int, int>(internalMap);
            // Is underlying map internally synchronized
            WriteLine(map.IsSynchronized); // true
            // Run 50 tasks in parallel.
            Parallel.For(0, 50, (int i) => map[i % 5] = i);
            // Print values
            foreach (var kv in map) WriteLine($"{kv.Key}={string.Join(',', kv.Value)}");
            // 0 = 30
            // 1 = 31
            // 2 = 32
            // 3 = 43
            // 4 = 29
        }
    }
}

