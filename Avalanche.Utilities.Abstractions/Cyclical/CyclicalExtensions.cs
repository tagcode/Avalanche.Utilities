// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

/// <summary>Extension methods for <see cref="ICyclical"/>.</summary>
public static class CyclicalExtensions
{
    /// <summary>Set IsCyclical</summary>
    public static T SetCyclical<T>(this T record, bool isCyclical) where T : ICyclical { record.IsCyclical = isCyclical; return record; }
}

