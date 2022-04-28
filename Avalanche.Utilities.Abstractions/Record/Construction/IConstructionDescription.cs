// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Collections.Generic;

// <docs>
/// <summary>Description of how to apply <see cref="IConstructorDescription"/>, a constructor-parameter-to-field correlaton.</summary>
public interface IConstructionDescription
{
    /// <summary>Constructor to match</summary>
    IConstructorDescription Constructor { get; set; }
    /// <summary>Parameters to match to</summary>
    IParameterDescription[] Parameters { get; set; }
    /// <summary>Fields to match to</summary>
    IFieldDescription[] Fields { get; set; }

    /// <summary>Constructor Parameter to Record Field correlations</summary>
    IDictionary<IParameterDescription, IFieldDescription> ParameterToField { get; set; }
    /// <summary>Record Field to Constructo Parameter</summary>
    IDictionary<IFieldDescription, IParameterDescription> FieldToParameter { get; set; }

    /// <summary>Unmatched constructor parameters</summary>
    IList<IParameterDescription> UnmatchedParameters { get; set; }
    /// <summary>Unmatched constructor fields</summary>
    IList<IFieldDescription> UnmatchedFields { get; set; }
}
// </docs>

