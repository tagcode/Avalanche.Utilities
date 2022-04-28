using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalanche.Utilities;
using static System.Console;

public class lockablelist
{
    public static void Run()
    {
        {
            List<string> list = new List<string>();
            LockableList<string> lockableList = new LockableList<string>(list);
        }
        {
            List<string> list = new List<string>();
            var locakableList = LockableList.Create(typeof(string), list);
        }
        {
            IList list = LockableList.Create(typeof(String));
        }
        {
            // Create list
            var list = new LockableList<string> { "Value1", "Value2" };
            list.SetReadOnly();
            try
            {
                list.Add("Value3");
            }
            catch (InvalidOperationException)
            {
            }
        }
        {
            // Create non-internally synchronized list
            var list = new LockableList<int>();
            // Is underlying map internally synchronized
            WriteLine(list.IsSynchronized); // False
            // Add concurrently
            Parallel.For(0, 10, (int i) => { lock (list.SyncRoot) list.Add(i); });
            // Lock
            list.SetReadOnly();
            // Print values
            WriteLine(string.Join(',', list)); // 8,7,5,0,4,6,9,1,3,2
        }
        {
            // Create internal list (internally synchronized)
            var internalList = new ArrayList<int>();
            // Create wrapper
            var list = new LockableList<int>(internalList);
            // Is underlying map internally synchronized
            WriteLine(list.IsSynchronized); // True
            // Add concurrently
            Parallel.For(0, 10, (int i) => list.Add(i));
            // Lock
            list.SetReadOnly();
            // Print values
            WriteLine(string.Join(',', list)); // 0,1,4,5,3,7,8,2,6,9
        }
    }
}

