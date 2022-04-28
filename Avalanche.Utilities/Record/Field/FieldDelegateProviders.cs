// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using Avalanche.Utilities.Provider;

/// <summary></summary>
public static class FieldDelegateProviders
{
    /// <summary>C</summary>
    static readonly IProvider<IFieldDescription, IFieldDelegates> create = new FieldDelegatesProvider(FieldRead.CachedResult, FieldWrite.CachedResult, RecreateWith.CachedResult);
    /// <summary>C</summary>
    static readonly IProvider<IFieldDescription, IResult<IFieldDelegates>> createResult = create.ResultCaptured();
    /// <summary>Weak cached</summary>
    static readonly IProvider<IFieldDescription, IResult<IFieldDelegates>> cachedResult = CreateResult.AsReadOnly().WeakCached<IFieldDescription, IResult<IFieldDelegates>>();
    /// <summary>Weak cached</summary>
    static readonly IProvider<IFieldDescription, IFieldDelegates> cached = cachedResult.ResultOpened();

    /// <summary>Field description constructor</summary>
    public static IProvider<IFieldDescription, IFieldDelegates> Create => create;
    /// <summary>Field description constructor</summary>
    public static IProvider<IFieldDescription, IResult<IFieldDelegates>> CreateResult => createResult;
    /// <summary>Field description contructor with weak cache</summary>
    public static IProvider<IFieldDescription, IResult<IFieldDelegates>> CachedResult => cachedResult;
    /// <summary>Field description contructor with weak cache</summary>
    public static IProvider<IFieldDescription, IFieldDelegates> Cached => cached;
}

/// <summary>Creates <see cref="IFieldDelegates"/>.</summary>
public class FieldDelegatesProvider : ProviderBase<IFieldDescription, IFieldDelegates>, IProviderStack
{
    /// <summary><see cref="FieldRead{Record, Field}"/></summary>
    protected IProvider<IFieldDescription, IResult<Delegate>>? readFieldProvider;
    /// <summary><see cref="FieldWrite{Record, Field}"/></summary>
    protected IProvider<IFieldDescription, IResult<Delegate>>? writeFieldProvider;
    /// <summary><see cref="RecreateWith{Record, Field}"/></summary>
    protected IProvider<IFieldDescription, IResult<Delegate>>? recreateWithProvider;

    /// <summary></summary>
    public IProvider[]? Sources
    {
        get
        {
            StructList3<IProvider> list = new();
            if (readFieldProvider != null) list.Add(readFieldProvider);
            if (writeFieldProvider != null) list.Add(writeFieldProvider);
            if (recreateWithProvider != null) list.Add(recreateWithProvider);
            return list.ToArray();
        }
    }

    /// <summary>Create provider</summary>
    /// <param name="readFieldProvider"></param>
    /// <param name="writeFieldProvider"></param>
    /// <param name="recreateWithProvider"></param>
    public FieldDelegatesProvider(IProvider<IFieldDescription, IResult<Delegate>>? readFieldProvider, IProvider<IFieldDescription, IResult<Delegate>>? writeFieldProvider, IProvider<IFieldDescription, IResult<Delegate>>? recreateWithProvider)
    {
        this.readFieldProvider = readFieldProvider;
        this.writeFieldProvider = writeFieldProvider;
        this.recreateWithProvider = recreateWithProvider;
    }

    /// <summary>Create record description</summary>
    public override bool TryGetValue(IFieldDescription fieldDescription, out IFieldDelegates result)
    {
        //
        FieldDelegates delegates = FieldDelegates.Create(fieldDescription.Record!.Type, fieldDescription.Type);
        // Assign 
        delegates.FieldDescription = fieldDescription;
        delegates.FieldRead = readFieldProvider?[fieldDescription]?.Value;
        delegates.FieldWrite = writeFieldProvider?[fieldDescription]?.Value;
        delegates.RecreateWith = recreateWithProvider?[fieldDescription]?.Value;
        // Make read-only
        delegates.SetReadOnly();
        // Assign
        result = delegates;
        return true;
    }
}

