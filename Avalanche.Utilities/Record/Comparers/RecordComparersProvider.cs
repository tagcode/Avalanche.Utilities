// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Avalanche.Utilities.Provider;
using Avalanche.Utilities.Record;

/// <summary>Provides <see cref="IComparers"/> for records.</summary>
public class RecordComparersProvider : ProviderBase<IRecordDescription, IComparers>
{
    /// <summary>Comparers constructor</summary>
    static readonly IProvider<IRecordDescription, IComparers> create = new RecordComparersProvider();
    /// <summary>Comparers constructor</summary>
    static readonly IProvider<IRecordDescription, IResult<IComparers>> createResult = create.ResultCaptured();
    /// <summary>Comparers provider from type</summary>
    static readonly IProvider<Type, IResult<IComparers>> createResultFromType = new ResultProviderConcat<Type, IRecordDescription, IComparers>(RecordDescription.CachedResult, createResult);
    /// <summary>Comparers provider from type</summary>
    static readonly IProvider<Type, IComparers> createFromType = createResultFromType.ResultOpened();
    /// <summary>Comparers provider from type, cached</summary>
    static readonly IProvider<Type, IResult<IComparers>> cachedResultFromType = createResultFromType.AsReadOnly().WeakCached();
    /// <summary>Comparers provider from type, cached</summary>
    static readonly IProvider<Type, IComparers> cachedFromType = cachedResultFromType.ResultOpened();

    /// <summary>Comparers constructor</summary>
    public static IProvider<IRecordDescription, IComparers> Create => create;
    /// <summary>Comparers constructor</summary>
    public static IProvider<IRecordDescription, IResult<IComparers>> CreateResult => createResult;
    /// <summary>Comparers provider from type</summary>
    public static IProvider<Type, IResult<IComparers>> CreateResultFromType => createResultFromType;
    /// <summary>Comparers provider from type</summary>
    public static IProvider<Type, IComparers> CreateFromType => createFromType;
    /// <summary>Comparers provider from type, cached</summary>
    public static IProvider<Type, IResult<IComparers>> CachedResultFromType => cachedResultFromType;
    /// <summary>Comparers provider from type, cached</summary>
    public static IProvider<Type, IComparers> CachedFromType => cachedFromType;

    /// <summary>Provides objects that implement <see cref="IEqualityComparer"/> and <see cref="IGraphEqualityComparable"/></summary>
    protected IProvider<Type, IEqualityComparer> elementEqualityComparer;
    /// <summary>Provides objects that implement <see cref="IComparer"/> and <see cref="IGraphComparable"/></summary>
    protected IProvider<Type, IComparer> elementComparer;

    /// <summary></summary>
    /// <param name="elementEqualityComparer">Provides objects that implement <see cref="IEqualityComparer"/> and <see cref="IGraphEqualityComparable"/></param>
    /// <param name="elementComparer">Provides objects that implement <see cref="IComparer"/> and <see cref="IGraphComparable"/></param>
    public RecordComparersProvider(IProvider<Type, IEqualityComparer>? elementEqualityComparer = default, IProvider<Type, IComparer>? elementComparer = default)
    {
        this.elementEqualityComparer = elementEqualityComparer ?? DefaultEqualityComparerProvider.Create;
        this.elementComparer = elementComparer ?? DefaultComparerProvider.Create;
    }

    /// <summary>Create contract</summary>
    /// <param name="recordDescription"></param>
    /// <param name="contract"></param>
    /// <returns></returns>
    public override bool TryGetValue(IRecordDescription recordDescription, out IComparers contract)
    {
        //
        Type intf = recordDescription.Type;
        // Place here cyclic info
        bool cyclic = false;
        // Place fields here
        StructList8<IFieldDescription> fields = new();
        // Qualify fields
        foreach (var fieldDescription in recordDescription.Fields)
        {
            // Get field
            bool ok = true;
            // Check annotations
            foreach (object annotation in fieldDescription.Annotations)
            {
                ok &= annotation is not IgnoreCompareAttribute;
                ok &= annotation is not IgnoreDataMemberAttribute;
            }
            // Field is disqualified
            if (!ok) continue;
            // Test if field can cause cycles
            if (!fieldDescription.Type.Equals(typeof(object)))
            {
                if (!cyclic) cyclic |= fieldDescription.Type.IsAssignableTo(intf);
                if (!cyclic && TypeUtilities.TryGetTypeArgumentOfCorrespondingDefinedType(fieldDescription.Type, typeof(IEnumerable<>), 0, out Type? _valueType)) cyclic |= _valueType.IsAssignableTo(intf);
            }
            // Ok
            fields.Add(fieldDescription);
        }

        // Create field comparers
        IEqualityComparer[] fieldEqualityComparers = new IEqualityComparer[fields.Count];
        IComparer[] fieldComparers = new IComparer[fields.Count];

        // Assign field comparers
        for (int i = 0; i < fields.Count; i++)
        {
            // Get field description
            IFieldDescription fieldDescription = fields[i];
            Type valueType = fieldDescription.Type ?? throw new ArgumentNullException("value type");
            //
            if (!FieldReadFunc.TryCreateFieldReadFunc(fieldDescription, out Delegate? fieldReader)) throw new InvalidOperationException($"Could not create field reader: {fieldDescription}");
            // Place here value comparers
            object? valueEqualityComparer = elementEqualityComparer[valueType];
            object? valueComparer = elementComparer[valueType];
            // Create comparers
            fieldEqualityComparers[i] = DelegateEqualityComparer.Create(intf, fieldDescription.Type, valueEqualityComparer, fieldReader);
            fieldComparers[i] = DelegateComparer.Create(intf, fieldDescription.Type, valueComparer, fieldReader);
        }
        // Create record comparer
        IEqualityComparer equalityComparer = RecordEqualityComparer.Create(recordDescription.Type, fieldEqualityComparers).SetReadOnly();
        IComparer comparer = RecordComparer.Create(recordDescription.Type, fieldComparers).SetReadOnly();

        // Create contract
        contract = Comparers
            .Create(intf)
            .SetCyclic(cyclic)
            .SetEqualityComparer(equalityComparer)
            .SetEqualityComparerT(equalityComparer)
            .SetComparer(comparer)
            .SetComparerT(comparer)
            .SetGraphEqualityComparer((IGraphEqualityComparer)equalityComparer)
            .SetGraphEqualityComparerT((IGraphEqualityComparer)equalityComparer)
            .SetGraphComparer((IGraphComparer)comparer)
            .SetGraphComparerT((IGraphComparer)comparer);
        // Return
        return true;
    }
}


