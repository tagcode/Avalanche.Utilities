using System;
using System.Reflection;
using Avalanche.Utilities.Record;
using static System.Console;

class recordcopy
{
    public static void Run()
    {
        // Action<object[], Record>
        {
            // Create description
            IRecordDescription recordDescription = RecordDescription.Create[typeof(MyClass)];
            // Create delegate
            recordDescription.TryCreateRecordCopyAction(out Delegate @delegate);
            // Cast delegate
            Action<MyClass, MyClass> recordCopier = (Action<MyClass, MyClass>)@delegate;
            // Create records
            MyClass src = new MyClass(10), dst = new MyClass(0);
            // Copy record fields
            recordCopier(src, dst);
            // Print value
            WriteLine(dst.value); // 10
        }

        {
            // Create description
            IRecordDescription recordDescription = RecordDescription.Create[typeof(MyClass)];
            // Create delegate            
            Action<MyClass, MyClass> recordCopier = (Action<MyClass, MyClass>)RecordCopyAction.Create[recordDescription];
            // Create records
            MyClass src = new MyClass(10), dst = new MyClass(0);
            // Copy record fields
            recordCopier(src, dst);
            // Print value
            WriteLine(dst.value); // 10
        }
        {
            // Create description
            IRecordDescription recordDescription = RecordDescription.Cached[typeof(MyClass)];
            // Create delegate            
            Action<MyClass, MyClass> recordCopier = (Action<MyClass, MyClass>)RecordCopyAction.Cached[recordDescription];
            // Create records
            MyClass src = new MyClass(10), dst = new MyClass(0);
            // Copy record fields
            recordCopier(src, dst);
            // Print value
            WriteLine(dst.value); // 10
        }
        {
            // Create delegate            
            Action<MyClass, MyClass> recordCopier = (Action<MyClass, MyClass>)RecordCopyAction.CreateFromType[typeof(MyClass)];
            // Create records
            MyClass src = new MyClass(10), dst = new MyClass(0);
            // Copy record fields
            recordCopier(src, dst);
            // Print value
            WriteLine(dst.value); // 10
        }
        {
            // Create delegate            
            Action<MyClass, MyClass> recordCopier = (Action<MyClass, MyClass>)RecordCopyAction.CachedFromType[typeof(MyClass)];
            // Create records
            MyClass src = new MyClass(10), dst = new MyClass(0);
            // Copy record fields
            recordCopier(src, dst);
            // Print value
            WriteLine(dst.value); // 10
        }

        // Action<object, object>
        {
            // Create description
            IRecordDescription recordDescription = RecordDescription.Create[typeof(MyClass)];
            // Create delegate
            recordDescription.TryCreateRecordCopyActionOO(out Action<object, object> recordCopier);
            // Create records
            MyClass src = new MyClass(10), dst = new MyClass(0);
            // Copy record fields
            recordCopier(src, dst);
            // Print value
            WriteLine(dst.value); // 10
        }
        {
            // Create description
            IRecordDescription recordDescription = RecordDescription.Create[typeof(MyClass)];
            // Create delegate            
            Action<object, object> recordCopier = RecordCopyActionOO.Create[recordDescription];
            // Create records
            MyClass src = new MyClass(10), dst = new MyClass(0);
            // Copy record fields
            recordCopier(src, dst);
            // Print value
            WriteLine(dst.value); // 10
        }
        {
            // Create description
            IRecordDescription recordDescription = RecordDescription.Cached[typeof(MyClass)];
            // Create delegate            
            Action<object, object> recordCopier = RecordCopyActionOO.Cached[recordDescription];
            // Create records
            MyClass src = new MyClass(10), dst = new MyClass(0);
            // Copy record fields
            recordCopier(src, dst);
            // Print value
            WriteLine(dst.value); // 10
        }
        {
            // Create delegate            
            Action<object, object> recordCopier = RecordCopyActionOO.CreateFromType[typeof(MyClass)];
            // Create records
            MyClass src = new MyClass(10), dst = new MyClass(0);
            // Copy record fields
            recordCopier(src, dst);
            // Print value
            WriteLine(dst.value); // 10
        }
        {
            // Create delegate            
            Action<object, object> recordCopier = RecordCopyActionOO.CachedFromType[typeof(MyClass)];
            // Create records
            MyClass src = new MyClass(10), dst = new MyClass(0);
            // Copy record fields
            recordCopier(src, dst);
            // Print value
            WriteLine(dst.value); // 10
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
