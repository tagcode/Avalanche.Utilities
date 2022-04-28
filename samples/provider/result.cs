using System;
using System.Xml.Linq;
using Avalanche.Utilities.Provider;
using static System.Console;

class provider_result
{
    public static void Run()
    {
        {
            IProvider<double, double> squareProvider = Providers.Func((double x) => x * x);
            IProvider<double, IResult<double>> squareResultProvider = squareProvider.ResultCaptured();
            WriteLine(squareResultProvider[10].Value); // -> "100"
        }
        {
            IProvider<double, double> squareProvider = Providers.Func((double x) => x * x);
            IProvider<double, IResult<double>> squareResultProvider = squareProvider.ResultCaptured();
            IProvider<double, IResult<double>> squareCachedResultProvider = squareResultProvider.Cached();
            IProvider<double, double> squareCachedProvider = squareCachedResultProvider.ResultOpened();
            WriteLine(squareCachedProvider[10]); // -> "100"
        }
        {
            IProvider<string, XDocument> docProvider =
                Providers.Func<string, XDocument>(XDocument.Load)
                .ResultCaptured() // Captures exception
                .Cached()         // ConcurrentDictionary
                .ResultOpened();  // Rethrows exception

            XDocument doc = docProvider["http://avalanche.fi/sitemap.xml"];
        }
        {
            IProvider<string, XDocument> docProvider =
                Providers.Func<string, XDocument>(XDocument.Load)
                .ValueResultCaptured() // Captures exception
                .Cached()              // ConcurrentDictionary
                .ValueResultOpened();  // Rethrows exception

            XDocument doc = docProvider["http://avalanche.fi/sitemap.xml"];
        }

        {
            // Integer printer, cached
            IProvider<int, string> intPrinter =
                Providers.Func<int, string>(i => i.ToString())
                .ResultCaptured() // Captures exception
                .Cached()         // ConcurrentDictionary
                .ResultOpened();  // Rethrows exception

            // Print integers to strings
            string print1 = intPrinter[10];
            string print2 = intPrinter[10];
            // Compare object reference
            WriteLine(object.ReferenceEquals(print1, print2)); // "True"

            // Invalidate cache
            intPrinter.InvalidateCache(deep: true);
            // Compare object reference
            WriteLine(object.ReferenceEquals(print1, intPrinter[10])); // "False"
        }

        {
            try
            {
                IProvider<double, IResult<double>> squareResultProvider = new SquareResultProvider();
                WriteLine(squareResultProvider[10].Value); // -> "100"
                IResult<double> result = squareResultProvider[double.NaN];
                WriteLine(result.Error); // -> "ArgumentException"
                IResult<double> result_ = squareResultProvider[10];
                WriteLine(squareResultProvider[10].AssertValue()); // -> "100"
                WriteLine(squareResultProvider[double.NaN].AssertValue()); // -> "100"
            }
            catch (Exception) { }
        }
    }

    public class SquareResultProvider : ResultProviderBase<double, double>
    {
        protected override bool TryGetValue(double x, out double value)
        {
            // NaN
            if (double.IsNaN(x)) throw new ArgumentException("NaN");
            // Infinity
            if (double.IsInfinity(x)) throw new ArgumentException("Infinity");
            // Assign value
            value = x * x;
            // Return
            return true;
        }
    }
}
