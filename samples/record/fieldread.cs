using System;
using System.Reflection;
using Avalanche.Utilities.Record;
using static System.Console;

class fieldread
{
    public static void Run()
    {
        // FieldRead<Record, Field>
        {
            // Get field reference
            FieldInfo fi = typeof(MyStruct).GetField(nameof(MyStruct.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.Create[fi];
            // Create delegate
            fieldDescription.TryCreateFieldReadDelegate(out Delegate @delegate);
            // Cast delegate
            FieldRead<MyStruct, int> fieldRead = (FieldRead<MyStruct, int>)@delegate;
            // Create struct
            MyStruct myStruct = new MyStruct(10);
            // Read field
            int value = fieldRead(ref myStruct);
            // Print value
            WriteLine(value); // 10
        }

        {
            // Get field reference
            FieldInfo fi = typeof(MyStruct).GetField(nameof(MyStruct.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.Cached[fi];
            // Create delegate
            FieldRead<MyStruct, int> fieldRead = (FieldRead<MyStruct, int>)FieldRead.Create[fieldDescription];
            // Create struct
            MyStruct myStruct = new MyStruct(10);
            // Read field
            int value = fieldRead(ref myStruct);
            // Print value
            WriteLine(value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyStruct).GetField(nameof(MyStruct.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.Cached[fi];
            // Get (weak key) cached delegate
            FieldRead<MyStruct, int> fieldRead = (FieldRead<MyStruct, int>)FieldRead.Cached[fieldDescription];
            // Create struct
            MyStruct myStruct = new MyStruct(10);
            // Read field
            int value = fieldRead(ref myStruct);
            // Print value
            WriteLine(value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyStruct).GetField(nameof(MyStruct.value))!;
            // Create delegate
            FieldRead<MyStruct, int> fieldRead = (FieldRead<MyStruct, int>)FieldRead.CreateFromObject[fi];
            // Create struct
            MyStruct myStruct = new MyStruct(10);
            // Read field
            int value = fieldRead(ref myStruct);
            // Print value
            WriteLine(value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyStruct).GetField(nameof(MyStruct.value))!;
            // Get (weak key) cached delegate
            FieldRead<MyStruct, int> fieldRead = (FieldRead<MyStruct, int>)FieldRead.CachedFromObject[fi];
            // Create struct
            MyStruct myStruct = new MyStruct(10);
            // Read field
            int value = fieldRead(ref myStruct);
            // Print value
            WriteLine(value); // 10
        }


        // Func<Record, Field>
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.Create[fi];
            // Create delegate
            fieldDescription.TryCreateFieldReadFunc(out Delegate @delegate);
            // Cast delegate
            Func<MyClass, int> fieldRead = (Func<MyClass, int>)@delegate;
            // Create class
            MyClass myClass = new MyClass(10);
            // Read field
            int value = fieldRead(myClass);
            // Print value
            WriteLine(value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.Cached[fi];
            // Create delegate
            Func<MyClass, int> fieldRead = (Func<MyClass, int>)FieldReadFunc.Create[fieldDescription];
            // Create class
            MyClass myClass = new MyClass(10);
            // Read field
            int value = fieldRead(myClass);
            // Print value
            WriteLine(value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.Cached[fi];
            // Get (weak key) cached delegate
            Func<MyClass, int> fieldRead = (Func<MyClass, int>)FieldReadFunc.Cached[fieldDescription];
            // Create Class
            MyClass myClass = new MyClass(10);
            // Read field
            int value = fieldRead(myClass);
            // Print value
            WriteLine(value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Create delegate
            Func<MyClass, int> fieldRead = (Func<MyClass, int>)FieldReadFunc.CreateFromObject[fi];
            // Create Class
            MyClass myClass = new MyClass(10);
            // Read field
            int value = fieldRead(myClass);
            // Print value
            WriteLine(value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Get (weak key) cached delegate
            Func<MyClass, int> fieldRead = (Func<MyClass, int>)FieldReadFunc.CachedFromObject[fi];
            // Create Class
            MyClass myClass = new MyClass(10);
            // Read field
            int value = fieldRead(myClass);
            // Print value
            WriteLine(value); // 10
        }



        // Func<object, object>
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.Create[fi];
            // Create delegate
            fieldDescription.TryCreateFieldReadFuncOO(out Func<object, object> fieldRead);
            // Create class
            MyClass myClass = new MyClass(10);
            // Read field
            int value = (int)fieldRead(myClass);
            // Print value
            WriteLine(value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.Cached[fi];
            // Create delegate
            Func<object, object> fieldRead = FieldReadFuncOO.Create[fieldDescription];
            // Create class
            MyClass myClass = new MyClass(10);
            // Read field
            int value = (int)fieldRead(myClass);
            // Print value
            WriteLine(value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.Cached[fi];
            // Get (weak key) cached delegate
            Func<object, object> fieldRead = FieldReadFuncOO.Cached[fieldDescription];
            // Create Class
            MyClass myClass = new MyClass(10);
            // Read field
            int value = (int)fieldRead(myClass);
            // Print value
            WriteLine(value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Create delegate
            Func<object, object> fieldRead = FieldReadFuncOO.CreateFromObject[fi];
            // Create Class
            MyClass myClass = new MyClass(10);
            // Read field
            int value = (int)fieldRead(myClass);
            // Print value
            WriteLine(value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Get (weak key) cached delegate
            Func<object, object> fieldRead = FieldReadFuncOO.CachedFromObject[fi];
            // Create Class
            MyClass myClass = new MyClass(10);
            // Read field
            int value = (int)fieldRead(myClass);
            // Print value
            WriteLine(value); // 10
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
    public struct MyStruct
    {
        public int value;

        public MyStruct(int value)
        {
            this.value = value;
        }
    }
}
