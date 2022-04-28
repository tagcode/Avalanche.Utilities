// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Avalanche.Utilities.Provider;

/// <summary>Creates <![CDATA[Func<object[], object>]]> delegates that instantiate record-like classes and structs.</summary>
public static class RecordCreateFuncOO
{
    /// <summary>Creates <![CDATA[Func<object[], object>]]>.</summary>
    static readonly IProvider<IRecordDescription, Func<object[], object>> create = Providers.Func<IRecordDescription, Func<object[], object>>(TryCreateCreateFuncOO);
    /// <summary>Creates <![CDATA[IResult<Func<object[], object>>]]>.</summary>
    static readonly IProvider<IRecordDescription, IResult<Func<object[], object>>> createResult = create.ResultCaptured();
    /// <summary>Creates and caches <![CDATA[IResult<Func<object[], object>>]]>.</summary>
    static readonly IProvider<IRecordDescription, IResult<Func<object[], object>>> cachedResult = createResult.WeakCached();
    /// <summary>Creates and caches <![CDATA[Func<object[], object>]]>.</summary>
    static readonly IProvider<IRecordDescription, Func<object[], object>> cached = cachedResult.ResultOpened();
    /// <summary>Creates <![CDATA[Func<object[], object>]]>.</summary>
    static readonly IProvider<Type, Func<object[], object>> createFromType = RecordDescription.CreateResult.Concat(createResult).ResultOpened();
    /// <summary>Creates and caches <![CDATA[Func<object[], object>]]>.</summary>
    static readonly IProvider<Type, Func<object[], object>> cachedFromType = RecordDescription.CachedResult.Concat(cachedResult).ResultOpened();

    /// <summary>Creates <![CDATA[Func<object[], object>]]>.</summary>
    public static IProvider<IRecordDescription, Func<object[], object>> Create => create;
    /// <summary>Creates <![CDATA[IResult<Func<object[], object>>]]>.</summary>
    public static IProvider<IRecordDescription, IResult<Func<object[], object>>> CreateResult => createResult;
    /// <summary>Creates and caches <![CDATA[IResult<Func<object[], object>>]]>.</summary>
    public static IProvider<IRecordDescription, IResult<Func<object[], object>>> CachedResult => cachedResult;
    /// <summary>Creates and caches <![CDATA[Func<object[], object>]]>.</summary>
    public static IProvider<IRecordDescription, Func<object[], object>> Cached => cached;
    /// <summary>Creates <![CDATA[Func<object[], object>]]>.</summary>
    public static IProvider<Type, Func<object[], object>> CreateFromType => createFromType;
    /// <summary>Creates and caches <![CDATA[Func<object[], object>]]>.</summary>
    public static IProvider<Type, Func<object[], object>> CachedFromType => cachedFromType;

    /// <summary>Create <![CDATA[Func<object[], object>]]> delegate</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static bool TryCreateCreateFuncOO(this IRecordDescription? recordDescription, [NotNullWhen(true)] out Func<object[], object> @delegate)
    {
        // Get
        IConstructionDescription? constructionDescription = recordDescription?.Construction as IConstructionDescription;
        // 
        if (constructionDescription == null) { @delegate = null!; return false; }
        // Create LambdaExpression
        if (!constructionDescription.TryCreateCreateExpression(out LambdaExpression? expression, typeof(object))) { @delegate = null!; return false; }
        // Compile
        @delegate = (Func<object[], object>)expression.Compile();
        //
        return true;
    }
}
