// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Runtime.InteropServices;

/// <summary>Fowler/Noll/Vo hash</summary>
public struct FNVHash32
{
    /// <summary></summary>
    public const int FNVHashBasis = unchecked((int)2166136261);
    /// <summary></summary>
    public const int FNVHashPrime = 16777619;
    /// <summary>Hash value 32bit</summary>
    public int Hash;

    /// <summary>Initialize hasher</summary>
    public FNVHash32()
    {
        Hash = FNVHashBasis;
    }

    /// <summary>Hash in <paramref name="bytespan"/></summary>
    public void HashBytes(ReadOnlySpan<byte> bytespan)
    {
        // No data
        if (bytespan.Length == 0) return;
        // Number of chunks of 8
        int chunks = bytespan.Length / 8;
        // Hash in chunks of 8
        if (chunks > 0)
        {
            // Convert access to pointer
            ReadOnlySpan<int> lp = MemoryMarshal.Cast<byte, int>(bytespan);
            //
            for (int i = 0; i < chunks; i++)
            {
                // Hash-in
                Hash *= FNVHashPrime;
                Hash ^= lp[i];
            }
            // Splice
            bytespan = bytespan.Slice(chunks << 3);
        }

        // Hash in rest
        if (bytespan.Length > 0)
        {
            // Collect hash here
            int hash = 0;
            //
            hash |= (int)bytespan[0];
            if (bytespan.Length >= 2) hash |= ((int)bytespan[1]) << 8;
            if (bytespan.Length >= 3) hash |= ((int)bytespan[2]) << 16;
            // Hash-in
            Hash *= FNVHashPrime;
            Hash ^= hash;
        }
    }

    /// <summary>Hash in <paramref name="tspan"/></summary>
    public void HashIn<T>(ReadOnlySpan<T> tspan) where T : struct
    {
        // Convert access to pointer
        ReadOnlySpan<byte> bytes = MemoryMarshal.Cast<T, byte>(tspan);
        // Hash-in bytes
        HashBytes(bytes);
    }

    /// <summary>Hash in <paramref name="str"/></summary>
    public void HashIn(String? str)
    {
        //
        if (str == null) return;
        // Get chars
        ReadOnlySpan<char> chars = str;
        // Convert access to pointer
        ReadOnlySpan<byte> bytes = MemoryMarshal.Cast<char, byte>(chars);
        // Hash-in bytes
        HashBytes(bytes);
    }

    /// <summary>Hash in <paramref name="value"/></summary>
    public void HashIn(bool value)
    {
        // Hash-in
        Hash *= FNVHashPrime;
        Hash ^= value ? 1 : 0;
    }

    /// <summary>Hash in <paramref name="value"/></summary>
    public void HashIn(int value)
    {
        // Hash-in
        Hash *= FNVHashPrime;
        Hash ^= unchecked((int)value);
    }

    /// <summary>Hash in <paramref name="value"/></summary>
    public void HashIn(uint value)
    {
        // Hash-in
        Hash *= FNVHashPrime;
        Hash ^= unchecked((int)value);
    }

    /// <summary>Hash in <paramref name="value"/></summary>
    public void HashIn(ulong value)
    {
        // Hash-in
        Hash *= FNVHashPrime;
        Hash ^= unchecked((int)value);
        Hash ^= unchecked((int)(value >> 32));
    }

    /// <summary>Hash in <paramref name="value"/></summary>
    public void HashIn(long value)
    {
        // Hash-in
        Hash *= FNVHashPrime;
        Hash ^= unchecked((int)value);
        Hash ^= unchecked((int)(value >> 32));
    }

    /// <summary>Hash in <paramref name="object"/></summary>
    public void HashInObject(object? @object)
    {
        // Hash-in
        Hash *= FNVHashPrime;
        if (@object != null) Hash ^= @object.GetHashCode();
    }

    /// <summary>Hash in <paramref name="value"/></summary>
    public void HashIn<T>(T value)
    {
        // Hash-in
        Hash *= FNVHashPrime;
        if (value != null) Hash ^= value.GetHashCode();
    }

    /*
    /// <summary>Hash in <paramref name="value"/></summary>
    public void HashIn(long value)
    {
        // Hash-in
        Hash *= FNVHashPrime;
        Hash ^= (ulong)value;
    }

    /// <summary>Hash in <paramref name="value"/></summary>
    public void HashIn(ulong value)
    {
        // Hash-in
        Hash *= FNVHashPrime;
        Hash ^= value;
    }*/

    /// <summary>Write hash as hex to<paramref name="dst"/></summary>
    /// <param name="dst"></param>
    public void WriteHash32AsHex(Span<char> dst)
    {
        // Assert length
        if (dst.Length < 8) throw new ArgumentException("Too short", nameof(dst));
        // 
        int hash = this.Hash;
        // 
        for (int i = 0; i < 8; i++)
        {
            // Get highest 4 bits
            byte da = (byte)((hash >> 60) & 0xf);
            // Convert to char
            char ch = (char)(da < 10 ? 48 + da : 55 + da);
            // Shift left
            hash <<= 4;
            // Write
            dst[i] = ch;
        }
    }

    /// <summary>Print hash</summary>
    public override string ToString()
    {
        // Allocate
        Span<char> chars = stackalloc char[8];
        // Write
        WriteHash32AsHex(chars);
        // Create string
        string str = new string(chars);
        // Return
        return str;
    }
}
