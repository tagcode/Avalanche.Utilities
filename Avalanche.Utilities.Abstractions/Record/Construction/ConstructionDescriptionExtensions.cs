// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;

/// <summary>Extension methods for <see cref="IConstructionDescription"/>.</summary>
public static class ConstructionDescriptionExtensions
{
    /// <summary>Constructor to match</summary>
    public static T SetConstructor<T>(this T construction, IConstructorDescription value) where T : IConstructionDescription { construction.Constructor = value; return construction; }
    /// <summary>Parameters to match to</summary>
    public static T SetParameters<T>(this T construction, IParameterDescription[] value) where T : IConstructionDescription { construction.Parameters = value; return construction; }
    /// <summary>Fields to match to</summary>
    public static T SetFields<T>(this T construction, IFieldDescription[] value) where T : IConstructionDescription { construction.Fields = value; return construction; }

    /// <summary>Constructor Parameter to Record Field correlations</summary>
    public static T SetParameterToField<T>(this T construction, IDictionary<IParameterDescription, IFieldDescription> value) where T : IConstructionDescription { construction.ParameterToField = value; return construction; }
    /// <summary>Record Field to Constructo Parameter</summary>
    public static T SetFieldToParameter<T>(this T construction, IDictionary<IFieldDescription, IParameterDescription> value) where T : IConstructionDescription { construction.FieldToParameter = value; return construction; }

    /// <summary>Unmatched constructor parameters</summary>
    public static T SetUnmatchedParameters<T>(this T construction, IList<IParameterDescription> value) where T : IConstructionDescription { construction.UnmatchedParameters = value; return construction; }
    /// <summary>Unmatched constructor parameters</summary>
    public static T SetUnmatchedFields<T>(this T construction, IList<IFieldDescription> value) where T : IConstructionDescription { construction.UnmatchedFields = value; return construction; }

    /// <summary>Count score</summary>
    public static int Score(this IConstructionDescription constructionDescription)
    {
        // Count score here
        int score = 0;
        // Add score for each readable field that matches to constructor parameter
        score += constructionDescription.ParameterToField.Count * 10000;
        // Reduce score for each unmatched parameter
        score += constructionDescription.UnmatchedParameters.Count * -1000;
        // Reduce score for each unwritable field
        foreach (var field in constructionDescription.UnmatchedFields) if (field.Writer == null) score -= 1000;
        // Return
        return score;
    }


}
