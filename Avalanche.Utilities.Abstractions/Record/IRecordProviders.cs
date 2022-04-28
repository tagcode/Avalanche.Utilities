// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System;
using Avalanche.Utilities.Provider;

// <docs>
/// <summary>Provides record and field descriptions and delegates.</summary>
public interface IRecordProviders
{
    /// <summary>Get record description for recordType.</summary>
    IProvider<Type, IResult<IRecordDescription>> RecordDescription { get; set; }

    /// <summary>Get all record delegates.</summary>
    IProvider<Type, IResult<IRecordDelegates>> RecordDelegatesByType { get; set; }
    /// <summary>Get all record delegates.</summary>
    IProvider<IRecordDescription, IResult<IRecordDelegates>> RecordDelegates { get; set; }

    /// <summary>Get <![CDATA[Func<object[], Record>]]> delegate.</summary>
    IProvider<IRecordDescription, IResult<Delegate>> RecordCreate { get; set; }

    /// <summary>Get all field delegates.</summary>
    IProvider<IFieldDescription, IResult<IFieldDelegates>> FieldDelegates { get; set; }
    /// <summary>Get specific field delegate.</summary>
    IProvider<IFieldDescription, IResult<Delegate>> FieldRead { get; set; }
    /// <summary>Get specific field delegate.</summary>
    IProvider<IFieldDescription, IResult<Delegate>> FieldWrite { get; set; }
    /// <summary>Get specific field delegate.</summary>
    IProvider<IFieldDescription, IResult<Delegate>> RecreateWith { get; set; }
}
// </docs>
