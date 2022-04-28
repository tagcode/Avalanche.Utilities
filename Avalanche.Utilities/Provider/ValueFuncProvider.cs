// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider;

/// <summary>Decoration that applies <see cref="Func{T, TResult}"/> on provider value</summary>
public class ValueFuncProvider<Key, SrcValue, DstValue> : ProviderBase<Key, DstValue>, IDecoration
{
    /// <summary></summary>
    protected IProvider<Key, SrcValue> source;
    /// <summary></summary>
    protected Func<SrcValue, DstValue> valueFunc;
    /// <summary></summary>
    public virtual IProvider<Key, SrcValue> Source => source;
    /// <summary></summary>
    public virtual Func<SrcValue, DstValue> ValueFunc => valueFunc;
    /// <summary></summary>
    bool IDecoration.IsDecoration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    /// <summary></summary>
    object? IDecoration.Decoree { get => source; set => throw new InvalidOperationException(); }

    /// <summary></summary>
    public ValueFuncProvider(IProvider<Key, SrcValue> source, Func<SrcValue, DstValue> valueFunc)
    {
        this.source = source ?? throw new ArgumentNullException(nameof(source));
        this.valueFunc = valueFunc ?? throw new ArgumentNullException(nameof(valueFunc));
    }

    /// <summary></summary>
    public override bool TryGetValue(Key key, out DstValue dstValue)
    {
        // Try get
        if (!source.TryGetValue(key, out SrcValue srcValue)) { dstValue = default!; return false; }
        // Apply func on value
        dstValue = valueFunc(srcValue);
        return true;
    }
}
