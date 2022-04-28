// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Avalanche.Utilities.Provider;

/// <summary>Provides <![CDATA[FieldRead<R, F>]]>.</summary>
public static class FieldRead
{
    /// <summary><see cref="FieldRead{Record, Field}"/> provider</summary>
    static readonly IProvider<IFieldDescription, Delegate> create = Providers.Func<IFieldDescription, Delegate>(TryCreateFieldReadDelegate);
    /// <summary><see cref="FieldRead{Record, Field}"/> provider</summary>
    static readonly IProvider<IFieldDescription, IResult<Delegate>> createResult = create.ResultCaptured();
    /// <summary><see cref="FieldRead{Record, Field}"/> provider</summary>
    static readonly IProvider<IFieldDescription, IResult<Delegate>> cachedResult = createResult.WeakCached();
    /// <summary><see cref="FieldRead{Record, Field}"/> provider</summary>
    static readonly IProvider<IFieldDescription, Delegate> cached = cachedResult.ResultOpened();

    /// <summary><see cref="FieldRead{Record, Field}"/> provider</summary>
    static readonly IProvider<object, Delegate> createFromObject = FieldDescription.Create.Concat(create);
    /// <summary><see cref="FieldRead{Record, Field}"/> provider</summary>
    static readonly IProvider<object, IResult<Delegate>> createResultFromObject = FieldDescription.CreateResult.Concat(createResult);
    /// <summary><see cref="FieldRead{Record, Field}"/> provider</summary>
    static readonly IProvider<object, IResult<Delegate>> cachedResultFromObject = FieldDescription.CachedResult.Concat(cachedResult);
    /// <summary><see cref="FieldRead{Record, Field}"/> provider</summary>
    static readonly IProvider<object, Delegate> cachedFromObject = FieldDescription.Cached.Concat(cached);

    /// <summary><see cref="FieldRead{Record, Field}"/> provider</summary>
    public static IProvider<IFieldDescription, Delegate> Create => create;
    /// <summary><see cref="FieldRead{Record, Field}"/> provider</summary>
    public static IProvider<IFieldDescription, IResult<Delegate>> CreateResult => createResult;
    /// <summary><see cref="FieldRead{Record, Field}"/> provider</summary>
    public static IProvider<IFieldDescription, IResult<Delegate>> CachedResult => cachedResult;
    /// <summary><see cref="FieldRead{Record, Field}"/> provider</summary>
    public static IProvider<IFieldDescription, Delegate> Cached => cached;

    /// <summary><see cref="FieldRead{Record, Field}"/> provider</summary>
    public static IProvider<object, Delegate> CreateFromObject => createFromObject;
    /// <summary><see cref="FieldRead{Record, Field}"/> provider</summary>
    public static IProvider<object, IResult<Delegate>> CreateResultFromObject => createResultFromObject;
    /// <summary><see cref="FieldRead{Record, Field}"/> provider</summary>
    public static IProvider<object, IResult<Delegate>> CachedResultFromObject => cachedResultFromObject;
    /// <summary><see cref="FieldRead{Record, Field}"/> provider</summary>
    public static IProvider<object, Delegate> CachedFromObject => cachedFromObject;

    /// <summary>Create <see cref="FieldRead{Record, Field}"/> delegate</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static bool TryCreateFieldReadDelegate(this IFieldDescription field, [NotNullWhen(true)] out Delegate @delegate)
    {
        // Create LambdaExpression
        if (!TryCreateFieldReadExpression(field, out LambdaExpression? expression)) { @delegate = null!; return false; }
        // Compile
        @delegate = expression.Compile();
        // Return
        return true;
    }

    /// <summary>Create <see cref="FieldRead{Record, Field}"/> delegate</summary>
    /// <exception cref="Exception">On any error.</exception>
    public static bool TryCreateFieldReadDelegate(this IFieldDescription field, [NotNullWhen(true)] out Delegate @delegate, Type? delegateRecordType, Type? delegateFieldType)
    {
        // Create LambdaExpression
        if (!TryCreateFieldReadExpression(field, out LambdaExpression? expression, delegateRecordType, delegateFieldType)) { @delegate = null!; return false; }
        // Compile
        @delegate = expression.Compile();
        // Return
        return true;
    }

    /// <summary>Create <see cref="FieldRead{Record, Field}"/> expression</summary>
    /// <param name="delegateRecordType">Record type for delegate</param>
    /// <param name="field">Field type for delegate</param>
    public static bool TryCreateFieldReadExpression(IFieldDescription field, [NotNullWhen(true)] out LambdaExpression expression, Type? delegateRecordType = default, Type? delegateFieldType = default)
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

        // Record type on reflection
        Type recordType = memberInfo.ReflectedType ?? memberInfo.DeclaringType ?? field.Record?.Type!;
        // Field type on reflection
        Type fieldType = pi?.PropertyType ?? fi?.FieldType ?? field.Type;
        // Record type on delegate
        if (delegateRecordType == null) delegateRecordType = field.Record?.Type ?? field.Record?.Type ?? recordType;
        // Field type on delegate
        if (delegateFieldType == null) delegateFieldType = field.Type ?? fieldType;
        // No types
        if (delegateRecordType == null || delegateFieldType == null || recordType == null || fieldType == null) { expression = null!; return false; }
        // Record type by ref
        System.Type recordByRefType = delegateRecordType.MakeByRefType();

        // Create expression
        ParameterExpression pe = Expression.Parameter(recordByRefType, "record");
        Expression pe_ = delegateRecordType.Equals(recordType) ? pe : Expression.Convert(pe, recordType);
        Expression body = getter != null ? Expression.Call(pe_, getter) : Expression.Field(pe_, fi!);
        body = delegateFieldType.Equals(body.Type) ? body : Expression.Convert(body, delegateFieldType);
        System.Type delegateType = typeof(FieldRead<,>).MakeGenericType(delegateRecordType, delegateFieldType);
        // Create lambda expression
        expression = Expression.Lambda(delegateType, body, pe);
        // Return
        return true;
    }
}
