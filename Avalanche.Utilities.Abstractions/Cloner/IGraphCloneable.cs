// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

// <docs>
/// <summary>Object cloneable with graph awareness.</summary>
public interface IGraphCloneable
{
    // <summary>Can value be cyclic</summary>
    //bool IsCyclical { get; }

    /// <summary>Clone </summary>
    /// <param name="context">Object graph reference mapping.</param>
    object Clone(IGraphClonerContext context);
}
// </docs>
