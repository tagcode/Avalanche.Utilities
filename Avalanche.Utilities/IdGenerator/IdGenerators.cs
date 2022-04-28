// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

/// <summary></summary>
public static class IdGenerators
{
    /// <summary></summary>
    public static IIdGenerator<int> Integer => IntegerGenerator.Instance;
    /// <summary></summary>
    public static IIdGenerator<long> Long => LongGenerator.Instance;
    /// <summary></summary>
    public static IIdGenerator<Guid> Guid => GuidGenerator.Instance;
}
