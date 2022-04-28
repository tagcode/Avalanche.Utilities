// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider;

/// <summary>Provider evaluation result</summary>
public abstract record Result : ReadOnlyAssignableRecord, IResult, IReadOnly
{
    /// <summary>Ctor for <see cref="Result{T}"/>.</summary>
    static readonly ConstructorT<Result> ctor = new(typeof(Result<>));
    /// <summary>Ctor for <see cref="Result{T}"/>.</summary>
    public static Result Create(Type t) => ctor.Create(t);

    /*
    /// <summary>Create ok result.</summary>
    public static Result<T> Ok(T value) => new Result<T> { Value = value, Status = ResultStatus.Ok }.SetReadOnly();
    /// <summary>No result.</summary>
    public static Result<T> NoResult(T value) => new Result<T> { Status = ResultStatus.NoResult }.SetReadOnly();
    /// <summary>No result.</summary>
    public static Result<T> NoResult(T value) => new Result<T> { Status = ResultStatus.NoResult }.SetReadOnly();
    */

    /// <summary>Assert writable</summary>
    Result AssertWritable => @readonly ? throw new InvalidOperationException("Read-only") : this;

    /// <summary>Request</summary>
    protected object? request;
    /// <summary>Evaluation status</summary>
    protected ResultStatus status;
    /// <summary>Error code</summary>
    protected Exception? error;

    /// <summary>Request</summary>
    public object? Request { get => request; set => AssertWritable.request = value; }
    /// <summary>Evaluation status</summary>
    public ResultStatus Status { get => status; set => AssertWritable.status = value; }
    /// <summary>Error code</summary>
    public Exception? Error { get => error; set => AssertWritable.error = value; }
    /// <summary>Result value</summary>
    public object? Value { get => getValue(); set => setValue(value); }

    /// <summary></summary>
    protected abstract object? getValue();
    /// <summary></summary>
    protected abstract void setValue(object? value);
}

/// <summary>Provider evaluation result</summary>
public record Result<T> : Result, IResult<T>
{
    /// <summary>Assert writable</summary>
    Result<T> AssertWritable => @readonly ? throw new InvalidOperationException("Read-only") : this;
    /// <summary>Result value</summary>
    protected T value = default!;
    /// <summary>Result value</summary>
    public new T Value { get => value; set => AssertWritable.value = value; }

    /// <summary></summary>
    protected override object? getValue() => value;
    /// <summary></summary>
    protected override void setValue(object? value) => this.value = value == null ? default(T)! : (T)value;
    /// <summary></summary>
    public override string ToString() => value?.ToString() ?? "";
}

/// <summary>Create immutable  <see cref="ResultStatus.NoResult"/></summary>
/// <typeparam name="T"></typeparam>
public class NoResult<T> : IResult<T>
{
    /// <summary></summary>
    public T Value { get => default!; set => throw new InvalidOperationException("Read-only"); }
    /// <summary></summary>
    public object? Request { get => request; set => request = value; }
    /// <summary></summary>
    public ResultStatus Status { get => ResultStatus.NoResult; set => throw new InvalidOperationException("Read-only"); }
    /// <summary></summary>
    public Exception? Error { get => null; set => throw new InvalidOperationException("Read-only"); }
    /// <summary></summary>
    object? IResult.Value { get => null; set => throw new InvalidOperationException("Read-only"); }
    /// <summary></summary>
    protected object? request;

    /// <summary></summary>
    public NoResult()
    {
    }

    /// <summary></summary>
    public NoResult(object? request)
    {
        this.request = request;
    }
    /// <summary></summary>
    public override string ToString() => $"NoResult<{CanonicalName.Print(typeof(T), CanonicalNameOptions.IncludeGenerics)}>";
}

/// <summary>Create immutable result <see cref="ResultStatus.Ok"/>.</summary>
/// <typeparam name="T"></typeparam>
public class ResultOk<T> : IResult<T>
{
    /// <summary></summary>
    public T Value { get => value; set => throw new InvalidOperationException("Read-only"); }
    /// <summary></summary>
    public object? Request { get => request; set => request = value; }
    /// <summary></summary>
    public ResultStatus Status { get => ResultStatus.Ok; set => throw new InvalidOperationException("Read-only"); }
    /// <summary></summary>
    public Exception? Error { get => null; set => throw new InvalidOperationException("Read-only"); }
    /// <summary></summary>
    object? IResult.Value { get => value; set => throw new InvalidOperationException("Read-only"); }

    /// <summary></summary>
    protected T value;
    /// <summary></summary>
    protected object? request;

    /// <summary></summary>
    public ResultOk(T value)
    {
        this.value = value;
    }

    /// <summary></summary>
    public ResultOk(T value, object? request)
    {
        this.value = value;
        this.request = request;
    }
    /// <summary></summary>
    public override string ToString() => value?.ToString()??"";
}

/// <summary>Create immutable result <see cref="ResultStatus.Error"/>.</summary>
/// <typeparam name="T"></typeparam>
public class ResultError<T> : IResult<T>
{
    /// <summary></summary>
    public T Value { get => default!; set => throw new InvalidOperationException("Read-only"); }
    /// <summary></summary>
    public object? Request { get => request; set => request = value; }
    /// <summary></summary>
    public ResultStatus Status { get => ResultStatus.Error; set => throw new InvalidOperationException("Read-only"); }
    /// <summary></summary>
    public Exception? Error { get => error; set => throw new InvalidOperationException("Read-only"); }
    /// <summary></summary>
    object? IResult.Value { get => null; set => throw new InvalidOperationException("Read-only"); }

    /// <summary></summary>
    protected Exception error;
    /// <summary></summary>
    protected object? request;

    /// <summary></summary>
    public ResultError(Exception error)
    {
        this.error = error;
    }

    /// <summary></summary>
    public ResultError(Exception error, object? request)
    {
        this.error = error;
        this.request = request;
    }
    /// <summary></summary>
    public override string ToString() => error?.ToString()??"";
}

