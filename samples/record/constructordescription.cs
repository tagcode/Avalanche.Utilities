using System;
using Avalanche.Utilities;
using Avalanche.Utilities.Record;

class constructordescription
{
    public static void Run()
    {
        {
            IConstructorDescription constructorDescription = new ConstructorDescription
            {
                Constructor = null!,
                Type = typeof(MyClass),
                Parameters = new IParameterDescription[0],
                Annotations = new object[0],
                Record = null
            }.SetReadOnly();
        }
        {
            IConstructorDescription constructorDescription =
                new ConstructorDescription()
                .SetConstructor(null!)
                .SetType(typeof(MyClass))
                .SetParameters(new IParameterDescription[0])
                .SetAnnotations(new object[0])
                .SetRecord(null)
                .SetReadOnly();
            IConstructorDescription clone = constructorDescription.Clone().SetReadOnly();
        }

        {
            // Create delegate ctor
            Func<int, MyClass> ctor = (int value) => new MyClass(value);
            // Read info into description
            IRecordDescription recordDescription = new RecordDescription().Read(typeof(MyClass));
            // Create constructor description from delegate
            IConstructorDescription constructorDescription = new ConstructorDescription().SetRecord(recordDescription).Read(ctor);
        }
    }
    public class MyClass
    {
        public readonly int value;

        public MyClass(int value)
        {
            this.value = value;
        }
    }
}
