// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System;
using System.Reflection;

// <docs>
/// <summary>Constructor description</summary>
public interface IConstructorDescription : IAnnotable
{
    /// <summary>
    /// Describes record constructor method: 
    ///     <see cref="ConstructorInfo"/>
    ///     <see cref="MethodInfo"/>
    ///     <see cref="Delegate"/>
    ///     <![CDATA[IWriterBase]]> as <see cref="ValueType"/>
    ///     <![CDATA[EmitLine]]> (e.g. (OpCodes.Newobj, ci), (OpCodes.Initobj, ValueType) )
    ///     <![CDATA[IEnumerable<EmitLine>]]>.
    /// </summary>
    object Constructor { get; set; }
    /// <summary>Record type</summary>
    Type Type { get; set; }
    /// <summary>Parameters</summary>
    IParameterDescription[] Parameters { get; set; }
    /// <summary>Record which constructor is member of.</summary>
    IRecordDescription? Record { get; set; }
}
// </docs>

