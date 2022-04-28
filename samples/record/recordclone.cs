using System;
using System.Reflection;
using Avalanche.Utilities.Record;
using static System.Console;

class recordclone
{
    public static void Run()
    {
        // Func<object[], Record>
        {
            // Create description
            IRecordDescription recordDescription = RecordDescription.Create[typeof(MyStruct)];
            // Create delegate
            recordDescription.TryCreateRecordCloneFunc(out Delegate @delegate);
            // Cast delegate
            Func<MyStruct, MyStruct> recordCloner = (Func<MyStruct, MyStruct>)@delegate;
            // Create record
            MyStruct myStruct = new MyStruct(10);
            // Clone record
            MyStruct clone = recordCloner(myStruct);
            // Print value
            WriteLine(clone.value); // 10
        }

        {
            // Create description
            IRecordDescription recordDescription = RecordDescription.Create[typeof(MyStruct)];
            // Create delegate            
            Func<MyStruct, MyStruct> recordCloner = (Func<MyStruct, MyStruct>)RecordCloneFunc.Create[recordDescription];
            // Create record
            MyStruct myStruct = new MyStruct(10);
            // Clone record
            MyStruct clone = recordCloner(myStruct);
            // Print value
            WriteLine(clone.value); // 10
        }
        {
            // Create description
            IRecordDescription recordDescription = RecordDescription.Cached[typeof(MyStruct)];
            // Create delegate            
            Func<MyStruct, MyStruct> recordCloner = (Func<MyStruct, MyStruct>)RecordCloneFunc.Cached[recordDescription];
            // Create record
            MyStruct myStruct = new MyStruct(10);
            // Clone record
            MyStruct clone = recordCloner(myStruct);
            // Print value
            WriteLine(clone.value); // 10
        }
        {
            // Create delegate            
            Func<MyStruct, MyStruct> recordCloner = (Func<MyStruct, MyStruct>)RecordCloneFunc.CreateFromType[typeof(MyStruct)];
            // Create record
            MyStruct myStruct = new MyStruct(10);
            // Clone record
            MyStruct clone = recordCloner(myStruct);
            // Print value
            WriteLine(clone.value); // 10
        }
        {
            // Create delegate            
            Func<MyStruct, MyStruct> recordCloner = (Func<MyStruct, MyStruct>)RecordCloneFunc.CachedFromType[typeof(MyStruct)];
            // Create record
            MyStruct myStruct = new MyStruct(10);
            // Clone record
            MyStruct clone = recordCloner(myStruct);
            // Print value
            WriteLine(clone.value); // 10
        }

        // Func<object, object>
        {
            // Create description
            IRecordDescription recordDescription = RecordDescription.Create[typeof(MyStruct)];
            // Create delegate
            recordDescription.TryCreateRecordCloneFuncOO(out Func<object, object> recordCloner);
            // Create record
            MyStruct myStruct = new MyStruct(10);
            // Clone record
            MyStruct clone = (MyStruct)recordCloner(myStruct);
            // Print value
            WriteLine(clone.value); // 10
        }
        {
            // Create description
            IRecordDescription recordDescription = RecordDescription.Create[typeof(MyStruct)];
            // Create delegate            
            Func<object, object> recordCloner = RecordCloneFuncOO.Create[recordDescription];
            // Create record
            MyStruct myStruct = new MyStruct(10);
            // Clone record
            MyStruct clone = (MyStruct)recordCloner(myStruct);
            // Print value
            WriteLine(clone.value); // 10
        }
        {
            // Create description
            IRecordDescription recordDescription = RecordDescription.Cached[typeof(MyStruct)];
            // Create delegate            
            Func<object, object> recordCloner = RecordCloneFuncOO.Cached[recordDescription];
            // Create record
            MyStruct myStruct = new MyStruct(10);
            // Clone record
            MyStruct clone = (MyStruct)recordCloner(myStruct);
            // Print value
            WriteLine(clone.value); // 10
        }
        {
            // Create delegate            
            Func<object, object> recordCloner = RecordCloneFuncOO.CreateFromType[typeof(MyStruct)];
            // Create record
            MyStruct myStruct = new MyStruct(10);
            // Clone record
            MyStruct clone = (MyStruct)recordCloner(myStruct);
            // Print value
            WriteLine(clone.value); // 10
        }
        {
            // Create delegate            
            Func<object, object> recordCloner = RecordCloneFuncOO.CachedFromType[typeof(MyStruct)];
            // Create record
            MyStruct myStruct = new MyStruct(10);
            // Clone record
            MyStruct clone = (MyStruct)recordCloner(myStruct);
            // Print value
            WriteLine(clone.value); // 10
        }
    }
    public struct MyStruct
    {
        public int value;

        public MyStruct(int value)
        {
            this.value = value;
        }
    }
}
