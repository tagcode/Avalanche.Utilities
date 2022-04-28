// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Avalanche.Utilities.Provider;

/// <summary>Creates <![CDATA[Func<object[], Record]]> delegates that instantiate record-like classes and structs.</summary>
public static class RecordCreateFunc
{
    /// <summary>Creates <![CDATA[Func<object[], Record>]]>}"/>.</summary>
    static readonly IProvider<IRecordDescription, Delegate> create = Providers.Func<IRecordDescription, Delegate>(TryCreateCreateFunc);
    /// <summary>Creates <![CDATA[IResult<Func<object[], Record>>]]>.</summary>
    static readonly IProvider<IRecordDescription, IResult<Delegate>> createResult = create.ResultCaptured();
    /// <summary>Creates and caches <![CDATA[IResult<Func<object[], Record>>]]>.</summary>
    static readonly IProvider<IRecordDescription, IResult<Delegate>> cachedResult = createResult.WeakCached();
    /// <summary>Creates and caches <![CDATA[Func<object[], Record>]]>.</summary>
    static readonly IProvider<IRecordDescription, Delegate> cached = cachedResult.ResultOpened();
    /// <summary>Creates <![CDATA[Func<object[], Record>]]>.</summary>
    static readonly IProvider<Type, Delegate> createFromType = RecordDescription.CreateResult.Concat(createResult).ResultOpened();
    /// <summary>Creates and caches <![CDATA[Func<object[], Record>]]>.</summary>
    static readonly IProvider<Type, Delegate> cachedFromType = RecordDescription.CachedResult.Concat(cachedResult).ResultOpened();

    /// <summary>Creates <![CDATA[Func<object[], Record>]]>.</summary>
    public static IProvider<IRecordDescription, Delegate> Create => create;
    /// <summary>Creates <![CDATA[IResult<Func<object[], Record>>]]>.</summary>
    public static IProvider<IRecordDescription, IResult<Delegate>> CreateResult => createResult;
    /// <summary>Creates and caches <![CDATA[IResult<Func<object[], Record>>]]>.</summary>
    public static IProvider<IRecordDescription, IResult<Delegate>> CachedResult => cachedResult;
    /// <summary>Creates and caches <![CDATA[Func<object[], Record>]]>.</summary>
    public static IProvider<IRecordDescription, Delegate> Cached => cached;
    /// <summary>Creates <![CDATA[Func<object[], Record>]]>.</summary>
    public static IProvider<Type, Delegate> CreateFromType => createFromType;
    /// <summary>Creates and caches <![CDATA[Func<object[], Record>]]>.</summary>
    public static IProvider<Type, Delegate> CachedFromType => cachedFromType;

    /// <summary>Create <![CDATA[Func<object[], Record>]]> delegate</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static bool TryCreateCreateFunc(this IRecordDescription? recordDescription, [NotNullWhen(true)] out Delegate @delegate)
    {
        // Get
        IConstructionDescription? constructionDescription = recordDescription?.Construction as IConstructionDescription;
        // 
        if (constructionDescription == null) { @delegate = null!; return false; }
        // Create LambdaExpression
        if (!constructionDescription.TryCreateCreateExpression(out LambdaExpression? expression)) { @delegate = null!; return false; }
        // Compile
        @delegate = expression.Compile();
        //
        return true;
    }

    /// <summary>Create <![CDATA[Func<object[], Record>]]> delegate</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static bool TryCreateCreateFunc(this IConstructionDescription? constructionDescription, [NotNullWhen(true)] out Delegate? @delegate)
    {
        // No construction description
        if (constructionDescription == null) { @delegate = null; return false; }
        // Create LambdaExpression
        if (!TryCreateCreateExpression(constructionDescription, out LambdaExpression? expression)) { @delegate = null; return false; }
        // Compile
        @delegate = expression.Compile();
        // Return
        return true;
    }
    /// <summary>Create <![CDATA[Func<object[], Record>]]> delegate</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static bool TryCreateCreateFunc(this IConstructionDescription? constructionDescription, [NotNullWhen(true)] out Delegate? @delegate, Type? delegateReturnType)
    {
        // No construction description
        if (constructionDescription == null) { @delegate = null; return false; }
        // Create LambdaExpression
        if (!TryCreateCreateExpression(constructionDescription, out LambdaExpression? expression, delegateReturnType)) { @delegate = null; return false; }
        // Compile
        @delegate = expression.Compile();
        // Return
        return true;
    }

    /// <summary>Create <![CDATA[Func<object[], Record>]]> expression</summary>
    public static bool TryCreateCreateExpression(this IConstructionDescription constructionDescription, [NotNullWhen(true)] out LambdaExpression? expression, Type? delegateReturnType = default)
    {
        // Assign return type
        if (delegateReturnType == null) delegateReturnType = constructionDescription.Constructor.Type;
        //
        ParameterExpression argsArray = Expression.Parameter(typeof(object?[]), "args");
        // Place arg expressions here as array access 'parameters[i]'
        List<Expression> fieldValues = new List<Expression>(constructionDescription.Fields.Length);
        // Choose expression for each field
        for (int fieldIndex = 0; fieldIndex < constructionDescription.Fields.Length; fieldIndex++)
        {
            // Get field
            IFieldDescription field = constructionDescription.Fields[fieldIndex];
            // Create expression that accesses the field value from array
            Expression e = Expression.ArrayAccess(argsArray, Expression.Constant(fieldIndex));
            // Type-cast, if needed
            if (!typeof(object).Equals(field.Type)) e = Expression.Convert(e, field.Type);
            // Add to list
            fieldValues.Add(e);
        }

        // Create Create expression
        if (!TryCreateExpression(constructionDescription, fieldValues, out Expression? body)) { expression = null; return false; }
        // Cast body
        if (!body.Type.Equals(delegateReturnType)) body = Expression.Convert(body, delegateReturnType);
        // Choose delegate type
        Type delegateType = typeof(Func<,>).MakeGenericType(typeof(object[]), delegateReturnType);
        // Compile
        expression = Expression.Lambda(delegateType, body, argsArray);
        // Return
        return true;
    }

    /// <summary>Create create expression</summary>
    /// <param name="constructionDescription"></param>
    /// <param name="fieldValues">Expressions that provide field values, correlate with <paramref name="constructionDescription"/>.Fields</param>
    /// <param name="expression">Returns <see cref="UnaryExpression"/> that constructs record</param>
    public static bool TryCreateExpression(IConstructionDescription constructionDescription, IList<Expression> fieldValues, [NotNullWhen(true)] out Expression? expression)
    {
        // Get constructor
        object constructorObj = constructionDescription.Constructor.Constructor;
        //
        bool toBind = false;
        // Record Type
        Type recordType = constructionDescription.Constructor.Type;
        // ref Record Type
        Type recordTypeByRef = recordType.MakeByRefType();
        //
        ParameterExpression argsArray = Expression.Parameter(typeof(object?[]), "args");

        // Place body here
        Expression? body = null;
        //

        // Create constructor that reads each field.
        if (constructorObj != null)
        {
            // New struct
            if (constructorObj is EmitLine line && line.OpCode == OpCodes.Initobj && line.Count == 1 && line.Arg0 is System.Type _recordType)
            {
                // NewObj
                body = Expression.New(_recordType);
                // Assert no parameters
                if (constructionDescription.Parameters != null && constructionDescription.Parameters.Length > 0) throw new ArgumentException($"Cannot have parameters with NewObj construction.");
                // 
                toBind = true;
            }
            // .ctor
            else if (constructorObj is ConstructorInfo ci)
            {
                // Params
                Expression[] parameters = new Expression[constructionDescription.Parameters.Length];
                //
                ParameterInfo[] parameterInfos = ci.GetParameters();
                //
                for (int i = 0; i < parameters.Length; i++)
                {
                    // Get parameter
                    IParameterDescription parameter = constructionDescription.Parameters[i];
                    // Get field
                    if (!constructionDescription.ParameterToField.TryGetValue(parameter, out IFieldDescription? field)) { expression = null; return false; }
                    // Get field index
                    int fieldIndex = constructionDescription.Fields.IndexOf(field, (IEqualityComparer<object>)System.Collections.Generic.ReferenceEqualityComparer.Instance);
                    // Field was not found ??
                    if (fieldIndex < 0) throw new ArgumentException($"Expected to find field {field.Name} in {nameof(IConstructionDescription)}");
                    // Get value soure expression
                    Expression fieldExpression = fieldValues[fieldIndex];
                    // Cast
                    //if (!fieldExpression.Type.Equals(parameter.Type) && fieldExpression.Type.IsAssignableTo(parameter.Type)) fieldExpression = Expression.Convert(fieldExpression, parameter.Type);
                    if (!fieldExpression.Type.Equals(parameterInfos[i].ParameterType)) fieldExpression = Expression.Convert(fieldExpression, parameterInfos[i].ParameterType);
                    // Assign value
                    parameters[i] = fieldExpression;
                }
                //
                body = Expression.New(ci, parameters);
                // 
                toBind = true;
            }

            // Not implemented
            else if (constructorObj is MethodInfo) { expression = null; return false; }
            // Not implemented
            else if (constructorObj is Delegate) { expression = null; return false; }
            // Not implemented
            else { expression = null; return false; }
        }

        //
        toBind &= body is NewExpression;

        // Place init bindings here
        List<MemberBinding> initBindings = new List<MemberBinding>(constructionDescription.UnmatchedFields.Count);
        // To execute
        List<Expression> blockExps = new List<Expression>();
        // Record in local variable
        ParameterExpression? recordInLocalVariable = null;
        // Write rest of the fields that constructor did not access
        foreach (IFieldDescription field2 in constructionDescription.UnmatchedFields)
        {
            // Get field index
            int fieldIndex = constructionDescription.Fields.IndexOf(field2, (IEqualityComparer<object>)System.Collections.Generic.ReferenceEqualityComparer.Instance);
            // Field was not found ??
            if (fieldIndex < 0) throw new ArgumentException($"Expected to find field {field2.Name} in {nameof(IConstructionDescription)}");
            // Get value soure expression
            Expression argValue = fieldValues[fieldIndex];
            // Bind value
            if (toBind && (field2.Writer is FieldInfo || field2.Writer is PropertyInfo))
            {
                // Bind expression
                MemberAssignment bind = Expression.Bind((MemberInfo)field2.Writer, argValue);
                // Add to list
                initBindings.Add(bind);
            }
            // Write value
            else
            {
                // Get writer
                if (!FieldWrite.TryCreateFieldWriteExpression(field2, out LambdaExpression? lambdaExpression)) { expression = null; return false; }
                // Put record to local
                if (recordInLocalVariable == null)
                {
                    // Create local
                    blockExps.Add(recordInLocalVariable = Expression.Variable(recordType));
                    // Write to local
                    blockExps.Add(Expression.Assign(recordInLocalVariable, body!));
                }
                // Invoke write
                blockExps.Add(Expression.Invoke(lambdaExpression!, recordInLocalVariable, argValue));
            }
        }

        // Bind
        if (toBind && initBindings.Count > 0) body = Expression.MemberInit((NewExpression)body!, initBindings);
        // Run block expressions
        if (blockExps.Count > 0)
        {
            //
            blockExps.Add(recordInLocalVariable!);
            //
            body = Expression.Block(recordType, new ParameterExpression[] { recordInLocalVariable! }, blockExps);
        }

        // Return
        expression = body!;
        return true;
    }

}
