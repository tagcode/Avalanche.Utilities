// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

/// <summary>Property that is being varied.</summary>
public class Property
{
    /// <summary>Name of the property</summary>
    public readonly string Name;

    /// <summary>List of attached cases</summary>
    internal readonly List<Case> cases = new List<Case>();

    /// <summary>List of dependencies from attached test cases.</summary>
    internal readonly HashSet<string> dependencies = new HashSet<string>();

    /// <summary>Create property for permutation</summary>
    public Property(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    /// <summary>Add property case.</summary>
    public void AddCase(Case @case)
    {
        cases.Add(@case);
        foreach (string dependency in @case.PropertyDependencies)
            dependencies.Add(dependency);
    }

    /// <summary>Print information</summary>
    public override string ToString() => Name;
}
