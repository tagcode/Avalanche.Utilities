// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using Avalanche.Utilities.Provider;

/// <summary></summary>
public class RecordDelegatesProvider : ProviderBase<IRecordDescription, IRecordDelegates>
{
    /// <summary>Constructs <see cref="IRecordDelegates"/>.</summary>
    static readonly IProvider<IRecordDescription, IRecordDelegates> create = new RecordDelegatesProvider(FieldDelegateProviders.CreateResult);
    /// <summary>Constructs <![CDATA[IResult<IRecordDelegates>]]> and captures errors.</summary>
    static readonly IProvider<IRecordDescription, IResult<IRecordDelegates>> createResult = create.ResultCaptured();
    /// <summary>Constructs <![CDATA[IResult<IRecordDelegates>]]> and caches reults with errors captured in a weak hashmap.</summary>
    static readonly IProvider<IRecordDescription, IResult<IRecordDelegates>> cachedResult = createResult.WeakCached();
    /// <summary>Constructs and caches <see cref="IRecordDelegates"/> in a weak hashmap.</summary>
    static readonly IProvider<IRecordDescription, IRecordDelegates> cached = cachedResult.ResultOpened();
    /// <summary>Constructs and caches <see cref="IRecordDelegates"/> in a weak hashmap.</summary>
    static readonly IProvider<Type, IRecordDelegates> cachedFromType = RecordDescription.CachedResult.Concat(cachedResult).ResultOpened();
    /// <summary>Constructs <![CDATA[IResult<IRecordDelegates>]]> but uses no caching</summary>
    static readonly IProvider<Type, IResult<IRecordDelegates>> createResultFromType = RecordDescription.CreateResult.Concat(createResult);
    /// <summary>Constructs <![CDATA[IResult<IRecordDelegates>]]> and caches reults with errors captured in a weak hashmap.</summary>
    static readonly IProvider<Type, IResult<IRecordDelegates>> cachedResultFromType = RecordDescription.CachedResult.Concat(cachedResult);

    /// <summary>Constructs <see cref="IRecordDelegates"/>.</summary>
    public static IProvider<IRecordDescription, IRecordDelegates> Create => create;
    /// <summary>Constructs <![CDATA[IResult<IRecordDelegates>]]> and captures errors.</summary>
    public static IProvider<IRecordDescription, IResult<IRecordDelegates>> CreateResult => createResult;
    /// <summary>Constructs <![CDATA[IResult<IRecordDelegates>]]> and caches reults with errors captured in a weak hashmap.</summary>
    public static IProvider<IRecordDescription, IResult<IRecordDelegates>> CachedResult => cachedResult;
    /// <summary>Constructs and caches <see cref="IRecordDelegates"/> in a weak hashmap.</summary>
    public static IProvider<IRecordDescription, IRecordDelegates> Cached => cached;
    /// <summary>Constructs and caches <see cref="IRecordDelegates"/> in a weak hashmap.</summary>
    public static IProvider<Type, IRecordDelegates> CachedFromType => cachedFromType;
    /// <summary>Constructs <![CDATA[IResult<IRecordDelegates>]]> but uses no caching</summary>
    public static IProvider<Type, IResult<IRecordDelegates>> CreateResultFromType => createResultFromType;
    /// <summary>Constructs <![CDATA[IResult<IRecordDelegates>]]> and caches reults with errors captured in a weak hashmap.</summary>
    public static IProvider<Type, IResult<IRecordDelegates>> CachedResultFromType => cachedResultFromType;


    /// <summary></summary>
    protected IProvider<IFieldDescription, IResult<IFieldDelegates>> fieldDelegatesProvider;

    /// <summary></summary>
    public RecordDelegatesProvider(IProvider<IFieldDescription, IResult<IFieldDelegates>> fieldDelegatesProvider)
    {
        this.fieldDelegatesProvider = fieldDelegatesProvider;
    }

    /// <summary></summary>
    public override bool TryGetValue(IRecordDescription recordDescription, out IRecordDelegates recordDelegates)
    {
        //
        Type? recordType = recordDescription.Type;
        //
        if (recordType == null) throw new ArgumentException(nameof(recordDescription));
        //
        recordDelegates = RecordDelegates.Create(recordType);
        //
        recordDelegates.RecordCreate = ((IConstructionDescription)recordDescription.Construction!).TryCreateCreateFunc(out Delegate? createRecord) ? createRecord : null;
        recordDelegates.RecordDescription = recordDescription;
        // Create field delegates
        recordDelegates.FieldDelegates = new IFieldDelegates[recordDescription.Fields.Length];
        // Assign each field delegates
        for (int i = 0; i < recordDescription.Fields.Length; i++)
        {
            // Get field description
            IFieldDescription fieldDescription = recordDescription.Fields[i];
            // Get field delegates
            recordDelegates.FieldDelegates[i] = fieldDelegatesProvider[fieldDescription].Value!;
        }
        //
        return true;
    }
}

