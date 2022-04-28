using System.Runtime.Serialization;
using Avalanche.Utilities.Record;
using Avalanche.Utilities.Provider;

class recordproviders
{
    public static void Run()
    {
        {
            IRecordProviders recordDescriptionProvider = RecordProviders.Create;
        }
        {
            IRecordProviders recordDescriptionProvider = RecordProviders.NewCached();
        }
        {
            IRecordProviders recordDescriptionProvider = RecordProviders.Cached;
            IRecordDescription recordDescription =
                recordDescriptionProvider
                .RecordDescription[typeof(MyClass)]
                .AssertValue();
            IRecordDelegates recordDelegates =
                recordDescriptionProvider
                .RecordDelegates[recordDescription]
                .AssertValue();
            IRecordDelegates recordDelegates2 =
                recordDescriptionProvider
                .RecordDelegatesByType[typeof(MyClass)]
                .AssertValue();
            IFieldDelegates fieldDelegates =
                recordDescriptionProvider
                .FieldDelegates[recordDescription.Fields[0]]
                .AssertValue();
            FieldRead<MyClass, int>? reader =
                recordDescriptionProvider
                .FieldDelegates[recordDescription.Fields[0]].GetFieldRead()
                .AssertValue()
                as FieldRead<MyClass, int>;
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
