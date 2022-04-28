using Avalanche.Utilities;

class passthroughcloner
{
    public static void Run()
    {
        {
            // Create cloner
            ICloner<int> cloner = PassthroughCloner<int>.Instance;
            // Passthrough
            int x = cloner.Clone(5);
        }
        {
            // Create cloner
            ICloner<int> cloner =
                (ICloner<int>)
                PassthroughCloner.Create(typeof(int));
            // Passthrough
            int x = cloner.Clone(5);
        }
    }
}
