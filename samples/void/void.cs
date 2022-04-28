using System;

class @void
{
    public static void Run()
    {
        Print(new Avalanche.Void());
        Print(Avalanche.Void.Default);
        Print(default(Avalanche.Void));
        Print2(Avalanche.Void.Box);
    }

    public static void Print<T>(T value) => Console.WriteLine(value);

    public static void Print2(object value) => Console.WriteLine(value);
}
