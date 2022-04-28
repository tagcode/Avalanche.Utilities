using System.Reflection;
using System.Runtime.Serialization;
using Avalanche.Utilities;
using Avalanche.Utilities.Record;

class fielddelegates
{
    public static void Run()
    {
        {
            IFieldDelegates<MyClass, int> fieldDelegates =
                new FieldDelegates<MyClass, int>
                {
                    FieldRead = null,
                    FieldWrite = null,
                    RecreateWith = null,
                    FieldDescription = null
                }.SetReadOnly();
        }
        {
            IFieldDelegates fieldDelegates =
                FieldDelegates
                .Create(typeof(MyClass), typeof(int))
                .SetFieldRead(null)
                .SetFieldWrite(null)
                .SetRecreateWith(null)
                .SetFieldDescription(null)
                .SetReadOnly();
            IFieldDelegates clone = fieldDelegates.Clone().SetReadOnly();
        }
        {
            // Choose field
            FieldInfo fi = typeof(MyClass).GetField("value")!;
            // Create record description
            IRecordDescription recordDescription =
                new RecordDescription()
                .Read(typeof(MyClass))
                .AssignConstructors()
                .ChooseConstruction()
                .SetReadOnly();
            // Create field description
            IFieldDescription fieldDescription =
                recordDescription
                .Fields
                .GetByName("value");
            // Create delegates
            IFieldDelegates<MyClass, int> fieldDelegates =
                fieldDescription
                .CreateFieldDelegates<MyClass, int>()
                .SetReadOnly();
            // Create
            MyClass myClass = new MyClass(0);
            // Write
            fieldDelegates.FieldWrite!(ref myClass, 5);
            // Read
            int value = fieldDelegates.FieldRead!(ref myClass);
            // Recreate new MyClass instance
            fieldDelegates.RecreateWith!(ref myClass, 10);
        }
    }

    public class MyClass
    {
        [DataMember(Order = 1)]
        public int value;

        public MyClass(int value)
        {
            this.value = value;
        }
    }
}
