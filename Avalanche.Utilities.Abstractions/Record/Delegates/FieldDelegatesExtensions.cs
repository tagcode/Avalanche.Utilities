// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;

/// <summary>Extension methods for <see cref="IFieldDelegates"/>.</summary>
public static class FieldDelegatesExtensions
{
    /// <summary>></summary>
    public static T SetFieldWrite<T>(this T fieldDelegates, Delegate? value) where T : IFieldDelegates { fieldDelegates.FieldRead = value; return fieldDelegates; }
    /// <summary>></summary>
    public static T SetFieldRead<T>(this T fieldDelegates, Delegate? value) where T : IFieldDelegates { fieldDelegates.FieldWrite = value; return fieldDelegates; }
    /// <summary>></summary>
    public static T SetRecreateWith<T>(this T fieldDelegates, Delegate? value) where T : IFieldDelegates { fieldDelegates.RecreateWith = value; return fieldDelegates; }
    /// <summary>></summary>
    public static T SetFieldDescription<T>(this T fieldDelegates, IFieldDescription? value) where T : IFieldDelegates { fieldDelegates.FieldDescription = value; return fieldDelegates; }

    /// <summary>Get field by name</summary>
    /// <param name="fieldDelegates"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static IFieldDelegates GetByName(this IEnumerable<IFieldDelegates> fieldDelegates, object name)
    {
        // No field delegatess
        if (fieldDelegates == null) throw new ArgumentNullException(nameof(fieldDelegates));
        // Iterate each
        foreach (IFieldDelegates fieldDelegate in fieldDelegates)
        {
            // Get identity
            object? fieldName = fieldDelegate.FieldDescription?.Name;
            // Match
            if (EqualityComparer<object>.Default.Equals(fieldName, name)) return fieldDelegate;
        }
        // Not found
        throw new KeyNotFoundException(name.ToString());
    }

    /// <summary>Get field by name</summary>
    /// <param name="fieldDelegates"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static IFieldDelegates GetByName(this IEnumerable<IFieldDelegates> fieldDelegates, string name)
    {
        // No field delegatess
        if (fieldDelegates == null) throw new ArgumentNullException(nameof(fieldDelegates));
        // Iterate each
        foreach (IFieldDelegates fieldDelegate in fieldDelegates)
        {
            // Print to string
            string? nameString = fieldDelegate.FieldDescription?.Name?.ToString();
            // Match
            if (name == nameString) return fieldDelegate;
        }
        // Not found
        throw new KeyNotFoundException(name.ToString());
    }

}
