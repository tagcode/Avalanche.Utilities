// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Avalanche.Utilities.Provider;

/// <summary>Provides field writers</summary>
public static class FieldWrite
{
    /// <summary><see cref="FieldWrite{Record, Field}"/> provider</summary>
    static readonly IProvider<IFieldDescription, Delegate> create = Providers.Func<IFieldDescription, Delegate>(TryCreateFieldWriteDelegate);
    /// <summary><see cref="FieldWrite{Record, Field}"/> provider</summary>
    static readonly IProvider<IFieldDescription, IResult<Delegate>> createResult = create.ResultCaptured();
    /// <summary><see cref="FieldWrite{Record, Field}"/> provider</summary>
    static readonly IProvider<IFieldDescription, IResult<Delegate>> cachedResult = createResult.WeakCached();
    /// <summary><see cref="FieldWrite{Record, Field}"/> provider</summary>
    static readonly IProvider<IFieldDescription, Delegate> cached = cachedResult.ResultOpened();

    /// <summary><see cref="FieldWrite{Record, Field}"/> provider</summary>
    static readonly IProvider<object, Delegate> createFromObject = FieldDescription.Create.Concat(create);
    /// <summary><see cref="FieldWrite{Record, Field}"/> provider</summary>
    static readonly IProvider<object, IResult<Delegate>> createResultFromObject = FieldDescription.CreateResult.Concat(createResult);
    /// <summary><see cref="FieldWrite{Record, Field}"/> provider</summary>
    static readonly IProvider<object, IResult<Delegate>> cachedResultFromObject = FieldDescription.CachedResult.Concat(cachedResult);
    /// <summary><see cref="FieldWrite{Record, Field}"/> provider</summary>
    static readonly IProvider<object, Delegate> cachedFromObject = FieldDescription.Cached.Concat(cached);

    /// <summary><see cref="FieldWrite{Record, Field}"/> provider</summary>
    public static IProvider<IFieldDescription, Delegate> Create => create;
    /// <summary><see cref="FieldWrite{Record, Field}"/> provider</summary>
    public static IProvider<IFieldDescription, IResult<Delegate>> CreateResult => createResult;
    /// <summary><see cref="FieldWrite{Record, Field}"/> provider</summary>
    public static IProvider<IFieldDescription, IResult<Delegate>> CachedResult => cachedResult;
    /// <summary><see cref="FieldWrite{Record, Field}"/> provider</summary>
    public static IProvider<IFieldDescription, Delegate> Cached => cached;

    /// <summary><see cref="FieldWrite{Record, Field}"/> provider</summary>
    public static IProvider<object, Delegate> CreateFromObject => createFromObject;
    /// <summary><see cref="FieldWrite{Record, Field}"/> provider</summary>
    public static IProvider<object, IResult<Delegate>> CreateResultFromObject => createResultFromObject;
    /// <summary><see cref="FieldWrite{Record, Field}"/> provider</summary>
    public static IProvider<object, IResult<Delegate>> CachedResultFromObject => cachedResultFromObject;
    /// <summary><see cref="FieldWrite{Record, Field}"/> provider</summary>
    public static IProvider<object, Delegate> CachedFromObject => cachedFromObject;

    /// <summary>Create <see cref="FieldWrite{Record, Field}"/> delegate</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static bool TryCreateFieldWriteDelegate(this IFieldDescription field, [NotNullWhen(true)] out Delegate @delegate)
    {
        // Create LambdaExpression
        if (!TryCreateFieldWriteExpression(field, out LambdaExpression? expression)) { @delegate = null!; return false; }
        // Compile
        @delegate = expression.Compile();
        // Return
        return true;
    }

    /// <summary>Create <see cref="FieldWrite{Record, Field}"/> delegate</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static bool TryCreateFieldWriteDelegate(this IFieldDescription field, [NotNullWhen(true)] out Delegate @delegate, Type? delegateRecordType, Type? delegateFieldType)
    {
        // Create LambdaExpression
        if (!TryCreateFieldWriteExpression(field, out LambdaExpression? expression, delegateRecordType, delegateFieldType)) { @delegate = null!; return false; }
        // Compile
        @delegate = expression.Compile();
        // Return
        return true;
    }

    /// <summary>Create <see cref="FieldWrite{Record, Field}"/> expression</summary>
    /// <param name="delegateRecordType">Record type for delegate</param>
    /// <param name="delegateFieldType">Field type for delegate</param>
    public static bool TryCreateFieldWriteExpression(IFieldDescription field, [NotNullWhen(true)] out LambdaExpression expression, Type? delegateRecordType = default, Type? delegateFieldType = default)
    {
        //
        MemberInfo? memberInfo = field.Writer as MemberInfo;
        FieldInfo? fi = field.Writer as FieldInfo;
        PropertyInfo? pi = field.Writer as PropertyInfo;
        // Get setter
        MethodInfo? setter = field.Writer as MethodInfo ?? pi?.GetSetMethod();

        //
        if (memberInfo == null || (fi == null && setter == null)) { expression = null!; return false; }
        // Field cannot be written
        if (fi != null && (fi.IsPrivate || fi.IsInitOnly)) { expression = null!; return false; }
        // Property cannot be written
        if (pi != null && !pi.CanWrite) { expression = null!; return false; }

        // Record type on reflection
        Type memberRecordType = memberInfo.ReflectedType ?? memberInfo.DeclaringType ?? field.Record?.Type!;
        // Field type on reflection
        Type memberFieldType = pi?.PropertyType ?? fi?.FieldType ?? field.Type;
        // Record type on delegate
        if (delegateRecordType == null) delegateRecordType = field.Record?.Type ?? field.Record?.Type ?? memberRecordType;
        // Field type on delegate
        if (delegateFieldType == null) delegateFieldType = field.Type ?? memberFieldType;
        // No types
        if (delegateRecordType == null || delegateFieldType == null || memberRecordType == null || memberFieldType == null) { expression = null!; return false; }
        // Record type by ref
        System.Type recordByRefType = delegateRecordType.MakeByRefType();

        // Create expression
        ParameterExpression pe1 = Expression.Parameter(recordByRefType, "record");
        ParameterExpression pe2 = Expression.Parameter(delegateFieldType, "value");
        Expression pe1_ = delegateRecordType.Equals(memberRecordType) ? pe1 : Expression.Convert(pe1, memberRecordType);
        Expression pe2_ = delegateFieldType.Equals(memberFieldType) ? pe2 : Expression.Convert(pe2, memberFieldType);
        Expression body = setter != null ? Expression.Call(pe1_, setter, pe2_) : Expression.Assign(Expression.Field(pe1_, fi!), pe2_);
        System.Type delegateType = typeof(FieldWrite<,>).MakeGenericType(delegateRecordType, delegateFieldType);
        expression = Expression.Lambda(delegateType, body, pe1, pe2);
        // Return
        return true;
    }
 
}
