// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Avalanche.Utilities.Provider;

/// <summary>Provides field writers</summary>
public static class FieldWriteActionOO
{
    /// <summary><![CDATA[Action<object, object>]]> provider</summary>
    static readonly IProvider<IFieldDescription, Action<object, object>> create = Providers.Func<IFieldDescription, Action<object, object>>(TryCreateFieldWriteActionOO);
    /// <summary><![CDATA[Action<object, object>]]> provider</summary>
    static readonly IProvider<IFieldDescription, IResult<Action<object, object>>> createResult = create.ResultCaptured();
    /// <summary><![CDATA[Action<object, object>]]> provider</summary>
    static readonly IProvider<IFieldDescription, IResult<Action<object, object>>> cachedResult = createResult.WeakCached();
    /// <summary><![CDATA[Action<object, object>]]> provider</summary>
    static readonly IProvider<IFieldDescription, Action<object, object>> cached = cachedResult.ResultOpened();

    /// <summary><![CDATA[Action<object, object>]]> provider</summary>
    static readonly IProvider<object, Action<object, object>> createFromObject = FieldDescription.Create.Concat(create);
    /// <summary><![CDATA[Action<object, object>]]> provider</summary>
    static readonly IProvider<object, IResult<Action<object, object>>> createResultFromObject = FieldDescription.CreateResult.Concat(createResult);
    /// <summary><![CDATA[Action<object, object>]]> provider</summary>
    static readonly IProvider<object, IResult<Action<object, object>>> cachedResultFromObject = FieldDescription.CachedResult.Concat(cachedResult);
    /// <summary><![CDATA[Action<object, object>]]> provider</summary>
    static readonly IProvider<object, Action<object, object>> cachedFromObject = FieldDescription.Cached.Concat(cached);

    /// <summary><![CDATA[Action<object, object>]]> provider</summary>
    public static IProvider<IFieldDescription, Action<object, object>> Create => create;
    /// <summary><![CDATA[Action<object, object>]]> provider</summary>
    public static IProvider<IFieldDescription, IResult<Action<object, object>>> CreateResult => createResult;
    /// <summary><![CDATA[Action<object, object>]]> provider</summary>
    public static IProvider<IFieldDescription, IResult<Action<object, object>>> CachedResult => cachedResult;
    /// <summary><![CDATA[Action<object, object>]]> provider</summary>
    public static IProvider<IFieldDescription, Action<object, object>> Cached => cached;

    /// <summary><![CDATA[Action<object, object>]]> provider</summary>
    public static IProvider<object, Action<object, object>> CreateFromObject => createFromObject;
    /// <summary><![CDATA[Action<object, object>]]> provider</summary>
    public static IProvider<object, IResult<Action<object, object>>> CreateResultFromObject => createResultFromObject;
    /// <summary><![CDATA[Action<object, object>]]> provider</summary>
    public static IProvider<object, IResult<Action<object, object>>> CachedResultFromObject => cachedResultFromObject;
    /// <summary><![CDATA[Action<object, object>]]> provider</summary>
    public static IProvider<object, Action<object, object>> CachedFromObject => cachedFromObject;

    /// <summary>Create <![CDATA[Action<object, object>]]> delegate</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static bool TryCreateFieldWriteActionOO(this IFieldDescription field, [NotNullWhen(true)] out Action<object, object> @delegate)
    {
        // Create LambdaExpression
        if (!FieldWriteAction.TryCreateFieldWriteExpressionAction(field, out LambdaExpression? expression, typeof(object), typeof(object))) { @delegate = null!; return false; }
        // Compile
        @delegate = (Action<object, object>)expression.Compile();
        // Return
        return true;
    }
}
