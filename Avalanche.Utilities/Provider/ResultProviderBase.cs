// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Provider;

/// <summary>Base class for providers</summary>
public abstract class ResultProviderBase<TKey, TValue> : IProvider<TKey, IResult<TValue>>
{
    /// <summary></summary>
    public Type Key => typeof(TKey);
    /// <summary></summary>
    public Type Value => typeof(IResult<TValue>);

    /// <summary>Get value</summary>
    /// <exception cref="Exception">Any expception is captured and turned into <see cref="ResultStatus.Error"/> result.</exception>
    protected abstract bool TryGetValue(TKey key, out TValue value);

    /// <summary>Key to value indexer</summary>
    public IResult<TValue> this[TKey key]
    {
        get
        {
            try
            {
                // Return ok
                if (TryGetValue(key, out TValue value)) return new ResultOk<TValue>(value) /*{ Request = key }*/; // key is strong cyclic reference
                // Return no result
                else return new NoResult<TValue>() /*{ Request = key }*/;
            }
            catch (Exception e)
            {
                // Return no result
                return new ResultError<TValue>(e) /*{ Request = key }*/;
            }
        }
    }

    /// <summary>Try get value</summary>
    public bool TryGetValue(TKey key, out IResult<TValue> value)
    {
        try
        {
            // Return ok
            if (TryGetValue(key, out TValue v)) value = new ResultOk<TValue>(v) { Request = key };
            // Return no result
            else value = new NoResult<TValue>() /*{ Request = key }*/;
        }
        catch (Exception e)
        {
            // Return no result
            value = new ResultError<TValue>(e) /*{ Request = key }*/;
        }
        // 
        return true;
    }

    /// <summary>Try get value</summary>
    /// <exception cref="InvalidCastException">If key is not <typeparamref name="TKey"/>.</exception>
    bool IProvider.TryGetValue(object key, out object value)
    {
        try
        {
            // Return ok
            if (TryGetValue((TKey)key, out TValue v)) value = new ResultOk<TValue>(v) { Request = key };
            // Return no result
            else value = new NoResult<TValue>() /*{ Request = key }*/;
        }
        catch (Exception e)
        {
            // Return no result
            value = new ResultError<TValue>(e) /*{ Request = key }*/;
        }
        // 
        return true;
    }
}
