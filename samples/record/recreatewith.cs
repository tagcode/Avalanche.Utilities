using System;
using System.Reflection;
using System.Runtime.Serialization;
using Avalanche.Utilities.Record;
using static System.Console;

class recreatewith
{
    public static void Run()
    {
        // RecreateWith<Record, Field>
        {
            // Get field reference
            FieldInfo fi = typeof(MyStruct).GetField(nameof(MyStruct.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.CachedWithRecord[fi];
            // Create delegate
            fieldDescription.TryCreateRecreateWith(out Delegate @delegate);
            // Cast delegate
            RecreateWith<MyStruct, int> recreate = (RecreateWith<MyStruct, int>)@delegate;
            // Create struct
            MyStruct myStruct = new MyStruct(2, "abc");
            // Recreate record
            recreate(ref myStruct, 10);
            // Print value
            WriteLine(myStruct.value); // 10
        }

        {
            // Get field reference
            FieldInfo fi = typeof(MyStruct).GetField(nameof(MyStruct.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.CachedWithRecord[fi];
            // Cast delegate
            RecreateWith<MyStruct, int> recreate = (RecreateWith<MyStruct, int>)RecreateWith.Create[fieldDescription];
            // Create struct
            MyStruct myStruct = new MyStruct(2, "abc");
            // Recreate record
            recreate(ref myStruct, 10);
            // Print value
            WriteLine(myStruct.value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyStruct).GetField(nameof(MyStruct.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.CachedWithRecord[fi];
            // Cast delegate
            RecreateWith<MyStruct, int> recreate = (RecreateWith<MyStruct, int>)RecreateWith.Cached[fieldDescription];
            // Create struct
            MyStruct myStruct = new MyStruct(2, "abc");
            // Recreate record
            recreate(ref myStruct, 10);
            // Print value
            WriteLine(myStruct.value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyStruct).GetField(nameof(MyStruct.value))!;
            // Cast delegate
            RecreateWith<MyStruct, int> recreate = (RecreateWith<MyStruct, int>)RecreateWith.CreateFromObject[fi];
            // Create struct
            MyStruct myStruct = new MyStruct(2, "abc");
            // Recreate record
            recreate(ref myStruct, 10);
            // Print value
            WriteLine(myStruct.value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyStruct).GetField(nameof(MyStruct.value))!;
            // Cast delegate
            RecreateWith<MyStruct, int> recreate = (RecreateWith<MyStruct, int>)RecreateWith.CachedFromObject[fi];
            // Create struct
            MyStruct myStruct = new MyStruct(2, "abc");
            // Recreate record
            recreate(ref myStruct, 10);
            // Print value
            WriteLine(myStruct.value); // 10
        }

        // Func<Record, Field, Record>
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.CachedWithRecord[fi];
            // Create delegate
            fieldDescription.TryCreateRecreateWithFunc(out Delegate @delegate);
            // Cast delegate
            Func<MyClass, int, MyClass> recreate = (Func<MyClass, int, MyClass>)@delegate;
            // Create class
            MyClass myClass = new MyClass(2, "abc");
            // Recreate record
            myClass = recreate(myClass, 10);
            // Print value
            WriteLine(myClass.value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.CachedWithRecord[fi];
            // Create delegate
            Func<MyClass, int, MyClass> recreate = (Func<MyClass, int, MyClass>)RecreateWithFunc.Create[fieldDescription];
            // Create class
            MyClass myClass = new MyClass(2, "abc");
            // Recreate record
            myClass = recreate(myClass, 10);
            // Print value
            WriteLine(myClass.value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.CachedWithRecord[fi];
            // Create delegate
            Func<MyClass, int, MyClass> recreate = (Func<MyClass, int, MyClass>)RecreateWithFunc.Cached[fieldDescription];
            // Create class
            MyClass myClass = new MyClass(2, "abc");
            // Recreate record
            myClass = recreate(myClass, 10);
            // Print value
            WriteLine(myClass.value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Create delegate
            Func<MyClass, int, MyClass> recreate = (Func<MyClass, int, MyClass>)RecreateWithFunc.CreateFromObject[fi];
            // Create class
            MyClass myClass = new MyClass(2, "abc");
            // Recreate record
            myClass = recreate(myClass, 10);
            // Print value
            WriteLine(myClass.value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Create delegate
            Func<MyClass, int, MyClass> recreate = (Func<MyClass, int, MyClass>)RecreateWithFunc.CachedFromObject[fi];
            // Create class
            MyClass myClass = new MyClass(2, "abc");
            // Recreate record
            myClass = recreate(myClass, 10);
            // Print value
            WriteLine(myClass.value); // 10
        }

        // Func<object, object, object>
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.CachedWithRecord[fi];
            // Create delegate
            fieldDescription.TryCreateRecreateWithFuncOOO(out Func<object, object, object> recreate);
            // Create class
            MyClass myClass = new MyClass(2, "abc");
            // Recreate record
            myClass = (MyClass)recreate(myClass, 10);
            // Print value
            WriteLine(myClass.value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.CachedWithRecord[fi];
            // Create delegate
            Func<object, object, object> recreate = RecreateWithFuncOOO.Create[fieldDescription];
            // Create class
            MyClass myClass = new MyClass(2, "abc");
            // Recreate record
            myClass = (MyClass) recreate(myClass, 10);
            // Print value
            WriteLine(myClass.value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Convert to description
            IFieldDescription fieldDescription = FieldDescription.CachedWithRecord[fi];
            // Create delegate
            Func<object, object, object> recreate = RecreateWithFuncOOO.Cached[fieldDescription];
            // Create class
            MyClass myClass = new MyClass(2, "abc");
            // Recreate record
            myClass = (MyClass)recreate(myClass, 10);
            // Print value
            WriteLine(myClass.value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Create delegate
            Func<object, object, object> recreate = RecreateWithFuncOOO.CreateFromObject[fi];
            // Create class
            MyClass myClass = new MyClass(2, "abc");
            // Recreate record
            myClass = (MyClass)recreate(myClass, 10);
            // Print value
            WriteLine(myClass.value); // 10
        }
        {
            // Get field reference
            FieldInfo fi = typeof(MyClass).GetField(nameof(MyClass.value))!;
            // Create delegate
            Func<object, object, object> recreate = RecreateWithFuncOOO.CachedFromObject[fi];
            // Create class
            MyClass myClass = new MyClass(2, "abc");
            // Recreate record
            myClass = (MyClass)recreate(myClass, 10);
            // Print value
            WriteLine(myClass.value); // 10
        }
    }

    public class MyClass
    {
        [DataMember(Order = 0)]
        public readonly int value;
        [DataMember(Order = 1)]
        public readonly string name;

        public MyClass(int value, string name)
        {
            this.value = value;
            this.name = name;
        }
    }
    public struct MyStruct
    {
        [DataMember(Order = 0)]
        public readonly int value;
        [DataMember(Order = 1)]
        public readonly string name;

        public MyStruct(int value, string name)
        {
            this.value = value;
            this.name = name;
        }
    }
}
