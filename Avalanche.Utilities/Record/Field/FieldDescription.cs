// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Reflection;
using Avalanche.Utilities.Provider;

/// <summary>Field description</summary>
public record FieldDescription : ReadOnlyAssignableRecord, IFieldDescription, IReadOnly
{
    /// <summary>Creates <see cref="IFieldDescription"/>.</summary>
    static readonly IProvider<object, IFieldDescription> create = Providers.Func((object fieldobject) => (IFieldDescription)new FieldDescription().Read(fieldobject));
    /// <summary>Creates <![CDATA[IResult<IFieldDescription>]]>.</summary>
    static readonly IProvider<object, IResult<IFieldDescription>> createResult = create.ResultCaptured();
    /// <summary>Creates and caches <![CDATA[IResult<IFieldDescription>]]>.</summary>
    static readonly IProvider<object, IResult<IFieldDescription>> cachedResult = createResult.AsReadOnly().WeakCached();
    /// <summary>Creates and caches <see cref="IFieldDescription"/>.</summary>
    static readonly IProvider<object, IFieldDescription> cached = cachedResult.ResultOpened();

    /// <summary>Creates <see cref="IFieldDescription"/>.</summary>
    static readonly IProvider<object, IFieldDescription> createWithRecord = Providers.Func<object, IFieldDescription>(readWithRecord);
    /// <summary>Creates <![CDATA[IResult<IFieldDescription>]]>.</summary>
    static readonly IProvider<object, IResult<IFieldDescription>> createResultWithRecord = createWithRecord.ResultCaptured();
    /// <summary>Creates and caches <![CDATA[IResult<IFieldDescription>]]>.</summary>
    static readonly IProvider<object, IResult<IFieldDescription>> cachedResultWithRecord = createResultWithRecord.AsReadOnly().WeakCached();
    /// <summary>Creates and caches <see cref="IFieldDescription"/>.</summary>
    static readonly IProvider<object, IFieldDescription> cachedWithRecord = cachedResultWithRecord.ResultOpened();

    /// <summary>Creates <see cref="IFieldDescription"/>.</summary>
    public static IProvider<object, IFieldDescription> Create => create;
    /// <summary>Creates <![CDATA[IResult<IFieldDescription>]]>.</summary>
    public static IProvider<object, IResult<IFieldDescription>> CreateResult => createResult;
    /// <summary>Creates and caches <![CDATA[IResult<IFieldDescription>]]>.</summary>
    public static IProvider<object, IResult<IFieldDescription>> CachedResult => cachedResult;
    /// <summary>Creates and caches <see cref="IFieldDescription"/>.</summary>
    public static IProvider<object, IFieldDescription> Cached => cached;

    /// <summary>Creates <see cref="IFieldDescription"/>.</summary>
    public static IProvider<object, IFieldDescription> CreateWithRecord => createWithRecord;
    /// <summary>Creates <![CDATA[IResult<IFieldDescription>]]>.</summary>
    public static IProvider<object, IResult<IFieldDescription>> CreateResultWithRecord => createResultWithRecord;
    /// <summary>Creates and caches <![CDATA[IResult<IFieldDescription>]]>.</summary>
    public static IProvider<object, IResult<IFieldDescription>> CachedResultWithRecord => cachedResultWithRecord;
    /// <summary>Creates and caches <see cref="IFieldDescription"/>.</summary>
    public static IProvider<object, IFieldDescription> CachedWithRecord => cachedWithRecord;

    /// <summary>Try create field description with record description attached.</summary>
    /// <param name="fieldObject"><see cref="FieldInfo"/>, <see cref="ParameterInfo"/> or <see cref="PropertyInfo"/></param>
    /// <returns></returns>
    static IFieldDescription readWithRecord(object fieldObject)
    {
        // Place record type here
        Type? recordType = null;
        // Got member
        if (fieldObject is MemberInfo mi) recordType = mi.ReflectedType;
        // Got parameter
        else if (fieldObject is ParameterInfo pi && pi.Member != null) recordType = pi.Member.ReflectedType;

        // Got record type
        if (recordType != null)
        {
            // Get record description
            IRecordDescription recordDescription = RecordDescription.Cached[recordType];
            // Get fields
            foreach (IFieldDescription fieldDescription in recordDescription.Fields)
                // Got reflection
                if (fieldDescription.Reader == fieldObject) return fieldDescription;
        }

        // Revert to field description without record
        return (IFieldDescription)new FieldDescription().Read(fieldObject);
    }

    /// <summary>Field name</summary>
    protected object? name;
    /// <summary>Field type</summary>
    protected Type? type;
    /// <summary>Describes field writer: <see cref="ParameterInfo"/>, <see cref="FieldDescription"/>, <see cref="PropertyInfo"/>, <see cref="MethodInfo"/>, <see cref="Delegate"/>, <![CDATA[IWriterBase]]></summary>
    protected Object? writer;
    /// <summary>Describes field reader: <see cref="FieldInfo"/>, <see cref="PropertyInfo"/>, <see cref="MethodInfo"/>, <see cref="Delegate"/>, <![CDATA[IWriterBase]]></summary>
    protected Object? reader;
    /// <summary>Describes field reader: <see cref="FieldInfo"/></summary>
    protected Object? referer;
    /// <summary>Annotations, such as <see cref="Attribute"/></summary>
    protected object[]? annotations;
    /// <summary>Record which field is member of.</summary>
    protected IRecordDescription? record;
    /// <summary>Initial value</summary>
    protected Object? initialValue { get; set; }


    /// <summary>Field name</summary>
    public virtual object Name { get => name!; set => this.AssertWritable().name = value; }
    /// <summary>Field type</summary>
    public virtual Type Type { get => type!; set => this.AssertWritable().type = value; }
    /// <summary>Describes field constructor: <see cref="ParameterInfo"/>, <see cref="FieldDescription"/>, <see cref="PropertyInfo"/>, <see cref="MethodInfo"/>, <see cref="Delegate"/>, <![CDATA[IWriterBase]]></summary>
    public virtual Object? Writer { get => writer; set => this.AssertWritable().writer = value; }
    /// <summary>Describes field deconstructor: <see cref="FieldInfo"/>, <see cref="PropertyInfo"/>, <see cref="MethodInfo"/>, <see cref="Delegate"/>, <![CDATA[IWriterBase]]></summary>
    public virtual Object? Reader { get => reader; set => this.AssertWritable().reader = value; }
    /// <summary>Describes field refering: <see cref="FieldInfo"/></summary>
    public virtual Object? Referer { get => referer; set => this.AssertWritable().referer = value; }
    /// <summary>Annotations, such as <see cref="Attribute"/></summary>
    public virtual object[] Annotations { get => annotations ?? Array.Empty<object>(); set => this.AssertWritable().annotations = value; }
    /// <summary>Record which field is member of.</summary>
    public virtual IRecordDescription? Record { get => record; set => this.AssertWritable().record = value; }
    /// <summary>Initial value</summary>
    public virtual Object? InitialValue { get => initialValue; set => this.AssertWritable().initialValue = value; }

    /// <summary></summary>
    protected override void setReadOnly() { hash_cached = this.CalcHash64(); @readonly = true; }
    /// <summary>Cached hashcode, calculated at <see cref="IReadOnly"/>.</summary>
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

    /// <summary>Print information</summary>
    public override string ToString() => $"{Type} {Name}";
}
