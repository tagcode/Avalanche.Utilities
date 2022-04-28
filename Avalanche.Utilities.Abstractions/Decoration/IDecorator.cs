// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Diagnostics.CodeAnalysis;

// <docs>
/// <summary>Decorates <typeparamref name="T"/> instances.</summary>
public interface IDecorator<T>
{
    /// <summary>Decorate <paramref name="object"/>.</summary>
    bool TryDecorate(T @object, [NotNullWhen(false)] out T? decoree);
}
// </docs>
