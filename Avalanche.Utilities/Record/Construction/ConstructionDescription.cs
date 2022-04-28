// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Runtime.Serialization;

/// <summary>Description of record construction strategy.</summary>
public record ConstructionDescription : ReadOnlyAssignableRecord, IConstructionDescription
{
    /// <summary></summary>
    static IParameterDescription[] no_parameters = new IParameterDescription[0];
    /// <summary></summary>
    static IFieldDescription[] no_fields = new IFieldDescription[0];

    /// <summary>Constructor to match</summary>
    protected IConstructorDescription constructor = null!;
    /// <summary>Parameters to match to</summary>
    protected IParameterDescription[] parameters = no_parameters;
    /// <summary>Fields to match to</summary>
    protected IFieldDescription[] fields = no_fields;

    /// <summary>Left = parameter, right = Field correlations</summary>
    protected IDictionary<IParameterDescription, IFieldDescription> parameterToField = new Dictionary<IParameterDescription, IFieldDescription>(ReferenceEqualityComparer<IParameterDescription>.Instance);
    /// <summary>Left = parameter, right = Field correlations</summary>
    protected IDictionary<IFieldDescription, IParameterDescription> fieldToParameter = new Dictionary<IFieldDescription, IParameterDescription>(ReferenceEqualityComparer<IFieldDescription>.Instance);

    /// <summary>Unmatched constructor parameters</summary>
    protected IList<IParameterDescription> unmatchedParameters = new List<IParameterDescription>();
    /// <summary>Unmatched constructor parameters</summary>
    protected IList<IFieldDescription> unmatchedFields = new List<IFieldDescription>();

    /// <summary>Constructor to match</summary>
    public IConstructorDescription Constructor { get => constructor; set => this.AssertWritable().constructor = value; }
    /// <summary>Parameters to match to</summary>
    public IParameterDescription[] Parameters { get => parameters; set => this.AssertWritable().parameters = value; }
    /// <summary>Fields to match to</summary>
    public IFieldDescription[] Fields { get => fields; set => this.AssertWritable().fields = value; }

    /// <summary>Left = parameter, right = Field correlations</summary>
    public IDictionary<IParameterDescription, IFieldDescription> ParameterToField { get => parameterToField; set => this.AssertWritable().parameterToField = value; }
    /// <summary>Left = parameter, right = Field correlations</summary>
    public IDictionary<IFieldDescription, IParameterDescription> FieldToParameter { get => fieldToParameter; set => this.AssertWritable().fieldToParameter = value; }

    /// <summary>Unmatched constructor parameters</summary>
    public IList<IParameterDescription> UnmatchedParameters { get => unmatchedParameters; set => this.AssertWritable().unmatchedParameters = value; }
    /// <summary>Unmatched constructor parameters</summary>
    public IList<IFieldDescription> UnmatchedFields { get => unmatchedFields; set => this.AssertWritable().unmatchedFields = value; }

    /// <summary></summary>
    protected override void setReadOnly() { hash_cached = this.CalcHash64(); @readonly = true; }
    /// <summary>Cached hashcode, calculated at ReadOnly set.</summary>
    [IgnoreDataMember]
    protected ulong hash_cached;
    /// <summary>Get-or-calc 64bit hash</summary>
    [IgnoreDataMember]
    public ulong Hash64 => @readonly ? hash_cached : this.CalcHash64();
    /// <summary>Calculate hashcode</summary>
    public override int GetHashCode()
    {
        // Get or calc hash
        ulong hash64 = Hash64;
        // 64bit to 32bit
        int hash32 = unchecked((int)((hash64 >> 32) ^ (hash64 & 0xFFFFFFFFUL)));
        // 
        return hash32;
    }

    /// <summary>Print information</summary>
    public override string ToString() => Constructor?.ToString() ?? "";
}
