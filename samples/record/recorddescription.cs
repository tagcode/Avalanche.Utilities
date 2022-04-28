using System.Runtime.Serialization;
using Avalanche.Utilities;
using Avalanche.Utilities.Record;

class recorddescription
{
    public static void Run()
    {
        {
            IRecordDescription recordDescription = new RecordDescription
            {
                Name = "",
                Type = typeof(MyClass),
                Constructors = new IConstructorDescription[0],
                Deconstructor = null,
                Fields = new IFieldDescription[0],
                Annotations = new object[0],
                Construction = null
            }.SetReadOnly();
        }
        {
            IRecordDescription recordDescription =
                new RecordDescription()
                .SetName("")
                .SetType(typeof(MyClass))
                .SetConstructors(new IConstructorDescription[0])
                .SetDeconstructor(null)
                .SetFields(new IFieldDescription[0])
                .SetAnnotations(new object[0])
                .SetConstruction(null)
                .SetReadOnly();
            IRecordDescription clone = recordDescription.Clone().SetReadOnly();
        }
        {
            // Read info
            IRecordDescription recordDescription = new RecordDescription()
                .Read(typeof(MyClass))
                .AssignConstructors()
                .ChooseConstruction()
                .SetReadOnly();
        }
        {
            IRecordDescription recordDescription = RecordDescription.Create[typeof(MyClass)];
        }
        {
            IRecordDescription recordDescription = RecordDescription.Cached[typeof(MyClass)];
        }
    }

    public class MyClass
    {
        [IgnoreDataMember]
        public bool HasCache => false;

        [DataMember(Order = 1)]
        public readonly int value;

        public MyClass(int value)
        {
            this.value = value;
        }
    }
}
