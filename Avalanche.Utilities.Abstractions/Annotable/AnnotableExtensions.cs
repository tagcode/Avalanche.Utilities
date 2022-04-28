// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections.Generic;

/// <summary>Extension methods for <see cref="IAnnotable"/>.</summary>
public static class AnnotableExtensions
{
    /// <summary>Annotations, typically strings, but always easily serializable.</summary>
    public static T SetAnnotations<T>(this T datatype, object[]? value) where T : notnull, IAnnotable { datatype.Annotations = value!; return datatype; }

    /// <summary>Read annotations that implement <typeparamref name="T"/>.</summary>
    /// <param name="list">Write values here</param>
    public static void ReadAnnotationsOf<T, List>(this IAnnotable? annotable, ref List list) where List : IList<T>
    {
        // Get attributes
        object[]? attributes = annotable?.Annotations;
        // No annotations
        if (attributes == null) return;
        // Get values
        foreach (object o in attributes) if (o is T value) list.Add(value);
    }

    /// <summary>Read indices of annotations that implement <typeparamref name="T"/>.</summary>
    /// <param name="list">Write values here</param>
    public static void ReadAnnotationsIndicesOf<T, List>(this IAnnotable? annotable, ref List list) where List : IList<int>
    {
        // Get attributes
        object[]? attributes = annotable?.Annotations;
        // No annotations
        if (attributes == null) return;
        // Get values
        for (int i = 0; i < attributes.Length; i++) if (attributes[i] is T) list.Add(i);
    }

    /// <summary>Add annotation</summary>
    public static T AddAnnotation<T>(this T dataType, object annotation) where T : IAnnotable
    {
        // Assert not null
        if (annotation == null) throw new ArgumentNullException(nameof(annotation));
        // Get annotations
        object[] annotations = dataType.Annotations;
        // Add annotation
        if (annotations == null) { dataType.Annotations = new object[] { annotation }; return dataType; }
        // Scan annotations
        foreach (object _annotation in annotations)
        {
            // Annotation already found
            if (_annotation.Equals(annotation)) return dataType;
        }
        // Add to array
        Add(ref annotations!, annotation);
        dataType.Annotations = annotations;
        // Not identity
        return dataType;
    }

    /// <summary>Remove annotation</summary>
    public static T RemoveAnnotation<T>(this T datatype, object annotation) where T : IAnnotable
    {
        // Assert not null
        if (annotation == null) throw new ArgumentNullException(nameof(annotation));
        // Get annotations
        object[] annotations = datatype.Annotations;
        // No annotations
        if (annotations == null) return datatype;
        // Search annotation
        int ix = annotations.IndexOf(annotation);
        // No annotation
        if (ix < 0) return datatype;
        // Add to array
        datatype.Annotations = WithoutAt(annotations, ix);
        // Return
        return datatype;
    }

    /// <summary>Remove annotation</summary>
    public static T RemoveAnnotationType<T>(this T datatype, Type annotationType) where T : IAnnotable
    {
        // Assert not null
        if (annotationType == null) throw new ArgumentNullException(nameof(annotationType));
        // Get annotations
        object[] annotations = datatype.Annotations;
        // No annotations
        if (annotations == null) return datatype;
        // Count annotations to remove
        int toRemove = 0;
        for (int i = 0; i < annotations.Length; i++) if (annotations[i].GetType().IsAssignableTo(annotationType)) toRemove++;
        // New array
        object[]? newArray = new object[annotations.Length - toRemove];
        //
        int ix = 0;
        for (int i = 0; i < annotations.Length; i++) if (!annotations[i].GetType().IsAssignableTo(annotationType)) newArray[ix++] = annotations[i];
        // Assign
        datatype.Annotations = newArray;
        // Return
        return datatype;
    }

    /// <summary>Tests if <paramref name="datatype"/> has <paramref name="annotation"/>.</summary>
    public static bool HasAnnotation(this IAnnotable datatype, object annotation)
    {
        // Get array
        object[]? annotations = datatype.Annotations;
        // No array
        if (annotations == null) return false;
        // Scan annotations
        foreach (object _annotation in annotations)
        {
            // Annotation already found
            if (_annotation.Equals(annotation)) return true;
        }
        // Return 
        return false;
    }

    /// <summary>Tests if <paramref name="datatype"/> has <paramref name="annotationType"/>.</summary>
    public static bool HasAnnotationType(this IAnnotable datatype, Type annotationType)
    {
        // Get array
        object[]? annotations = datatype.Annotations;
        // No array
        if (annotations == null) return false;
        // Scan annotations
        foreach (object _annotation in annotations)
        {
            // Annotation already found
            if (_annotation.GetType().IsAssignableTo(annotationType)) return true;
        }
        // Return 
        return false;
    }

    /// <summary>Add <paramref name="element"/> to <paramref name="array"/>.</summary>
    static void Add<T>(ref T[]? array, T element)
    {
        // Create new array
        if (array == null) { array = new T[] { element }; return; }
        // Allocate new
        T[] newArray = new T[array.Length + 1];
        // Copy
        Array.Copy(array, newArray, array.Length);
        // Add
        newArray[array.Length] = element;
        // Assign
        array = newArray;
    }

    /// <summary>Create array without element at <paramref name="index"/></summary>
    /// <param name="index">element index, of less than 0 to do nothing.</param>
    /// <returns>An array without element</returns>
    static T[] WithoutAt<T>(T[] array, int index)
    {
        // Create new array
        if (array == null) return array!;
        // Not found
        if (index < 0) return array;
        // Emptyr array
        if (array.Length == 1 && index == 0) return new T[0];
        // Allocate new
        T[] newArray = new T[array.Length - 1];
        // Copy
        if (index > 0) Array.Copy(array, 0, newArray, 0, index);
        if (index < array.Length - 1) Array.Copy(array, index + 1, newArray, index, array.Length - index - 1);
        // Return
        return newArray;
    }

    /// <summary>Find <paramref name="element"/> from <paramref name="source"/>.</summary>
    /// <typeparam name="T">source element type</typeparam>
    /// <param name="source">source sequence</param>
    /// <param name="element">element</param>
    /// <returns>element index or -1</returns>
    static int IndexOf<T>(this IEnumerable<T>? source, T element, IEqualityComparer<T>? comparer = default)
    {
        // No source
        if (source == null) return -1;
        // Get comparer
        if (comparer == null) comparer = EqualityComparer<T>.Default;
        // Index
        int index = 0;
        // Enumerate
        foreach (T s in source)
        {
            // Compare
            if (comparer.Equals(element, s)) return index;
            // Next
            index++;
        }
        // No result
        return -1;
    }
}

