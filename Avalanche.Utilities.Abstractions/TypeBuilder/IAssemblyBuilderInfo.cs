// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Reflection;
using System.Reflection.Emit;

/// <summary>Assembly and module builder info</summary>
public interface IAssemblyBuilderInfo
{
    /// <summary></summary>
    AssemblyName AssemblyName { get; set; }
    /// <summary></summary>
    string ModuleName { get; set; }
    /// <summary></summary>
    AssemblyBuilder AssemblyBuilder { get; set; }
    /// <summary></summary>
    ModuleBuilder ModuleBuilder { get; set; }
}
