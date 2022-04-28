using System;
using System.Runtime.Serialization;
using Avalanche.Utilities;

class @readonly
{
    public static void Run()
    {
        {
            MyClass myClass = new MyClass { Id = 5, Label = "ABC" }.SetReadOnly();
        }

        {
            // Create record
            MyRecord myRecord = new MyRecord { Id = 5, Label = "ABC" }.SetReadOnly();
            // Clone using 'with' operator
            MyRecord myRecord2 = (myRecord with { Label = "XYZ " }).SetReadOnly();
        }

        {
            // Create records
            MyRecord[] records =
                new MyRecord[]
                {
                    new MyRecord { Id = 1, Label = "A" },
                    new MyRecord { Id = 2, Label = "B" },
                    new MyRecord { Id = 3, Label = "C" }
                }.SetElementsReadOnly();
        }
    }

    public class MyClass : IReadOnly
    {
        /// <summary>Is read-only state</summary>
        [IgnoreDataMember] protected bool @readonly;
        /// <summary>Is read-only state</summary>
        [IgnoreDataMember] bool IReadOnly.ReadOnly { get => @readonly; set { if (@readonly == value) return; if (!value) throw new InvalidOperationException(); @readonly = true; } }

        /// <summary>Id</summary>
        protected int id;
        /// <summary>Label</summary>
        protected string label = null!;

        /// <summary>Id</summary>
        public int Id { get => id; set => this.AssertWritable().id = value; }
        /// <summary>Label</summary>
        public string Label { get => label; set => this.AssertWritable().label = value; }
    }

    public record MyRecord : ReadOnlyAssignableRecord
    {
        /// <summary>Id</summary>
        protected int id;
        /// <summary>Label</summary>
        protected string label = null!;

        /// <summary>Id</summary>
        public int Id { get => id; set => this.AssertWritable().id = value; }
        /// <summary>Label</summary>
        public string Label { get => label; set => this.AssertWritable().label = value; }
    }
    public class MyClass2 : ReadOnlyAssignableClass
    {
        /// <summary>Id</summary>
        protected int id;
        /// <summary>Label</summary>
        protected string label = null!;

        /// <summary>Id</summary>
        public int Id { get => id; set => this.AssertWritable().id = value; }
        /// <summary>Label</summary>
        public string Label { get => label; set => this.AssertWritable().label = value; }
    }

}

