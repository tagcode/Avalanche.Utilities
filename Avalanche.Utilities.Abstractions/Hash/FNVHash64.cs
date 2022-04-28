// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Runtime.InteropServices;

/// <summary>Fowler/Noll/Vo hash</summary>
public struct FNVHash64
{
    /// <summary>consts for hashcoding.</summary>
    public const ulong FNVHashBasis = 0xcbf29ce484222325;
    /// <summary>consts for hashcoding.</summary>
    public const ulong FNVHashPrime = 0x100000001b3;
    /// <summary>Hash value 64bit</summary>
    public ulong Hash;
    /// <summary>Hash value 32bit</summary>
    public int Hash32 => unchecked(((int)(Hash & 0xFFFFFFFF)) | ((int)((Hash >> 32) & 0xFFFFFFFF)));

    /// <summary>Initialize hasher</summary>
    public FNVHash64()
    {
        Hash = FNVHashBasis;
    }

    /// <summary>Hash in <paramref name="bytedata"/></summary>
    public void HashBytes(ReadOnlySpan<byte> bytedata)
    {
        // No data
        if (bytedata.Length == 0) return;
        // Number of chunks of 8
        int chunks = bytedata.Length / 8;
        // Hash in chunks of 8
        if (chunks > 0)
        {
            // Convert access to pointer
            ReadOnlySpan<ulong> lp = MemoryMarshal.Cast<byte, ulong>(bytedata);
            //
            for (int i = 0; i < chunks; i++)
            {
                // Hash-in
                Hash *= FNVHashPrime;
                Hash ^= lp[i];
            }
            // Splice
            bytedata = bytedata.Slice(chunks << 3);
        }

        // Hash in rest
        if (bytedata.Length > 0)
        {
            // Collect hash here
            ulong hash = 0;
            //
            hash |= (ulong)bytedata[0];
            if (bytedata.Length >= 2) hash |= ((ulong)bytedata[1]) << 8;
            if (bytedata.Length >= 3) hash |= ((ulong)bytedata[2]) << 16;
            if (bytedata.Length >= 4) hash |= ((ulong)bytedata[3]) << 24;
            if (bytedata.Length >= 5) hash |= ((ulong)bytedata[4]) << 32;
            if (bytedata.Length >= 6) hash |= ((ulong)bytedata[5]) << 40;
            if (bytedata.Length >= 7) hash |= ((ulong)bytedata[6]) << 48;
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
        Hash ^= value ? 1ul : 0ul;
    }

    /// <summary>Hash in <paramref name="value"/></summary>
    public void HashIn(int value)
    {
        // Hash-in
        Hash *= FNVHashPrime;
        Hash ^= unchecked((ulong)value);
    }

    /// <summary>Hash in <paramref name="value"/></summary>
    public void HashIn(char value)
    {
        // Hash-in
        Hash *= FNVHashPrime;
        Hash ^= unchecked((ulong)value);
    }

    /// <summary>Hash in <paramref name="value"/></summary>
    public void HashIn(uint value)
    {
        // Hash-in
        Hash *= FNVHashPrime;
        Hash ^= unchecked((ulong)value);
    }

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
    }

    /// <summary>Hash in <paramref name="object"/></summary>
    public void HashInObject(object? @object)
    {
        // Hash-in
        Hash *= FNVHashPrime;
        if (@object != null) Hash ^= unchecked((ulong)@object.GetHashCode());
    }

    /// <summary>Write hash as hex to<paramref name="dst"/></summary>
    /// <param name="dst"></param>
    public void WriteHash64AsHex(Span<char> dst)
    {
        // Assert length
        if (dst.Length < 16) throw new ArgumentException("Too short", nameof(dst));
        // 
        ulong hash = this.Hash;
        // 
        for (int i = 0; i < 16; i++)
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
        Span<char> chars = stackalloc char[16];
        // Write
        WriteHash64AsHex(chars);
        // Create string
        string str = new string(chars);
        // Return
        return str;
    }

}

