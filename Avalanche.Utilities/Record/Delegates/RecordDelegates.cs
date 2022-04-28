// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Runtime.Serialization;

/// <summary></summary>
public abstract class RecordDelegates : IRecordDelegates, IReadOnly, ICloneable
{
    /// <summary>Constructor</summary>
    static readonly ConstructorT<RecordDelegates> constructor = new ConstructorT<RecordDelegates>(typeof(RecordDelegates<>));
    /// <summary>Create</summary>
    public static RecordDelegates Create(Type recordType) => constructor.Create(recordType);

    /// <summary><![CDATA[Func<object[], Record>]]></summary>
    protected internal Delegate? recordCreate;
    /// <summary>Field delegates</summary>
    protected internal IFieldDelegates[]? fieldDelegates;
    /// <summary></summary>
    protected internal IRecordDescription? recordDescription;

    /// <summary>Record Type</summary>
    public abstract Type RecordType { get; set; }
    /// <summary><![CDATA[Func<object[], Record>]]></summary>
    public virtual Delegate? RecordCreate { get => recordCreate; set => AssertWritable.recordCreate = value; }
    /// <summary>Field delegates</summary>
    public virtual IFieldDelegates[]? FieldDelegates { get => fieldDelegates; set => AssertWritable.fieldDelegates = value; }
    /// <summary></summary>
    public virtual IRecordDescription? RecordDescription { get => recordDescription; set => AssertWritable.recordDescription = value; }

    /// <summary>Is read-only state</summary>
    protected bool @readonly;
    /// <summary>Is read-only state</summary>
    [IgnoreDataMember] bool IReadOnly.ReadOnly { get => @readonly; set { if (@readonly == value) return; if (!value) throw new InvalidOperationException("Read-only"); @readonly = value; } }
    /// <summary>Assert writable</summary>
    protected RecordDelegates AssertWritable => @readonly ? throw new InvalidOperationException("Read-only") : this;

    /// <summary>Clone self.</summary>
    public virtual object Clone() => RecordDelegatesExtensions_.Clone(this);
}

/// <summary></summary>
public class RecordDelegates<Record> : RecordDelegates, IRecordDelegates<Record>
{
    /// <summary><![CDATA[Func<object[], Record>]]></summary>
    public new virtual Func<object[], Record>? RecordCreate { get => (Func<object[], Record>)recordCreate!; set => AssertWritable.recordCreate = (Func<object[], Record>)value!; }
    /// <summary>Record Type</summary>
    public override Type RecordType { get => typeof(Record); set => throw new InvalidOperationException(); }
}

