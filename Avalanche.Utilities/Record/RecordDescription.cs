// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Reflection;
using Avalanche.Utilities.Provider;

/// <summary>Record information description.</summary>
/// <remarks>See <see cref="RecordDescriptionExtensions"/> and <see cref="RecordDescriptionExtensions_"/> for extension methods.</remarks>
public record RecordDescription : ReadOnlyAssignableRecord, IRecordDescription, IReadOnly
{
    /// <summary>Creates <see cref="IRecordDescription"/>.</summary>
    static readonly IProvider<Type, IRecordDescription> create = Providers.Func((Type recordType) => (IRecordDescription)new RecordDescription().Read(recordType).AssignConstructors().ChooseConstruction().SetReadOnlyDeep(true));
    /// <summary>Creates <![CDATA[IResult<IRecordDescription>]]>.</summary>
    static readonly IProvider<Type, IResult<IRecordDescription>> createResult = create.ResultCaptured();
    /// <summary>Creates and caches <![CDATA[IResult<IRecordDescription>]]>.</summary>
    static readonly IProvider<Type, IResult<IRecordDescription>> cachedResult = createResult.AsReadOnly().WeakCached();
    /// <summary>Creates and caches <see cref="IRecordDescription"/>.</summary>
    static readonly IProvider<Type, IRecordDescription> cached = cachedResult.ResultOpened();

    /// <summary>Creates <see cref="IRecordDescription"/>.</summary>
    public static IProvider<Type, IRecordDescription> Create => create;
    /// <summary>Creates <![CDATA[IResult<IRecordDescription>]]>.</summary>
    public static IProvider<Type, IResult<IRecordDescription>> CreateResult => createResult;
    /// <summary>Creates and caches <![CDATA[IResult<IRecordDescription>]]>.</summary>
    public static IProvider<Type, IResult<IRecordDescription>> CachedResult => cachedResult;
    /// <summary>Creates and caches <see cref="IRecordDescription"/>.</summary>
    public static IProvider<Type, IRecordDescription> Cached => cached;

    /// <summary>Record name</summary>
    protected object? name;
    /// <summary>Record type</summary>
    protected Type? type;
    /// <summary>All record constructors.</summary>
    protected IConstructorDescription[]? constructors;
    /// <summary>Describes record deconstructor as <see cref="ValueTuple"/>: <see cref="MethodInfo"/>, <see cref="Delegate"/>, <![CDATA[IWriterBase]]></summary>
    protected Object? deconstructor;
    /// <summary>Fields</summary>
    protected IFieldDescription[]? fields;
    /// <summary>Annotations, such as <see cref="Attribute"/></summary>
    protected object[]? annotations;
    /// <summary>Record construction strategy</summary>
    protected object? construction;

    /// <summary>Record name</summary>
    public virtual object Name { get => name!; set => this.AssertWritable().name = value; }
    /// <summary>Record type</summary>
    public virtual Type Type { get => type!; set => this.AssertWritable().type = value; }
    /// <summary>All record constructors.</summary>
    public virtual IConstructorDescription[]? Constructors { get => constructors; set => this.AssertWritable().constructors = value; }
    /// <summary>Describes record deconstructor as <see cref="ValueTuple"/>: <see cref="MethodInfo"/>, <see cref="Delegate"/>, <![CDATA[IWriterBase]]></summary>
    public virtual Object? Deconstructor { get => deconstructor; set => this.AssertWritable().deconstructor = value; }
    /// <summary>Fields</summary>
    public virtual IFieldDescription[] Fields { get => fields ?? Array.Empty<IFieldDescription>(); set => this.AssertWritable().fields = value; }
    /// <summary>Annotations, such as <see cref="Attribute"/></summary>
    public virtual object[] Annotations { get => annotations ?? Array.Empty<object>(); set => this.AssertWritable().annotations = value; }
    /// <summary>Record construction strategy, typically <see cref="IConstructionDescription"/>.</summary>
    public virtual object? Construction { get => construction; set => this.AssertWritable().construction = value; }

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
    public override string ToString() => $"{Type} {Name} {{ {string.Join(", ", (IEnumerable<object>)Fields)} }}";
}
