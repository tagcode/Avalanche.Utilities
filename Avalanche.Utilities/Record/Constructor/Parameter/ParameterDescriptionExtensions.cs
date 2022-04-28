// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Reflection;
using System.Runtime.InteropServices;

/// <summary>Extension methods for <see cref="IParameterDescription"/>.</summary>
public static class ParameterDescriptionExtensions_
{
    /// <summary>Read parameter info from <paramref name="parameterInfo"/> and write to <paramref name="parameterDescription"/>.</summary>
    /// <exception cref="ArgumentException">If <paramref name="parameterInfo"/> is not expected type.</exception>
    public static T Read<T>(this T parameterDescription, object parameterInfo) where T : IParameterDescription
    {
        // Get annotations
        object[] annotations = (parameterInfo as MemberInfo)?.GetCustomAttributes(true) ?? (parameterInfo as ParameterInfo)?.GetCustomAttributes(true) ?? Array.Empty<object>();
        // Assign annotations
        parameterDescription.Annotations = annotations;
        //
        bool hasOptionalAttribute = false;
        //
        foreach (object annotation in annotations) hasOptionalAttribute |= annotation is OptionalAttribute;

        // Handle field info
        if (parameterInfo is FieldInfo fi1)
        {
            // Assign type 
            parameterDescription.Type = fi1.FieldType;
            // Assign name
            parameterDescription.Name = fi1.Name ?? "";
            // Assign constructor
            parameterDescription.Writer = fi1;
            // Assign optional
            parameterDescription.Optional = hasOptionalAttribute;
            //
            return parameterDescription;
        }

        // Handle property info
        if (parameterInfo is PropertyInfo pi1)
        {
            // Assign type 
            parameterDescription.Type = pi1.PropertyType;
            // Assign name
            parameterDescription.Name = pi1.Name ?? "";
            // Assign constructor
            parameterDescription.Writer = pi1;
            // Assign optional
            parameterDescription.Optional = hasOptionalAttribute;
            //
            return parameterDescription;
        }

        // Handle parameter info
        else if (parameterInfo is ParameterInfo pi2)
        {
            // Assign type 
            parameterDescription.Type = pi2.ParameterType;
            // Assign name
            parameterDescription.Name = pi2.Name ?? "";
            // Assign constructor
            parameterDescription.Writer = pi2;
            // Assign optional
            parameterDescription.Optional = hasOptionalAttribute || pi2.IsOptional;
            //
            return parameterDescription;
        }

        // Not supported
        throw new InvalidOperationException($"{parameterInfo.GetType()} not supported.");
    }

    /// <summary>Clone <paramref name="src"/> in writable state.</summary>
    public static ParameterDescription Clone(this IParameterDescription src)
    {
        // Create result
        ParameterDescription result = new ParameterDescription();
        //
        result.Name = src.Name;
        result.Type = src.Type;
        result.Writer = src.Writer;
        result.SetAnnotations(src.Annotations);
        result.Member = src.Member;
        result.Optional = src.Optional;
        // Return
        return result;
    }

    /// <summary>Calculate hash</summary>
    public static void HashIn(this IParameterDescription parameterDescription, ref FNVHash64 hash, bool hashName = true, bool hashType = true, bool hashConstructor = true, bool hashDeconstructor = true, bool hashAnnotations = true, bool hashOptional = true)
    {
        // Hash Name
        if (hashName && parameterDescription.Name != null) hash.HashIn(parameterDescription.Name.GetHashCode());
        //
        if (hashType) hash.HashIn(parameterDescription.Type?.FullName);
        // Hash-in constructor
        if (hashConstructor && parameterDescription.Writer != null) hash.HashIn(parameterDescription.Writer.GetHashCode());
        // Hash annotations
        if (hashAnnotations && parameterDescription.Annotations != null)
        {
            foreach (var annotation in parameterDescription.Annotations)
            {
                hash.HashIn(annotation.GetHashCode());
            }
        }
        // Optional
        if (hashOptional && parameterDescription.Optional is bool _optional)
        {
            hash.HashIn(_optional ? 452342 : 74575);
        }
    }

    /// <summary>Calculate hash64</summary>
    public static ulong CalcHash64(this IParameterDescription parameterDescription)
    {
        // Init
        FNVHash64 hash = new FNVHash64();
        // Hashin
        parameterDescription.HashIn(ref hash);
        // Return
        return hash.Hash;
    }

}
