// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider;

/// <summary></summary>
public class IndirectProvider<Key, Value> : ProviderBase<Key, Value>
{
    /// <summary>Function that provides provider</summary>
    protected Func<IProvider<Key, Value>> providerFunc;
    /// <summary>Function that provides provider</summary>
    public virtual Func<IProvider<Key, Value>> ProviderFunc => providerFunc;

    /// <summary></summary>
    public IndirectProvider(Func<IProvider<Key, Value>> providerFunc)
    {
        this.providerFunc = providerFunc ?? throw new ArgumentNullException(nameof(providerFunc));
    }

    /// <summary></summary>
    public override bool TryGetValue(Key key, out Value value)
    {
        // Get provider
        var _provider = ProviderFunc();
        // Try get value
        if (_provider == null || !_provider.TryGetValue(key, out value)) { value = default!; return false; }
        // Done
        return true;
    }
}
