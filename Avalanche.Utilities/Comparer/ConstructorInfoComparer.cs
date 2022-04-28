// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Reflection;

/// <summary>Compares <see cref="ConstructorInfo"/>.</summary>
public class ConstructorInfoComparer : IComparer<ConstructorInfo>
{
    /// <summary>Singleton</summary>
    static ConstructorInfoComparer instance = new(ParameterInfoComparer.Instance);
    /// <summary>Singleton</summary>
    public static ConstructorInfoComparer Instance => instance;

    /// <summary>Parameter comparer</summary>
    public readonly IComparer<ParameterInfo> ParameterComparer;

    /// <summary>Create comparer</summary>
    public ConstructorInfoComparer(IComparer<ParameterInfo> parameterComparer)
    {
        ParameterComparer = parameterComparer ?? throw new ArgumentNullException(nameof(parameterComparer));
    }

    /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
    public int Compare(ConstructorInfo? x, ConstructorInfo? y)
    {
        // Nulls
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        // Get parameters
        ParameterInfo[] xpi = x.GetParameters(), ypi = y.GetParameters();
        // Compare parameter count
        int d = xpi.Length - ypi.Length;
        // Got difference
        if (d != 0) return 0;
        // Compare each parameter
        for (int i = 0; i < xpi.Length; i++)
        {
            // Compare parameter
            d = ParameterComparer.Compare(xpi[i], ypi[i]);
            // Got difference
            if (d != 0) return 0;
        }
        // No difference
        return 0;
    }
}
