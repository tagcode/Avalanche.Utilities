// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Runtime.Serialization;

// <docs>
/// <summary>Graph comparable object. Detects cycles.</summary>
public interface IGraphComparable
{
    /// <summary>Is object structure cyclical.</summary>
    [IgnoreDataMember] bool IsCyclical { get; set; }

    /// <summary>Compares to <paramref name="obj"/>.</summary>
    /// <param name="obj"></param>
    /// <param name="context">Compare context for tracking already compared pairs</param>
    /// <returns>
    /// Signed interger that indicates relative value to <paramref name="obj"/>. 
    ///     <![CDATA[<0]]> if <paramref name="obj"/> is preceding.
    ///     <![CDATA[>0]]> if <paramref name="obj"/> is trailing.
    ///     <![CDATA[0]]> equals.
    /// </returns>
    int CompareTo(object obj, IGraphComparerContext2 context);
}
// </docs>

// <docsT>
/// <summary>Graph comparable obejct. Detects cycles.</summary>
/// <remarks>
/// Implementation may be start node distinctive or agnostic. 
/// If former, then implementation typically uses order specific hashing, e.g. FNV. 
/// If later, then uses add or xor hashing between objects.
/// </remarks>
public interface IGraphComparable<in T>
{
    /// <summary>Is object structure cyclical.</summary>
    [IgnoreDataMember] bool IsCyclical { get; set; }

    /// <summary>Compares to <paramref name="obj"/>.</summary>
    /// <param name="obj"></param>
    /// <param name="context">Compare context for tracking already compared pairs</param>
    /// <returns>
    /// Signed interger that indicates relative value to <paramref name="obj"/>. 
    ///     <![CDATA[<0]]> if <paramref name="obj"/> is preceding.
    ///     <![CDATA[>0]]> if <paramref name="obj"/> is trailing.
    ///     <![CDATA[0]]> equals.
    /// </returns>
    int CompareTo(T? obj, IGraphComparerContext2 context);
}
// </docsT>
