// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

/// <summary>Extension methods for <see cref="IFieldDescription"/>.</summary>
public static class FieldDescriptionExtensions
{
    /// <summary></summary>
    static IFieldDescription[] noFields = new IFieldDescription[0];
    /// <summary></summary>
    public static IFieldDescription[] NO_FIELDS => noFields;

    /// <summary>Set field name</summary>
    public static T SetName<T>(this T fieldDescription, object value) where T : IFieldDescription { fieldDescription.Name = value; return fieldDescription; }
    /// <summary>Set field type</summary>
    public static T SetType<T>(this T fieldDescription, Type value) where T : IFieldDescription { fieldDescription.Type = value; return fieldDescription; }
    /// <summary>Set field writer: <see cref="ParameterInfo"/>, <see cref="FieldInfo"/>, <see cref="PropertyInfo"/>, <see cref="MethodInfo"/>, <see cref="Delegate"/>, <![CDATA[IWriterBase]]>, <![CDATA[IRefererBase]]></summary>
    public static T SetWriter<T>(this T fieldDescription, Object? value) where T : IFieldDescription { fieldDescription.Writer = value; return fieldDescription; }
    /// <summary>Set field reader: <see cref="FieldInfo"/>, <see cref="PropertyInfo"/>, <see cref="MethodInfo"/>, <see cref="Delegate"/>, <![CDATA[IWriterBase]]>, <![CDATA[IRefererBase]]></summary>
    public static T SetReader<T>(this T fieldDescription, Object? value) where T : IFieldDescription { fieldDescription.Reader = value; return fieldDescription; }
    /// <summary>Set field referer: <see cref="FieldInfo"/>></summary>
    public static T SetReferer<T>(this T fieldDescription, Object? value) where T : IFieldDescription { fieldDescription.Referer = value; return fieldDescription; }
    /// <summary>Annotations, such as <see cref="Attribute"/></summary>
    public static T SetAnnotations<T>(this T fieldDescription, IEnumerable<object> value) where T : IFieldDescription { fieldDescription.Annotations = value.ToArray(); return fieldDescription; }
    /// <summary>Set record which field is member of.</summary>
    public static T SetRecord<T>(this T fieldDescription, IRecordDescription? value) where T : IFieldDescription { fieldDescription.Record = value; return fieldDescription; }
    /// <summary>Set initial value.</summary>
    public static T SetInitialValue<T>(this T fieldDescription, object? value) where T : IFieldDescription { fieldDescription.InitialValue = value; return fieldDescription; }

    /// <summary>Get field by name</summary>
    /// <param name="fieldDescriptions"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static IFieldDescription GetByName(this IEnumerable<IFieldDescription> fieldDescriptions, object name)
    {
        // No field descriptions
        if (fieldDescriptions == null) throw new ArgumentNullException(nameof(fieldDescriptions));
        // Iterate each
        foreach (IFieldDescription fieldDescription in fieldDescriptions)
        {
            //
            object? elementName = fieldDescription.Name;
            //
            if ((elementName == null) != (name == null)) continue;
            //
            if (elementName == null && name == null) return fieldDescription;
            // Match
            if (elementName!.Equals(name) || name!.Equals(elementName)) return fieldDescription;
        }
        // Not found
        throw new KeyNotFoundException(name.ToString());
    }
    /*
    /// <summary>Get field by name</summary>
    /// <param name="fieldDescriptions"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static IFieldDescription GetByName(this IEnumerable<IFieldDescription> fieldDescriptions, string name)
    {
        // No field descriptions
        if (fieldDescriptions == null) throw new ArgumentNullException(nameof(fieldDescriptions));
        // Iterate each
        foreach (IFieldDescription fieldDescription in fieldDescriptions)
        {
            // Print to string
            string? nameString = fieldDescription.Name?.ToString();
            // Match
            if (name == nameString) return fieldDescription;
        }
        // Not found
        throw new KeyNotFoundException(name.ToString());
    }
    */
    /// <summary>Get field by name</summary>
    /// <param name="fieldDescriptions"></param>
    /// <param name="name"></param>
    /// <returns>Index or -1</returns>
    public static int GetIndexByName(this IEnumerable<IFieldDescription> fieldDescriptions, object name)
    {
        // No field descriptions
        if (fieldDescriptions == null) throw new ArgumentNullException(nameof(fieldDescriptions));
        //
        int ix = 0;
        // Iterate each
        foreach (IFieldDescription fieldDescription in fieldDescriptions)
        {
            //
            object? elementName = fieldDescription.Name;
            //
            if ((elementName == null) != (name == null)) continue;
            //
            if (elementName == null && name == null) return ix;
            // Match
            if (elementName!.Equals(name) || name!.Equals(elementName)) return ix;
            //
            ix++;
        }
        // Not found
        return -1;
    }
    /*
    /// <summary>Get field by name</summary>
    /// <param name="fieldDescriptions"></param>
    /// <param name="name"></param>
    /// <returns>Index or -1</returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static int GetIndexByName(this IEnumerable<IFieldDescription> fieldDescriptions, string name)
    {
        // No field descriptions
        if (fieldDescriptions == null) throw new ArgumentNullException(nameof(fieldDescriptions));
        //
        int ix = 0;
        // Iterate each
        foreach (IFieldDescription fieldDescription in fieldDescriptions)
        {
            // Print to string
            string? nameString = fieldDescription.Name?.ToString();
            // Match
            if (name == nameString) return ix;
            //
            ix++;
        }
        // Not found
        return -1;
    }
    */
    /// <summary>Try Get field by name</summary>
    public static bool TryGetByName(this IEnumerable<IFieldDescription> fieldDescriptions, object name, [NotNullWhen(true)] out IFieldDescription result)
    {
        // No field descriptions
        if (fieldDescriptions == null) { result = null!; return false; }
        // Iterate each
        foreach (IFieldDescription fieldDescription in fieldDescriptions)
        {
            //
            object? elementName = fieldDescription.Name;
            //
            if ((elementName == null) != (name == null)) continue;
            //
            if (elementName == null && name == null) { result = fieldDescription; return true; }
            // Match
            if (elementName!.Equals(name) || name!.Equals(elementName)) { result = fieldDescription; return true; }
        }
        // Not found
        result = null!;
        return false;
    }
    /*
    /// <summary>Try get field by name</summary>
    public static bool TryGetByName(this IEnumerable<IFieldDescription> fieldDescriptions, string name, [NotNullWhen(true)] out IFieldDescription result)
    {
        // No field descriptions
        if (fieldDescriptions == null) { result = null!; return false; }
        // Iterate each
        foreach (IFieldDescription fieldDescription in fieldDescriptions)
        {
            // Print to string
            string? nameString = fieldDescription.Name?.ToString();
            // Match
            if (name == nameString) { result = fieldDescription; return true; }
        }
        // Not found
        result = null!;
        return false;
    }
    */
}
