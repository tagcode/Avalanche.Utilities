// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

// <docs>
/// <summary>Graph cloner context</summary>
public interface IGraphClonerContext
{
    /// <summary>Associate <paramref name="clone"/> as clone of <paramref name="src"/>.</summary>
    /// <returns>true if was added, false if has already been added.</returns>
    bool Add<T>(in T src, in T clone);

    /// <summary>Test whether <paramref name="src"/> has been marked cloned.</summary>
    /// <returns>true if <paramref name="src"/> is cloned</returns>
    bool Contains<T>(in T src);

    /// <summary>Get cloned counterpart of <paramref name="src"/>.</summary>
    /// <returns>true if <paramref name="clone"/> existed.</returns>
    bool TryGet<T>(in T src, out T clone);
}
// </docs>
