// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Reflection;

/// <summary>Extension methods for <see cref="IConstructorDescription"/>.</summary>
public static class ConstructorDescriptionExtensions
{
    /// <summary>Set parameter type</summary>
    public static T SetType<T>(this T constructorDescription, Type value) where T : IConstructorDescription { constructorDescription.Type = value; return constructorDescription; }
    /// <summary>Set constructor constructor: <see cref="ConstructorInfo"/>, <see cref="MethodInfo"/>, <see cref="Delegate"/>, <![CDATA[IWriterBase]]> as <see cref="ValueType"/>, or <![CDATA[EmitLine]]> as indication of struct no-args constructor.</summary>
    public static T SetConstructor<T>(this T constructorDescription, Object value) where T : IConstructorDescription { constructorDescription.Constructor = value; return constructorDescription; }
    /// <summary>Read parameters from <paramref name="value"/> and make a copy.</summary>
    public static T SetParameters<T>(this T constructorDescription, IEnumerable<IParameterDescription> value) where T : IConstructorDescription { constructorDescription.Parameters = value.ToArray(); return constructorDescription; }
    /// <summary>Annotations, such as <see cref="Attribute"/></summary>
    public static T SetAnnotations<T>(this T constructorDescription, IEnumerable<object> value) where T : IConstructorDescription { constructorDescription.Annotations = value.ToArray(); return constructorDescription; }
    /// <summary>Set record which constructor is member of.</summary>
    public static T SetRecord<T>(this T constructorDescription, IRecordDescription? value) where T : IConstructorDescription { constructorDescription.Record = value; return constructorDescription; }

    /// <summary>Put into read-only state.</summary>
    /// <param name="applyParameters">Apply parameters into readonly state too</param>
    public static T SetReadOnlyDeep<T>(this T constructorDescription, bool applyParameters = false) where T : IConstructorDescription
    {
        // Assign as read-only
        if (constructorDescription is IReadOnly @readonly) @readonly.ReadOnly = true;
        //
        if (applyParameters)
        {
            // Get parameters
            var parameters = constructorDescription.Parameters;
            //
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    // Assign as read-only
                    if (parameter is IReadOnly _parameter) _parameter.ReadOnly = true;
                }
            }
        }
        //
        return constructorDescription;
    }

}
