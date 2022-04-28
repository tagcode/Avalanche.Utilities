// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System;
using System.Reflection;

// <docs>
/// <summary>Function or Constructor parameter description</summary>
public interface IParameterDescription : IAnnotable
{
    /// <summary>Parameter name, <see cref="string"/> or <![CDATA[IIdentity]]>.</summary>
    object Name { get; set; }
    /// <summary>Parameter type</summary>
    Type Type { get; set; }
    /// <summary>Describes parameter writer: <see cref="ParameterInfo"/></summary>
    object? Writer { get; set; }
    /// <summary>Function or constructor which parameter is member of, as <see cref="IConstructorDescription"/>.</summary>
    IConstructorDescription? Member { get; set; }
    /// <summary>Parameter is optional</summary>
    bool? Optional { get; set; }
}
// </docs>

