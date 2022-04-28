// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider;

/// <summary></summary>
public record struct ValueResult<T> : IResult<T>
{
    /// <summary></summary>
    public static implicit operator ValueResult<T>(T value) => new ValueResult<T> { Value = value, Status = ResultStatus.Ok };
    /// <summary></summary>
    public static implicit operator ValueResult<T>(Exception exception) => new ValueResult<T> { Error = exception, Status = ResultStatus.Error };

    /// <summary></summary>
    public static ValueResult<T> CopyFrom(IResult src) => new ValueResult<T> { Value = src.Value is T casted ? casted : default!, Request = src.Request, Status = src.Status, Error = src.Error };
    /// <summary></summary>
    public T Value { get; set; }
    /// <summary></summary>
    public object? Request { get; set; }
    /// <summary></summary>
    public ResultStatus Status { get; set; }
    /// <summary></summary>
    public Exception? Error { get; set; }
    /// <summary></summary>
    object? IResult.Value { get => Value; set => this.Value = value == null ? default! : (T)Value!; }
}
