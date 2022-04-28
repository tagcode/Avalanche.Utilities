using System;
using System.Reflection;
using Avalanche.Utilities.Record;
using static System.Console;

class recordcreate
{
    public static void Run()
    {
        // Func<object[], Record>
        {
            // Create description
            IRecordDescription recordDescription = RecordDescription.Create[typeof(MyStruct)];
            // Create delegate
            recordDescription.TryCreateCreateFunc(out Delegate @delegate);
            // Cast delegate
            Func<object[], MyStruct> recordCreate = (Func<object[], MyStruct>)@delegate;
            // Create record
            MyStruct myStruct = recordCreate(new object[] { 10 });
            // Print value
            WriteLine(myStruct.value); // 10
        }

        {
            // Create description
            IRecordDescription recordDescription = RecordDescription.Create[typeof(MyStruct)];
            // Create delegate            
            Func<object[], MyStruct> recordCreate = (Func<object[], MyStruct>)RecordCreateFunc.Create[recordDescription];
            // Create record
            MyStruct myStruct = recordCreate(new object[] { 10 });
            // Print value
            WriteLine(myStruct.value); // 10
        }
        {
            // Create description
            IRecordDescription recordDescription = RecordDescription.Cached[typeof(MyStruct)];
            // Create delegate            
            Func<object[], MyStruct> recordCreate = (Func<object[], MyStruct>)RecordCreateFunc.Cached[recordDescription];
            // Create record
            MyStruct myStruct = recordCreate(new object[] { 10 });
            // Print value
            WriteLine(myStruct.value); // 10
        }
        {
            // Create delegate            
            Func<object[], MyStruct> recordCreate = (Func<object[], MyStruct>)RecordCreateFunc.CreateFromType[typeof(MyStruct)];
            // Create record
            MyStruct myStruct = recordCreate(new object[] { 10 });
            // Print value
            WriteLine(myStruct.value); // 10
        }
        {
            // Create delegate            
            Func<object[], MyStruct> recordCreate = (Func<object[], MyStruct>)RecordCreateFunc.CachedFromType[typeof(MyStruct)];
            // Create record
            MyStruct myStruct = recordCreate(new object[] { 10 });
            // Print value
            WriteLine(myStruct.value); // 10
        }


        // Func<object[], object>
        {
            // Create description
            IRecordDescription recordDescription = RecordDescription.Create[typeof(MyStruct)];
            // Create delegate
            recordDescription.TryCreateCreateFuncOO(out Func<object[], object> recordCreate);
            // Create record
            MyStruct myStruct = (MyStruct)recordCreate(new object[] { 10 });
            // Print value
            WriteLine(myStruct.value); // 10
        }
        {
            // Create description
            IRecordDescription recordDescription = RecordDescription.Create[typeof(MyStruct)];
            // Create delegate
            RecordCreateFunc.TryCreateCreateFunc((IConstructionDescription)recordDescription.Construction!, out Delegate? @delegate, typeof(object));
            // Cast delegate
            Func<object[], object> recordCreate = (Func<object[], object>)@delegate!;
            // Create record
            MyStruct myStruct = (MyStruct)recordCreate!(new object[] { 10 });
            // Print value
            WriteLine(myStruct.value); // 10
        }

        {
            // Create description
            IRecordDescription recordDescription = RecordDescription.Create[typeof(MyStruct)];
            // Create delegate            
            Func<object[], object> recordCreate = RecordCreateFuncOO.Create[recordDescription];
            // Create record
            MyStruct myStruct = (MyStruct)recordCreate(new object[] { 10 });
            // Print value
            WriteLine(myStruct.value); // 10
        }
        {
            // Create description
            IRecordDescription recordDescription = RecordDescription.Cached[typeof(MyStruct)];
            // Create delegate            
            Func<object[], object> recordCreate = RecordCreateFuncOO.Cached[recordDescription];
            // Create record
            MyStruct myStruct = (MyStruct)recordCreate(new object[] { 10 });
            // Print value
            WriteLine(myStruct.value); // 10
        }
        {
            // Create delegate            
            Func<object[], object> recordCreate = RecordCreateFuncOO.CreateFromType[typeof(MyStruct)];
            // Create record
            MyStruct myStruct = (MyStruct)recordCreate(new object[] { 10 });
            // Print value
            WriteLine(myStruct.value); // 10
        }
        {
            // Create delegate            
            Func<object[], object> recordCreate = RecordCreateFuncOO.CachedFromType[typeof(MyStruct)];
            // Create record
            MyStruct myStruct = (MyStruct)recordCreate(new object[] { 10 });
            // Print value
            WriteLine(myStruct.value); // 10
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
