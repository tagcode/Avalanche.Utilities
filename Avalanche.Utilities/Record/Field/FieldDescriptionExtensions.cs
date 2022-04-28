// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Reflection;

/// <summary>Extension methods for <see cref="IFieldDescription"/>.</summary>
public static class FieldDescriptionExtensions_
{
    /// <summary>Read field info from <paramref name="fieldInfo"/> and write to <paramref name="fieldDescription"/>.</summary>
    /// <exception cref="ArgumentException">If <paramref name="fieldInfo"/> is not expected type.</exception>
    public static T Read<T>(this T fieldDescription, object fieldInfo) where T : IFieldDescription
    {
        // Get annotations
        object[] annotations = (fieldInfo as MemberInfo)?.GetCustomAttributes(true) ?? Array.Empty<object>();
        // Assign annotations
        fieldDescription.Annotations = annotations;

        // Handle field info
        if (fieldInfo is FieldInfo fi1)
        {
            // Assign type 
            fieldDescription.Type = fi1.FieldType;
            // Assign name
            fieldDescription.Name = fi1.Name ?? "";
            // Assign reader
            fieldDescription.Reader = fi1;
            // Assign writer
            if (!fi1.IsInitOnly) fieldDescription.Writer = fi1;
            // Assign referer
            fieldDescription.Referer = fi1;
            //
            return fieldDescription;
        }

        // Handle property info
        if (fieldInfo is PropertyInfo pi1)
        {
            // Assign type 
            fieldDescription.Type = pi1.PropertyType;
            // Assign name
            fieldDescription.Name = pi1.Name ?? "";
            // Assign reader
            if (pi1.GetGetMethod() != null && pi1.CanRead) fieldDescription.Reader = pi1;
            // Assign writer
            if (pi1.GetSetMethod() != null) fieldDescription.Writer = pi1;
            //
            return fieldDescription;
        }

        // Handle parameter info
        if (fieldInfo is ParameterInfo pi2)
        {
            // Assign type 
            fieldDescription.Type = pi2.ParameterType;
            // Assign name
            fieldDescription.Name = pi2.Name ?? "";
            // Assign writer
            fieldDescription.Writer = pi2;
            //
            return fieldDescription;
        }

        // Not supported
        throw new InvalidOperationException($"{fieldInfo.GetType()} not supported.");
    }

    /// <summary>Get record type.</summary>
    public static Type? RecordType(this IFieldDescription fieldDescription)
    {
        // Get type
        Type? recordType = fieldDescription?.Record?.Type;
        // Got type
        if (recordType != null) return recordType;
        // Got member info
        if (fieldDescription?.Reader is MemberInfo mi && mi.ReflectedType != null) return mi.ReflectedType!;
        // Got parameter info
        if (fieldDescription?.Reader is ParameterInfo pi && pi.Member != null && pi.Member.ReflectedType != null) return pi.Member.ReflectedType;
        // No associated record.
        return null;
    }

    /// <summary>Clone <paramref name="fieldDescription"/> in writable state.</summary>
    public static FieldDescription Clone(this IFieldDescription fieldDescription)
    {
        // Create result
        FieldDescription result = new FieldDescription();
        //
        result.Name = fieldDescription.Name;
        result.Type = fieldDescription.Type;
        result.Reader = fieldDescription.Reader;
        result.Writer = fieldDescription.Writer;
        result.Referer = fieldDescription.Referer;
        result.Annotations = fieldDescription.Annotations;
        result.Record = fieldDescription.Record;
        result.InitialValue = fieldDescription.InitialValue;
        // Return
        return result;
    }


    /// <summary>Calculate hash</summary>
    public static void HashIn(this IFieldDescription fieldDescription, ref FNVHash64 hash, bool hashName = true, bool hashType = true, bool hashConstructor = true, bool hashDeconstructor = true, bool hashAnnotations = true)
    {
        // Hash Name
        if (hashName && fieldDescription.Name != null) hash.HashIn(fieldDescription.Name.GetHashCode());
        //
        if (hashType) hash.HashIn(fieldDescription.Type?.FullName);
        // Hash-in writer
        if (hashConstructor && fieldDescription.Writer != null) hash.HashIn(fieldDescription.Writer.GetHashCode());
        // Hash-in reader
        if (hashDeconstructor && fieldDescription.Reader != null) hash.HashIn(fieldDescription.Reader.GetHashCode());
        // Hash-in referer
        if (hashDeconstructor && fieldDescription.Referer != null) hash.HashIn(fieldDescription.Referer.GetHashCode());
        // Hash annotations
        if (hashAnnotations && fieldDescription.Annotations != null)
        {
            foreach (var annotation in fieldDescription.Annotations)
            {
                hash.HashIn(annotation.GetHashCode());
            }
        }
        // Hash-in initial value
        if (fieldDescription.InitialValue != null)
        {
            hash.HashIn(fieldDescription.InitialValue.GetHashCode());
        }
    }

    /// <summary>Calculate hash64</summary>
    public static ulong CalcHash64(this IFieldDescription fieldDescription)
    {
        // Init
        FNVHash64 hash = new FNVHash64();
        // Hashin
        fieldDescription.HashIn(ref hash);
        // Return
        return hash.Hash;
    }

}
