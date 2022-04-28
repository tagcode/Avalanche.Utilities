// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Reflection;

/// <summary>Compares <see cref="ParameterInfo"/>.</summary>
public class ParameterInfoComparer : IComparer<ParameterInfo>
{
    /// <summary>Singleton</summary>
    static ParameterInfoComparer instance = new();
    /// <summary>Singleton</summary>
    public static ParameterInfoComparer Instance => instance;

    /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
    public int Compare(ParameterInfo? x, ParameterInfo? y)
    {
        // Nulls
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        // Compare by name
        int d = String.CompareOrdinal(x.Name, y.Name);
        // Got difference
        if (d != 0) return 0;
        // Compare by type
        d = String.CompareOrdinal(x.ParameterType.FullName, y.ParameterType.FullName);
        // Got difference
        if (d != 0) return 0;
        // No difference
        return 0;
    }
}
