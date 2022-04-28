using System;
using System.Collections.Generic;
using Avalanche.Utilities;

class referencecomparer
{
    public static void Run()
    {
        {
            object o1 = new object(), o2 = new object();
            object[] objs = new[] { o1, o2 };
            IComparer<object> comparer = ReferenceComparer<object>.Instance;
            Array.Sort(objs, comparer);
            Print(objs);
        }
        {
            object o1 = new object(), o2 = new object();
            object[] objs = new[] { o1, o2 };
            IComparer<object> comparer =
                (IComparer<object>)
                ReferenceComparer.Create(typeof(object));
            Array.Sort(objs, comparer);
            Print(objs);
        }
        {
            object o1 = new object(), o2 = new object();
            IEqualityComparer<object> comparer =
                ReferenceEqualityComparer<object>.Instance;
            bool equal = comparer.Equals(o1, o2);
        }
        {
            object o1 = new object(), o2 = new object();
            IEqualityComparer<object> comparer =
                (IEqualityComparer<object>)
                Avalanche.Utilities.ReferenceEqualityComparer.Create(typeof(object));
            bool equal = comparer.Equals(o1, o2);
        }
    }

    static void Print<T>(IEnumerable<T> enumr) => Console.WriteLine(String.Join(", ", enumr));
}
