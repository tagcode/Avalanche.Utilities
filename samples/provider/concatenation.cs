using System;
using System.Collections.Generic;
using System.Linq;
using Avalanche.Utilities.Provider;
using static System.Console;

class provider_concatenation
{
    public static void Run()
    {
        {
            IProvider<double, double> squareProvider = Providers.Func((double x) => x * x);
            IProvider<double, double> squareRootProvider = Providers.Func((double x) => Math.Sqrt(x));
            IProvider<double, double> provider = squareProvider.Concat(squareRootProvider);
            WriteLine(provider[10]); // -> "10"
        }
        {
            // 0,1,2,3,... provider
            IProvider<int, IEnumerable<int>> indicesProvider = Providers.Func<int, IEnumerable<int>>(count => Enumerable.Range(0, count));
            // 10,10,10,10,,... provider
            IProvider<int, IEnumerable<int>> repeatProvider = Providers.Func<int, IEnumerable<int>>(value => Enumerable.Repeat(value, 5));
            //
            IProvider<int, IEnumerable<int>>[] providers = { indicesProvider, repeatProvider };
            // Decorate to concat results
            IProvider<int, IEnumerable<int>> concatProvider = Providers.EnumerableConcat<int, int>(providers, distinctValues: false);
            // "0, 1, 2, 3, 3, 3, 3, 3"
            WriteLine(string.Join(", ", concatProvider[3]));
        }
    }
}
