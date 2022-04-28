// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

/// <summary>Extension methods for <see cref="IComparers"/>.</summary>
public static class ComparersExtensions_
{
    /// <summary>Create clone</summary>
    /// <returns>Clone in mutable state</returns>
    public static Comparers Clone(this IComparers src)
    {
        // Create contract
        Comparers result = Comparers
            .Create(src.Type)
            .SetCyclic(src.IsCyclical)
            .SetEqualityComparer(src.EqualityComparer)
            .SetEqualityComparerT(src.EqualityComparerT)
            .SetComparer(src.Comparer)
            .SetComparerT(src.ComparerT)
            .SetGraphEqualityComparer(src.GraphEqualityComparer)
            .SetGraphEqualityComparerT(src.GraphEqualityComparerT)
            .SetGraphComparer(src.GraphComparer)
            .SetGraphComparerT(src.GraphComparerT);
        // Return clone
        return result;
    }
}
