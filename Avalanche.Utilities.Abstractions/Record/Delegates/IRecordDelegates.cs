// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System;

// <docs>
/// <summary>Record delegates</summary>
public interface IRecordDelegates
{
    /// <summary>Record Type</summary>
    Type RecordType { get; set; }
    /// <summary>Creates record with initial values <![CDATA[Func<object[], Record>]]></summary>
    Delegate? RecordCreate { get; set; }
    /// <summary>Field delegates</summary>
    IFieldDelegates[]? FieldDelegates { get; set; }
    /// <summary></summary>
    IRecordDescription? RecordDescription { get; set; }
}

/// <summary>Record delegates</summary>
public interface IRecordDelegates<Record> : IRecordDelegates
{
    /// <summary>Creates record with initial values <![CDATA[Func<object[], Record>]]></summary>
    new Func<object[], Record>? RecordCreate { get; set; }
}
// </docs>

