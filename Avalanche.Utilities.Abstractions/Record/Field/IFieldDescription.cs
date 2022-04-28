// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System;
using System.Reflection;

// <docs>
/// <summary>Field description</summary>
public interface IFieldDescription : IAnnotable
{
    /// <summary>Field name</summary>
    object Name { get; set; }
    /// <summary>Field type</summary>
    Type Type { get; set; }
    /// <summary>Describes field writer: <see cref="ParameterInfo"/>, <see cref="FieldInfo"/>, <see cref="PropertyInfo"/>, <see cref="MethodInfo"/>, <see cref="Delegate"/>, <![CDATA[IWriterBase]]>, <![CDATA[IRefererBase]]> or <![CDATA[EmitLine]]></summary>
    Object? Writer { get; set; }
    /// <summary>Describes field reader: <see cref="FieldInfo"/>, <see cref="PropertyInfo"/>, <see cref="MethodInfo"/>, <see cref="Delegate"/>, <![CDATA[IWriterBase]]>, <![CDATA[IRefererBase]]>, or <![CDATA[EmitLine]]></summary>
    Object? Reader { get; set; }
    /// <summary>Describes field reader: <see cref="FieldInfo"/></summary>
    Object? Referer { get; set; }
    /// <summary>Record which field is member of.</summary>
    IRecordDescription? Record { get; set; }
    /// <summary>Initial value</summary>
    Object? InitialValue { get; set; }
}
// </docs>

