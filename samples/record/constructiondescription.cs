using System;
using Avalanche.Utilities;
using Avalanche.Utilities.Record;

class constructiondescription
{
    public static void Run()
    {
        {
            IConstructionDescription constructionDescription = new ConstructionDescription()
            {
                Constructor = null!,
                Parameters = new IParameterDescription[0],
                Fields = new IFieldDescription[0],
                ParameterToField = null!,
                FieldToParameter = null!,
                UnmatchedParameters = new IParameterDescription[0],
                UnmatchedFields = new IFieldDescription[0]
            }.SetReadOnly();
        }
        {
            IConstructionDescription constructionDescription =
                new ConstructionDescription()
                .SetConstructor(null!)
                .SetParameters(new IParameterDescription[0])
                .SetFields(new IFieldDescription[0])
                .SetUnmatchedParameters(new IParameterDescription[0])
                .SetUnmatchedFields(new IFieldDescription[0])
                .SetReadOnly();
            IConstructionDescription clone = constructionDescription.Clone().SetReadOnly();
        }
        {
            // Create delegate ctor
            Func<int, MyClass> ctor = (int value) => new MyClass(value);
            //
            IRecordDescription recordDescription = new RecordDescription().Read(typeof(MyClass));
            // Create constructor description from delegate
            IConstructorDescription constructorDescription = new ConstructorDescription().SetRecord(recordDescription).Read(ctor);
            // Create construction description
            IConstructionDescription constructionDescription = constructorDescription.CreateConstructionDescription();
            int score = constructionDescription.Score();
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
