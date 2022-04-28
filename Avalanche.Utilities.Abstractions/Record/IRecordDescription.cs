// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System;
using System.Reflection;

// <docs>
/// <summary>Record description</summary>
public interface IRecordDescription : IAnnotable
{
    /// <summary>Record name, typically as <see cref="string"/> or <![CDATA[IIdentity]]>.</summary>
    object Name { get; set; }
    /// <summary>Record type</summary>
    Type Type { get; set; }
    /// <summary>All record constructors</summary>
    IConstructorDescription[]? Constructors { get; set; }
    /// <summary>Describes record deconstructor such as <see cref="MethodInfo"/>, <see cref="Delegate"/>, <![CDATA[IWriterBase]]></summary>
    object? Deconstructor { get; set; }
    /// <summary>Fields</summary>
    IFieldDescription[] Fields { get; set; }
    /// <summary>Selected construction strategy, typically <see cref="IConstructionDescription"/>.</summary>
    object? Construction { get; set; }
}
// </docs>

