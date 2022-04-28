// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Threading;

// <docs>
/// <summary>Clones object with cyclic object graph references.</summary>
public interface IGraphCloner
{
    /// <summary>Thread local for context while visiting non-graph cloners.</summary>
    static ThreadLocal<IGraphClonerContext?> context = new();
    /// <summary>Thread local for context while visiting non-graph cloners.</summary>
    public static ThreadLocal<IGraphClonerContext?> Context => context;

    /// <summary>Can value be cyclic</summary>
    bool IsCyclical { get; }

    /// <summary>Clone <paramref name="src"/></summary>
    /// <param name="src"></param>
    /// <param name="context">Object graph reference mapping.</param>
    object Clone(object src, IGraphClonerContext context);
}
// </docs>

// <docsT>
/// <summary>Clones object with cyclic object graph references.</summary>
public interface IGraphCloner<T>
{
    /// <summary>Can value be cyclic</summary>
    bool IsCyclical { get; }

    /// <summary>Clone <paramref name="src"/></summary>
    /// <param name="src"></param>
    /// <param name="context">Object graph reference mapping.</param>
    T Clone(in T src, IGraphClonerContext context);
}
// </docsT>
