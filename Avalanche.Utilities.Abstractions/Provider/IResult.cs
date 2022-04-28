// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider;
using System;

// <docs>
/// <summary>Evaluation result</summary>
public interface IResult
{
    /// <summary>Request</summary>
    object? Request { get; set; }
    /// <summary>Evaluation status</summary>
    ResultStatus Status { get; set; }
    /// <summary>Result value</summary>
    object? Value { get; set; }
    /// <summary>Error code</summary>
    Exception? Error { get; set; }
}

/// <summary>Evaluation status </summary>
public enum ResultStatus : int
{
    /// <summary>No evaluation</summary>
    Unassigned = 0,
    /// <summary>Resulted to no value</summary>
    NoResult = 1,
    /// <summary>Resulted to successful value</summary>
    Ok = 2,
    /// <summary>Resulted to error</summary>
    Error = 3,
}
// </docs>

// <docsT>
/// <summary>Evaluation result</summary>
public interface IResult<T> : IResult
{
    /// <summary>Result value, valid only if <see cref="ResultStatus.Ok"/></summary>
    new T Value { get; set; }
}
// </docsT>
