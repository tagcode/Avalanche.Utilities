using System;

class cloner
{
    public class MyRecord : ICloneable
    {
        /// <summary></summary>
        public readonly int Id;
        /// <summary></summary>
        public readonly string Name;

        /// <summary></summary>
        public MyRecord(int id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>Clone</summary>
        public object Clone() => new MyRecord(Id, Name);
    }
}
