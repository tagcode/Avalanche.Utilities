// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using Avalanche.Utilities.Provider;

/// <summary>Table that contains record and field descriptions and delegate providers.</summary>
public record RecordProviders : ReadOnlyAssignableRecord, IRecordProviders
{
    /// <summary>Create providers only</summary>
    static RecordProviders create = new RecordProviders()
    {
        RecordDescription = Avalanche.Utilities.Record.RecordDescription.CreateResult,
        RecordDelegates = RecordDelegatesProvider.CreateResult,
        RecordDelegatesByType = RecordDelegatesProvider.CreateResultFromType,
        RecordCreate = Record.RecordCreateFunc.CreateResult,
        FieldDelegates = FieldDelegateProviders.CreateResult,
        FieldRead = Record.FieldRead.CreateResult,
        FieldWrite = Record.FieldWrite.CreateResult,
        RecreateWith = Record.RecreateWith.CreateResult,
    };
    /// <summary>Singleton</summary>
    static Lazy<RecordProviders> weak = new(() => new RecordProviders()
    {
        RecordDescription = Avalanche.Utilities.Record.RecordDescription.CachedResult,
        RecordDelegates = RecordDelegatesProvider.CachedResult,
        RecordDelegatesByType = RecordDelegatesProvider.CachedResultFromType,
        RecordCreate = Record.RecordCreateFunc.CachedResult,
        FieldDelegates = FieldDelegateProviders.CachedResult,
        FieldRead = Record.FieldRead.CachedResult,
        FieldWrite = Record.FieldWrite.CachedResult,
        RecreateWith = Record.RecreateWith.CachedResult,
    });
    /// <summary>Singleton for string keyed provider</summary>
    public static RecordProviders NewCached()
    {
        var fieldDelegatesProvider = FieldDelegateProviders.CreateResult.Cached();
        var recordDelegatesProvider = new RecordDelegatesProvider(fieldDelegatesProvider).ResultCaptured().Cached();
        var recordDelegatesByType = new ResultProviderConcat<Type, IRecordDescription, IRecordDelegates>(Avalanche.Utilities.Record.RecordDescription.CreateResult, recordDelegatesProvider);
        return new RecordProviders()
        {
            RecordDescription = Avalanche.Utilities.Record.RecordDescription.CreateResult.Cached(),
            RecordDelegates = recordDelegatesProvider,
            RecordDelegatesByType = recordDelegatesByType,
            RecordCreate = Record.RecordCreateFunc.CreateResult.Cached(),
            FieldDelegates = fieldDelegatesProvider,
            FieldRead = Record.FieldRead.CreateResult.Cached(),
            FieldWrite = Record.FieldWrite.CreateResult.Cached(),
            RecreateWith = Record.RecreateWith.CreateResult.Cached(),
        };
    }
    /// <summary>Singleton for weak keyed caches</summary>
    public static RecordProviders Cached => weak.Value;
    /// <summary>New create providers but no cache.</summary>
    public static RecordProviders Create => create;

    /// <summary>Get record description for recordType.</summary>
    IProvider<Type, IResult<IRecordDescription>> recordDescription = null!;
    /// <summary>Get all record delegates.</summary>
    IProvider<Type, IResult<IRecordDelegates>> recordDelegatesByType = null!;
    /// <summary>Get all record delegates.</summary>
    IProvider<IRecordDescription, IResult<IRecordDelegates>> recordDelegates = null!;
    /// <summary>Get <![CDATA[Func<object[], Record>]]> delegate.</summary>
    IProvider<IRecordDescription, IResult<Delegate>> createRecord = null!;
    /// <summary>Get all field delegates.</summary>
    IProvider<IFieldDescription, IResult<IFieldDelegates>> fieldDelegates = null!;
    /// <summary>Get specific field delegate.</summary>
    IProvider<IFieldDescription, IResult<Delegate>> readField = null!;
    /// <summary>Get specific field delegate.</summary>
    IProvider<IFieldDescription, IResult<Delegate>> writeField = null!;
    /// <summary>Get specific field delegate.</summary>
    IProvider<IFieldDescription, IResult<Delegate>> recreateWith = null!;

    /// <summary>Get record description for recordType.</summary>
    public IProvider<Type, IResult<IRecordDescription>> RecordDescription { get => recordDescription; set => this.AssertWritable().recordDescription = value; }
    /// <summary>Get all record delegates.</summary>
    public IProvider<Type, IResult<IRecordDelegates>> RecordDelegatesByType { get => recordDelegatesByType; set => this.AssertWritable().recordDelegatesByType = value; }
    /// <summary>Get all record delegates.</summary>
    public IProvider<IRecordDescription, IResult<IRecordDelegates>> RecordDelegates { get => recordDelegates; set => this.AssertWritable().recordDelegates = value; }
    /// <summary>Get <![CDATA[Func<object[], Record>]]> delegate.</summary>
    public IProvider<IRecordDescription, IResult<Delegate>> RecordCreate { get => createRecord; set => this.AssertWritable().createRecord = value; }
    /// <summary>Get all field delegates.</summary>
    public IProvider<IFieldDescription, IResult<IFieldDelegates>> FieldDelegates { get => fieldDelegates; set => this.AssertWritable().fieldDelegates = value; }
    /// <summary>Get specific field delegate.</summary>
    public IProvider<IFieldDescription, IResult<Delegate>> FieldRead { get => readField; set => this.AssertWritable().readField = value; }
    /// <summary>Get specific field delegate.</summary>
    public IProvider<IFieldDescription, IResult<Delegate>> FieldWrite { get => writeField; set => this.AssertWritable().writeField = value; }
    /// <summary>Get specific field delegate.</summary>
    public IProvider<IFieldDescription, IResult<Delegate>> RecreateWith { get => recreateWith; set => this.AssertWritable().recreateWith = value; }

}
