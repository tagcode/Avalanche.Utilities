// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Text;

/// <summary>Scenario is one combination of cases.</summary>
public class Scenario
{
    /// <summary></summary>
    public readonly Case[] Cases;
    /// <summary></summary>
    public readonly IEnumerable<KeyValuePair<string, object>>? initialParameters;

    /// <summary></summary>
    public Case? this[string propertyName] { get => Cases.Where(_case => _case.PropertyName == propertyName).FirstOrDefault(); }

    /// <summary></summary>
    public Scenario(Case[] cases, IEnumerable<KeyValuePair<string, object>>? initialParameters = null)
    {
        Cases = cases ?? throw new ArgumentNullException(nameof(cases));
        this.initialParameters = initialParameters;
    }

    /// <summary>Append to <paramref name="sb"/></summary>
    public StringBuilder AppendToStringBuilder(StringBuilder sb)
    {
        sb.Append("(");
        int c = Cases.Length;
        for (int i = 0; i < c; i++)
        {
            if (i > 0) sb.Append(", ");
            sb.Append(Cases[i].PropertyName);
            sb.Append("=");
            sb.Append(Cases[i].Name);
        }
        sb.Append(")");
        return sb;
    }

    /// <summary>Print information</summary>
    public override string ToString()
    {
        // 
        StringBuilder sb = new StringBuilder();
        // "Scenario"
        sb.Append(nameof(Scenario));
        // Append
        AppendToStringBuilder(sb);
        // Print
        return sb.ToString();
    }

    /// <summary>Return new run.</summary>
    public Run Run()
    {
        // Copy
        Dictionary<string, object?> parameters = initialParameters != null ? new(initialParameters!) : new();
        // Create new run
        return new Run(this, parameters);
    }
}
