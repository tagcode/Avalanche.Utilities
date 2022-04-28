// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

/// <summary>Creates long identities between 0 .. 9223372036854775807L</summary>
public class LongGenerator : IdGeneratorBase<long>
{
    /// <summary>Singleton</summary>
    static IIdGenerator<long> instance = new LongGenerator(IIdGenerator.Scope.Process);
    /// <summary>Singleton</summary>
    public static IIdGenerator<long> Instance => instance;

    /// <summary>Last value</summary>
    protected long value = -1;
    /// <summary>Lock</summary>
    protected object mLock = new object();

    /// <summary>Create provider</summary>
    public LongGenerator(IIdGenerator.Scope uniqueness = IIdGenerator.Scope.Instance) : base(uniqueness) { }

    /// <summary>Test if there are more Ids</summary>
    public override bool HasMore { get { lock (mLock) return value < long.MaxValue; } }
    /// <summary>Return next identity</summary>
    public override bool TryGetNext(out long value)
    {
        lock (mLock)
        {
            // Test whether there are more ids
            if (this.value == long.MaxValue) { value = long.MinValue; return false; }
            // Increment and assign
            value = ++this.value;
            // 
            return true;
        }
    }
}
