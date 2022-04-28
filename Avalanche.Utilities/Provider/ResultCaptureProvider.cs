// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Avalanche.Utilities.Provider;

/// <summary>Captures value or error and wraps into <see cref="IResult"/>.</summary>
public abstract class ResultCaptureProvider : IProvider, IProviderStack, IDecoration, ICached
{
    /// <summary>Constructor</summary>
    static readonly ConstructorT2<IProvider, ResultCaptureProvider> ctor = new(typeof(ResultCaptureProvider<,>));
    /// <summary>Create</summary>
    public static ResultCaptureProvider Create(Type keyType, Type valueType, IProvider createProvider) => ctor.Create(keyType, valueType, createProvider);
    /// <summary>Create</summary>
    public static ResultCaptureProvider Create(IProvider createProvider) => ctor.Create(createProvider.Key, createProvider.Value, createProvider);

    /// <summary></summary>
    public abstract Type Key { get; }
    /// <summary></summary>
    public abstract Type Value { get; }
    /// <summary></summary>
    public abstract IProvider[]? Sources { get; }
    /// <summary></summary>
    bool IDecoration.IsDecoration { get => true; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public abstract object? Decoree { get; set; }
    /// <summary></summary>
    public bool IsCached { get => Decoree is ICached cached ? cached.IsCached : false; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public void InvalidateCache(bool deep = false) { if (Decoree is ICached cached) cached.InvalidateCache(deep); }

    /// <summary>Get value that corresponds <paramref name="key"/></summary>
    /// <exception cref="InvalidCastException">If key is wrong type.</exception>
    public abstract bool TryGetValue(object key, out object value);
}

/// <summary>Captures value or error and wraps into <see cref="IResult"/>.</summary>
public class ResultCaptureProvider<TKey, TValue> : ResultCaptureProvider, IProvider<TKey, IResult<TValue>>
{
    /// <summary></summary>
    public override Type Key => typeof(TKey);
    /// <summary></summary>
    public override Type Value => typeof(IResult<TValue>);
    /// <summary></summary>
    public override IProvider[]? Sources => new[] { source };
    /// <summary></summary>
    protected IProvider<TKey, TValue> source;
    /// <summary></summary>
    public override object? Decoree { get => source; set => throw new NotImplementedException(); }

    /// <summary></summary>
    public ResultCaptureProvider(IProvider<TKey, TValue> source)
    {
        this.source = source ?? throw new ArgumentNullException(nameof(source));
    }

    /// <summary>Key to value indexer</summary>
    public IResult<TValue> this[TKey key]
    {
        get
        {
            try
            {
                // Return ok
                if (source.TryGetValue(key, out TValue value)) return new ResultOk<TValue>(value);
                // Return no result
                else return new NoResult<TValue>();
            }
            catch (Exception e)
            {
                // Return no result
                return new ResultError<TValue>(e);
            }
        }
    }

    /// <summary>Try get value</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public bool TryGetValue(TKey key, out IResult<TValue> value)
    {
        try
        {
            // Return ok
            if (source.TryGetValue(key, out TValue v)) value = new ResultOk<TValue>(v);
            // Return no result
            else value = new NoResult<TValue>();
        }
        catch (Exception e)
        {
            // Return no result
            value = new ResultError<TValue>(e);
        }
        // 
        return true;
    }

    /// <summary>Try get value</summary>
    /// <exception cref="InvalidCastException">If key is not <typeparamref name="TKey"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public override bool TryGetValue(object key, out object value)
    {
        try
        {
            // Return ok
            if (source.TryGetValue((TKey)key, out TValue v)) value = new ResultOk<TValue>(v);
            // Return no result
            else value = new NoResult<TValue>();
        }
        catch (Exception e)
        {
            // Return no result
            value = new ResultError<TValue>(e);
        }
        // 
        return true;
    }

    /// <summary>Print information</summary>
    public override string ToString() => $"{source}.ResultCaptured()";
}

/// <summary>Captures value or error and wraps into <see cref="IResult"/>.</summary>
public class ValueResultCaptureProvider<TKey, TValue> : ResultCaptureProvider, IProvider<TKey, ValueResult<TValue>>
{
    /// <summary></summary>
    public override Type Key => typeof(TKey);
    /// <summary></summary>
    public override Type Value => typeof(ValueResult<TValue>);
    /// <summary></summary>
    public override IProvider[]? Sources => new[] { source };
    /// <summary></summary>
    protected IProvider<TKey, TValue> source;
    /// <summary></summary>
    public override object? Decoree { get => source; set => throw new NotImplementedException(); }

    /// <summary></summary>
    public ValueResultCaptureProvider(IProvider<TKey, TValue> source)
    {
        this.source = source ?? throw new ArgumentNullException(nameof(source));
    }

    /// <summary>Key to value indexer</summary>
    public ValueResult<TValue> this[TKey key]
    {
        get
        {
            try
            {
                // Return ok
                if (source.TryGetValue(key, out TValue value)) return value;
                // Return no result
                else return new ValueResult<TValue> { Status = ResultStatus.NoResult };
            }
            catch (Exception e)
            {
                // Return no result
                return new ValueResult<TValue> { Status = ResultStatus.Error, Error = e };
            }
        }
    }

    /// <summary>Try get value</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public bool TryGetValue(TKey key, out ValueResult<TValue> value)
    {
        try
        {
            // Return ok
            if (source.TryGetValue(key, out TValue v)) value = v;
            // Return no result
            else value = new ValueResult<TValue> { Status = ResultStatus.NoResult };
        }
        catch (Exception e)
        {
            // Return no result
            value = new ValueResult<TValue> { Status = ResultStatus.Error, Error = e };
        }
        // 
        return true;
    }

    /// <summary>Try get value</summary>
    /// <exception cref="InvalidCastException">If key is not <typeparamref name="TKey"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public override bool TryGetValue(object key, out object value)
    {
        try
        {
            // Return ok
            if (source.TryGetValue((TKey)key, out TValue v)) value = new ValueResult<TValue> { Status = ResultStatus.Ok, Value = v };
            // Return no result
            else value = new ValueResult<TValue> { Status = ResultStatus.NoResult };
        }
        catch (Exception e)
        {
            // Return no result
            value = new ValueResult<TValue> { Error = e, Status = ResultStatus.Error };
        }
        // 
        return true;
    }

    /// <summary>Print information</summary>
    public override string ToString() => $"{source}.ResultCaptured()";
}

