using System;
using Avalanche.Utilities.Provider;
using static System.Console;

class provider_providerbase
{
    public static void Run()
    {
        {
            IProvider<double, double> squareProvider = new SquareProvider();
            WriteLine(squareProvider[10]); // -> "100"
        }
    }


    public class SquareProvider : ProviderBase<double, double>
    {
        public override bool TryGetValue(double x, out double value)
        {
            value = x * x;
            return true;
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
