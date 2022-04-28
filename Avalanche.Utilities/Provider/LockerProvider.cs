// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider;
using Avalanche.Utilities.Provider;

/// <summary>Decorates provider so that it locks result if it implements <see cref="IReadOnly"/>.</summary>
public static class LockerProvider
{
    /// <summary>Constructor</summary>
    static readonly ConstructorT2<IProvider, IProvider> constructor = new(typeof(LockerProvider<,>));
    /// <summary>Constructor</summary>
    static readonly ConstructorT2<IProvider, IProvider> resultConstructor = new(typeof(ResultLockerProvider<,>));
    /// <summary>Constructor</summary>
    public static IProvider Create(Type keyType, Type valueType, IProvider provider)
    {
        // IResult<Value> provider
        if (TypeUtilities.TryGetTypeArgumentOfCorrespondingDefinedType(valueType, typeof(IResult<>), 0, out Type resultValueType))
            return resultConstructor.Create(keyType, resultValueType, provider);
        // Value provider
        else return constructor.Create(keyType, valueType, provider);
    }
    /// <summary></summary>
    public static IProvider Create(IProvider provider)
    {
        // Get types
        Type keyType = provider.Key, valueType = provider.Value;
        // IResult<Value> provider
        if (TypeUtilities.TryGetTypeArgumentOfCorrespondingDefinedType(valueType, typeof(IResult<>), 0, out Type resultValueType))
            return resultConstructor.Create(keyType, resultValueType, provider);
        // Value provider
        else return constructor.Create(keyType, valueType, provider);
    }
}

/// <summary>Decorates provider so that it locks result if it implements <see cref="IReadOnly"/>.</summary>
public class LockerProvider<Key, Value> : ProviderBase<Key, Value>, IProviderStack, IDecoration
{
    /// <summary>Provider</summary>
    protected IProvider<Key, Value> provider;
    /// <summary></summary>
    protected IProvider[] sources;
    /// <summary></summary>
    public IProvider[]? Sources => sources;
    /// <summary></summary>
    bool IDecoration.IsDecoration { get => true; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    object? IDecoration.Decoree { get => sources == null || sources.Length != 1 ? null : sources[0]; set => throw new NotImplementedException(); }

    /// <summary></summary>
    /// <param name="provider"></param>
    public LockerProvider(IProvider<Key, Value> provider)
    {
        this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
        this.sources = new IProvider[] { provider };
    }

    /// <summary></summary>
    public override bool TryGetValue(Key key, out Value value)
    {
        // Get value
        bool ok = provider.TryGetValue(key, out value);
        // Lock it
        if (ok && value is IReadOnly @readonly) @readonly.ReadOnly = true;
        // Return
        return ok;
    }
}

/// <summary>Decorates provider so that it locks result if it implements <see cref="IReadOnly"/>.</summary>
public class ResultLockerProvider<Key, Value> : ProviderBase<Key, IResult<Value>>, IProviderStack, IDecoration
{
    /// <summary>Provider</summary>
    protected IProvider<Key, IResult<Value>> provider;
    /// <summary></summary>
    protected IProvider[] sources;
    /// <summary></summary>
    public IProvider[]? Sources => sources;
    /// <summary></summary>
    bool IDecoration.IsDecoration { get => true; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    object? IDecoration.Decoree { get => sources == null || sources.Length != 1 ? null : sources[0]; set => throw new NotImplementedException(); }

    /// <summary></summary>
    /// <param name="provider"><![CDATA[IProvider<Key, IResult<Value>>]]></param>
    public ResultLockerProvider(IProvider provider)
    {
        this.provider = (IProvider<Key, IResult<Value>>)provider ?? throw new ArgumentNullException(nameof(provider));
        this.sources = new IProvider[] { this.provider };
    }

    /// <summary></summary>
    public override bool TryGetValue(Key key, out IResult<Value> value)
    {
        // Get value
        bool ok = provider.TryGetValue(key, out value);
        // Lock it
        if (ok && value.Status == ResultStatus.Ok && value.Value is IReadOnly @readonly) @readonly.ReadOnly = true;
        // Return
        return ok;
    }
    /// <summary>Print information</summary>
    public override string ToString() => $"{provider}.AsReadOnly()";
}

