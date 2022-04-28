// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Reflection;
using System.Reflection.Emit;

/// <summary>Extension methods for <see cref="ILGenerator"/></summary>
public static class ILGeneratorExtensions
{
    /// <summary>Emit <paramref name="line"/> to <paramref name="il"/>.</summary>
    /// <exception cref="ArgumentException"></exception>
    public static ILGenerator Emit(this ILGenerator il, EmitLine line)
    {
        // Get opcode
        OpCode opcode = line.OpCode;
        // Argument count
        int count = line.Count;
        // Emit(OpCode opcode)
        if (count == 0) il.Emit(opcode);
        // Emit(OpCode opcode, string str)
        else if (count == 1 && line.Arg0 is string text) il.Emit(opcode, text);
        // Emit(OpCode opcode, byte arg)
        else if (count == 1 && line.Arg0 is byte value1) il.Emit(opcode, value1);
        // Emit(OpCode opcode, double arg)
        else if (count == 1 && line.Arg0 is double value2) il.Emit(opcode, value2);
        // Emit(OpCode opcode, short arg)
        else if (count == 1 && line.Arg0 is short value3) il.Emit(opcode, value3);
        // Emit(OpCode opcode, int arg)
        else if (count == 1 && line.Arg0 is int value4) il.Emit(opcode, value4);
        // Emit(OpCode opcode, long arg)
        else if (count == 1 && line.Arg0 is long value5) il.Emit(opcode, value5);
        // Emit(OpCode opcode, sbyte arg)
        else if (count == 1 && line.Arg0 is sbyte value13) il.Emit(opcode, value13);
        // Emit(OpCode opcode, float arg)
        else if (count == 1 && line.Arg0 is float value14) il.Emit(opcode, value14);
        // Emit(OpCode opcode, Type)
        else if (count == 1 && line.Arg0 is Type type) il.Emit(opcode, type);
        // Emit(OpCode opcode, ConstructorInfo con)
        else if (count == 1 && line.Arg0 is ConstructorInfo con) il.Emit(opcode, con);
        // Emit(OpCode opcode, Label label)
        else if (count == 1 && line.Arg0 is Label label) il.Emit(opcode, label);
        // Emit(OpCode opcode, Label[] labels)
        else if (count == 1 && line.Arg0 is Label[] labels) il.Emit(opcode, labels);
        // Emit(OpCode opcode, LocalBuilder local)
        else if (count == 1 && line.Arg0 is LocalBuilder local) il.Emit(opcode, local);
        // Emit(OpCode opcode, SignatureHelper signature)
        else if (count == 1 && line.Arg0 is SignatureHelper signature) il.Emit(opcode, signature);
        // Emit(OpCode opcode, FieldInfo field)
        else if (count == 1 && line.Arg0 is FieldInfo field) il.Emit(opcode, field);
        // Emit(OpCode opcode, MethodInfo meth)
        else if (count == 1 && line.Arg0 is MethodInfo meth) il.Emit(opcode, meth);
        // EmitCall(OpCode opcode, MethodInfo methodInfo, Type[]? optionalParameterTypes)
        else if (count == 2 && line.Arg0 is MethodInfo methodInfo) il.EmitCall(opcode, methodInfo, line.Arg1 as Type[]);
        // EmitCalli(OpCode opcode, CallingConventions callingConvention, Type? returnType, Type[]? parameterTypes, Type[]? optionalParameterTypes)
        else if (count == 4 && line.Arg0 is CallingConventions callingConvention) il.EmitCalli(opcode, callingConvention, line.Arg1 as Type, line.Arg2 as Type[], line.Arg3 as Type[]);
        // EmitCalli(OpCode opcode, CallingConvention unmanagedCallConv, Type? returnType, Type[]? parameterTypes)
        else if (count == 3 && line.Arg0 is System.Runtime.InteropServices.CallingConvention unmanagedCallConv) il.EmitCalli(opcode, unmanagedCallConv, line.Arg1 as Type, line.Arg2 as Type[]);
        // Error
        else throw new ArgumentException($"Cannot emit {opcode} with parameters \"{string.Join(',', line.Arguments!)}\"");
        //
        return il;
    }

    /// <summary>Emit <paramref name="lines"/> to <paramref name="il"/>.</summary>
    /// <exception cref="ArgumentException"></exception>
    public static ILGenerator Emit(this ILGenerator il, IEnumerable<EmitLine> lines)
    {
        //
        foreach (var line in lines)
            Emit(il, line);
        //
        return il;
    }

}
