// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

/// <summary></summary>
public class PermutationSetup
{
    /// <summary></summary>
    public readonly IDictionary<string, object>? initialParameters;
    /// <summary></summary>
    Dictionary<string, Property> properties = new Dictionary<string, Property>();
    /// <summary></summary>
    Property[]? propertiesSorted, runProperties;

    /// <summary></summary>
    public PermutationSetup(IDictionary<string, object>? initialParameters = default)
    {
        this.initialParameters = initialParameters ?? new Dictionary<string, object>();
    }

    /// <summary></summary>
    protected virtual void ClearCache()
    {
        propertiesSorted = null;
        runProperties = null;
    }

    /// <summary></summary>
    public PermutationSetup Add(Case @case)
    {
        Property? property = null;
        if (!properties.TryGetValue(@case.PropertyName, out property))
            properties[@case.PropertyName] = property = new Property(@case.PropertyName);
        property.AddCase(@case);
        ClearCache();
        return this;
    }

    /// <summary></summary>
    public PermutationSetup Add(string propertyName, string caseName, string[]? propertyDependencies = null, Func<Run, object>? initializer = null, Action<Run>? cleanup = null)
        => Add(new Case(propertyName, caseName, propertyDependencies, initializer, cleanup));

    /// <summary></summary>
    public Case? GetCase(string propertyName, string caseName)
    {
        Property? property;
        if (!properties.TryGetValue(propertyName, out property)) return null;
        return property.cases.Where(c => c.Name.Equals(caseName)).FirstOrDefault();
    }

    /// <summary></summary>
    IEnumerable<Property> ListProperties()
    {
        Dictionary<string, Property> visited = new Dictionary<string, Property>();
        LinkedList<Property> queue = new LinkedList<Property>(properties.Values);
        while (queue.Count > 0)
        {
            Property property = queue.First!.Value;
            queue.RemoveFirst();
            if (visited.ContainsKey(property.Name)) continue;

            // Dependencies ok
            bool depsOk = true;
            foreach (string depName in property.dependencies)
            {
                depsOk &= visited.ContainsKey(depName);
                if (!depsOk) break;
            }

            if (depsOk)
            {
                visited[property.Name] = property;
                yield return property;
            }
            else
            {
                queue.AddLast(property);
            }
        }
    }

    /// <summary></summary>
    IEnumerable<Property> ListRunProperties() => Properties.Where(p => p.cases.Where(c => c.RunFunc != null).FirstOrDefault() != null);

    /// <summary>List all properties. Return in order of least dependencies to properties with most dependencies.</summary>
    public Property[] Properties => propertiesSorted ?? (propertiesSorted = ListProperties().ToArray());

    /// <summary>List properties that have atleast one case with a runFunc.</summary>
    public Property[] RunProperties => runProperties ?? (runProperties = ListRunProperties().ToArray());

    /// <summary>Enumerate list of all test scenarios</summary>
    public IEnumerable<Scenario> Scenarios
    {
        get
        {
            Property[] properties = Properties;
            int c = properties.Length;
            int[] counts = properties.Select(p => p.cases.Count).ToArray();
            int totalCount = 1;
            foreach (int caseCount in counts) totalCount *= caseCount;
            if (totalCount == 0) yield break;
            int[] ixs = new int[c];

            while (true)
            {
                Case[] cases = new Case[ixs.Length];
                for (int i = 0; i < c; i++)
                    cases[i] = properties[i].cases[ixs[i]];
                yield return new Scenario(cases, initialParameters);

                // Go to next index
                int j = 0;
                while (true)
                {
                    ixs[j]++;
                    if (ixs[j] < counts[j]) break;
                    ixs[j] %= counts[j];
                    j++;
                    if (j >= c) yield break;
                }
            }
        }
    }
}
