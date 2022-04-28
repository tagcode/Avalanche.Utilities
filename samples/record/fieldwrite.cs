using System;
using System.Reflection;
using Avalanche.Utilities.Record;
using static System.Console;

class fieldwrite
{
    public static void Run()
    {
        // FieldWrite<Record, Field>
        {
            // Get field reference
            FieldInfo fi = typeof(MyStruct).GetField(nameof(MyStruct.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.Create[fi];
            // Create delegate
            fieldDescription.TryCreateFieldWriteDelegate(out Delegate @delegate);
            // Cast delegate
            FieldWrite<MyStruct, int> fieldWrite = (FieldWrite<MyStruct, int>)@delegate;
            // Create struct
            MyStruct myStruct = new MyStruct(2);
            // Write field
            fieldWrite(ref myStruct, 10);
            // Print value
            WriteLine(myStruct.value); // 10
        }

        {
            // Get field reference
            FieldInfo fi = typeof(MyStruct).GetField(nameof(MyStruct.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.Cached[fi];
            // Create delegate
            FieldWrite<MyStruct, int> fieldWrite = (FieldWrite<MyStruct, int>)FieldWrite.Create[fieldDescription];
            // Create struct
            MyStruct myStruct = new MyStruct(2);
            // Write field
            fieldWrite(ref myStruct, 10);
            // Print value
            WriteLine(myStruct.value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyStruct).GetField(nameof(MyStruct.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.Cached[fi];
            // Get (weak key) cached delegate
            FieldWrite<MyStruct, int> fieldWrite = (FieldWrite<MyStruct, int>)FieldWrite.Cached[fieldDescription];
            // Create struct
            MyStruct myStruct = new MyStruct(2);
            // Write field
            fieldWrite(ref myStruct, 10);
            // Print value
            WriteLine(myStruct.value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyStruct).GetField(nameof(MyStruct.value))!;
            // Create delegate
            FieldWrite<MyStruct, int> fieldWrite = (FieldWrite<MyStruct, int>)FieldWrite.CreateFromObject[fi];
            // Create struct
            MyStruct myStruct = new MyStruct(2);
            // Write field
            fieldWrite(ref myStruct, 10);
            // Print value
            WriteLine(myStruct.value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyStruct).GetField(nameof(MyStruct.value))!;
            // Get (weak key) cached delegate
            FieldWrite<MyStruct, int> fieldWrite = (FieldWrite<MyStruct, int>)FieldWrite.CachedFromObject[fi];
            // Create struct
            MyStruct myStruct = new MyStruct(2);
            // Write field
            fieldWrite(ref myStruct, 10);
            // Print value
            WriteLine(myStruct.value); // 10
        }


        // Action<Record, Field>
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.Create[fi];
            // Create delegate
            fieldDescription.TryCreateFieldWriteAction(out Delegate @delegate);
            // Cast delegate
            Action<MyClass, int> fieldWrite = (Action<MyClass, int>)@delegate;
            // Create class
            MyClass myClass = new MyClass(2);
            // Write field
            fieldWrite(myClass, 10);
            // Print value
            WriteLine(myClass.value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.Cached[fi];
            // Create delegate
            Action<MyClass, int> fieldWrite = (Action<MyClass, int>)FieldWriteAction.Create[fieldDescription];
            // Create class
            MyClass myClass = new MyClass(2);
            // Write field
            fieldWrite(myClass, 10);
            // Print value
            WriteLine(myClass.value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.Cached[fi];
            // Get (weak key) cached delegate
            Action<MyClass, int> fieldWrite = (Action<MyClass, int>)FieldWriteAction.Cached[fieldDescription];
            // Create Class
            MyClass myClass = new MyClass(2);
            // Write field
            fieldWrite(myClass, 10);
            // Print value
            WriteLine(myClass.value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Create delegate
            Action<MyClass, int> fieldWrite = (Action<MyClass, int>)FieldWriteAction.CreateFromObject[fi];
            // Create Class
            MyClass myClass = new MyClass(2);
            // Write field
            fieldWrite(myClass, 10);
            // Print value
            WriteLine(myClass.value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Get (weak key) cached delegate
            Action<MyClass, int> fieldWrite = (Action<MyClass, int>)FieldWriteAction.CachedFromObject[fi];
            // Create Class
            MyClass myClass = new MyClass(2);
            // Write field
            fieldWrite(myClass, 10);
            // Print value
            WriteLine(myClass.value); // 10
        }



        // Action<object, object>
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.Create[fi];
            // Create delegate
            fieldDescription.TryCreateFieldWriteActionOO(out Action<object, object> fieldWrite);
            // Create class
            MyClass myClass = new MyClass(2);
            // Write field
            fieldWrite(myClass, 10);
            // Print value
            WriteLine(myClass.value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.Cached[fi];
            // Create delegate
            Action<object, object> fieldWrite = (Action<object, object>)FieldWriteActionOO.Create[fieldDescription];
            // Create class
            MyClass myClass = new MyClass(2);
            // Write field
            fieldWrite(myClass, 10);
            // Print value
            WriteLine(myClass.value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.Cached[fi];
            // Get (weak key) cached delegate
            Action<object, object> fieldWrite = (Action<object, object>)FieldWriteActionOO.Cached[fieldDescription];
            // Create Class
            MyClass myClass = new MyClass(2);
            // Write field
            fieldWrite(myClass, 10);
            // Print value
            WriteLine(myClass.value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Create delegate
            Action<object, object> fieldWrite = (Action<object, object>)FieldWriteActionOO.CreateFromObject[fi];
            // Create Class
            MyClass myClass = new MyClass(2);
            // Write field
            fieldWrite(myClass, 10);
            // Print value
            WriteLine(myClass.value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Get (weak key) cached delegate
            Action<object, object> fieldWrite = (Action<object, object>)FieldWriteActionOO.CachedFromObject[fi];
            // Create Class
            MyClass myClass = new MyClass(2);
            // Write field
            fieldWrite(myClass, 10);
            // Print value
            WriteLine(myClass.value); // 10
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
