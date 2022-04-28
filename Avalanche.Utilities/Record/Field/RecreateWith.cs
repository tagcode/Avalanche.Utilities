// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Avalanche.Utilities.Provider;

/// <summary>Creates <see cref="RecreateWith{Record, Field}"/>.</summary>
public static class RecreateWith
{
    /// <summary><see cref="RecreateWith{Record, Field}"/> provider</summary>
    static readonly IProvider<IFieldDescription, Delegate> create = Providers.Func<IFieldDescription, Delegate>(TryCreateRecreateWith);
    /// <summary><see cref="RecreateWith{Record, Field}"/> provider</summary>
    static readonly IProvider<IFieldDescription, IResult<Delegate>> createResult = create.ResultCaptured();
    /// <summary><see cref="RecreateWith{Record, Field}"/> provider</summary>
    static readonly IProvider<IFieldDescription, IResult<Delegate>> cachedResult = createResult.WeakCached();
    /// <summary><see cref="RecreateWith{Record, Field}"/> provider</summary>
    static readonly IProvider<IFieldDescription, Delegate> cached = cachedResult.ResultOpened();

    /// <summary><see cref="RecreateWith{Record, Field}"/> provider</summary>
    static readonly IProvider<object, Delegate> createFromObject = FieldDescription.CreateWithRecord.Concat(create);
    /// <summary><see cref="RecreateWith{Record, Field}"/> provider</summary>
    static readonly IProvider<object, IResult<Delegate>> createResultFromObject = FieldDescription.CreateResultWithRecord.Concat(createResult);
    /// <summary><see cref="RecreateWith{Record, Field}"/> provider</summary>
    static readonly IProvider<object, IResult<Delegate>> cachedResultFromObject = FieldDescription.CachedResultWithRecord.Concat(cachedResult);
    /// <summary><see cref="RecreateWith{Record, Field}"/> provider</summary>
    static readonly IProvider<object, Delegate> cachedFromObject = FieldDescription.CachedWithRecord.Concat(cached);

    /// <summary><see cref="RecreateWith{Record, Field}"/> provider</summary>
    public static IProvider<IFieldDescription, Delegate> Create => create;
    /// <summary><see cref="RecreateWith{Record, Field}"/> provider</summary>
    public static IProvider<IFieldDescription, IResult<Delegate>> CreateResult => createResult;
    /// <summary><see cref="RecreateWith{Record, Field}"/> provider</summary>
    public static IProvider<IFieldDescription, IResult<Delegate>> CachedResult => cachedResult;
    /// <summary><see cref="RecreateWith{Record, Field}"/> provider</summary>
    public static IProvider<IFieldDescription, Delegate> Cached => cached;

    /// <summary><see cref="RecreateWith{Record, Field}"/> provider</summary>
    public static IProvider<object, Delegate> CreateFromObject => createFromObject;
    /// <summary><see cref="RecreateWith{Record, Field}"/> provider</summary>
    public static IProvider<object, IResult<Delegate>> CreateResultFromObject => createResultFromObject;
    /// <summary><see cref="RecreateWith{Record, Field}"/> provider</summary>
    public static IProvider<object, IResult<Delegate>> CachedResultFromObject => cachedResultFromObject;
    /// <summary><see cref="RecreateWith{Record, Field}"/> provider</summary>
    public static IProvider<object, Delegate> CachedFromObject => cachedFromObject;

    /// <summary>Create <see cref="RecreateWith{Record, Field}"/> delegate</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static bool TryCreateRecreateWith(this IFieldDescription field, IConstructionDescription? constructionDescription, [NotNullWhen(true)] out Delegate? @delegate)
    {
        // No construction description
        if (constructionDescription == null) { @delegate = null; return false; }
        // Create LambdaExpression
        if (!TryCreateRecreateWithExpression(constructionDescription, field, out LambdaExpression? expression)) { @delegate = null; return false; }
        // Compile
        @delegate = expression.Compile();
        // Return
        return true;
    }

    /// <summary>Create <see cref="RecreateWith{Record, Field}"/> delegate</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static bool TryCreateRecreateWith(this IFieldDescription field, [NotNullWhen(true)] out Delegate @delegate)
    {
        //
        IConstructionDescription? constructionDescription = field.Record?.Construction as IConstructionDescription;
        // No construction description
        if (constructionDescription == null) { @delegate = null!; return false; }
        // Create LambdaExpression
        if (!TryCreateRecreateWithExpression(constructionDescription, field, out LambdaExpression? expression)) { @delegate = null!; return false; }
        // Compile
        @delegate = expression.Compile();
        // Return
        return true;
    }

    /// <summary>Create <see cref="RecreateWith{Record, Field}"/> expression</summary>
    /// <param name="fieldDescription">The field whose value is overwritten</param>
    /// <param name="constructionDescription">Construction description</param>
    /// <param name="expression">Expression for <see cref="RecreateWith{Record, Field}"/>.</param>
    public static bool TryCreateRecreateWithExpression(IConstructionDescription constructionDescription, IFieldDescription fieldDescription, [NotNullWhen(true)] out LambdaExpression? expression)
    {
        // Record Type
        Type recordType = constructionDescription.Constructor.Type;
        // Field type
        Type fieldType = fieldDescription.Type;
        // Place arg expressions here as array access 'parameters[i]'
        List<Expression> fieldValues = new List<Expression>(constructionDescription.Fields.Length);
        // Record argument
        ParameterExpression recordArgument = Expression.Parameter(recordType.MakeByRefType(), "record");
        // Field value
        ParameterExpression fieldValueArgument = Expression.Parameter(fieldType, "fieldValue");
        // Choose expression for each field
        for (int fieldIndex = 0; fieldIndex < constructionDescription.Fields.Length; fieldIndex++)
        {
            // Get field
            IFieldDescription field = constructionDescription.Fields[fieldIndex];
            // Use parameter
            if (field == fieldDescription) fieldValues.Add(fieldValueArgument);
            // Create read expression
            else
            {
                // Get reader
                if (!FieldRead.TryCreateFieldReadExpression(field, out LambdaExpression? lambdaExpression)) { expression = null; return false; }
                // 
                Expression readExpression = Expression.Invoke(lambdaExpression, recordArgument);
                // Add reader
                fieldValues.Add(readExpression);
            }
        }

        // Create Create expression
        if (!RecordCreateFunc.TryCreateExpression(constructionDescription, fieldValues, out Expression? body)) { expression = null; return false; }
        // Assign body to record
        body = Expression.Assign(recordArgument, body);
        // Choose delegate type
        Type delegateType = typeof(RecreateWith<,>).MakeGenericType(recordType, fieldType);
        // Compile
        expression = Expression.Lambda(delegateType, body, recordArgument, fieldValueArgument);
        // Return
        return true;
    }

}
