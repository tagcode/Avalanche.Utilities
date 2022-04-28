// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Reflection;
using System.Reflection.Emit;

/// <summary>Singleton for assembly that used for building run-time types.</summary>
public class AssemblyBuilderSingleton
{
    /// <summary>Assembly name</summary>
    public const string DefaultName = "Avalanche.Runtime";
    /// <summary>Assembly name</summary>
    static AssemblyName defaultAssemblyName = new AssemblyName(DefaultName);
    /// <summary>Assembly name</summary>
    public static AssemblyName DefaultAssemblyName => defaultAssemblyName;
    /// <summary>Assembly builder</summary>
    static Lazy<AssemblyBuilder> instance = new Lazy<AssemblyBuilder>(() => System.Reflection.Emit.AssemblyBuilder.DefineDynamicAssembly(DefaultAssemblyName, AssemblyBuilderAccess.RunAndCollect));
    /// <summary>Singleton for assembly that used for building run-time types.</summary>
    public static AssemblyBuilder Instance => instance.Value;
}
