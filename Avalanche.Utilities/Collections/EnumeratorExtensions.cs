// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections.Generic;

/// <summary>Extension methods for <see cref="IEnumerator{T}"/>.</summary>
public static class EnumeratorExtensions
{
    /// <summary>Compares using two IEnumerators (in stack).</summary>
    /// <typeparam name="ET"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="a_etor1"></param>
    /// <param name="a_etor2"></param>
    /// <returns>-1 if a_str1 is before a_str2, 0 if they are equal, 1 if a_str1 is after a_str2.</returns>
    public static int CompareEnumerators<ET, T>(ET a_etor1, ET a_etor2) where ET : IEnumerator<T> where T : IComparable<T>
    {
        bool hasNext1, hasNext2;
        for (hasNext1 = a_etor1.MoveNext(), hasNext2 = a_etor2.MoveNext(); hasNext1 && hasNext2; hasNext1 = a_etor1.MoveNext(), hasNext2 = a_etor2.MoveNext())
        {
            int value = a_etor1.Current.CompareTo(a_etor2.Current);
            if (value != 0) return value;
        }
        return hasNext1 ? (hasNext2 ? 0 : 1) : (hasNext2 ? -1 : 0);
    }

    /// <summary>Compares using two IEnumerators (in stack).</summary>
    /// <typeparam name="ET1"></typeparam>
    /// <typeparam name="ET2"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="a_etor1"></param>
    /// <param name="a_etor2"></param>
    /// <returns>-1 if a_str1 is before a_str2, 0 if they are equal, 1 if a_str1 is after a_str2.</returns>
    public static int CompareEnumerators<ET1, ET2, T>(ET1 a_etor1, ET2 a_etor2) where ET1 : IEnumerator<T> where ET2 : IEnumerator<T> where T : IComparable<T>
    {
        bool hasNext1, hasNext2;
        for (hasNext1 = a_etor1.MoveNext(), hasNext2 = a_etor2.MoveNext(); hasNext1 && hasNext2; hasNext1 = a_etor1.MoveNext(), hasNext2 = a_etor2.MoveNext())
        {
            int value = a_etor1.Current.CompareTo(a_etor2.Current);
            if (value != 0) return value;
        }
        return hasNext1 ? (hasNext2 ? 0 : 1) : (hasNext2 ? -1 : 0);
    }

    /// <summary>Count the number of elements in <paramref name="enumerator"/>. Resets enumerator afterwards.</summary>
    /// <typeparam name="ETOR"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerator"></param>
    /// <returns></returns>
    public static int CountElements<ETOR, T>(this ETOR enumerator) where ETOR : IEnumerator<T>
    {
        // Init
        int count = 0;
        // Enumerate
        while (enumerator.MoveNext()) count++;
        // Reset
        enumerator.Reset();
        // Return
        return count;
    }


}
