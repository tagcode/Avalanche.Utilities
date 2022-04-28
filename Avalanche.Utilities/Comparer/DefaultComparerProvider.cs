// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using Avalanche.Utilities.Provider;

/// <summary></summary>
public class DefaultComparerProvider : ProviderBase<Type, IComparer>
{
    /// <summary></summary>
    static readonly IProvider<Type, IComparer> create = new DefaultComparerProvider();
    /// <summary></summary>
    static readonly IProvider<Type, IResult<IComparer>> createResult = create.ResultCaptured();
    /// <summary></summary>
    static readonly IProvider<Type, IResult<IComparer>> createResultCached = createResult.Cached();
    /// <summary></summary>
    static readonly IProvider<Type, IComparer> cached = createResultCached.ResultOpened();
    /// <summary></summary>
    public static IProvider<Type, IComparer> Create => create;
    /// <summary></summary>
    public static IProvider<Type, IComparer> Cached => cached;

    /// <summary></summary>
    protected IProvider<Type, IComparer> rootProvider;

    /// <summary></summary>
    public DefaultComparerProvider(IProvider<Type, IComparer>? rootProvider = default)
    {
        this.rootProvider = rootProvider ?? this;
    }

    /// <summary></summary>
    public override bool TryGetValue(Type type, out IComparer comparer)
    {
        // T[]
        if (type.IsArray && type.GetArrayRank() == 1)
        {
            // Get element type
            Type elementType = type.GetElementType()!;
            // Get element comparer
            if (!rootProvider.TryGetValue(elementType, out IComparer elementComparer)) { comparer = null!; return false; }
            // Create comparer
            comparer = EnumerableGraphComparer.Create(elementType, elementComparer);
            // Ok
            return true;
        }

        // IEnumerable<T>
        if (TypeUtilities.TryGetTypeArgumentOfCorrespondingDefinedType(type, typeof(IEnumerable<>), 0, out Type elementType_))
        {
            // Get element comparer
            if (!rootProvider.TryGetValue(elementType_, out IComparer elementComparer)) { comparer = null!; return false; }
            // Create comparer
            comparer = EnumerableGraphComparer.Create(elementType_, elementComparer);
            // Ok
            return true;
        }

        // object
        comparer = type.IsAssignableTo(typeof(IGraphComparable)) ? (IComparer)GraphComparer.Create(type) : (IComparer)ComparerProvider.Get(type);
        // Ok
        return true;
    }
}

