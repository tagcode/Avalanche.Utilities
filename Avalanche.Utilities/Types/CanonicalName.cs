// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Utilities for printing <see cref="Type"/> information.
/// </summary>
public static class CanonicalName
{
    /// <summary>
    /// Print full canonical name for <paramref name="type"/> with <paramref name="options"/>.
    /// 
    /// e.g. <see cref="List{T}"/> -> "System.Collections.Generics.List&lt;System.Int&gt;".
    /// </summary>
    /// <param name="type"></param>
    /// <param name="options"></param>
    /// <returns>Full canonical name</returns>
    public static string Print(Type type, CanonicalNameOptions options = CanonicalNameOptions.Default)
    {
        // No type
        if (type == null) return "";
        // Calculate length
        int len = GetLength(type, options);
        // Create builder
        StringBuilder sb = new StringBuilder(len);
        // Append type
        Append(type, sb, options);
        // Print
        return sb.ToString();
    }

    /// <summary>
    /// Append full canonical name for <paramref name="type"/> with <paramref name="options"/>.
    /// 
    /// e.g. <see cref="List{T}"/> -> "System.Collections.Generics.List&lt;System.Int&gt;".
    /// </summary>
    /// <param name="type"></param>
    /// <param name="sb"></param>
    /// <param name="options"></param>
    public static void Append(Type type, StringBuilder sb, CanonicalNameOptions options = CanonicalNameOptions.Default)
    {
        // "T"
        if (type.IsGenericParameter) { sb.Append(type.Name); return; }

        // Options for this specific type 
        CanonicalNameOptions localOptions = options;
        // Recurse to declaring type "MyClass+Mutable"
        if (type.DeclaringType != null)
        {
            // Recurse 
            Append(type.DeclaringType, sb, options & ~(CanonicalNameOptions.IncludeAssembly | CanonicalNameOptions.IncludeGenerics));
            // +
            sb.Append('+');
            // Change options
            localOptions = options & (CanonicalNameOptions.IncludeAssembly | CanonicalNameOptions.IncludeGenerics);
        }

        // Include Namespace "System.Collections.Generic"
        if (((localOptions & CanonicalNameOptions.IncludeNamespace) != 0) && !type.IsGenericParameter && type.Namespace != null) { sb.Append(type.Namespace); sb.Append("."); }

        // Include Name - without generics argument count `1 "SomeType`1"
        if (type.IsGenericType)
        {
            // Choose name
            string name = type.Name ?? type.FullName!;
            // Find `
            int ix = name.IndexOf('`');
            // Append to `
            if (ix > 0) sb.Append(name, 0, ix); else sb.Append(name);
        }
        else
            // Name part
            sb.Append(type.Name);

        // Assembly
        if ((localOptions & CanonicalNameOptions.IncludeAssembly) != 0)
        {
            // Include separator
            sb.Append('@');
            // Include Assembly name
            sb.Append(type.Assembly.GetName().Name);
        }

        // Generics<>
        if (((options & CanonicalNameOptions.IncludeGenerics) != 0) && type.IsGenericType)
        {
            // Open 
            sb.Append('<');
            // T, T2
            Type[] args = type.GetGenericArguments();
            for (int i = 0; i < args.Length; i++)
            {
                if (i > 0) sb.Append(",");
                Append(args[i], sb, options);
            }
            // Close
            sb.Append('>');
        }
    }

    /// <summary>
    /// Calculate length of canonical name.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    private static int GetLength(Type type, CanonicalNameOptions options = CanonicalNameOptions.Default)
    {
        // Place result here
        int result = 0;

        // "T"
        if (type.IsGenericParameter) { result += type.Name.Length; return result; }

        // Declaring Type "MyClass+Mutable"
        CanonicalNameOptions localOptions = options;
        if (type.DeclaringType != null)
        {
            result += GetLength(type.DeclaringType, options & ~(CanonicalNameOptions.IncludeAssembly | CanonicalNameOptions.IncludeGenerics));
            result++;
            localOptions = options & (CanonicalNameOptions.IncludeAssembly | CanonicalNameOptions.IncludeGenerics);
        }

        // Namespace: "System.Collections.Generic"
        if (((localOptions & CanonicalNameOptions.IncludeNamespace) != 0) && !type.IsGenericParameter && type.Namespace != null) result += type.Namespace.Length + 1;

        // Name
        // Remove generics argument count `1 "SomeType`1"
        if (type.IsGenericType)
            result += type.Name.IndexOf('`');
        else
            result += type.Name.Length;

        // Assembly Name "@assembly"
        if ((localOptions & CanonicalNameOptions.IncludeAssembly) != 0)
        {
            string? name = type.Assembly.GetName().Name;
            result += 1 + (name == null ? 0 : name.Length);
        }

        // Generics<>
        if (((options & CanonicalNameOptions.IncludeGenerics) != 0) && type.IsGenericType)
        {
            // <>
            result += 2;

            // T, T2
            Type[] args = type.GetGenericArguments();

            // ,
            if (args.Length > 1) result += args.Length - 1;
            // T
            foreach (Type arg in args)
                result += GetLength(arg);
        }

        return result;
    }
}

/// <summary>
/// Canonical name part.
/// </summary>
public class CanonicalNamePart
{
    /// <summary>Pattern that matches to canonical names</summary>
    public static Regex CanonicalPattern = new Regex(@"(?<name>[^@<]*)(@(?<assy>[^@<]*))?(<(?<arguments>.*)>)?", RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.ExplicitCapture);

    /// <summary>Name</summary>
    public string? Name;
    /// <summary>Assembly</summary>
    public string? AssemblyName;
    /// <summary>Arguments</summary>
    public List<CanonicalNamePart>? Arguments;

    /// <summary>Read part from <paramref name="type"/></summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static CanonicalNamePart FromType(Type type)
    {
        CanonicalNamePart result = new CanonicalNamePart();
        result.Name = CanonicalName.Print(type, CanonicalNameOptions.IncludeNamespace);
        result.AssemblyName = type.Assembly.GetName().Name ?? type.Assembly.GetName().FullName;
        if (type.IsGenericType)
        {
            Type[] args = type.GetGenericArguments();
            result.Arguments = new List<CanonicalNamePart>(args.Length);
            for (int i = 0; i < args.Length; i++)
                result.Arguments.Add(FromType(args[i]));
        }
        return result;
    }

    /// <summary>Parse <paramref name="name"/> into part</summary>
    /// <param name="name"></param>
    /// <returns>Part</returns>
    public static CanonicalNamePart Parse(String name)
    {
        int ix = 0;
        return Parse(name, ref ix, name.Length);
    }

    /// <summary>Parse <paramref name="name"/> into part</summary>
    /// <param name="name"></param>
    /// <param name="index"></param>
    /// <param name="length"></param>
    /// <returns>Part</returns>
    public static CanonicalNamePart Parse(string name, ref int index, int length)
    {
        Match m = CanonicalPattern.Match(name, index, length);
        if (!m.Success) throw new ArgumentException();
        CanonicalNamePart result = new CanonicalNamePart();
        result.Name = m.Groups["name"].Value;
        result.AssemblyName = m.Groups["assy"].Value;
        Group argumentGroup = m.Groups["arguments"];
        if (argumentGroup.Success)
        {
            result.Arguments = new List<CanonicalNamePart>(1);
            for (int ix = argumentGroup.Index; ix < argumentGroup.Index + argumentGroup.Length - 1;)
            {
                CanonicalNamePart arg = Parse(name, ref ix, argumentGroup.Length);
                result.Arguments.Add(arg);
                if (ix == argumentGroup.Index + argumentGroup.Length) break;
                if (ix >= name.Length) throw new ArgumentException($"Unexpected end of char stream at {ix}, for string \"{name}\".");
                char ch = name[ix];
                if (ch == ',') { /*Good*/}
                else throw new ArgumentException($"Unexpected character '{ch}' at index {ix}, when expecting ','.");
            }
        }
        index = m.Index + m.Length;
        return result;
    }

    /// <summary>
    /// Build part into <see cref="Type"/>.
    /// </summary>
    /// <returns>type</returns>
    public Type BuildType()
    {
        Assembly asm = Assembly.Load(AssemblyName!);
        string typename = Name!;

        // Get non-generics type
        if (Arguments == null || Arguments.Count == 0)
        {
            return asm.GetType(typename)!;
        }

        // Get generics type
        int count = Arguments.Count;
        Type genericsType = asm.GetType(typename + "`" + count)!;
        Type[] argTypes = Arguments.Select(arg => arg.BuildType()).ToArray();
        return genericsType.MakeGenericType(argTypes)!;
    }

    /// <summary>Visit <paramref name="visitor"/>.</summary>
    /// <param name="visitor"></param>
    public void Visit(Action<CanonicalNamePart> visitor)
    {
        // Visit this
        visitor(this);
        // Visit arguments
        if (Arguments != null) foreach (var arg in Arguments) arg.Visit(visitor);
    }

    /// <summary>Append name to <paramref name="sb"/></summary>
    /// <param name="sb"></param>
    public void AppendToString(StringBuilder sb)
    {
        if (Name != null) sb.Append(Name);
        if (AssemblyName != null) { sb.Append('@'); sb.Append(AssemblyName); }
        if (Arguments != null && Arguments.Count >= 0)
        {
            sb.Append('<');
            for (int i = 0; i < Arguments.Count; i++)
            {
                Arguments[i].AppendToString(sb);
                if (i > 0) sb.Append(',');
            }
            sb.Append('>');
        }
    }

    /// <summary>Print name</summary>
    /// <returns>Name</returns>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        AppendToString(sb);
        return sb.ToString();
    }
}

/// <summary>
/// Options for <see cref="CanonicalName"/>.
/// </summary>
[Flags]
public enum CanonicalNameOptions : UInt32
{
    /// <summary>Print no parts</summary>
    None = 0,
    /// <summary>Print all parts</summary>
    All = 0xffffffff,
    /// <summary>Include namespace</summary>
    IncludeNamespace = 1,
    /// <summary>Include generics information</summary>
    IncludeGenerics = 2,
    /// <summary>Include assembly</summary>
    IncludeAssembly = 4,
    /// <summary>Default settings</summary>
    Default = IncludeNamespace | IncludeGenerics,
}
