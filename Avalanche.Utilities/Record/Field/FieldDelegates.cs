// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;

/// <summary></summary>
public abstract record FieldDelegates : ReadOnlyAssignableRecord, IFieldDelegates, IReadOnly
{
    /// <summary>Constructor</summary>
    static readonly ConstructorT2<FieldDelegates> constructor = new(typeof(FieldDelegates<,>));
    /// <summary>Create</summary>
    public static FieldDelegates Create(Type recordType, Type fieldType) => constructor.Create(recordType, fieldType);

    /// <summary>Assign new value to newValue.</summary>
    protected internal Delegate? readField;
    /// <summary>Read field value</summary>
    protected internal Delegate? writeField;
    /// <summary>Create new instance of record with newValue.</summary>
    protected internal Delegate? recreateWith;
    /// <summary></summary>
    protected internal IFieldDescription? fieldDescription;

    /// <summary>Record Type</summary>
    public abstract Type RecordType { get; set; }
    /// <summary>Field Type</summary>
    public abstract Type FieldType { get; set; }
    /// <summary>Assign new value to newValue.</summary>
    public virtual Delegate? FieldRead { get => readField; set => this.AssertWritable().readField = value; }
    /// <summary>Read field value</summary>
    public virtual Delegate? FieldWrite { get => writeField; set => this.AssertWritable().writeField = value; }
    /// <summary>Create new instance of record with value newValue.</summary>
    public virtual Delegate? RecreateWith { get => recreateWith; set => this.AssertWritable().recreateWith = value; }
    /// <summary></summary>
    public virtual IFieldDescription? FieldDescription { get => fieldDescription; set => this.AssertWritable().fieldDescription = value; }
}

/// <summary></summary>
public abstract record FieldDelegates<Record> : FieldDelegates, IFieldDelegates<Record>
{
}

/// <summary></summary>
public record FieldDelegates<Record, Field> : FieldDelegates<Record>, IFieldDelegates<Record, Field>
{
    /// <summary>Record Type</summary>
    public override Type RecordType { get => typeof(Record); set => throw new InvalidOperationException(); }
    /// <summary>Field Type</summary>
    public override Type FieldType { get => typeof(Field); set => throw new InvalidOperationException(); }

    /// <summary></summary>
    public new virtual FieldRead<Record, Field>? FieldRead { get => (FieldRead<Record, Field>)readField!; set => this.AssertWritable().readField = value!; }
    /// <summary></summary>
    public new virtual FieldWrite<Record, Field>? FieldWrite { get => (FieldWrite<Record, Field>)writeField!; set => this.AssertWritable().writeField = value!; }
    /// <summary></summary>
    public new virtual RecreateWith<Record, Field>? RecreateWith { get => (RecreateWith<Record, Field>)recreateWith!; set => this.AssertWritable().recreateWith = value!; }
}
