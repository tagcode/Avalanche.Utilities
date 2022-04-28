// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

/// <summary>
/// This class constructs generic types with runtime parametrization.
/// This version uses constructor with no arguments, and one generic parameter.
/// 
/// <![CDATA[
///   ConstructorT<int, IList> listConstructor = new ConstructorT<int, IList>(typeof(List<>));
///   
///   Func<int, IList> constructor = listConstructor.GetOrCreateConstructor();
///   IList x = constructor.Create(256);
///   
///   IList x2 = listConstructor.Create(typeof(byte), 256);
/// ]]>
/// </summary>
/// <typeparam name="ReturnType"></typeparam>
public class ConstructorT<ReturnType>
{
    /// <summary>A generic type with one generic parameter, for example <see cref="List{T}"/>. Must be assignable to ReturnType.</summary>
    Type genericType;
    /// <summary>Cache of constructors.</summary>
    ConcurrentDictionary<Type, Func<ReturnType>> constructorCache = new ConcurrentDictionary<Type, Func<ReturnType>>();
    /// <summary>Creat function.</summary>
    Func<Type, Func<ReturnType>> createFunc;
    /// <summary>Array of constructor arguments</summary>
    Type[] constructorParamTypes = new Type[] { };
    /// <summary>Array of constructor parameters</summary>
    ParameterExpression[] constructorParams = new ParameterExpression[0];

    /// <summary>Create object that can create constructors generic types with run-time parametrisation.</summary>
    /// <param name="genericType">A generic type with one generic parameter, for example <see cref="List{T}"/></param>
    public ConstructorT(Type genericType)
    {
        // Assert
        this.genericType = genericType ?? throw new ArgumentNullException(nameof(genericType));
        Type[] genericArguments = genericType.GetGenericArguments();
        if (genericArguments.Length != 1 || genericArguments[0].IsGenericType) throw new ArgumentException($"Needs exactly one generic argument. {genericType.FullName} doesn't match criteria for {GetType().FullName}");
        // Create constructor function
        createFunc = CreateConstructor;
    }

    /// <summary>Creates new constructor delegate for GenericType{t}. </summary>
    Func<ReturnType> CreateConstructor(Type type)
    {
        // Make defined type
        Type runtimeType = genericType.MakeGenericType(type);
        // Choose constructor
        ConstructorInfo? ci = runtimeType.GetConstructor(constructorParamTypes);
        // Make create expression
        Expression body = ci != null ? Expression.New(ci, constructorParams) : !runtimeType.IsClass ? Expression.New(runtimeType) : throw new InvalidOperationException($"{GetType().FullName}: Could not find constructor for {runtimeType.FullName}({String.Join(", ", constructorParamTypes.Select(_ => _.FullName))})");
        // Cast
        if (!typeof(ReturnType).Equals(body.Type)) body = Expression.Convert(body, typeof(ReturnType));
        // Create constructor function
        return (Func<ReturnType>)Expression.Lambda(typeof(Func<ReturnType>), body, constructorParams).Compile();
    }

    /// <summary>Create-and-cache constructor for generic type argument <paramref name="t0"/>.</summary>
    public Func<ReturnType> Constructor(Type t0) => constructorCache.GetOrAdd(t0, createFunc);
    /// <summary>Create new instance with generic argument <paramref name="t0"/>.</summary>
    public ReturnType Create(Type t0) => constructorCache.GetOrAdd(t0, createFunc)();
}

/// <summary>
/// This class constructs generic types with runtime parametrization.
/// This version uses constructor with one argument, and one generic parameter.
/// </summary>
/// <typeparam name="ReturnType"></typeparam>
/// <typeparam name="A0">First argument type of the constructor</typeparam>
public class ConstructorT<A0, ReturnType>
{
    /// <summary>A generic type with one generic parameter, for example <see cref="List{T}"/>. Must be assignable to ReturnType.</summary>
    Type genericType;
    /// <summary>Cache of constructors.</summary>
    ConcurrentDictionary<Type, Func<A0, ReturnType>> constructorCache = new ConcurrentDictionary<Type, Func<A0, ReturnType>>();
    /// <summary>Create function.</summary>
    Func<Type, Func<A0, ReturnType>> createFunc;
    /// <summary>Array of constructor arguments</summary>
    Type[] constructorParamTypes = new Type[] { typeof(A0) };
    /// <summary>Array of constructor parameters</summary>
    ParameterExpression[] constructorParams = new ParameterExpression[] { Expression.Parameter(typeof(A0)) };

    /// <summary>Create object that can create constructors generic types with run-time parametrisation.</summary>
    /// <param name="genericType">A generic type with one generic parameter, for example <see cref="List{T}"/></param>
    public ConstructorT(Type genericType)
    {
        // Assert
        this.genericType = genericType ?? throw new ArgumentNullException(nameof(genericType));
        Type[] genericArguments = genericType.GetGenericArguments();
        if (genericArguments.Length != 1 || genericArguments[0].IsGenericType) throw new ArgumentException($"Needs exactly one generic argument. {genericType.FullName} doesn't match criteria for {GetType().FullName}");
        // Create constructor function
        createFunc = CreateConstructor;
    }

    /// <summary>Create new constructor delegate for GenericType <paramref name="type"/>. Does not use cache.</summary>
    /// <returns>Constructor function</returns>
    Func<A0, ReturnType> CreateConstructor(Type type)
    {
        Type runtimeType = genericType.MakeGenericType(type);
        ConstructorInfo? ci = runtimeType.GetConstructor(constructorParamTypes);
        // Make create expression
        Expression body = ci != null ? Expression.New(ci, constructorParams) : throw new InvalidOperationException($"{GetType().FullName}: Could not find constructor for {runtimeType.FullName}({String.Join(", ", constructorParamTypes.Select(_ => _.FullName))})");
        // Cast
        if (!typeof(ReturnType).Equals(body.Type)) body = Expression.Convert(body, typeof(ReturnType));
        // Create constructor function
        return (Func<A0, ReturnType>)Expression.Lambda(typeof(Func<A0, ReturnType>), body, constructorParams).Compile();
    }

    /// <summary>Create-and-cache constructor for generic type argument <paramref name="t0"/>.</summary>
    public Func<A0, ReturnType> Constructor(Type t0) => constructorCache.GetOrAdd(t0, createFunc);
    /// <summary>Create new instance with generic type <paramref name="t0"/> and constructor <paramref name="arg0"/>.</summary>
    public ReturnType Create(Type t0, A0 arg0) => constructorCache.GetOrAdd(t0, createFunc)(arg0);
}

/// <summary>
/// This class constructs generic types with runtime parametrization.
/// This version uses constructor with two arguments, and one generic parameters.
/// </summary>
/// <typeparam name="ReturnType"></typeparam>
/// <typeparam name="A0">First argument type of the constructor</typeparam>
/// <typeparam name="A1">Second argument type of the constructor</typeparam>
public class ConstructorT<A0, A1, ReturnType>
{
    /// <summary>A generic type with one generic parameter, for example <see cref="List{T}"/> Must be assignable to <typeparamref name="ReturnType"/>.</summary>
    Type genericType;
    /// <summary>Cache of constructors.</summary>
    ConcurrentDictionary<Type, Func<A0, A1, ReturnType>> constructorCache = new ConcurrentDictionary<Type, Func<A0, A1, ReturnType>>();
    /// <summary>A delegate for cache.</summary>
    Func<Type, Func<A0, A1, ReturnType>> createFunc;
    /// <summary>Array of constructor arguments</summary>
    Type[] constructorParamTypes = new Type[] { typeof(A0), typeof(A1) };
    /// <summary>Array of constructor parameters</summary>
    ParameterExpression[] constructorParams = new ParameterExpression[] { Expression.Parameter(typeof(A0)), Expression.Parameter(typeof(A1)) };

    /// <summary>Create object that can create constructors generic types with run-time parametrisation.</summary>
    /// <param name="genericType">A generic type with one generic parameter, for example <see cref="List{T}"/></param>
    public ConstructorT(Type genericType)
    {
        // Assert
        this.genericType = genericType ?? throw new ArgumentNullException(nameof(genericType));
        Type[] genericArguments = genericType.GetGenericArguments();
        if (genericArguments.Length != 1 || genericArguments[0].IsGenericType) throw new ArgumentException($"Needs exactly one generic argument. {genericType.FullName} doesn't match criteria for {GetType().FullName}");
        // Create constructor function
        createFunc = CreateConstructor;
    }

    /// <summary>Create new constructor delegate for GenericType <paramref name="type"/>. Does not use cache.</summary>
    /// <returns>Constructor function</returns>
    public Func<A0, A1, ReturnType> CreateConstructor(Type type)
    {
        Type runtimeType = genericType.MakeGenericType(type);
        ConstructorInfo? ci = runtimeType.GetConstructor(constructorParamTypes);
        // Make create expression
        Expression body = ci != null ? Expression.New(ci, constructorParams) : throw new InvalidOperationException($"{GetType().FullName}: Could not find constructor for {runtimeType.FullName}({String.Join(", ", constructorParamTypes.Select(_ => _.FullName))})");
        // Cast
        if (!typeof(ReturnType).Equals(body.Type)) body = Expression.Convert(body, typeof(ReturnType));
        // Create delegate
        return (Func<A0, A1, ReturnType>)Expression.Lambda(typeof(Func<A0, A1, ReturnType>), body, constructorParams).Compile();
    }

    /// <summary>Create-and-cache constructor for generic type argument <paramref name="t0"/>.</summary>
    public Func<A0, A1, ReturnType> Constructor(Type t0) => constructorCache.GetOrAdd(t0, createFunc);
    /// <summary>Create new instance with generic type <paramref name="t0"/> and <paramref name="arg0"/> and <paramref name="arg1"/> constructor parameters.</summary>
    public ReturnType Create(Type t0, A0 arg0, A1 arg1) => constructorCache.GetOrAdd(t0, createFunc)(arg0, arg1);
}

/// <summary>
/// This class constructs generic types with runtime parametrization.
/// This version uses constructor with three arguments, and one generic parameters.
/// </summary>
/// <typeparam name="ReturnType"></typeparam>
/// <typeparam name="A0">First argument type of the constructor</typeparam>
/// <typeparam name="A1">Second argument type of the constructor</typeparam>
/// <typeparam name="A2">Third argument type of the constructor</typeparam>
public class ConstructorT<A0, A1, A2, ReturnType>
{
    /// <summary>A generic type with one generic parameter, for example <see cref="List{T}"/>. Must be assignable to ReturnType.</summary>
    Type genericType;
    /// <summary>Cache of constructors.</summary>
    ConcurrentDictionary<Type, Func<A0, A1, A2, ReturnType>> constructorCache = new ConcurrentDictionary<Type, Func<A0, A1, A2, ReturnType>>();
    /// <summary>Create function.</summary>
    Func<Type, Func<A0, A1, A2, ReturnType>> createFunc;
    /// <summary>Array of constructor arguments</summary>
    Type[] constructorParamTypes = new Type[] { typeof(A0), typeof(A1), typeof(A2) };
    /// <summary>Array of constructor parameters</summary>
    ParameterExpression[] constructorParams = new ParameterExpression[] { Expression.Parameter(typeof(A0)), Expression.Parameter(typeof(A1)), Expression.Parameter(typeof(A2)) };

    /// <summary>Create object that can create constructors generic types with run-time parametrisation.</summary>
    /// <param name="genericType">A generic type with one generic parameter, for example <see cref="List{T}"/></param>
    public ConstructorT(Type genericType)
    {
        // Assert
        this.genericType = genericType ?? throw new ArgumentNullException(nameof(genericType));
        Type[] genericArguments = genericType.GetGenericArguments();
        if (genericArguments.Length != 1 || genericArguments[0].IsGenericType) throw new ArgumentException($"Needs exactly one generic argument. {genericType.FullName} doesn't match criteria for {GetType().FullName}");
        // Create constructor function
        createFunc = CreateConstructor;
    }

    /// <summary>Create new constructor delegate for GenericType <paramref name="type"/>. Does not use cache.</summary>
    /// <returns>Constructor function</returns>
    Func<A0, A1, A2, ReturnType> CreateConstructor(Type type)
    {
        Type runtimeType = genericType.MakeGenericType(type);
        ConstructorInfo? ci = runtimeType.GetConstructor(constructorParamTypes);
        // Make create expression
        Expression body = ci != null ? Expression.New(ci, constructorParams) : throw new InvalidOperationException($"{GetType().FullName}: Could not find constructor for {runtimeType.FullName}({String.Join(", ", constructorParamTypes.Select(_ => _.FullName))})");
        // Cast
        if (!typeof(ReturnType).Equals(body.Type)) body = Expression.Convert(body, typeof(ReturnType));
        // Create constructor function
        return (Func<A0, A1, A2, ReturnType>)Expression.Lambda(typeof(Func<A0, A1, A2, ReturnType>), body, constructorParams).Compile();
    }

    /// <summary>Create-and-cache constructor for generic type argument <paramref name="t0"/>.</summary>
    public Func<A0, A1, A2, ReturnType> Constructor(Type t0) => constructorCache.GetOrAdd(t0, createFunc);
    /// <summary>Create new instance with generic type <paramref name="t0"/> and <paramref name="arg0"/>, <paramref name="arg1"/> and <paramref name="arg2"/> constructor parameters.</summary>
    public ReturnType Create(Type t0, A0 arg0, A1 arg1, A2 arg2) => constructorCache.GetOrAdd(t0, createFunc)(arg0, arg1, arg2);
}

/// <summary>
/// This class constructs generic types with runtime parametrization.
/// This version uses constructor with three arguments, and one generic parameters.
/// </summary>
/// <typeparam name="ReturnType"></typeparam>
/// <typeparam name="A0">First argument type of the constructor</typeparam>
/// <typeparam name="A1">Second argument type of the constructor</typeparam>
/// <typeparam name="A2">Third argument type of the constructor</typeparam>
/// <typeparam name="A3">Fourth argument type of the constructor</typeparam>
public class ConstructorT<A0, A1, A2, A3, ReturnType>
{
    /// <summary>A generic type with one generic parameter, for example <see cref="List{T}"/>. Must be assignable to ReturnType.</summary>
    Type genericType;
    /// <summary>Cache of constructors.</summary>
    ConcurrentDictionary<Type, Func<A0, A1, A2, A3, ReturnType>> constructorCache = new ConcurrentDictionary<Type, Func<A0, A1, A2, A3, ReturnType>>();
    /// <summary>Create function.</summary>
    Func<Type, Func<A0, A1, A2, A3, ReturnType>> createFunc;
    /// <summary>Array of constructor arguments</summary>
    Type[] constructorParamTypes = new Type[] { typeof(A0), typeof(A1), typeof(A2), typeof(A3) };
    /// <summary>Array of constructor parameters</summary>
    ParameterExpression[] constructorParams = new ParameterExpression[] { Expression.Parameter(typeof(A0)), Expression.Parameter(typeof(A1)), Expression.Parameter(typeof(A2)), Expression.Parameter(typeof(A3)) };

    /// <summary>Create object that can create constructors generic types with run-time parametrisation.</summary>
    /// <param name="genericType">A generic type with one generic parameter, for example <see cref="List{T}"/></param>
    public ConstructorT(Type genericType)
    {
        // Assert
        this.genericType = genericType ?? throw new ArgumentNullException(nameof(genericType));
        Type[] genericArguments = genericType.GetGenericArguments();
        if (genericArguments.Length != 1 || genericArguments[0].IsGenericType) throw new ArgumentException($"Needs exactly one generic argument. {genericType.FullName} doesn't match criteria for {GetType().FullName}");
        // Create constructor function
        createFunc = CreateConstructor;
    }

    /// <summary>Create new constructor delegate for GenericType <paramref name="type"/>. Does not use cache.</summary>
    /// <returns>Constructor function</returns>
    Func<A0, A1, A2, A3, ReturnType> CreateConstructor(Type type)
    {
        Type runtimeType = genericType.MakeGenericType(type);
        ConstructorInfo? ci = runtimeType.GetConstructor(constructorParamTypes);
        // Make create expression
        Expression body = ci != null ? Expression.New(ci, constructorParams) : throw new InvalidOperationException($"{GetType().FullName}: Could not find constructor for {runtimeType.FullName}({String.Join(", ", constructorParamTypes.Select(_ => _.FullName))})");
        // Cast
        if (!typeof(ReturnType).Equals(body.Type)) body = Expression.Convert(body, typeof(ReturnType));
        // Create constructor function
        return (Func<A0, A1, A2, A3, ReturnType>)Expression.Lambda(typeof(Func<A0, A1, A2, A3, ReturnType>), body, constructorParams).Compile();
    }

    /// <summary>Create-and-cache constructor for generic type argument <paramref name="t0"/>.</summary>
    public Func<A0, A1, A2, A3, ReturnType> Constructor(Type t0) => constructorCache.GetOrAdd(t0, createFunc);
    /// <summary>Create new instance with generic type <paramref name="t0"/> and <paramref name="arg0"/>, <paramref name="arg1"/>, <paramref name="arg2"/> and <paramref name="arg3"/> constructor parameters.</summary>
    public ReturnType Create(Type t0, A0 arg0, A1 arg1, A2 arg2, A3 arg3) => constructorCache.GetOrAdd(t0, createFunc)(arg0, arg1, arg2, arg3);
}

/// <summary>
/// This class constructs generic types with runtime parametrization.
/// This version uses constructor with three arguments, and one generic parameters.
/// </summary>
/// <typeparam name="ReturnType"></typeparam>
/// <typeparam name="A0">First argument type of the constructor</typeparam>
/// <typeparam name="A1">Second argument type of the constructor</typeparam>
/// <typeparam name="A2">Third argument type of the constructor</typeparam>
/// <typeparam name="A3">Fourth argument type of the constructor</typeparam>
/// <typeparam name="A4">Fifth argument type of the constructor</typeparam>
public class ConstructorT<A0, A1, A2, A3, A4, ReturnType>
{
    /// <summary>A generic type with one generic parameter, for example <see cref="List{T}"/>. Must be assignable to ReturnType.</summary>
    Type genericType;
    /// <summary>Cache of constructors.</summary>
    ConcurrentDictionary<Type, Func<A0, A1, A2, A3, A4, ReturnType>> constructorCache = new ConcurrentDictionary<Type, Func<A0, A1, A2, A3, A4, ReturnType>>();
    /// <summary>Create function.</summary>
    Func<Type, Func<A0, A1, A2, A3, A4, ReturnType>> createFunc;
    /// <summary>Array of constructor arguments</summary>
    Type[] constructorParamTypes = new Type[] { typeof(A0), typeof(A1), typeof(A2), typeof(A3), typeof(A4) };
    /// <summary>Array of constructor parameters</summary>
    ParameterExpression[] constructorParams = new ParameterExpression[] { Expression.Parameter(typeof(A0)), Expression.Parameter(typeof(A1)), Expression.Parameter(typeof(A2)), Expression.Parameter(typeof(A3)), Expression.Parameter(typeof(A4)) };

    /// <summary>Create object that can create constructors generic types with run-time parametrisation.</summary>
    /// <param name="genericType">A generic type with one generic parameter, for example <see cref="List{T}"/></param>
    public ConstructorT(Type genericType)
    {
        // Assert
        this.genericType = genericType ?? throw new ArgumentNullException(nameof(genericType));
        Type[] genericArguments = genericType.GetGenericArguments();
        if (genericArguments.Length != 1 || genericArguments[0].IsGenericType) throw new ArgumentException($"Needs exactly one generic argument. {genericType.FullName} doesn't match criteria for {GetType().FullName}");
        // Create constructor function
        createFunc = CreateConstructor;
    }

    /// <summary>Create new constructor delegate for GenericType <paramref name="type"/>. Does not use cache.</summary>
    /// <returns>Constructor function</returns>
    Func<A0, A1, A2, A3, A4, ReturnType> CreateConstructor(Type type)
    {
        Type runtimeType = genericType.MakeGenericType(type);
        ConstructorInfo? ci = runtimeType.GetConstructor(constructorParamTypes);
        // Make create expression
        Expression body = ci != null ? Expression.New(ci, constructorParams) : throw new InvalidOperationException($"{GetType().FullName}: Could not find constructor for {runtimeType.FullName}({String.Join(", ", constructorParamTypes.Select(_ => _.FullName))})");
        // Cast
        if (!typeof(ReturnType).Equals(body.Type)) body = Expression.Convert(body, typeof(ReturnType));
        // Create constructor function
        return (Func<A0, A1, A2, A3, A4, ReturnType>)Expression.Lambda(typeof(Func<A0, A1, A2, A3, A4, ReturnType>), body, constructorParams).Compile();
    }

    /// <summary>Create-and-cache constructor for generic type argument <paramref name="t0"/>.</summary>
    public Func<A0, A1, A2, A3, A4, ReturnType> Constructor(Type t0) => constructorCache.GetOrAdd(t0, createFunc);
    /// <summary>Create new instance with generic type <paramref name="t0"/> and <paramref name="arg0"/>, <paramref name="arg1"/>, <paramref name="arg2"/>, <paramref name="arg3"/> and <paramref name="arg4"/> constructor parameters.</summary>
    public ReturnType Create(Type t0, A0 arg0, A1 arg1, A2 arg2, A3 arg3, A4 arg4) => constructorCache.GetOrAdd(t0, createFunc)(arg0, arg1, arg2, arg3, arg4);
}

