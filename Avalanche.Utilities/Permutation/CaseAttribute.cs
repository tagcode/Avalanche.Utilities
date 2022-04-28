// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

/// <summary></summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class CaseAttribute : Attribute
{
    /// <summary>Name of the propety this is case of</summary>
    public readonly string PropertyName;
    /// <summary>Name of the case</summary>
    public readonly string Name;

    /// <summary>Dependencies to other properties.</summary>
    public readonly string[]? PropertyDependencies;

    /// <summary>Create case attribute</summary>
    public CaseAttribute(string propertyName, string name, string[]? propertyDependencies = null)
    {
        PropertyName = propertyName;
        Name = name;
        PropertyDependencies = propertyDependencies;
    }
}
