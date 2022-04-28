// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using Avalanche.Utilities;

/// <summary>Extension methods for <see cref="IRecordDescription"/>.</summary>
public static class RecordDescriptionExtensions_
{
    /// <summary>Read record info from <paramref name="recordType"/> and write to <paramref name="recordInfo"/>.</summary>
    public static T Read<T>(this T recordInfo, System.Type recordType) where T : IRecordDescription
    {
        // Place member enumerator here
        IEnumerable<MemberInfo> members;
        // Get class members
        if (recordType.IsClass || recordType.IsValueType) members = recordType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        // Get interface members
        else if (recordType.IsInterface) members = recordType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy).Concat(recordType.GetInterfaces().SelectMany(t => t.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)));
        // Unsupported type
        else throw new NotSupportedException($"Type {recordType} is not supported for {nameof(IRecordDescription)} evaluation.");
        // Choose fields and properties
        members = members.Where(mi => mi is System.Reflection.FieldInfo || mi is PropertyInfo);
        // Exclude if IgnoreDataMember
        members = members.Where(mi => mi.GetCustomAttributes(typeof(IgnoreDataMemberAttribute), inherit: true).Count() == 0);
        // Sort by 1. DataMemberAttribute.Order, then 2. occurance
        members = members
                .Select<MemberInfo, (MemberInfo, long)>((MemberInfo mi, int ix) => (mi, ix + ((mi.GetCustomAttribute(typeof(DataMemberAttribute)) as DataMemberAttribute)?.Order ?? int.MaxValue) * 2147483648L))
                .OrderBy(pair => pair.Item2)
                .Select(pair => pair.Item1);
        // To Array
        MemberInfo[] fields = members.ToArray();
        // 
        IFieldDescription[] fieldDescriptions = new IFieldDescription[fields.Length];
        // Cast fields into field infos
        for (int i = 0; i < fieldDescriptions.Length; i++) fieldDescriptions[i] = new FieldDescription().Read(fields[i]).SetRecord(recordInfo);
        // Get annotations
        object[] annotations = recordType.GetCustomAttributes(true) ?? Array.Empty<object>();

        // Assign record type 
        recordInfo.Type = recordType;
        // Assign name
        recordInfo.Name = recordType.AssemblyQualifiedName ?? recordType.FullName ?? recordType.Name;
        // Assign fields
        recordInfo.Fields = fieldDescriptions;
        // Assign annotations
        recordInfo.Annotations = annotations;
        // Return
        return recordInfo;
    }

    /// <summary>Create constructors</summary>
    public static IEnumerable<IConstructorDescription> CreateConstructorDescriptions(this IRecordDescription recordDescription)
    {
        // Get constructors
        IEnumerable<object> constructorInfos = recordDescription.Type.GetConstructors().Where(ci => ci.IsPublic);
        // Add struct no-args constructor
        if (recordDescription.Type.IsValueType)
        {
            // 
            EmitLine line = new EmitLine(OpCodes.Initobj, recordDescription.Type);
            //
            constructorInfos = constructorInfos.Append(line);
        }
        // Create constructor descriptions
        IEnumerable<IConstructorDescription> constructorDescriptions = constructorInfos.Select(ci => new ConstructorDescription().Read(ci).SetRecord(recordDescription).SetType(recordDescription.Type));
        // Return
        return constructorDescriptions;
    }

    /// <summary>Assign <see cref="RecordDescription.Constructors"/>.</summary>
    public static T AssignConstructors<T>(this T recordDescription) where T : IRecordDescription
    {
        // Create constructor descriptions
        IConstructorDescription[] constructorDescriptions = recordDescription.CreateConstructorDescriptions().ToArray();
        // Assign constructors
        recordDescription.Constructors = constructorDescriptions;
        //
        return recordDescription;
    }

    /// <summary>Choose and assign best matching construction where field match parameters. <see cref="RecordDescription.Constructors"/> must have been assigned.</summary>
    public static T ChooseConstruction<T>(this T recordDescription) where T : IRecordDescription
    {
        // Create constructions
        IEnumerable<IConstructionDescription> constructions = recordDescription.Constructors!.CreateConstructions();
        // Choose construction
        IConstructionDescription? bestConstruction = constructions.BestConstruction();
        // No construction
        if (bestConstruction == null)
        {
            // Assign construction strategy
            recordDescription.Construction = null;
        }
        //
        else
        {
            // Assign construction strategy
            recordDescription.Construction = bestConstruction;

            // Get constructor annotations
            object[]? constructorAnnotations = (recordDescription.Construction as IConstructionDescription)?.Constructor?.Annotations;
            // Forward constructor annotations to record
            if (constructorAnnotations != null && constructorAnnotations.Length > 0) recordDescription.Annotations = recordDescription.Annotations.ConcatToArray(constructorAnnotations);

            // Forward parameter annotations to fields
            if (bestConstruction.ParameterToField != null)
            {
                // Evaluate each parameter
                foreach (var pf in bestConstruction.ParameterToField)
                {
                    // No annotations
                    if (pf.Key.Annotations == null || pf.Key.Annotations.Length == 0) continue;
                    // Concat annotations
                    pf.Value.Annotations = pf.Value.Annotations.ConcatToArray(pf.Key.Annotations);
                }
            }

        }

        // Return
        return recordDescription;
    }

    /// <summary>Clone <paramref name="src"/> in writable state.</summary>
    public static RecordDescription Clone(this IRecordDescription src)
    {
        // Create result
        RecordDescription result = new RecordDescription();
        //
        result.Name = src.Name;
        result.Type = src.Type;
        result.Constructors = src.Constructors;
        result.Deconstructor = src.Deconstructor;
        result.SetFields(src.Fields);
        result.SetAnnotations(src.Annotations);
        result.Construction = src.Construction;
        // Return
        return result;
    }

    /// <summary>Calculate hash</summary>
    public static void HashIn(this IRecordDescription recordDescription, ref FNVHash64 hash, bool hashName = true, bool hashType = true, bool hashConstructor = true, bool hashDeconstructor = true, bool hashFields = true, bool hashAnnotations = true)
    {
        // Hash Name
        if (hashName && recordDescription.Name != null) hash.HashIn(recordDescription.Name.GetHashCode());
        //
        if (hashType) hash.HashIn(recordDescription.Type?.FullName);
        // Hash-in constructor
        if (hashConstructor && recordDescription.Constructors != null)
        {
            foreach (var ctor in recordDescription.Constructors)
                hash.HashIn(ctor.GetHashCode());
        }
        // Hash-in deconstructor
        if (hashDeconstructor && recordDescription.Deconstructor != null) hash.HashIn(recordDescription.Deconstructor.GetHashCode());
        //
        if (hashFields && recordDescription.Fields != null)
        {
            foreach (var field in recordDescription.Fields)
            {
                field.HashIn(ref hash);
            }
        }
        // Hash annotations
        if (hashAnnotations && recordDescription.Annotations != null)
        {
            foreach (var annotation in recordDescription.Annotations)
            {
                hash.HashIn(annotation.GetHashCode());
            }
        }
    }

    /// <summary>Calculate hash64</summary>
    public static ulong CalcHash64(this IRecordDescription recordInfo)
    {
        // Init
        FNVHash64 hash = new FNVHash64();
        // Hashin
        recordInfo.HashIn(ref hash);
        // Return
        return hash.Hash;
    }

}
