using System;
using Avalanche.Utilities;

public class tupleutilities
{
    public static void Run()
    {
        {
            Type tupleType = TupleUtilities.CreateValueTupleType(typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int));
            Console.WriteLine(tupleType);
            // System.ValueTuple`8[System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.ValueTuple`5[System.Int32,System.Int32,System.Int32,System.Int32,System.Int32]]
            Type[] parameterTypes = TupleUtilities.GetParameterTypes(tupleType);
            foreach (var type in parameterTypes)
                Console.WriteLine(type);
            Type[] tupleTypes = TupleUtilities.GetTupleTypes(tupleType);
            foreach (var type in tupleTypes)
                Console.WriteLine(type);
        }
    }
}

