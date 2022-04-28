// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Avalanche.Utilities.Provider;

/// <summary>Creates <![CDATA[Func<Record, Field, Record>]]>.</summary>
public static class RecreateWithFunc
{
    /// <summary><![CDATA[Func<Record, Field, Record>]]> provider</summary>
    static readonly IProvider<IFieldDescription, Delegate> create = Providers.Func<IFieldDescription, Delegate>(TryCreateRecreateWithFunc);
    /// <summary><![CDATA[Func<Record, Field, Record>]]> provider</summary>
    static readonly IProvider<IFieldDescription, IResult<Delegate>> createResult = create.ResultCaptured();
    /// <summary><![CDATA[Func<Record, Field, Record>]]> provider</summary>
    static readonly IProvider<IFieldDescription, IResult<Delegate>> cachedResult = createResult.WeakCached();
    /// <summary><![CDATA[Func<Record, Field, Record>]]> provider</summary>
    static readonly IProvider<IFieldDescription, Delegate> cached = cachedResult.ResultOpened();

    /// <summary><![CDATA[Func<Record, Field, Record>]]> provider</summary>
    static readonly IProvider<object, Delegate> createFromObject = FieldDescription.CreateWithRecord.Concat(create);
    /// <summary><![CDATA[Func<Record, Field, Record>]]> provider</summary>
    static readonly IProvider<object, IResult<Delegate>> createResultFromObject = FieldDescription.CreateResultWithRecord.Concat(createResult);
    /// <summary><![CDATA[Func<Record, Field, Record>]]> provider</summary>
    static readonly IProvider<object, IResult<Delegate>> cachedResultFromObject = FieldDescription.CachedResultWithRecord.Concat(cachedResult);
    /// <summary><![CDATA[Func<Record, Field, Record>]]> provider</summary>
    static readonly IProvider<object, Delegate> cachedFromObject = FieldDescription.CachedWithRecord.Concat(cached);

    /// <summary><![CDATA[Func<Record, Field, Record>]]> provider</summary>
    public static IProvider<IFieldDescription, Delegate> Create => create;
    /// <summary><![CDATA[Func<Record, Field, Record>]]> provider</summary>
    public static IProvider<IFieldDescription, IResult<Delegate>> CreateResult => createResult;
    /// <summary><![CDATA[Func<Record, Field, Record>]]> provider</summary>
    public static IProvider<IFieldDescription, IResult<Delegate>> CachedResult => cachedResult;
    /// <summary><![CDATA[Func<Record, Field, Record>]]> provider</summary>
    public static IProvider<IFieldDescription, Delegate> Cached => cached;

    /// <summary><![CDATA[Func<Record, Field, Record>]]> provider</summary>
    public static IProvider<object, Delegate> CreateFromObject => createFromObject;
    /// <summary><![CDATA[Func<Record, Field, Record>]]> provider</summary>
    public static IProvider<object, IResult<Delegate>> CreateResultFromObject => createResultFromObject;
    /// <summary><![CDATA[Func<Record, Field, Record>]]> provider</summary>
    public static IProvider<object, IResult<Delegate>> CachedResultFromObject => cachedResultFromObject;
    /// <summary><![CDATA[Func<Record, Field, Record>]]> provider</summary>
    public static IProvider<object, Delegate> CachedFromObject => cachedFromObject;

    /// <summary>Create <![CDATA[Func<Record, Field, Record>]]> delegate</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static bool TryCreateRecreateWithFunc(this IFieldDescription field, IConstructionDescription? constructionDescription, [NotNullWhen(true)] out Delegate? @delegate)
    {
        // No construction description
        if (constructionDescription == null) { @delegate = null; return false; }
        // Create LambdaExpression
        if (!TryCreateRecreateWithExpressionFunc(constructionDescription, field, out LambdaExpression? expression)) { @delegate = null; return false; }
        // Compile
        @delegate = expression.Compile();
        // Return
        return true;
    }

    /// <summary>Create <![CDATA[Func<Record, Field, Record>]]> delegate</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static bool TryCreateRecreateWithFunc(this IFieldDescription field, [NotNullWhen(true)] out Delegate @delegate)
    {
        //
        IConstructionDescription? constructionDescription = field.Record?.Construction as IConstructionDescription;
        // No construction description
        if (constructionDescription == null) { @delegate = null!; return false; }
        // Create LambdaExpression
        if (!TryCreateRecreateWithExpressionFunc(constructionDescription, field, out LambdaExpression? expression)) { @delegate = null!; return false; }
        // Compile
        @delegate = expression.Compile();
        // Return
        return true;
    }

    /// <summary>Create <![CDATA[Func<Record, Field, Record>]]> delegate</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static bool TryCreateRecreateWithFunc(this IFieldDescription field, [NotNullWhen(true)] out Delegate @delegate, Type? delegateRecordType, Type? delegateFieldType, Type? delegateReturnType)
    {
        //
        IConstructionDescription? constructionDescription = field.Record?.Construction as IConstructionDescription;
        // No construction description
        if (constructionDescription == null) { @delegate = null!; return false; }
        // Create LambdaExpression
        if (!TryCreateRecreateWithExpressionFunc(constructionDescription, field, out LambdaExpression? expression, delegateRecordType, delegateFieldType, delegateReturnType)) { @delegate = null!; return false; }
        // Compile
        @delegate = expression.Compile();
        // Return
        return true;
    }

    /// <summary>Create <![CDATA[Func<Record, Field, Record>]]> expression</summary>
    /// <param name="fieldDescription">The field whose value is overwritten</param>
    /// <param name="constructionDescription">Construction description</param>
    /// <param name="expression">Expression for <![CDATA[Func<Record, Field, Record>]]>.</param>
    /// <param name="delegateRecordType">(optional) Record type for Func</param>
    /// <param name="delegateFieldType">(optional) Field type for Func</param>
    /// <param name="delegateReturnType">(optional) Return type for Func</param>
    public static bool TryCreateRecreateWithExpressionFunc(IConstructionDescription constructionDescription, IFieldDescription fieldDescription, [NotNullWhen(true)] out LambdaExpression? expression, Type? delegateRecordType = default, Type? delegateFieldType = default, Type? delegateReturnType = default)
    {
        // Record Type
        Type recordType = constructionDescription.Constructor.Type;
        // Field type
        Type fieldType = fieldDescription.Type;
        //
        if (delegateRecordType == null) delegateRecordType = recordType;
        if (delegateFieldType == null) delegateFieldType = fieldType;
        if (delegateReturnType == null) delegateReturnType = delegateRecordType;
        // Place arg expressions here as array access 'parameters[i]'
        List<Expression> fieldValues = new List<Expression>(constructionDescription.Fields.Length);
        // Record argument
        ParameterExpression recordArgument = Expression.Parameter(delegateRecordType, "record");
        Expression recordArgument_ = recordArgument.Type.Equals(recordType) ? recordArgument : Expression.Convert(recordArgument, recordType);
        // Field value
        ParameterExpression fieldValueArgument = Expression.Parameter(delegateFieldType, "fieldValue");
        Expression fieldValueArgument_ = fieldValueArgument.Type.Equals(fieldType) ? fieldValueArgument : Expression.Convert(fieldValueArgument, fieldType);
        // Choose expression for each field
        for (int fieldIndex = 0; fieldIndex < constructionDescription.Fields.Length; fieldIndex++)
        {
            // Get field
            IFieldDescription field = constructionDescription.Fields[fieldIndex];
            // Use parameter
            if (field == fieldDescription) fieldValues.Add(fieldValueArgument_);
            // Create read expression
            else
            {
                // Get reader
                if (!FieldRead.TryCreateFieldReadExpression(field, out LambdaExpression? lambdaExpression, recordArgument_.Type, default)) { expression = null; return false; }
                // 
                Expression readExpression = Expression.Invoke(lambdaExpression, recordArgument_);
                // Add reader
                fieldValues.Add(readExpression);
            }
        }

        // Create Create expression
        if (!RecordCreateFunc.TryCreateExpression(constructionDescription, fieldValues, out Expression? body)) { expression = null; return false; }
        // Cast body
        if (!body.Type.Equals(delegateReturnType)) body = Expression.Convert(body, delegateReturnType);
        // Choose delegate type
        Type delegateType = typeof(Func<,,>).MakeGenericType(delegateRecordType, delegateFieldType, delegateReturnType);
        // Compile
        expression = Expression.Lambda(delegateType, body, recordArgument, fieldValueArgument);
        // Return
        return true;
    }

}
