// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;

/// <summary>Status code utilities.</summary>
public static class StatusCodeUtilities
{
    /// <summary>Make code id by breaking <paramref name="namespace"/> into namespace parts and for each part create modulo id. Uses 100 modulo.</summary>
    public static int ModuloId100(this string @namespace) => MakeModuleId100(@namespace);

    /// <summary>
    /// Calculates code id from <paramref name="namespace"/> string.
    /// 
    /// Calculates hash for first four words of <paramref name="namespace"/> and takes modulo of each.
    /// Uses modulo 99 for each word. Adds one to each number if word exists. Each word gets allocation of 2 decimal digits. 
    /// Adds billion (1,000,000,000) as prefix.
    /// 
    /// For example "Avalanche.Service.Remote" creates event id base number as following
    ///              ^^^^^^^30 ^^^^^60 ^^^^65
    ///            1        30 60 65 000
    /// 
    /// For example: public const int RemoteBaseId = MakeEventId("Avalanche.Service.Remote"), // 1 30 60 65 000
    /// 
    /// The purpose is to be able to use one integer32 number space for events from different class libraries (and hoping that numbers won't overlap).
    /// This way the number space forms a logical tree structure as well.
    /// </summary>
    /// <param name="namespace"></param>
    /// <param name="delimiter"></param>
    /// <returns>code id</returns>
    public static int MakeModuleId100(ReadOnlySpan<char> @namespace, char delimiter = '.')
    {
        // Hash of each word
        uint hash1 = 2166136261, hash2 = 2166136261, hash3 = 2166136261, hash4 = 2166136261;
        // Character count of each word
        int c1 = 0, c2 = 0, c3 = 0, c4 = 0;
        //
        int ix = 0;
        char ch;
        // Hash each
        unchecked
        {
            // Hash1
            while (ix < @namespace.Length && (ch = @namespace[ix++]) != delimiter) { hash1 ^= ch * 0x01000193U; c1++; }
            while (ix < @namespace.Length && (ch = @namespace[ix++]) != delimiter) { hash2 ^= ch * 0x01000193U; c2++; }
            while (ix < @namespace.Length && (ch = @namespace[ix++]) != delimiter) { hash3 ^= ch * 0x01000193U; c3++; }
            while (ix < @namespace.Length && (ch = @namespace[ix++]) != delimiter) { hash4 ^= ch * 0x01000193U; c4++; }
        }
        // Take modulos
        hash1 = c1 > 0 ? 1U + (hash1 % 99U) : 0U;
        hash2 = c2 > 0 ? 1U + (hash2 % 99U) : 0U;
        hash3 = c3 > 0 ? 1U + (hash3 % 99U) : 0U;
        hash4 = c4 > 0 ? 1U + (hash4 % 99U) : 0U;
        // Sum up.
        uint result = 1_000_000_000 + hash1 * 1_00_00_00_0 + hash2 * 1_00_00_0 + hash3 * 1_00_0 + hash4 * 1_0;
        // Return
        return (int)result;
    }

    /// <summary>Hash <paramref name="word"/> and take <paramref name="modulo"/>. Adds one if word length is not 0</summary>
    /// <returns>The hash or 0 if <paramref name="word"/> length is 0.</returns>
    /// <example>For modulo 99, value range is 0=if empty and 1-99 if not.</example>
    public static int HashModulo(ReadOnlySpan<char> word, uint modulo = 99U)
    {
        // Hash of each word
        uint hash = 2166136261;
        //
        int ix = 0;
        // Hash each
        unchecked
        {
            // Hash
            while (ix < word.Length) { hash = (hash ^ word[ix++] * 0x01000193U); }
        }
        // Take modulo 99, assign 0 if length 0
        hash = word.Length > 0 ? 1U + (hash % modulo) : 0U;
        // Return
        return (int)hash;
    }


}
