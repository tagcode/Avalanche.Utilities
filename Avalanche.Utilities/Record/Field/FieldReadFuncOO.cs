// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Avalanche.Utilities.Provider;

/// <summary>Provides <![CDATA[Func<R, F>]]>.</summary>
public static class FieldReadFuncOO
{
    /// <summary><![CDATA[Func<object, object>]]> provider</summary>
    static readonly IProvider<IFieldDescription, Func<object, object>> create = Providers.Func<IFieldDescription, Func<object, object>>(TryCreateFieldReadFuncOO);
    /// <summary><![CDATA[Func<object, object>]]> provider</summary>
    static readonly IProvider<IFieldDescription, IResult<Func<object, object>>> createResult = create.ResultCaptured();
    /// <summary><![CDATA[Func<object, object>]]> provider</summary>
    static readonly IProvider<IFieldDescription, IResult<Func<object, object>>> cachedResult = createResult.WeakCached();
    /// <summary><![CDATA[Func<object, object>]]> provider</summary>
    static readonly IProvider<IFieldDescription, Func<object, object>> cached = cachedResult.ResultOpened();

    /// <summary><![CDATA[Func<object, object>]]> provider</summary>
    static readonly IProvider<object, Func<object, object>> createFromObject = FieldDescription.Create.Concat(create);
    /// <summary><![CDATA[Func<object, object>]]> provider</summary>
    static readonly IProvider<object, IResult<Func<object, object>>> createResultFromObject = FieldDescription.CreateResult.Concat(createResult);
    /// <summary><![CDATA[Func<object, object>]]> provider</summary>
    static readonly IProvider<object, IResult<Func<object, object>>> cachedResultFromObject = FieldDescription.CachedResult.Concat(cachedResult);
    /// <summary><![CDATA[Func<object, object>]]> provider</summary>
    static readonly IProvider<object, Func<object, object>> cachedFromObject = FieldDescription.Cached.Concat(cached);

    /// <summary><![CDATA[Func<object, object>]]> provider</summary>
    public static IProvider<IFieldDescription, Func<object, object>> Create => create;
    /// <summary><![CDATA[Func<object, object>]]> provider</summary>
    public static IProvider<IFieldDescription, IResult<Func<object, object>>> CreateResult => createResult;
    /// <summary><![CDATA[Func<object, object>]]> provider</summary>
    public static IProvider<IFieldDescription, IResult<Func<object, object>>> CachedResult => cachedResult;
    /// <summary><![CDATA[Func<object, object>]]> provider</summary>
    public static IProvider<IFieldDescription, Func<object, object>> Cached => cached;

    /// <summary><![CDATA[Func<object, object>]]> provider</summary>
    public static IProvider<object, Func<object, object>> CreateFromObject => createFromObject;
    /// <summary><![CDATA[Func<object, object>]]> provider</summary>
    public static IProvider<object, IResult<Func<object, object>>> CreateResultFromObject => createResultFromObject;
    /// <summary><![CDATA[Func<object, object>]]> provider</summary>
    public static IProvider<object, IResult<Func<object, object>>> CachedResultFromObject => cachedResultFromObject;
    /// <summary><![CDATA[Func<object, object>]]> provider</summary>
    public static IProvider<object, Func<object, object>> CachedFromObject => cachedFromObject;

    /// <summary>Create <![CDATA[Func<object, object>]]> delegate</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static bool TryCreateFieldReadFuncOO(this IFieldDescription field, [NotNullWhen(true)] out Func<object, object> @delegate)
    {
        // Create LambdaExpression
        if (!FieldReadFunc.TryCreateFieldReadFuncExpression(field, out LambdaExpression? expression, typeof(object), typeof(object))) { @delegate = null!; return false; }
        // Compile
        @delegate = (Func<object, object>)expression.Compile();
        // Return
        return true;
    }
}

