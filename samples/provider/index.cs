using Avalanche.Utilities.Provider;
using static System.Console;

class provider_index
{
    public static void Run()
    {
        {
            // Create provider
            IProvider<double, double> squareProvider = Providers.Func<double, double>(d => d * d);
            // Use with indexer[value]
            WriteLine(squareProvider[10.0]); // -> "100"
        }
        {
            // Create provider
            IProvider<double, double> squareProvider = Providers.Func<double, double>(d => d * d);
            // Try Get value
            if (squareProvider.TryGetValue(10.0, out double squared))
                WriteLine(squared); // -> "100"
        }
        {
            // Create provider
            IProvider squareProvider = Providers.Func<double, double>(d => d * d);
            //
            object input = 10.0;
            // Try Get value
            if (squareProvider.TryGetValue(input, out object squaredObject))
                WriteLine(squaredObject); // -> "100"
        }

    }


}
