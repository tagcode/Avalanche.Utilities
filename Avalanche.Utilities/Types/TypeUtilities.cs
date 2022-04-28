// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

/// <summary></summary>
public static class TypeUtilities
{
    /// <summary>Get the <paramref name="typeArgumentIndex"/>th argument type from <paramref name="definedType"/> that corresponds to <paramref name="undefinedType"/>.</summary>
    /// <param name="definedType">Type to evaluate, e.g <![CDATA[List<int>]]>, <![CDATA[int[]]]></param>
    /// <param name="undefinedType">Template class or interface, e.g. <![CDATA[List<>]]> or <![CDATA[IList<>]]></param>
    /// <param name="typeArgumentIndex">Type argument index</param>
    /// <returns></returns>
    public static bool TryGetTypeArgumentOfCorrespondingDefinedType(Type definedType, Type undefinedType, int typeArgumentIndex, [NotNullWhen(true)] out Type argumentType)
    {
        // Not generic type
        if (!undefinedType.IsGenericType) { argumentType = null!; return false; }
        // Index out of range
        if (typeArgumentIndex < 0) throw new IndexOutOfRangeException();
        // 
        Type[] undefinedTypeArgs = undefinedType.GetGenericArguments();
        // Compare to implemented interfaces
        if (undefinedType.IsInterface)
        {
            // Get interfaces
            Type[] intfTypes = definedType.GetInterfaces();
            // Self is generic interface
            if (definedType.IsGenericType && definedType.IsInterface)
            {
                // 
                Type[] definedTypeArgs = definedType.GetGenericArguments();
                // Get generic type
                Type typeGenericType = definedType.GetGenericTypeDefinition();
                // Compare equality to IList<>
                if (undefinedType.Equals(typeGenericType))
                {
                    //
                    if (definedTypeArgs == null || definedTypeArgs.Length < typeArgumentIndex) { argumentType = null!; return false; }
                    // Assign
                    argumentType = definedTypeArgs[typeArgumentIndex];
                    //
                    return true;
                }
                // Compare equality to IList<T>
                if (undefinedType.IsGenericType && undefinedType.ContainsGenericParameters && undefinedType.GetGenericTypeDefinition().Equals(typeGenericType) && AreAssignableOrDefinableTo(definedTypeArgs, undefinedTypeArgs))
                {
                    //
                    if (definedTypeArgs == null || definedTypeArgs.Length < typeArgumentIndex) { argumentType = null!; return false; }
                    // Assign
                    argumentType = definedTypeArgs[typeArgumentIndex];
                    //
                    return true;
                }
            }
            // Iterate
            for (int i = 0; i < intfTypes.Length; i++)
            {
                // Get type
                Type intfType = intfTypes[i];
                // 
                if (!intfType.IsGenericType) continue;
                // 
                Type[] definedTypeArgs = intfType.GetGenericArguments();
                // Get generic type
                Type intfGenericType = intfType.GetGenericTypeDefinition();
                // Compare equality
                if (undefinedType.Equals(intfGenericType))
                {
                    //
                    if (definedTypeArgs == null || definedTypeArgs.Length < typeArgumentIndex) { argumentType = null!; return false; }
                    // Assign
                    argumentType = definedTypeArgs[typeArgumentIndex];
                    // 
                    return true;
                }
                // Compare equality to IList<T>
                if (undefinedType.IsGenericType && undefinedType.ContainsGenericParameters && undefinedType.GetGenericTypeDefinition().Equals(intfGenericType) && AreAssignableOrDefinableTo(definedTypeArgs, undefinedTypeArgs))
                {
                    //
                    if (definedTypeArgs == null || definedTypeArgs.Length < typeArgumentIndex) { argumentType = null!; return false; }
                    // Assign
                    argumentType = definedTypeArgs[typeArgumentIndex];
                    // 
                    return true;
                }
            }
        }
        // Compare to classes and structs
        else if (undefinedType.IsClass || undefinedType.IsValueType)
        {
            // 
            for (Type? t = definedType; t != null; t = t.BaseType)
            {
                // 
                if (!t.IsGenericType) continue;
                // Get generic type
                Type genericType2 = t.GetGenericTypeDefinition();
                // 
                Type[] definedTypeArgs = t.GetGenericArguments();
                // Compare equality
                if (undefinedType.Equals(genericType2))
                {
                    //
                    if (definedTypeArgs == null || definedTypeArgs.Length < typeArgumentIndex) { argumentType = null!; return false; }
                    // Assign
                    argumentType = definedTypeArgs[typeArgumentIndex];
                    // 
                    return true;
                }
                // Compare equality to List<T> - Should type constraints be checked? How?
                if (undefinedType.IsGenericType && undefinedType.ContainsGenericParameters && undefinedType.GetGenericTypeDefinition().Equals(genericType2) && AreAssignableOrDefinableTo(definedTypeArgs, undefinedTypeArgs))
                {
                    //
                    if (definedTypeArgs == null || definedTypeArgs.Length < typeArgumentIndex) { argumentType = null!; return false; }
                    // Assign
                    argumentType = definedTypeArgs[typeArgumentIndex];
                    // 
                    return true;
                }
            }
        }
        // Not found
        argumentType = null!;
        return false;
    }

    /// <summary>Tests whether <paramref name="defined"/> type is definable to <paramref name="undefined"/> type.</summary>
    /// <param name="defined">Defined type e.g. <see cref="int"/> or generic type such as <![CDATA[List<int>]]></param>
    /// <param name="undefined">Undefined type or generic type, such as <see cref="IList{T}"/>.</param>
    public static bool IsDefinableTo(Type defined, Type undefined)
    {
        // value and T
        if (!defined.IsGenericType && !undefined.IsGenericType && undefined.IsGenericTypeParameter && PassesConstraints(defined, undefined)) return true;
        // Compare to interfaces
        if (undefined.IsInterface)
        {
            // Get interfaces
            Type[] intfTypes = defined.GetInterfaces();
            // Self is generic interface
            if (defined.IsGenericType && defined.IsInterface)
            {
                // Get generic type
                Type definedTypeDefinition = defined.GetGenericTypeDefinition();
                // Compare equality
                if (undefined.Equals(definedTypeDefinition)) return true;
                // Compare equality
                if (undefined.IsGenericType && undefined.ContainsGenericParameters && undefined.GetGenericTypeDefinition().Equals(definedTypeDefinition) && AreAssignableOrDefinableTo(defined.GetGenericArguments(), undefined.GetGenericArguments())) return true;
            }
            // Test all types
            for (int i = 0; i < intfTypes.Length; i++)
            {
                // Get type
                Type intfType = intfTypes[i];
                // 
                if (!intfType.IsGenericType) continue;
                // Get generic type
                Type intfGenericType = intfType.GetGenericTypeDefinition();
                // Compare equality
                if (undefined.Equals(intfGenericType)) return true;
                // Compare equality
                if (undefined.IsGenericType && undefined.ContainsGenericParameters && undefined.GetGenericTypeDefinition().Equals(intfGenericType) && AreAssignableOrDefinableTo(intfType.GetGenericArguments(), undefined.GetGenericArguments())) return true;
            }
        }
        // Compare to classes
        else if (undefined.IsClass || undefined.IsValueType)
        {
            // 
            for (Type? t = defined; t != null; t = t.BaseType)
            {
                // 
                if (!t.IsGenericType) continue;
                // Get generic type
                Type genericType2 = t.GetGenericTypeDefinition();
                // Compare equality
                if (undefined.Equals(genericType2)) return true;
                // Compare equality
                if (undefined.IsGenericType && undefined.ContainsGenericParameters && undefined.GetGenericTypeDefinition().Equals(genericType2) && AreAssignableOrDefinableTo(t.GetGenericArguments(), undefined.GetGenericArguments())) return true;
            }
        }
        // Not found
        return false;
    }

    /// <summary>Tests whether each of <paramref name="definedTypes"/> IsAssignableTo corresponding <paramref name="undefinedTypes"/>, or is definable to generic parameter <paramref name="undefinedTypes"/> 'T' with its constraints.</summary>
    /// <param name="definedTypes">Types where types are defined e.g. "int"</param>
    /// <param name="undefinedTypes">Generic parameter types, where typed defined or parameter types e.g. "T" where T : struct</param>
    static bool AreAssignableOrDefinableTo(Type[] definedTypes, Type[] undefinedTypes)
    {
        // Count mismatch
        if (definedTypes.Length != undefinedTypes.Length) return false;
        //
        for (int i = 0; i < definedTypes.Length; i++)
        {
            // 
            Type type = definedTypes[i], parameterType = undefinedTypes[i];
            //
            if (!PassesConstraints(type, parameterType)) return false;
        }
        // All assignable
        return true;
    }

    /// <summary>Get exactly one generic argument of <paramref name="type"/>.</summary>
    public static bool TryGetGenericArgument(Type type, int index, [NotNullWhen(true)] out Type arg0)
    {
        // Get generic arguments
        Type[] genericArguments = type.GetGenericArguments();
        // 
        if (genericArguments.Length < index) { arg0 = null!; return false; }
        //
        arg0 = genericArguments[index];
        //
        return true;
    }

    /// <summary>Get exactly two generic argument of <paramref name="type"/>.</summary>
    public static bool TryGetGenericArguments(Type type, [NotNullWhen(true)] out Type arg0, [NotNullWhen(true)] out Type arg1)
    {
        // Get generic arguments
        Type[] genericArguments = type.GetGenericArguments();
        // 
        if (genericArguments.Length != 2) { arg0 = null!; arg1 = null!; return false; }
        //
        arg0 = genericArguments[0];
        arg1 = genericArguments[1];
        //
        return true;
    }

    /// <summary>Try to convert undefined template method into defined one.</summary>
    /// <param name="undefinedMethodInfo"></param>
    /// <param name="paramMap"></param>
    /// <param name="definedMethodInfo"></param>
    /// <returns></returns>
    public static bool TryMakeGenericMethod(MethodInfo undefinedMethodInfo, Dictionary<Type, Type>? paramMap, out MethodInfo definedMethodInfo)
    {
        // Already defined 
        if (!undefinedMethodInfo.IsGenericMethod) { definedMethodInfo = undefinedMethodInfo; return true; }
        //
        Type[] undefinedTypeArgs = undefinedMethodInfo.GetGenericArguments();
        //
        Type[] definedTypeArgs = new Type[undefinedTypeArgs.Length];
        //
        for (int i = 0; i < undefinedTypeArgs.Length; i++)
        {
            // Get type arg
            Type undefinedArgType = undefinedTypeArgs[i];
            //
            if (!undefinedArgType.IsGenericParameter) { definedTypeArgs[i] = undefinedArgType; continue; }
            // Get correlating type
            if (paramMap == null || !paramMap.TryGetValue(undefinedTypeArgs[i], out Type? definedArgType)) { definedMethodInfo = null!; return false; }
            //
            definedTypeArgs[i] = definedArgType;
        }
        //
        definedMethodInfo = undefinedMethodInfo.MakeGenericMethod(definedTypeArgs);
        return true;
    }

    /// <summary>Matches recursively all generic parameters from <paramref name="paramMap"/>.</summary>
    /// <param name="undefinedType">Template type, e.g. <![CDATA[IServiceRequest<T>]]></param>
    /// <param name="paramMap">Map of generic parameters 'T' to actual types.</param>
    /// <param name="definedType">Type where template parameters are replaced with defined types</param>
    /// <returns>True if was matched</returns>
    public static bool TryMakeDefinedType(Type undefinedType, Dictionary<Type, Type>? paramMap, [NotNullWhen(true)] out Type definedType)
    {
        // Already defined
        if (!undefinedType.ContainsGenericParameters) { definedType = undefinedType; return true; }
        // Type itself is a generic parameter as is 'T'
        if (undefinedType.IsGenericParameter) { definedType = null!; return paramMap != null && paramMap.TryGetValue(undefinedType, out definedType!); }
        // Already defined 
        if (!undefinedType.IsGenericType || !undefinedType.ContainsGenericParameters) { definedType = undefinedType; return true; }
        //
        Type? undefinedTypeDefinition = undefinedType.GetGenericTypeDefinition();
        // No generic type
        if (undefinedTypeDefinition == null) { definedType = undefinedType; return true; }
        //
        Type[] undefinedTypeArgs = undefinedType.GetGenericArguments();
        // No generic parameters
        if (undefinedTypeArgs.Length == 0) { definedType = undefinedType; return true; }
        //
        Type[] definedTypeArgs = new Type[undefinedTypeArgs.Length];
        //
        for (int i = 0; i < undefinedTypeArgs.Length; i++)
        {
            // Get type arg
            Type undefinedTypeArg = undefinedTypeArgs[i];
            // Place type arg when defined here
            Type? definedTypeArg;
            // Assign as is
            if (!undefinedTypeArg.IsGenericParameter) definedTypeArg = undefinedTypeArg;
            //
            else
            {
                // Get implementing match for T
                if (paramMap == null || !paramMap.TryGetValue(undefinedTypeArg, out definedTypeArg)) { definedType = null!; return false; }
                // Check constraints
                if (!PassesConstraints(definedTypeArg, undefinedTypeArg, paramMap)) { definedType = null!; return false; }
            }
            //
            if (!TryMakeDefinedType(definedTypeArg, paramMap, out definedTypeArg)) { definedType = null!; return false; }
            // Assign 
            definedTypeArgs[i] = definedTypeArg;
        }
        // Sometimes template type is broken, and this fixes it.
        if (undefinedType.FullName == null) undefinedType = undefinedType.GetGenericTypeDefinition();
        // Define type
        definedType = undefinedTypeDefinition.MakeGenericType(definedTypeArgs);
        // Done
        return true;
    }

    /// <summary></summary>
    public class TypeEqualsComparer : IEqualityComparer<Type>
    {
        /// <summary></summary>
        static TypeEqualsComparer instance = new TypeEqualsComparer();
        /// <summary></summary>
        public static TypeEqualsComparer Instance => instance;

        /// <summary></summary>
        public bool Equals(Type? x, Type? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.Equals(y);
        }
        /// <summary></summary>
        public int GetHashCode([DisallowNull] Type obj) => obj.GetHashCode();
    }

    /// <summary>Compares with <see cref="Type.IsAssignableTo(Type?)"/>.</summary>
    /// <remarks>Implementation violates comparer contract by not being symmetrical, also hash doesnt reflect equality.</remarks>
    public class TypeAssignableComparer : IEqualityComparer<Type>
    {
        /// <summary></summary>
        static IEqualityComparer<Type> instance = new TypeAssignableComparer();
        /// <summary></summary>
        public static IEqualityComparer<Type> Instance => instance;

        /// <summary></summary>
        public bool Equals(Type? x, Type? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.IsAssignableTo(y);
        }
        /// <summary></summary>
        public int GetHashCode([DisallowNull] Type obj) => obj.GetHashCode();
    }

    /// <summary>Compares with with castability.</summary>
    /// <remarks>Implementation violates comparer contract by not being symmetrical, also hash doesnt reflect equality.</remarks>
    public class TypeCastableComparer : IEqualityComparer<Type>
    {
        /// <summary></summary>
        static IEqualityComparer<Type> instance = new TypeAssignableComparer();
        /// <summary></summary>
        public static IEqualityComparer<Type> Instance => instance;

        /// <summary></summary>
        public bool Equals(Type? x, Type? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            if (x.IsAssignableTo(y)) return true;
            if (x.IsValueType && y.Equals(typeof(object))) return true;
            if (x.Equals(typeof(object)) && y.IsValueType) return true;
            return false;
        }
        /// <summary></summary>
        public int GetHashCode([DisallowNull] Type obj) => obj.GetHashCode();
    }

    /// <summary>Tests if <paramref name="typeValue"/> is assignable as defined type to type parameter<paramref name="typeArg"/>.</summary>
    /// <param name="typeValue">Typ</param>
    /// <param name="typeArg">generic type parameter</param>
    /// <param name="paramMap">Param map</param>
    /// <returns>True if <paramref name="typeValue"/> passes all constraints in <paramref name="typeArg"/>.</returns>
    public static bool PassesConstraints(Type typeValue, Type typeArg, Dictionary<Type, Type>? paramMap = null)
    {
        // Not generic parameter
        if (!typeArg.IsGenericParameter) return typeValue.IsAssignableTo(typeArg);
        // Get attributes
        GenericParameterAttributes attr = typeArg.GenericParameterAttributes;
        // where T : basetype/interface
        foreach (Type constraint in typeArg.GetGenericParameterConstraints() ?? Type.EmptyTypes)
        {
            // If constraint depends on another type-argument, make the constraint more defined
            if (!TryMakeDefinedType(constraint, paramMap, out Type adaptedConstraint)) return false;
            //
            if (!typeValue.IsAssignableTo(adaptedConstraint)) return false;
        }
        // Where T : new()
        if (attr.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
        {
            // Structs are always new()
            if (typeValue.IsClass)
            {
                // Get constructor.
                ConstructorInfo? ci = typeValue.GetConstructor(Type.EmptyTypes);
                // Init
                if (ci == null || !ci.IsPublic) return false;
            }
        }
        // Where T : struct
        if (attr.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint))
        {
            if (!typeValue.IsValueType) return false;
        }
        // Where T : class
        if (attr.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint))
        {
            if (!typeValue.IsClass) return false;
        }
        /*
        // Covariant 'out' 
        if (attr.HasFlag(GenericParameterAttributes.Covariant)) {  }
        // Contravariant 'in'
        else if (attr.HasFlag(GenericParameterAttributes.Contravariant)) {  }
        // Exact match
        else { }
        */

        // Seems legit
        return true;
    }

    /// <summary>
    /// Tries to match applicable generic parameters by filling <paramref name="paramMap"/>.
    /// 
    /// Examines how generic type parameters should be assigned on <paramref name="undefinedType"/> so that it matches <paramref name="definedType"/>.
    /// 
    /// Assigns generic parameters recursively, but does not check type parameter constraints or assignability. 
    /// See <see cref="TryGetApplicableGenericParameters"/> that checks assignability and constraints.
    /// </summary>
    /// <param name="definedType"></param>
    /// <param name="undefinedType"></param>
    /// <param name="paramMap">Map of undefined type parameters (e.g. 'T') to defined type assignments.</param>
    /// <returns>true if <paramref name="undefinedType"/> type arguments structurally matched to <paramref name="definedType"/>.</returns>
    internal static bool TryMatchCorrespondingGenericParameters(Type definedType, Type undefinedType, ref Dictionary<Type, Type>? paramMap)
    {
        // As is
        if (undefinedType.Equals(definedType)) return true;
        // As 'T'
        if (undefinedType.IsGenericParameter)
        {
            // Create type-arg to defined-type map
            if (paramMap == null) paramMap = new();
            // Assign type-arg to defined-type map
            paramMap[undefinedType] = definedType;
            // 
            return true;
        }
        // 
        if (!undefinedType.IsGenericType || !definedType.IsGenericType || !undefinedType.ContainsGenericParameters) return undefinedType.IsAssignableTo(definedType);
        //
        Type? definedTypesTypeDefinition = definedType.GetGenericTypeDefinition();
        //
        if (definedTypesTypeDefinition == null/* || !undefinedType.IsAssignableTo(definedTypesTypeDefinition)*/) return false;
        //
        Type[] undefinedTypeArgs = undefinedType.GetGenericArguments(), definedTypeArgs = definedType.GetGenericArguments();
        //
        if (undefinedTypeArgs.Length != definedTypeArgs.Length) return false;
        //
        int c = undefinedTypeArgs.Length;
        //
        for (int i = 0; i < c; i++)
        {
            // Get type arg
            Type undefinedTypeArg = undefinedTypeArgs[i], definedTypeArg = definedTypeArgs[i];
            // Get implementing match for T
            if (!TryMatchCorrespondingGenericParameters(definedTypeArg, undefinedTypeArg, ref paramMap)) return false;
        }
        //
        return true;
    }

    /// <summary>
    /// Tries to infer applicable generic parameters. 
    /// 
    /// Examines how generic type parameters should be assigned on <paramref name="undefinedType"/> so that it matches <paramref name="definedType"/>.
    /// </summary>
    /// <param name="definedType"></param>
    /// <param name="undefinedType"></param>
    /// <param name="paramMap">
    ///     Map of undefined type parameters (e.g. 'T') to defined type assignments.
    ///     
    ///     Note that, if return value is 'false' then previously existing param map is trashed with wrong value assignments.
    ///     Caller intends to continue to use parammap, caller must take backup before call and revert to if..
    /// </param>
    /// <returns>true if <paramref name="undefinedType"/> can be matched to <paramref name="definedType"/>.</returns>
    /// <remarks>This implementation is not perfect but can cope in simple cases.</remarks>
    public static bool TryGetApplicableGenericParameters(Type definedType, Type undefinedType, ref Dictionary<Type, Type>? paramMap, bool testAssignability = true)
    {
        // As is
        if (undefinedType.Equals(definedType)) return true;
        // Match 'T' to values structurally
        if (!TryMatchCorrespondingGenericParameters(definedType, undefinedType, ref paramMap)) return false;
        // Check constraints
        if (paramMap != null)
        {
            foreach (var paramAssignment in paramMap)
            {
                if (!PassesConstraints(paramAssignment.Value, paramAssignment.Key, paramMap)) return false;
            }
        }

        // Workaround
        if (testAssignability)
        {
            // Create defined type
            if (!TryMakeDefinedType(undefinedType, paramMap, out Type _definedType)) return false;
            // Assert assignable
            if (!_definedType.IsAssignableTo(definedType)) return false;
        }
        //
        return true;
    }

    /// <summary>Tests if <paramref name="type"/> is public, including argument types.</summary>
    public static bool IsPublicDeep(Type type)
    {
        // Handle nested type
        if (type.IsNested)
        {
            // Not public
            if (!type.IsPublic && !type.IsNestedPublic) return false;
            // Handle parent type
            if (type.DeclaringType != null && !IsPublicDeep(type.DeclaringType)) return false;
        }
        // Non-nested type.
        else
        {
            // Not public
            if (!type.IsPublic && !type.IsNestedPublic) return false;
        }
        //
        Type[]? args = type.GetGenericArguments();
        //
        if (args != null && args.Length > 0)
        {
            // Test each
            foreach (Type arg in args)
                if (!IsPublicDeep(arg))
                    return false;
        }
        //
        return true;
    }
}
