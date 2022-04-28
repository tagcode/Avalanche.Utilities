using System.Reflection;
using Avalanche.Utilities;
using Avalanche.Utilities.Record;

class fielddescription
{
    public static void Run()
    {
        {
            IFieldDescription fieldDescription = new FieldDescription
            {
                Name = "",
                Type = typeof(string),
                Writer = null,
                Reader = null,
                Referer = null,
                Annotations = new object[0],
                Record = null,
                InitialValue = null,
            }.SetReadOnly();
        }
        {
            IFieldDescription fieldDescription =
                new FieldDescription()
                .SetName("")
                .SetType(typeof(string))
                .SetWriter(null)
                .SetReader(null)
                .SetReferer(null)
                .SetAnnotations(new object[0])
                .SetRecord(null)
                .SetInitialValue(null)
                .SetReadOnly();
            IFieldDescription clone = fieldDescription.Clone().SetReadOnly();

        }
        {
            FieldInfo fi = typeof(MyClass).GetField("name")!;
            IFieldDescription fieldDescription =
                new FieldDescription()
                .SetName("Name")
                .SetType(typeof(string))
                .SetReader(fi)
                .SetWriter(fi)
                .SetReferer(fi)
                .SetReadOnly();
        }
        {
            PropertyInfo pi = typeof(MyClass).GetProperty("Name")!;
            IFieldDescription fieldDescription =
                new FieldDescription()
                .SetName("Name")
                .SetType(typeof(string))
                .SetReader(pi)
                .SetWriter(pi)
                .SetReadOnly();
        }
        {
            MethodInfo miGet = typeof(MyClass).GetMethod("GetName")!;
            MethodInfo miSet = typeof(MyClass).GetMethod("SetName")!;
            IFieldDescription fieldDescription =
                new FieldDescription()
                .SetName("Name")
                .SetType(typeof(string))
                .SetReader(miGet)
                .SetWriter(miSet)
                .SetReadOnly();
        }
        {
            FieldInfo fi = typeof(MyClass).GetField("name")!;
            IFieldDescription fieldDescription = new FieldDescription().Read(fi).SetReadOnly();
        }
        {
            FieldInfo fi = typeof(MyClass).GetField("name")!;
            IFieldDescription fieldDescription = FieldDescription.Create[fi];
        }
        {
            FieldInfo fi = typeof(MyClass).GetField("name")!;
            IFieldDescription fieldDescription = FieldDescription.Cached[fi];
        }
    }

    public class MyClass
    {
        public string name = null!;

        public string Name { get => name; set => name = value; }

        public void SetName(string name) => this.name = name;
        public string GetName() => name;
    }
}
