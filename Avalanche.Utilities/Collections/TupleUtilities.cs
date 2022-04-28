// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

/// <summary>
/// Utilities for handling tuples.
/// 
/// When tuples have more than 7-arguments, tuples are nested in "TRest" type argument, see <see cref="ValueTuple{T1, T2, T3, T4, T5, T6, T7, TRest}"/>.
/// Utilities are required for generalized way to process broad tuples.
/// </summary>
public static class TupleUtilities
{
    /// <summary>Create value type</summary>
    /// <param name="fieldTypes"></param>
    /// <returns></returns>
    public static Type CreateValueTupleType(params Type[] fieldTypes)
    {
        // 
        int count = fieldTypes.Length;
        // No args
        if (count == 0) return typeof(ValueTuple);
        // Place result here
        Type result = null!;
        //
        int ix = fieldTypes.Length;
        // 
        while (count > 0)
        {
            //
            int c = count % 7;
            //
            if (c == 0) c = 7;
            // 
            Type[] types = new Type[c + (result == null ? 0 : 1)];
            //
            for (int i = 0; i < c; i++) types[i] = fieldTypes[ix - c + i];
            //
            if (result != null) types[7] = result;
            //
            result = types.Length switch
            {
                1 => typeof(ValueTuple<>).MakeGenericType(types),
                2 => typeof(ValueTuple<,>).MakeGenericType(types),
                3 => typeof(ValueTuple<,,>).MakeGenericType(types),
                4 => typeof(ValueTuple<,,,>).MakeGenericType(types),
                5 => typeof(ValueTuple<,,,,>).MakeGenericType(types),
                6 => typeof(ValueTuple<,,,,,>).MakeGenericType(types),
                7 => typeof(ValueTuple<,,,,,,>).MakeGenericType(types),
                _ => typeof(ValueTuple<,,,,,,,>).MakeGenericType(types)
            };
            //
            count -= c;
            ix -= c;
        }
        // Return
        return result;
    }

    /// <summary>Get <see cref="ValueTuple"/> parameter types.</summary>
    /// <param name="valueTupleType"></param>
    /// <returns></returns>
    public static Type[] GetParameterTypes(Type valueTupleType)
    {
        // No args
        if (valueTupleType.Equals(typeof(ValueTuple))) return Type.EmptyTypes;
        // Place count here
        int count = 0;
        // Count
        for (Type? t = valueTupleType; t != null;)
        {
            // Not generic type
            if (!t.IsGenericType) throw new ArgumentException(nameof(valueTupleType));
            //
            Type[]? typeArgs = t.GenericTypeArguments;
            //
            if (typeArgs == null) throw new ArgumentException(nameof(valueTupleType));
            //
            if (typeArgs.Length < 8) { count += typeArgs.Length; break; }
            // 
            count += 7;
            t = typeArgs[7];
        }
        // Allocate
        Type[] result = new Type[count];
        // Index
        int ix = 0;
        // Assign
        for (Type? t = valueTupleType; t != null;)
        {
            // Not generic type
            if (!t.IsGenericType) throw new ArgumentException(nameof(valueTupleType));
            //
            Type[]? typeArgs = t.GenericTypeArguments;
            //
            if (typeArgs == null) throw new ArgumentException(nameof(valueTupleType));
            // 
            int c = typeArgs.Length < 8 ? typeArgs.Length : 7;
            // Assign each
            for (int i = 0; i < c; i++) result[ix++] = typeArgs[i];
            //
            if (typeArgs.Length < 8) break;
            // 
            t = typeArgs[7];
        }

        // Return
        return result;
    }

    /// <summary>Visits <see cref="ValueTuple{T1, T2, T3, T4, T5, T6, T7, TRest}"/> types from root towards tail (Non 8-argument type).</summary>
    public static Type[] GetTupleTypes(Type valueTupleType)
    {
        //
        StructList4<Type> types = new StructList4<Type>();
        //
        for (Type? t = valueTupleType;
             t != null;
             TypeUtilities.TryGetTypeArgumentOfCorrespondingDefinedType(t, typeof(ValueTuple<,,,,,,,>), 7, out t))
            types.Add(t);
        //
        return types.ToArray();
    }

    /// <summary>Emit opcodes that construct value tuple from values in stack. Leaves the tuple in stack.</summary>
    /// <remarks>Arguments in stack must match types from <see cref="GetParameterTypes(Type)"/></remarks>
    /// <param name="valueTupleType"></param>
    /// <returns></returns>
    public static EmitLine[] EmitConstructTuple(Type valueTupleType)
    {
        //
        StructList8<EmitLine> lines = new();
        //
        StructList4<Type> tupleTypes = new StructList4<Type>();
        //
        for (Type? t = valueTupleType;
             t != null;
             TypeUtilities.TryGetTypeArgumentOfCorrespondingDefinedType(t, typeof(ValueTuple<,,,,,,,>), 7, out t))
            tupleTypes.Add(t);
        // Visit from tail to root
        for (int i = tupleTypes.Count - 1; i >= 0; i--)
        {
            //
            Type tupleType = tupleTypes[i];
            // Get .ctor
            ConstructorInfo ci = tupleType.GetConstructor(tupleType.GetGenericArguments())!;
            //
            lines.Add(new EmitLine(OpCodes.Newobj, ci));
        }
        //
        return lines.ToArray();
    }


    /// <summary>Emit opcodes that init value tuple from values in stack. Leaves the tuple in stack.</summary>
    /// <remarks>Arguments in stack must match types from <see cref="GetParameterTypes(Type)"/></remarks>
    /// <param name="valueTupleType"></param>
    /// <returns></returns>
    public static EmitLine[] EmitInitTuple(Type valueTupleType)
    {
        //
        EmitLine line = new EmitLine(OpCodes.Initobj, valueTupleType);
        //
        return new EmitLine[] { line };
        /*
        //
        StructList8<EmitLine> lines = new();
        //
        StructList4<Type> tupleTypes = new StructList4<Type>();
        //
        for (Type? t = valueTupleType;
             t != null;
             TypeUtils.TryGetTypeArgumentOfCorrespondingDefinedType(t, typeof(ValueTuple<,,,,,,,>), 7, out t))
            tupleTypes.Add(t);
        // Visit from tail to root
        for (int i = tupleTypes.Count - 1; i >= 0; i--)
        {
            //
            Type tupleType = tupleTypes[i];
            // Get .ctor
            ConstructorInfo ci = tupleType.GetConstructor(tupleType.GetGenericArguments())!;
            //
            lines.Add(new EmitLine(OpCodes.Newobj, ci));
        }
        //
        return lines.ToArray();
        */
    }


    /// <summary>Emit opcodes that convert tuple pointer in stack to tuple argument.</summary>
    /// <param name="valueTupleType"></param>
    /// <remarks>Use opcodes such as <see cref="OpCodes.Ldloca"/>, <see cref="OpCodes.Ldarga"/> to load tuple address first before call.</remarks>
    public static EmitLine[] EmitTupleArgumentPointer(Type valueTupleType, int fieldIndex)
    {
        //
        StructList8<EmitLine> lines = new();
        // Visit tuple types from root to tail (the non TRest type)
        for (Type? t = valueTupleType;
             t != null;
             TypeUtilities.TryGetTypeArgumentOfCorrespondingDefinedType(t, typeof(ValueTuple<,,,,,,,>), 7, out t))
        {
            // Get fields
            FieldInfo[] fields = t.GetFields();
            //
            int _fieldIndex = fieldIndex < 7 ? fieldIndex : 7;
            // Get field
            FieldInfo fi = fields[_fieldIndex];
            // Load field address
            lines.Add(new EmitLine(OpCodes.Ldflda, fi));
            // 
            fieldIndex -= _fieldIndex;
            //
            if (fieldIndex <= 0) break;
        }
        //
        return lines.ToArray();
    }

    /// <summary>Drill down into tuple types until <paramref name="fieldIndex"/> is found. </summary>
    /// <param name="valueTupleType">Type that implements <see cref="ITuple"/></param>
    /// <param name="fieldIndex"></param>
    /// <returns>Opcodes that drill down in tuple types and field info.</returns>
    public static (EmitLine[] ops, FieldInfo fieldInfo) DrillIntoField(Type valueTupleType, int fieldIndex)
    {
        //
        StructList3<EmitLine> ops = new StructList3<EmitLine>();
        // Visit tuple types from root to tail (the non TRest type)
        for (Type? t = valueTupleType;
             t != null;
             TypeUtilities.TryGetTypeArgumentOfCorrespondingDefinedType(t, typeof(ValueTuple<,,,,,,,>), 7, out t))
        {
            // Get fields
            FieldInfo[] fields = t.GetFields();
            //
            if (fieldIndex < 7) return (ops.ToArray(), fields[fieldIndex]);
            //
            int _fieldIndex = fieldIndex < 7 ? fieldIndex : 7;
            // Get field
            FieldInfo fi = fields[_fieldIndex];
            // Load field address
            ops.Add(new EmitLine(OpCodes.Ldflda, fi));
            // 
            fieldIndex -= _fieldIndex;
            //
            if (fieldIndex <= 0) break;
        }
        //
        throw new InvalidOperationException();
    }

}
