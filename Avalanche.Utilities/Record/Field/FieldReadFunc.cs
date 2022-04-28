// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Avalanche.Utilities.Provider;

/// <summary>Provides <![CDATA[Func<R, F>]]>.</summary>
public static class FieldReadFunc
{
    /// <summary><see cref="Func{Record, Field}"/> provider</summary>
    static readonly IProvider<IFieldDescription, Delegate> create = Providers.Func<IFieldDescription, Delegate>(TryCreateFieldReadFunc);
    /// <summary><see cref="Func{Record, Field}"/> provider</summary>
    static readonly IProvider<IFieldDescription, IResult<Delegate>> createResult = create.ResultCaptured();
    /// <summary><see cref="Func{Record, Field}"/> provider</summary>
    static readonly IProvider<IFieldDescription, IResult<Delegate>> cachedResult = createResult.WeakCached();
    /// <summary><see cref="Func{Record, Field}"/> provider</summary>
    static readonly IProvider<IFieldDescription, Delegate> cached = cachedResult.ResultOpened();

    /// <summary><see cref="FieldRead{Record, Field}"/> provider</summary>
    static readonly IProvider<object, Delegate> createFromObject = FieldDescription.Create.Concat(create);
    /// <summary><see cref="FieldRead{Record, Field}"/> provider</summary>
    static readonly IProvider<object, IResult<Delegate>> createResultFromObject = FieldDescription.CreateResult.Concat(createResult);
    /// <summary><see cref="FieldRead{Record, Field}"/> provider</summary>
    static readonly IProvider<object, IResult<Delegate>> cachedResultFromObject = FieldDescription.CachedResult.Concat(cachedResult);
    /// <summary><see cref="FieldRead{Record, Field}"/> provider</summary>
    static readonly IProvider<object, Delegate> cachedFromObject = FieldDescription.Cached.Concat(cached);

    /// <summary><see cref="Func{Record, Field}"/> provider</summary>
    public static IProvider<IFieldDescription, Delegate> Create => create;
    /// <summary><see cref="Func{Record, Field}"/> provider</summary>
    public static IProvider<IFieldDescription, IResult<Delegate>> CreateResult => createResult;
    /// <summary><see cref="Func{Record, Field}"/> provider</summary>
    public static IProvider<IFieldDescription, IResult<Delegate>> CachedResult => cachedResult;
    /// <summary><see cref="Func{Record, Field}"/> provider</summary>
    public static IProvider<IFieldDescription, Delegate> Cached => cached;

    /// <summary><see cref="FieldRead{Record, Field}"/> provider</summary>
    public static IProvider<object, Delegate> CreateFromObject => createFromObject;
    /// <summary><see cref="FieldRead{Record, Field}"/> provider</summary>
    public static IProvider<object, IResult<Delegate>> CreateResultFromObject => createResultFromObject;
    /// <summary><see cref="FieldRead{Record, Field}"/> provider</summary>
    public static IProvider<object, IResult<Delegate>> CachedResultFromObject => cachedResultFromObject;
    /// <summary><see cref="FieldRead{Record, Field}"/> provider</summary>
    public static IProvider<object, Delegate> CachedFromObject => cachedFromObject;

    /// <summary>Create <see cref="Func{Record, Field}"/> delegate</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static bool TryCreateFieldReadFunc(this IFieldDescription field, [NotNullWhen(true)] out Delegate @delegate)
    {
        // Create LambdaExpression
        if (!TryCreateFieldReadFuncExpression(field, out LambdaExpression? expression)) { @delegate = null!; return false; }
        // Compile
        @delegate = expression.Compile();
        // Return
        return true;
    }

    /// <summary>Create <see cref="Func{Record, Field}"/> expression</summary>
    /// <param name="delegateRecordType">Record type for Func</param>
    public static bool TryCreateFieldReadFuncExpression(IFieldDescription field, [NotNullWhen(true)] out LambdaExpression expression, Type? delegateRecordType = null, Type? delegateFieldType = null)
    {
        //
        MemberInfo? memberInfo = field.Reader as MemberInfo;
        FieldInfo? fi = field.Reader as FieldInfo;
        PropertyInfo? pi = field.Reader as PropertyInfo;
        // Get getter
        MethodInfo? getter = field.Reader as MethodInfo ?? pi?.GetGetMethod();

        //
        if (memberInfo == null || (fi == null && getter == null)) { expression = null!; return false; }
        // Field cannot be written
        if (fi != null && fi.IsPrivate) { expression = null!; return false; }
        // Property cannot be written
        if (pi != null && !pi.CanRead) { expression = null!; return false; }
        // Get member info
        Type recordType = memberInfo.ReflectedType ?? memberInfo.DeclaringType!;
        Type fieldType = pi?.PropertyType ?? fi?.FieldType!;
        // Get record type
        if (delegateRecordType == null) delegateRecordType = field.Record?.Type ?? memberInfo.ReflectedType ?? memberInfo.DeclaringType;
        // Get value type
        if (delegateFieldType == null) delegateFieldType = field.Type ?? pi?.PropertyType ?? fi?.FieldType;
        // No types
        if (delegateRecordType == null || delegateFieldType == null) { expression = null!; return false; }

        // Create expression
        ParameterExpression pe = Expression.Parameter(delegateRecordType, "record");
        Expression pe_ = delegateRecordType.Equals(recordType) ? pe : Expression.Convert(pe, recordType);
        Expression body = getter != null ? Expression.Call(pe_, getter) : Expression.Field(pe_, fi!);
        if (!body.Type.Equals(delegateFieldType)) body = Expression.Convert(body, delegateFieldType);
        System.Type delegateType = typeof(Func<,>).MakeGenericType(delegateRecordType, delegateFieldType);
        // Create lambda expression
        expression = Expression.Lambda(delegateType, body, pe);
        // Return
        return true;
    }
}

