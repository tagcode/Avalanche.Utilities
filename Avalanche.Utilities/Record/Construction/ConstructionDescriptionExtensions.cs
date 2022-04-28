// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;

/// <summary>Extension methods for <see cref="IConstructionDescription"/>.</summary>
public static class ConstructionDescriptionExtensions_
{
    /// <summary>Correlate <paramref name="constructor"/> to <paramref name="fields"/>.</summary>
    public static IConstructionDescription Correlate(this IConstructionDescription constructionDescription, IConstructorDescription constructor, IFieldDescription[] fields)
    {
        // Assign and assert
        constructionDescription.Constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
        constructionDescription.Parameters = constructor.Parameters ?? throw new ArgumentNullException(nameof(constructor.Parameters));
        constructionDescription.Fields = fields ?? throw new ArgumentNullException(nameof(fields));

        // Parameters by name.
        BijectionMap<string, IParameterDescription> ParametersByName = new(StringComparer.Ordinal, ReferenceEqualityComparer<IParameterDescription>.Instance);
        // Parameters by name ignroe case.
        BijectionMap<string, IParameterDescription> ParametersByNameIgnoreCase = new(StringComparer.OrdinalIgnoreCase, ReferenceEqualityComparer<IParameterDescription>.Instance);

        // Fields by name.
        BijectionMap<string, IFieldDescription> FieldsByName = new(StringComparer.Ordinal, ReferenceEqualityComparer<IFieldDescription>.Instance);
        // Fields by name ignroe case.
        BijectionMap<string, IFieldDescription> FieldsByNameIgnoreCase = new(StringComparer.OrdinalIgnoreCase, ReferenceEqualityComparer<IFieldDescription>.Instance);

        // Assign parameter names
        for (int i = 0; i < constructor.Parameters.Length; i++)
        {
            // Get parameter
            IParameterDescription parameter = constructor.Parameters[i];
            // Get name
            string name = parameter.Name?.ToString() ?? i.ToString()!;
            // Assign to maps
            ParametersByName[name] = parameter;
            ParametersByNameIgnoreCase[name] = parameter;
        }

        // Assign field names
        for (int i = 0; i < fields.Length; i++)
        {
            // Get parameter
            IFieldDescription field = fields[i];
            // Get name
            string name = field.Name?.ToString() ?? i.ToString()!;
            // Assign to maps
            FieldsByName[name] = field;
            FieldsByNameIgnoreCase[name] = field;
        }

        // List of parameters to match
        List<IParameterDescription> parametersToMatch = new List<IParameterDescription>(constructionDescription.Parameters);
        // List of fields to match
        List<IFieldDescription> fieldsToMatch = new List<IFieldDescription>(constructionDescription.Fields);

        // Match parameter to field by exact name
        foreach (IParameterDescription parameter in constructionDescription.Parameters)
        {
            // Get parameter identity
            object parameterIdentity = ParametersByName.GetLeft(parameter);
            // Get matching field
            if (!FieldsByName.TryGetRight(parameterIdentity.ToString()!, out IFieldDescription? field)) continue;
            // Field is not readable
            if (field!.Reader == null) continue;
            // Add match
            constructionDescription.ParameterToField[parameter] = field;
            constructionDescription.FieldToParameter[field] = parameter;
            // Remove from list of objects to match
            parametersToMatch.Remove(parameter);
            fieldsToMatch.Remove(field);
        }

        // Match by case ignore
        foreach (IParameterDescription parameter in constructionDescription.Parameters)
        {
            // Get parameter identity
            object parameterIdentity = ParametersByName.GetLeft(parameter);
            // Get matching field
            if (!FieldsByNameIgnoreCase.TryGetRight(parameterIdentity.ToString()!, out IFieldDescription? field)) continue;
            // Field is not readable
            if (field!.Reader == null) continue;
            // Add match
            constructionDescription.ParameterToField[parameter] = field;
            constructionDescription.FieldToParameter[field] = parameter;
            // Remove from list of objects to match
            parametersToMatch.Remove(parameter);
            fieldsToMatch.Remove(field);
        }

        // Add unmatched parameters
        foreach (var parameter in parametersToMatch) constructionDescription.UnmatchedParameters.Add(parameter);
        // Add unmatched fields
        foreach (var field in fieldsToMatch) constructionDescription.UnmatchedFields.Add(field);

        // Return
        return constructionDescription;
    }

    /// <summary>Create construction strategy for constructor. Requires that <see cref="IConstructorDescription.Record"/> is assigned</summary>
    public static ConstructionDescription CreateConstructionDescription(this IConstructorDescription constructorDescription)
    {
        // Get record description
        IRecordDescription recordDescription = constructorDescription.Record ?? throw new ArgumentException($"{nameof(IConstructorDescription.Record)} must be assigned.");
        // Create construction strategy
        ConstructionDescription construction = new ConstructionDescription();
        // Correlate parameters to fields
        construction.Correlate(constructorDescription, recordDescription.Fields);
        // Return
        return construction;
    }

    /// <summary>Create construction strategies for record.</summary>
    public static IEnumerable<IConstructionDescription> CreateConstructions(this IEnumerable<IConstructorDescription> constructorDescriptions)
    {
        // Create construction
        IEnumerable<IConstructionDescription> constructions = constructorDescriptions.Select(CreateConstructionDescription);
        // Return
        return constructions;
    }

    /// <summary>Choose best matching construction strategy.</summary>
    public static IConstructionDescription? BestConstruction(this IEnumerable<IConstructionDescription> constructionDescriptions)
    {
        // Place here best construction strategy
        IConstructionDescription? bestConstruction = null;
        // Place score of best match
        int bestScore = int.MinValue;

        // Choose best constructor match
        foreach (IConstructionDescription construction in constructionDescriptions)
        {
            // Not most suitable construction
            if (construction.Score() < bestScore) continue;
            // Assign 
            bestScore = construction.Score();
            bestConstruction = construction;
        }

        // Return
        return bestConstruction;
    }

    /// <summary>Clone <paramref name="src"/> in writable state.</summary>
    public static ConstructionDescription Clone(this IConstructionDescription src)
    {
        // Create result
        ConstructionDescription result = new ConstructionDescription();
        //
        result.Constructor = src.Constructor;
        result.Parameters = (IParameterDescription[]?)src.Parameters?.Clone()!;
        result.Fields = (IFieldDescription[]?)src.Fields?.Clone()!;
        result.ParameterToField = new Dictionary<IParameterDescription, IFieldDescription>(src.ParameterToField);
        result.FieldToParameter = new Dictionary<IFieldDescription, IParameterDescription>(src.FieldToParameter);
        result.UnmatchedParameters = new List<IParameterDescription>(src.UnmatchedParameters);
        result.UnmatchedFields = new List<IFieldDescription>(src.UnmatchedFields);
        // Return
        return result;
    }

    /// <summary>Calculate hash</summary>
    public static void HashIn(this IConstructionDescription construction, ref FNVHash64 hash)
    {
        // 
        if (construction.Constructor != null) construction.Constructor.HashIn(ref hash);
        /*
        // Hash constructor
        if (hashConstructor && construction.Constructor != null) hash.HashIn(construction.Constructor.GetHashCode());
        // Hash parameters
        if (hashParameters && construction.Parameters != null)
        {
            foreach (var parameter in construction.Parameters)
            {
                parameter.HashIn(ref hash);
            }
        }
        // Hash annotations
        if (hashAnnotations && construction.Annotations != null)
        {
            foreach (var annotation in construction.Annotations)
            {
                hash.HashIn(annotation.GetHashCode());
            }
        }
        */
    }

    /// <summary>Calculate hash64</summary>
    public static ulong CalcHash64(this IConstructionDescription construction)
    {
        // Init
        FNVHash64 hash = new FNVHash64();
        // Hashin
        construction.HashIn(ref hash);
        // Return
        return hash.Hash;
    }


    /// <summary>Removes fields and field correlations from <paramref name="constructionDescription"/>.</summary>
    public static T StripToParameters<T>(this T constructionDescription) where T : IConstructionDescription
    {
        IFieldDescription[] fields = new IFieldDescription[constructionDescription.Parameters.Length];
        int ix = 0;
        foreach (IFieldDescription field in constructionDescription.Fields)
        {
            if (!constructionDescription.FieldToParameter.ContainsKey(field)) continue;
            fields[ix++] = field;
        }
        constructionDescription.Fields = fields;
        constructionDescription.UnmatchedFields.Clear();
        return constructionDescription;
    }
}
