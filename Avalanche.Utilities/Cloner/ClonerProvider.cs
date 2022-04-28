// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using Avalanche.Utilities.Provider;
using Avalanche.Utilities.Record;

/// <summary>
/// Providers a cloner for types.
/// 
/// Uses following rules:
///     Arrays -> <see cref="ArrayCloner"/>,
///     <see cref="IRecord"/> -> <see cref="RecordCloner"/>
///     Others <see cref="PassthroughCloner"/>.
/// </summary>
public class ClonerProvider : ProviderBase<Type, ICloner>
{
    /// <summary></summary>
    static readonly IProvider<Type, ICloner> create = new ClonerProvider();
    /// <summary></summary>
    static readonly IProvider<Type, IResult<ICloner>> createResult = create.ResultCaptured();
    /// <summary></summary>
    static readonly IProvider<Type, IResult<ICloner>> cachedResult = createResult.WeakCached();
    /// <summary></summary>
    static readonly IProvider<Type, ICloner> cached = cachedResult.ResultOpened();
    /// <summary></summary>
    public static IProvider<Type, ICloner> Create => create;
    /// <summary></summary>
    public static IProvider<Type, IResult<ICloner>> CreateResult => createResult;
    /// <summary></summary>
    public static IProvider<Type, IResult<ICloner>> CachedResult => cachedResult;
    /// <summary></summary>
    public static IProvider<Type, ICloner> Cached => cached;

    /// <summary>Create cloner for <paramref name="type"/>.</summary>
    public override bool TryGetValue(Type type, out ICloner cloner)
    {
        //
        bool isCyclical = type.IsAssignableTo(typeof(ICyclical));
        // Primitive
        if (type.IsPrimitive)
        {
            cloner = PassthroughCloner.Create(type);
        }
        // Array cloner
        else if (type.IsArray)
        {
            // Rank = 1
            if (type.GetArrayRank() == 1)
            {
                cloner = ArrayCloner.Create(type.GetElementType()!, this).SetCyclical(isCyclical).SetReadOnly();
            }
            // Rank > 0
            else
            {
                cloner = PassthroughCloner.Create(type);
            }
        }
        // IList
        else if (type.IsAssignableTo(typeof(IList)) || TypeUtilities.TryGetTypeArgumentOfCorrespondingDefinedType(type, typeof(IList<>), 0, out Type elementType0))
        {
            cloner = ListCloner.Create(type, elementCloner: this).SetCyclical(isCyclical).SetReadOnly();
        }
        // ICloneable or IGraphCloneable
        else if (type.IsAssignableTo(typeof(ICloneable)) || type.IsAssignableTo(typeof(IGraphCloneable)))
        {
            cloner = Cloner.Create(type).SetCyclical(isCyclical).SetReadOnly();
        }
        // IEnumerable
        else if (type.IsAssignableTo(typeof(IEnumerable)) || TypeUtilities.TryGetTypeArgumentOfCorrespondingDefinedType(type, typeof(IEnumerable<>), 0, out Type elementType1))
        {
            cloner = EnumerableCloner.Create(type, elementCloner: this).SetCyclical(isCyclical).SetReadOnly();
        }
        // Record cloner
        else if (type.IsAssignableTo(typeof(IRecord)))
        {
            cloner = RecordCloner.Create(type).SetCyclical(isCyclical).SetReadOnly();
        }
        // Passthrough "cloner"
        else
        {
            cloner = PassthroughCloner.Create(type);
        }
        // Return
        return true;
    }
}

