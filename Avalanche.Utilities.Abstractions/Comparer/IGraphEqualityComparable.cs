// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Runtime.Serialization;

// <docs>
/// <summary>Graph equality comparable. Detects cycles.</summary>
public interface IGraphEqualityComparable
{
    /// <summary>Is object structure cyclical.</summary>
    [IgnoreDataMember] bool IsCyclical { get; set; }

    /// <summary>Compares for equality to <paramref name="obj"/>. Detects object cycles.</summary>
    /// <param name="obj"></param>
    /// <param name="context">Compare context</param>
    /// <returns></returns>
    bool EqualTo(object? obj, IGraphComparerContext2 context);

    /// <summary>Return a hash code in graph traverse aware request.</summary>
    /// <param name="context">Compare context</param>
    int GetHashCode(IGraphComparerContext context);
}
// </docs>

// <docsT>
/// <summary>Graph equality comparable. Detects cycles.</summary>
/// <remarks>
/// Implementation may be start node distinctive or agnostic. 
/// If former, then implementation typically uses order specific hashing, e.g. FNV. 
/// If later, then uses add or xor hashing between objects.
/// </remarks>
public interface IGraphEqualityComparable<in T>
{
    /// <summary>Is object structure cyclical.</summary>
    [IgnoreDataMember] bool IsCyclical { get; set; }

    /// <summary>Compares for equality to <paramref name="obj"/>. Detects object cycles.</summary>
    /// <param name="obj"></param>
    /// <param name="context">Compare context</param>
    /// <returns></returns>
    bool EqualTo(T? obj, IGraphComparerContext2 context);

    /// <summary>Return a hash code in graph traverse aware request.</summary>
    /// <param name="context">Compare context</param>
    int GetHashCode(IGraphComparerContext context);
}
// </docsT>
