using System;
using System.Collections;
using System.Collections.Generic;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

class defaultcomparerprovider
{
    public static void Run()
    {
        {
            IComparer<string[]> comparer =
                (IComparer<string[]>)
                DefaultComparerProvider.Create[typeof(string[])];
        }
        {
            IComparer<string[]> comparer =
                (IComparer<string[]>)
                DefaultComparerProvider.Cached[typeof(string[])];
        }

        {
            IProvider<Type, IEqualityComparer> comparerProvider = DefaultEqualityComparerProvider.Create;
            IProvider<Type, IEqualityComparer> comparerProvider2 = DefaultEqualityComparerProvider.Cached;
        }
        {
            IEqualityComparer<string[]> comparer =
                (IEqualityComparer<string[]>)
                DefaultEqualityComparerProvider.Create[typeof(string[])];
        }
        {
            IEqualityComparer<string[]> comparer =
                (IEqualityComparer<string[]>)
                DefaultEqualityComparerProvider.Cached[typeof(string[])];
        }
    }
}
