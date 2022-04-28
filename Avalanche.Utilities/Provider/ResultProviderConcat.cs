// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider;

/// <summary>Concats two <see cref="IResult"/> based providers.</summary>
public static class ResultProviderConcat
{
    /// <summary></summary>
    public static readonly ConstructorT3<IProvider, IProvider, IProvider> Constructor = new(typeof(ResultProviderConcat<,,>));
    /// <summary></summary>
    public static IProvider Create(IProvider providerAB, IProvider providerBC) =>
        TypeUtilities.TryGetTypeArgumentOfCorrespondingDefinedType(providerAB.Value, typeof(IResult<>), 0, out Type b) ?
        TypeUtilities.TryGetTypeArgumentOfCorrespondingDefinedType(providerBC.Value, typeof(IResult<>), 0, out Type c) ?
        Constructor.Create(providerAB.Key, b, c, providerAB, providerBC) :
        throw new ArgumentException(nameof(providerAB)) :
        throw new ArgumentException(nameof(providerBC));
}

/// <summary>Concats two <see cref="IResult"/> based providers.</summary>
public class ResultProviderConcat<A, B, C> : ProviderBase<A, IResult<C>>
{
    /// <summary></summary>
    IProvider<A, IResult<B>> providerAB;
    /// <summary></summary>
    IProvider<B, IResult<C>> providerBC;
    /// <summary></summary>
    public ResultProviderConcat(IProvider<A, IResult<B>> providerAB, IProvider<B, IResult<C>> providerBC)
    {
        this.providerAB = providerAB ?? throw new ArgumentNullException(nameof(providerAB));
        this.providerBC = providerBC ?? throw new ArgumentNullException(nameof(providerBC));
    }
    /// <summary></summary>
    public override bool TryGetValue(A key, out IResult<C> value)
    {
        // Get inner result
        if (!providerAB.TryGetValue(key, out IResult<B> b)) { value = new NoResult<C>(); return true; }
        // No result
        if (b.Status == ResultStatus.NoResult || b.Status == ResultStatus.Unassigned) { value = new NoResult<C>(); return true; }
        // Error
        if (b.Status == ResultStatus.Error) { value = new ResultError<C>(b.Error!); return true; }
        // Ok
        if (!providerBC.TryGetValue(b.Value!, out IResult<C> c)) { value = new NoResult<C>(); return true; }
        //
        value = c;
        return true;
    }
}
