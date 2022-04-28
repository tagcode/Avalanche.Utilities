// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Reflection;

/// <summary>Extension methods for <see cref="IConstructorDescription"/>.</summary>
public static class ConstructorDescriptionExtensions_
{
    /// <summary>Read constructor info from <paramref name="constructorInfo"/> and write to <paramref name="constructorDescription"/>.</summary>
    /// <remarks><see cref="IConstructorDescription.Type"/> is not assigned.</remarks>
    /// <param name="constructorInfo"><see cref="ConstructorInfo"/>, <![CDATA[EmitLine]]>, <see cref="MethodInfo"/>, <see cref="Delegate"/></param>
    /// <exception cref="ArgumentException">If <paramref name="constructorInfo"/> is not expected type.</exception>
    public static IConstructorDescription Read(this IConstructorDescription constructorDescription, object constructorInfo)
    {
        // Get annotations
        object[] annotations = (constructorInfo as MemberInfo)?.GetCustomAttributes(true) ?? Array.Empty<object>();
        // Assign annotations
        constructorDescription.Annotations = annotations;


        // Opcodes that initialize record, such as struct no-args constructor
        if (constructorInfo is EmitLine || constructorInfo is IEnumerable<EmitLine>)
        {
            // Assign constructor parameters
            constructorDescription.Parameters = Array.Empty<IParameterDescription>();
            // Assign constructor
            constructorDescription.Constructor = constructorInfo;
            // Return
            return constructorDescription;
        }

        // Args constructor
        if (constructorInfo is ConstructorInfo ci)
        {
            // Assign constructor parameters
            constructorDescription.Parameters = ci.GetParameters().Select(pi => new ParameterDescription().Read(pi).SetMember(constructorDescription)).ToArray();
            // Assign constructor
            constructorDescription.Constructor = ci;
            // Assign annotations
            constructorDescription.Annotations = ci.GetCustomAttributes(true);
            // Return
            return constructorDescription;
        }

        // Static create method
        if (constructorInfo is MethodInfo mi && mi.IsStatic)
        {
            // Assign constructor parameters
            constructorDescription.Parameters = mi.GetParameters().Select(pi => new ParameterDescription().Read(pi).SetMember(constructorDescription)).ToArray();
            // Assign constructor
            constructorDescription.Constructor = mi;
            // Assign annotations
            constructorDescription.Annotations = mi.GetCustomAttributes(true);
            // Return
            return constructorDescription;
        }

        // Create delegate
        if (constructorInfo is Delegate @delegate)
        {
            // Get method
            MethodInfo mi2 = @delegate.GetMethodInfo();
            // Assign constructor parameters
            constructorDescription.Parameters = mi2.GetParameters().Select(pi => new ParameterDescription().Read(pi).SetMember(constructorDescription)).ToArray();
            // Assign constructor
            constructorDescription.Constructor = @delegate;
            // Assign annotations
            constructorDescription.Annotations = mi2.GetCustomAttributes(true);
            // Return
            return constructorDescription;
        }

        // Not supported
        throw new InvalidOperationException($"{constructorInfo.GetType()} not supported.");
    }

    /// <summary>Clone <paramref name="src"/> in writable state.</summary>
    public static ConstructorDescription Clone(this IConstructorDescription src)
    {
        // Create result
        ConstructorDescription result = new ConstructorDescription();
        //
        result.Constructor = src.Constructor;
        result.Type = src.Type;
        result.SetParameters(src.Parameters);
        result.SetAnnotations(src.Annotations);
        result.Record = src.Record;
        // Return
        return result;
    }

    /// <summary>Calculate hash</summary>
    public static void HashIn(this IConstructorDescription constructorInfo, ref FNVHash64 hash, bool hashType = true, bool hashConstructor = true, bool hashParameters = true, bool hashAnnotations = true)
    {
        // Hash type
        if (hashType) hash.HashIn(constructorInfo.Type?.FullName);
        // Hash constructor
        if (hashConstructor && constructorInfo.Constructor != null) hash.HashIn(constructorInfo.Constructor.GetHashCode());
        // Hash parameters
        if (hashParameters && constructorInfo.Parameters != null)
        {
            foreach (var parameter in constructorInfo.Parameters)
            {
                parameter.HashIn(ref hash);
            }
        }
        // Hash annotations
        if (hashAnnotations && constructorInfo.Annotations != null)
        {
            foreach (var annotation in constructorInfo.Annotations)
            {
                hash.HashIn(annotation.GetHashCode());
            }
        }
    }

    /// <summary>Calculate hash64</summary>
    public static ulong CalcHash64(this IConstructorDescription constructorInfo)
    {
        // Init
        FNVHash64 hash = new FNVHash64();
        // Hashin
        constructorInfo.HashIn(ref hash);
        // Return
        return hash.Hash;
    }
}
