// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Avalanche.Utilities.Provider;

/// <summary>Creates <![CDATA[Func<object, object, object>]]>.</summary>
public static class RecreateWithFuncOOO
{
    /// <summary><![CDATA[Func<object, object, object>]]> provider</summary>
    static readonly IProvider<IFieldDescription, Func<object, object, object>> create = Providers.Func<IFieldDescription, Func<object, object, object>>(TryCreateRecreateWithFuncOOO);
    /// <summary><![CDATA[Func<object, object, object>]]> provider</summary>
    static readonly IProvider<IFieldDescription, IResult<Func<object, object, object>>> createResult = create.ResultCaptured();
    /// <summary><![CDATA[Func<object, object, object>]]> provider</summary>
    static readonly IProvider<IFieldDescription, IResult<Func<object, object, object>>> cachedResult = createResult.WeakCached();
    /// <summary><![CDATA[Func<object, object, object>]]> provider</summary>
    static readonly IProvider<IFieldDescription, Func<object, object, object>> cached = cachedResult.ResultOpened();

    /// <summary><![CDATA[Func<object, object, object>]]> provider</summary>
    static readonly IProvider<object, Func<object, object, object>> createFromObject = FieldDescription.CreateWithRecord.Concat(create);
    /// <summary><![CDATA[Func<object, object, object>]]> provider</summary>
    static readonly IProvider<object, IResult<Func<object, object, object>>> createResultFromObject = FieldDescription.CreateResultWithRecord.Concat(createResult);
    /// <summary><![CDATA[Func<object, object, object>]]> provider</summary>
    static readonly IProvider<object, IResult<Func<object, object, object>>> cachedResultFromObject = FieldDescription.CachedResultWithRecord.Concat(cachedResult);
    /// <summary><![CDATA[Func<object, object, object>]]> provider</summary>
    static readonly IProvider<object, Func<object, object, object>> cachedFromObject = FieldDescription.CachedWithRecord.Concat(cached);

    /// <summary><![CDATA[Func<object, object, object>]]> provider</summary>
    public static IProvider<IFieldDescription, Func<object, object, object>> Create => create;
    /// <summary><![CDATA[Func<object, object, object>]]> provider</summary>
    public static IProvider<IFieldDescription, IResult<Func<object, object, object>>> CreateResult => createResult;
    /// <summary><![CDATA[Func<object, object, object>]]> provider</summary>
    public static IProvider<IFieldDescription, IResult<Func<object, object, object>>> CachedResult => cachedResult;
    /// <summary><![CDATA[Func<object, object, object>]]> provider</summary>
    public static IProvider<IFieldDescription, Func<object, object, object>> Cached => cached;

    /// <summary><![CDATA[Func<object, object, object>]]> provider</summary>
    public static IProvider<object, Func<object, object, object>> CreateFromObject => createFromObject;
    /// <summary><![CDATA[Func<object, object, object>]]> provider</summary>
    public static IProvider<object, IResult<Func<object, object, object>>> CreateResultFromObject => createResultFromObject;
    /// <summary><![CDATA[Func<object, object, object>]]> provider</summary>
    public static IProvider<object, IResult<Func<object, object, object>>> CachedResultFromObject => cachedResultFromObject;
    /// <summary><![CDATA[Func<object, object, object>]]> provider</summary>
    public static IProvider<object, Func<object, object, object>> CachedFromObject => cachedFromObject;
    
    /// <summary>Create <![CDATA[Func<object, object, object>]]> delegate</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static bool TryCreateRecreateWithFuncOOO(this IFieldDescription field, IConstructionDescription? constructionDescription, [NotNullWhen(true)] out Func<object, object, object>? @delegate)
    {
        // No construction description
        if (constructionDescription == null) { @delegate = null; return false; }
        // Create LambdaExpression
        if (!RecreateWithFunc.TryCreateRecreateWithExpressionFunc(constructionDescription, field, out LambdaExpression? expression, typeof(object), typeof(object), typeof(object))) { @delegate = null; return false; }
        // Compile
        @delegate = (Func<object, object, object>)expression.Compile();
        // Return
        return true;
    }

    /// <summary>Create <![CDATA[Func<object, object, object>]]> delegate</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static bool TryCreateRecreateWithFuncOOO(this IFieldDescription field, [NotNullWhen(true)] out Func<object, object, object> @delegate)
    {
        //
        IConstructionDescription? constructionDescription = field.Record?.Construction as IConstructionDescription;
        // No construction description
        if (constructionDescription == null) { @delegate = null!; return false; }
        // Create LambdaExpression
        if (!RecreateWithFunc.TryCreateRecreateWithExpressionFunc(constructionDescription, field, out LambdaExpression? expression, typeof(object), typeof(object), typeof(object))) { @delegate = null!; return false; }
        // Compile
        @delegate = (Func<object, object, object>)expression.Compile();
        // Return
        return true;
    }


}
