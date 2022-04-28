// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using Avalanche.Utilities.Provider;

/// <summary></summary>
public class DefaultEqualityComparerProvider : ProviderBase<Type, IEqualityComparer>
{
    /// <summary></summary>
    static readonly IProvider<Type, IEqualityComparer> create = new DefaultEqualityComparerProvider();
    /// <summary></summary>
    static readonly IProvider<Type, IResult<IEqualityComparer>> createResult = create.ResultCaptured();
    /// <summary></summary>
    static readonly IProvider<Type, IResult<IEqualityComparer>> createResultCached = createResult.Cached();
    /// <summary></summary>
    static readonly IProvider<Type, IEqualityComparer> cached = createResultCached.ResultOpened();
    /// <summary></summary>
    public static IProvider<Type, IEqualityComparer> Create => create;
    /// <summary></summary>
    public static IProvider<Type, IEqualityComparer> Cached => cached;

    /// <summary></summary>
    protected IProvider<Type, IEqualityComparer> rootProvider;

    /// <summary></summary>
    public DefaultEqualityComparerProvider(IProvider<Type, IEqualityComparer>? rootProvider = default)
    {
        this.rootProvider = rootProvider ?? this;
    }

    /// <summary></summary>
    public override bool TryGetValue(Type type, out IEqualityComparer comparer)
    {
        // T[]
        if (type.IsArray && type.GetArrayRank() == 1)
        {
            // Get element type
            Type elementType = type.GetElementType()!;
            // Get element comparer
            if (!rootProvider.TryGetValue(elementType, out IEqualityComparer elementComparer)) { comparer = null!; return false; }
            // Create comparer
            comparer = EnumerableGraphEqualityComparer.Create(elementType, elementComparer);
            // Ok
            return true;
        }

        // IEnumerable<T>
        if (TypeUtilities.TryGetTypeArgumentOfCorrespondingDefinedType(type, typeof(IEnumerable<>), 0, out Type elementType_))
        {
            // Get element comparer
            if (!rootProvider.TryGetValue(elementType_, out IEqualityComparer elementComparer)) { comparer = null!; return false; }
            // Create comparer
            comparer = EnumerableGraphEqualityComparer.Create(elementType_, elementComparer);
            // Ok
            return true;
        }

        // object
        comparer = type.IsAssignableTo(typeof(IGraphEqualityComparable)) ? (IEqualityComparer)GraphEqualityComparer.Create(type) : (IEqualityComparer)EqualityComparerProvider.Get(type);
        // Ok
        return true;
    }
}
