// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Reflection;
using Avalanche.Utilities.Provider;

/// <summary>Provides the default <see cref="Comparer{T}"/> for a run-time type. Uses <see cref="IComparable{T}"/> implementation of type.</summary>
/// <remarks>Example: <![CDATA[IComparer<bool> comparer = (IComparer<bool>)ComparerProvider.Create[typeof(bool)].AssertValue()]]></remarks>
public class ComparerProvider : ProviderBase<Type, object>
{
    /// <summary></summary>
    static readonly IProvider<Type, object> create = new ComparerProvider();
    /// <summary></summary>
    static Lazy<IProvider<Type, IResult<object>>> cached = new(() => create.ResultCaptured().Cached());
    /// <summary></summary>
    public static IProvider<Type, object> Create => create;
    /// <summary></summary>
    public static IProvider<Type, IResult<object>> Cached => cached.Value;

    /// <summary>Get <see cref="Comparer{T}"/> for <paramref name="type"/>.</summary>
    /// <returns><see cref="IComparer{T}"/></returns>
    public static object Get(Type type) => Cached[type].AssertValue(type);

    /// <summary></summary>
    public override bool TryGetValue(Type type, out object value)
    {
        // Create getter
        MethodInfo getter = typeof(Comparer<>).MakeGenericType(type).GetProperty("Default")!.GetMethod!;
        // Read "Default"
        value = getter.Invoke(null, null)!;
        // Ok
        return true;
    }
}
