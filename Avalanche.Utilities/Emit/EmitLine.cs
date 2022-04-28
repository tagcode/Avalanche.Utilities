// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Reflection.Emit;

/// <summary>A line to emit to <see cref="ILGenerator"/> as <see cref="System.Reflection.Emit.OpCode"/> and parameters.</summary>
public struct EmitLine
{
    /// <summary>Create with <![CDATA[object[]]]></summary>
    public static EmitLine Create(OpCode code, params object[]? arguments)
    {
        // No args
        if (arguments == null || arguments.Length == 0) return new EmitLine(code);
        // 
        if (arguments.Length == 1) return new EmitLine(code, arguments[0]);
        // 
        if (arguments.Length == 2) return new EmitLine(code, arguments[0], arguments[1]);
        // 
        if (arguments.Length == 3) return new EmitLine(code, arguments[0], arguments[1], arguments[2]);
        // 
        if (arguments.Length == 4) return new EmitLine(code, arguments[0], arguments[1], arguments[2], arguments[3]);
        // Cut array
        object[] argRest = new object[arguments.Length - 4];
        //
        Array.Copy(arguments, 4, argRest, 0, arguments.Length - 4);
        // 
        return new EmitLine(code, arguments[0], arguments[1], arguments[2], arguments[3], argRest);
    }

    /// <summary>Opcode</summary>
    public readonly OpCode OpCode;
    /// <summary>Argument 0</summary>
    public readonly object? Arg0;
    /// <summary>Argument 1</summary>
    public readonly object? Arg1;
    /// <summary>Argument 2</summary>
    public readonly object? Arg2;
    /// <summary>Argument 3</summary>
    public readonly object? Arg3;
    /// <summary>Arguments 4-></summary>
    public readonly object[]? ArgRest;

    /// <summary></summary>
    public static explicit operator EmitLine(OpCode opcode) => new EmitLine(opcode);
    /// <summary></summary>
    public static explicit operator EmitLine((OpCode opcode, object[]? parameters) tuple) => EmitLine.Create(tuple.opcode, arguments: tuple.parameters);
    /// <summary></summary>
    public static explicit operator (OpCode, object[]?)(EmitLine line) => (line.OpCode, line.Arguments);

    /// <summary>Argument count</summary>
    public int Count
    {
        get
        {
            if (ArgRest != null) return ArgRest.Length + 4;
            if (Arg3 != null) return 4;
            if (Arg2 != null) return 3;
            if (Arg1 != null) return 2;
            if (Arg0 != null) return 1;
            return 0;

        }
    }

    /// <summary>Get arguments as array.</summary>
    public object[]? Arguments
    {
        get
        {
            // Put arguments into object[]
            if (ArgRest != null)
            {
                object[] parameters = new object[ArgRest.Length + 4];
                Array.Copy(ArgRest, 0, parameters, 4, ArgRest.Length);
                parameters[0] = Arg0!;
                parameters[1] = Arg1!;
                parameters[2] = Arg2!;
                parameters[3] = Arg3!;
                return parameters;
            }
            if (Arg3 != null) return new object[] { Arg0!, Arg1!, Arg2!, Arg3 };
            if (Arg2 != null) return new object[] { Arg0!, Arg1!, Arg2 };
            if (Arg1 != null) return new object[] { Arg0!, Arg1 };
            if (Arg0 != null) return new object[] { Arg0 };
            return null;
        }
    }

    /// <summary></summary>
    public EmitLine(OpCode opcode)
    {
        this.OpCode = opcode;
        this.Arg0 = null;
        this.Arg1 = null;
        this.Arg2 = null;
        this.Arg3 = null;
        this.ArgRest = null;
    }

    /// <summary></summary>
    public EmitLine(OpCode opcode, object arg0)
    {
        this.OpCode = opcode;
        this.Arg0 = arg0;
        this.Arg1 = null;
        this.Arg2 = null;
        this.Arg3 = null;
        this.ArgRest = null;
    }

    /// <summary></summary>
    public EmitLine(OpCode opcode, object arg0, object arg1)
    {
        this.OpCode = opcode;
        this.Arg0 = arg0;
        this.Arg1 = arg1;
        this.Arg2 = null;
        this.Arg3 = null;
        this.ArgRest = null;
    }

    /// <summary></summary>
    public EmitLine(OpCode opcode, object arg0, object arg1, object arg2)
    {
        this.OpCode = opcode;
        this.Arg0 = arg0;
        this.Arg1 = arg1;
        this.Arg2 = arg2;
        this.Arg3 = null;
        this.ArgRest = null;
    }

    /// <summary></summary>
    public EmitLine(OpCode opcode, object arg0, object arg1, object arg2, object arg3)
    {
        this.OpCode = opcode;
        this.Arg0 = arg0;
        this.Arg1 = arg1;
        this.Arg2 = arg2;
        this.Arg3 = arg3;
        this.ArgRest = null;
    }

    /// <summary></summary>
    public EmitLine(OpCode opcode, object arg0, object arg1, object arg2, object arg3, object[] argRest)
    {
        this.OpCode = opcode;
        this.Arg0 = arg0;
        this.Arg1 = arg1;
        this.Arg2 = arg2;
        this.Arg3 = arg3;
        this.ArgRest = argRest;
    }
}
