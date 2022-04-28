// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

/// <summary>Extension methods for <see cref="IAnnotable"/>.</summary>
public static class AnnotableExtensions_
{
    /// <summary>Get annotations that implement <typeparamref name="T"/>.</summary>
    public static IEnumerable<T> GetAnnotationsOf<T>(this IAnnotable? annotable)
    {
        // Get attributes
        object[]? attributes = annotable?.Annotations;
        // No annotations
        if (attributes == null) return Array.Empty<T>();
        // Place here values
        StructList2<T> values = new();
        // Get values
        foreach (object o in attributes) if (o is T value) values.Add(value);
        // Return array
        return values.ToArray();
    }

    /// <summary>Try get first annotation of <typeparamref name="T"/>.</summary>
    public static bool TryGetAnnotation<T>(this IAnnotable? annotable, out T value)
    {
        // Get attributes
        object[]? attributes = annotable?.Annotations;
        // Find annotation
        if (attributes != null) { for (int i = 0; i < attributes.Length; i++) if (attributes[i] is T v) { value = v; return true; } }
        // No annotation
        value = default!;
        return false;
    }

    /// <summary>Read <paramref name="annotable"/> for <see cref="DescriptionAttribute"/>, compile them into a string using "\n" as separator.</summary>
    public static string? GetDescriptionAttribute(this IAnnotable? annotable)
    {
        // Place here descriptions
        StructList2<DescriptionAttribute> descriptions = new();
        // Read descriptions
        annotable.ReadAnnotationsOf<DescriptionAttribute, StructList2<DescriptionAttribute>>(ref descriptions);
        // No descriptions
        if (descriptions.Count == 0) return null;
        // One description
        if (descriptions.Count == 1) return descriptions[0].Description;
        // Get length
        int len = 0;
        for (int i = 0; i < descriptions.Count; i++) len += descriptions[i].Description.Length + (i > 0 ? 1 : 0);
        // Allocate
        Span<char> chars = len < 512 ? stackalloc char[len] : new char[len];
        // Add
        int ix = 0;
        for (int i = 0; i < descriptions.Count; i++)
        {
            // Linefeed
            if (i > 0) chars[ix++] = '\n';
            // Get string
            String description = descriptions[i].Description;
            // Append
            description.CopyTo(chars.Slice(ix));
            ix += description.Length;
        }
        // Return
        return new string(chars);
    }

    /// <summary>Replace all <see cref="DescriptionAttribute"/> with one containing <paramref name="description"/>.</summary>
    /// <param name="description">Description to assign. If null all <see cref="DescriptionAttribute"/> are removed.</param>
    public static T SetDescriptionAttribute<T>(this T annotable, string? description) where T : IAnnotable
    {
        // Place here indices
        StructList2<int> indices = new();
        // Read indices
        annotable.ReadAnnotationsIndicesOf<DescriptionAttribute, StructList2<int>>(ref indices);
        // No previous descriptions 
        if (indices.Count == 0)
        {
            // Assign
            if (description != null) annotable.AddAnnotation(new DescriptionAttribute(description));
        }
        // One previous description
        else if (indices.Count == 1)
        {
            // Remove
            if (description == null) annotable.Annotations = annotable.Annotations.WithoutAt(indices[0]);
            // Replace
            else annotable.Annotations[indices[0]] = new DescriptionAttribute(description);
        }
        // Many previous descriptions
        else
        {
            // Remove all
            if (description == null) for (int i = 0; i < indices.Count; i++) annotable.Annotations.WithoutAt(indices[i]);
            // Replace
            else
            {
                // Replace first
                annotable.Annotations[indices[0]] = new DescriptionAttribute(description);
                // Remove other
                for (int i = 1; i < indices.Count; i++) annotable.Annotations.WithoutAt(indices[i]);
            }
        }
        // Return
        return annotable;
    }

}

