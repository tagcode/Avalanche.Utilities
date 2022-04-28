// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;

/// <summary>Cloner that passes value or reference as is.</summary>
public class PassthroughCloner : ICloner, IGraphCloner
{
    /// <summary></summary>
    static readonly ConstructorT<PassthroughCloner> constructor = new(typeof(PassthroughCloner<>));
    /// <summary></summary>
    public static PassthroughCloner Create(Type elementType) => constructor.Create(elementType);
    /// <summary>Element type</summary>
    public virtual Type ElementType => null!;
    /// <summary>Is cyclic value</summary>
    public bool IsCyclical => false;
    /// <summary></summary>
    public virtual object Clone(object src, IGraphClonerContext context) => src;
    /// <summary></summary>
    public virtual object Clone(object src) => src;
}

/// <summary>Cloner that passes value or reference as is.</summary>
public class PassthroughCloner<T> : PassthroughCloner, ICloner<T>, IGraphCloner<T>
{
    /// <summary></summary>
    static PassthroughCloner<T> instance = new PassthroughCloner<T>();
    /// <summary></summary>
    public static PassthroughCloner<T> Instance => instance;
    /// <summary>Element type</summary>
    public override Type ElementType => typeof(T);
    /// <summary></summary>
    public virtual T Clone(in T src, IGraphClonerContext context) => src;
    /// <summary></summary>
    public virtual T Clone(in T src) => src;
}

