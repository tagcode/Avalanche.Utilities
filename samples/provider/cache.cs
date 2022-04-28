using System.Collections.Generic;
using System.Linq;
using Avalanche.Utilities.Provider;
using static System.Console;

class provider_cache
{
    public static void Run()
    {
        {
            IProvider<double, double> squareProvider = Providers.Func((double x) => x * x);
            IProvider<double, IResult<double>> squareResultProvider = squareProvider.ResultCaptured();
            IProvider<double, IResult<double>> squareCachedResultProvider = squareResultProvider.Cached();
        }
        {
            IProvider<IList<int>, int> sumProvider = Providers.Func((IList<int> list) => list.Sum());
            IProvider<IList<int>, IResult<int>> sumResultProvider = sumProvider.ResultCaptured();
            IProvider<IList<int>, IResult<int>> sumCachedResultProvider = sumResultProvider.WeakCached();
            IProvider<IList<int>, int> sumCachedProvider = sumCachedResultProvider.ResultOpened();

            int[] list = { 1, 2, 3, 4, 5, 6 };
            WriteLine(sumCachedProvider[list]); // -> "21"
        }

        {
            // Create provider that prints integers and caches between -100..100
            IProvider<int, string> intPrinter =
                Providers.Func<int, string>(i => i.ToString())
                .Cached(toCacheCriteria: i => i >= -100 && i <= 100);

            // Print integer
            WriteLine(intPrinter[10]); // "10"
            // Reference equals
            WriteLine(object.ReferenceEquals(intPrinter[100], intPrinter[100])); // "True" same string reference from cache
            WriteLine(object.ReferenceEquals(intPrinter[1000], intPrinter[1000])); // "False" different string reference. Not from cache.
        }
        {
            IProvider<IList<int>, object> sumProvider = Providers.Func<IList<int>, object>((IList<int> list) => (object)list.Sum())
                .WeakCached<IList<int>, object>(toCacheCriteria: list => list.Count <= 3);

            int[] list1 = { 1, 2, 3 };
            int[] list2 = { 1, 2, 3, 4, 5, 6 };
            WriteLine(sumProvider[list1]); // -> "6"

            // Reference equals
            WriteLine(object.ReferenceEquals(sumProvider[list1], sumProvider[list1])); // "True" same sum reference from cache
            WriteLine(object.ReferenceEquals(sumProvider[list2], sumProvider[list2])); // "False" different sum reference. Not from cache.
        }
        {
#pragma warning disable CS8632
            IProvider<string?, string?> suffixer = Providers.Func((string? name) => name == null ? null : name + " World");
            IProvider<string?, string?> suffixerCached = suffixer.CachedNullableKey();
            WriteLine(suffixerCached[null]); // null
            WriteLine(suffixerCached["Hello"]); // "Hello World"
#pragma warning restore CS8632
        }
        {
            // Integer printer, cached
            IProvider<int, string> intPrinter = Providers.Func<int, string>(i => i.ToString()).Cached();

            // Print integers to strings
            string print1 = intPrinter[10];
            string print2 = intPrinter[10];
            // Compare object reference
            WriteLine(object.ReferenceEquals(print1, print2)); // "True"

            // Invalidate cache
            intPrinter.InvalidateCache(deep: false);

            // Print again
            string print3 = intPrinter[10];
            // Compare object reference
            WriteLine(object.ReferenceEquals(print1, print3)); // "False"
        }

    }
}
