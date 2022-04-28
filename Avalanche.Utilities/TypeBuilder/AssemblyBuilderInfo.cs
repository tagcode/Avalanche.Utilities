// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Reflection;
using System.Reflection.Emit;

/// <summary></summary>
public record AssemblyBuilderInfo : ReadOnlyAssignableRecord, IAssemblyBuilderInfo
{
    /// <summary></summary>
    protected AssemblyName assemblyName = null!;
    /// <summary></summary>
    protected string moduleName = null!;
    /// <summary></summary>
    protected AssemblyBuilder assemblyBuilder = null!;
    /// <summary></summary>
    protected ModuleBuilder moduleBuilder = null!;

    /// <summary></summary>
    public AssemblyName AssemblyName { get => assemblyName; set => this.AssertWritable().assemblyName = value; }
    /// <summary></summary>
    public string ModuleName { get => moduleName; set => this.AssertWritable().moduleName = value; }
    /// <summary></summary>
    public AssemblyBuilder AssemblyBuilder { get => assemblyBuilder; set => this.AssertWritable().assemblyBuilder = value; }
    /// <summary></summary>
    public ModuleBuilder ModuleBuilder { get => moduleBuilder; set => this.AssertWritable().moduleBuilder = value; }

    /// <summary></summary>
    public static AssemblyBuilderInfo Create(string name)
    {
        //
        AssemblyBuilderInfo result = new AssemblyBuilderInfo();
        //
        result.assemblyName = new AssemblyName(name);
        //
        result.moduleName = "RefEmit_InMemoryManifestModule";
        // Create new
        result.assemblyBuilder = System.Reflection.Emit.AssemblyBuilder.DefineDynamicAssembly(result.assemblyName, AssemblyBuilderAccess.RunAndCollect);
        // 
        result.moduleBuilder = result.assemblyBuilder.GetDynamicModule(result.moduleName) ?? result.assemblyBuilder.DefineDynamicModule(result.moduleName);
        //
        return result;
    }
}
