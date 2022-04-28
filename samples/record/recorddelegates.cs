using Avalanche.Utilities;
using Avalanche.Utilities.Record;
using Avalanche.Utilities.Provider;

class recorddelegates
{
    public static void Run()
    {
        {
            IRecordDelegates<MyClass> recordDelegates =
                new RecordDelegates<MyClass>
                {
                    RecordCreate = null,
                    FieldDelegates = new IFieldDelegates[0],
                    RecordDescription = null
                }.SetReadOnly();
        }
        {
            IRecordDelegates recordDelegates =
                RecordDelegates
                .Create(typeof(MyClass))
                .SetRecordCreate(null)
                .SetFieldDelegates(null)
                .SetRecordDescription(null)
                .SetReadOnly();
            IRecordDelegates clone = recordDelegates.Clone().SetReadOnly();
        }
        {
            // Read description
            IRecordDescription recordDescription = new RecordDescription()
                .Read(typeof(MyClass))
                .AssignConstructors()
                .ChooseConstruction()
                .SetReadOnly();
            IRecordDelegates<MyClass> recordDelegates = recordDescription.CreateRecordDelegates<MyClass>();
        }
        {
            IRecordDelegates<MyClass> recordDelegates = RecordProviders.Cached.GetRecordDelegates<MyClass>().AssertValue();
            MyClass myClass = recordDelegates.RecordCreate!(new object[] { 5 });
            FieldRead<MyClass, int> reader =
                (FieldRead<MyClass, int>)
                recordDelegates
                .FieldDelegates!
                .GetByName("value")
                .FieldRead!;
            // Read
            int value = reader(ref myClass);
        }
    }

    public class MyClass
    {
        public int value;

        public MyClass(int value)
        {
            this.value = value;
        }
    }
}
