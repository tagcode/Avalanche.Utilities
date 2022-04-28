// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Reflection;

/// <summary>Extension methods for <see cref="IParameterDescription"/>.</summary>
public static class ParameterDescriptionExtensions
{
    /// <summary>Set parameter name</summary>
    public static T SetName<T>(this T parameterDescription, object value) where T : IParameterDescription { parameterDescription.Name = value; return parameterDescription; }
    /// <summary>Set parameter type</summary>
    public static T SetType<T>(this T parameterDescription, Type value) where T : IParameterDescription { parameterDescription.Type = value; return parameterDescription; }
    /// <summary>Set parameter writer: <see cref="ParameterInfo"/></summary>
    public static T SetWriter<T>(this T parameterDescription, Object? value) where T : IParameterDescription { parameterDescription.Writer = value; return parameterDescription; }
    /// <summary>Annotations, such as <see cref="Attribute"/></summary>
    public static T SetAnnotations<T>(this T parameterDescription, IEnumerable<object> value) where T : IParameterDescription { parameterDescription.Annotations = value.ToArray(); return parameterDescription; }
    /// <summary>Set record which parameter is member of.</summary>
    public static T SetMember<T>(this T parameterDescription, IConstructorDescription? value) where T : IParameterDescription { parameterDescription.Member = value; return parameterDescription; }
    /// <summary>Set optionality</summary>
    public static T SetOptional<T>(this T parameterDescription, bool? value) where T : IParameterDescription { parameterDescription.Optional = value; return parameterDescription; }

    /// <summary>Get parameter by name</summary>
    /// <param name="parameterDescriptions"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static IParameterDescription GetByName(this IEnumerable<IParameterDescription> parameterDescriptions, object name)
    {
        // No parameter descriptions
        if (parameterDescriptions == null) throw new ArgumentNullException(nameof(parameterDescriptions));
        // Iterate each
        foreach (IParameterDescription parameterDescription in parameterDescriptions)
        {
            //
            object? elementName = parameterDescription.Name;
            //
            if ((elementName == null) != (name == null)) continue;
            //
            if (elementName == null && name == null) return parameterDescription;
            // Match
            if (elementName!.Equals(name) || name!.Equals(elementName)) return parameterDescription;
        }
        // Not found
        throw new KeyNotFoundException(name.ToString());
    }
    /*
    /// <summary>Get parameter by name</summary>
    /// <param name="parameterDescriptions"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static IParameterDescription GetByName(this IEnumerable<IParameterDescription> parameterDescriptions, string name)
    {
        // No parameter descriptions
        if (parameterDescriptions == null) throw new ArgumentNullException(nameof(parameterDescriptions));
        // Iterate each
        foreach (IParameterDescription parameterDescription in parameterDescriptions)
        {
            // Print to string
            string? nameString = parameterDescription.Name?.ToString();
            // Match
            if (name == nameString) return parameterDescription;
        }
        // Not found
        throw new KeyNotFoundException(name.ToString());
    }
    */
    /// <summary>Evaluate whether there is default value.</summary>
    public static bool HasDefaultValue(this IParameterDescription parameterDescription)
    {
        // ParameterInfo
        if (parameterDescription.Writer is ParameterInfo pi) return pi.HasDefaultValue;
        // 
        return false;
    }

    /// <summary>Get default value.</summary>
    /// <returns>Default value. Is null if parameter is value-type is default default.</returns>
    public static object? GetDefaultValue(this IParameterDescription parameterDescription)
    {
        // ParameterInfo
        if (parameterDescription.Writer is ParameterInfo pi) return pi.DefaultValue;
        // 
        return null;
    }

}
