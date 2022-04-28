// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Diagnostics.CodeAnalysis;

/// <summary></summary>
public static class NullableUtilities
{
    /// <summary>Tests whether <paramref name="possiblyNullableValueType"/> is value type and <see cref="Nullable{T}"/>.</summary>
    public static bool IsNullableValueType(Type possiblyNullableValueType)
    {
        // Not value type
        if (!possiblyNullableValueType.IsValueType) return false;
        // Not generic
        if (!possiblyNullableValueType.IsGenericType) return false;
        // Not Nullable<>
        if (!typeof(Nullable<>).Equals(possiblyNullableValueType.GetGenericTypeDefinition())) return false;
        // Got Nullable<T>
        return true;
    }

    /// <summary>Get T of <see cref="Nullable{T}"/>.</summary>
    public static Type? GetNullableValueType(Type possiblyNullableValueType)
    {
        // Not value type
        if (!possiblyNullableValueType.IsValueType) return null;
        // Not generic
        if (!possiblyNullableValueType.IsGenericType) return null;
        // Not Nullable<>
        if (!typeof(Nullable<>).Equals(possiblyNullableValueType.GetGenericTypeDefinition())) return null;
        // Get T
        Type? valueType = possiblyNullableValueType.GetGenericArguments()[0];
        // Got Nullable<T>
        return valueType;
    }

    /// <summary>Get T of <see cref="Nullable{T}"/>.</summary>
    public static bool TryGetNullableValueType(Type possiblyNullableValueType, [NotNullWhen(false)] out Type valueType)
    {
        // Not value type
        if (!possiblyNullableValueType.IsValueType) { valueType = null!; return false; }
        // Not generic
        if (!possiblyNullableValueType.IsGenericType) { valueType = null!; return false; }
        // Not Nullable<>
        if (!typeof(Nullable<>).Equals(possiblyNullableValueType.GetGenericTypeDefinition())) { valueType = null!; return false; }
        // Get T
        valueType = possiblyNullableValueType.GetGenericArguments()[0];
        // Got Nullable<T>
        return true;
    }

}
