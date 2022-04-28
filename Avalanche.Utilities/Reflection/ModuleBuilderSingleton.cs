// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Reflection.Emit;

/// <summary>Singleton for assembly that used for building run-time types.</summary>
public class ModuleBuilderSingleton
{
    /// <summary>Module name</summary>
    public const string DefaultName = "RefEmit_InMemoryManifestModule";
    /// <summary>Module builder</summary>
    static ModuleBuilder? _instance;
    /// <summary>Singleton for module that used for building run-time types.</summary>
    public static ModuleBuilder Instance => _instance ?? (_instance = AssemblyBuilderSingleton.Instance.GetDynamicModule(DefaultName) ?? AssemblyBuilderSingleton.Instance.DefineDynamicModule(DefaultName));
}
