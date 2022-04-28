// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider;
using System;
using Avalanche.Utilities.Provider;

/// <summary>Opens <see cref="IResult"/> and returns its value. Rethrows captured exceptions.</summary>
public class ResultOpenedProvider : IProvider, IProviderStack, IDecoration, ICached
{
    /// <summary></summary>
    static readonly ConstructorT2<IProvider, IProvider> constructor = new(typeof(ResultOpenedProvider<,>));
    /// <summary></summary>
    /// <param name="keyType"></param>
    /// <param name="valueType">Value type, excluding <see cref="IResult{T}"/>.</param>
    public static IProvider Create(Type keyType, Type valueType, IProvider provider) => constructor.Create(keyType, valueType, provider);
    /// <summary></summary>
    public static IProvider Create(IProvider provider) => TypeUtilities.TryGetTypeArgumentOfCorrespondingDefinedType(provider.Value, typeof(IResult<>), 0, out Type _valueType) ? constructor.Create(provider.Key, _valueType, provider) : throw new ArgumentException(nameof(Provider));

    /// <summary>Source provider</summary>
    protected IProvider source;
    /// <summary>Value type</summary>
    protected Type? valueType;
    /// <summary>Source provider</summary>
    public IProvider[]? Sources => new IProvider[] { source };

    /// <summary></summary>
    bool IDecoration.IsDecoration { get => true; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    object? IDecoration.Decoree { get => source; set => throw new InvalidOperationException(); }

    /// <summary>Key type</summary>
    public Type Key => source.Key;
    /// <summary>Value type</summary>
    public Type Value => valueType ?? (TypeUtilities.TryGetTypeArgumentOfCorrespondingDefinedType(source.Value, typeof(IResult<>), 0, out valueType) ? valueType : throw new InvalidOperationException());
    /// <summary></summary>
    public bool IsCached { get => source is ICached cached ? cached.IsCached : false ; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public void InvalidateCache(bool deep = false) { if (source is ICached cached) cached.InvalidateCache(deep); }

    /// <summary></summary>
    public ResultOpenedProvider(IProvider source)
    {
        this.source = source ?? throw new ArgumentNullException(nameof(source));
    }

    /// <summary></summary>
    public bool TryGetValue(object key, out object value)
    {
        // Read value
        if (!source.TryGetValue(key, out object resultValue)) { value = null!; return false; }
        // 
        if (resultValue is not IResult result) { value = null!; return false; }
        //
        if (result.Status == ResultStatus.NoResult) { value = null!; return true; }
        if (result.Status == ResultStatus.Unassigned) { value = null!; return false; }
        if (result.Status == ResultStatus.Error) throw ExceptionUtilities.Wrap(result.Error!);
        value = result.Value!;
        return true;
    }

    /// <summary>Print information</summary>
    public override string ToString() => $"{source}.ResultOpened()";
}

/// <summary>Opens <see cref="IResult"/> and returns its value. Rethrows captured exceptions.</summary>
public class ResultOpenedProvider<Key, Value> : ResultOpenedProvider, IProvider<Key, Value>
{
    /// <summary></summary>
    protected new IProvider<Key, IResult<Value>> source;

    /// <summary></summary>
    public ResultOpenedProvider(IProvider source) : base(source)
    {
        this.source = (IProvider<Key, IResult<Value>>)source ?? throw new ArgumentNullException(nameof(source));
    }

    /// <summary></summary>
    public Value this[Key key]
    {
        get
        {
            // Get result object from source
            IResult<Value> result = source[key];
            // Read value
            Value value = result.AssertValue(key);
            // Return
            return value;
        }
    }

    /// <summary></summary>
    public bool TryGetValue(Key key, out Value value)
    {
        // Read value
        if (!source.TryGetValue(key, out IResult<Value> result)) { value = default!; return false; }
        //
        if (result.Status == ResultStatus.NoResult) { value = default!; return false; }
        if (result.Status == ResultStatus.Unassigned) { value = default!; return false; }
        if (result.Status == ResultStatus.Error) throw ExceptionUtilities.Wrap(result.Error!);
        value = result.Value!;
        return true;
    }
}

/// <summary>Opens <see cref="IResult"/> and returns its value. Rethrows captured exceptions.</summary>
public class ValueResultOpenedProvider<Key, Value> : ResultOpenedProvider, IProvider<Key, Value>
{
    /// <summary></summary>
    protected new IProvider<Key, ValueResult<Value>> source;

    /// <summary></summary>
    public ValueResultOpenedProvider(IProvider source) : base(source)
    {
        this.source = (IProvider<Key, ValueResult<Value>>)source ?? throw new ArgumentNullException(nameof(source));
    }

    /// <summary></summary>
    public Value this[Key key]
    {
        get {
            // Get result object from source
            ValueResult<Value> result = source[key];
            // Read value
            Value value = result.AssertValue(key);
            // Return
            return value;
        }
    }

    /// <summary></summary>
    public bool TryGetValue(Key key, out Value value)
    {
        // Read value
        if (!source.TryGetValue(key, out ValueResult<Value> result)) { value = default!; return false; }
        //
        if (result.Status == ResultStatus.NoResult) { value = default!; return false; }
        if (result.Status == ResultStatus.Unassigned) { value = default!; return false; }
        if (result.Status == ResultStatus.Error) throw ExceptionUtilities.Wrap(result.Error!);
        value = result.Value!;
        return true;
    }
}

