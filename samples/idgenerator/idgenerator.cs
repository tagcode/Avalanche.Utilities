using Avalanche.Utilities;
using static System.Console;

class idgenerator
{
    public static void Run()
    {
        WriteLine(IdGenerators.Integer.Next); // "0"
        WriteLine(IdGenerators.Integer.Next); // "1"
        WriteLine(IdGenerators.Long.Next); // "0"
        WriteLine(IdGenerators.Guid.Next); // "2623ff82-be3f-4f29-944e-d07d6b6f1df3"
    }
}
