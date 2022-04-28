// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Avalanche.Utilities.Provider;

/// <summary>Creates <![CDATA[Func<Record, Record>]]> that makes shallow, non-cyclic delegate cloners of records.</summary>
public static class RecordCloneFunc
{
    /// <summary><![CDATA[Func<Record, Record>]]> provider</summary>
    static readonly IProvider<IRecordDescription, Delegate> create = Providers.Func<IRecordDescription, Delegate>(TryCreateRecordCloneFunc);
    /// <summary><![CDATA[Func<Record, Record>]]> provider</summary>
    static readonly IProvider<IRecordDescription, IResult<Delegate>> createResult = create.ResultCaptured();
    /// <summary><![CDATA[Func<Record, Record>]]> provider</summary>
    static readonly IProvider<IRecordDescription, IResult<Delegate>> cachedResult = createResult.WeakCached();
    /// <summary><![CDATA[Func<Record, Record>]]> provider</summary>
    static readonly IProvider<IRecordDescription, Delegate> cached = cachedResult.ResultOpened();
    /// <summary>Creates <![CDATA[Func<object[], Record>]]>.</summary>
    static readonly IProvider<Type, Delegate> createFromType = RecordDescription.CreateResult.Concat(createResult).ResultOpened();
    /// <summary>Creates and caches <![CDATA[Func<object[], Record>]]>.</summary>
    static readonly IProvider<Type, Delegate> cachedFromType = RecordDescription.CachedResult.Concat(cachedResult).ResultOpened();

    /// <summary><![CDATA[Func<Record, Record>]]> provider</summary>
    public static IProvider<IRecordDescription, Delegate> Create => create;
    /// <summary><![CDATA[Func<Record, Record>]]> provider</summary>
    public static IProvider<IRecordDescription, IResult<Delegate>> CreateResult => createResult;
    /// <summary><![CDATA[Func<Record, Record>]]> provider</summary>
    public static IProvider<IRecordDescription, IResult<Delegate>> CachedResult => cachedResult;
    /// <summary><![CDATA[Func<Record, Record>]]> provider</summary>
    public static IProvider<IRecordDescription, Delegate> Cached => cached;
    /// <summary>Creates from type.</summary>
    public static IProvider<Type, Delegate> CreateFromType => createFromType;
    /// <summary>Creates and caches from type.</summary>
    public static IProvider<Type, Delegate> CachedFromType => cachedFromType;

    /// <summary>Create <![CDATA[Func<Record, Record>]]> delegate</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static bool TryCreateRecordCloneFunc(this IRecordDescription record, [NotNullWhen(true)] out Delegate @delegate)
    {
        //
        IConstructionDescription? constructionDescription = record?.Construction as IConstructionDescription;
        // No construction description
        if (constructionDescription == null) { @delegate = null!; return false; }
        // Create LambdaExpression
        if (!TryCreateRecordCloneExpression(constructionDescription, out LambdaExpression? expression)) { @delegate = null!; return false; }
        // Compile
        @delegate = expression.Compile();
        // Return
        return true;
    }

    /// <summary>Create <![CDATA[Func<Record, Record>]]> delegate</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static bool TryCreateRecordCloneFunc(this IRecordDescription record, [NotNullWhen(true)] out Delegate @delegate, Type? delegateRecordType, Type? delegateReturnType)
    {
        //
        IConstructionDescription? constructionDescription = record?.Construction as IConstructionDescription;
        // No construction description
        if (constructionDescription == null) { @delegate = null!; return false; }
        // Create LambdaExpression
        if (!TryCreateRecordCloneExpression(constructionDescription, out LambdaExpression? expression, delegateRecordType, delegateReturnType)) { @delegate = null!; return false; }
        // Compile
        @delegate = expression.Compile();
        // Return
        return true;
    }

    /// <summary>Create <![CDATA[Func<Record, Record>]]> expression</summary>
    /// <param name="constructionDescription">Construction description</param>
    /// <param name="expression">Expression for <![CDATA[Func<Record, Record>]]>.</param>
    public static bool TryCreateRecordCloneExpression(IConstructionDescription constructionDescription, [NotNullWhen(true)] out LambdaExpression? expression, Type? delegateRecordType = default, Type? delegateReturnType = default)
    {
        // Record Type
        Type recordType = constructionDescription.Constructor.Type;
        //
        if (delegateRecordType == null) delegateRecordType = recordType;
        if (delegateReturnType == null) delegateReturnType = recordType;
        // Place arg expressions here as array access 'parameters[i]'
        List<Expression> fieldValues = new List<Expression>(constructionDescription.Fields.Length);
        // Record argument
        ParameterExpression recordArgument = Expression.Parameter(delegateRecordType, "record");
        Expression recordArgument_ = recordArgument.Type.Equals(recordType) ? recordArgument : Expression.Convert(recordArgument, recordType);
        // Choose expression for each field
        for (int fieldIndex = 0; fieldIndex < constructionDescription.Fields.Length; fieldIndex++)
        {
            // Get field
            IFieldDescription field = constructionDescription.Fields[fieldIndex];
            // Get reader
            if (!FieldRead.TryCreateFieldReadExpression(field, out LambdaExpression? lambdaExpression)) { expression = null; return false; }
            // 
            Expression readExpression = Expression.Invoke(lambdaExpression, recordArgument_);
            // Add reader
            fieldValues.Add(readExpression);
        }

        // Create Create expression
        if (!RecordCreateFunc.TryCreateExpression(constructionDescription, fieldValues, out Expression? body)) { expression = null; return false; }
        // Cast body
        if (!body.Type.Equals(delegateReturnType)) body = Expression.Convert(body, delegateReturnType);
        // Assign body to record
        body = Expression.Assign(recordArgument, body);
        // Choose delegate type
        Type delegateType = typeof(Func<,>).MakeGenericType(delegateRecordType, delegateReturnType);
        // Compile
        expression = Expression.Lambda(delegateType, body, recordArgument);
        // Return
        return true;
    }

}
