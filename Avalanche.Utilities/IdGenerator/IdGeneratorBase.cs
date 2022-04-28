// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

/// <summary>Base class for identity providers</summary>
public abstract class IdGeneratorBase<T> : IIdGenerator<T>
{
    /// <summary>Uniqueness level</summary>
    protected IIdGenerator.Scope uniqueness;

    /// <summary>Uniqueness level</summary>
    public virtual IIdGenerator.Scope Uniqueness => uniqueness;
    /// <summary>Identity type</summary>
    public virtual Type Type => typeof(int);
    /// <summary>Test if there are more Ids</summary>
    public virtual bool HasMore { get; }
    /// <summary>Get next identity</summary>
    object IIdGenerator.Next => TryGetNext(out T value) ? (object)value! : throw new InvalidOperationException($"{GetType().Name}: No more identities");
    /// <summary>Get next identity</summary>
    public virtual T Next => TryGetNext(out T value) ? value : throw new InvalidOperationException($"{GetType().Name}: No more identities");

    /// <summary>Create provider</summary>
    public IdGeneratorBase(IIdGenerator.Scope uniqueness)
    {
        this.uniqueness = uniqueness;
    }

    /// <summary>Try create next identity</summary>
    public abstract bool TryGetNext(out T value);
    /// <summary>Try create next identity</summary>
    public virtual bool TryGetNext(out object value)
    {
        if (TryGetNext(out T _value)) { value = _value!; return true; }
        value = null!;
        return false;
    }
}
