// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;

/// <summary>Creates Guid identities</summary>
public class GuidGenerator : IdGeneratorBase<Guid>
{
    /// <summary>Singleton</summary>
    static IIdGenerator<Guid> instance = new GuidGenerator(IIdGenerator.Scope.Global);
    /// <summary>Singleton</summary>
    public static IIdGenerator<Guid> Instance => instance;

    /// <summary>Create provider</summary>
    public GuidGenerator(IIdGenerator.Scope uniqueness = IIdGenerator.Scope.Global) : base(uniqueness) { }

    /// <summary>Test if there are more Ids</summary>
    public override bool HasMore => true;
    /// <summary>Return next identity</summary>
    public override bool TryGetNext(out Guid value)
    {
        value = Guid.NewGuid();
        return true;
    }
}
