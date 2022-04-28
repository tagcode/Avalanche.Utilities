// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System;

// <docs>
/// <summary>Field delegates</summary>
public interface IFieldDelegates
{
    /// <summary>Record Type of delegates</summary>
    Type RecordType { get; set; }
    /// <summary>Field Type  of delegates</summary>
    Type FieldType { get; set; }
    /// <summary>Assign new value</summary>
    Delegate? FieldRead { get; set; }
    /// <summary>Read field value</summary>
    Delegate? FieldWrite { get; set; }
    /// <summary>Create new instance of record with new value.</summary>
    Delegate? RecreateWith { get; set; }
    /// <summary></summary>
    IFieldDescription? FieldDescription { get; set; }
}

/// <summary>Field delegates</summary>
public interface IFieldDelegates<Record> : IFieldDelegates { }

/// <summary>Field delegates</summary>
public interface IFieldDelegates<Record, Field> : IFieldDelegates<Record>
{
    /// <summary>Read field value</summary>
    new FieldRead<Record, Field>? FieldRead { get; set; }
    /// <summary>Assign new value to newValue.</summary>
    new FieldWrite<Record, Field>? FieldWrite { get; set; }
    /// <summary>Create new instance of record with newValue.</summary>
    new RecreateWith<Record, Field>? RecreateWith { get; set; }
}
// </docs>
