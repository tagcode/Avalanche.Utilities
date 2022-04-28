// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Avalanche.Utilities.Provider;

/// <summary>Creates <![CDATA[Func<object, object>]]> that makes shallow, non-cyclic delegate cloners of records.</summary>
public static class RecordCloneFuncOO
{
    /// <summary><![CDATA[Func<object, object>]]> provider</summary>
    static readonly IProvider<IRecordDescription, Func<object, object>> create = Providers.Func<IRecordDescription, Func<object, object>>(TryCreateRecordCloneFuncOO);
    /// <summary><![CDATA[Func<object, object>]]> provider</summary>
    static readonly IProvider<IRecordDescription, IResult<Func<object, object>>> createResult = create.ResultCaptured();
    /// <summary><![CDATA[Func<object, object>]]> provider</summary>
    static readonly IProvider<IRecordDescription, IResult<Func<object, object>>> cachedResult = createResult.WeakCached();
    /// <summary><![CDATA[Func<object, object>]]> provider</summary>
    static readonly IProvider<IRecordDescription, Func<object, object>> cached = cachedResult.ResultOpened();
    /// <summary>Creates <![CDATA[Func<object[], Record>]]>.</summary>
    static readonly IProvider<Type, Func<object, object>> createFromType = RecordDescription.CreateResult.Concat(createResult).ResultOpened();
    /// <summary>Creates and caches <![CDATA[Func<object[], Record>]]>.</summary>
    static readonly IProvider<Type, Func<object, object>> cachedFromType = RecordDescription.CachedResult.Concat(cachedResult).ResultOpened();

    /// <summary><![CDATA[Func<object, object>]]> provider</summary>
    public static IProvider<IRecordDescription, Func<object, object>> Create => create;
    /// <summary><![CDATA[Func<object, object>]]> provider</summary>
    public static IProvider<IRecordDescription, IResult<Func<object, object>>> CreateResult => createResult;
    /// <summary><![CDATA[Func<object, object>]]> provider</summary>
    public static IProvider<IRecordDescription, IResult<Func<object, object>>> CachedResult => cachedResult;
    /// <summary><![CDATA[Func<object, object>]]> provider</summary>
    public static IProvider<IRecordDescription, Func<object, object>> Cached => cached;
    /// <summary>Creates and caches from type.</summary>
    public static IProvider<Type, Func<object, object>> CreateFromType => createFromType;
    /// <summary>Creates and caches from type.</summary>
    public static IProvider<Type, Func<object, object>> CachedFromType => cachedFromType;

    /// <summary>Create <![CDATA[Func<object, object>]]> delegate</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static bool TryCreateRecordCloneFuncOO(this IRecordDescription record, [NotNullWhen(true)] out Func<object, object> @delegate)
    {
        //
        IConstructionDescription? constructionDescription = record?.Construction as IConstructionDescription;
        // No construction description
        if (constructionDescription == null) { @delegate = null!; return false; }
        // Create LambdaExpression
        if (!RecordCloneFunc.TryCreateRecordCloneExpression(constructionDescription, out LambdaExpression? expression, typeof(object), typeof(object))) { @delegate = null!; return false; }
        // Compile
        @delegate = (Func<object, object>)expression.Compile();
        // Return
        return true;
    }
}
