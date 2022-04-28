using System;
using Avalanche.Utilities;
using Avalanche.Utilities.Record;

class fieldcloner
{
    public static void Run()
    {
        {
            // Create cloner
            FieldCloner<MyRecord, string> cloner =
                new FieldCloner<MyRecord, string>()
                .SetReader((MyRecord record) => record.Name)
                .SetWriter((MyRecord record, string value) => { record.Name = value; })
                .SetReadOnly();

            // Create records
            MyRecord myRecord1 = new MyRecord { Name = "ABC" };
            MyRecord myRecord2 = new MyRecord();

            // Copy field value
            cloner.CloneField(ref myRecord1, ref myRecord2);
        }
        {
            FieldCloner<MyRecord, string> cloner =
                (FieldCloner<MyRecord, string>)
                FieldCloner.Create(typeof(MyRecord), typeof(string))
                .SetReader((MyRecord record) => record.Name)
                .SetWriter((MyRecord record, string value) => { record.Name = value; })
                .SetReadOnly();
            // Create records
            MyRecord myRecord1 = new MyRecord { Name = "ABC" };
            MyRecord myRecord2 = new MyRecord();

            // Copy field value
            cloner.CloneField(ref myRecord1, ref myRecord2);
        }

        {
            // Create delegate
            Func<MyRecord, string> reader = (MyRecord record) => record.Name;

            // Create cloner
            FieldCloner<MyRecord, string> cloner =
                new FieldCloner<MyRecord, string>()
                .SetReader(reader);
        }

        {
            // Create record description
            IRecordDescription recordDescription = RecordDescription.Cached[typeof(MyRecord)];

            // Create delegate
            FieldRead<MyRecord, string> reader =
                (FieldRead<MyRecord, string>)
                FieldRead.Cached[recordDescription.Fields[0]];

            // Create cloner
            FieldCloner<MyRecord, string> cloner =
                new FieldCloner<MyRecord, string>()
                .SetReader(reader);
        }

        {
            // Create delegate
            Action<MyRecord, string> writer = (MyRecord record, string value) => { record.Name = value; };

            // Create cloner
            FieldCloner<MyRecord, string> cloner =
                new FieldCloner<MyRecord, string>()
                .SetWriter(writer);
        }

        {
            // Create record description
            IRecordDescription recordDescription = RecordDescription.Cached[typeof(MyRecord)];

            // Create delegate
            FieldWrite<MyRecord, string> reader =
                (FieldWrite<MyRecord, string>)
                FieldWrite.Cached[recordDescription.Fields[0]];

            // Create cloner
            FieldCloner<MyRecord, string> cloner =
                new FieldCloner<MyRecord, string>()
                .SetWriter(reader);
        }

        {
            // Create cloner
            FieldCloner<MyRecord, string> cloner =
                new FieldCloner<MyRecord, string>()
                .SetReader((MyRecord record) => record.Name)
                .SetWriter((MyRecord record, string value) => { record.Name = value; })
                .SetCloner(PassthroughCloner<string>.Instance)
                .SetReadOnly();

            // 
            MyRecord myRecord1 = new MyRecord { Name = "ABC" };
            MyRecord myRecord2 = new MyRecord();

            // Copy field value
            cloner.CloneField(ref myRecord1, ref myRecord2);
        }
        {
            // Create cloner
            FieldCloner<MyRecord, string> cloner =
                new FieldCloner<MyRecord, string>()
                .SetReader((MyRecord record) => record.Name)
                .SetWriter((MyRecord record, string value) => { record.Name = value; })
                .SetClonerProvider(ClonerProvider.Cached)
                .SetReadOnly();

            // 
            MyRecord myRecord1 = new MyRecord { Name = "ABC" };
            MyRecord myRecord2 = new MyRecord();

            // Copy field value
            cloner.CloneField(ref myRecord1, ref myRecord2);
        }
    }

    public record MyRecord
    {
        public string Name = null!;
    }

}
