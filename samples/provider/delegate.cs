using Avalanche.Utilities.Provider;
using static System.Console;

class provider_delegate
{
    public static void Run()
    {
        {
            IProvider<double, double> squareProvider = Providers.Func((double x) => x * x);
            WriteLine(squareProvider[10]); // -> "100"
        }
        {
            IProvider<double, double> squareProvider = Providers.Func<double, double>(TrySquare);
            WriteLine(squareProvider[10]); // -> "100"
        }
        {
            IProvider<double, double> provider = Providers.Func((double x) => x * x);
            provider = provider.ValueFunc(x => x * 2.0);
            WriteLine(provider[10]); // -> "200"
        }
    }


    public static bool TrySquare(double x, out double result) { result = x * x; return true; }

}
