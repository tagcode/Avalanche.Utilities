// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Text;

/// <summary>One permutation run</summary>
public class Run : IDisposable
{
    /// <summary></summary>
    public readonly Scenario Scenario;
    /// <summary></summary>
    public readonly Dictionary<string, object?> Parameters;

    /// <summary></summary>
    public object? this[string propertyName] { get => Parameters[propertyName]; set { Parameters[propertyName] = value; } }
    /// <summary></summary>
    public object? this[Type property] { get => Parameters[property.Name]; set { Parameters[property.Name] = value; } }

    /// <summary></summary>
    public Run(Scenario testScenario, Dictionary<string, object?> testParameters)
    {
        Scenario = testScenario;
        Parameters = testParameters;
    }

    /// <summary></summary>
    public Run Initialize()
    {
        // Initialize run
        foreach (Case @case in Scenario.Cases)
            Parameters[@case.PropertyName] = @case?.Initialize(this);
        return this;
    }

    /// <summary></summary>
    public void Dispose()
    {
        // Take off from ~finalizer queue.
        GC.SuppressFinalize(this);
        // Cleanup
        foreach (Case @case in Scenario.Cases)
            @case.Cleanup(this);
    }

    /// <summary></summary>
    public T Get<T>(string propertyName) => (T)Parameters[propertyName]!;
    /// <summary></summary>
    public T Get<T, PropertyName>() => (T)Parameters[typeof(PropertyName).Name]!;
    /// <summary></summary>
    public PropertyName Get<PropertyName>() => (PropertyName)Parameters[typeof(PropertyName).Name]!;
    /// <summary></summary>
    public T Set<T>(string propertyName, T value) { Parameters[propertyName] = value; return value; }
    /// <summary></summary>
    public T Set<T, PropertyName>(T value) { Parameters[typeof(PropertyName).Name] = value; return value; }
    /// <summary></summary>
    public T Set<T>(T value) { Parameters[typeof(T).Name] = value; return value; }

    /// <summary>Print information</summary>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(nameof(Run));
        sb.Append("(");
        int c = 0;
        foreach (var kv in Parameters)
        {
            if (c > 0) sb.Append(", ");
            sb.Append(kv.Key);
            sb.Append("=");
            sb.Append(kv.Value);
            c++;
        }
        sb.Append(")");
        return sb.ToString();
    }

}
