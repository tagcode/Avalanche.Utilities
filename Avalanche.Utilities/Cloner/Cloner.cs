// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;

/// <summary>Forwards clone to <see cref="ICloneable"/> and <see cref="IGraphCloneable"/></summary>
public abstract class Cloner : ReadOnlyAssignableClass, ICloner, IGraphCloner, ICyclical
{
    /// <summary></summary>
    static readonly ConstructorT<Cloner> constructor = new(typeof(Cloner<>));
    /// <summary></summary>
    /// <returns><![CDATA[RecordCloner<T>]]></returns>
    public static Cloner Create(Type recordType) => constructor.Create(recordType);

    /// <summary>Singleton</summary>
    public static Cloner<object> Instance => Cloner<object>.Instance;

    /// <summary></summary>
    protected bool isCyclical;
    /// <summary></summary>
    public bool IsCyclical { get => isCyclical; set => this.AssertWritable().isCyclical = value; }

    /// <summary>The implementation type</summary>
    public abstract Type Type { get; }

    /// <summary></summary>
    public virtual object Clone(object src)
    {
        // Got null
        if (src == null) return default!;
        // Move to cyclical 
        if (isCyclical)
        {
            // Get previous context
            IGraphClonerContext? prevContext = IGraphCloner.Context.Value;
            // Place here context
            IGraphClonerContext context = prevContext ?? setContext(new GraphClonerContext())!;
            try
            {
                return Clone(src, context);
            }
            finally
            {
                // Revert to previous context
                IGraphCloner.Context.Value = prevContext;
            }
        }
        // Clone
        if (src is ICloneable cloneable) return cloneable.Clone();
        // Clone
        if (src is IGraphCloneable graphCloneable)
        {
            // Get previous context
            IGraphClonerContext? prevContext = IGraphCloner.Context.Value;
            // Place here context
            IGraphClonerContext context = prevContext ?? setContext(new GraphClonerContext())!;
            try
            {
                return graphCloneable.Clone(context);
            }
            finally
            {
                // Revert to previous context
                IGraphCloner.Context.Value = prevContext;
            }
        }

        // Return
        throw new InvalidOperationException($"Not {nameof(ICloneable)} or {nameof(IGraphCloneable)}.");
    }

    /// <summary></summary>
    public virtual object Clone(object src, IGraphClonerContext context)
    {
        // Got null
        if (src == null) return default!;
        // Exists in context
        if (context.TryGet(src, out object? _dst)) return _dst!;

        // Graph clone
        if (src is IGraphCloneable graphClonable) return graphClonable.Clone(context);
        // Transition to regular clone
        else if (src is ICloneable cloneable)
        {
            // Get previous context
            IGraphClonerContext? prevContext = IGraphCloner.Context.Value;
            // Assign context
            IGraphCloner.Context.Value = context;
            try
            {
                object clone = cloneable.Clone();
                context.Add(this, clone);
                return clone;
            }
            finally
            {
                // Revert to previous context
                IGraphCloner.Context.Value = prevContext;
            }
        }
        // Error
        throw new InvalidOperationException($"Not {nameof(ICloneable)} or {nameof(IGraphCloneable)}.");

    }

    /// <summary>Assign <paramref name="context"/> to <see cref="IGraphCloner.Context"/> and return it.</summary>
    /// <returns><paramref name="context"/></returns>
    protected IGraphClonerContext? setContext(IGraphClonerContext? context) { IGraphCloner.Context.Value = context; return context; }
}

/// <summary>Forwards clone to <see cref="ICloneable"/> and <see cref="IGraphCloneable"/></summary>
/// <typeparam name="T"></typeparam>
public class Cloner<T> : Cloner, ICloner<T>, IGraphCloner<T>
{
    /// <summary>Singleton</summary>
    static Cloner<T> instance = new Cloner<T>();
    /// <summary>Singleton</summary>
    public new static Cloner<T> Instance => instance;

    /// <summary></summary>
    public override Type Type => typeof(T);

    /// <summary></summary>
    public virtual T Clone(in T src)
    {
        // Got null
        if (src == null) return default!;
        // Move to cyclical 
        if (isCyclical)
        {
            // Get previous context
            IGraphClonerContext? prevContext = IGraphCloner.Context.Value;
            // Place here context
            IGraphClonerContext context = prevContext ?? setContext(new GraphClonerContext())!;
            try
            {
                return Clone(src, context);
            }
            finally
            {
                // Revert to previous context
                IGraphCloner.Context.Value = prevContext;
            }
        }
        // Clone
        if (src is ICloneable cloneable) return (T)cloneable.Clone();
        // Clone
        if (src is IGraphCloneable graphCloneable)
        {
            // Get previous context
            IGraphClonerContext? prevContext = IGraphCloner.Context.Value;
            // Place here context
            IGraphClonerContext context = prevContext ?? setContext(new GraphClonerContext())!;
            try
            {
                return (T)graphCloneable.Clone(context);
            }
            finally
            {
                // Revert to previous context
                IGraphCloner.Context.Value = prevContext;
            }
        }

        // Return
        throw new InvalidOperationException($"Not {nameof(ICloneable)} or {nameof(IGraphCloneable)}.");
    }

    /// <summary></summary>
    public virtual T Clone(in T src, IGraphClonerContext context)
    {
        // Got null
        if (src == null) return default!;
        // Exists in context
        if (context.TryGet(src, out T? _dst)) return (T)_dst!;
        // Graph clone
        if (src is IGraphCloneable graphClonable) return (T)graphClonable.Clone(context);
        // Transition to regular clone
        else if (src is ICloneable cloneable)
        {
            // Get previous context
            IGraphClonerContext? prevContext = IGraphCloner.Context.Value;
            // Assign context
            IGraphCloner.Context.Value = context;
            try
            {
                T clone = (T)cloneable.Clone();
                context.Add(src, clone);
                return clone;
            }
            finally
            {
                // Revert to previous context
                IGraphCloner.Context.Value = prevContext;
            }
        }
        // Error
        throw new InvalidOperationException($"Not {nameof(ICloneable)} or {nameof(IGraphCloneable)}.");
    }
}
