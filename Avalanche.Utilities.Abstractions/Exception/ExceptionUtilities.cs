// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Collections;
using System.Reflection;

/// <summary><see cref="Exception"/> utilities</summary>
public static class ExceptionUtilities
{
    /// <summary>Create wrapper of <paramref name="e"/> for rethrowing. Tries to create exception of same type as <paramref name="e"/>, but fallbacks to <see cref="Exception"/>.</summary>
    public static Exception Wrap(Exception e)
    {
        // Create clone or fallback
        if (!TryCreate2(e.GetType(), e, e.Message, out Exception ne)) ne = new Exception(e.Message, e);
        // Copy hresult
        ne.HResult = e.HResult;
        // Copy source
        ne.Source = e.Source;
        // Copy data fields
        foreach (DictionaryEntry entry in e.Data) ne.Data[entry.Key] = entry.Value;
        // 
        return ne;
    }

    /// <summary>Create wrapper of <paramref name="errors"/> for rethrowing. Tries to create exception of same type, but fallbacks to <see cref="AggregateException"/>.</summary>
    public static Exception Wrap<List>(List errors) where List : IList<Exception>
    {
        // No exceptions
        if (errors.Count == 0) throw new AggregateException();
        // Got one exception
        if (errors.Count == 1)
        {
            //
            Exception e = errors[0];
            // Create clone or fallback
            if (!TryCreate2(e.GetType(), e, e.Message, out Exception ne)) ne = new AggregateException(e.Message, e);
            // Copy hresult
            ne.HResult = e.HResult;
            // Copy source
            ne.Source = e.Source;
            // Copy data fields
            foreach (DictionaryEntry entry in e.Data) ne.Data[entry.Key] = entry.Value;
            // 
            return ne;
        }
        // Multiple exceptions
        else {
            // Create AggregateException
            AggregateException ae = new AggregateException(errors);
            // 
            return ae;
        }
    }

    /// <summary>Try to create a <paramref name="exceptionType"/> with <paramref name="message"/> and <paramref name="innerException"/>.</summary>
    public static bool TryCreate(Type exceptionType, Exception? innerException, string? message, out Exception exception)
    {
        // Got message and inner exception
        if (message != null && innerException != null)
        {
            // Try get constructor
            ConstructorInfo? c = exceptionType.GetConstructor(bindingAttr: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, binder: null, types: typesSE, modifiers: null);
            // Create 
            if (c != null) { exception = (Exception)c.Invoke(new object[] { message, innerException })!; return true; }
            // Try get constructor
            c = exceptionType.GetConstructor(bindingAttr: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, binder: null, types: typesES, modifiers: null);
            // Create 
            if (c != null) { exception = (Exception)c.Invoke(new object[] { innerException, message })!; return true; }
            // Failed
            exception = null!;
            return false;
        }

        // Got inner exception
        if (innerException != null)
        {
            // Try get constructor
            ConstructorInfo? c = exceptionType.GetConstructor(bindingAttr: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, binder: null, types: typesE, modifiers: null);
            // Create 
            if (c != null) { exception = (Exception)c.Invoke(new object[] { innerException })!; return true; }
            // Failed
            exception = null!;
            return false;
        }

        // Got Message
        if (message != null)
        {
            // Try get constructor
            ConstructorInfo? c = exceptionType.GetConstructor(bindingAttr: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, binder: null, types: typesS, modifiers: null);
            // Create 
            if (c != null) { exception = (Exception)c.Invoke(new object[] { message })!; return true; }
            // Failed
            exception = null!;
            return false;
        }

        // No args
        {
            // Try get constructor
            ConstructorInfo? c = exceptionType.GetConstructor(bindingAttr: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, binder: null, types: types0, modifiers: null);
            // Create 
            if (c != null) { exception = (Exception)c.Invoke(no_args)!; return true; }
            // Failed
            exception = null!;
            return false;
        }
    }

    /// <summary>No types</summary>
    static readonly Type[] types0 = { };
    /// <summary>string, Exception</summary>
    static readonly Type[] typesSE = { typeof(string), typeof(Exception) };
    /// <summary>Exception, string</summary>
    static readonly Type[] typesES = { typeof(Exception), typeof(string) };
    /// <summary>string </summary>
    static readonly Type[] typesS = { typeof(string) };
    /// <summary>Exception</summary>
    static readonly Type[] typesE = { typeof(Exception) };
    /// <summary>No args</summary>
    static readonly object[] no_args = { };

    /// <summary>Try to create a <paramref name="exceptionType"/>. Tries to get constructor with message and innerexception, fallbacks to ones without if not found.</summary>
    public static bool TryCreate2(Type exceptionType, Exception? innerException, string? message, out Exception exception)
    {
        // Got message and inner exception
        if (message != null && innerException != null)
        {
            // Try get constructor
            ConstructorInfo? c = exceptionType.GetConstructor(bindingAttr: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, binder: null, types: typesSE, modifiers: null);
            // Create 
            if (c != null) { exception = (Exception)c.Invoke(new object[] { message, innerException })!; return true; }
            // Try get constructor
            c = exceptionType.GetConstructor(bindingAttr: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, binder: null, types: typesES, modifiers: null);
            // Create 
            if (c != null) { exception = (Exception)c.Invoke(new object[] { innerException, message })!; return true; }
        }

        // Got inner exception
        if (innerException != null)
        {
            // Try get constructor
            ConstructorInfo? c = exceptionType.GetConstructor(bindingAttr: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, binder: null, types: typesE, modifiers: null);
            // Create 
            if (c != null) { exception = (Exception)c.Invoke(new object[] { innerException })!; return true; }
        }

        // Got Message
        if (message != null)
        {
            // Try get constructor
            ConstructorInfo? c = exceptionType.GetConstructor(bindingAttr: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, binder: null, types: typesS, modifiers: null);
            // Create 
            if (c != null) { exception = (Exception)c.Invoke(new object[] { message })!; return true; }
        }

        // No args
        {
            // Try get constructor
            ConstructorInfo? c = exceptionType.GetConstructor(bindingAttr: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, binder: null, types: types0, modifiers: null);
            // Create 
            if (c != null) { exception = (Exception)c.Invoke(no_args)!; return true; }
        }

        exception = null!;
        return false;
    }

}
