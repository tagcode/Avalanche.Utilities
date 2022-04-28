// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Reflection;
using Avalanche.Utilities.Provider;

/// <summary>Provides the default <see cref="EqualityComparer{T}"/> for run-time type.</summary>
/// <remarks>Example: <![CDATA[IEqualityComparer<bool> comparer = (IEqualityComparer<bool>)EqualityComparerProvider.Create[typeof(bool)]]]></remarks>
public class EqualityComparerProvider : ProviderBase<Type, object>
{
    /// <summary></summary>
    static readonly IProvider<Type, object> create = new EqualityComparerProvider();
    /// <summary></summary>
    static Lazy<IProvider<Type, IResult<object>>> cached = new(() => create.ResultCaptured().Cached());
    /// <summary></summary>
    public static IProvider<Type, object> Create => create;
    /// <summary></summary>
    public static IProvider<Type, IResult<object>> Cached => cached.Value;

    /// <summary>Get <see cref="EqualityComparer{T}"/> for <paramref name="type"/>.</summary>
    /// <returns><see cref="IEqualityComparer{T}"/></returns>
    public static object Get(Type type) => Cached[type].AssertValue(type);

    /// <summary></summary>
    public override bool TryGetValue(Type type, out object value)
    {
        // Create getter
        MethodInfo getter = typeof(EqualityComparer<>).MakeGenericType(type).GetProperty("Default")!.GetMethod!;
        // Read "Default"
        value = getter.Invoke(null, null)!;
        // Ok
        return true;
    }
}
