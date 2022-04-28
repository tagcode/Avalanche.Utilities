// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Reflection;
using System.Runtime.Serialization;

/// <summary>Parameter description</summary>
public class ParameterDescription : IParameterDescription, IReadOnly, ICloneable//, IValidable
{
    /// <summary>Parameter name</summary>
    protected object? name;
    /// <summary>Parameter type</summary>
    protected Type? type;
    /// <summary>Describes parameter writer: <see cref="ParameterInfo"/></summary>
    protected Object? writer;
    /// <summary>Annotations, such as <see cref="Attribute"/></summary>
    protected object[]? annotations;
    /// <summary>Constructor which parameter is member of.</summary>
    protected IConstructorDescription? member;
    /// <summary>Parameter is optional</summary>
    protected bool? optional;

    /// <summary>Parameter name</summary>
    public virtual object Name { get => name!; set => AssertWritable.name = value; }
    /// <summary>Parameter type</summary>
    public virtual Type Type { get => type!; set => AssertWritable.type = value; }
    /// <summary>Describes parameter constructor: <see cref="ParameterInfo"/></summary>
    public virtual Object? Writer { get => writer; set => AssertWritable.writer = value; }
    /// <summary>Annotations, such as <see cref="Attribute"/></summary>
    public virtual object[] Annotations { get => annotations ?? Array.Empty<object>(); set => AssertWritable.annotations = value; }
    /// <summary>Constructor which parameter is member of.</summary>
    public virtual IConstructorDescription? Member { get => member; set => AssertWritable.member = value; }
    /// <summary>Parameter is optional</summary>
    public virtual bool? Optional { get => optional; set => AssertWritable.optional = value; }

    /// <summary>Is read-only state</summary>
    protected bool @readonly;
    /// <summary>Is read-only state</summary>
    [IgnoreDataMember] bool IReadOnly.ReadOnly { get => @readonly; set { if (@readonly == value) return; if (!value) throw new InvalidOperationException("Read-only"); hash_cached = this.CalcHash64(); @readonly = value; } }
    /// <summary>Assert writable</summary>
    protected ParameterDescription AssertWritable => @readonly ? throw new InvalidOperationException("Read-only") : this;

    //public bool IsValid => name != null && type != null;

    /// <summary>Cached hashcode, calculated at SetReadOnly.</summary>
    protected ulong hash_cached;
    /// <summary>Get-or-calc 64bit hash</summary>
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

    /// <summary>Clone self. Parameters are </summary>
    /// <returns>Clone of self with same references but in writable state</returns>
    public virtual object Clone() => ParameterDescriptionExtensions_.Clone(this);

    /// <summary>Print information</summary>
    public override string ToString() => $"{Type} {Name}";
}
