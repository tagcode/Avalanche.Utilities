// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Avalanche.Utilities.Provider;

/// <summary>Creates <![CDATA[Action<object, object>]]> that copies every field from one record to another.</summary>
public static class RecordCopyActionOO
{
    /// <summary><see cref="Action{Record, Record}"/> provider</summary>
    static readonly IProvider<IRecordDescription, Action<object, object>> create = Providers.Func<IRecordDescription, Action<object, object>>(TryCreateRecordCopyActionOO);
    /// <summary><see cref="Action{Record, Record}"/> provider</summary>
    static readonly IProvider<IRecordDescription, IResult<Action<object, object>>> createResult = create.ResultCaptured();
    /// <summary><see cref="Action{Record, Record}"/> provider</summary>
    static readonly IProvider<IRecordDescription, IResult<Action<object, object>>> cachedResult = createResult.WeakCached();
    /// <summary><see cref="Action{Record, Record}"/> provider</summary>
    static readonly IProvider<IRecordDescription, Action<object, object>> cached = cachedResult.ResultOpened();
    /// <summary>Creates from type.</summary>
    static readonly IProvider<Type, Action<object, object>> createFromType = RecordDescription.CreateResult.Concat(createResult).ResultOpened();
    /// <summary>Creates and caches from type.</summary>
    static readonly IProvider<Type, Action<object, object>> cachedFromType = RecordDescription.CachedResult.Concat(cachedResult).ResultOpened();

    /// <summary><see cref="Action{Record, Record}"/> provider</summary>
    public static IProvider<IRecordDescription, Action<object, object>> Create => create;
    /// <summary><see cref="Action{Record, Record}"/> provider</summary>
    public static IProvider<IRecordDescription, IResult<Action<object, object>>> CreateResult => createResult;
    /// <summary><see cref="Action{Record, Record}"/> provider</summary>
    public static IProvider<IRecordDescription, IResult<Action<object, object>>> CachedResult => cachedResult;
    /// <summary><see cref="Action{Record, Record}"/> provider</summary>
    public static IProvider<IRecordDescription, Action<object, object>> Cached => cached;
    /// <summary>Creates from type.</summary>
    public static IProvider<Type, Action<object, object>> CreateFromType => createFromType;
    /// <summary>Creates and caches from type.</summary>
    public static IProvider<Type, Action<object, object>> CachedFromType => cachedFromType;

    /// <summary>Create <see cref="Action{Record, Record}"/> delegate</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static bool TryCreateRecordCopyActionOO(this IRecordDescription record, [NotNullWhen(true)] out Action<object, object> @delegate)
    {
        // Create LambdaExpression
        if (!RecordCopyAction.TryCreateRecordCopyExpression(record, out LambdaExpression? expression, typeof(object))) { @delegate = null!; return false; }
        // Compile
        @delegate = (Action<object, object>)expression.Compile();
        // Return
        return true;
    }

}
