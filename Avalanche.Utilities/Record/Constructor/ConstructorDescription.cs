// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Reflection;
using System.Runtime.Serialization;

/// <summary>Constructor information description.</summary>
public record ConstructorDescription : ReadOnlyAssignableRecord, IConstructorDescription //, IValidable
{
    /// <summary>Constructor type</summary>
    protected Type? type;
    /// <summary>Describes constructor: <see cref="ConstructorInfo"/>, <see cref="MethodInfo"/>, <see cref="Delegate"/>, <![CDATA[IWriterBase]]> as <see cref="ValueType"/>, or <![CDATA[EmitLine]]> <![CDATA[IEnumerable<EmitLine>]]>.</summary>
    protected Object? constructor;
    /// <summary>Parameters</summary>
    protected IParameterDescription[]? parameters;
    /// <summary>Annotations, such as <see cref="Attribute"/></summary>
    protected object[]? annotations;
    /// <summary>Record which is member of.</summary>
    protected IRecordDescription? record;

    /// <summary>Parameter type</summary>
    public Type Type { get => type!; set => this.AssertWritable().type = value; }
    /// <summary>Describes constructor constructor: <see cref="ConstructorInfo"/>, <see cref="MethodInfo"/>, <see cref="Delegate"/>, <![CDATA[IWriterBase]]> as <see cref="ValueType"/>, or <![CDATA[EmitLine]]> <![CDATA[IEnumerable<EmitLine>]]>.</summary>
    public Object Constructor { get => constructor!; set => this.AssertWritable().constructor = value; }
    /// <summary>Parameters</summary>
    public IParameterDescription[] Parameters { get => parameters ?? Array.Empty<IParameterDescription>(); set => this.AssertWritable().parameters = value; }
    /// <summary>Annotations, such as <see cref="Attribute"/></summary>
    public object[] Annotations { get => annotations ?? Array.Empty<object>(); set => this.AssertWritable().annotations = value; }
    /// <summary>Record which is member of.</summary>
    public virtual IRecordDescription? Record { get => record; set => this.AssertWritable().record = value; }

    // <summary></summary>
    //public bool IsValid => constructor != null && type != null && parameters != null;

    /// <summary></summary>
    protected override void setReadOnly() { hash_cached = this.CalcHash64(); @readonly = true; }
    /// <summary>Cached hashcode, calculated at ReadOnly set.</summary>
    [IgnoreDataMember]
    protected ulong hash_cached;
    /// <summary>Get-or-calc 64bit hash</summary>
    [IgnoreDataMember]
    public ulong Hash64 => @readonly ? hash_cached : this.CalcHash64();
    /// <summary>Calculate hashcode</summary>
    public override int GetHashCode()
    {
        // Get or calc hash
        ulong hash64 = Hash64;
        // 64bit to 32bit
        int hash32 = unchecked((int)((hash64 >> 32) ^ (hash64 & 0xFFFFFFFFUL)));
        // 
        return hash32;
    }

    /// <summary>Print information</summary>
    public override string ToString() => $"{Type}({string.Join(", ", (IEnumerable<object>)Parameters)})";
}
