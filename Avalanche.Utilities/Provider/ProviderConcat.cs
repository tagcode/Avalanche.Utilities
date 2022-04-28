// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Avalanche.Utilities.Provider;

/// <summary>Concats two <see cref="IResult"/> based providers.</summary>
public static class ProviderConcat
{
    /// <summary></summary>
    public static readonly ConstructorT3<IProvider, IProvider, IProvider> Constructor = new(typeof(ProviderConcat<,,>));
    /// <summary></summary>
    public static IProvider Create(IProvider providerAB, IProvider providerBC) => Constructor.Create(providerAB.Key, providerBC.Key, providerBC.Value, providerAB, providerBC);
}

/// <summary>Concats two <see cref="IResult"/> based providers.</summary>
public class ProviderConcat<A, B, C> : ProviderBase<A, C>
{
    /// <summary></summary>
    IProvider<A, B> providerAB;
    /// <summary></summary>
    IProvider<B, C> providerBC;

    /// <summary></summary>
    public ProviderConcat(IProvider<A, B> providerAB, IProvider<B, C> providerBC)
    {
        this.providerAB = providerAB ?? throw new ArgumentNullException(nameof(providerAB));
        this.providerBC = providerBC ?? throw new ArgumentNullException(nameof(providerBC));
    }

    /// <summary></summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public override bool TryGetValue(A key, out C value)
    {
        // Get result
        if (!providerAB.TryGetValue(key, out B b)) { value = default!; return false; }
        // Get result
        if (!providerBC.TryGetValue(b, out C c)) { value = default!; return false; }
        // Ok
        value = c;
        return true;
    }

    /// <summary>Print information</summary>
    public override string ToString() => $"{providerAB}.Concat({providerBC})";
}
